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

    public SpriteRenderer player;

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
        player.color = new Color32(244, 146, 146, 255);
        StartCoroutine(IcePlayer());            // ���� �ð�
        StartCoroutine(InviPlayer());           // ���� �ð�
    }

    IEnumerator IcePlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        player.color = Color.white;
        controller.enabled = true;              // û�ұ� ��Ʈ�ѷ� Ȱ��ȭ
    }

    IEnumerator InviPlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        isInviState = false;
    }
}
