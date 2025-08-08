using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUIController : MonoBehaviour
{
    [SerializeField] private RectTransform catFaceUI;

    [Header("Y 좌표")]
    [SerializeField] private float yHidden = -1000f; // 항상 완전 숨는 위치
    [SerializeField] private float yWait = -400f; // 쉐도우 등장 시 대기 위치
    [SerializeField] private float yUp = -330f; // 루틴 '위'
    [SerializeField] private float yDown = -380f; // 루틴 '아래'

    [Header("속도 (px/sec)")]
    [SerializeField] private float waitSpeed = 450f; // 숨김↔대기
    [SerializeField] private float upSpeed = 300f; // 아래→위
    [SerializeField] private float downSpeed = 300f; // 위→아래
    [SerializeField] private float hideSpeed = 500f; // 어디서든 숨김으로

    [Header("루틴 옵션")]
    [SerializeField] private int loops = 5;
    [SerializeField] private float holdUp = 0.25f;
    [SerializeField] private float holdDown = 0.5f;

    [SerializeField] private bool useUnscaledTime = true;

    private enum UIState { Hidden, Waiting, Routine }
    private UIState state = UIState.Hidden;

    private Coroutine running;

    void Start()
    {
        SnapY(yHidden);
        state = UIState.Hidden;
    }

    /* -------- 외부에서 호출하는 API -------- */

    // 캣 쉐도우 등장(true) / 퇴장(false)
    public void OnShadowActiveChanged(bool isActive)
    {
        if (state == UIState.Routine) return; // 루틴 중엔 무시

        if (isActive)
        {
            if (state == UIState.Hidden) StartSolo(MoveY(yWait, waitSpeed, () => state = UIState.Waiting));
        }
        else
        {
            if (state != UIState.Hidden) StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
        }
    }

    // 공격 성공 → 루틴 실행 후 숨김
    public void OnAttackSuccess()
    {
        if (state == UIState.Routine) return;
        StartSolo(RunRoutineThenHide());
    }

    // 공격 실패 → 즉시 숨김으로
    public void OnAttackFail()
    {
        if (state == UIState.Hidden) return;
        StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
    }

    /* -------------- 내부 로직 -------------- */

    private IEnumerator RunRoutineThenHide()
    {
        state = UIState.Routine;

        // 어디서든 루틴 시작은 위 위치로
        yield return MoveY(yUp, upSpeed, null);

        for (int i = 0; i < loops; i++)
        {
            yield return MoveY(yDown, downSpeed, null);
            if (holdDown > 0) yield return Wait(holdDown);

            yield return MoveY(yUp, upSpeed, null);
            if (holdUp > 0) yield return Wait(holdUp);
        }

        // 루틴 끝나면 항상 숨김
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