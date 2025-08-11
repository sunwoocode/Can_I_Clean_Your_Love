using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelSO", menuName = "LevelSO")]
public class LevelSO : ScriptableObject
{
    public string levelName;                                    // 레벨 이름
    public int levelPoint;                                      // 레벨
    public Sprite levelImage;                                   // 레벨 선택 이미지
    public List<string> levelContent = new List<string>();      // 다음 레벨 선택
    public string partContect;                                  // 파츠 설명
}