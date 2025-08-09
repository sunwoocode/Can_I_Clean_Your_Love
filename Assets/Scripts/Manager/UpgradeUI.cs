using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour      // 업그레이드 UI
{
    public UpgradeManager upgradeManager;

    public List<TextMeshProUGUI> itemTitle = new List<TextMeshProUGUI>();
    public List<Image> itemImage = new List<Image>();                           // 스킨 이미지
    public List<Image> itemImagePanel = new List<Image>();                      // 레벨 선택창
    public List<TextMeshProUGUI> itemContent = new List<TextMeshProUGUI>();

    public void UpdateItemTitle()
    {
        int count = Mathf.Min(itemTitle.Count, upgradeManager.levelSOList.Count);
        
        for (int i = 0; i < count; i++)
        {
            if (upgradeManager.levelSOList[i].levelPoint < 6)          // 업그레이드 가능한 레벨 5까지
            {
                itemTitle[i].text = upgradeManager.levelSOList[i].levelPoint + "단 " + upgradeManager.levelSOList[i].levelName;
                itemImage[i].sprite = upgradeManager.levelSOList[i].levelImage;
                itemContent[i].text = upgradeManager.levelSOList[i].levelContent;

                if (upgradeManager.levelSOList[i].levelPoint == 1)
                {
                    itemContent[i].text += "\n\n\n매력 + 99";
                }
            }
            else                // 만랩
            {
                itemImagePanel[i].color = new Color32(109, 109, 109, 255);
                itemTitle[i].gameObject.SetActive(false);
                itemImage[i].gameObject.SetActive(false);
                itemContent[i].gameObject.SetActive(false);

                // EventTrigger 비활성화
                var trigger = itemImagePanel[i].GetComponent<EventTrigger>();
                if (trigger != null)
                    trigger.enabled = false;
            }
        }
    }
}
