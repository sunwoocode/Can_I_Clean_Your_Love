using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToIntroButton : MonoBehaviour
{
    public void OnClickIntroButton()
    {
        SceneManager.LoadScene("Intro"); 
    }

    public void ReTry()
    {
        SceneManager.LoadScene("stage 1");
    }
}
