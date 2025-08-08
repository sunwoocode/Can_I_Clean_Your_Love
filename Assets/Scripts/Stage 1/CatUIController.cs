using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUIController : MonoBehaviour
{
    [SerializeField] private RectTransform catFaceUI;

    #region Positions
    [Header("Y ��ǥ")]
    [SerializeField] private float yHidden = -848f; // �׻� ���� ���� ��ġ
    [SerializeField] private float yWait = -645f; // ������ ���� �� ��� ��ġ
    [SerializeField] private float yUp = -330f; // ��ƾ '��'
    [SerializeField] private float yDown = -380f; // ��ƾ '�Ʒ�'
    #endregion

    #region Speeds
    [Header("�ӵ� (px/sec)")]
    [SerializeField] private float waitSpeed = 450f;  // �������
    [SerializeField] private float upSpeed = 1200f; // �Ʒ�����
    [SerializeField] private float downSpeed = 1200f; // ����Ʒ�
    [SerializeField] private float hideSpeed = 500f;  // ��𼭵� ��������
    #endregion

    #region Routine
    [Header("��ƾ �ɼ�")]
    [SerializeField] private int loops = 7;
    [SerializeField] private float holdUp = 0.05f;
    [SerializeField] private float holdDown = 0.05f;
    #endregion

    [SerializeField] private bool useUnscaledTime = true;

    private enum UIState { Hidden, Waiting, Routine }
    private UIState state = UIState.Hidden;

    // �ܺο��� ���� Ȯ�ο� (CatHandAttack�� ���� ��ȣ ���� ���� �Ǵ��� �� ���)
    public bool IsInRoutine => state == UIState.Routine;

    // ������ ���� �� �ּ� ���ð�(���ϸ� 0���� �θ� ��)
    [SerializeField] private float hideDebounceTime = 0.15f;
    private float waitUntil = -1f; // �� �ð� ������ ������ ���� (SetShadowMinWait�� ����)
    private Coroutine hideDebounce;

    private Coroutine running;

    private void Start()
    {
        SnapY(yHidden);
        state = UIState.Hidden;
    }

    /* =====================  �ܺο��� ȣ���ϴ� API  ===================== */

    /// <summary>�߰ݽð� ������ ���� �ʰ� �ּ� ����ð� ����(����)</summary>
    public void SetShadowMinWait(float seconds)
    {
        float now = useUnscaledTime ? Time.unscaledTime : Time.time;
        waitUntil = now + Mathf.Max(0f, seconds);
    }

    /// <summary>Ĺ ������ ����/���� �˸�. ���� �� -848��-645�� "�̵�"���� ó��</summary>
    public void OnShadowActiveChanged(bool isActive)
    {
        if (state == UIState.Routine) return; // ��ƾ �߿� �ܺ� ��ȣ ����

        if (isActive)
        {
            // ���� ���� ���̸� ���
            if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
            if (state == UIState.Hidden)
                StartSolo(MoveY(yWait, waitSpeed, () => state = UIState.Waiting)); // �ε巴�� -645�� �̵�
        }
        else
        {
            // �ٷ� ������ ���� �ణ�� ��ٿ + (�����Ǿ� ������) �ּ� ���ð����� ��ٸ� �� ����
            if (hideDebounce != null) StopCoroutine(hideDebounce);
            hideDebounce = StartCoroutine(HideAfterMinWait());
        }
    }

    /// <summary>���� ����: -645���� -330���� �ö󰡼� ��ƾ 7ȸ �� ����</summary>
    public void OnAttackSuccess()
    {
        if (state == UIState.Routine) return;
        if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
        StartSolo(RunRoutineThenHide());
    }

    /// <summary>���� ����: ��ƾ ���� ��� ����</summary>
    public void OnAttackFail()
    {
        if (state == UIState.Hidden) return;
        if (hideDebounce != null) { StopCoroutine(hideDebounce); hideDebounce = null; }
        StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
    }

    /* =====================  ���� ����  ===================== */

    private IEnumerator HideAfterMinWait()
    {
        // 1) ª�� ��ٿ
        float t = 0f;
        while (t < hideDebounceTime)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }

        // 2) �ּ� ���ð� ����
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

        // ���(-645) �� ��(-330)���� "�̵�" �� ��ƾ ����
        yield return MoveY(yUp, upSpeed, null);

        for (int i = 0; i < loops; i++)
        {
            yield return MoveY(yDown, downSpeed, null);
            if (holdDown > 0f) yield return Wait(holdDown);

            yield return MoveY(yUp, upSpeed, null);
            if (holdUp > 0f) yield return Wait(holdUp);
        }

        // ����(-848)���� �̵�
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