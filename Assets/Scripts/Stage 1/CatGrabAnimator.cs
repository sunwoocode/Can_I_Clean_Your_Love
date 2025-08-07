using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHandAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject catCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Collider2D[] spawnZones;
    [SerializeField] private float chaseDuration = 7f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float attackMoveDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 1f;

    [SerializeField] private Transform redGauge; // 추가: 레드게이지 오브젝트
    [SerializeField] private float redGaugeMaxScale = 5f; // 최대 크기


    private float alphaStart = 109f;
    private float alphaEnd = 255f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = catCollider.transform.localScale;

        spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
        spriteRenderer.enabled = false;

        if (redGauge != null)
            redGauge.localScale = Vector3.zero; // 시작 시 0으로
    }

    private void Start()
    {
        StartCoroutine(MainRoutine());
    }

    private IEnumerator MainRoutine()
    {
        yield return new WaitForSeconds(5f); // 게임 시작 후 5초 뒤 첫 등장

        while (true)
        {
            // 등장 위치 설정
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            Vector2 spawnPos = bounds.center;
            transform.position = spawnPos;

            // 초기화
            transform.localScale = originalScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
            spriteRenderer.enabled = true;

            // 추격 전 대기
            yield return new WaitForSeconds(0.5f);

            // 추격
            float chaseTimer = 0f;
            Vector2 attackTarget = Vector2.zero;

            while (chaseTimer < chaseDuration)
            {
                if (chaseTimer >= chaseDuration - 0.8f)
                    attackTarget = playerTransform.position;

                Vector2 targetPos = playerTransform.position;
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * chaseSpeed);

                // RedGauge 커지게 만들기
                if (redGauge != null)
                {
                    float t = chaseTimer / chaseDuration;
                    float scale = Mathf.Lerp(0f, redGaugeMaxScale, t);
                    redGauge.localScale = new Vector3(scale, scale, 1f);
                }

                chaseTimer += Time.deltaTime;
                yield return null;
            }

            // 공격 지점 도착 순간: 갑자기 작아지고 알파값 변경
            transform.position = attackTarget;
            transform.localScale = targetScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaEnd / 255f);

            if (redGauge != null)
                redGauge.localScale = Vector3.zero;

            // 콜라이더 발동
            catCollider.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            catCollider.SetActive(false);

            // 페이드아웃
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // 리셋
            spriteRenderer.enabled = false;
            transform.localScale = originalScale;

            // 다음 등장을 위해 대기
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float timer = 0f;
        Color c = spriteRenderer.color;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(from, to, timer / duration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha / 255f);
            timer += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(c.r, c.g, c.b, to / 255f);
    }
}