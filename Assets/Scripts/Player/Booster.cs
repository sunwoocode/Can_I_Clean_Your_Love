using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    // Booster 관련
    public float boosterSpeed = 10f;                // Booster 속도
    public float boosterDuration = 1f;              // Booster 지속 시간
    public float boosterCooldown = 4f;              // Booster 쿨타임
    private float boosterCooldownTimer = 0f;        // Booster 쿨타임 타이머
    private bool isBoosterActive = false;           // Booster 중인지 체크
    private bool isZeroBoosting = false;            // Booster 이전 속도가 0인지 체크

    public Image boosterCooldownImage;                      // 쿨타임 숫자용 fill bar
    [SerializeField] private Image GaugedownOverlay;        // 부스터 지속시간 동안 위에서 아래로 줄어드는 게이지
    [SerializeField] private Image cooldownOverlayImage;    // 쿨타임 동안 그냥 덮이는 이미지
    [SerializeField] private TextMeshProUGUI cooldownText;  // 숫자 텍스트

    public TextMeshProUGUI gaugeText;                       // 계기판 텍스트

    public VacuumController vacuumController;

    public void BoosterStarting()
    {
        // 중복 실행 + 쿨타임 중에는 무시
        if (isBoosterActive || boosterCooldownTimer > 0f) return;

        isBoosterActive = true;

        if (vacuumController.currentSpeed < 7f)
        {
            StartCoroutine(LowBooster(8f, 0.5f));
        }
        else if (vacuumController.currentSpeed < 15f)
        {
            StartCoroutine(MiddleBooster(0.5f));
        }
        else
        {
            StartCoroutine(HighBooster(1f));
        }
    }


    IEnumerator LowBooster(float boostAmount, float duration)
    {
        Debug.Log("LowBooster");

        float passTime = 0f;
        float initialSpeed = vacuumController.currentSpeed;
        float targetSpeed = initialSpeed + boostAmount;

        // 부스터 게이지 UI 켜기
        GaugedownOverlay.fillAmount = 1f;
        GaugedownOverlay.gameObject.SetActive(true);

        while (passTime < duration)
        {
            float t = passTime / duration;
            vacuumController.currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, t);
            GaugedownOverlay.fillAmount = 1f - t;
            passTime += Time.deltaTime;
            yield return null;
        }

        vacuumController.currentSpeed = targetSpeed;

        boosterCooldownTimer = boosterCooldown;

        GaugedownOverlay.gameObject.SetActive(false);
        cooldownOverlayImage.gameObject.SetActive(true);

        StartCoroutine(StartBoosterCooldown());

        isBoosterActive = false;
    }

    IEnumerator MiddleBooster(float duration)
    {
        Debug.Log("MiddleBooster");

        float passTime = 0f;
        float originalSpeed = vacuumController.currentSpeed;
        float boostedSpeed = originalSpeed * 1.3f;

        // UI 연출 시작
        GaugedownOverlay.fillAmount = 1f;
        GaugedownOverlay.gameObject.SetActive(true);

        while (passTime < duration)
        {
            float t = passTime / duration;
            vacuumController.currentSpeed = Mathf.Lerp(originalSpeed, boostedSpeed, t);
            GaugedownOverlay.fillAmount = 1f - t;
            passTime += Time.deltaTime;
            yield return null;
        }

        vacuumController.currentSpeed = boostedSpeed;

        // 증가폭 판단
        float delta = boostedSpeed - originalSpeed;
        if (delta < 1f)
            vacuumController.currentSpeed = vacuumController.maxSpeed * 0.9f;
        else
            vacuumController.currentSpeed = vacuumController.maxSpeed - 1f;

        boosterCooldownTimer = boosterCooldown;

        GaugedownOverlay.gameObject.SetActive(false);
        cooldownOverlayImage.gameObject.SetActive(true);

        StartCoroutine(StartBoosterCooldown());

        isBoosterActive = false;  // 종료 시 활성 상태 해제
    }

    IEnumerator HighBooster(float duration)
    {
        Debug.Log("HighBooster");

        float passTime = 0f;
        float startSpeed = 25f;
        float endSpeed = 20f;

        GaugedownOverlay.fillAmount = 1f;
        GaugedownOverlay.gameObject.SetActive(true);

        while (passTime < duration)
        {
            float t = passTime / duration;
            vacuumController.currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t);
            GaugedownOverlay.fillAmount = 1f - t;
            passTime += Time.deltaTime;
            yield return null;
        }

        vacuumController.currentSpeed = endSpeed;

        boosterCooldownTimer = boosterCooldown;
        GaugedownOverlay.gameObject.SetActive(false);
        cooldownOverlayImage.gameObject.SetActive(true);
        StartCoroutine(StartBoosterCooldown());

        isBoosterActive = false;
    }


    IEnumerator StartBoosterCooldown()          // Booster 쿨타임 UI
    {
        cooldownText.gameObject.SetActive(true);

        int count = Mathf.CeilToInt(boosterCooldown);
        while (count >= 0)
        {
            cooldownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        cooldownText.gameObject.SetActive(false);
        cooldownOverlayImage.gameObject.SetActive(false); // 쿨타임 끝났으니까 덮기 이미지 꺼주기

        isBoosterActive = false;
    }

    void Update()           // UI 업데이트? - 개별 매서드로 고치고 싶다.
    {
        HandleBoosterUI();
        UpdateGaugeUI();
    }

    void HandleBoosterUI()      // Booster
    {
        if (boosterCooldownTimer > 0f)
        {
            boosterCooldownTimer -= Time.deltaTime;
            boosterCooldownImage.fillAmount = boosterCooldownTimer / boosterCooldown;
        }
        else
        {
            boosterCooldownImage.fillAmount = 0f;
        }
    }

    void UpdateGaugeUI()        // 개기판 UI 업데이트
    {
        gaugeText.text = vacuumController.currentSpeed.ToString();
    }
}
