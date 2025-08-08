using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour               // HP ����
{
    public int heartIndex;                          // HP ����
    public LevelSO heartSO;                         // HP ���� ����
    public List<Image> hpBar = new List<Image>();   // HP UI
    public Sturn sturn;                             // ���� ���� ����

    void Start()
    {
        heartIndex = heartSO.levelPoint + 1;
        HeartSetting();
    }

    void HeartSetting()                             // �ʱ� HP ����
    {
        for (int i = 0; i < heartIndex; i++)
        {
            hpBar[i].gameObject.SetActive(true);
        }
    }

    public void HeartCounter()                      // HP ���� �޼���
    {
        if (sturn.isInviState) return;              // ���� ���¶�� ������
        heartIndex--;
        UpdateHeartsUI(false);
    }

    public void HeartAdder()                        // HP ���� �޼���
    {
        heartIndex++;
        UpdateHeartsUI(true);
    }

    void UpdateHeartsUI(bool b)                     // HP ���� �޼���
    {
        if (b)
        {
            if (heartIndex - 1 >= 0 && heartIndex - 1 < hpBar.Count)
                hpBar[heartIndex - 1].gameObject.SetActive(true);
        }
        else
        {
            if (heartIndex >= 0 && heartIndex < hpBar.Count)
                hpBar[heartIndex].gameObject.SetActive(false);
        }

        if (heartIndex == 0)                        // ���� ����
            SceneManager.LoadScene("GameOver");
    }
}
