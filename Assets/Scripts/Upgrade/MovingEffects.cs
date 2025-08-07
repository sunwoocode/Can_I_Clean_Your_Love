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
                vc.ApplySlow();
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

            if (vc != null)
            {
                vc.RemoveSlow();
            }

            if (booster != null) booster.lowBoostSpeed = 8f;
        }
    }
}
// 레이스 버그 수정해야함 부스터 중간에 들어와도 감소되도록