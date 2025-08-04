using System.Collections;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    private VacuumController controller;

    void Start()
    {
        controller = GetComponentInParent<VacuumController>();  // �������� VacuumController ã��
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            controller.currentSpeed = 0f;
        }
    }
}
