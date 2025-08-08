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
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float attackMoveDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 1f;

    [SerializeField] private Transform redGauge; // �߰�: ��������� ������Ʈ
    [SerializeField] private float redGaugeMaxScale = 5f; // �ִ� ũ��
    [SerializeField] private float attackLockTime = 6.9f; // �� �ʿ� ���� ��ǥ �����

    private float alphaStart = 109f;
    private float alphaEnd = 255f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private Coroutine mainRoutine; // ���� ��ƾ ����

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = catCollider.transform.localScale;

        spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
        spriteRenderer.enabled = false;

        if (redGauge != null)
            redGauge.localScale = Vector3.zero; // ���� �� 0����
    }

    private void OnEnable()
    {
        // �ٽ� ������ �� ��ƾ �����
        RestartRoutine();
    }

    private void OnDisable()
    {
        // ��ƾ ����
        if (mainRoutine != null)
        {
            StopCoroutine(mainRoutine);
            mainRoutine = null;
        }

        // ���� �ʱ�ȭ
        spriteRenderer.enabled = false;
        transform.localScale = originalScale;

        if (redGauge != null)
            redGauge.localScale = Vector3.zero;

        catCollider.SetActive(false);
    }

    private void Start()
    {
        // Start���� ���� 1ȸ ���� (�� ���� ��)
        RestartRoutine();
    }

    private void RestartRoutine()
    {
        // ���� ��ƾ ���� �� �����
        if (mainRoutine != null)
            StopCoroutine(mainRoutine);

        mainRoutine = StartCoroutine(MainRoutine());
    }

    private IEnumerator MainRoutine()
    {
        yield return new WaitForSeconds(5f); // ���� ���� �� 5�� �� ù ����

        while (true)
        {
            // ���� ��ġ ����
            int rand = Random.Range(0, spawnZones.Length);
            Bounds bounds = spawnZones[rand].bounds;
            Vector2 spawnPos = bounds.center;
            transform.position = spawnPos;

            // �ʱ�ȭ
            transform.localScale = originalScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaStart / 255f);
            spriteRenderer.enabled = true;

            // �߰� �� ���
            yield return new WaitForSeconds(0.5f);

            // �߰�
            float chaseTimer = 0f;
            Vector2 attackTarget = Vector2.zero;
            bool targetLocked = false;

            while (chaseTimer < chaseDuration)
            {
                // �� ���� �ð��� �� �� ���� ���� ��ǥ ���
                if (!targetLocked && chaseTimer >= attackLockTime)
                {
                    attackTarget = playerTransform.position;
                    targetLocked = true;
                }

                Vector2 targetPos = playerTransform.position;
                transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * chaseSpeed);

                // RedGauge Ŀ���� �����
                if (redGauge != null)
                {
                    float t = chaseTimer / chaseDuration;
                    float scale = Mathf.Lerp(0f, redGaugeMaxScale, t);
                    redGauge.localScale = new Vector3(scale, scale, 1f);
                }

                chaseTimer += Time.deltaTime;
                yield return null;
            }

            // ���� ���� ���� ����: ���ڱ� �۾����� ���İ� ����
            transform.position = attackTarget;
            transform.localScale = targetScale;
            spriteRenderer.color = new Color(0, 0, 0, alphaEnd / 255f);

            if (redGauge != null)
                redGauge.localScale = Vector3.zero;

            // �ݶ��̴� �ߵ�
            catCollider.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            catCollider.SetActive(false);

            // ���̵�ƿ�
            yield return StartCoroutine(FadeAlpha(alphaEnd, 0f, fadeOutDuration));

            // ����
            spriteRenderer.enabled = false;
            transform.localScale = originalScale;

            // ���� ������ ���� ���
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float fromA = Mathf.Clamp01(from / 255f);
        float toA = Mathf.Clamp01(to / 255f);
        float t = 0f;
        float dur = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            var col = spriteRenderer.color;   // ���� �� �б�
            col.a = Mathf.Lerp(fromA, toA, t);
            spriteRenderer.color = col;
            yield return null;
        }

        var end = spriteRenderer.color;
        end.a = toA;
        spriteRenderer.color = end;
    }
}
