using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStart : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private GameObject dialogueRoot;      // Dialogue Box 루트(패널)
    [SerializeField] private Image roseImage;               // Canvas/Rose (Image)
    [SerializeField] private TMP_Text nameText;             // Dialogue Box/Name
    [SerializeField] private TMP_Text dialogueText;         // Dialogue Box/DialogueText
    [SerializeField] private Button nextButton;             // Dialogue Box/NextButton

    [Header("Canvas Root (통째로 끌 대상)")]
    [SerializeField] private GameObject dialogueCanvasRoot; // Canvas 전체( GreyOverMask, Rose, Dialogue Box 포함 )


    [Header("Player Control")]
    [SerializeField] private VacuumController vacuumController; // 플레이어의 VacuumController

    [Header("Enable on end")]
    [SerializeField] private GameObject gameplayCanvasRoot;


    [Header("Rose Faces (index: 0=face1, 1=face2, 2=face3, 3=face4)")]
    [SerializeField] private Sprite[] faces;

    // ── 중간 표정 전환 큐 ──
    [System.Serializable]
    private class FaceCue
    {
        public int lineIndex;   // 0-based (0=첫 줄, 1=둘째 줄, 2=셋째 줄...)
        public string trigger;  // 이 문자열이 타이핑된 순간
        public int face;        // 1~4 (faces 배열은 0~3)
        [HideInInspector] public bool fired;
    }

    [SerializeField] private List<FaceCue> faceCues = new List<FaceCue>();

    [Header("Typing")]
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float charsPerSecond = 40f;

    [Header("Name (fixed)")]
    [SerializeField] private string fixedName = "로즈";

    [Header("GreyOverMask")]
    [SerializeField] private Graphic greyOverMask;        // Canvas/GreyOverMask
    [SerializeField][Range(0, 255)] private int guideMaskAlpha = 70;

    // ====== 연출 파트 ======
    [Header("Guide (camera/player)")]
    [SerializeField] private Transform player;              // Player 루트
    [SerializeField] private Camera cam;                    // Player 자식 Main Camera

    [SerializeField] private float y1 = 10.4f;              // 1차 도착 (Y만)
    [SerializeField] private float x2 = -80.7f;             // 2차 도착 (X만)
    [SerializeField] private float y3 = 15.2f;              // 마지막 도착 (Y만)
    [SerializeField] private Vector3 resetPos = new Vector3(-45.5f, -57.76f, 0f);

    [SerializeField] private float speedLeg = 10f;          // 1·2 구간 속도
    [SerializeField] private float speedFinal = 5f;         // 마지막 구간 속도
    [SerializeField] private float rotateTime = 0.12f;      // 회전 전환 시간(빠르게)

    [SerializeField] private float camSizeFrom = 5f;
    [SerializeField] private float camSizeTo = 15f;
    [SerializeField] private float camZoomTime = 0.6f;
    [SerializeField] private float startDelay = 1f;        // 마지막 대사 뜬 뒤 연출 시작까지 딜레이(1초)

    [Header("Next 버튼 색/입력 잠금")]
    [SerializeField] private Image nextButtonImage;         // 없으면 nextButton.targetGraphic 사용
    [SerializeField] private Color nextNormalColor = Color.white;                   // #FFFFFF
    [SerializeField] private Color nextDimColor = new Color(0f, 0f, 0f, 100f / 255f); // 스샷처럼 어둡게
    [SerializeField] private float unlockAfterGuideStart = 4f; // 연출 "시작" 후 4초 지나면 복구

    [Header("Player Visibility")]
    [SerializeField] private List<SpriteRenderer> playerSprites = new List<SpriteRenderer>(); // 비워두면 자동 결선
    [SerializeField, Range(0, 255)] private int hiddenAlpha = 0;
    [SerializeField, Range(0, 255)] private int visibleAlpha = 255;

    // 내부 상태
    private int i = -1;
    private bool typing;
    private Coroutine typeCo;

    private bool inputLocked = false;   // 스페이스/엔터 잠금
    private bool guidePlaying = false;  // 연출 중 여부
    private Coroutine guideCo;

    private (string text, int face)[] lines;

    private void Awake()
    {
        if (nameText) nameText.text = fixedName;

        // (빈 슬롯이면 자동 결선 — 선택)
        if (!vacuumController)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) vacuumController = p.GetComponentInChildren<VacuumController>(true);
        }

        lines = new (string, int)[]
        {
            ("내 이름은 로즈… 충전 완료.", 1),
            ("여기서 청소하는 건 처음이야.\n충전 포트에서 봤을 땐 정말 멋진 저택이었는데.", 1),
            ("백화점에선 그랬지.\n이곳엔 멋진 가전들도 있고… 사랑도 있을 거라고.", 3),
            ("자, 시작하자.\n열심히 청소하면 좋은 청소기를 만날 수 있을지도 모르니까.", 2),
            ("오늘 청소할 곳으로 가자.\n주인님의 얘기를 들었을 땐 쭉 가서 코너를 돌면 된다 그랬어.", 3),
        };

        // 3번째 대사에서 "사랑도" 타이핑되면 face4로 전환
        faceCues.Add(new FaceCue { lineIndex = 2, trigger = "사랑도", face = 4 });

        // Player 하위의 모든 SpriteRenderer 자동 결선(인스펙터에 비워뒀을 때)
        if (playerSprites.Count == 0 && player)
            playerSprites.AddRange(player.GetComponentsInChildren<SpriteRenderer>(true));
    }

    private void Start()
    {
        if (dialogueRoot) dialogueRoot.SetActive(true);
        if (nextButton) nextButton.onClick.AddListener(OnClickNext);
        if (cam && cam.orthographicSize <= 0f) cam.orthographicSize = camSizeFrom;

        i = -1;
        ShowNext();

        SetPlayerAlpha(hiddenAlpha);  // 마지막 대사 전까지 숨김(알파 0)
    }

    private void Update()
    {
        if (dialogueRoot && dialogueRoot.activeInHierarchy)
        {
            if (!inputLocked && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
                OnClickNext();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (nameText) nameText.text = fixedName;
    }
#endif

    public void OnClickNext()
    {
        // 연출 중 + 입력 허용 상태면 스킵(즉시 종료)
        if (guidePlaying && !inputLocked)
        {
            ForceFinishGuide();
            return;
        }
        ShowNext();
    }

    private void ShowNext()
    {
        if (typing) { FinishTyping(); return; }

        i++;
        if (i >= lines.Length)
        {
            EndDialogue();
            return;
        }

        var (text, face1Based) = lines[i];

        // 이 줄의 큐 상태 초기화
        foreach (var cue in faceCues)
            if (cue.lineIndex == i) cue.fired = false;

        int idx = Mathf.Clamp(face1Based - 1, 0, faces.Length - 1);
        if (roseImage && faces != null && faces.Length > 0)
            roseImage.sprite = faces[idx];

        if (!useTypewriter || dialogueText == null)
        {
            if (dialogueText) dialogueText.text = text;
        }
        else
        {
            if (typeCo != null) StopCoroutine(typeCo);
            typeCo = StartCoroutine(TypeRoutine(text));
        }

        // 마지막 대사로 넘어가자마자: 즉시 잠금 + 버튼 비활성 + 색 변경
        if (i == lines.Length - 1)
        {
            LockNextImmediate();           // 버튼/키 잠금 + 색 어둡게
            guidePlaying = true;           // 연출 구간 플래그 ON

            // 1초 뒤 연출 시작
            guideCo = StartCoroutine(GuideSequence());

            // 연출 "시작" 후 4초 지나면 입력/버튼 복구
            StartCoroutine(UnlockAfter(startDelay + unlockAfterGuideStart));
        }
    }

    private IEnumerator TypeRoutine(string full)
    {
        typing = true;
        dialogueText.text = "";
        float shown = 0f;

        // 추가: 이 줄에 해당하는 큐들을 미리 계산 ===
        int lineAtStart = i; // 시작 시점의 라인 인덱스 스냅샷
                             // (cue, endIndex) 리스트
        List<(FaceCue cue, int endIdx)> cueList = new List<(FaceCue, int)>();
        for (int ci = 0; ci < faceCues.Count; ci++)
        {
            var cue = faceCues[ci];
            if (cue.lineIndex != lineAtStart) continue;
            int pos = string.IsNullOrEmpty(cue.trigger) ? -1 : full.IndexOf(cue.trigger);
            if (pos >= 0) cueList.Add((cue, pos + cue.trigger.Length));
        }
        

        while (shown < full.Length)
        {
            shown += charsPerSecond * Time.unscaledDeltaTime;
            int len = Mathf.Clamp(Mathf.FloorToInt(shown), 0, full.Length);
            dialogueText.text = full.Substring(0, len);

            // === 추가: 큐 발사(아직 안쐈고, 길이가 트리거 끝 인덱스 이상이면) ===
            for (int k = 0; k < cueList.Count; k++)
            {
                var (cue, endIdx) = cueList[k];
                if (!cue.fired && len >= endIdx)
                {
                    cue.fired = true;
                    int fidx = Mathf.Clamp(cue.face - 1, 0, faces != null ? faces.Length - 1 : 0);
                    if (roseImage && faces != null && faces.Length > 0)
                        roseImage.sprite = faces[fidx];
                }
            }
           

            yield return null;
        }

        typing = false;
        dialogueText.text = full;

        // 추가: 안전장치(라인 끝났는데 아직 안쏜 큐가 있으면 마지막 모습으로 고정) ===
        for (int k = 0; k < cueList.Count; k++)
        {
            var (cue, _) = cueList[k];
            if (!cue.fired)
            {
                cue.fired = true;
                int fidx = Mathf.Clamp(cue.face - 1, 0, faces != null ? faces.Length - 1 : 0);
                if (roseImage && faces != null && faces.Length > 0)
                    roseImage.sprite = faces[fidx];
            }
        }
       
    }

    private void FinishTyping()
    {
        typing = false;
        if (typeCo != null) StopCoroutine(typeCo);
        dialogueText.text = lines[i].text;

        int lastPos = -1;
        FaceCue last = null;
        string full = lines[i].text;
        for (int ci = 0; ci < faceCues.Count; ci++)
        {
            var cue = faceCues[ci];
            if (cue.lineIndex != i) continue;
            int p = string.IsNullOrEmpty(cue.trigger) ? -1 : full.IndexOf(cue.trigger);
            if (p > lastPos) { lastPos = p; last = cue; }
        }
        if (last != null && roseImage && faces != null && faces.Length > 0)
        {
            int fidx = Mathf.Clamp(last.face - 1, 0, faces.Length - 1);
            roseImage.sprite = faces[fidx];
        }

    }

    private void EndDialogue()
    {
        if (vacuumController) vacuumController.enabled = true;
        if (gameplayCanvasRoot) gameplayCanvasRoot.SetActive(true);

        if (dialogueCanvasRoot) dialogueCanvasRoot.SetActive(false);
        else if (dialogueRoot) dialogueRoot.SetActive(false);

        enabled = false;
        if (nextButton) nextButton.onClick.RemoveListener(OnClickNext);
    }

    // ───────────── 입력/버튼 색 제어 ─────────────
    private void LockNextImmediate()
    {
        inputLocked = true;
        if (nextButton) nextButton.interactable = false;
        SetNextColor(nextDimColor);
    }

    private IEnumerator UnlockAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        inputLocked = false;
        if (nextButton) nextButton.interactable = true;
        SetNextColor(nextNormalColor);
    }

    private void SetNextColor(Color c)
    {
        if (nextButtonImage) nextButtonImage.color = c;
        else if (nextButton && nextButton.targetGraphic) nextButton.targetGraphic.color = c;
    }

    // ───────────── 연출 본체 ─────────────
    private IEnumerator GuideSequence()
    {
        // 마지막 대사로 넘어간 뒤 startDelay(1초) 대기
        yield return new WaitForSecondsRealtime(startDelay);

        // GreyOverMask 알파 220 적용
        SetGreyMaskAlpha(guideMaskAlpha);
        SetPlayerAlpha(visibleAlpha);


        // 카메라 줌 5 -> 10
        if (cam) yield return LerpSize(cam, Mathf.Max(cam.orthographicSize, camSizeFrom), camSizeTo, camZoomTime);


        // Y만 y1으로 (속도 10)
        Vector3 p1 = new Vector3(player.position.x, y1, player.position.z);
        yield return MoveTo(player, p1, speedLeg);

        // 회전 Z=90
        yield return RotateZ(player, 90f, rotateTime);

        // X만 x2로 (속도 10)
        Vector3 p2 = new Vector3(x2, player.position.y, player.position.z);
        yield return MoveTo(player, p2, speedLeg);

        // 회전 Z=0
        yield return RotateZ(player, 0f, rotateTime);

        // Y만 y3으로 (속도 5)
        Vector3 p3 = new Vector3(player.position.x, y3, player.position.z);
        yield return MoveTo(player, p3, speedFinal);

        // 연출은 여기서 종료(사용자가 버튼/스페이스/엔터로 마무리)
        guideCo = null;
    }

    private void ForceFinishGuide()
    {
        if (guideCo != null) { StopCoroutine(guideCo); guideCo = null; }

        SetPlayerAlpha(visibleAlpha);
        // 즉시 복구
        if (cam) cam.orthographicSize = camSizeFrom;
        if (player) player.position = resetPos;

        // 바로 튜토리얼 종료
        EndDialogue();
    }

    // ───────────── Helpers ─────────────

    private void SetPlayerAlpha(int a255)
    {
        float a = Mathf.Clamp01(a255 / 255f);
        for (int n = 0; n < playerSprites.Count; n++)
        {
            var sr = playerSprites[n];
            if (!sr) continue;
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }

    private void SetGreyMaskAlpha(int a255)
    {
        if (!greyOverMask) return;
        var c = greyOverMask.color;
        c.a = Mathf.Clamp01(a255 / 255f);
        greyOverMask.color = c;
    }

    private IEnumerator LerpSize(Camera c, float from, float to, float dur)
    {
        if (dur <= 0f) { c.orthographicSize = to; yield break; }
        float t = 0f;
        while (t < dur)
        {
            float u = Mathf.SmoothStep(0f, 1f, t / dur);
            c.orthographicSize = Mathf.LerpUnclamped(from, to, u);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        c.orthographicSize = to;
    }

    private IEnumerator MoveTo(Transform tr, Vector3 target, float speed)
    {
        const float eps = 0.01f;
        while ((tr.position - target).sqrMagnitude > eps * eps)
        {
            Vector3 dir = target - tr.position;
            float step = speed * Time.deltaTime;
            if (dir.sqrMagnitude <= step * step) { tr.position = target; break; }
            tr.position += dir.normalized * step;
            yield return null;
        }
        tr.position = target;
    }

    private IEnumerator RotateZ(Transform tr, float targetZ, float dur)
    {
        float startZ = tr.eulerAngles.z;
        if (dur <= 0f) { tr.rotation = Quaternion.Euler(0, 0, targetZ); yield break; }

        float t = 0f;
        while (t < dur)
        {
            float z = Mathf.LerpAngle(startZ, targetZ, t / dur);
            tr.rotation = Quaternion.Euler(0f, 0f, z);
            t += Time.deltaTime;
            yield return null;
        }
        tr.rotation = Quaternion.Euler(0f, 0f, targetZ);
    }
}