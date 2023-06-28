using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTest : MonoBehaviour
{
    public RangeSpawner rangeSpawner;
    public Transform Particle;
    [Button]
    public void SetParticle()
    {
        for(int i =0; i< 10; i++)
        {            
            Transform t= EZ_Pooling.EZ_PoolManager.Spawn(Particle, rangeSpawner.GetRandomPosition(), new Quaternion());
            t.GetComponent<MonsterParticleController>().SetMonster(Monster.MonsterParticleType.Creature);
        }
        
    }
    [Button]
    public void Disbaleparticle()
    {
        EZ_Pooling.EZ_PoolManager.Despawn(Particle);
    }
    public List<GameObject> Coins = new List<GameObject>();
    public ItemController itemController;
    public GameObject Player;
    public GameObject TargetObject;
    public GameObject AngleObject;
    private void Start()
    {
        //TestItem(30, new Vector3(-0.5f, 0f, 0));
        //TestItem(4, new Vector3(0, 0f, 0));
        //TestItem(46, new Vector3(0, 0.5f, 0));
        //TestItem(2, new Vector3(0.5f, 0.5f, 0));
    }
    public Vector2 GetRotateVector(Vector2 v, Vector2 pPos, float degrees)
    {
        //Vector2 newRotateVector = new Vector2();
        //newRotateVector.x = pPos.x * Mathf.Cos(Mathf.Deg2Rad*degrees) - pPos.y * Mathf.Sin(Mathf.Deg2Rad * degrees);
        //newRotateVector.y = pPos.x * Mathf.Sin(Mathf.Deg2Rad * degrees) + pPos.y + Mathf.Cos(Mathf.Deg2Rad * degrees);
        //return newRotateVector;
        float _x = v.x * Mathf.Cos(Mathf.Deg2Rad * degrees) - v.y * Mathf.Sin(Mathf.Deg2Rad * degrees);
        float _y = v.x * Mathf.Sin(Mathf.Deg2Rad * degrees) + v.y * Mathf.Cos(Mathf.Deg2Rad * degrees);
        return new Vector2(_x, _y);
    }
    [Button]
    public void AngleTest(float angle)
    {
        //AngleObject.transform.position = GetRotateVector(TargetObject.transform.position,Player.GetComponent<PlayerController>().shootController.transform.position,angle);
        Debug.Log(angle);
    }
    [Button]
    public void CheckProbablity(double chnace)
    {
        float trueCount = 0;
        float falseCount = 0;
        for(int i =0; i< 10000000; i++)
        {
            if(GameManager.Instance.FindProbability(chnace))
            {
                trueCount++;
            }
            else
            {
                falseCount++;
            }
        }
        Debug.Log( (trueCount / 10000000f)*100f + " %     " + (falseCount/ 10000000f)*100f+" % ");
        
    }
    [Button]
    public void TestCoin()
    {
        GameManager.Instance.Spawn(GameManager.SpawnType.Coin, new Vector3(0,0,0), 20);
        //GameManager.Instance.Spawn(GameManager.SpawnType.Hp, GameManager.Instance.Player.transform.localPosition, 20);

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
    public void TestItem(int itemIndex,Vector3 pos,Transform pTransfrom= null)
    {
        itemController.MakeItem(itemIndex,pos, pTransfrom);        
    }
    [Button]
    public void AllItem()
    {
        Vector3 initPos = new Vector3(0, 0, 0);
        for(int i =0; i< itemController.GetMaxItemCount(); i++)
        {
            initPos.x = i * 0.5f;
            itemController.MakeItem(i, initPos,null);
        }
    }
    [Button]
    public void MakeMaker()
    {
        MapMaker.Instance.MakeMap();
    }
    public void MakeLaser()
    {
        itemController.MakeItem(38,GameManager.Instance.Player.transform.position,null);
    }
}
