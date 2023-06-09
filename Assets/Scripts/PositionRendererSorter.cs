using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    private int srotingOrderBase = 5000;
    public int offset = 0;

    float timer;
    float timerMax = 1;
    Renderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<Renderer>();
    }
    private void LateUpdate()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = transform.GetSortingOrder();
        
    }
}
