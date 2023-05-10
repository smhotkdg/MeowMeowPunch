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
    public bool isBossRoom = false;
    GameObject NextRoomObejct;
    GameObject ItemObject;
    public bool isMakeLootbox = false;
    private void OnEnable()
    {
        IsOpen = false;
        isMakeChest = false;
        FindAllMonsters();
    }
    public void StartMonster()
    {
        for(int i =0; i< Monsters.Count; i++)
        {
            Monsters[i].gameObject.GetComponent<Monster>().isStartMonster = true; 
        }
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
            if(isBossRoom)
            {
                if(tr.name == "NextStage")
                {
                    NextRoomObejct = tr.gameObject;
                    NextRoomObejct.SetActive(false);
                }
                if(tr.name == "Item_object")
                {
                    ItemObject = tr.gameObject;
                    ItemObject.SetActive(false);
                }
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

    bool isMakeChest = false;
    void makeChest()
    {
        Vector2 newPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f),0) + transform.position;
        GameManager.Instance.Spawn(GameManager.SpawnType.NoramlLootBox, newPosition,1);
    }
    private void FixedUpdate()
    {
        if (Monsters.Count <= 0)
        {
            IsOpen = true;
            if(isBossRoom)
            {
                NextRoomObejct.SetActive(true);
                ItemObject.SetActive(true);
            }
            if(isMakeChest ==false && isMakeLootbox==true)
            {
                makeChest();
                isMakeChest = true;
            }
        }
        if(GameManager.Instance.isVisibleMap)
        {
            MinimapGFX.SetActive(true);
        }
        else
        {
            MinimapGFX.SetActive(isCome);
        }
    }
}
