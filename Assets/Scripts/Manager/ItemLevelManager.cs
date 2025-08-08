using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevelManager : MonoBehaviour       // ���� ������ ��� ����
{
    #region ����
    [SerializeField] private VacuumController controller;

    [Header("���̽�")]
    public GameObject fasterMoving;

    [Header("�߹ٴ�")]
    public GameObject frontDuster;  // �չ�
    public GameObject backDuster;   // �޹�
    public List<GameObject> dusterArea = new List<GameObject>();    // ���� ����

    [Header("����")]
    [SerializeField] private Sturn sturn;

    [Header("���� ����")]
    public GameObject healpack;
    [SerializeField] private PlayerHP playerHP;

    [Header("�ν���")]
    public Booster playerBoostComponent;
    public Image boostUI;
    #endregion

    public void ItemLevelApply(int i, int j)    // i�� ���� ��ȣ, j�� ���� ����
    {
        if (i == 0)         // ���̽� �̼�
        {
            switch (j)
            {
                case 2:
                    fasterMoving.SetActive(true);
                    controller.slowRate = 0.3f;
                    break;
                case 3:
                    controller.slowRate = 0.45f;
                    break;
                case 4:
                    controller.slowRate = 0.6f;
                    break;
                case 5:
                    controller.slowRate = 0.85f;
                    break;
                case 6:
                    controller.slowRate = 1f;
                    break;
            }
        }

        if (i == 1)         // ���� ���� �ð� ����
        {
            switch (j)
            {
                case 2:
                    sturn.sturnTime = 4f;
                    sturn.inviTime = 5f;
                    break;
                case 3:
                    sturn.sturnTime = 3.5f;
                    sturn.inviTime = 4.5f;
                    break;
                case 4:
                    sturn.sturnTime = 3f;
                    sturn.inviTime = 4f;
                    break;
                case 5:
                    sturn.sturnTime = 2.5f;
                    sturn.inviTime = 3.5f;
                    break;
                case 6:
                    sturn.sturnTime = 2f;
                    sturn.inviTime = 3f;
                    break;
            }
        }

        if (i == 2)         // ���� HP ����
        {
            switch (j)
            {
                case 2:
                    healpack.SetActive(true);
                    playerHP.HeartAdder();
                    break;
                case 3:
                    playerHP.HeartAdder();
                    break;
                case 4:
                    playerHP.HeartAdder();
                    break;
                case 5:
                    playerHP.HeartAdder();
                    break;
                case 6:
                    playerHP.HeartAdder();
                    break;
            }
        }

        if (i == 3)         // ���� �ν���
        {
            switch (j)
            {
                case 2:
                    playerBoostComponent.enabled = true;
                    boostUI.gameObject.SetActive(true);
                    playerBoostComponent.boosterCooldown = 6f;
                    break;
                case 3:
                    playerBoostComponent.boosterCooldown = 5.5f;
                    break;
                case 4:
                    playerBoostComponent.boosterCooldown = 5f;
                    break;
                case 5:
                    playerBoostComponent.boosterCooldown = 4.5f;
                    break;
                case 6:
                    playerBoostComponent.boosterCooldown = 4f;
                    break;
            }
        }

        if (i == 4)         // �߹ٴ� �۱�
        {
            switch (j)
            {
                case 2:
                    frontDuster.SetActive(true);
                    dusterArea[0].SetActive(false);
                    dusterArea[1].SetActive(true);
                    break;
                case 3:
                    dusterArea[1].SetActive(false);
                    dusterArea[2].SetActive(true);
                    break;
                case 4:
                    dusterArea[2].SetActive(false);
                    dusterArea[3].SetActive(true);
                    break;
                case 5:
                    backDuster.SetActive(true);
                    dusterArea[3].SetActive(false);
                    dusterArea[4].SetActive(true);
                    break;
                case 6:
                    dusterArea[4].SetActive(false);
                    dusterArea[5].SetActive(true);
                    break;
            }
        }
    }
}
