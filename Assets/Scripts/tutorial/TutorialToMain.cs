using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialToMain : MonoBehaviour         // 튜토리얼 이동
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
