using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour       // PlayerID 계산 공식
{
    public PlayerInfoSO playerInfoSO;
    public List<GameObject> playerPartList = new List<GameObject>();

    void Start()
    {
        WhatThePartName();
    }

    public void WhatThePartName()
    {
        int id = playerInfoSO.playerID;
        int booster = (id / 10000) % 10;    // 부스터
        int vacuum = (id / 1000) % 10;      // 흡입기
        int wheel = (id / 100) % 10;        // 튼튼바퀴
        int armor = (id / 10) % 10;         // 철갑통
        int head = id % 10;                 // 머리머리

        if (booster == 1) playerPartList[0].SetActive(true);
        if (vacuum == 1) playerPartList[1].SetActive(true);
        if (wheel == 1) playerPartList[2].SetActive(true);
        if (armor == 1) playerPartList[3].SetActive(true);
        if (head == 1) playerPartList[4].SetActive(true);
        else return;
    }
}
