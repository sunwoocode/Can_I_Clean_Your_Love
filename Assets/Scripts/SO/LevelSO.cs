using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelSO", menuName = "LevelSO")]
public class LevelSO : ScriptableObject
{
    public string levelName;                                    // ���� �̸�
    public int levelPoint;                                      // ����
    public Sprite levelImage;                                   // ���� ���� �̹���
    public List<string> levelContent = new List<string>();      // ���� ���� ����
    public string partContect;                                  // ���� ����
}