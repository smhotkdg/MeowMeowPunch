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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Player")
        {
            switch(itemType)
            {
                case GetItemType.coin:
                    if (GameManager.Instance.AddCoin(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
                    }
                    break;
                  
                case GetItemType.key:
                    if (GameManager.Instance.AddKey(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
                    }
                    break;
                case GetItemType.Hp:
                    if(GameManager.Instance.AddHp(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
                    }
                    break;
                case GetItemType.MaxHp:
                    if (GameManager.Instance.AddMaxHp(ItemCount))
                    {
                        EZ_PoolManager.Despawn(transform);
                    }
                    break;                    
            }
            
        }
    }
}
