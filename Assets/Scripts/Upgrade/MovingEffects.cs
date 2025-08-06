using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEffects : MonoBehaviour       // 이동 장애 효과
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VacuumController vc = other.GetComponent<VacuumController>();
            Booster booster = other.GetComponent<Booster>();
            if (vc != null)
            {
                vc.isSlowed = true;
                vc.currentSpeed *= 0.5f;
            }

            if (booster != null) booster.lowBoostSpeed = 4f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VacuumController vc = other.GetComponent<VacuumController>();
            Booster booster = other.GetComponent<Booster>();
            if (vc != null) vc.isSlowed = false;
            if (booster != null) booster.lowBoostSpeed = 8f;
        }
    }
}
