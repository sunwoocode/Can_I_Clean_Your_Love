using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemSpawner : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject fallingItemPrefab; // (필수) FallingItem.cs가 붙은 와인
    [SerializeField] private GameObject shadowPrefab;      // (선택) 템플릿이 없을 때만 사용

    [Header("BombArea 자식 '그림자 템플릿' (우선 사용)")]
    [SerializeField] private Transform shadowTemplate;     // BombArea 프리팹의 자식(WineShadow 같은 것)
    [SerializeField] private string shadowTemplateName = "WineShadow"; // 비워두면 Find로 자동 결선

    [Header("부모(선택)")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform shadowsParent;

    [Header("자동 스폰 타겟 (플레이어 등)")]
    [SerializeField] private Transform followTarget;       // 비우면 Player 태그로 자동 탐색
    [SerializeField] private string followTag = "Player";
    [SerializeField] private Vector2 areaOffset = Vector2.zero;

    [Header("스폰존(범위) 지정 방법 - 아래 중 '하나'만 세팅")]
    [SerializeField] private GameObject spawnAreaInstance; // 씬 인스턴스(그대로 재사용/위치만 이동)
    [SerializeField] private GameObject spawnAreaPrefab;   // 프리팹(한 번만 Instantiate해서 재사용)
    [SerializeField] private Collider2D spawnAreaTemplate; // 콜라이더 컴포넌트(프리팹/프로젝트 에셋도 OK, 인스턴스 안 만듦)

    [Header("스폰존 표시 옵션")]
    [SerializeField] private bool showSpawnArea = true;    // true면 스폰 중에만 켜기
    [SerializeField] private bool controlActive = true;    // 우리가 active on/off를 제어할지

    [Header("자동 스폰 타이밍")]
    [SerializeField] private float firstDelay = 1f;
    [SerializeField] private float cycleInterval = 5f;     // 다음 좌표 스냅샷까지 대기
    [SerializeField] private int spawnCount = 5;       // 한 사이클 몇 개
    [SerializeField] private float spawnGap = 0.8f;    // 간격

    [Header("기타 옵션")]
    [SerializeField] private float shadowZOffset = 0f;
    [SerializeField] private string intactChildName = "Intact";
    [SerializeField] private string brokenChildName = "Broken";
    [SerializeField] private float shadowFallbackDestroyTime = 1.2f;

    [Header("수동 스폰 전용 옵션")]
    [SerializeField] private bool singleShot = false;      // SpawnFallingItem(pos) 호출을 1회만 허용할 때

    private bool hasSpawned;                               // singleShot 전용 플래그
    private GameObject areaGO;                             // 표시용 스폰존(재사용)
    private Collider2D areaCol;                            // 실제 랜덤 좌표 계산에 쓰는 콜라이더
    private Coroutine loopCo;
    private bool paused;

    // ────────────────────────────────────── Public API (수동 스폰 유지) ──────────────────────────────────────
    public void SpawnFallingItem(Vector2 spawnPos)
    {
        if (singleShot && hasSpawned) return;
        hasSpawned = true;
        SpawnInternal(spawnPos);
    }
    public void ResetSpawner() => hasSpawned = false;

    public void SetPaused(bool p) => paused = p;

    // ────────────────────────────────────── LifeCycle ─────────────────────────────────────────────────────
    private void OnEnable()
    {
        // BombArea 자식 템플릿 자동 결선(비워져 있으면 이름으로 찾음)
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

    // ────────────────────────────────────── Core: 자동 루프 ───────────────────────────────────────────────
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

    // ────────────────────────────────────── Helpers ──────────────────────────────────────────────────────
    private void SpawnInternal(Vector2 spawnPos)
    {
        if (!fallingItemPrefab)
        {
            Debug.LogError("[FallingItemSpawner] fallingItemPrefab이 비어있습니다.");
            return;
        }

        // 1) 그림자(템플릿 우선, 없으면 프리팹 폴백)
        GameObject shadow = null;
        if (shadowTemplate) // BombArea 자식 템플릿 복제
        {
            Vector3 sPos = new Vector3(spawnPos.x, spawnPos.y, shadowZOffset);
            shadow = Instantiate(shadowTemplate.gameObject, sPos, Quaternion.identity, shadowsParent);
            if (!shadow.activeSelf) shadow.SetActive(true); // 템플릿이 비활성이라면 켜줌
        }
        else if (shadowPrefab) // 별도 프리팹 사용
        {
            Vector3 sPos = new Vector3(spawnPos.x, spawnPos.y, shadowZOffset);
            shadow = Instantiate(shadowPrefab, sPos, Quaternion.identity, shadowsParent);
        }

        // 2) 와인(아이템)
        GameObject item = Instantiate(fallingItemPrefab, spawnPos, Quaternion.identity, itemsParent);

        // 3) 깨짐 감지→그림자 정리
        if (shadow && item) StartCoroutine(DestroyShadowWhenBroken(item.transform, shadow));
    }

    private IEnumerator DestroyShadowWhenBroken(Transform itemRoot, GameObject shadow)
    {
        Transform intact = itemRoot.Find(intactChildName);
        Transform broken = itemRoot.Find(brokenChildName);

        float t = 0f;
        while (t < shadowFallbackDestroyTime)
        {
            if (intact && !intact.gameObject.activeInHierarchy) break; // Intact 꺼짐
            if (broken && broken.gameObject.activeInHierarchy) break;  // Broken 켜짐
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

    // 스폰존 준비(인스턴스/프리팹/템플릿 순)
    private void ResolveSpawnArea()
    {
        if (spawnAreaInstance)
        {
            areaGO = spawnAreaInstance;
            areaCol = areaGO.GetComponent<Collider2D>() ?? areaGO.GetComponentInChildren<Collider2D>(true);
            if (!areaCol) Debug.LogWarning("[FallingItemSpawner] spawnAreaInstance에 Collider2D가 필요합니다.");
            if (areaGO && controlActive) areaGO.SetActive(false);
            return;
        }

        if (spawnAreaPrefab)
        {
            areaGO = Instantiate(spawnAreaPrefab, transform);
            areaGO.name = "SpawnArea(Auto)";
            areaCol = areaGO.GetComponent<Collider2D>() ?? areaGO.GetComponentInChildren<Collider2D>(true);
            if (!areaCol) Debug.LogWarning("[FallingItemSpawner] spawnAreaPrefab에 Collider2D가 필요합니다.");
            if (areaGO && controlActive) areaGO.SetActive(false);
            return;
        }

        if (spawnAreaTemplate)
        {
            areaGO = null;            // 표시 안 함
            areaCol = spawnAreaTemplate;
            return;
        }

        Debug.LogError("[FallingItemSpawner] 스폰존이 설정되지 않았습니다. spawnAreaInstance / spawnAreaPrefab / spawnAreaTemplate 중 하나는 필요합니다.");
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