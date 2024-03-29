using DungeonMaker;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{    
    public enum RoomStyle
    {
        normal,
        left,
        right,
        vertical
    }

    public List<GameObject> SingList;

    public List<Sprite> DoorSingNoramlList;
    public List<Sprite> DoorSingBottomList;

    public RoomStyle roomStyle = RoomStyle.normal;

    public Vector2 LeftTopPos;
    public float xy_margin;

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
        for(int i=0; i <SingList.Count;i++)
        {
            SingList[i].SetActive(false);
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
            Monsters[i].gameObject.GetComponent<Monster>().MinimapGFX.SetActive(true);
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
                tr.gameObject.GetComponent<Monster>().MinimapGFX.SetActive(false);
                Monsters.Add(tr.gameObject);
                findMaxIndex++;
                MonsterDestory.Add(false);
            }
            if(isBossRoom)
            {
                if(tr.name == "NextStage")
                {
                    NextRoomObejct = tr.gameObject;
                    //NextRoomObejct.SetActive(false);
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
    [Button]
    void makeChest()
    {
        bool isSpawnPossible = false;
        int randCount = 0;
        Vector2 newPosition = new Vector2();
        while (true)
        {
            newPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0) + GameManager.Instance.Player.transform.position;
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

                int doorSingIndex = 0;
                switch (DoorList[i].gameObject.transform.parent.parent.name)
                {
                    case "TOP":
                        doorSingIndex = 0;
                        break;
                    case "BOTTOM":
                        doorSingIndex = 1;
                        break;
                    case "LEFT":
                        doorSingIndex = 2;
                        break;
                    case "RIGHT":
                        doorSingIndex = 3;
                        break;
                }
                SingList[doorSingIndex].SetActive(true);
                if (RoomIndex[i] == (int)RoomType.Shop)
                {
                    if(doorSingIndex !=1)
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingNoramlList[1];
                    }
                    else
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingBottomList[1];
                    }
                }
                else if (RoomIndex[i] == (int)RoomType.Item)
                {
                    if (doorSingIndex != 1)
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingNoramlList[0];
                    }
                    else
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingBottomList[0];
                    }
                }
                else if (RoomIndex[i] == (int)RoomType.Boss)
                {
                    if (doorSingIndex != 1)
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingNoramlList[3];
                    }
                    else
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingBottomList[3];
                    }
                }
                else if(RoomIndex[i] == (int)RoomType.Noraml)
                {
                    if (doorSingIndex != 1)
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingNoramlList[2];
                    }
                    else
                    {
                        SingList[doorSingIndex].GetComponent<SpriteRenderer>().sprite = DoorSingBottomList[2];
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
                //NextRoomObejct.SetActive(true);
                NextRoomObejct.GetComponent<NextStageController>().OpenNext();
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
