using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatGrabAnimator : MonoBehaviour
{
    private Transform grabHand;
    private Transform shadowHand;
    private SpriteRenderer shadowRenderer;

    [Header("애니메이션 설정")]
    public float duration = 0.8f;

    private Vector2 realStartScale = new Vector2(2.5f, 2.5f);
    private Vector2 realTargetScale = new Vector2(2.0f, 2.0f);

    private Vector2 shadowStartScale = new Vector2(4.0f, 4.0f);
    private Vector2 shadowTargetScale = new Vector2(2.2f, 2.2f);

    private float shadowStartAlpha = 100f / 255f;
    private float shadowTargetAlpha = 230f / 255f;

    void Awake()
    {
        grabHand = transform.Find("Grab");
        shadowHand = transform.Find("Shadow 1");
        shadowRenderer = shadowHand.GetComponent<SpriteRenderer>();

        if (grabHand == null || shadowHand == null || shadowRenderer == null)
            Debug.LogError("Grab 또는 Shadow 1 연결 안됨. 컴포넌트 확인 ㄱ");
    }

    public void StartGrab()
    {
        StartCoroutine(AnimateGrab());
    }

    IEnumerator AnimateGrab()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            grabHand.localScale = Vector2.Lerp(realStartScale, realTargetScale, t);
            shadowHand.localScale = Vector2.Lerp(shadowStartScale, shadowTargetScale, t);

            Color col = shadowRenderer.color;
            col.a = Mathf.Lerp(shadowStartAlpha, shadowTargetAlpha, t);
            shadowRenderer.color = col;

            time += Time.deltaTime;
            yield return null;

        }

        // 보정
        grabHand.localScale = realTargetScale;
        shadowHand.localScale = shadowTargetScale;

        Color final = shadowRenderer.color;
        final.a = shadowTargetAlpha;
        shadowRenderer.color = final;
    }
}
