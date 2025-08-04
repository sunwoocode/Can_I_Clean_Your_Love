using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelSO", menuName = "LevelSO")]
public class LevelSO : ScriptableObject
{
    public string levelName;    // 레벨 이름
    public int levelPoint;        // 레벨
    public Sprite levelImage;    // 레벨 선택 이미지
    public Sprite skin;             // 플레이어 이미지
}