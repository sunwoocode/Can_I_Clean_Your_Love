using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 수정 안하시는걸 추천함 
public class FallingItem : MonoBehaviour
{
    [Header("오브젝트")]
    [SerializeField] private GameObject intactObj;
    [SerializeField] private GameObject brokenObj;

    [Header("파편 목표 위치 (로컬 기준)")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();

    [Header("파편 목표 각도 (Z축 회전)")]
    [SerializeField] private List<float> targetRotations = new List<float>();

    [Header("정렬(Sorting) 참조(자동 할당)")]
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private Collider2D playerCollider;

    [SerializeField] private int belowOffset = -2;   // 밖이면 player-2

    private Transform[] shards;
    private bool scatterFinished = false;
    private HashSet<Transform> locked = new HashSet<Transform>();
    private Dictionary<Transform, int> lockedOrder = new Dictionary<Transform, int>();

    public PlayerHP playerHP;
    public Sturn sturn;

    void Awake()
    {
        if (playerSR == null || playerCollider == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                if (playerSR == null) playerSR = p.GetComponentInChildren<SpriteRenderer>();
                if (playerCollider == null) playerCollider = p.GetComponentInChildren<Collider2D>();
            }
        }
    }

    void Start() => StartCoroutine(FallRoutine());

    IEnumerator FallRoutine()
    {
        // 1초 동안 intact만 서서히 줄어듦
        float duration = 1f, elapsed = 0f;
        Vector3 initialScale = intactObj.transform.localScale;
        Vector3 targetScale = initialScale * 0.8f;

        while (elapsed < duration)
        {
            if (intactObj) intactObj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (intactObj) intactObj.transform.localScale = targetScale;

        // 파편으로 교체
        if (intactObj) intactObj.SetActive(false);
        if (brokenObj)
        {
            brokenObj.SetActive(true);

            // ★ 퍼지기 전엔 절대 못 먹게 전부 Untagged
            SetCollectibleTag(brokenObj.transform, false);

            yield return StartCoroutine(MoveShards(brokenObj.transform));

            // ★ 퍼지기 완료 → 이제부터 먹을 수 있게 Trash로 전환
            SetCollectibleTag(brokenObj.transform, true);

            // 초기 정렬 확정
            InitialFinalize(brokenObj.transform);
            scatterFinished = true;
        }
    }

    IEnumerator MoveShards(Transform broken)
    {
        var collectibles = broken.GetComponentsInChildren<ShardCollectible>(true);
        int count = Mathf.Min(targetPositions.Count, targetRotations.Count, collectibles.Length);

        shards = new Transform[count];
        Vector3[] startPos = new Vector3[count];
        Quaternion[] startRot = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            shards[i] = collectibles[i].transform;
            startPos[i] = shards[i].localPosition;
            startRot[i] = shards[i].localRotation;
        }

        float duration = 1f, elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < count; i++)
            {
                if (!shards[i]) continue;
                shards[i].localPosition = Vector3.Lerp(startPos[i], targetPositions[i], t);
                shards[i].localRotation = Quaternion.Lerp(startRot[i], Quaternion.Euler(0, 0, targetRotations[i]), t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            if (!shards[i]) continue;
            shards[i].localPosition = targetPositions[i];
            shards[i].localRotation = Quaternion.Euler(0, 0, targetRotations[i]);
        }
    }

    // ★ 파편/그룹 태그 일괄 스위치 (enable=true → "Trash", false → "Untagged")
    void SetCollectibleTag(Transform broken, bool enable)
    {
        string tagName = enable ? "Trash" : "Untagged";

        // 파편(Shard_*)들
        var parts = broken.GetComponentsInChildren<ShardCollectible>(true);
        foreach (var p in parts) if (p) p.gameObject.tag = tagName;

        // 그룹 오브젝트(GroupA/GroupB)도 맞춰두면 Vacuum에서 다루기 편함
        var groups = broken.GetComponentsInChildren<ShardGroup>(true);
        foreach (var g in groups) if (g) g.gameObject.tag = tagName;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("고양이가 얍!");

            playerHP.HeartCounter();
            sturn.SturnEffect();
        }
    }

    void InitialFinalize(Transform broken)
    {
        if (playerSR == null || playerCollider == null || shards == null) return;
        int baseOrder = playerSR.sortingOrder;

        for (int i = 0; i < shards.Length; i++)
        {
            var tr = shards[i];
            if (!tr) continue;
            var sr = tr.GetComponent<SpriteRenderer>();
            if (!sr) continue;

            bool inside = sr.bounds.Intersects(playerCollider.bounds);
            if (!inside)
            {
                int targetOrder = baseOrder + belowOffset;
                sr.sortingOrder = targetOrder;
                locked.Add(tr);
                lockedOrder[tr] = targetOrder;
            }
        }
    }

    void LateUpdate()
    {
        if (!scatterFinished || playerSR == null || playerCollider == null || shards == null) return;

        foreach (var tr in shards)
        {
            if (!tr) continue;
            if (locked.Contains(tr))
            {
                var srL = tr.GetComponent<SpriteRenderer>();
                if (srL && lockedOrder.TryGetValue(tr, out int ord) && srL.sortingOrder != ord)
                    srL.sortingOrder = ord;
                continue;
            }

            var sr = tr.GetComponent<SpriteRenderer>();
            if (!sr) continue;

            bool insideNow = sr.bounds.Intersects(playerCollider.bounds);
            if (!insideNow)
            {
                int targetOrder = playerSR.sortingOrder + belowOffset;
                sr.sortingOrder = targetOrder;
                locked.Add(tr);
                lockedOrder[tr] = targetOrder;
            }
        }
    }
}
