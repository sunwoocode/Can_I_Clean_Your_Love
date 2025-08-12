using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerWineSpawner : MonoBehaviour
{
    [Header("Zone detect")]
    [SerializeField] private string zoneTag = "BombZone";
    [SerializeField] private string tableTag = "Table";
    [SerializeField] private bool logState = false;

    [Header("Burst settings")]
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.8f;
    [SerializeField] private float loopCooldown = 5f;

    [Header("Spawn range (drag child CircleCollider2D)")]
    [SerializeField] private CircleCollider2D spawnRange; // ★ 자식 콜라이더 드래그
    [SerializeField] private float spawnZ = 0f;

    [Header("Prefab")]
    [SerializeField] private GameObject winePrefab;

    [Header("Player body collider (Table 감지용)")]
    [SerializeField] private Collider2D playerBodyCollider;
    [SerializeField] private bool includeTriggerTables = true;

    // internal
    private readonly HashSet<Collider2D> zonesInside = new();
    private Coroutine loopCo;
    private bool paused;

    private static readonly Collider2D[] overlapBuffer = new Collider2D[16];
    private ContactFilter2D overlapFilter;

    void Awake()
    {
        // 자동 결선(비어 있으면 자식에서 찾아옴)
        if (!spawnRange) spawnRange = GetComponentInChildren<CircleCollider2D>(true);
        if (spawnRange) spawnRange.isTrigger = true;

        if (!playerBodyCollider)
        {
            foreach (var c in GetComponentsInParent<Collider2D>(true))
            {
                if (c != spawnRange) { playerBodyCollider = c; break; }
            }
        }

        overlapFilter = new ContactFilter2D();
        overlapFilter.useTriggers = includeTriggerTables; // 트리거 테이블도 포함할지
    }

    void OnDisable() => StopLoopIfRunning();

    public void SetPaused(bool pause)
    {
        if (paused == pause) return;
        paused = pause;
        if (!ShouldRun()) StopLoopIfRunning();
        else TryStartLoop();
    }

    // ※ 루트에 Rigidbody2D가 있어야 이 콜백이 여기로 올라옵니다!
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(zoneTag)) return;
        zonesInside.Add(other);
        TryStartLoop();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(zoneTag)) return;
        zonesInside.Remove(other);
        if (!ShouldRun()) StopLoopIfRunning();
    }

    private void TryStartLoop()
    {
        if (loopCo == null && isActiveAndEnabled && ShouldRun())
        {
            if (logState) Debug.Log("[PlayerWineSpawner] start");
            loopCo = StartCoroutine(BurstLoop());
        }
    }

    private void StopLoopIfRunning()
    {
        if (loopCo != null)
        {
            if (logState) Debug.Log("[PlayerWineSpawner] stop");
            StopCoroutine(loopCo);
            loopCo = null;
        }
    }

    private IEnumerator BurstLoop()
    {
        while (ShouldRun())
        {
            var points = SamplePoints(burstCount);
            for (int i = 0; i < points.Count; i++)
            {
                if (!ShouldRun()) { StopLoopIfRunning(); yield break; }
                Instantiate(winePrefab, points[i], Quaternion.identity);
                if (i < points.Count - 1) yield return WaitOrAbort(burstInterval);
            }
            yield return WaitOrAbort(loopCooldown);
        }
        StopLoopIfRunning();
    }

    private IEnumerator WaitOrAbort(float sec)
    {
        float t = 0f;
        while (t < sec)
        {
            if (!ShouldRun()) { StopLoopIfRunning(); yield break; }
            t += Time.deltaTime; yield return null;
        }
    }

    private bool ShouldRun() => zonesInside.Count > 0 && !paused && !IsPlayerBodyTouchingTable();

    private bool IsPlayerBodyTouchingTable()
    {
        if (!playerBodyCollider) return false;
        int hit = playerBodyCollider.OverlapCollider(overlapFilter, overlapBuffer);
        for (int i = 0; i < hit; i++) if (overlapBuffer[i].CompareTag(tableTag)) return true;
        return false;
    }

    private List<Vector3> SamplePoints(int count)
    {
        var list = new List<Vector3>(count);
        if (!spawnRange) return list;

        Vector3 center = spawnRange.transform.TransformPoint(spawnRange.offset);
        float scale = Mathf.Max(Mathf.Abs(spawnRange.transform.lossyScale.x),
                                Mathf.Abs(spawnRange.transform.lossyScale.y));
        float r = spawnRange.radius * scale;

        for (int i = 0; i < count; i++)
        {
            Vector2 delta = Random.insideUnitCircle * r;
            list.Add(new Vector3(center.x + delta.x, center.y + delta.y, spawnZ));
        }
        return list;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!spawnRange) return;
        Gizmos.color = Color.cyan;
        float scale = Mathf.Max(Mathf.Abs(spawnRange.transform.lossyScale.x),
                                Mathf.Abs(spawnRange.transform.lossyScale.y));
        UnityEditor.Handles.DrawWireDisc(
            spawnRange.transform.TransformPoint(spawnRange.offset),
            Vector3.forward,
            spawnRange.radius * scale
        );
    }
#endif
}
