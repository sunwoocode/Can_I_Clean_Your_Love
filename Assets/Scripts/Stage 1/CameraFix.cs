using UnityEngine;

public class CameraFix : MonoBehaviour
{
    void LateUpdate()
    {
        // 회전을 항상 초기화 (0도)
        transform.rotation = Quaternion.identity;
    }
}