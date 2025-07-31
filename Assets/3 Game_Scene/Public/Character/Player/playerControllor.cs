using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour    
{

    // 다른 청소기(경쟁자들)한테도 써야하는 변수라 public 처리해써용
    public float rotationSpeed = 100f;     // 회전 속도입니당
    public float acceleration = 5f;        // 가속도
    public float brakePower = 20f;         // 브레이크 강도
    public float deceleration = 10f;       // 자연 감속
    public float maxSpeed = 30f;           // 최고속도
    public float maxBackSpeed = 10f;       // 후진 속도

    public float currentSpeed = 0f;        // 현재 속도(가만히 있을땐 0이어야함. 이동하면서 점점 속도 바뀌게 디테일 넣음)
    
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");   // 전진/후진 입력
        float h = Input.GetAxis("Horizontal"); // 회전 입력

        // ------ 가속도 처리 ------
        if (v > 0)
        {
            // 전진 가속
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if(v < 0)
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
        }
        else
        {
            // 자연 감속
            if (currentSpeed > 0)
                currentSpeed -= deceleration * Time.fixedDeltaTime;
            else if (currentSpeed < 0)
                currentSpeed += deceleration * Time.fixedDeltaTime;
        }

        // ------ 브레이크 처리 ------
        if (Input.GetKey(KeyCode.Space))
        {
            if (currentSpeed > 0)
                currentSpeed -= brakePower * Time.fixedDeltaTime;
            else if (currentSpeed < 0)
                currentSpeed += brakePower * Time.fixedDeltaTime;
        }

        // 최고속도 제한(계속 빨라지면 안되니까)
        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackSpeed, maxSpeed);

        // 이동(후진 전진)
        rb.MovePosition(rb.position + (Vector2)(transform.up * currentSpeed * Time.fixedDeltaTime));

        // ------ 회전 ------
        float rotation = -h * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }
}
