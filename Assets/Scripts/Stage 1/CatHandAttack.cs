using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHandAttack : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject catCollider;      // (���� �״��, �±�/������ �װ� �ϴ� ���)
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
        yield return new WaitForSeconds(5f); // ù ���� ����

        while (true)
        {
            // ���� ��ġ
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            transform.position = bounds.center;

            // �ʱ�ȭ & ���̱�
            transform.localScale = originalScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
            spriteRenderer.enabled = true;

            // �� �ε巴�� -645�� �̵� (���� X)
            if (catUI != null) catUI.OnShadowActiveChanged(true);

            // �߰� �� ��¦ ���
            yield return new WaitForSeconds(0.5f);

            // �߰�
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

            // ���� ���� (�۾����� ���ϰ�)
            transform.position = attackTarget;
            transform.localScale = targetScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaEnd / 255f);

            if (redGauge != null) redGauge.localScale = Vector3.zero;

            // ��Ʈ�ڽ� ����(���� ���δ� VacuumController�� ó��)
            catCollider.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            catCollider.SetActive(false);

            // �� ���� ��ȣ�� �� �Դٸ� ���� ó��
            if (catUI != null && !catUI.IsInRoutine)
                catUI.OnAttackFail();

            // �� ��������Ʈ ���̵� �ƿ�
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // �����ϰ� ���� �˸�
            if (catUI != null) catUI.OnShadowActiveChanged(false);

            // ���� �� ���� ���
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