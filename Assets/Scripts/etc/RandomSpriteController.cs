using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteController : MonoBehaviour
{
    public List<Sprite> spriteRenderers = new List<Sprite>();
    public float interval = 0.2f;

    private float timer = 0.0f;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {        
        if (timer >= interval)
        {
            spriteRenderer.sprite = spriteRenderers[Random.Range(0, spriteRenderers.Count)];
            timer = 0.0f;
        }

        timer += Time.deltaTime; 
    }
}
