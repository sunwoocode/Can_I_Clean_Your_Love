using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChange : MonoBehaviour          // M ������ ��ü �� Ȱ��/��Ȱ�� ����
{
    public GameObject changingMap;              // ��ü �� �θ� ������Ʈ

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (changingMap.activeSelf) changingMap.SetActive(false);
            else changingMap.SetActive(true);
        }
    }
}
