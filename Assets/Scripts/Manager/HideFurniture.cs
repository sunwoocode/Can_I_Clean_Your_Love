using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideFurniture : MonoBehaviour
{
    [Header("table")]
    public SpriteRenderer table;
    public List<Sprite> tableSprite = new List<Sprite>();

    void Start()
    {
        table = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("aaa");
        if (other.gameObject.CompareTag("Player"))
        {
            table.sprite = tableSprite[1];
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            table.sprite = tableSprite[0];
        }
    }
}
