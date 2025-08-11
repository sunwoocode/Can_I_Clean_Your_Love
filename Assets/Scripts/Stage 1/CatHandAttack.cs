using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHandAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject catCollider;      // (태그/세팅은 기존대로)
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CatUIController catUI;

    [SerializeField] private Collider2D[] spawnZones;
    [SerializeField] private float chaseDuration = 7f;
    [SerializeField] private float chaseSpeed = 2f;          // 추격 강도 (Lerp 계수)
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

    // 일시정지 플래그 (Pause/Resume 전용)
    private bool isPaused;


    [SerializeField] private bool allowAttack = false;   // 튜토리얼: false로 시작
    public void EnableAttack() => allowAttack = true;
    public void DisableAttack() => allowAttack = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = catCollider ? catCollider.transform.localScale : Vector3.one;

        if (spriteRenderer)
        {
            var c = spriteRenderer.color;
            c.a = alphaStart / 255f;
            spriteRenderer.color = c;
            spriteRenderer.enabled = false;
        }
        if (redGauge) redGauge.localScale = Vector3.zero;
        if (catCollider) catCollider.SetActive(false);
    }

    // ★ 중복 재시작 방지: OnEnable은 비워두고 Start에서만 시작
    private void OnEnable() { /* no-op */ }
    private void Start() { RestartRoutine(); }

    private void OnDisable()
    {
        if (mainRoutine != null) { StopCoroutine(mainRoutine); mainRoutine = null; }

        // 비활성화 시 초기화(기존 동작 유지). 레벨업 중에는 SetActive를 건드리지 않고 SetPaused만 호출하세요.
        if (spriteRenderer) { spriteRenderer.enabled = false; }
        transform.localScale = originalScale;
        if (redGauge) redGauge.localScale = Vector3.zero;
        if (catCollider) catCollider.SetActive(false);
        if (catUI) catUI.OnShadowActiveChanged(false);
    }

    private void RestartRoutine()
    {
        if (mainRoutine != null) StopCoroutine(mainRoutine);
        mainRoutine = StartCoroutine(MainRoutine());
    }

    // 외부에서 호출: 일시정지/재개
    public void SetPaused(bool pause)
    {
        isPaused = pause;
        // 시각만 숨기고 싶으면 필요 시 아래 라인 사용
        // if (spriteRenderer) spriteRenderer.enabled = !pause;
    }

    // Pause를 인지하는 지연 (WaitForSeconds 대체)
    private IEnumerator PauseAwareDelay(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (!isPaused) t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MainRoutine()
    {
        // 첫 등장 지연(일시정지 인지)
        yield return StartCoroutine(PauseAwareDelay(5f));

        while (true)
        {
            // ====== 등장 위치 설정 (기존 그대로 center 사용) ======
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            transform.position = bounds.center;

            // ====== 초기화 & 보이기 ======
            transform.localScale = originalScale;
            if (spriteRenderer)
            {
                var c = spriteRenderer.color;
                c.a = alphaStart / 255f;
                spriteRenderer.color = c;
                spriteRenderer.enabled = true;
            }

            // UI 알림 (부드럽게 올라오는 연출은 CatUI 쪽에서 처리)
            if (catUI != null) catUI.OnShadowActiveChanged(true);

            // 추격 전 0.5초 예고 (일시정지 인지)
            yield return StartCoroutine(PauseAwareDelay(0.5f));

            // ====== 추격 ======
            float chaseTimer = 0f;
            Vector2 attackTarget = Vector2.zero;
            bool targetLocked = false;

            while (chaseTimer < chaseDuration)
            {
                if (!isPaused)
                {
                    if (!targetLocked && chaseTimer >= attackLockTime)
                    {
                        attackTarget = playerTransform ? (Vector2)playerTransform.position : (Vector2)transform.position;
                        targetLocked = true;
                    }

                    Vector2 targetPos = playerTransform ? (Vector2)playerTransform.position : (Vector2)transform.position;
                    // 감쇠형 추격(기존 유지): 프레임 독립은 아니지만 의도 보존
                    transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * chaseSpeed);

                    if (redGauge)
                    {
                        float t = chaseTimer / chaseDuration;
                        float scale = Mathf.Lerp(0f, redGaugeMaxScale, t);
                        redGauge.localScale = new Vector3(scale, scale, 1f);
                    }

                    chaseTimer += Time.deltaTime;
                }
                yield return null;
            }

            if (!targetLocked)
                attackTarget = playerTransform ? (Vector2)playerTransform.position : (Vector2)transform.position;

            // ====== 공격 연출 (대시로 접근 + 진하게 + 축소) ======
            if (redGauge) redGauge.localScale = Vector3.zero;

            Vector2 dashStart = transform.position;
            float dashT = 0f;
            while (dashT < 1f)
            {
                if (!isPaused)
                {
                    dashT += Time.deltaTime / Mathf.Max(0.0001f, attackMoveDuration);
                    float e = dashT * dashT; // Ease-in
                    transform.position = Vector2.LerpUnclamped(dashStart, attackTarget, e);

                    if (spriteRenderer)
                    {
                        var c = spriteRenderer.color;
                        c.a = Mathf.Lerp(alphaStart / 255f, alphaEnd / 255f, dashT);
                        spriteRenderer.color = c;
                    }
                    transform.localScale = Vector3.Lerp(originalScale, targetScale, dashT);
                }
                yield return null;
            }

            // 히트박스 오픈(성공 여부는 외부가 처리)
            if (catCollider) catCollider.SetActive(true);
            yield return StartCoroutine(PauseAwareDelay(0.3f));
            if (catCollider) catCollider.SetActive(false);

            // 실패 피드백(성공 신호 없을 때)
            if (catUI != null && !catUI.IsInRoutine)
                catUI.OnAttackFail();

            // 손 스프라이트 페이드 아웃(일시정지 인지)
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // 숨김 알림 및 리셋
            if (catUI != null) catUI.OnShadowActiveChanged(false);
            if (spriteRenderer) spriteRenderer.enabled = false;
            transform.localScale = originalScale;

            // 다음 사이클까지 대기 (일시정지 인지)
            yield return StartCoroutine(PauseAwareDelay(5f));
        }
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (!spriteRenderer) yield break;

        float fromA = Mathf.Clamp01(from / 255f);
        float toA = Mathf.Clamp01(to / 255f);
        float t = 0f;
        float dur = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            if (!isPaused) t += Time.deltaTime / dur;
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
