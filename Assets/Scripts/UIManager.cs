using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameObject GetItemPanelObject;
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
    }
    public void SetDamageNumber(GameObject target, float _damage)
    {
        DamageNumber newDamageNumber = DamageNumberObject.Spawn(target.transform.position, _damage);
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
                HpList[i].GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                HpList[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
    }

    public void ShowItemPanel(Item _item)
    {
        GetItemPanelObject.SetActive(true);
        GetItemPanelObject.GetComponent<GetItemPanel>().setData(_item);
    }
}
