using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardGroup : MonoBehaviour
{
    [Tooltip("이 그룹에 함께 먹힐 파편(Shard)의 Transform")]
    public List<Transform> shardMembers = new();

    private bool collected;

    void Awake()
    {
        // 비워두면 자식에서 자동 수집 (Shard_* 만)
        if (shardMembers.Count == 0)
        {
            foreach (Transform c in transform)
            {
                if (c.name.StartsWith("Shard_")) shardMembers.Add(c);
            }
        }
    }

    public void AbsorbAll(Transform vacuum, float suckSpeed, float shrinkSpeed, System.Action<int> onAllDone)
    {
        if (collected) return;
        collected = true;
        StartCoroutine(AbsorbRoutine(vacuum, suckSpeed, shrinkSpeed, onAllDone));
    }

    IEnumerator AbsorbRoutine(Transform vacuum, float suckSpeed, float shrinkSpeed, System.Action<int> onAllDone)
    {
        int gained = 0;
        List<Coroutine> running = new();

        foreach (var t in shardMembers)
        {
            if (!t) continue;
            gained++;
            running.Add(StartCoroutine(AbsorbOne(t, vacuum, suckSpeed, shrinkSpeed)));
        }

        foreach (var co in running) if (co != null) yield return co;

        onAllDone?.Invoke(gained);
        // 그룹 오브젝트는 더 이상 필요 없으면 제거해도 됨
        // Destroy(gameObject);
    }

    IEnumerator AbsorbOne(Transform tr, Transform vacuum, float suckSpeed, float shrinkSpeed)
    {
        if (!tr) yield break;
        Vector3 s0 = tr.localScale, s1 = Vector3.zero;
        float t = 0f;

        while (tr && tr.localScale.sqrMagnitude > 0.0001f)
        {
            tr.position   = Vector3.MoveTowards(tr.position, vacuum.position, suckSpeed * Time.deltaTime);
            tr.localScale = Vector3.Lerp(s0, s1, t);
            t += Time.deltaTime * shrinkSpeed;
            yield return null;
        }
        if (tr) Destroy(tr.gameObject);
    }
}
