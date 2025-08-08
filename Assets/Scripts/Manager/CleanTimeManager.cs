using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanTimeManager : MonoBehaviour
{
    public TextMeshProUGUI cleanTimerText;
    [SerializeField] private int totalCleanTime = 60;   // 총 청소 시간
    private float currentTime;                         // 초 단위 부동소수점
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
            yield return null; // 매 프레임 체크
        }

        catHandAttack.SetActive(false);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Main");
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
