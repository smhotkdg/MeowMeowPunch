using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public LootBox pLoot;
    public bool isTrigger;
    public GameObject TriggerObejct;
    public Sprite CloseSprite;
    public Sprite OpenSprite;
    
    public SpriteRenderer spriteRenderer;
    public bool isOpen = false;


    public BounceDue bounceDue;
    float canGet = 0.5f;
    private void FixedUpdate()
    {
        canGet -= Time.deltaTime;
        if (canGet <= 0)
        {
            canGet = 0;
        }
        TriggerObejct.SetActive(GameManager.Instance.playerController.isFly);
        TriggerObejct.transform.localPosition = new Vector3(0, 0, 0);
    }
    private void Awake()
    {
        isOpen = false;
        //boxCollider = GetComponent<BoxCollider2D>();
        //spriteRenderer = transform.Find("Box").gameObject.GetComponent<SpriteRenderer>();

        //weightItem.Add(WeightItem.normal, 96);
        ////weightItem.Add(WeightItem.normalBox, 2);
        ////weightItem.Add(WeightItem.epicBox, 2);
        //normalItem.Add(NormalItemType.coin, 65);
        //normalItem.Add(NormalItemType.hp, 25);
        //normalItem.Add(NormalItemType.key, 10);
        if(isTrigger)
        {
            isOpen = pLoot.isOpen;
            if (isOpen)
            {
                spriteRenderer.sprite = OpenSprite;
            }
            else
            {
                spriteRenderer.sprite = CloseSprite;
            }
        }
       
    }
    private void OnEnable()
    {        
        canGet = 0.5f;

        if(isTrigger)
        {
            if (pLoot.isOpen == false)
            {
                isOpen = false;
                spriteRenderer.sprite = CloseSprite;
            }
        }
        else
        {
            isOpen = false;
            spriteRenderer.sprite = CloseSprite;
        }
        
    }
    void GetItem()
    {
        isOpen = true;
        CheckItem();
        spriteRenderer.sprite = OpenSprite;
    }
    bool CheckRetrun()
    {
        if (isTrigger != GameManager.Instance.playerController.isFly)
            return false;
        if (canGet > 0)
        {
            return false;
        }
        if (isOpen)
        {
            return false;
        }
        return true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(CheckRetrun())
        {
            if (collision.gameObject.tag == "Player")
            {
                GetItem();
            }
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CheckRetrun())
        {
            if (collision.gameObject.tag == "Player")
            {
                GetItem();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckRetrun())
        {
            if (collision.gameObject.tag == "Player")
            {
                GetItem();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CheckRetrun())
        {
            if (collision.gameObject.tag == "Player")
            {
                GetItem();
            }
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
                    GameManager.Instance.Spawn(GameManager.SpawnType.Coin, bounceDue.gameObject.transform.localPosition, Random.Range(1, 4));
                    break;
                case GameManager.NormalItemType.hp:
                    GameManager.Instance.Spawn(GameManager.SpawnType.Hp, bounceDue.gameObject.transform.localPosition, 1);
                    break;
                case GameManager.NormalItemType.key:
                    GameManager.Instance.Spawn(GameManager.SpawnType.Key, bounceDue.gameObject.transform.localPosition, 1);
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
