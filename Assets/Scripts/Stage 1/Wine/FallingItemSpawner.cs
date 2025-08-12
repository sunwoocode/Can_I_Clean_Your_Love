using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemSpawner : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private GameObject fallingItemPrefab; // (�ʼ�) FallingItem.cs�� ���� ����
    [SerializeField] private GameObject shadowPrefab;      // (����) ���ø��� ���� ���� ���

    [Header("BombArea �ڽ� '�׸��� ���ø�' (�켱 ���)")]
    [SerializeField] private Transform shadowTemplate;     // BombArea �������� �ڽ�(WineShadow ���� ��)
    [SerializeField] private string shadowTemplateName = "WineShadow"; // ����θ� Find�� �ڵ� �ἱ

    [Header("�θ�(����)")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform shadowsParent;

    [Header("�ڵ� ���� Ÿ�� (�÷��̾� ��)")]
    [SerializeField] private Transform followTarget;       // ���� Player �±׷� �ڵ� Ž��
    [SerializeField] private string followTag = "Player";
    [SerializeField] private Vector2 areaOffset = Vector2.zero;

    [Header("������(����) ���� ��� - �Ʒ� �� '�ϳ�'�� ����")]
    [SerializeField] private GameObject spawnAreaInstance; // �� �ν��Ͻ�(�״�� ����/��ġ�� �̵�)
    [SerializeField] private GameObject spawnAreaPrefab;   // ������(�� ���� Instantiate�ؼ� ����)
    [SerializeField] private Collider2D spawnAreaTemplate; // �ݶ��̴� ������Ʈ(������/������Ʈ ���µ� OK, �ν��Ͻ� �� ����)

    [Header("������ ǥ�� �ɼ�")]
    [SerializeField] private bool showSpawnArea = true;    // true�� ���� �߿��� �ѱ�
    [SerializeField] private bool controlActive = true;    // �츮�� active on/off�� ��������

    [Header("�ڵ� ���� Ÿ�̹�")]
    [SerializeField] private float firstDelay = 1f;
    [SerializeField] private float cycleInterval = 5f;     // ���� ��ǥ ���������� ���
    [SerializeField] private int spawnCount = 5;       // �� ����Ŭ �� ��
    [SerializeField] private float spawnGap = 0.8f;    // ����

    [Header("��Ÿ �ɼ�")]
    [SerializeField] private float shadowZOffset = 0f;
    [SerializeField] private string intactChildName = "Intact";
    [SerializeField] private string brokenChildName = "Broken";
    [SerializeField] private float shadowFallbackDestroyTime = 1.2f;

    [Header("���� ���� ���� �ɼ�")]
    [SerializeField] private bool singleShot = false;      // SpawnFallingItem(pos) ȣ���� 1ȸ�� ����� ��

    private bool hasSpawned;                               // singleShot ���� �÷���
    private GameObject areaGO;                             // ǥ�ÿ� ������(����)
    private Collider2D areaCol;                            // ���� ���� ��ǥ ��꿡 ���� �ݶ��̴�
    private Coroutine loopCo;
    private bool paused;

    // ���������������������������������������������������������������������������� Public API (���� ���� ����) ����������������������������������������������������������������������������
    public void SpawnFallingItem(Vector2 spawnPos)
    {
        if (singleShot && hasSpawned) return;
        hasSpawned = true;
        SpawnInternal(spawnPos);
    }
    public void ResetSpawner() => hasSpawned = false;

    public void SetPaused(bool p) => paused = p;

    // ���������������������������������������������������������������������������� LifeCycle ����������������������������������������������������������������������������������������������������������
    private void OnEnable()
    {
        // BombArea �ڽ� ���ø� �ڵ� �ἱ(����� ������ �̸����� ã��)
        if (!shadowTemplate && !string.IsNullOrEmpty(shadowTemplateName))
        {
            var t = transform.Find(shadowTemplateName);
            if (t) shadowTemplate = t;
        }

        ResolveFollowTarget();
        ResolveSpawnArea();
        if (loopCo == null) loopCo = StartCoroutine(AutoLoop());
    }

    private void OnDisable()
    {
        if (loopCo != null) { StopCoroutine(loopCo); loopCo = null; }
        if (areaGO && controlActive) areaGO.SetActive(false);
    }

    // ���������������������������������������������������������������������������� Core: �ڵ� ���� ����������������������������������������������������������������������������������������������
    private IEnumerator AutoLoop()
    {
        if (firstDelay > 0f) yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            while (paused) yield return null;

            Vector3 basePos = followTarget ? (Vector3)followTarget.position : transform.position;
            basePos += (Vector3)areaOffset;
            MoveSpawnAreaTo(basePos);

            for (int i = 0; i < spawnCount; i++)
            {
                while (paused) yield return null;

                if (TryGetRandomPointInArea(basePos, out Vector2 pos))
                    SpawnInternal(pos);

                if (i < spawnCount - 1 && spawnGap > 0f)
                    yield return new WaitForSeconds(spawnGap);
            }

            if (areaGO && controlActive) areaGO.SetActive(false);

            if (cycleInterval > 0f) yield return new WaitForSeconds(cycleInterval);
            else yield return null;
        }
    }

    // ���������������������������������������������������������������������������� Helpers ������������������������������������������������������������������������������������������������������������
    private void SpawnInternal(Vector2 spawnPos)
    {
        if (!fallingItemPrefab)
        {
            Debug.LogError("[FallingItemSpawner] fallingItemPrefab�� ����ֽ��ϴ�.");
            return;
        }

        // 1) �׸���(���ø� �켱, ������ ������ ����)
        GameObject shadow = null;
        if (shadowTemplate) // BombArea �ڽ� ���ø� ����
        {
            Vector3 sPos = new Vector3(spawnPos.x, spawnPos.y, shadowZOffset);
            shadow = Instantiate(shadowTemplate.gameObject, sPos, Quaternion.identity, shadowsParent);
            if (!shadow.activeSelf) shadow.SetActive(true); // ���ø��� ��Ȱ���̶�� ����
        }
        else if (shadowPrefab) // ���� ������ ���
        {
            Vector3 sPos = new Vector3(spawnPos.x, spawnPos.y, shadowZOffset);
            shadow = Instantiate(shadowPrefab, sPos, Quaternion.identity, shadowsParent);
        }

        // 2) ����(������)
        GameObject item = Instantiate(fallingItemPrefab, spawnPos, Quaternion.identity, itemsParent);

        // 3) ���� ������׸��� ����
        if (shadow && item) StartCoroutine(DestroyShadowWhenBroken(item.transform, shadow));
    }

    private IEnumerator DestroyShadowWhenBroken(Transform itemRoot, GameObject shadow)
    {
        Transform intact = itemRoot.Find(intactChildName);
        Transform broken = itemRoot.Find(brokenChildName);

        float t = 0f;
        while (t < shadowFallbackDestroyTime)
        {
            if (intact && !intact.gameObject.activeInHierarchy) break; // Intact ����
            if (broken && broken.gameObject.activeInHierarchy) break;  // Broken ����
            t += Time.deltaTime;
            yield return null;
        }
        if (shadow) Destroy(shadow);
    }

    private void ResolveFollowTarget()
    {
        if (followTarget) return;
        var p = GameObject.FindGameObjectWithTag(followTag);
        if (p) followTarget = p.transform;
    }

    // ������ �غ�(�ν��Ͻ�/������/���ø� ��)
    private void ResolveSpawnArea()
    {
        if (spawnAreaInstance)
        {
            areaGO = spawnAreaInstance;
            areaCol = areaGO.GetComponent<Collider2D>() ?? areaGO.GetComponentInChildren<Collider2D>(true);
            if (!areaCol) Debug.LogWarning("[FallingItemSpawner] spawnAreaInstance�� Collider2D�� �ʿ��մϴ�.");
            if (areaGO && controlActive) areaGO.SetActive(false);
            return;
        }

        if (spawnAreaPrefab)
        {
            areaGO = Instantiate(spawnAreaPrefab, transform);
            areaGO.name = "SpawnArea(Auto)";
            areaCol = areaGO.GetComponent<Collider2D>() ?? areaGO.GetComponentInChildren<Collider2D>(true);
            if (!areaCol) Debug.LogWarning("[FallingItemSpawner] spawnAreaPrefab�� Collider2D�� �ʿ��մϴ�.");
            if (areaGO && controlActive) areaGO.SetActive(false);
            return;
        }

        if (spawnAreaTemplate)
        {
            areaGO = null;            // ǥ�� �� ��
            areaCol = spawnAreaTemplate;
            return;
        }

        Debug.LogError("[FallingItemSpawner] �������� �������� �ʾҽ��ϴ�. spawnAreaInstance / spawnAreaPrefab / spawnAreaTemplate �� �ϳ��� �ʿ��մϴ�.");
    }

    private void MoveSpawnAreaTo(Vector3 worldPos)
    {
        if (areaGO)
        {
            areaGO.transform.position = worldPos;
            if (showSpawnArea && controlActive) areaGO.SetActive(true);
        }
    }

    private bool TryGetRandomPointInArea(Vector3 centerWorld, out Vector2 pos)
    {
        pos = centerWorld;

        if (areaCol)
        {
            if (areaCol is BoxCollider2D b)
            {
                Vector2 size = b.size;
                Vector2 center = b.offset;
                float rx = Random.Range(-size.x * 0.5f, size.x * 0.5f);
                float ry = Random.Range(-size.y * 0.5f, size.y * 0.5f);
                Vector3 local = new Vector3(center.x + rx, center.y + ry, 0f);
                pos = b.transform.TransformPoint(local);
                return true;
            }
            if (areaCol is CircleCollider2D c)
            {
                float r = c.radius;
                Vector2 center = c.offset;
                float th = Random.Range(0f, Mathf.PI * 2f);
                float rd = Mathf.Sqrt(Random.value) * r;
                Vector3 local = new Vector3(center.x + Mathf.Cos(th) * rd,
                                            center.y + Mathf.Sin(th) * rd, 0f);
                pos = c.transform.TransformPoint(local);
                return true;
            }

            Bounds bounds = areaCol.bounds;
            for (int i = 0; i < 20; i++)
            {
                var p = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
                                    Random.Range(bounds.min.y, bounds.max.y));
                if (areaCol.OverlapPoint(p)) { pos = p; return true; }
            }
            return false;
        }

        if (spawnAreaTemplate is BoxCollider2D tb)
        {
            Vector2 size = tb.size;
            Vector2 off = tb.offset;
            float rx = Random.Range(-size.x * 0.5f, size.x * 0.5f);
            float ry = Random.Range(-size.y * 0.5f, size.y * 0.5f);
            pos = (Vector2)(centerWorld + new Vector3(off.x + rx, off.y + ry, 0f));
            return true;
        }
        if (spawnAreaTemplate is CircleCollider2D tc)
        {
            float r = tc.radius;
            Vector2 off = tc.offset;
            float th = Random.Range(0f, Mathf.PI * 2f);
            float rd = Mathf.Sqrt(Random.value) * r;
            pos = (Vector2)(centerWorld + new Vector3(off.x + Mathf.Cos(th) * rd,
                                                      off.y + Mathf.Sin(th) * rd, 0f));
            return true;
        }
        return false;
    }
}