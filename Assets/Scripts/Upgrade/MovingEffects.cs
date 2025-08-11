using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEffects : MonoBehaviour       // �̵� ��� ȿ��
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
                // �ν��Ͱ� Ȱ��ȭ ������ ���� �ӵ� ����
                if (booster.isBoosterActive)
                {
                    booster.lowBoostSpeed = 4f;
                }
                else
                {
                    booster.lowBoostSpeed = 4f; // ��Ȱ�� ���¿����� �⺻������ ����
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
                booster.lowBoostSpeed = 8f; // ���� �ӵ��� ����
            }
        }
    }
}
