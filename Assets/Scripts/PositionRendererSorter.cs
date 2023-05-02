using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    private int srotingOrderBase = 5000;
    public int offset = 0;

    float timer;
    float timerMax = 1;
    Renderer myRenderer;

    private void Awake()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }
    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        //if (timer <= 0f)
        {
            timer = timerMax;
            myRenderer.sortingOrder = (int)(srotingOrderBase - transform.position.y - offset);
        }
    }
}
