using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFix : MonoBehaviour
{
    void LateUpdate()
    {
        // ȸ���� �׻� �ʱ�ȭ (0��)
        transform.rotation = Quaternion.identity;
    }
}