using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUIController : MonoBehaviour
{
    [SerializeField] private RectTransform catFaceUI;

    #region Positions
    [Header("Y 좌표")]
    [SerializeField] private float yHidden = -848f; // 항상 완전 숨는 위치
    [SerializeField] private float yWait = -645f; // 쉐도우 등장 시 대기 위치
    [SerializeField] private float yUp = -330f; // 루틴 '위'
    [SerializeField] private float yDown = -380f; // 루틴 '아래'
    #endregion

    #region Speeds
    [Header("속도 (px/sec)")]
    [SerializeField] private float waitSpeed = 450f;  // 숨김↔대기
    [SerializeField] private float upSpeed = 1200f; // 아래→위
    [SerializeField] private float downSpeed = 1200f; // 위→아래
    [SerializeField] private float hideSpeed = 500f;  // 어디서든 숨김으로
    #endregion

    #region Routine
    [Header("루틴 옵션")]
    [SerializeField] private int loops = 7;
    [SerializeField] private float holdUp = 0.05f;
    [SerializeField] private float holdDown = 0.05f;
    #endregion

    [SerializeField] private bool useUnscaledTime = true;

    private enum UIState { Hidden, Waiting, Routine }
    private UIState state = UIState.Hidden;

    // 외부에서 상태 확인용 (CatHandAttack이 성공 신호 수신 여부 판단할 때 사용)
    public bool IsInRoutine => state == UIState.Routine;

    // 깜빡임 방지 및 최소 대기시간(원하면 0으로 두면 됨)
    [SerializeField] private float hideDebounceTime = 0.15f;
    private float waitUntil = -1f; // 이 시간 전에는 숨기지 않음 (SetShadowMinWait로 설정)
    private Coroutine hideDebounce;

    private Coroutine running;

    private void Start()
    {
        SnapY(yHidden);
        state = UIState.Hidden;
    }

    /* =====================  외부에서 호출하는 API  ===================== */

    /// <summary>추격시간 동안은 숨지 않게 최소 보장시간 설정(선택)</summary>
    public void SetShadowMinWait(float seconds)
    {
        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        waitUntil = now + Mathf.Max(0f, seconds);
    }

    /// <summary>캣 쉐도우 등장/퇴장 알림. 등장 시 -848→-645를 "이동"으로 처리</summary>
    public void OnShadowActiveChanged(bool isActive)
    {
        if (state == UIState.Routine) return; // 루틴 중엔 외부 신호 무시

        if (isActive)
        {
            // 숨김 예약 중이면 취소
            if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
            if (state == UIState.Hidden)
                StartSolo(MoveY(yWait, waitSpeed, () => state = UIState.Waiting)); // 부드럽게 -645로 이동
        }
        else
        {
            // 바로 숨기지 말고 약간의 디바운스 + (설정되어 있으면) 최소 대기시간까지 기다린 뒤 숨김
            if (hideDebounce != null) StopCoroutine(hideDebounce);
            hideDebounce = StartCoroutine(HideAfterMinWait());
        }
    }

    /// <summary>공격 성공: -645에서 -330으로 올라가서 루틴 7회 후 숨김</summary>
    public void OnAttackSuccess()
    {
        if (state == UIState.Routine) return;
        if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
        StartSolo(RunRoutineThenHide());
    }

    /// <summary>공격 실패: 루틴 없이 즉시 숨김</summary>
    public void OnAttackFail()
    {
        if (state == UIState.Hidden) return;
        if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
        StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
    }

    /* =====================  내부 로직  ===================== */

    private IEnumerator HideAfterMinWait()
    {
        // 1) 짧은 디바운스
        float t = 0f;
        while (t < hideDebounceTime)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }

        // 2) 최소 대기시간 보장
        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        if (now < waitUntil)
        {
            float remain = waitUntil - now;
            float r = 0f;
            while (r < remain)
            {
                r += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
                yield return null;
            }
        }

        hideDebounce = null;

        if (state != UIState.Routine && state != UIState.Hidden)
            StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
    }

    private IEnumerator RunRoutineThenHide()
    {
        state = UIState.Routine;

        // 대기(-645) → 위(-330)으로 "이동" 후 루틴 시작
        yield return MoveY(yUp, upSpeed, null);

        for (int i = 0; i < loops; i++)
        {
            yield return MoveY(yDown, downSpeed, null);
            if (holdDown > 0f) yield return Wait(holdDown);

            yield return MoveY(yUp, upSpeed, null);
            if (holdUp > 0f) yield return Wait(holdUp);
        }

        // 숨김(-848)으로 이동
        yield return MoveY(yHidden, hideSpeed, null);
        state = UIState.Hidden;
    }

    private void StartSolo(IEnumerator routine)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Wrap(routine));
    }

    private IEnumerator Wrap(IEnumerator routine)
    {
        yield return routine;
        running = null;
    }

    private IEnumerator MoveY(float targetY, float speed, System.Action onDone)
    {
        Vector2 start = catFaceUI.anchoredPosition;
        float distance = Mathf.Abs(start.y - targetY);
        float duration = distance / Mathf.Max(0.001f, speed);

        float t = 0f;
        while (t < 1f)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / duration;
            float y = Mathf.Lerp(start.y, targetY, Mathf.Clamp01(t));
            catFaceUI.anchoredPosition = new Vector2(start.x, y);
            yield return null;
        }

        SnapY(targetY);
        onDone?.Invoke();
    }

    private void SnapY(float y)
    {
        var p = catFaceUI.anchoredPosition;
        p.y = y;
        catFaceUI.anchoredPosition = p;
    }

    private WaitForSecondsRealtime Wait(float s) => new WaitForSecondsRealtime(s);
}