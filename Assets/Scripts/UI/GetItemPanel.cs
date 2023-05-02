using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetItemPanel : MonoBehaviour
{
    public Image Icon;
    public Text TItle;
    public Text Info;

    Item selectItem;
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void setData(Item _item)
    {
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
        strinfo += "����Ÿ�� = " + _item.attack_type + "\n";
        strinfo += "���ݹ�� = " + _item.attack_method + "\n";
        if (_item.flying == true) strinfo += "����ɷ�\n";
        if (_item.map == true) strinfo += "����\n";
        Info.text = strinfo;
    }
    public void GetItme()
    {
        selectItem.itemController.GetItem(selectItem);
        Destroy(selectItem.gameObject);
    }
}
