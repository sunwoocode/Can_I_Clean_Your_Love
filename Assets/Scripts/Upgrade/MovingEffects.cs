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

            if (booster != null)
            {
                // 부스터가 활성화 상태일 때도 속도 감소
                if (booster.isBoosterActive)
                {
                    booster.lowBoostSpeed = 4f;
                }
                else
                {
                    booster.lowBoostSpeed = 4f; // 비활성 상태에서도 기본적으로 감소
                }
            }

            if (vc.currentSpeed == 18f) vc.currentSpeed = 10f;
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

            if (booster != null)
            {
                booster.lowBoostSpeed = 8f; // 원래 속도로 복귀
            }
        }
    }
}
