using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanTimeManager : MonoBehaviour
{
    public TextMeshProUGUI cleanTimerText;
    [SerializeField] private int totalCleanTime = 60;   // �� û�� �ð�
    private float currentTime;                         // �� ���� �ε��Ҽ���
    private bool isPaused = false;

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

        SceneManager.LoadScene("gameOver");
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
