using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassManager : MonoBehaviour     // ���׷��̵� ��� ���
{
    public List<Image> classList = new List<Image>();           // ���׷��̵� ��� �̹���
    public List<LevelSO> lebelSOList = new List<LevelSO>();     // ���׷��̵� ����

    [SerializeField] private VacuumSystem vacuumSystem;
    [SerializeField] private CleanTimeManager cleanTimeManager;

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
        int n = 0;
        for (int i = 0; i < lebelSOList.Count; i++)
        {
            if (selectLevel.name == lebelSOList[n].levelName)
            {
                Debug.Log(lebelSOList[n].levelName);
                Debug.Log(lebelSOList[n].levelPoint);
                lebelSOList[n].levelPoint++;
            }
            n++;
        }

        HideClassUIList();
    }

    public void CursorOver()        // Ŀ���� �÷����� ��
    {

    }
}
