using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VacuumController : MonoBehaviour       // 청소기 이동 컨트롤러
{
    [SerializeField] private float rotationSpeed = 100f;              // 회전 속도
    [SerializeField] private float brakePower = 20f;                  // 브레이크 속도
    [SerializeField] private float deceleration = 10f;                // 감속도
    [SerializeField] private float maxBackSpeed = 10f;                // 후진 최고 속도

    public float acceleration = 3f;                 // 가속도
    public float maxSpeed = 20f;                    // 전진 최고 속도
    public float currentSpeed = 0f;                 // 현재 속도
    public Rigidbody2D rb;                          // 플레이어 Rigidbody

    public float baseAcceleration = 3f;             // 항상 유지되는 원래 가속도
    public bool isSlowed = false;
    public bool isBoosting = false;

    public Booster booster;
    public CleanTimeManager cleanTimeManager;
    public TextMeshProUGUI gaugeText;                       // 계기판 텍스트

    public float slowRate = 0.2f;                   // 기본 감속 비율

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (cleanTimeManager.isPaused) return;

        UpdateGaugeUI();
    }

    void FixedUpdate()
    {
        if (cleanTimeManager.isPaused) return;

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.Space)) // 브레이크
        {
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(currentSpeed - brakePower * Time.fixedDeltaTime, 0);
                if (currentSpeed < 0.01f) currentSpeed = 0f;
            }
            else if (currentSpeed < 0)
            {
                currentSpeed = Mathf.Min(currentSpeed + brakePower * Time.fixedDeltaTime, 0);
                if (currentSpeed > -0.01f) currentSpeed = 0f;
            }
        }
        else
        {
            if (v > 0)
                currentSpeed += acceleration * Time.fixedDeltaTime;
            else if (v < 0)
                currentSpeed -= acceleration * Time.fixedDeltaTime;
            else
            {
                if (currentSpeed > 0)
                {
                    currentSpeed -= deceleration * Time.fixedDeltaTime;
                    if (currentSpeed < 0.01f) currentSpeed = 0f;
                }
                else if (currentSpeed < 0)
                {
                    currentSpeed += deceleration * Time.fixedDeltaTime;
                    if (currentSpeed > -0.01f) currentSpeed = 0f;
                }
            }
        }

        rb.MovePosition(rb.position + (Vector2)(transform.up * currentSpeed * Time.fixedDeltaTime));

        float rotation = -h * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackSpeed, maxSpeed);
    }

    public void ApplySlow()
    {
        isSlowed = true;
        currentSpeed *= slowRate;
    }

    public void RemoveSlow()
    {
        isSlowed = false;
    }

    void UpdateGaugeUI()        // 개기판 UI 업데이트
    {
        gaugeText.text = currentSpeed.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cat"))
        {
            Debug.Log("플레이어를 공격하는걸 성공!");
        }
    }
}