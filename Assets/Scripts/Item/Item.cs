using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class Item : MonoBehaviour
{
    public int cost = -1;

    public bool isAdsItem = false; 
    public bool isShopItem = false;
    public ItemController itemController;
    public List<int> getList = new List<int>();
    public int item_code;
    public string m_name = "none";
    public float damage;
    public float damage_x_time;
    public float attack_speed;
    public float attack_speed_x_time;
    public float range;
    public float range_x_time;
    public float shot_speed;
    public float shot_speed_x_time;
    public float luck;
    public float curse;
    public float move_speed;
    public int tier;
    public int hp;
    public int max_hp;
    public int gold;
    public int key;
    public string attack_type = "normal";
    public string attack_method = "normal";
    public bool flying;
    public bool map;
    public bool use = true;
    public int position;
    public bool isOnce = true;
    public string other = "normal";

    SpriteRenderer spriteRenderer; 
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetIcon()
    {
        string spriteStr = "icon_" + item_code;
        if(Resources.Load<Sprite>("ItemIcon/" + spriteStr) as Sprite)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("ItemIcon/" + spriteStr) as Sprite;
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("ItemIcon/icon_1") as Sprite;
        }
        
    }
    private void OnDisable()
    {
        cost = -1;
    }
    public void GetItem()
    {
        itemController.GetItem(GetComponent<Item>());
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //GetItem();
            if(isShopItem ==false)
            {
                UIManager.Instance.ShowItemPanel(GetComponent<Item>());
            }
            else
            {
                UIManager.Instance.ShowShopItemPanel(GetItemPanel.ShopItemType.item, GetComponent<Item>(),cost);
            }
        }
    }
}
