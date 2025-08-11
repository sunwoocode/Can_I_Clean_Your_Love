using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [SerializeField] private GameObject intactObj;  // 깨지기 전 와인
    [SerializeField] private GameObject brokenObj;  // 깨진 와인

    [Header("파편 목표 위치 (로컬 기준)")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();

    [Header("파편 목표 각도 (Z축 회전)")]
    [SerializeField] private List<float> targetRotations = new List<float>();

    public PlayerHP playerHP;
    public Sturn sturn;

    private void Start()
    {
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        // 1초 동안 intact만 서서히 줄어듦
        float duration = 1f;
        float elapsed = 0f;
        Vector3 initialScale = intactObj.transform.localScale;
        Vector3 targetScale = initialScale * 0.8f;

        while (elapsed < duration)
        {
            if (intactObj != null)
                intactObj.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (intactObj != null)
            intactObj.transform.localScale = targetScale;

        // 파편으로 교체
        if (intactObj != null) intactObj.SetActive(false);
        if (brokenObj != null)
        {
            brokenObj.SetActive(true);
            StartCoroutine(MoveShards(brokenObj.transform));
        }
    }

    IEnumerator MoveShards(Transform broken)
    {
        int count = Mathf.Min(targetPositions.Count, targetRotations.Count, broken.childCount);

        Vector3[] startPositions = new Vector3[count];
        Quaternion[] startRotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            Transform shard = broken.GetChild(i);
            startPositions[i] = shard.localPosition;
            startRotations[i] = shard.localRotation;
        }

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < count; i++)
            {
                Transform shard = broken.GetChild(i);
                shard.localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                shard.localRotation = Quaternion.Lerp(startRotations[i], Quaternion.Euler(0, 0, targetRotations[i]), t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치 보정
        for (int i = 0; i < count; i++)
        {
            Transform shard = broken.GetChild(i);
            shard.localPosition = targetPositions[i];
            shard.localRotation = Quaternion.Euler(0, 0, targetRotations[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("고양이가 얍!");

            playerHP.HeartCounter();
            sturn.SturnEffect();
        }
    }
}
