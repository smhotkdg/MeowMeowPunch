using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColider : MonoBehaviour
{
    public Monster.Status status = Monster.Status.Normal;
    public float xMargin = 0.2f;
    public bool isPenetration_object = false;
    public bool isPenetation_monster = false;
    public bool isSplit = false;
    public float Power = 1;
    //public GameObject Target;
    public bool isDisable = false;
    public bool isCritical = false;


    public GameObject PrevTarget;

    public delegate void OnObstacleEnter(Vector3 collisionPos);
    public event OnObstacleEnter OnObstacleEnterEventHandler;

    public Vector2 initPos;
 
    public float knockbackForce = 2;
    public bool isEnable = false;
    public Color DamangeColor;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnable == false)
            return;
        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "Room_wall")
        {            
            if (isSplit)
            {
                Split(collision.gameObject);
                isSplit = false;
            }
            if (isDisable == true)
            {
                EZ_Pooling.EZ_PoolManager.Despawn(transform);
            }
        }
        if (collision.tag == "Monster" && collision.gameObject != PrevTarget)
        {
            float _damage = Power;
            if(isCritical)
            {
                _damage = Power * 3.2f;
            }
            Vector2 direction = (collision.transform.position- transform.position).normalized;
            float randPower = Random.Range(1.5f, 3f);
            Vector2 knocback = direction * randPower;            
                        
            if (knockbackForce > 0)
            {
                collision.GetComponent<Monster>().SetDamage(_damage, knocback,status, DamangeColor); 
            }
            else
            {
                collision.GetComponent<Monster>().SetDamage(_damage, new Vector2(0,0), status, DamangeColor);
            }
            

            UIManager.Instance.SetDamageNumber(collision.gameObject, _damage);

            if (isSplit)
            {   
                Split(collision.gameObject);
                isSplit = false;
            }
            if (isDisable==true && isPenetation_monster ==false)
            {
                EZ_Pooling.EZ_PoolManager.Despawn(transform);
            }   
        }
        if (collision.tag == "Obstacle" && collision.gameObject != PrevTarget)
        {
            if (isSplit)
            {
                Split(collision.gameObject);
                isSplit = false;
            }
            if (isDisable ==true && isPenetration_object==false)
            {               
                EZ_Pooling.EZ_PoolManager.Despawn(transform);
            }
        }
    } 
  
    Vector2 SetLIner(GameObject target,float margin)
    {
        float y1 = initPos.y;
        float x1 = initPos.x;
        float y2 = target.transform.position.y;
        float x2 = target.transform.position.x;
        float a, b;

        a = (y2 - y1) / (x2 - x1);
        b = (x2 * y1 - x1 * y2) / (x2 - x1);

        float temp = (x2 + margin) * a + b;

        return new Vector2(x2+ margin, temp);
    } 
    void Split(GameObject target)
    {
        GameObject bullet = GetComponent<Bullet>().gameObject;
        Vector3 direction = new Vector3();
        Vector3 direction_split = new Vector3();
        float randDiv = Random.Range(1f, 4f);
        switch (bullet.GetComponent<Bullet>().bulletDirection)
        {
            case Bullet.BulletDirection.forward:
                direction = bullet.transform.up;
                randDiv = Random.Range(1f, 4f);
                direction_split = bullet.transform.right / randDiv;
                break;
            case Bullet.BulletDirection.back:
                direction = -bullet.transform.up;
                randDiv = Random.Range(1f, 4f);
                direction_split = bullet.transform.right / randDiv;
                break;
            case Bullet.BulletDirection.left:
                direction = -bullet.transform.right;
                randDiv = Random.Range(1f, 4f);
                direction_split = bullet.transform.up / randDiv;
                break;
            case Bullet.BulletDirection.right:
                direction = bullet.transform.right;
                randDiv = Random.Range(1f, 4f);
                direction_split = bullet.transform.up / randDiv;
                break;
        }
        Transform temp =  EZ_Pooling.EZ_PoolManager.Spawn(bullet.transform, target.transform.position, new Quaternion());
        temp.GetComponent<Bullet>().SetAttackMethods(GetComponent<Bullet>().m_attackMethods, GetComponent<Bullet>().m_Target);
        temp.GetComponent<Bullet>().bulletDirection = GetComponent<Bullet>().bulletDirection;
        temp.GetComponent<Bullet>().SetSplit(target);
        temp.GetComponent<Bullet>().isSplit = false;
        temp.transform.localScale = temp.transform.localScale / 2;
        
        Vector2 moveVec = direction + direction_split;
        temp.GetComponent<Rigidbody2D>().velocity = moveVec * GetComponent<Bullet>().speed;


        Transform temp_2 = EZ_Pooling.EZ_PoolManager.Spawn(bullet.transform, target.transform.position, new Quaternion());
        temp_2.GetComponent<Bullet>().SetAttackMethods(GetComponent<Bullet>().m_attackMethods, GetComponent<Bullet>().m_Target);
        temp_2.GetComponent<Bullet>().bulletDirection = GetComponent<Bullet>().bulletDirection;
        temp_2.GetComponent<Bullet>().SetSplit(target);        
        moveVec = direction - direction_split;
        
        temp_2.transform.localScale = temp_2.transform.localScale / 2;
        temp_2.GetComponent<Bullet>().isSplit = false;
        temp_2.GetComponent<Rigidbody2D>().velocity = moveVec * GetComponent<Bullet>().speed;
    }
}
