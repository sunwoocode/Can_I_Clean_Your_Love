using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScore : MonoBehaviour
{
    public int trashCount { get; private set; } = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash"))
        {
            trashCount++;
            Destroy(other.gameObject);

            Debug.Log($"{gameObject.name}가 모은 쓰레기 개수: {trashCount}");
        }
    }
}
