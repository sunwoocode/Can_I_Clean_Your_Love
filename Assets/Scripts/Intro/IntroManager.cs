using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public void OnClickStartButton()
    {
        DialogueManager.currentID = "S1_01";      // ��� ���� �ʱ�ȭ
        PlayerPrefs.DeleteKey("ReturnID");        // Ȥ�� ����� �� ������ ����
        SceneManager.LoadScene("Main");           // ���� ������ �̵�
    }

}
