using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class BombTriggerGate : MonoBehaviour
{
    [Header("Spawner Prefab (FallingItemSpawner 붙은 프리팹)")]
    [SerializeField] private GameObject spawnerPrefab;   // Project 에셋

    [Header("생성 위치")]
    [SerializeField] private bool spawnAtPlayer = true;
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnZ = 0f;

    [Header("입장 지연")]
    [SerializeField] private float enterDelay = 1f;      // 플레이어 들어온 뒤 대기 시간

    [Header("인스턴스 관리")]
    [SerializeField] private bool onlyOneInstance = true;   // 존당 1개 유지
    [SerializeField] private bool destroyOnExit = true;     // 나가면 파괴(아래 둘은 destroy가 false일 때만 의미)
    [SerializeField] private bool pauseOnExit = false;      // 나가면 spawner.SetPaused(true)
    [SerializeField] private bool deactivateOnExit = true;  // 나가면 spawned.SetActive(false)

    [Header("탐지 설정")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string requiredPlayerName = ""; // 빈 문자열이면 이름 검사 안 함
    [SerializeField] private bool autoSetTrigger = true;

    private GameObject spawned;     // 생성된 스포너 인스턴스(프리팹 루트)
    private Transform playerTr;     // 마지막 플레이어
    private Coroutine waitCo;
    private bool playerInside;

    // ───── 에디터 보조 ─────
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }
    private void OnValidate()
    {
        if (!autoSetTrigger) return;
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger) col.isTrigger = true;
    }

    // 업그레이드 UI 등에서 멈춤/재개 신호 전달
    public void SetPaused(bool pause)
    {
        if (!spawned) return;
        // 자식들 중 SetPaused(bool) 메서드 가진 컴포넌트에 브로드캐스트
        var monos = spawned.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var m in monos)
        {
            var mi = m.GetType().GetMethod("SetPaused", new System.Type[] { typeof(bool) });
            if (mi != null) mi.Invoke(m, new object[] { pause });
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = true;
        playerTr = other.transform;

        if (spawnerPrefab == null)
        {
            Debug.LogError("[BombTriggerZone] spawnerPrefab이 비어있습니다.");
            return;
        }

        // 이미 하나 있고 onlyOneInstance면: 꺼져있거나 일시정지면 재개만
        if (onlyOneInstance && spawned != null)
        {
            if (!spawned.activeSelf) spawned.SetActive(true);
            SetPaused(false);
            return;
        }

        // 중복 대기 방지
        if (waitCo != null) StopCoroutine(waitCo);
        waitCo = StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        float t = 0f;
        while (t < enterDelay)
        {
            if (!playerInside) yield break; // 대기 중 이탈 시 취소
            t += Time.deltaTime;
            yield return null;
        }

        Vector3 pos = spawnAtPlayer && playerTr ? (Vector3)playerTr.position : transform.position;
        pos += (Vector3)spawnOffset;
        pos.z = spawnZ;

        spawned = Instantiate(spawnerPrefab, pos, Quaternion.identity, spawnParent);
        waitCo = null;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;

        if (waitCo != null) { StopCoroutine(waitCo); waitCo = null; } // 지연 중이면 취소

        if (!spawned) return;

        if (destroyOnExit)
        {
            Destroy(spawned);
            spawned = null;
        }
        else
        {
            if (pauseOnExit) SetPaused(true);
            if (deactivateOnExit && spawned.activeSelf) spawned.SetActive(false);
        }
    }

    private void OnDisable()
    {
        playerInside = false;
        if (waitCo != null) { StopCoroutine(waitCo); waitCo = null; }

        if (!spawned) return;

        if (destroyOnExit)
        {
            Destroy(spawned);
            spawned = null;
        }
        else
        {
            if (pauseOnExit) SetPaused(true);
            if (deactivateOnExit && spawned.activeSelf) spawned.SetActive(false);
        }
    }

    private bool IsPlayer(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return false;
        if (!string.IsNullOrEmpty(requiredPlayerName) && other.name != requiredPlayerName) return false;
        return true;
    }
}