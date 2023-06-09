using DungeonMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonController : MonoBehaviour
{
    //public bool isInit = false;
    public double weight;
    public GameObject MinimapGFX;
    public bool isCome =false;
    public bool IsOpen = false;
    public List<GameObject> Monsters = new List<GameObject>();
    public List<bool> MonsterDestory = new List<bool>();
    public bool isBossRoom = false;
    GameObject NextRoomObejct;
    GameObject ItemObject;
    public bool isMakeLootbox = false;

    public List<Animator> DoorList; 
    private void OnEnable()
    {
        IsOpen = false;
        isMakeChest = false;
        doorOpen = true;
        FindAllMonsters();
        for (int i = 0; i < DoorList.Count; i++)
        {
            DoorList[i].Play("init");                   
        }
    }
    public void SetInitDoor()
    {
        IsOpen = false;
        for (int i = 0; i < DoorList.Count; i++)
        {            
            if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap != null)
            {
                if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap.name == "ItemRoom")
                {
                    //DoorList[i].SetTrigger("Close");
                    DoorList[i].Play("Close");
                    DoorList[i].gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    DoorList[i].GetComponent<DoorController>().isItem = true;
                }
            }
        }
        doorOpen = false;
    }


    public void StartMonster()
    {
        for(int i =0; i< Monsters.Count; i++)
        {
            Monsters[i].gameObject.GetComponent<Monster>().isStartMonster = true;
            Monsters[i].gameObject.GetComponent<Monster>().pObject = this.gameObject;
        }
        if(IsOpen ==false)
        {
            for (int i = 0; i < DoorList.Count; i++)
            {
                DoorList[i].Play("Close");
                //DoorList[i].SetTrigger("Close");
            }
        }
      
    }
    int findMaxIndex;
    public void AddObject(GameObject monsterObj)
    {
        Monsters.Add(monsterObj);
        MonsterDestory.Add(false);
        monsterObj.GetComponent<Monster>().MonsterIndex = findMaxIndex;
        monsterObj.GetComponent<Monster>().DestoryEventHandler += DungeonController_DestoryEventHandler;
        findMaxIndex++;
    }
    void FindAllMonsters()
    {
        Transform t = transform.Find("Tiles");
        int findIndex = 0;
        foreach (Transform tr in t)
        {
            if (tr.tag == "Monster")
            {
                tr.gameObject.GetComponent<Monster>().MonsterIndex = findMaxIndex;                
                tr.gameObject.GetComponent<Monster>().DestoryEventHandler += DungeonController_DestoryEventHandler;
                Monsters.Add(tr.gameObject);
                findMaxIndex++;
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
    public void DoorOpen()
    {
        doorOpen = true;
        for (int i = 0; i < DoorList.Count; i++)
        {
            if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap != null)
            {
                if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap.name != "ItemRoom")
                {
                    //DoorList[i].SetTrigger("Open");
                    DoorList[i].Play("Open");
                }                    
            }
        }
    }
    bool doorOpen = true;
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
            if(doorOpen ==false)
            {
                DoorOpen();
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
