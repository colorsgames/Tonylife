using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float lifeTime;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length - 1)];
        spriteRenderer.color = new Color(1, 1, 1, Random.Range(0.3f, 1f));
        transform.rotation = Quaternion.Euler(0, 0, Random.value * 360);

        Destroy(gameObject, lifeTime);
    }
}
