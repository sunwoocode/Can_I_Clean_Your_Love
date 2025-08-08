using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevelManager : MonoBehaviour       // 파츠 레벨업 기능 적용
{
    #region 변수
    [SerializeField] private VacuumController controller;

    [Header("레이스")]
    public GameObject fasterMoving;

    [Header("발바닥")]
    public GameObject frontDuster;  // 앞발
    public GameObject backDuster;   // 뒷발
    public List<GameObject> dusterArea = new List<GameObject>();    // 흡입 범위

    [Header("모자")]
    [SerializeField] private Sturn sturn;

    [Header("피통 증가")]
    public GameObject healpack;
    [SerializeField] private PlayerHP playerHP;

    [Header("부스터")]
    public Booster playerBoostComponent;
    public Image boostUI;
    #endregion

    public void ItemLevelApply(int i, int j)    // i는 파츠 번호, j는 파츠 레벨
    {
        if (i == 0)         // 레이스 이속
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

        if (i == 1)         // 모자 스턴 시간 감소
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

        if (i == 2)         // 오이 HP 증가
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

        if (i == 3)         // 꼬리 부스터
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

        if (i == 4)         // 발바닥 닦기
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
