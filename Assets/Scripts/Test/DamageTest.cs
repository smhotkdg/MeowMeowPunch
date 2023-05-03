using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTest : MonoBehaviour
{
    public List<GameObject> Coins = new List<GameObject>();
    public ItemController itemController;
    public GameObject Player;
    [Button]
    public void TestLife()
    {
        GameManager.Instance.Spawn(GameManager.SpawnType.Hp, new Vector3(0,0,0), 2);
        GameManager.Instance.Spawn(GameManager.SpawnType.MaxHp, new Vector3(0, 0, 0), 2);
    }
    [Button]
    public void Test()
    {
        for (int i = 0; i < Coins.Count; i++)
        {
            Coins[i].SetActive(false);            
        }
        StartCoroutine(CoinRoutine());
    }
    IEnumerator CoinRoutine()
    {        
        for(int i =0; i < Coins.Count; i++)
        {
            Coins[i].SetActive(true);
            yield return new WaitForSeconds(.1f);
        }
    }

    [Button]
    public void TestItem(int itemIndex,Vector3 pos)
    {
        itemController.MakeItem(itemIndex,pos);        
    }
    [Button]
    public void AllItem()
    {
        Vector3 initPos = new Vector3(0, 0, 0);
        for(int i =0; i< itemController.GetMaxItemCount(); i++)
        {
            initPos.x = i * 0.5f;
            itemController.MakeItem(i, initPos);
        }
    }
}
