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

    public SpriteRenderer player;
    public PlayerInfoSO playerInfoSO;

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
        for (int i = 0; i < lebelSOList.Count; i++)
        {
            if (selectLevel.name == lebelSOList[i].levelName)
            {
                lebelSOList[i].levelPoint++;
                chooseLevelText.text = lebelSOList[i].levelName + "\n" + lebelSOList[i].levelPoint;     // 선택한 레벨 출력

                Debug.Log(i);
                PlyerChange(i);
            }
        }
        HideClassUIList();
    }

    void PlyerChange(int i)      // 플레이어 외형 변화
    {
        Debug.Log(i);
        player.sprite = lebelSOList[i].skin;
        playerInfoSO.playerID = 11;
    }

    public void CursorOver()        // 커서가 올려졌을 때
    {

    }
}
