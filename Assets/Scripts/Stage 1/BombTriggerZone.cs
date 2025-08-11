using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTriggerZone : MonoBehaviour
{
    [SerializeField] private GameObject shadowPrefab;       // 그림자 프리팹
    [SerializeField] private GameObject winePrefab;         // 와인 프리팹
    [SerializeField] private Transform shadowSpawnPoint;    // 그림자 생성 위치 (직접 드래그)

    public bool hasTriggered;                // true면 스폰 루프 동작(플레이어가 존 안에 있을 때)
    public Collider2D tableCollider;         // 안전 지역 (미사용이면 그대로 둠)

    [SerializeField] private float spawnInterval = 2f; // 스폰 간격
    private Coroutine spawnLoopCo;

    // 레벨업 일시정지 플래그(코루틴/타이머가 멈추고, 재개 시 이어서 진행)
    private bool isPaused = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.name != "Player") return;
        if (hasTriggered) return;

        hasTriggered = true;
        if (spawnLoopCo == null) spawnLoopCo = StartCoroutine(SpawnLoop());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.name != "Player") return;

        hasTriggered = false;
        if (spawnLoopCo != null) { StopCoroutine(spawnLoopCo); spawnLoopCo = null; }
    }

    // 외부에서 호출(UpgradeManager): 레벨업 중 일시정지/재개
    public void SetPaused(bool pause)
    {
        isPaused = pause;
        // 필요하면 여기서 시각적 처리(예: 그림자 알파 낮추기) 추가 가능
    }

    private IEnumerator SpawnLoop()
    {
        while (hasTriggered)
        {
            yield return SpawnSequence();

            // spawnInterval 동안 대기 (일시정지 중에는 시간 진행 정지)
            yield return PauseAwareDelay(spawnInterval);
        }
        spawnLoopCo = null;
    }

    private IEnumerator SpawnSequence()
    {
        // 연출을 위한 1초 대기 (일시정지 인지)
        yield return PauseAwareDelay(1f);

        Vector3 spawnPos = shadowSpawnPoint != null ? shadowSpawnPoint.position : transform.position;

        // 그림자 생성
        GameObject shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

        // 와인 생성
        GameObject wine = Instantiate(winePrefab, spawnPos, Quaternion.identity);

        // 와인 축소 → Intact 꺼질 때 그림자 제거 → Broken 켜기
        yield return FallAndShrink(wine, shadow);
    }

    private IEnumerator FallAndShrink(GameObject wineObj, GameObject shadowObj)
    {
        float duration = 1f;
        float elapsed = 0f;

        Vector3 startScale = wineObj.transform.localScale;
        Vector3 endScale = new Vector3(0.8f, 0.8f, 1f);

        // 줄어드는 애니메이션 (일시정지 인지)
        while (elapsed < duration)
        {
            if (!isPaused)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                wineObj.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
        wineObj.transform.localScale = endScale;

        // Intact 비활성화
        Transform intact = wineObj.transform.Find("Intact");
        if (intact != null) intact.gameObject.SetActive(false);

        // 그림자 제거
        if (shadowObj != null) Destroy(shadowObj);

        // Broken 활성화
        Transform broken = wineObj.transform.Find("Broken");
        if (broken != null) broken.gameObject.SetActive(true);

        // 필요하면 여기서 파편 코루틴 호출(일시정지 체크만 동일하게 적용)
        // e.g., StartCoroutine(ShardRoutine(broken));
    }

    // WaitForSeconds 대체: 일시정지 동안에는 시간이 흐르지 않음
    private IEnumerator PauseAwareDelay(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (!isPaused) t += Time.deltaTime;
            yield return null;
        }
    }
}
