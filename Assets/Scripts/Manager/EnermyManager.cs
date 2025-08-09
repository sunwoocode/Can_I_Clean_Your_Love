using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyManager : MonoBehaviour
{
    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private float moveDuration = 1.5f;

    private int currentTargetIndex = 0;

    public PlayerHP playerHP;
    public Sturn sturn;

    private SpriteRenderer spriteRenderer;

    private bool isPaused = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (isPaused)
            {
                yield return null;
                continue;  // 일시정지 시 루프 대기
            }

            Vector3 startPos = transform.position;
            Vector3 targetPos = positions[currentTargetIndex];
            float timer = 0f;

            Vector3 direction = (targetPos - startPos).normalized;
            UpdateSpriteDirection(direction);

            while (timer < moveDuration)
            {
                if (isPaused) break;  // 중간에 멈추는 것도 반영
                timer += Time.deltaTime;
                float t = timer / moveDuration;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            currentTargetIndex = (currentTargetIndex + 1) % positions.Count;
        }
    }


    private void UpdateSpriteDirection(Vector3 dir)
    {
        if (spriteRenderer == null) return;

        // 가장 큰 축 기준으로 방향 판단
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // 좌우
            spriteRenderer.flipX = dir.x < 0;
            spriteRenderer.flipY = false;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            // 상하
            spriteRenderer.flipX = false;
            if (dir.y > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f); // 위
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f); // 아래
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("교통사고 빵야!");

            playerHP.HeartCounter();
            sturn.SturnEffect();
        }
    }

    public void PauseMove()
    {
        isPaused = true;
    }

    public void ResumeMove()
    {
        isPaused = false;
    }
}
