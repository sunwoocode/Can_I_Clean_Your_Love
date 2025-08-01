using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VacuumSystem : MonoBehaviour       // û�� �ý���
{
    public float suckSpeed = 5f;                // ���� ���� �̵� �ӵ�
    public float shrinkSpeed = 5f;              // ũ�� �پ��� �ӵ�
    public float knockbackForce = 5f;           // ��ֹ� �˹� ��

    [SerializeField] private int counter = 0;   // û�� ����
    public TextMeshProUGUI countingTextUI;        // ������ ī���� �ؽ�Ʈ

    [SerializeField] private Transform gaugeFillTransform; // ������ ���� �̹���
    [SerializeField] private int maxCount = 3;             // �� ������ ���� ���� (������ 100%)

    [SerializeField] private GameObject rewardPauseOverlay;             // ȸ�� ������ �̹���
    [SerializeField] private VacuumController vacuumController;         // �÷��̾� ������ ���� ���
    [SerializeField] private MonoBehaviour[] competitorControllers;     // ���߿� �߰��� �����ڵ�

    void CountingUpdateUI()
    {
        countingTextUI.text = counter.ToString();
    }

    void UpdateGaugeUI()
    {
        float fillAmount = Mathf.Clamp01((float)counter / maxCount);
        Vector3 scale = gaugeFillTransform.localScale;
        scale.x = fillAmount;
        gaugeFillTransform.localScale = scale;

        if (counter >= maxCount)
        {
            EnterRewardPause();
        }
    }

    void EnterRewardPause()
    {
        if (vacuumController != null)
            vacuumController.enabled = false;

        if (competitorControllers != null)
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = false;
            }
        }

        if (rewardPauseOverlay != null)
            rewardPauseOverlay.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D parentRb = transform.parent.GetComponent<Rigidbody2D>();

        // ������ ó��
        if (other.CompareTag("Trash"))
        {
            StartCoroutine(Trash(other.transform)); // ���Ƶ��̴� ���
            counter++;

            CountingUpdateUI();
            UpdateGaugeUI();
        }

        // ��ֹ� ó��
        else if (other.CompareTag("Obstacle"))
        {
            if (parentRb != null)
            {
                parentRb.velocity = Vector2.zero;  // ���� �ӵ� ����
            }

            // currentSpeed�� ����
            VacuumController controller = parentRb.GetComponent<VacuumController>();
            if (controller != null)
            {
                controller.currentSpeed = 0f;
            }

            // �˹鵵 �۵��ϵ���
            if (parentRb != null)
            {
                StartCoroutine(Obstacle(parentRb, other));
            }
        }
    }

    private IEnumerator Trash(Transform trash)      // ������ ����
    {
        Vector3 startScale = trash.localScale;
        Vector3 endScale = Vector3.zero;
        float t = 0f;

        while (trash != null && trash.localScale.magnitude > 0.01f)      // ����� ������
        {
            trash.position = Vector3.MoveTowards(trash.position, transform.position, suckSpeed * Time.deltaTime);
            trash.localScale = Vector3.Lerp(startScale, endScale, t);

            t += Time.deltaTime * shrinkSpeed;
            yield return null;
        }

        if (trash != null)
        {
            Destroy(trash.gameObject); // ������ ������� ����
        }
    }

    private IEnumerator Obstacle(Rigidbody2D parentRb, Collider2D obstacle)
    {
        yield return new WaitForFixedUpdate(); // �ӵ� 0 ���� �� �˹� ó��

        if (parentRb != null)
        {
            Vector2 dir = (parentRb.position - (Vector2)obstacle.transform.position).normalized;
            parentRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse); // �ڿ������� �и�
        }
    }
}
