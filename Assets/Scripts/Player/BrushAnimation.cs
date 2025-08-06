using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushAnimation : MonoBehaviour
{
    [SerializeField] private Transform leftBrush;  // Left Brush ������Ʈ ����
    [SerializeField] private Transform rightBrush; // Right Brush ������Ʈ ����

    [SerializeField] private float rotationAmplitude = 30f;  // �귯�� �ִ� ȸ�� ����
    [SerializeField] private float rotationSpeed = 2f;        // ȸ�� �ӵ�

    private float timer;

    private Quaternion leftBaseRotation;
    private Quaternion rightBaseRotation;

    void Start()
    {
        // ���� ȸ���� ���� (����: +45��, ������: -45��)
        leftBaseRotation = Quaternion.Euler(0f, 0f, 45f);
        rightBaseRotation = Quaternion.Euler(0f, 0f, -45f);
    }
    void Update()
    {
        timer += Time.deltaTime * rotationSpeed;

        // -rotationAmplitude ~ +rotationAmplitude ������ �ε巯�� ���� ���
        float angle = Mathf.Sin(timer) * rotationAmplitude;

        // ������ �귯���� ���� �ӵ���, �ݴ� �������� ȸ���ϰ� ����
        leftBrush.localRotation = Quaternion.Euler(0f, 0f, angle);
        rightBrush.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
}
