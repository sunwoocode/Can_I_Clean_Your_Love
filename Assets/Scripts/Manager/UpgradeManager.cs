using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour     // ���׷��̵� ��� ���
{
    public List<Image> classList = new List<Image>();           // ���׷��̵� ��� �̹���
    public List<LevelSO> levelSOList = new List<LevelSO>();     // ���׷��̵� ����
    public TextMeshProUGUI chooseLevelText;                    // ������ ���׷��̵�

    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private VacuumController vacuumController;
    [SerializeField] private CleanTimeManager cleanTimeManager;
    [SerializeField] private PlayerInfoSO playerInfoSO;
    [SerializeField] private PlayerSprite playerSprite;
    [SerializeField] private UpgradeUI upgradeUI;

    public void ShowClassUIList()           // ������ ���� ��� �޼���
    {
        vacuumController.currentSpeed = 0;
        upgradeUI.UpdateItemTitle();

        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(true);
        }
        cleanTimeManager.PauseTimer();
    }

    public void HideClassUIList()           // Ŭ���� ���� ���� �޼���
    {
        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(false);
        }

        vacuumSystem.ExitRewardPause();
        cleanTimeManager.ResumeTimer();
    }

    public void GetLevel(GameObject selectLevel)      // ������ ����
    {
        for (int i = 0; i < levelSOList.Count; i++)
        {
            if (selectLevel.name == levelSOList[i].name)
            {
                levelSOList[i].levelPoint++;
                chooseLevelText.text = levelSOList[i].levelName + "\n" + levelSOList[i].levelPoint;     // ������ ���� ���

                PlyerChange(i);
                WhatTheLevelEffect(i);
            }
        }
        HideClassUIList();
    }

    void PlyerChange(int i)      // �÷��̾� ���� ��ȭ
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

    void WhatTheLevelEffect(int i)      // ����
    {
        if (i == 4 && levelSOList[i].levelPoint == 1)
        {
            ;
        }
    }
}
