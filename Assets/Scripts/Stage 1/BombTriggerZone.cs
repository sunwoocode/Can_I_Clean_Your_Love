using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTriggerZone : MonoBehaviour
{
    [SerializeField] private GameObject shadowPrefab;       // 그림자 프리팹
    [SerializeField] private GameObject winePrefab;         // 와인 프리팹
    [SerializeField] private Transform shadowSpawnPoint;    // 그림자 생성 위치 (직접 드래그)

    public bool hasTriggered;                      // true 시 낙하
    public Collider2D tableCollider;                // 물건 낙하하지 않는 안전 지역

    [SerializeField] private float spawnInterval = 2f;      // 2초 간격
    private Coroutine spawnLoopCo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.name != "Player") return;      // 판정
        if (hasTriggered) return;                      // 이미 돌고 있으면 무시

        hasTriggered = true;
        if (spawnLoopCo == null) spawnLoopCo = StartCoroutine(SpawnLoop());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.name != "Player") return;   // 메인콜라이더만

        hasTriggered = false;
        if (spawnLoopCo != null) { StopCoroutine(spawnLoopCo); spawnLoopCo = null; }
    }

    private IEnumerator SpawnLoop()
    {
        while (hasTriggered)
        {
            yield return SpawnSequence();
            yield return new WaitForSeconds(spawnInterval);
        }
        spawnLoopCo = null;
    }

    private IEnumerator SpawnSequence()
    {
        // 1초 대기 (원래 로직 유지)
        yield return new WaitForSeconds(1f);

        Vector3 spawnPos = shadowSpawnPoint != null ? shadowSpawnPoint.position : transform.position;

        // 그림자 생성
        GameObject shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

        // 와인 생성
        GameObject wine = Instantiate(winePrefab, spawnPos, Quaternion.identity);

        // 와인 축소 → Intact 꺼질 때 그림자 제거
        yield return StartCoroutine(FallAndShrink(wine, shadow));
    }

    private IEnumerator FallAndShrink(GameObject wineObj, GameObject shadowObj)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startScale = wineObj.transform.localScale;
        Vector3 endScale = new Vector3(0.8f, 0.8f, 1f);

        // 줄어드는 애니메이션
        while (elapsed < duration)
        {
            wineObj.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        wineObj.transform.localScale = endScale;

        // Intact 비활성화
        Transform intact = wineObj.transform.Find("Intact");
        if (intact != null) intact.gameObject.SetActive(false);

        // 그림자 제거
        if (shadowObj != null) Destroy(shadowObj);

        // Broken 켜기 (있으면)
        Transform broken = wineObj.transform.Find("Broken");
        if (broken != null) broken.gameObject.SetActive(true);

        // 여기서 파편 흩뿌리기 로직 호출 가능
    }
}