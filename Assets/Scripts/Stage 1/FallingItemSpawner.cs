using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fallingItemPrefab; // ������ ������ ������

    private bool hasSpawned = false;

    public void SpawnFallingItem(Vector2 spawnPos)
    {
        if (hasSpawned) return;  // �̹� ���������� ����
        hasSpawned = true;

        Instantiate(fallingItemPrefab, spawnPos, Quaternion.identity);
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
    }
}