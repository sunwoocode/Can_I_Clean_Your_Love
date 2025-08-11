using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyManager : MonoBehaviour
{
    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private float moveDuration = 1.5f;

    private int currentTargetIndex = 0;

    public PlayerHP playerHP;
    public Sturn sturn;

    private SpriteRenderer spriteRenderer;

    private bool isPaused = false;

    public List<GameObject> movingPattern = new List<GameObject>();     // �̵� ���� ǥ�� ������Ʈ
    private int movingPatternCounter = 0;
    [SerializeField] private float previewTime = 0.6f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ���� �� ��� ���� ����
        for (int i = 0; i < movingPattern.Count; i++)
            if (movingPattern[i]) movingPattern[i].SetActive(false);

        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (isPaused) { yield return null; continue; }

            // ���� ����(from)�� ���� ����(to)
            int fromIdx = currentTargetIndex;
            int toIdx = (currentTargetIndex + 1) % positions.Count;

            // 1) ���� ���� ������ �ѱ� (�̵� �� + �̵� �� ���� �ѵ�)
            ShowSegment(fromIdx, true);

            // 2) ������ ���(0.5��). �Ͻ����� �ÿ��� Ÿ�̸� ���� ����, ������� ��� ���� ���� ����
            float p = 0f;
            while (p < previewTime)
            {
                if (isPaused) { yield return null; continue; }
                p += Time.deltaTime;
                yield return null;
            }

            // 3) ���� �̵� (������� ��� On ����)
            Vector3 startPos = transform.position;
            Vector3 targetPos = positions[toIdx];

            float t = 0f;
            Vector3 direction = (targetPos - startPos).normalized;
            UpdateSpriteDirection(direction);

            while (t < moveDuration)
            {
                if (isPaused) { yield return null; continue; }
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, t / moveDuration);
                yield return null;
            }

            // 4) ����: ��ġ ���� + �̹� ���� ������ ����
            transform.position = targetPos;
            ShowSegment(fromIdx, false);

            // 5) ���� ����Ŭ��
            currentTargetIndex = toIdx;
        }
    }

    // ������ ���׸�Ʈ On/Off (�ε��� ���� ó��)
    void ShowSegment(int segIdx, bool on)
    {
        // ���� ��ȣ
        if (movingPattern == null || movingPattern.Count == 0) return;

        // ���׸�Ʈ ������ positions ����:
        // ��ȯ �̵��̸� "fromIdx ����"���� ���׸�Ʈ ������ positions.Count�� �����ϰ� �δ� �� ���.
        segIdx = Mathf.Clamp(segIdx, 0, movingPattern.Count - 1);

        // �ϳ��� �Ѱ� �������� ����(���ü� ����)
        for (int i = 0; i < movingPattern.Count; i++)
        {
            var go = movingPattern[i];
            if (!go) continue;
            go.SetActive(on && i == segIdx);
        }
    }

    private void UpdateSpriteDirection(Vector3 dir)
    {
        if (spriteRenderer == null) return;

        // ���� ū �� �������� ���� �Ǵ�
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // �¿�
            spriteRenderer.flipX = dir.x < 0;
            spriteRenderer.flipY = false;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            // ����
            spriteRenderer.flipX = false;
            if (dir.y > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f); // ��
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f); // �Ʒ�
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("������ ����!");

            playerHP.HeartCounter();
            sturn.SturnEffect();
        }
    }

    public void PauseMove()
    {
        isPaused = true;
    }

    public void ResumeMove()
    {
        isPaused = false;
    }
}
