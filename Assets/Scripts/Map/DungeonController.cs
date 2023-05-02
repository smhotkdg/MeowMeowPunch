using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonController : MonoBehaviour
{
    public GameObject MinimapGFX;
    public bool isCome =false;
    public bool IsOpen = false;
    public List<GameObject> Monsters = new List<GameObject>();
    public List<bool> MonsterDestory = new List<bool>();
    private void OnEnable()
    {
        IsOpen = false;
        FindAllMonsters();
    }
    void FindAllMonsters()
    {
        Transform t = transform.Find("Tiles");
        int findIndex = 0;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Monster")
            {
                tr.gameObject.GetComponent<Monster>().MonsterIndex = findIndex;
                tr.gameObject.GetComponent<Monster>().DestoryEventHandler += DungeonController_DestoryEventHandler;
                Monsters.Add(tr.gameObject);
                findIndex++;
                MonsterDestory.Add(false);
            }
        }               
    }

    private void DungeonController_DestoryEventHandler(int position)
    {
        MonsterDestory[position] = true;
        bool allDestory = true;
        for(int i =0; i< MonsterDestory.Count;i++)
        {
            if(MonsterDestory[i]==false)
            {
                allDestory = false;
            }
        }
        if(allDestory)
        {
            Monsters.Clear();
        }
    }

    private void FixedUpdate()
    {
        if (Monsters.Count <= 0)
        {
            IsOpen = true;
        }
        MinimapGFX.SetActive(isCome);

    }
}
