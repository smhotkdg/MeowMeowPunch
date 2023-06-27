using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
public class GetItemController : MonoBehaviour
{
    public enum GetItemType
    {
        Hp,
        coin,
        key,
        MaxHp
    }
    public int cost =-1;
    public bool isShopItem = false;
    public GameObject TriggerObject;
    public GetItemType itemType = GetItemType.coin;
    public int ItemCount = 1;
    public BounceDue bounceDue;
    float canGet = 0.5f;
    public Transform ObjectTransfrom;
    public bool isTrigger =false;
    private void Awake()
    {
        
    }
    private void OnDisable()
    {
        cost = -1;
    }
    private void OnEnable()
    {
        canGet = 0.5f;        
    }
    private void FixedUpdate()
    {
        if(isShopItem)
        {
            return;
        }
        canGet -= Time.deltaTime;
        if(canGet <=0)
        {
            canGet = 0;
        }
        if (TriggerObject != null)
        {
            TriggerObject.SetActive(GameManager.Instance.playerController.isFly);
            TriggerObject.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(isTrigger != GameManager.Instance.playerController.isFly)
            return ;
        if (canGet > 0)
        {
            return;
        }
        if (collision.gameObject.tag =="Player")
        {
            if(isShopItem==false)
            {
                GetItme();
            }
            
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isTrigger != GameManager.Instance.playerController.isFly)
            return;
        if (canGet > 0)
        {
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            if (isShopItem == false)
            {
                GetItme();
            }          

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isTrigger != GameManager.Instance.playerController.isFly)
            return;
        if (canGet > 0)
        {
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            if (isShopItem == false)
            {
                GetItme();
            }
          
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isShopItem)
        {
            if (MapMaker.Instance.isMakeEnd ==false)
                return;
            if (itemType == GetItemType.Hp)
            {
                UIManager.Instance.ShowShopItemPanel(GetItemPanel.ShopItemType.Hp, null, cost,this.gameObject);
            }
            if (itemType == GetItemType.key)
            {
                UIManager.Instance.ShowShopItemPanel(GetItemPanel.ShopItemType.key, null, cost,this.gameObject);
            }
        }
        if (isTrigger != GameManager.Instance.playerController.isFly)
            return;
       
        if (canGet > 0)
        {
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            if (isShopItem == false)
            {
                GetItme();
            }
           
        }
    }

    void GetItme()
    {
        switch (itemType)
        {
            case GetItemType.coin:
                if (GameManager.Instance.AddCoin(ItemCount))
                {
                    EZ_PoolManager.Despawn(ObjectTransfrom);
                    GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, transform.position, 1);
                }
                else
                {
                    bounceDue.isMovePlayer = false;
                }
                break;

            case GetItemType.key:
                if (GameManager.Instance.AddKey(ItemCount))
                {
                    EZ_PoolManager.Despawn(ObjectTransfrom);
                    GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, transform.position, 1);
                }
                else
                {
                    bounceDue.isMovePlayer = false;
                }
                break;
            case GetItemType.Hp:
                if (GameManager.Instance.AddHp(ItemCount))
                {
                    EZ_PoolManager.Despawn(ObjectTransfrom);
                    GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, transform.position, 1);
                }
                else
                {
                    bounceDue.isMovePlayer = false;
                }
                break;
            case GetItemType.MaxHp:
                if (GameManager.Instance.AddMaxHp(ItemCount))
                {
                    EZ_PoolManager.Despawn(ObjectTransfrom);
                    GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, transform.position, 1);
                }
                else
                {
                    bounceDue.isMovePlayer = false;
                }
                break;
        }
    }
}
