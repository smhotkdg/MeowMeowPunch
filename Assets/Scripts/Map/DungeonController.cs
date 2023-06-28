using DungeonMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonController : MonoBehaviour
{
    public BossController bossController;
    public enum RoomType
    {
        Noraml,
        Item,
        Boss,
        Shop
    }
    public RangeSpawner rangeSpawner;
    public RoomType roomType = RoomType.Noraml;
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
    [SerializeField]
    List<int> RoomIndex = new List<int>();
    private void OnEnable()
    {
        if(isBossRoom)
        {
            bossController.onCompleteEnableHandler += BossController_onCompleteEnableHandler;
        }
        else
        {
            RoomIndex.Clear();
            IsOpen = false;
            isMakeChest = false;
            doorOpen = true;
            FindAllMonsters();
            for (int i = 0; i < DoorList.Count; i++)
            {
                DoorList[i].Play("init");
                RoomIndex.Add(0);
            }
        }
    
      
    }

    private void BossController_onCompleteEnableHandler()
    {
        RoomIndex.Clear();
        IsOpen = false;
        isMakeChest = false;
        doorOpen = true;
        FindAllMonsters();
        for (int i = 0; i < DoorList.Count; i++)
        {
            DoorList[i].Play("init");
            RoomIndex.Add(0);
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
                    RoomIndex[i] = (int)RoomType.Item;
                    //DoorList[i].Play("Treasure_Close");                    
                    DoorList[i].GetComponent<DoorController>().isItem = true;
                }
                else if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap.name == "Boss")
                {
                    RoomIndex[i] = (int)RoomType.Boss;
                    //DoorList[i].Play("Boss_Close");                    
                }
                else if (DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap.name == "Shop")
                {
                    RoomIndex[i] = (int)RoomType.Shop;
                    //DoorList[i].Play("Shop_Close");                    
                }
                else
                {
                    RoomIndex[i] = (int)RoomType.Noraml;
                    //DoorList[i].Play("Close");                    
                }
            }
        }
        doorOpen = false;
        
    }


    public void StartMonster()
    {
        for(int i =0; i< Monsters.Count; i++)
        {
            Monsters[i].gameObject.GetComponent<Monster>().rangeSpawner = rangeSpawner;
            Monsters[i].gameObject.GetComponent<Monster>().isStartMonster = true;
            Monsters[i].gameObject.GetComponent<Monster>().pObject = this.gameObject;
        }
        if(IsOpen ==false)
        {
            for (int i = 0; i < DoorList.Count; i++)
            {
                
                if (roomType == RoomType.Noraml)
                {
                    if(RoomIndex[i] == (int)RoomType.Shop)
                    {
                        DoorList[i].Play("Shop_Close");
                    }
                    else if(RoomIndex[i] == (int)RoomType.Item)
                    {
                        DoorList[i].Play("Treasure_Close");
                    }
                    else if(RoomIndex[i] == (int)RoomType.Boss)
                    {
                        DoorList[i].Play("Boss_Close");
                    }
                    else
                    {
                        DoorList[i].Play("Close");
                    }
                    
                }
                else
                {
                    switch (roomType)
                    {
                        case RoomType.Boss:
                            DoorList[i].Play("Boss_Close");
                            break;
                        case RoomType.Item:
                            DoorList[i].Play("Treasure_Close");
                            break;
                        case RoomType.Shop:
                            DoorList[i].Play("Shop_Close");
                            break;
                    }
                }
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
        bool isSpawnPossible = false;
        int randCount = 0;
        Vector2 newPosition = new Vector2();
        while (true)
        {
            newPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0) + transform.position;
            if (rangeSpawner.IsInside(newPosition))
            {
                isSpawnPossible = true;
                break;
            }
            randCount++;
            if (randCount > 100)
            {
                break;
            }
        }
        if(isSpawnPossible)
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.NoramlLootBox, newPosition, 1);
        }        
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
                    if(roomType == RoomType.Noraml)
                    {
                        if (RoomIndex[i] == (int)RoomType.Shop)
                        {
                            DoorList[i].Play("Shop_Open");
                        }
                        else if (RoomIndex[i] == (int)RoomType.Item)
                        {
                            DoorList[i].Play("Treasure_Open");
                        }
                        else if (RoomIndex[i] == (int)RoomType.Boss)
                        {
                            DoorList[i].Play("Boss_Open");
                        }
                        else
                        {
                            DoorList[i].Play("Open");
                        }
                    }
                    else
                    {
                        switch (roomType)
                        {
                            case RoomType.Boss:
                                DoorList[i].Play("Boss_Open");
                                break;
                            case RoomType.Item:
                                DoorList[i].Play("Treasure_Open");
                                break;
                            case RoomType.Shop:
                                DoorList[i].Play("Shop_Open");
                                break;
                        }
                    }
                }
                else
                {
                    if(DoorList[i].gameObject.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().isCome==true)
                    {
                        DoorList[i].Play("Treasure_Open");
                    }
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
