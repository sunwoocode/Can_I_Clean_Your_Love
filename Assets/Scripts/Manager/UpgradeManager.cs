using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour     // 업그레이드 페널 출력
{
    public List<Image> classList = new List<Image>();           // 업그레이드 페널 이미지
    public List<LevelSO> levelSOList = new List<LevelSO>();     // 업그레이드 종류
    public TextMeshProUGUI chooseLevelText;                    // 선택한 업그레이드

    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private VacuumController vacuumController;
    [SerializeField] private CleanTimeManager cleanTimeManager;
    [SerializeField] private PlayerInfoSO playerInfoSO;
    [SerializeField] private PlayerSprite playerSprite;
    [SerializeField] private UpgradeUI upgradeUI;

    public void ShowClassUIList()           // 레벨업 보상 출력 메서드
    {
        vacuumController.currentSpeed = 0;
        upgradeUI.UpdateItemTitle();

        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(true);
        }
        cleanTimeManager.PauseTimer();
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
        for (int i = 0; i < levelSOList.Count; i++)
        {
            if (selectLevel.name == levelSOList[i].name)
            {
                levelSOList[i].levelPoint++;
                chooseLevelText.text = levelSOList[i].levelName + "\n" + levelSOList[i].levelPoint;     // 선택한 레벨 출력

                PlyerChange(i);
                WhatTheLevelEffect(i);
            }
        }
        HideClassUIList();
    }

    void PlyerChange(int i)      // 플레이어 외형 변화
    {
        if (i == 0)
            playerInfoSO.playerID += 10000;
        if (i == 1)
            playerInfoSO.playerID += 1000;
        if (i == 2)
            playerInfoSO.playerID += 100;
        if (i == 3)
            playerInfoSO.playerID += 10;
        if (i == 4)
            playerInfoSO.playerID += 1;

        playerSprite.WhatThePartName();
    }

    void WhatTheLevelEffect(int i)      // 적용
    {
        if (i == 4 && levelSOList[i].levelPoint == 1)
        {
            ;
        }
    }
}
