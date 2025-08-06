using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VacuumSystem : MonoBehaviour       // û�� �ý���
{
    public float suckSpeed = 5f;                // ���� ���� �̵� �ӵ�
    public float shrinkSpeed = 5f;              // ũ�� �پ��� �ӵ�

    [SerializeField] private int counter = 0;   // û�� ����
    public TextMeshProUGUI countingTextUI;        // ������ ī���� �ؽ�Ʈ

    [SerializeField] private Transform gaugeFillTransform; // ������ ���� �̹���
    [SerializeField] private int maxCount = 3;             // �� ������ ���� ���� (������ 100%)

    [SerializeField] private GameObject rewardPauseOverlay;             // ȸ�� ������ �̹���
    [SerializeField] private VacuumController vacuumController;         // �÷��̾� ������ ���� ���
    [SerializeField] private MonoBehaviour[] competitorControllers;     // ���߿� �߰��� �����ڵ�
    [SerializeField] private UpgradeManager upgradeManager;                 // ������ ���� â ��� CS

    void UpdateGaugeUI()
    {
        float fillAmount = Mathf.Clamp01((float)counter / maxCount);
        Vector3 scale = gaugeFillTransform.localScale;
        scale.x = fillAmount;
        gaugeFillTransform.localScale = scale;

        if (counter >= maxCount)
        {
            EnterRewardPause();
            upgradeManager.ShowClassUIList();     // ������ ������ ���
        }
    }

    void EnterRewardPause()
    {
        if (vacuumController != null)           // û�ұ� ��Ʈ�ѷ� ��Ȱ��ȭ
            vacuumController.enabled = false;

        if (competitorControllers != null)      // ������ ������Ʈ ��Ȱ��ȭ
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = false;
            }
        }

        if (rewardPauseOverlay != null)         // ��Ȱ��ȭ ���̾� �̹��� Ȱ��ȭ
            rewardPauseOverlay.SetActive(true);
    }

    public void ExitRewardPause()
    {
        if (vacuumController != null)           // û�ұ� ��Ʈ�ѷ� Ȱ��ȭ
            vacuumController.enabled = true;

        if (competitorControllers != null)      // ������ ������Ʈ Ȱ��ȭ
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = true;
            }
        }

        if (rewardPauseOverlay != null)         // ��Ȱ��ȭ ���̾� �̹��� ��Ȱ��ȭ
            rewardPauseOverlay.SetActive(false);

        counter = 0;
        UpdateGaugeUI();
    }

    void OnTriggerEnter2D(Collider2D other)     // �ٸ� �ݶ��̴��� ����� ��
    {
        Rigidbody2D parentRb = transform.parent.GetComponent<Rigidbody2D>();    // �θ��� Rigidbody2D

        if (other.CompareTag("Trash"))      // ������ û�� ó��
        {
            other.tag = "Untagged";
            StartCoroutine(CleanTrash(other.transform)); // ���Ƶ��̴� ���
        }
    }

    private IEnumerator CleanTrash(Transform trash)      // ������ ����
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
            counter++;

            UpdateGaugeUI();
        }
    }
}
