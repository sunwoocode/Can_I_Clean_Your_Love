using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHandAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject catCollider;      // (�±�/������ �������)
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CatUIController catUI;

    [SerializeField] private Collider2D[] spawnZones;
    [SerializeField] private float chaseDuration = 7f;
    [SerializeField] private float chaseSpeed = 2f;          // �߰� ���� (Lerp ���)
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

    // �Ͻ����� �÷��� (Pause/Resume ����)
    private bool isPaused;


    [SerializeField] private bool allowAttack = false;   // Ʃ�丮��: false�� ����
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

    // �� �ߺ� ����� ����: OnEnable�� ����ΰ� Start������ ����
    private void OnEnable() { /* no-op */ }
    private void Start() { RestartRoutine(); }

    private void OnDisable()
    {
        if (mainRoutine != null) { StopCoroutine(mainRoutine); mainRoutine = null; }

        // ��Ȱ��ȭ �� �ʱ�ȭ(���� ���� ����). ������ �߿��� SetActive�� �ǵ帮�� �ʰ� SetPaused�� ȣ���ϼ���.
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

    // �ܺο��� ȣ��: �Ͻ�����/�簳
    public void SetPaused(bool pause)
    {
        isPaused = pause;
        // �ð��� ����� ������ �ʿ� �� �Ʒ� ���� ���
        // if (spriteRenderer) spriteRenderer.enabled = !pause;
    }

    // Pause�� �����ϴ� ���� (WaitForSeconds ��ü)
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
        // ù ���� ����(�Ͻ����� ����)
        yield return StartCoroutine(PauseAwareDelay(5f));

        while (true)
        {
            // ====== ���� ��ġ ���� (���� �״�� center ���) ======
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            transform.position = bounds.center;

            // ====== �ʱ�ȭ & ���̱� ======
            transform.localScale = originalScale;
            if (spriteRenderer)
            {
                var c = spriteRenderer.color;
                c.a = alphaStart / 255f;
                spriteRenderer.color = c;
                spriteRenderer.enabled = true;
            }

            // UI �˸� (�ε巴�� �ö���� ������ CatUI �ʿ��� ó��)
            if (catUI != null) catUI.OnShadowActiveChanged(true);

            // �߰� �� 0.5�� ���� (�Ͻ����� ����)
            yield return StartCoroutine(PauseAwareDelay(0.5f));

            // ====== �߰� ======
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
                    // ������ �߰�(���� ����): ������ ������ �ƴ����� �ǵ� ����
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

            // ====== ���� ���� (��÷� ���� + ���ϰ� + ���) ======
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

            // ��Ʈ�ڽ� ����(���� ���δ� �ܺΰ� ó��)
            if (catCollider) catCollider.SetActive(true);
            yield return StartCoroutine(PauseAwareDelay(0.3f));
            if (catCollider) catCollider.SetActive(false);

            // ���� �ǵ��(���� ��ȣ ���� ��)
            if (catUI != null && !catUI.IsInRoutine)
                catUI.OnAttackFail();

            // �� ��������Ʈ ���̵� �ƿ�(�Ͻ����� ����)
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // ���� �˸� �� ����
            if (catUI != null) catUI.OnShadowActiveChanged(false);
            if (spriteRenderer) spriteRenderer.enabled = false;
            transform.localScale = originalScale;

            // ���� ����Ŭ���� ��� (�Ͻ����� ����)
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
