using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushAnimation : MonoBehaviour
{
    [SerializeField] private Transform leftBrush;  // Left Brush 오브젝트 연결
    [SerializeField] private Transform rightBrush; // Right Brush 오브젝트 연결

    [SerializeField] private float rotationAmplitude = 30f;  // 브러쉬 최대 회전 각도
    [SerializeField] private float rotationSpeed = 2f;        // 회전 속도

    private float timer;

    private Quaternion leftBaseRotation;
    private Quaternion rightBaseRotation;

    void Start()
    {
        // 기준 회전값 설정 (왼쪽: +45도, 오른쪽: -45도)
        leftBaseRotation = Quaternion.Euler(0f, 0f, 45f);
        rightBaseRotation = Quaternion.Euler(0f, 0f, -45f);
    }
    void Update()
    {
        timer += Time.deltaTime * rotationSpeed;

        // -rotationAmplitude ~ +rotationAmplitude 범위의 부드러운 각도 계산
        float angle = Mathf.Sin(timer) * rotationAmplitude;

        // 각각의 브러쉬가 같은 속도로, 반대 방향으로 회전하게 설정
        leftBrush.localRotation = Quaternion.Euler(0f, 0f, angle);
        rightBrush.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
}
