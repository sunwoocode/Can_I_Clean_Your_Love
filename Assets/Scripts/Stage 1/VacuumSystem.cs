using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VacuumSystem : MonoBehaviour       // 청소 시스템
{
    public float suckSpeed = 5f;                // 빨려 들어가는 이동 속도
    public float shrinkSpeed = 5f;              // 크기 줄어드는 속도
    public float knockbackForce = 5f;           // 장애물 넉백 힘

    [SerializeField] private int counter = 0;   // 청소 점수
    public TextMeshProUGUI countingTextUI;        // 쓰래기 카운팅 텍스트

    [SerializeField] private Transform gaugeFillTransform; // 게이지 차는 이미지
    [SerializeField] private int maxCount = 3;             // 총 쓰레기 개수 기준 (게이지 100%)

    [SerializeField] private GameObject rewardPauseOverlay;             // 회색 반투명 이미지
    [SerializeField] private VacuumController vacuumController;         // 플레이어 움직임 멈출 대상
    [SerializeField] private MonoBehaviour[] competitorControllers;     // 나중에 추가될 경쟁자들

    void CountingUpdateUI()
    {
        countingTextUI.text = counter.ToString();
    }

    void UpdateGaugeUI()
    {
        float fillAmount = Mathf.Clamp01((float)counter / maxCount);
        Vector3 scale = gaugeFillTransform.localScale;
        scale.x = fillAmount;
        gaugeFillTransform.localScale = scale;

        if (counter >= maxCount)
        {
            EnterRewardPause();
        }
    }

    void EnterRewardPause()
    {
        if (vacuumController != null)
            vacuumController.enabled = false;

        if (competitorControllers != null)
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = false;
            }
        }

        if (rewardPauseOverlay != null)
            rewardPauseOverlay.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D parentRb = transform.parent.GetComponent<Rigidbody2D>();

        // 쓰레기 처리
        if (other.CompareTag("Trash"))
        {
            StartCoroutine(Trash(other.transform)); // 빨아들이는 기능
            counter++;

            CountingUpdateUI();
            UpdateGaugeUI();
        }

        // 장애물 처리
        else if (other.CompareTag("Obstacle"))
        {
            if (parentRb != null)
            {
                parentRb.velocity = Vector2.zero;  // 물리 속도 정지
            }

            // currentSpeed도 정지
            VacuumController controller = parentRb.GetComponent<VacuumController>();
            if (controller != null)
            {
                controller.currentSpeed = 0f;
            }

            // 넉백도 작동하도록
            if (parentRb != null)
            {
                StartCoroutine(Obstacle(parentRb, other));
            }
        }
    }

    private IEnumerator Trash(Transform trash)      // 쓰레기 흡입
    {
        Vector3 startScale = trash.localScale;
        Vector3 endScale = Vector3.zero;
        float t = 0f;

        while (trash != null && trash.localScale.magnitude > 0.01f)      // 사라질 때까지
        {
            trash.position = Vector3.MoveTowards(trash.position, transform.position, suckSpeed * Time.deltaTime);
            trash.localScale = Vector3.Lerp(startScale, endScale, t);

            t += Time.deltaTime * shrinkSpeed;
            yield return null;
        }

        if (trash != null)
        {
            Destroy(trash.gameObject); // 완전히 빨려들면 삭제
        }
    }

    private IEnumerator Obstacle(Rigidbody2D parentRb, Collider2D obstacle)
    {
        yield return new WaitForFixedUpdate(); // 속도 0 적용 후 넉백 처리

        if (parentRb != null)
        {
            Vector2 dir = (parentRb.position - (Vector2)obstacle.transform.position).normalized;
            parentRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse); // 자연스러운 밀림
        }
    }
}
