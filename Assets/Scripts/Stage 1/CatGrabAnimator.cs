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

    [SerializeField] private Transform redGauge; // �߰�: ��������� ������Ʈ
    [SerializeField] private float redGaugeMaxScale = 5f; // �ִ� ũ��


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
            redGauge.localScale = Vector3.zero; // ���� �� 0����
    }

    private void Start()
    {
        StartCoroutine(MainRoutine());
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

            while (chaseTimer < chaseDuration)
            {
                if (chaseTimer >= chaseDuration - 0.8f)
                    attackTarget = playerTransform.position;

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