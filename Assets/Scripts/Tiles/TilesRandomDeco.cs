using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesRandomDeco : MonoBehaviour
{
    public double DisablePercent = 0.5d;
    public List<Sprite> spriteList;

    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        SetDeco();
    }

    void SetDeco()
    {
        if (spriteList.Count == 0)
            return;
        double Probablilty = 1;
        Probablilty = (1 - DisablePercent);
        
        if(GameManager.Instance.FindProbability(Probablilty))
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = spriteList[Random.Range(0, spriteList.Count)];
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
