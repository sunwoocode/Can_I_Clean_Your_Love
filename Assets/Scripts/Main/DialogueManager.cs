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
        public string ChoiceText1;
        public string NextID1;
        public string ChoiceText2;
        public string NextID2;
        public string ChoiceText3;
        public string NextID3;
        public string NextID;
        public string IsChoicePoint;
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
    private string currentID = "S1_01";

    void Start()
    {
        LoadCSV();
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
                ChoiceText1 = parts[4].Trim(),
                NextID1 = parts[5].Trim(),
                ChoiceText2 = parts[6].Trim(),
                NextID2 = parts[7].Trim(),
                ChoiceText3 = parts[8].Trim(),
                NextID3 = parts[9].Trim(),
                NextID = parts[10].Trim(),
                IsChoicePoint = parts[11].Trim().ToLower()
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

        bool isChoice = data.IsChoicePoint == "true";

        NextButton.gameObject.SetActive(!isChoice);
        TwoAnswerPanel.SetActive(false);
        ThreeAnswerPanel.SetActive(false);

        if (isChoice)
        {
            bool hasThirdChoice = !string.IsNullOrWhiteSpace(data.ChoiceText3);

            if (hasThirdChoice)
            {
                // 3개 선택지
                ThreeAnswerPanel.SetActive(true);
                threeAnswerTexts[0].text = data.ChoiceText1;
                threeAnswerTexts[1].text = data.ChoiceText2;
                threeAnswerTexts[2].text = data.ChoiceText3;

                var b1 = ThreeAnswerPanel.transform.GetChild(0).GetComponent<Button>();
                var b2 = ThreeAnswerPanel.transform.GetChild(1).GetComponent<Button>();
                var b3 = ThreeAnswerPanel.transform.GetChild(2).GetComponent<Button>();

                b1.onClick.RemoveAllListeners();
                b2.onClick.RemoveAllListeners();
                b3.onClick.RemoveAllListeners();

                b1.onClick.AddListener(() => ShowDialogue(data.NextID1));
                b2.onClick.AddListener(() => ShowDialogue(data.NextID2));
                b3.onClick.AddListener(() => ShowDialogue(data.NextID3));
            }
            else
            {
                // 2개 선택지
                TwoAnswerPanel.SetActive(true);
                twoAnswerTexts[0].text = data.ChoiceText1;
                twoAnswerTexts[1].text = data.ChoiceText2;

                var btn1 = TwoAnswerPanel.transform.GetChild(0).GetComponent<Button>();
                var btn2 = TwoAnswerPanel.transform.GetChild(1).GetComponent<Button>();

                btn1.onClick.RemoveAllListeners();
                btn2.onClick.RemoveAllListeners();

                btn1.onClick.AddListener(() => ShowDialogue(data.NextID1));
                btn2.onClick.AddListener(() => ShowDialogue(data.NextID2));
            }
        }
    }

    public void OnClickNext()
    {
        DialogueData data = dialogueDictionary[currentID];
        Debug.Log($"현재 ID: {currentID}, 다음 ID: '{data.NextID}'");

        if (!string.IsNullOrEmpty(data.NextID))
        {
            string nextID = data.NextID;

            // nextID가 오직 한 글자고, 그게 숫자일 때만 씬 이동
            if (nextID.Length == 1 && char.IsDigit(nextID[0]))
            {
                switch (nextID)
                {
                    case "0":
                        Debug.Log("대사 종료, 튜토리얼로 이동");
                        SceneManager.LoadScene("tutorial");
                        break;
                    case "1":
                        Debug.Log("대사 종료, 스테이지1로 이동");
                        SceneManager.LoadScene("Stage 1");
                        break;
                    case "2":
                        Debug.Log("대사 종료, 스테이지2로 이동");
                        SceneManager.LoadScene("Stage 2");
                        break;
                    case "3":
                        Debug.Log("대사 종료, 스테이지3로 이동");
                        SceneManager.LoadScene("Stage 3");
                        break;
                    default:
                        Debug.LogWarning("기획자님 숫자 잘못 넣으셨어요ㅠ");
                        break;
                }
            }
            else
            {
                ShowDialogue(nextID);
            }
        }
        else
        {
            Debug.LogWarning("다음 대사 ID가 공란이네요 실수했네요 기획자 사마.");
        }
    }

}