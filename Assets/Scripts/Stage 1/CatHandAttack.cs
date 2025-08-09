using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHandAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject catCollider;      // (기존 그대로, 태그/세팅은 네가 하던 대로)
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CatUIController catUI;

    [SerializeField] private Collider2D[] spawnZones;
    [SerializeField] private float chaseDuration = 7f;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float attackMoveDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 1f;

    [SerializeField] private Transform redGauge;
    [SerializeField] private float redGaugeMaxScale = 5f;
    [SerializeField] private float attackLockTime = 6.9f;

    private float alphaStart = 109f;
    private float alphaEnd = 255f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private Coroutine mainRoutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = catCollider.transform.localScale;

        spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
        spriteRenderer.enabled = false;
        if (redGauge != null) redGauge.localScale = Vector3.zero;
    }

    private void OnEnable() { RestartRoutine(); }
    private void Start() { RestartRoutine(); }

    private void OnDisable()
    {
        if (mainRoutine != null) { StopCoroutine(mainRoutine); mainRoutine = null; }

        spriteRenderer.enabled = false;
        transform.localScale = originalScale;
        if (redGauge != null) redGauge.localScale = Vector3.zero;
        catCollider.SetActive(false);

        if (catUI != null) catUI.OnShadowActiveChanged(false);
    }

    private void RestartRoutine()
    {
        if (mainRoutine != null) StopCoroutine(mainRoutine);
        mainRoutine = StartCoroutine(MainRoutine());
    }

    private IEnumerator MainRoutine()
    {
        yield return new WaitForSeconds(5f); // 첫 등장 지연

        while (true)
        {
            // 등장 위치
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            transform.position = bounds.center;

            // 초기화 & 보이기
            transform.localScale = originalScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
            spriteRenderer.enabled = true;

            // ★ 부드럽게 -645로 이동 (점프 X)
            if (catUI != null) catUI.OnShadowActiveChanged(true);

            // 추격 전 살짝 대기
            yield return new WaitForSeconds(0.5f);

            // 추격
            float chaseTimer = 0f;
            Vector2 attackTarget = Vector2.zero;
            bool targetLocked = false;

            while (chaseTimer < chaseDuration)
            {
                if (!targetLocked && chaseTimer >= attackLockTime)
                {
                    attackTarget = playerTransform.position;
                    targetLocked = true;
                }

                Vector2 targetPos = playerTransform.position;
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * chaseSpeed);

                if (redGauge != null)
                {
                    float t = chaseTimer / chaseDuration;
                    float scale = Mathf.Lerp(0f, redGaugeMaxScale, t);
                    redGauge.localScale = new Vector3(scale, scale, 1f);
                }

                chaseTimer += Time.deltaTime;
                yield return null;
            }

            if (!targetLocked) attackTarget = playerTransform.position;

            // 공격 연출 (작아지고 진하게)
            transform.position = attackTarget;
            transform.localScale = targetScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaEnd / 255f);

            if (redGauge != null) redGauge.localScale = Vector3.zero;

            // 히트박스 오픈(성공 여부는 VacuumController가 처리)
            catCollider.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            catCollider.SetActive(false);

            // ★ 성공 신호가 안 왔다면 실패 처리
            if (catUI != null && !catUI.IsInRoutine)
                catUI.OnAttackFail();

            // 손 스프라이트 페이드 아웃
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // 안전하게 숨김 알림
            if (catUI != null) catUI.OnShadowActiveChanged(false);

            // 리셋 및 다음 대기
            spriteRenderer.enabled = false;
            transform.localScale = originalScale;
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float fromA = Mathf.Clamp01(from / 255f);
        float toA = Mathf.Clamp01(to / 255f);
        float t = 0f, dur = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            var col = spriteRenderer.color;
            col.a = Mathf.Lerp(fromA, toA, t);
            spriteRenderer.color = col;
            yield return null;
        }
        var end = spriteRenderer.color;
        end.a = toA;
        spriteRenderer.color = end;
    }
}