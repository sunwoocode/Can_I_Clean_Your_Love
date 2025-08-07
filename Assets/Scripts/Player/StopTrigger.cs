using System.Collections;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    [SerializeField] private VacuumController controller;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("aaaa");
            controller.currentSpeed = 0f;
        }
    }
}
