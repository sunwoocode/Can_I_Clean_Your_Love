using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueData
    {
        public string ID;
        public string Name;
        public string Dialogue;
        public string spriteID;
        public string IsChoicePoint;
        public string ChoiceText1;
        public string NextID1;
        public string ChoiceText2;
        public string NextID2;
        public string ChoiceText3;
        public string NextID3;
        public string NextID;
        public string SceneToLoad;
    }

    public TMP_Text DialogueText;
    public TMP_Text NameText;
    public Button NextButton;

    public GameObject TwoAnswerPanel;
    public GameObject ThreeAnswerPanel;

    public List<TMP_Text> twoAnswerTexts;
    public List<TMP_Text> threeAnswerTexts;

    public TextAsset csvFile;

    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();
    public static string currentID = "S1_01";

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[DialogueManager] 현재 씬: {currentScene}");

        LoadCSV();

        if (currentScene == "Intro")
        {
            currentID = "S1_01";
            PlayerPrefs.DeleteKey("ReturnID");
            Debug.Log("[DialogueManager] 인트로 씬 진입 -> currentID 초기화");
        }
        else if (PlayerPrefs.HasKey("ReturnID"))
        {
            currentID = PlayerPrefs.GetString("ReturnID");
            Debug.Log($"[DialogueManager] 저장된 다음 ID에서 이어감: {currentID}");
        }

        ShowDialogue(currentID);
    }

    void LoadCSV()
    {
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');

            DialogueData data = new DialogueData
            {
                ID = parts[0].Trim(),
                Name = parts[1].Trim(),
                Dialogue = parts[2].Trim(),
                spriteID = parts[3].Trim(),
                IsChoicePoint = parts[4].Trim().ToLower(),
                ChoiceText1 = parts[5].Trim(),
                NextID1 = parts[6].Trim(),
                ChoiceText2 = parts[7].Trim(),
                NextID2 = parts[8].Trim(),
                ChoiceText3 = parts[9].Trim(),
                NextID3 = parts[10].Trim(),
                NextID = parts[11].Trim(),
                SceneToLoad = parts.Length > 12 ? parts[12].Trim() : ""
            };

            dialogueDictionary[data.ID] = data;
        }

        Debug.Log($"CSV 로딩 완료, 총 {dialogueDictionary.Count}개 대사");
    }

    void ShowDialogue(string id)
    {
        if (!dialogueDictionary.ContainsKey(id))
        {
            Debug.LogError("해당 ID 없음: " + id);
            return;
        }

        DialogueData data = dialogueDictionary[id];
        currentID = id;

        DialogueText.text = data.Dialogue;
        NameText.text = data.Name;

        bool isChoice = data.IsChoicePoint.Trim().ToLower() == "true";

        // 선택지 여부에 따라 버튼 및 패널 활성화
        NextButton.gameObject.SetActive(!isChoice);
        TwoAnswerPanel.SetActive(false);
        ThreeAnswerPanel.SetActive(false);

        if (isChoice)
        {
            // 선택지 텍스트 비어있는지 확인
            bool hasChoice1 = !string.IsNullOrWhiteSpace(data.ChoiceText1);
            bool hasChoice2 = !string.IsNullOrWhiteSpace(data.ChoiceText2);
            bool hasChoice3 = !string.IsNullOrWhiteSpace(data.ChoiceText3);

            // 3개 선택지
            if (hasChoice1 && hasChoice2 && hasChoice3)
            {
                ThreeAnswerPanel.SetActive(true);

                threeAnswerTexts[0].text = data.ChoiceText1;
                threeAnswerTexts[1].text = data.ChoiceText2;
                threeAnswerTexts[2].text = data.ChoiceText3;

                for (int i = 0; i < 3; i++)
                {
                    var btn = ThreeAnswerPanel.transform.GetChild(i).GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    int choiceIndex = i; // 클로저 문제 방지용
                    btn.onClick.AddListener(() => ChoiceSelected(choiceIndex));
                }
            }
            // 2개 선택지
            else if (hasChoice1 && hasChoice2)
            {
                TwoAnswerPanel.SetActive(true);

                twoAnswerTexts[0].text = data.ChoiceText1;
                twoAnswerTexts[1].text = data.ChoiceText2;

                for (int i = 0; i < 2; i++)
                {
                    var btn = TwoAnswerPanel.transform.GetChild(i).GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    int choiceIndex = i;
                    btn.onClick.AddListener(() => ChoiceSelected(choiceIndex));
                }
            }
            else
            {
                Debug.LogError("선택지가 부족함. 최소 2개 필요");
            }
        }
    }

    public void ChoiceSelected(int index)
    {
        DialogueData data = dialogueDictionary[currentID];

        switch (index)
        {
            case 0:
                data.NextID = data.NextID1;
                break;
            case 1:
                data.NextID = data.NextID2;
                break;
            case 2:
                data.NextID = data.NextID3;
                break;
        }

        OnClickNext(); // 씬 이동 포함 통일된 처리
    }

    public void OnClickNext()
    {
        DialogueData data = dialogueDictionary[currentID];
        Debug.Log($"현재 ID: {currentID}, 다음 ID: '{data.NextID}', 이동할 씬: '{data.SceneToLoad}'");

        if (!string.IsNullOrEmpty(data.SceneToLoad))
        {
            PlayerPrefs.SetString("ReturnID", data.NextID);
            PlayerPrefs.Save();

            switch (data.SceneToLoad)
            {
                case "0":
                    Debug.Log("튜토리얼 씬 이동");
                    SceneManager.LoadScene("tutorial");
                    return;
                case "1":
                    Debug.Log("스테이지 1 이동");
                    SceneManager.LoadScene("Stage 1");
                    return;
                case "2":
                    Debug.Log("스테이지 2 이동");
                    SceneManager.LoadScene("Stage 2");
                    return;
                case "3":
                    Debug.Log("스테이지 3 이동");
                    SceneManager.LoadScene("Stage 3");
                    return;
                default:
                    Debug.LogWarning("SceneToLoad 값이 이상함. 숫자 0~3만 가능.");
                    return;
            }
        }

        if (!string.IsNullOrEmpty(data.NextID))
        {
            ShowDialogue(data.NextID);
        }
        else
        {
            Debug.LogWarning("다음 대사 ID가 공란이네요 기획자 사마.");
        }
    }
}