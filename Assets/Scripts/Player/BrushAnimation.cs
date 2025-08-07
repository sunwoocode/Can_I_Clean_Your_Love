using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushAnimation : MonoBehaviour         // �귯�� �ִϸ��̼�
{
    [SerializeField] private Transform leftBrush;  // Left Brush ������Ʈ ����
    [SerializeField] private Transform rightBrush; // Right Brush ������Ʈ ����

    [SerializeField] private float rotationAmplitude = 30f;  // �귯�� �ִ� ȸ�� ����
    [SerializeField] private float rotationSpeed = 20f;        // ȸ�� �ӵ�

    private float timer;

    private Quaternion leftBaseRotation;
    private Quaternion rightBaseRotation;

    void Start()
    {
        if (this.gameObject.name == "SpinBrushFront")
        {
            // ���� ȸ���� ���� (����: +45��, ������: -45��)
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
