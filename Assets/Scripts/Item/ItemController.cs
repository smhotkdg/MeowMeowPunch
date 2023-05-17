using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class ItemController : MonoBehaviour
{
    //Penetration 관통
    public GameObject ItemPrefab;
    public class ItemData
    {       
        public ItemController itemController;
        public List<int> getList = new List<int>();
        public int item_code;
        public string m_name = "none";
        public float damage;
        public float damage_x_time;
        public float attack_speed;
        public float attack_speed_x_time;
        public float range;
        public float range_x_time;
        public float shot_speed;
        public float shot_speed_x_time;
        public float luck;
        public float curse;
        public float move_speed;
        public int tier;
        public int hp;
        public int max_hp;
        public int gold;
        public int key;
        public string attack_type = "normal";
        public string attack_method = "normal";
        public bool flying;
        public bool map;
        public bool use = true;
        public int position;
        public bool isOnce = true;
        public string other = "normal";
    }
    public List<ItemData> items = new List<ItemData>();
    public List<ItemData> GetItems = new List<ItemData>();

    ItemController itemController;
    public delegate void OnChangeDamage(float damage,float tps,int shootCount,List<GameManager.AttackMethod> attackMethod,List<GameManager.AttackType> attackTypes,
        float range,float shootSpeed,float moveSpeed, List<GameManager.ItemOthers> ItemOthers,bool _fly);
    public event OnChangeDamage OnChangeDamageEvnetHandler;
    private static ItemController _instance = null;
    public static ItemController Instance
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
        itemController = GetComponent<ItemController>();
        SetData();
        //GetDamage();
    }

    void SetData()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("item");
        for(int i=0; i< data.Count; i++)
        {
            ItemData temp_item = new ItemData();            
            int item_code = int.Parse(data[i]["item_code"].ToString());
            string item_name = data[i]["item_name"].ToString();
            float damage = float.Parse(data[i]["damage"].ToString());
            float damage_x_time = float.Parse(data[i]["damage_x_time"].ToString());
            float attack_speed = float.Parse(data[i]["attack_speed"].ToString());
            float attack_speed_x_time = float.Parse(data[i]["attack_speed_x_time"].ToString());
            float range = float.Parse(data[i]["range"].ToString());
            float range_x_time = float.Parse(data[i]["range_x_time"].ToString());
            float shot_speed = float.Parse(data[i]["shot_speed"].ToString());
            float shot_speed_x_time = float.Parse(data[i]["shot_speed_x_time"].ToString());
            float luck = float.Parse(data[i]["luck"].ToString());
            float curse = float.Parse(data[i]["curse"].ToString());
            int tier = int.Parse(data[i]["tier"].ToString());
            float move_speed = float.Parse(data[i]["move_speed"].ToString());
            string attack_type = data[i]["attack_type"].ToString();
            string attack_method = data[i]["attack_method"].ToString();
            int hp = int.Parse(data[i]["hp"].ToString());
            int max_hp = int.Parse(data[i]["max_hp"].ToString());
            int gold = int.Parse(data[i]["gold"].ToString());
            int key = int.Parse(data[i]["key"].ToString());
            bool flying = bool.Parse(data[i]["flying"].ToString());
            bool map = bool.Parse(data[i]["map"].ToString());
            bool use = bool.Parse(data[i]["use"].ToString());
            bool once = bool.Parse(data[i]["once"].ToString());
            int position = int.Parse(data[i]["position"].ToString());
            string other = data[i]["other"].ToString();
            //string info = data[i]["info"].ToString();

            temp_item.itemController = itemController;
            temp_item.item_code = item_code;
            temp_item.m_name = item_name;
            temp_item.damage = damage;
            temp_item.damage_x_time = damage_x_time;
            temp_item.attack_speed = attack_speed;
            temp_item.attack_speed_x_time = attack_speed_x_time;
            temp_item.range = range;
            temp_item.range_x_time = range_x_time;
            temp_item.shot_speed = shot_speed;
            temp_item.shot_speed_x_time = shot_speed_x_time;
            temp_item.move_speed = move_speed;
            temp_item.luck = luck;
            temp_item.curse = curse;
            temp_item.tier = tier;
            temp_item.hp = hp;
            temp_item.max_hp = max_hp;
            temp_item.gold = gold;
            temp_item.key = key;
            temp_item.attack_type = attack_type;
            temp_item.attack_method = attack_method;
            temp_item.flying = flying;
            temp_item.map = map;
            temp_item.use = use;
            temp_item.position = position;
            temp_item.isOnce = once;
            temp_item.other = other;
            //temp_item.info = info;

            items.Add(temp_item);
        }
    }

    public void GetItem(Item item)
    {
        ItemData temp_item = new ItemData();
        {
            temp_item.itemController = item.itemController;
            temp_item.item_code = item.item_code;
            temp_item.m_name = item.m_name;
            temp_item.damage = item.damage;
            temp_item.damage_x_time = item.damage_x_time;
            temp_item.attack_speed = item.attack_speed;
            temp_item.attack_speed_x_time = item.attack_speed_x_time;
            temp_item.range = item.range;
            temp_item.range_x_time = item.range_x_time;
            temp_item.shot_speed = item.shot_speed;
            temp_item.shot_speed_x_time = item.shot_speed_x_time;
            temp_item.luck = item.luck;
            temp_item.curse = item.curse;
            temp_item.move_speed = item.move_speed;
            temp_item.tier = item.tier;
            temp_item.hp = item.hp;
            temp_item.max_hp = item.max_hp;
            temp_item.gold = item.gold;
            temp_item.key = item.key;
            temp_item.attack_type = item.attack_type;
            temp_item.attack_method = item.attack_method;
            temp_item.flying = item.flying;
            temp_item.map = item.map;
            temp_item.use = item.use;
            temp_item.position = item.position;
            temp_item.isOnce = item.isOnce;
            temp_item.other = item.other;
        }
        temp_item.getList.Add(1);
        
        GetItems.Add(temp_item);

        if(temp_item.hp>0)
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.Hp, item.gameObject.transform.position, temp_item.hp);
        }
        if(temp_item.max_hp>0)
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.MaxHp, item.gameObject.transform.position, temp_item.max_hp);
        }
        if(temp_item.gold >0)
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.Coin, item.gameObject.transform.position, temp_item.gold);
        }
        if(temp_item.key>0)
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.Key, item.gameObject.transform.position, temp_item.key);
        }
        if(temp_item.other == "random_coin")
        {
            GameManager.Instance.Spawn(GameManager.SpawnType.Coin, item.gameObject.transform.position, 7);
        }
        GameManager.Instance.Spawn(GameManager.SpawnType.GetEffect, item.gameObject.transform.position, 1);
        GameManager.Instance.isVisibleMap = temp_item.map;
        GetDamage();
    }
    public int GetMaxItemCount()
    {
        return items.Count;
    }
    public bool CanMakeItem(int itemIndex)
    {
        return items[itemIndex].use;
    }
    public void MakeItem(int itemIndex,Vector3 position,Transform pTransfrom)
    {
        //int rand = Random.Range(0, items.Count);
        int rand = itemIndex;  
        
        GameObject temp = Instantiate(ItemPrefab);
        Item temp_item =  temp.GetComponent<Item>();
        {
            temp_item.itemController = items[rand].itemController;
            temp_item.item_code = items[rand].item_code;
            temp_item.m_name = items[rand].m_name;
            temp_item.damage = items[rand].damage;
            temp_item.damage_x_time = items[rand].damage_x_time;
            temp_item.attack_speed = items[rand].attack_speed;
            temp_item.attack_speed_x_time = items[rand].attack_speed_x_time;
            temp_item.range = items[rand].range;
            temp_item.range_x_time = items[rand].range_x_time;
            temp_item.shot_speed = items[rand].shot_speed;
            temp_item.shot_speed_x_time = items[rand].shot_speed_x_time;
            temp_item.luck = items[rand].luck;
            temp_item.curse = items[rand].curse;
            temp_item.move_speed = items[rand].move_speed;
            temp_item.tier = items[rand].tier;
            temp_item.hp = items[rand].hp;
            temp_item.max_hp = items[rand].max_hp;
            temp_item.gold = items[rand].gold;
            temp_item.key = items[rand].key;
            temp_item.attack_type = items[rand].attack_type;
            temp_item.attack_method = items[rand].attack_method;
            temp_item.flying = items[rand].flying;
            temp_item.map = items[rand].map;
            temp_item.use = items[rand].use;
            temp_item.position = items[rand].position;
            temp_item.isOnce = items[rand].isOnce;
            temp_item.other = items[rand].other;
        }
        if(pTransfrom !=null)
            temp.transform.SetParent(pTransfrom);
        temp.transform.localPosition = position;
        temp_item.SetIcon();
    }
    public float GetDamage()
    {
        float v2 =0;
        float delayMod=0;
        float f1;
        float delay;
        float coins = 0;
        float razor = 0;
        float coal = 0;
        float x_time = 1;
        bool changeDelay = false;
        bool triple = false;
        bool quad = false;
        bool poly = false;
        int shootCount=1;
        //성스러운 심장
        bool Sacred = false;
        bool isSame_15 = false;
        float range = 0;
        float shootSpeed = 0;
        float moveSpeed = 0;
        float luck = 0;
        bool isMoneyPower = false;
        bool isFly = false;
        List<GameManager.AttackType> attackType= new List<GameManager.AttackType>();
        List<GameManager.AttackMethod> attackMethod = new List<GameManager.AttackMethod>();
        List<GameManager.ItemOthers> itemOthers = new List<GameManager.ItemOthers>();
        for (int i =0; i< GetItems.Count; i++)
        {
            if(GetItems[i].getList.Count >0)
            {
                isMoneyPower = false;
                if (GetItems[i].other != "normal")
                {
                    switch (GetItems[i].other)
                    {
                        case "followItem": itemOthers.Add(GameManager.ItemOthers.followItem); break;
                        case "half_damage": itemOthers.Add(GameManager.ItemOthers.half_damage); break;
                        case "money_power":
                            itemOthers.Add(GameManager.ItemOthers.money_power);
                            isMoneyPower = true; break;
                        case "shield_7": itemOthers.Add(GameManager.ItemOthers.shield_7); break;
                        case "pickup_up": itemOthers.Add(GameManager.ItemOthers.pickup_up); break;
                        case "hit_drop_item": itemOthers.Add(GameManager.ItemOthers.hit_drop_item); break;
                    }
                }
                if(GetItems[i].flying)
                {
                    isFly = GetItems[i].flying;
                }
                luck += GetItems[i].luck;
                if (GetItems[i].range>0)
                {
                    range += GetItems[i].range;
                }                
                if(GetItems[i].shot_speed>0)
                {
                    shootSpeed += GetItems[i].shot_speed;
                }
                if(GetItems[i].move_speed >0)
                {
                    moveSpeed += GetItems[i].move_speed;
                }
                if(GetItems[i].item_code == 182)
                {
                    Sacred = true;
                    delayMod += GetItems[i].attack_speed;
                    continue;
                }                
                if(isMoneyPower)
                {
                    v2 += Mathf.Min(GameManager.Instance.Coin, 99f) * 0.04f;
                }
                v2 += GetItems[i].damage;
                if(GetItems[i].damage_x_time >0)
                {
                    if(GetItems[i].item_code == 4 || GetItems[i].item_code == 12)
                    {
                        if (isSame_15 == false)
                        {
                            x_time *= GetItems[i].damage_x_time;
                            isSame_15 = true;
                        }
                    }
                    else
                    {
                        x_time *= GetItems[i].damage_x_time;
                    }
                }
                if (GetItems[i].item_code ==120)
                {
                    v2 *= 0.9f;
                    v2 -= 0.4f;
                }              
                //if(GetItems[i].other =="money_power")
                //{
                //    v2 += Mathf.Min(GameManager.Instance.coins, 99) * 0.04f;
                //}

                delayMod += GetItems[i].attack_speed;
                if(GetItems[i].item_code == 149 || GetItems[i].item_code == 2 || GetItems[i].item_code == 153 || GetItems[i].item_code == 169)
                {
                    changeDelay = true;
                }
                if (GetItems[i].item_code == 2) triple = true;
                if (GetItems[i].item_code == 153) quad = true;
                if (GetItems[i].item_code == 169) poly = true;
                if(GetItems[i].attack_type == "triple"){shootCount += 2;}
                if (GetItems[i].attack_type == "quad") { shootCount += 3; }
           

                if(GetItems[i].attack_type !="normal")
                {
                    switch(GetItems[i].attack_type)
                    {
                        case "back": attackType.Add(GameManager.AttackType.back); break;
                        case "critical": attackType.Add(GameManager.AttackType.critical); break;
                        case "cross": attackType.Add(GameManager.AttackType.cross); break;
                        case "fascination": attackType.Add(GameManager.AttackType.fascination); break;
                        case "penetration_object": attackType.Add(GameManager.AttackType.penetration_object); break;
                        case "plus_2_random": attackType.Add(GameManager.AttackType.plus_2_random); break;
                        case "poison": attackType.Add(GameManager.AttackType.posion); break;
                        case "slow": attackType.Add(GameManager.AttackType.slow); break;
                        case "split": attackType.Add(GameManager.AttackType.split); break;
                        case "stern": attackType.Add(GameManager.AttackType.stern); break;
                        case "Poly": attackType.Add(GameManager.AttackType.Poly); break;
                        case "penetration_monster":attackType.Add(GameManager.AttackType.penetration_monster); break;
                    }                    
                }
                if(GetItems[i].attack_method !="normal")
                {
                    switch(GetItems[i].attack_method)
                    {
                        case "homing": attackMethod.Add(GameManager.AttackMethod.homing);  break;
                        case "boomerang": attackMethod.Add(GameManager.AttackMethod.boomerang); break;
                        case "laser":attackMethod.Add(GameManager.AttackMethod.laser); break;
                    }
                }
              
            }            
        }
       
        f1 = Mathf.Sqrt(Mathf.Max(0, 1 + delayMod * 1.3f));
        delay = Mathf.Max(5, 16 - f1 * 6 - Mathf.Min(delayMod, 0) * 6);
        if(changeDelay)
        {
            delay = delay * 2.1f + 3;
        }
        if (delay % 1 == 0)
        {
            delay += 1;
        }
        float effectiveDelay = Mathf.Ceil(delay);
        float rof = 30 / effectiveDelay;     
        v2 = 3.5f * Mathf.Sqrt(1 + v2 * 1.2f);
        if (poly)
        {
            if (triple == true || quad == true)
            {
                v2 += 5;
            }
            else
            {
                v2 += 4;
                v2 *= 2;
            }
        }
        if(Sacred)
        {
            v2 *= 2.3f;
            v2 += 1;
        }
        v2 *= x_time;
        float dps = rof * v2;
        if (triple)
        {
            rof *= 3;
            dps *= 3;
        }
        GameManager.Instance.luck = luck;
        Debug.Log("Damage = "+v2+ "     dps = " + dps + "     tps = " + rof);

        OnChangeDamageEvnetHandler?.Invoke(v2, effectiveDelay, shootCount, attackMethod, attackType, range,shootSpeed,moveSpeed, itemOthers, isFly);
        return v2;
    }
}

