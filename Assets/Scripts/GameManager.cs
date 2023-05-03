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
    public Transform HpPrefab;
    public Transform MaxHpPrefab;
    public Transform CoinPrefab;
    public Transform KeyPrefab;
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
    }
    public enum GameStatus
    {
        NOTING,
        KNOCKBACK
    }
    public GameStatus gameStatus = GameStatus.NOTING;
    public int Stage =1;
    public int coins =0;
    public int MaxHp = 3;
    public int Hp = 3;
    public int luck=0;

    public int Coin = 0;
    public int Key = 0;

    public bool AddHp(int index)
    {
        bool canAddLife = true;
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
        UIManager.Instance.SetHp();
        return canAddLife;
    }
    public bool AddMaxHp(int index)
    {
        bool canAddLife = true;
        if (MaxHp >= 10)
        {            
            canAddLife = false;
        }
        else
        {
            MaxHp += index;
            if (MaxHp >= 10)
            {
                MaxHp = 10;
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
        return canAddLife;
    }
    public enum SpawnType
    {
        Hp,
        MaxHp,
        Key,
        Coin
    }
    public void Spawn(SpawnType spawnType,Vector3 InitPos, int SpawnCount)
    {
        Transform tempObject;
        for(int i =0; i< SpawnCount; i++)
        {
            switch(spawnType)
            {
                case SpawnType.Hp:
                    tempObject = EZ_PoolManager.Spawn(HpPrefab, InitPos,new Quaternion());
                    tempObject.transform.position = InitPos;
                    break;
                case SpawnType.MaxHp:
                    tempObject = EZ_PoolManager.Spawn(MaxHpPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    break;
                case SpawnType.Coin:
                    tempObject = EZ_PoolManager.Spawn(CoinPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    break;
                case SpawnType.Key:
                    tempObject = EZ_PoolManager.Spawn(KeyPrefab, InitPos, new Quaternion());
                    tempObject.transform.position = InitPos;
                    break;
            }
        }
    }
}
