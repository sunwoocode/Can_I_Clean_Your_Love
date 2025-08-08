using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUIController : MonoBehaviour
{
    [SerializeField] private RectTransform catFaceUI;

    [Header("Y ��ǥ")]
    [SerializeField] private float yHidden = -1000f; // �׻� ���� ���� ��ġ
    [SerializeField] private float yWait = -400f; // ������ ���� �� ��� ��ġ
    [SerializeField] private float yUp = -330f; // ��ƾ '��'
    [SerializeField] private float yDown = -380f; // ��ƾ '�Ʒ�'

    [Header("�ӵ� (px/sec)")]
    [SerializeField] private float waitSpeed = 450f; // �������
    [SerializeField] private float upSpeed = 300f; // �Ʒ�����
    [SerializeField] private float downSpeed = 300f; // ����Ʒ�
    [SerializeField] private float hideSpeed = 500f; // ��𼭵� ��������

    [Header("��ƾ �ɼ�")]
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

    /* -------- �ܺο��� ȣ���ϴ� API -------- */

    // Ĺ ������ ����(true) / ����(false)
    public void OnShadowActiveChanged(bool isActive)
    {
        if (state == UIState.Routine) return; // ��ƾ �߿� ����

        if (isActive)
        {
            if (state == UIState.Hidden) StartSolo(MoveY(yWait, waitSpeed, () => state = UIState.Waiting));
        }
        else
        {
            if (state != UIState.Hidden) StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
        }
    }

    // ���� ���� �� ��ƾ ���� �� ����
    public void OnAttackSuccess()
    {
        if (state == UIState.Routine) return;
        StartSolo(RunRoutineThenHide());
    }

    // ���� ���� �� ��� ��������
    public void OnAttackFail()
    {
        if (state == UIState.Hidden) return;
        StartSolo(MoveY(yHidden, hideSpeed, () => state = UIState.Hidden));
    }

    /* -------------- ���� ���� -------------- */

    private IEnumerator RunRoutineThenHide()
    {
        state = UIState.Routine;

        // ��𼭵� ��ƾ ������ �� ��ġ��
        yield return MoveY(yUp, upSpeed, null);

        for (int i = 0; i < loops; i++)
        {
            yield return MoveY(yDown, downSpeed, null);
            if (holdDown > 0) yield return Wait(holdDown);

            yield return MoveY(yUp, upSpeed, null);
            if (holdUp > 0) yield return Wait(holdUp);
        }

        // ��ƾ ������ �׻� ����
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