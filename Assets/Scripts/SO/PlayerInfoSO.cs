using UnityEngine;


[CreateAssetMenu(fileName = "PlayerInfoSO", menuName = "PlayerInfoSO")]
public class PlayerInfoSO : ScriptableObject
{
    public int playerID;
}


// playerID의 범위는 다음과 같다.
// 1 ~ 11111
// 각 자리수를 더하여 ID를 구성한다.

// Sprite는 playerID를 참조하여 적용하는 것을 원칙으로 한다.

