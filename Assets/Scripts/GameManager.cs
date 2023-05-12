using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public GameObject Player;
    public enum AttackType
    {        
        back,
        cross,
        slow,
        posion,
        split,
        penetration_object,
        critical,     
        plus_2_random,
        fascination,
        stern,
        Poly,
        penetration_monster
    }
    public enum AttackMethod
    {        
        homing,
        boomerang
    }
    public enum ItemOthers
    {
        followItem,
        half_damage,
        money_power,
        shield_7,
        pickup_up,
        hit_drop_item
    }
    public Transform HpPrefab;
    public Transform MaxHpPrefab;
    public Transform CoinPrefab;
    public Transform KeyPrefab;
    public Transform NormalLootBox;
    public Transform EpicLootBox;
    public Transform GetEffect;
    private static GameManager _instance = null;
    public static GameManager Instance
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
        playerController = Player.GetComponent<PlayerController>();
        InitLoot();
    }
    public enum GameStatus
    {
        NOTING,
        KNOCKBACK
    }
    public bool isVisibleMap = false;
    public GameStatus gameStatus = GameStatus.NOTING;
    public int Stage =1;
    //public int coins =0;
    public int MaxHp = 3;
    public int Hp = 3;
    public float luck=0;

    public int Coin = 0;
    public int Key = 0;


    public enum WeightItem
    {
        normal,
        normalBox,
        epicBox
    }
    public enum NormalItemType
    {
        coin,
        hp,
        key
    }
    public Dictionary<WeightItem, int> weightItem = new Dictionary<WeightItem, int>();
    public Dictionary<NormalItemType, int> normalItem = new Dictionary<NormalItemType, int>();    
    
    void InitLoot()
    {
        weightItem.Add(WeightItem.normal, 96);
        //weightItem.Add(WeightItem.normalBox, 2);
        //weightItem.Add(WeightItem.epicBox, 2);
        normalItem.Add(NormalItemType.coin, 65);
        normalItem.Add(NormalItemType.hp, 25);
        normalItem.Add(NormalItemType.key, 10);
    }

    public void GetLootItem(Vector2 pos)
    {
        NormalItemType NormalType = WeightedRandomizer.From(normalItem).TakeOne();
        switch (NormalType)
        {
            case NormalItemType.coin:
                Instance.Spawn(SpawnType.Coin, pos, 1);
                break;
            case NormalItemType.hp:
                Instance.Spawn(SpawnType.Hp, pos, 1);
                break;
            case NormalItemType.key:
                Instance.Spawn(SpawnType.Key, pos, 1);
                break;
        }
    }

    public PlayerController playerController;
    public bool AddHp(int index)
    {
        bool canAddLife = true;
        if (index >0)
        {
            
            if (Hp >= MaxHp)
            {
                Hp = MaxHp;
                canAddLife = false;
            }
            else
            {
                Hp += index;
                if (Hp >= MaxHp)
                {
                    Hp = MaxHp;
                }                
            }
        }
        else
        {
            if(playerController.m_ItemOthers[(int)ItemOthers.half_damage])
            {
                Hp -= 1;
            }
            else
            {
                Hp += index;
            }            
            if (Hp <= 0)
            {
                Hp = 0;
            }
            if (playerController.m_ItemOthers[(int)ItemOthers.hit_drop_item])
            {
                if(FindProbability(0.5d))
                {
                    GetLootItem(Player.transform.position);
                }
            }
        }
             
        UIManager.Instance.SetHp();
        return canAddLife;
    }
    public bool AddMaxHp(int index)
    {
        bool canAddLife = true;
        if (MaxHp >= 20)
        {            
            canAddLife = false;
        }
        else
        {
            MaxHp += index;
            if (MaxHp >= 20)
            {
                MaxHp = 20;
            }            
        }
        UIManager.Instance.SetHp();
        return canAddLife;
    }
    public bool AddCoin(int index)
    {
        bool canAddLife = true;
        if (Coin >= 99)
        {
            canAddLife = false;
        }
        else
        {
            Coin += index;
            if (Coin >= 99)
            {
                Coin = 99;
            }                       
        }
        if (playerController.m_ItemOthers[(int)ItemOthers.money_power])
        {
            ItemController.Instance.GetDamage();
        }
        return canAddLife;
    }
    public bool AddKey(int index)
    {
        bool canAddLife = true;
        if (Key >= 99)
        {
            canAddLife = false;
        }
        else
        {
            Key += index;
            if (Key >= 99)
            {
                Key = 99;
            }
        }
        UIManager.Instance.SetKeyText();
        return canAddLife;
    }

    public bool CheckHp(int index)
    {
        bool canAddLife = true;
        int addTemp = Hp + index;
        if (addTemp >= MaxHp)
        {            
            canAddLife = false;
        }   
        return canAddLife;
    }
    public bool CheckMaxHp(int index)
    {
        bool canAddLife = true;
        int addTemp = MaxHp + index;
        if (addTemp >= 10)
        {
            canAddLife = false;
        }             
        return canAddLife;
    }
    public bool CheckCoin(int index)
    {
        bool canAddLife = true;
        int addTemp = Coin + index;
        if (addTemp >= 99)
        {
            canAddLife = false;
        }     
        return canAddLife;
    }
    public bool CheckKey(int index)
    {
        bool canAddLife = true;
        int addTemp = Key + index;
        if (addTemp >= 99)
        {
            canAddLife = false;
        }          
        return canAddLife;
    }
    public enum SpawnType
    {
        Hp,
        MaxHp,
        Key,
        Coin,
        NoramlLootBox,
        EpicLootBox,
        GetEffect,
    }
    public void RemoveItems()
    {
        for(int i =0; i< DungeonItems.Count; i++)
        {
            EZ_PoolManager.Despawn(DungeonItems[i].transform);
        }
        DungeonItems.Clear();
    }
    public List<GameObject> DungeonItems = new List<GameObject>();
    IEnumerator SpawnRoutine(SpawnType spawnType, Vector3 InitPos, int SpawnCount)
    {
        Transform tempObject;        
        for (int i = 0; i < SpawnCount; i++)
        {
            switch (spawnType)
            {
                case SpawnType.Hp:
                    tempObject = EZ_PoolManager.Spawn(HpPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.MaxHp:
                    tempObject = EZ_PoolManager.Spawn(MaxHpPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.Coin:
                    tempObject = EZ_PoolManager.Spawn(CoinPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.Key:
                    tempObject = EZ_PoolManager.Spawn(KeyPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.NoramlLootBox:      
                    tempObject = EZ_PoolManager.Spawn(NormalLootBox, InitPos, new Quaternion());                    
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.EpicLootBox:
                    tempObject = EZ_PoolManager.Spawn(EpicLootBox, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    DungeonItems.Add(tempObject.gameObject);
                    break;
                case SpawnType.GetEffect:
                    tempObject = EZ_PoolManager.Spawn(GetEffect, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;                    
                    break;
            }
            
            yield return new WaitForSeconds(.05f);
        }
    }
    public void Spawn(SpawnType spawnType,Vector3 InitPos, int SpawnCount)
    {
        StartCoroutine(SpawnRoutine(spawnType, InitPos, SpawnCount));     
    }   
    public bool FindProbability(double chance)
    {
        // convert chance
        int target = (int)(chance * 1000f);

        // random value
        int random = Random.Range(1, 1001);

        // compare to probability range
        if (random >= 1 && random <= target)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
