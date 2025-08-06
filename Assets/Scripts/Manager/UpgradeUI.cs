using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour      // 업그레이드 UI
{
    public UpgradeManager upgradeManager;

    public List<TextMeshProUGUI> itemTitle = new List<TextMeshProUGUI>();
    public List<Image> itemImage = new List<Image>();
    public List<TextMeshProUGUI> itemContent = new List<TextMeshProUGUI>();

    public void UpdateItemTitle()
    {
        int count = Mathf.Min(itemTitle.Count, upgradeManager.levelSOList.Count);

        for (int i = 0; i < count; i++)
        {
            itemTitle[i].text = upgradeManager.levelSOList[i].levelPoint + "단\n" + upgradeManager.levelSOList[i].levelName;
            itemImage[i].sprite = upgradeManager.levelSOList[i].levelImage;
            itemContent[i].text = upgradeManager.levelSOList[i].levelContent;

            if (upgradeManager.levelSOList[i].levelPoint == 1)
            {
                itemContent[i].text += "\n\n\n매력이 1100101100001111만큼 상승합니다.";
            }
        }
    }
}
