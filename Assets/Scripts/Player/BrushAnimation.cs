using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushAnimation : MonoBehaviour         // 브러쉬 애니메이션
{
    [SerializeField] private Transform leftBrush;  // Left Brush 오브젝트 연결
    [SerializeField] private Transform rightBrush; // Right Brush 오브젝트 연결

    [SerializeField] private float rotationAmplitude = 30f;  // 브러쉬 최대 회전 각도
    [SerializeField] private float rotationSpeed = 20f;        // 회전 속도

    private float timer;

    private Quaternion leftBaseRotation;
    private Quaternion rightBaseRotation;

    void Start()
    {
        if (this.gameObject.name == "SpinBrushFront")
        {
            // 기준 회전값 설정 (왼쪽: +45도, 오른쪽: -45도)
            leftBaseRotation = Quaternion.Euler(0f, 0f, 45f);
            rightBaseRotation = Quaternion.Euler(0f, 0f, -45f);
        }
        else
        {
            leftBaseRotation = Quaternion.Euler(0f, 0f, -210f);
            rightBaseRotation = Quaternion.Euler(0f, 0f, 210f);
        }

        
    }

    void Update()
    {
        timer += Time.deltaTime * rotationSpeed;

        float angle = Mathf.Sin(timer) * rotationAmplitude;

        leftBrush.localRotation = leftBaseRotation * Quaternion.Euler(0f, 0f, angle);
        rightBrush.localRotation = rightBaseRotation * Quaternion.Euler(0f, 0f, -angle);
    }
}
