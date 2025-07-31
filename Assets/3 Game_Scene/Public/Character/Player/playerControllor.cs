using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour    
{

    // �ٸ� û�ұ�(�����ڵ�)���׵� ����ϴ� ������ public ó���ؽ��
    public float rotationSpeed = 100f;     // ȸ�� �ӵ��Դϴ�
    public float acceleration = 5f;        // ���ӵ�
    public float brakePower = 20f;         // �극��ũ ����
    public float deceleration = 10f;       // �ڿ� ����
    public float maxSpeed = 30f;           // �ְ�ӵ�
    public float maxBackSpeed = 10f;       // ���� �ӵ�

    public float currentSpeed = 0f;        // ���� �ӵ�(������ ������ 0�̾����. �̵��ϸ鼭 ���� �ӵ� �ٲ�� ������ ����)
    
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");   // ����/���� �Է�
        float h = Input.GetAxis("Horizontal"); // ȸ�� �Է�

        // ------ ���ӵ� ó�� ------
        if (v > 0)
        {
            // ���� ����
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if(v < 0)
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
        }
        else
        {
            // �ڿ� ����
            if (currentSpeed > 0)
                currentSpeed -= deceleration * Time.fixedDeltaTime;
            else if (currentSpeed < 0)
                currentSpeed += deceleration * Time.fixedDeltaTime;
        }

        // ------ �극��ũ ó�� ------
        if (Input.GetKey(KeyCode.Space))
        {
            if (currentSpeed > 0)
                currentSpeed -= brakePower * Time.fixedDeltaTime;
            else if (currentSpeed < 0)
                currentSpeed += brakePower * Time.fixedDeltaTime;
        }

        // �ְ�ӵ� ����(��� �������� �ȵǴϱ�)
        currentSpeed = Mathf.Clamp(currentSpeed, -maxBackSpeed, maxSpeed);

        // �̵�(���� ����)
        rb.MovePosition(rb.position + (Vector2)(transform.up * currentSpeed * Time.fixedDeltaTime));

        // ------ ȸ�� ------
        float rotation = -h * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }
}
