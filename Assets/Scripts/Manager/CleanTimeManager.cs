using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanTimeManager : MonoBehaviour
{
    public TextMeshProUGUI cleanTimerText;
    [SerializeField] private int totalCleanTime = 60;   // �� û�� �ð�
    private float currentTime;                         // �� ���� �ε��Ҽ���
    public bool isPaused = false;
    public GameObject catHandAttack;

    void Start()
    {
        currentTime = totalCleanTime;
        UpdateCleanTimerUI();
        StartCoroutine(UpdateCleanTime());
    }

    void UpdateCleanTimerUI()
    {
        cleanTimerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    IEnumerator UpdateCleanTime()
    {
        while (currentTime > 0)
        {
            if (!isPaused)
            {
                currentTime -= Time.deltaTime;
                UpdateCleanTimerUI();
            }
            yield return null; // �� ������ üũ
        }

        catHandAttack.SetActive(false);

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Stage 1") SceneManager.LoadScene("stage 2");
        if (currentSceneName == "Stage 2") SceneManager.LoadScene("stage 3");
        if (currentSceneName == "Stage 3") SceneManager.LoadScene("Intro");
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }
}
