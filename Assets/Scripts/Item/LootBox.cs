using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public Sprite CloseSprite;
    public Sprite OpenSprite;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    public bool isOpen = false;


    BounceDue bounceDue;
    float canGet = 0.5f;
    private void FixedUpdate()
    {
        canGet -= Time.deltaTime;
        if (canGet <= 0)
        {
            canGet = 0;
        }
    }
    private void Awake()
    {
        isOpen = false;
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = transform.Find("Box").gameObject.GetComponent<SpriteRenderer>();
        bounceDue = GetComponent<BounceDue>();
        //weightItem.Add(WeightItem.normal, 96);
        ////weightItem.Add(WeightItem.normalBox, 2);
        ////weightItem.Add(WeightItem.epicBox, 2);
        //normalItem.Add(NormalItemType.coin, 65);
        //normalItem.Add(NormalItemType.hp, 25);
        //normalItem.Add(NormalItemType.key, 10);
    }
    private void OnEnable()
    {
        canGet = 0.5f;
        isOpen = false;
        spriteRenderer.sprite = CloseSprite;
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canGet >0)
        {
            return;
        }
        if (isOpen)
        {
            return;
        }
        if (collision.gameObject.tag =="Player")
        {
            isOpen = true;
            CheckItem();
            spriteRenderer.sprite = OpenSprite;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canGet > 0)
        {
            return;
        }
        if (isOpen)
        {
            return;
        }
        if(collision.gameObject.tag =="Player")
        {
            isOpen = true;
            CheckItem();
            spriteRenderer.sprite = OpenSprite;
        }
    }    
    void GetNormalItem()
    {
        int randCount = Random.Range(2, 4);
        if(GameManager.Instance.playerController.m_ItemOthers[(int)GameManager.ItemOthers.pickup_up])
        {
            randCount = randCount * 2;
        }
        for (int i = 0; i < randCount; i++)
        {
           GameManager.NormalItemType NormalType = WeightedRandomizer.From(GameManager.Instance.normalItem).TakeOne();
            switch (NormalType)
            {
                case GameManager.NormalItemType.coin:
                    GameManager.Instance.Spawn(GameManager.SpawnType.Coin, transform.localPosition, Random.Range(1, 4));
                    break;
                case GameManager.NormalItemType.hp:
                    GameManager.Instance.Spawn(GameManager.SpawnType.Hp, transform.localPosition, 1);
                    break;
                case GameManager.NormalItemType.key:
                    GameManager.Instance.Spawn(GameManager.SpawnType.Key, transform.localPosition, 1);
                    break;

            }
        }
    }
    void CheckItem()
    {
        GameManager.WeightItem item = WeightedRandomizer.From(GameManager.Instance.weightItem).TakeOne();
        switch(item)
        {
            case GameManager.WeightItem.normal:
                GetNormalItem();                                           
                break;
        }
    }
}
