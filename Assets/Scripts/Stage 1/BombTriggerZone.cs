using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTriggerZone : MonoBehaviour
{
    [SerializeField] private GameObject shadowPrefab;       // 그림자 프리팹
    [SerializeField] private GameObject winePrefab;         // 와인 프리팹
    [SerializeField] private Transform shadowSpawnPoint;    // 그림자 생성 위치 (직접 드래그)

    private bool hasTriggered;                      // true 시 낙하
    public Collider2D tableCollider;                        // 물건 낙하하지 않는 안전 지역
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(SpawnSequence());
        }

        if (!hasTriggered && other.CompareTag("Table"))
        {
            hasTriggered = false;
        }
    }

    private IEnumerator SpawnSequence()
    {
        if (hasTriggered)
        {
            // 1초 대기
            yield return new WaitForSeconds(1f);

            // 그림자 위치 기준 생성
            Vector3 spawnPos = shadowSpawnPoint.position;
            GameObject shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

            // 3초 후 그림자 파괴
            Destroy(shadow, 3f);

            // 와인도 같은 위치에 생성
            GameObject wine = Instantiate(winePrefab, spawnPos, Quaternion.identity);

            // 와인 떨어지는 애니메이션
            StartCoroutine(FallAndShrink(wine));
        }
    }

    private IEnumerator FallAndShrink(GameObject obj)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startScale = obj.transform.localScale;
        Vector3 endScale = new Vector3(0.8f, 0.8f, 1f);  // 작아지는 최종 크기

        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = endScale;
        // 이후 파편 생성하거나 삭제 로직 넣을 수 있음
    }
}