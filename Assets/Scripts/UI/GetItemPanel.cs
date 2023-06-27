using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GetItemPanel : MonoBehaviour
{
    public enum ShopItemType
    {
        item,
        key,
        Hp
    }
    public ShopItemType itemType = ShopItemType.key;
    public bool isShop = false;
    public TextMeshProUGUI CostText;
    public Image Icon;
    public Text TItle;
    public Text Info;
    int m_cost;
    Item selectItem;
    GameObject SelectObj;
    private void OnEnable()
    {
        Time.timeScale = 0;
        
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void SetHp(GameObject itemObj, int cost)
    {
        itemType = ShopItemType.Hp;
        SelectObj = itemObj;
        if (CostText!=null)
        {
            CostText.text = cost.ToString();
            m_cost = cost;
        }
        if (Resources.Load<Sprite>("ItemIcon/Hp") as Sprite)
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/Hp") as Sprite;
        }
        else
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/icon_1") as Sprite;
        }
        TItle.text = "Hp";
        Info.text = "Hp +1";
    }
    public void SetKey(GameObject itemObj,int cost)
    {
        itemType = ShopItemType.key;
        SelectObj = itemObj;
        if (CostText != null)
        {
            CostText.text = cost.ToString();
            m_cost = cost;
        }
        if (Resources.Load<Sprite>("ItemIcon/Key") as Sprite)
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/Key") as Sprite;
        }
        else
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/icon_1") as Sprite;
        }
        TItle.text = "Key";
        Info.text = "Key +1";
    }
    public void setData(Item _item,int cost =0)
    {
        itemType = ShopItemType.item;
        selectItem = _item;
        string spriteStr = "icon_" + _item.item_code;
        if (Resources.Load<Sprite>("ItemIcon/" + spriteStr) as Sprite)
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/" + spriteStr) as Sprite;
        }
        else
        {
            Icon.sprite = Resources.Load<Sprite>("ItemIcon/icon_1") as Sprite;
        }
        TItle.text = _item.m_name;
        string strinfo = string.Empty; ;
        if (_item.damage > 0) strinfo += "���ݷ� + " + _item.damage + "\n";
        if (_item.damage_x_time > 0) strinfo += "���ݷ� ���� + " + _item.damage_x_time + "\n";
        if (_item.attack_speed > 0) strinfo += "���� �ӵ� + " + _item.attack_speed + "\n";
        if (_item.attack_speed_x_time > 0) strinfo += "���� �ӵ� ���� + " + _item.attack_speed_x_time + "\n";
        if (_item.range > 0) strinfo += "�����Ÿ� + " + _item.range + "\n";
        if (_item.range_x_time > 0) strinfo += "�����Ÿ� ���� + " + _item.range_x_time + "\n";
        if (_item.shot_speed > 0) strinfo += "�߻�ü �ӵ� + " + _item.shot_speed + "\n";
        if (_item.shot_speed_x_time > 0) strinfo += "�߻�ü �ӵ� ���� + " + _item.shot_speed_x_time + "\n";
        if (_item.luck > 0) strinfo += "��� + " + _item.luck + "\n";
        if (_item.curse > 0) strinfo += "���� + " + _item.curse + "\n";
        if (_item.move_speed > 0) strinfo += "�̵��ӵ� + " + _item.move_speed + "\n";
        if (_item.tier > 0) strinfo += "Ƽ�� = " + _item.tier + "\n";
        if (_item.hp > 0) strinfo += "ü�� + " + _item.hp + "\n";
        if (_item.max_hp > 0) strinfo += "�ִ� ü�� + " + _item.max_hp + "\n";
        if (_item.gold > 0) strinfo += "��� + " + _item.gold + "\n";
        if (_item.key > 0) strinfo += "���� + " + _item.key + "\n";
        //if (_item.attack_type != "noraml") strinfo += _item.attack_type + "\n";
        //if (_item.attack_method != "normal") strinfo += _item.attack_method + "\n";
        if (_item.other != "normal")
        {
            if(_item.other == "followItem") strinfo += "�ڼ� ������\n";
            if (_item.other == "price_50") strinfo += "���� �ݰ�\n";
            if (_item.other == "half_damage") strinfo += "�ǰݵ����� ����\n";
            if (_item.other == "money_power") strinfo += "�� = ������\n";
            if (_item.other == "random_coin") strinfo += "������ ����7��\n";
            if (_item.other == "shield_7") strinfo += "Ȯ�� ����\n";
            if (_item.other == "pickup_up") strinfo += "���� ����*2\n";
            if (_item.other == "hit_drop_item") strinfo += "�ǰݽ� Ȯ���� ����\n";
        }

        if (_item.attack_type != "normal")
            strinfo += "����Ÿ�� = " + _item.attack_type + "\n";
        if (_item.attack_method != "normal")
            strinfo += "���ݹ�� = " + _item.attack_method + "\n";
        if (_item.flying == true) strinfo += "����ɷ�\n";
        if (_item.map == true) strinfo += "����\n";
        
        
        Info.text = strinfo;
        if (CostText != null)
        {
            CostText.text = cost.ToString();
            m_cost = cost;
        }
    }
    public void GetItme()
    {
        if(isShop ==false)
        {
            selectItem.itemController.GetItem(selectItem);
            Destroy(selectItem.gameObject);
        }
        else
        {
            if(GameManager.Instance.Coin>= m_cost)
            {
                GameManager.Instance.Coin -= m_cost;
                UIManager.Instance.SetGoldText();
                switch (itemType)
                {
                    case ShopItemType.item:
                        selectItem.itemController.GetItem(selectItem);
                        Destroy(selectItem.gameObject);
                        break;
                    case ShopItemType.Hp:
                        GameManager.Instance.Spawn(GameManager.SpawnType.Hp, SelectObj.transform.position, 1);
                        Destroy(SelectObj);
                        break;
                    case ShopItemType.key:
                        GameManager.Instance.Spawn(GameManager.SpawnType.Key, SelectObj.transform.position, 1);
                        Destroy(SelectObj);
                        break;
                }
            }
            else
            {
                Debug.Log("������");
            }
           

        }
    }
}
