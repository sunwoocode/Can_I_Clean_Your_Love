using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardCollectible : MonoBehaviour
{
    public ShardGroup group;

    void Awake()
    {
        if (group == null) group = GetComponentInParent<ShardGroup>();
    }
}
