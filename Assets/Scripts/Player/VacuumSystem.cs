using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VacuumSystem : MonoBehaviour       // 청소 시스템
{
    public float suckSpeed = 5f;                // 빨려 들어가는 이동 속도
    public float shrinkSpeed = 5f;              // 크기 줄어드는 속도

    [SerializeField] private int counter = 0;   // 청소 점수
    public TextMeshProUGUI countingTextUI;        // 쓰래기 카운팅 텍스트

    [SerializeField] private Transform gaugeFillTransform; // 게이지 차는 이미지
    [SerializeField] private int maxCount = 3;             // 총 쓰레기 개수 기준 (게이지 100%)

    [SerializeField] private GameObject rewardPauseOverlay;             // 회색 반투명 이미지
    [SerializeField] private VacuumController vacuumController;         // 플레이어 움직임 멈출 대상
    [SerializeField] private MonoBehaviour[] competitorControllers;     // 나중에 추가될 경쟁자들
    [SerializeField] private UpgradeManager upgradeManager;                 // 레벨업 선택 창 출력 CS

    void UpdateGaugeUI()
    {
        float fillAmount = Mathf.Clamp01((float)counter / maxCount);
        Vector3 scale = gaugeFillTransform.localScale;
        scale.x = fillAmount;
        gaugeFillTransform.localScale = scale;

        if (counter >= maxCount)
        {
            EnterRewardPause();
            upgradeManager.ShowClassUIList();     // 레벨업 선택지 출력
        }
    }

    void EnterRewardPause()
    {
        if (vacuumController != null)           // 청소기 컨트롤러 비활성화
            vacuumController.enabled = false;

        if (competitorControllers != null)      // 경쟁자 오브젝트 비활성화
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = false;
            }
        }

        if (rewardPauseOverlay != null)         // 비활성화 레이어 이미지 활성화
            rewardPauseOverlay.SetActive(true);
    }

    public void ExitRewardPause()
    {
        if (vacuumController != null)           // 청소기 컨트롤러 활성화
            vacuumController.enabled = true;

        if (competitorControllers != null)      // 경쟁자 오브젝트 활성화
        {
            foreach (var c in competitorControllers)
            {
                if (c != null)
                    c.enabled = true;
            }
        }

        if (rewardPauseOverlay != null)         // 비활성화 레이어 이미지 비활성화
            rewardPauseOverlay.SetActive(false);

        counter = 0;
        UpdateGaugeUI();
    }

    void OnTriggerEnter2D(Collider2D other)     // 다른 콜라이더와 닿았을 때
    {
        Rigidbody2D parentRb = transform.parent.GetComponent<Rigidbody2D>();    // 부모의 Rigidbody2D

        if (other.CompareTag("Trash"))      // 쓰레기 청소 처리
        {
            other.tag = "Untagged";
            StartCoroutine(CleanTrash(other.transform)); // 빨아들이는 기능
        }
    }

    private IEnumerator CleanTrash(Transform trash)      // 쓰레기 흡입
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
            counter++;

            UpdateGaugeUI();
        }
    }
}
