using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VacuumController : MonoBehaviour       // 청소기 이동 컨트롤러
{
    public float rotationSpeed = 100f;
    public float acceleration = 5f;
    public float brakePower = 20f;
    public float deceleration = 10f;
    public float maxSpeed = 20f;
    public float maxBackSpeed = 10f;
    public float currentSpeed = 0f;

    public Rigidbody2D rb;

    // Booster 관련
    public float boosterSpeed = 10f;
    public float boosterDuration = 1f;
    public float boosterCooldown = 4f;
    private float boosterCooldownTimer = 0f;
    private bool isBoosterActive = false;
    private bool isZeroBoosting = false;

    public Image boosterCooldownImage; // 쿨타임 숫자용 fill bar
    [SerializeField] private Image GaugedownOverlay; // 부스터 지속시간 동안 위에서 아래로 줄어드는 게이지
    [SerializeField] private Image cooldownOverlayImage; // 쿨타임 동안 그냥 덮이는 이미지
    [SerializeField] private TextMeshProUGUI cooldownText; // 숫자 텍스트

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleBoosterUI();

        if (Input.GetKeyDown(KeyCode.B) && !isBoosterActive && boosterCooldownTimer <= 0f)
        {
            StartCoroutine(BoosterRoutine());
        }
    }

    void FixedUpdate()
    {
        if (isZeroBoosting) return;

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // 브레이크
        if (Input.GetKey(KeyCode.Space))
        {
            if (currentSpeed > 0)
                currentSpeed = Mathf.Max(currentSpeed - brakePower * Time.fixedDeltaTime, 0);
            else if (currentSpeed < 0)
                currentSpeed = Mathf.Min(currentSpeed + brakePower * Time.fixedDeltaTime, 0);
        }
        else
        {
            // 가속 / 후진 / 자연 감속
            if (v > 0)
                currentSpeed += acceleration * Time.fixedDeltaTime;
            else if (v < 0)
                currentSpeed -= acceleration * Time.fixedDeltaTime;
            else
            {
                if (currentSpeed > 0)
                    currentSpeed -= deceleration * Time.fixedDeltaTime;
                else if (currentSpeed < 0)
                    currentSpeed += deceleration * Time.fixedDeltaTime;
            }
        }

        // 최대 속도 제한
        if (!isBoosterActive)
            currentSpeed = Mathf.Clamp(currentSpeed, -maxBackSpeed, maxSpeed);

        rb.MovePosition(rb.position + (Vector2)(transform.up * currentSpeed * Time.fixedDeltaTime));

        float rotation = -h * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }

    IEnumerator BoosterRoutine()
    {
        isBoosterActive = true;

        float elapsed = 0f;
        GaugedownOverlay.fillAmount = 1f;
        GaugedownOverlay.gameObject.SetActive(true); // 줄어드는 게이지 켜기

        if (Mathf.Approximately(currentSpeed, 0f))
        {
            isZeroBoosting = true;

            float boostDistance = 5f;
            Vector2 start = rb.position;
            Vector2 target = start + (Vector2)(transform.up * boostDistance);

            while (elapsed < boosterDuration)
            {
                rb.MovePosition(Vector2.Lerp(start, target, elapsed / boosterDuration));
                GaugedownOverlay.fillAmount = 1f - (elapsed / boosterDuration); // 게이지 줄이기
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.MovePosition(target);
            currentSpeed = 0f;
            isZeroBoosting = false;
        }
        else
        {
            float boostedSpeed = currentSpeed + boosterSpeed;
            currentSpeed = boostedSpeed;

            while (elapsed < boosterDuration)
            {
                GaugedownOverlay.fillAmount = 1f - (elapsed / boosterDuration); // 게이지 줄이기
                elapsed += Time.deltaTime;
                yield return null;
            }

            float afterSpeed = GetAfterSpeed(boostedSpeed);
            currentSpeed = Mathf.Min(afterSpeed, maxSpeed);
        }

        isBoosterActive = false;
        boosterCooldownTimer = boosterCooldown;

        GaugedownOverlay.gameObject.SetActive(false); // 게이지 꺼주고
        cooldownOverlayImage.gameObject.SetActive(true); // 회색 덮기 이미지 켜기

        StartCoroutine(StartBoosterCooldown());
    }

    float GetAfterSpeed(float boostedSpeed)
    {
        float percent = 100f;

        if (boostedSpeed < 11f) percent = 120f;
        else if (boostedSpeed < 12f) percent = 119f;
        else if (boostedSpeed < 13f) percent = 118f;
        else if (boostedSpeed < 14f) percent = 117f;
        else if (boostedSpeed < 15f) percent = 116f;
        else if (boostedSpeed < 16f) percent = 115f;
        else if (boostedSpeed < 17f) percent = 114f;
        else if (boostedSpeed < 18f) percent = 113f;
        else if (boostedSpeed < 19f) percent = 112f;
        else if (boostedSpeed < 20f) percent = 111f;
        else if (Mathf.Approximately(boostedSpeed, 20f)) percent = 110f;

        return boostedSpeed * (percent / 100f);
    }

    IEnumerator StartBoosterCooldown()
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
    }

    void HandleBoosterUI()
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
}