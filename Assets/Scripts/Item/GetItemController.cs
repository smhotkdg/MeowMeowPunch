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

    public GetItemType itemType = GetItemType.coin;
    public int ItemCount = 1;
    BounceDue bounceDue;
    float canGet = 0.5f;
    private void Awake()
    {
        bounceDue = GetComponent<BounceDue>();
    }
    private void OnEnable()
    {
        canGet = 0.5f;
    }
    private void FixedUpdate()
    {
        canGet -= Time.deltaTime;
        if(canGet <=0)
        {
            canGet = 0;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canGet >0)
        {
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            switch (itemType)
            {
                case GetItemType.coin:
                    if (GameManager.Instance.AddCoin(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
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
                        EZ_PoolManager.Despawn(transform);
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
                        EZ_PoolManager.Despawn(transform);
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
                        EZ_PoolManager.Despawn(transform);
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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (canGet > 0)
        {
            return;
        }
        if (collision.gameObject.tag =="Player")
        {
            switch(itemType)
            {
                case GetItemType.coin:
                    if (GameManager.Instance.AddCoin(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
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
                        EZ_PoolManager.Despawn(transform);
                        GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, transform.position, 1);
                    }
                    else
                    {
                        bounceDue.isMovePlayer = false;
                    }
                    break;
                case GetItemType.Hp:
                    if(GameManager.Instance.AddHp(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
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
                        EZ_PoolManager.Despawn(transform);
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
}
