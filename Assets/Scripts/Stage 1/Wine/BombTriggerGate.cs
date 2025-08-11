using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class BombTriggerGate : MonoBehaviour
{
    [Header("Spawner Prefab (FallingItemSpawner ���� ������)")]
    [SerializeField] private GameObject spawnerPrefab;   // Project ����

    [Header("���� ��ġ")]
    [SerializeField] private bool spawnAtPlayer = true;
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnZ = 0f;

    [Header("���� ����")]
    [SerializeField] private float enterDelay = 1f;      // �÷��̾� ���� �� ��� �ð�

    [Header("�ν��Ͻ� ����")]
    [SerializeField] private bool onlyOneInstance = true;   // ���� 1�� ����
    [SerializeField] private bool destroyOnExit = true;     // ������ �ı�(�Ʒ� ���� destroy�� false�� ���� �ǹ�)
    [SerializeField] private bool pauseOnExit = false;      // ������ spawner.SetPaused(true)
    [SerializeField] private bool deactivateOnExit = true;  // ������ spawned.SetActive(false)

    [Header("Ž�� ����")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string requiredPlayerName = ""; // �� ���ڿ��̸� �̸� �˻� �� ��
    [SerializeField] private bool autoSetTrigger = true;

    private GameObject spawned;     // ������ ������ �ν��Ͻ�(������ ��Ʈ)
    private Transform playerTr;     // ������ �÷��̾�
    private Coroutine waitCo;
    private bool playerInside;

    // ���������� ������ ���� ����������
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

    // ���׷��̵� UI ��� ����/�簳 ��ȣ ����
    public void SetPaused(bool pause)
    {
        if (!spawned) return;
        // �ڽĵ� �� SetPaused(bool) �޼��� ���� ������Ʈ�� ��ε�ĳ��Ʈ
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
            Debug.LogError("[BombTriggerZone] spawnerPrefab�� ����ֽ��ϴ�.");
            return;
        }

        // �̹� �ϳ� �ְ� onlyOneInstance��: �����ְų� �Ͻ������� �簳��
        if (onlyOneInstance && spawned != null)
        {
            if (!spawned.activeSelf) spawned.SetActive(true);
            SetPaused(false);
            return;
        }

        // �ߺ� ��� ����
        if (waitCo != null) StopCoroutine(waitCo);
        waitCo = StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        float t = 0f;
        while (t < enterDelay)
        {
            if (!playerInside) yield break; // ��� �� ��Ż �� ���
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

        if (waitCo != null) { StopCoroutine(waitCo); waitCo = null; } // ���� ���̸� ���

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