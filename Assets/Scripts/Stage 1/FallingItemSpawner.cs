using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fallingItemPrefab; // 떨어질 아이템 프리팹

    private bool hasSpawned = false;

    public void SpawnFallingItem(Vector2 spawnPos)
    {
        if (hasSpawned) return;  // 이미 생성했으면 무시
        hasSpawned = true;

        Instantiate(fallingItemPrefab, spawnPos, Quaternion.identity);
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
    }
}