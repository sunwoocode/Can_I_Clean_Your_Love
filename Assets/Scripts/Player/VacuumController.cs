using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VacuumController : MonoBehaviour       // 청소기 이동 컨트롤러
{
    [SerializeField] private float rotationSpeed = 100f;              // 회전 속도
    [SerializeField] private float acceleration = 3f;                 // 가속도
    [SerializeField] private float brakePower = 20f;                  // 브레이크 속도
    [SerializeField] private float deceleration = 10f;                // 감속도
    [SerializeField] private float maxBackSpeed = 10f;                // 후진 최고 속도

    public float maxSpeed = 20f;                    // 전진 최고 속도
    public float currentSpeed = 0f;                 // 현재 속도
    public Rigidbody2D rb;                          // 플레이어 Rigidbody

    public Booster booster;
    public CleanTimeManager cleanTimeManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (cleanTimeManager.isPaused) return;

        if (Input.GetKeyDown(KeyCode.B))       // Booster 사용
        {
            booster.BoosterStarting();
        }

    }

    void FixedUpdate()      // 입력 처리 업데이트
    {
        if (cleanTimeManager.isPaused) return;

        float v = Input.GetAxis("Vertical");    // 앞뒤 입력
        float h = Input.GetAxis("Horizontal");  // 좌우 입력

        if (Input.GetKey(KeyCode.Space))        // 브레이크 (Space)
        {
            if (currentSpeed > 0)
            {
                currentSpeed = Mathf.Max(currentSpeed - brakePower * Time.fixedDeltaTime, 0);
                if (currentSpeed < 0.01f) currentSpeed = 0f; // 추가: 잔여 속도 정리
            }
            else if (currentSpeed < 0)
            {
                currentSpeed = Mathf.Min(currentSpeed + brakePower * Time.fixedDeltaTime, 0);
                if (currentSpeed > -0.01f) currentSpeed = 0f; // 추가: 잔여 속도 정리
            }
        }

        else
        {
            if (v > 0)          // 전진 감속
                currentSpeed += acceleration * Time.fixedDeltaTime;
            else if (v < 0)     // 후진 감속
                currentSpeed -= acceleration * Time.fixedDeltaTime;
            else                // 자연 감속
            {
                if (currentSpeed > 0)       // 전진 키를 땠을 때 자연 감속
                {
                    currentSpeed -= deceleration * Time.fixedDeltaTime;
                    if (currentSpeed < 0.01f) currentSpeed = 0f;  // 멈춤 처리
                }
                else if (currentSpeed < 0)   // 후진 키를 땠을 때 자연 감속
                {
                    currentSpeed += deceleration * Time.fixedDeltaTime;
                    if (currentSpeed > -0.01f) currentSpeed = 0f;  // 멈춤 처리
                }
            }
        }

        rb.MovePosition(rb.position + (Vector2)(transform.up * currentSpeed * Time.fixedDeltaTime));

        float rotation = -h * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackSpeed, maxSpeed);
    }
}