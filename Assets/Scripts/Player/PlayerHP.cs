using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour               // HP 적용
{
    public int heartIndex;                          // HP 갯수
    public LevelSO heartSO;                         // HP 레벨 참조
    public List<Image> hpBar = new List<Image>();   // HP UI
    public Sturn sturn;                             // 무적 상태 참조

    void Start()
    {
        heartIndex = heartSO.levelPoint + 1;
        HeartSetting();
    }

    void HeartSetting()                             // 초기 HP 설정
    {
        for (int i = 0; i < heartIndex; i++)
        {
            hpBar[i].gameObject.SetActive(true);
        }
    }

    public void HeartCounter()                      // HP 차감 메서드
    {
        if (sturn.isInviState) return;              // 무적 상태라면 나가기
        heartIndex--;
        UpdateHeartsUI(false);
    }

    public void HeartAdder()                        // HP 증가 메서드
    {
        heartIndex++;
        UpdateHeartsUI(true);
    }

    void UpdateHeartsUI(bool b)                     // HP 증감 메서드
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

        if (heartIndex == 0)                        // 게임 종료
            SceneManager.LoadScene("GameOver");
    }
}
