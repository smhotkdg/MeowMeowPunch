using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject LobyUI;
    public GameObject GameUI;
    public Text GoldText;
    public Text KeyText;
    public GameObject GetItemPanelObject;
    public GameObject ShopItemPanelObject;
    public DamageNumber DamageNumberObject;

    private static UIManager _instance = null;

    public List<GameObject> HpList;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            else
            {
                return _instance;
            }
        }
    }
    private void Awake()
    {
        if (_instance != null)
        {
        }
        else
        {
            _instance = this;
        }
    }
    private void Start()
    {
        SetHp();
        SetKeyText();
    }
    public void SetDamageNumber(GameObject target, float _damage)
    {
        if(GameManager.Instance.ShowHpBar)
        {
            DamageNumber newDamageNumber = DamageNumberObject.Spawn(target.transform.position, _damage);
        }        
    }
    public void SetHp()
    {
        for (int i = 0; i < HpList.Count; i++)
        {
            if (i >= GameManager.Instance.MaxHp)
            {
                HpList[i].SetActive(false);
            }
            else
            {
                HpList[i].SetActive(true);
            }
            if (i < GameManager.Instance.Hp)
            {
                HpList[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
            else
            {
                HpList[i].GetComponent<Image>().color = new Color(0, 0, 0);
            }
        }
    }

    public void SetKeyText()
    {
        KeyText.text = "+ " + GameManager.Instance.Key.ToString("N0");
    }
    public void SetGoldText()
    {
        GoldText.text = "+ " + GameManager.Instance.Coin.ToString("N0");
    }
    public void SetLobyUI()
    {
        LobyUI.SetActive(true);
        GameUI.SetActive(false);
    }
    public void SetGameUI()
    {
        LobyUI.SetActive(false);
        GameUI.SetActive(true);
    }
    public void ShowItemPanel(Item _item)
    {
        GetItemPanelObject.SetActive(true);
        GetItemPanelObject.GetComponent<GetItemPanel>().setData(_item);
    }
    public void ShowShopItemPanel(GetItemPanel.ShopItemType shopItemType, Item item,int cost,GameObject SelectObj =null)
    {
        ShopItemPanelObject.SetActive(true);
        if(item !=null)
        {
            ShopItemPanelObject.GetComponent<GetItemPanel>().setData(item, cost);
        }
        else
        {
            if(shopItemType == GetItemPanel.ShopItemType.Hp)
            {
                ShopItemPanelObject.GetComponent<GetItemPanel>().SetHp(SelectObj,cost);
            }
            if (shopItemType == GetItemPanel.ShopItemType.key)
            {
                ShopItemPanelObject.GetComponent<GetItemPanel>().SetKey(SelectObj,cost);
            }
        }
    }
 
}
