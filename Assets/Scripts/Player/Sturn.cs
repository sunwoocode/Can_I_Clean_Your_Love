using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sturn : MonoBehaviour              // 스턴 효과
{
    public float sturnTime;                     // 스턴 시간
    public float inviTime;                      // 무적 시간
    public LevelSO strunSO;                     // 스턴 레벨 참조
    public bool isInviState;                  // 무적 상태 체크

    [SerializeField] private VacuumController controller;
    [SerializeField] private ItemLevelManager itemLevel;

    public SpriteRenderer player;

    void Start()        // 스턴 값 초기화
    {
        if (strunSO.levelPoint == 1)
        {
            sturnTime = 5f;
            inviTime = 6f;
        }
        else itemLevel.ItemLevelApply(1, strunSO.levelPoint);
    }

    public void SturnEffect()                   // 스턴 적용
    {
        controller.currentSpeed = 0;
        controller.gaugeText.text = 0.ToString();
        controller.enabled = false;             // 청소기 컨트롤러 비활성화
        isInviState = true;
        player.color = new Color32(244, 146, 146, 255);
        StartCoroutine(IcePlayer());            // 스턴 시간
        StartCoroutine(InviPlayer());           // 무적 시간
    }

    IEnumerator IcePlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        player.color = Color.white;
        controller.enabled = true;              // 청소기 컨트롤러 활성화
    }

    IEnumerator InviPlayer()
    {
        yield return new WaitForSeconds(sturnTime);
        isInviState = false;
    }
}
