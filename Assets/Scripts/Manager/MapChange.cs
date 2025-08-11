using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChange : MonoBehaviour          // M 누르면 전체 맵 활성/비활성 변경
{
    public GameObject changingMap;              // 전체 맵 부모 오브젝트

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (changingMap.activeSelf) changingMap.SetActive(false);
            else changingMap.SetActive(true);
        }
    }
}
