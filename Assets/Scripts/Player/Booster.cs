using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Booster : MonoBehaviour
{
    // Booster ����
    public float boosterSpeed = 10f;                // Booster �ӵ�
    public float boosterDuration = 1f;              // Booster ���� �ð�
    public float boosterCooldown = 4f;              // Booster ��Ÿ��
    private float boosterCooldownTimer = 0f;        // Booster ��Ÿ�� Ÿ�̸�
    private bool isBoosterActive = false;           // Booster ������ üũ
    private bool isZeroBoosting = false;            // Booster ���� �ӵ��� 0���� üũ

    public Image boosterCooldownImage;                      // ��Ÿ�� ���ڿ� fill bar
    [SerializeField] private Image GaugedownOverlay;        // �ν��� ���ӽð� ���� ������ �Ʒ��� �پ��� ������
    [SerializeField] private Image cooldownOverlayImage;    // ��Ÿ�� ���� �׳� ���̴� �̹���
    [SerializeField] private TextMeshProUGUI cooldownText;  // ���� �ؽ�Ʈ

    public TextMeshProUGUI gaugeText;                       // ����� �ؽ�Ʈ

    public VacuumController vacuumController;

    public void BoosterStarting()
    {
        // �ߺ� ���� + ��Ÿ�� �߿��� ����
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

        // �ν��� ������ UI �ѱ�
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

        // UI ���� ����
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

        // ������ �Ǵ�
        float delta = boostedSpeed - originalSpeed;
        if (delta < 1f)
            vacuumController.currentSpeed = vacuumController.maxSpeed * 0.9f;
        else
            vacuumController.currentSpeed = vacuumController.maxSpeed - 1f;

        boosterCooldownTimer = boosterCooldown;

        GaugedownOverlay.gameObject.SetActive(false);
        cooldownOverlayImage.gameObject.SetActive(true);

        StartCoroutine(StartBoosterCooldown());

        isBoosterActive = false;  // ���� �� Ȱ�� ���� ����
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


    IEnumerator StartBoosterCooldown()          // Booster ��Ÿ�� UI
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
        cooldownOverlayImage.gameObject.SetActive(false); // ��Ÿ�� �������ϱ� ���� �̹��� ���ֱ�

        isBoosterActive = false;
    }

    void Update()           // UI ������Ʈ? - ���� �ż���� ��ġ�� �ʹ�.
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

    void UpdateGaugeUI()        // ������ UI ������Ʈ
    {
        gaugeText.text = vacuumController.currentSpeed.ToString();
    }
}
