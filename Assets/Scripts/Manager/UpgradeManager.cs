using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassManager : MonoBehaviour     // ���׷��̵� ��� ���
{
    public List<Image> classList = new List<Image>();           // ���׷��̵� ��� �̹���
    public List<LevelSO> lebelSOList = new List<LevelSO>();     // ���׷��̵� ����
    public TextMeshProUGUI chooseLevelText;                    // ������ ���׷��̵�

    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private CleanTimeManager cleanTimeManager;

    public SpriteRenderer player;
    public PlayerInfoSO playerInfoSO;

    public void ShowClassUIList()           // ������ ���� ��� �޼���
    {
        foreach (Image classUI in classList)
        {
            classUI.gameObject.SetActive(true);
        }

        UpdateLevelUPContents();
        cleanTimeManager.PauseTimer();
    }

    void UpdateLevelUPContents()            // ������ ���� ���� ������Ʈ
    {
        classList[0].sprite = lebelSOList[0].levelImage;
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
        for (int i = 0; i < lebelSOList.Count; i++)
        {
            if (selectLevel.name == lebelSOList[i].levelName)
            {
                lebelSOList[i].levelPoint++;
                chooseLevelText.text = lebelSOList[i].levelName + "\n" + lebelSOList[i].levelPoint;     // ������ ���� ���

                Debug.Log(i);
                PlyerChange(i);
            }
        }
        HideClassUIList();
    }

    void PlyerChange(int i)      // �÷��̾� ���� ��ȭ
    {
        Debug.Log(i);
        player.sprite = lebelSOList[i].skin;
        playerInfoSO.playerID = 11;
    }

    public void CursorOver()        // Ŀ���� �÷����� ��
    {

    }
}
