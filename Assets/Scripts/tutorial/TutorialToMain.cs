using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialToMain : MonoBehaviour         // Ʃ�丮�� �̵�
{
    public GameObject tutorialEndTrigger;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Stage 1");
        }
    }
}
