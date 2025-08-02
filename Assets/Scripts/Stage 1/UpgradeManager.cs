using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassManager : MonoBehaviour     // 업그레이드 페널 출력
{
    public List<Image> classList = new List<Image>();           // 업그레이드 페널 이미지
    public List<LevelSO> lebelSOList = new List<LevelSO>();     // 업그레이드 종류
    public TextMeshProUGUI chooseLevelText;                    // 선택한 업그레이드

    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private CleanTimeManager cleanTimeManager;

    public void ShowClassUIList()           // 레벨업 보상 출력 메서드
    {
        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(true);
        }

        UpdateLevelUPContents();
        cleanTimeManager.PauseTimer();
    }

    void UpdateLevelUPContents()            // 레벨업 보상 내용 업데이트
    {
        classList[0].sprite = lebelSOList[0].levelImage;
    }

    public void HideClassUIList()           // 클래스 보상 제거 메서드
    {
        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(false);
        }

        vacuumSystem.ExitRewardPause();
        cleanTimeManager.ResumeTimer();
    }

    public void GetLevel(GameObject selectLevel)      // 선택한 레벨
    {
        int n = 0;
        for (int i = 0; i < lebelSOList.Count; i++)
        {
            if (selectLevel.name == lebelSOList[n].levelName)
            {
                lebelSOList[n].levelPoint++;
                chooseLevelText.text = lebelSOList[n].levelName + "\n" + lebelSOList[n].levelPoint;     // 선택한 레벨 출력
            }
            n++;
        }

        HideClassUIList();
    }

    public void CursorOver()        // 커서가 올려졌을 때
    {

    }
}
