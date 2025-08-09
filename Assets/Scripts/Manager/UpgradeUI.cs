using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour      // ���׷��̵� UI
{
    public UpgradeManager upgradeManager;

    public List<TextMeshProUGUI> itemTitle = new List<TextMeshProUGUI>();
    public List<Image> itemImage = new List<Image>();                           // ��Ų �̹���
    public List<Image> itemImagePanel = new List<Image>();                      // ���� ����â
    public List<TextMeshProUGUI> itemContent = new List<TextMeshProUGUI>();

    public void UpdateItemTitle()
    {
        int count = Mathf.Min(itemTitle.Count, upgradeManager.levelSOList.Count);
        
        for (int i = 0; i < count; i++)
        {
            if (upgradeManager.levelSOList[i].levelPoint < 6)          // ���׷��̵� ������ ���� 5����
            {
                itemTitle[i].text = upgradeManager.levelSOList[i].levelPoint + "�� " + upgradeManager.levelSOList[i].levelName;
                itemImage[i].sprite = upgradeManager.levelSOList[i].levelImage;
                itemContent[i].text = upgradeManager.levelSOList[i].levelContent;

                if (upgradeManager.levelSOList[i].levelPoint == 1)
                {
                    itemContent[i].text += "\n\n\n�ŷ� + 99";
                }
            }
            else                // ����
            {
                itemImagePanel[i].color = new Color32(109, 109, 109, 255);
                itemTitle[i].gameObject.SetActive(false);
                itemImage[i].gameObject.SetActive(false);
                itemContent[i].gameObject.SetActive(false);

                // EventTrigger ��Ȱ��ȭ
                var trigger = itemImagePanel[i].GetComponent<EventTrigger>();
                if (trigger != null)
                    trigger.enabled = false;
            }
        }
    }
}
