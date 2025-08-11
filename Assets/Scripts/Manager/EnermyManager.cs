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

    public List<GameObject> movingPattern = new List<GameObject>();     // 이동 방향 표시 오브젝트
    private int movingPatternCounter = 0;
    [SerializeField] private float previewTime = 0.6f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 시작 시 모든 패턴 끄기
        for (int i = 0; i < movingPattern.Count; i++)
            if (movingPattern[i]) movingPattern[i].SetActive(false);

        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (isPaused) { yield return null; continue; }

            // 현재 지점(from)과 다음 지점(to)
            int fromIdx = currentTargetIndex;
            int toIdx = (currentTargetIndex + 1) % positions.Count;

            // 1) 다음 구간 프리뷰 켜기 (이동 전 + 이동 중 내내 켜둠)
            ShowSegment(fromIdx, true);

            // 2) 프리뷰 대기(0.5초). 일시정지 시에는 타이머 진행 멈춤, 프리뷰는 계속 켜진 상태 유지
            float p = 0f;
            while (p < previewTime)
            {
                if (isPaused) { yield return null; continue; }
                p += Time.deltaTime;
                yield return null;
            }

            // 3) 실제 이동 (프리뷰는 계속 On 상태)
            Vector3 startPos = transform.position;
            Vector3 targetPos = positions[toIdx];

            float t = 0f;
            Vector3 direction = (targetPos - startPos).normalized;
            UpdateSpriteDirection(direction);

            while (t < moveDuration)
            {
                if (isPaused) { yield return null; continue; }
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, targetPos, t / moveDuration);
                yield return null;
            }

            // 4) 도착: 위치 스냅 + 이번 구간 프리뷰 끄기
            transform.position = targetPos;
            ShowSegment(fromIdx, false);

            // 5) 다음 사이클로
            currentTargetIndex = toIdx;
        }
    }

    // 프리뷰 세그먼트 On/Off (인덱스 안전 처리)
    void ShowSegment(int segIdx, bool on)
    {
        // 범위 보호
        if (movingPattern == null || movingPattern.Count == 0) return;

        // 세그먼트 개수와 positions 매핑:
        // 순환 이동이면 "fromIdx 기준"으로 세그먼트 개수를 positions.Count와 동일하게 두는 게 깔끔.
        segIdx = Mathf.Clamp(segIdx, 0, movingPattern.Count - 1);

        // 하나만 켜고 나머지는 끄기(가시성 보장)
        for (int i = 0; i < movingPattern.Count; i++)
        {
            var go = movingPattern[i];
            if (!go) continue;
            go.SetActive(on && i == segIdx);
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
