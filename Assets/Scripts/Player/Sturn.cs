using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sturn : MonoBehaviour              // ���� ȿ��
{
    public float sturnTime;                     // ���� �ð�
    public float inviTime;                      // ���� �ð�
    public LevelSO strunSO;                     // ���� ���� ����
    public bool isInviState;                  // ���� ���� üũ

    [SerializeField] private VacuumController controller;
    [SerializeField] private ItemLevelManager itemLevel;

    void Start()        // ���� �� �ʱ�ȭ
    {
        if (strunSO.levelPoint == 1)
        {
            sturnTime = 5f;
            inviTime = 6f;
        }
        else itemLevel.ItemLevelApply(1, strunSO.levelPoint);
    }

    public void SturnEffect()                   // ���� ����
    {
        controller.currentSpeed = 0;
        controller.gaugeText.text = 0.ToString();
        controller.enabled = false;             // û�ұ� ��Ʈ�ѷ� ��Ȱ��ȭ
        isInviState = true;
        StartCoroutine(IcePlayer());            // ���� �ð�
        StartCoroutine(InviPlayer());           // ���� �ð�
    }

    IEnumerator IcePlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        controller.enabled = true;              // û�ұ� ��Ʈ�ѷ� Ȱ��ȭ
    }

    IEnumerator InviPlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        isInviState = false;
    }
}
