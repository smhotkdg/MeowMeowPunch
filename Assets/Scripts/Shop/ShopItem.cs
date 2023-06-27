using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public enum ItemGenreratorType
    {
        Item,
        HP,
        Key
    }
    public bool isAds =false;
    public int Cost;
    public ItemGenreratorType genreratorType = ItemGenreratorType.Item;
    public Vector3 GeneratorPosition = new Vector3(0, 0, 0);
    private void OnEnable()
    {
       switch(genreratorType)
        {
            case ItemGenreratorType.Item:
                ItemGenerator();
                break;
            case ItemGenreratorType.HP:
                HpGenerator();
                break;
            case ItemGenreratorType.Key:
                KeyGenerator();
                break;
        }
    }
    GameObject Shop_Hp;
    GameObject Shop_Key;
    void HpGenerator()
    {
        Shop_Hp = Instantiate(GameManager.Instance.HpPrefab.gameObject,transform);
        Shop_Hp.SetActive(false);
        Shop_Hp.GetComponent<BounceDue>().isBounce = false;        
        Shop_Hp.GetComponent<GetItemController>().isShopItem = true;
        Shop_Hp.GetComponent<GetItemController>().cost = Cost;        
        Shop_Hp.transform.localPosition = GeneratorPosition;
        costText.text = Cost.ToString();
        Shop_Hp.GetComponent<CircleCollider2D>().isTrigger = true;
        Shop_Hp.SetActive(true);
    }
    void KeyGenerator()
    {
        Shop_Key = Instantiate(GameManager.Instance.KeyPrefab.gameObject,transform);
        Shop_Key.SetActive(false);        
        Shop_Key.GetComponent<BounceDue>().isBounce = false;
        Shop_Key.GetComponent<GetItemController>().isShopItem = true;
        Shop_Key.GetComponent<GetItemController>().cost = Cost;        
        Shop_Key.transform.localPosition = GeneratorPosition;
        costText.text = Cost.ToString();
        Shop_Key.GetComponent<CircleCollider2D>().isTrigger = true;
        Shop_Key.SetActive(true);
    }
    void ItemGenerator()
    {
        int itemIndex = 0;
        while (true)
        {
            itemIndex = Random.Range(0, ItemController.Instance.GetMaxItemCount());
            if (ItemController.Instance.CanMakeItem(itemIndex))
            {
                break;
            }
        }
        GameObject ItemObj =  ItemController.Instance.MakeItem(itemIndex, GeneratorPosition, transform);
        if(ItemObj.GetComponent<Item>().tier == 0 || ItemObj.GetComponent<Item>().tier ==1)
        {
            Cost = 5;
        }
        else
        {
            Cost = ItemObj.GetComponent<Item>().tier * 5;
        }
        
        ItemObj.GetComponent<Item>().isShopItem = true;
        ItemObj.GetComponent<Item>().cost = Cost;
        costText.text = Cost.ToString();
    }
}
