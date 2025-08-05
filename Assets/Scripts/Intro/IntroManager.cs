using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public void OnClickStartButton()
    {
        DialogueManager.currentID = "S1_01";      // 대사 순번 초기화
        PlayerPrefs.DeleteKey("ReturnID");        // 혹시 저장된 값 있으면 삭제
        SceneManager.LoadScene("Main");           // 메인 씬으로 이동
    }

}
