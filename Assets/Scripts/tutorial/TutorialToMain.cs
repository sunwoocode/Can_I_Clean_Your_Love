using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialToMain : MonoBehaviour
{
    public void GoToMainScene()
    {
        SceneManager.LoadScene("Main");
    }
}
