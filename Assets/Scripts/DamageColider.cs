using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColider : MonoBehaviour
{
    public Transform SternBullet;
    public enum BulletType
    {
        player,
        monster
    }
    public Vector2 initScale;
    public List<bool> m_attackTypes = new List<bool>();
    public List<bool> m_attackMethods = new List<bool>();
    public List<bool> m_attackTypes_Bullets = new List<bool>();
    public List<bool> m_attackTypes_Bullets_before = new List<bool>();
    public float defaultKncokback;
    public Rigidbody2D rb;
    public BulletType bulletType = BulletType.player;
    public Monster.Status status = Monster.Status.Normal;
    public float xMargin = 0.2f;
    public bool isPenetration_object = false;
    public bool isPenetation_monster = false;
    public bool isSplit = false;
    public float Power = 1;
    //public GameObject Target;
    public bool isDisable = false;
    public bool isCritical = false;

    public Animator animator; 
    public GameObject PrevTarget;

    //public delegate void OnObstacleEnter(Vector3 collisionPos);
    //public event OnObstacleEnter OnObstacleEnterEventHandler;

    public Vector2 initPos;

    public float knockbackForce = 2;
    public bool isEnable = false;
    public Color DamangeColor;    
    public Transform bulletTransform;
    public bool Hit = false;

    private void OnEnable()
    {
        bulletType = BulletType.player;
        Hit = false;
        isStopMovement = false;
        //Vector2 dir = transform.GetComponent<Rigidbody2D>().velocity;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
    public virtual void OnObstacleEnterLaser(Vector3 HitPoint)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnable == false)
            return;

        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "Room_wall")
        {

            if (isSplit && GameManager.Instance.playerController.shootType ==
                     PlayerController.ShootType.normal)
            {
                Split(collision.gameObject);
                isSplit = false;
            }
            if (isDisable == true)
            {
                if (animator != null)
                {
                    rb.velocity = new Vector2(0, 0);
                    animator.SetTrigger("Hit");
                }
                else
                {
                    EZ_Pooling.EZ_PoolManager.Despawn(transform);
                }
                StopBullet();
            }
            OnObstacleEnterLaser(collision.ClosestPoint(transform.position));
            Hit = true;
        }
        if (bulletType == BulletType.player)
        {
            if (collision.tag == "Monster" && collision.gameObject != PrevTarget)
            {
                float _damage = Power;
                if (isCritical)
                {
                    _damage = Power * 3.2f;
                }
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                float randPower = Random.Range(1.5f, 3f);
                Vector2 knocback = direction * randPower;

                if (knockbackForce > 0)
                {                    
                    collision.GetComponent<Monster>().SetDamage(_damage, knocback, status, DamangeColor);
                }
                else
                {
                    collision.GetComponent<Monster>().SetDamage(_damage, new Vector2(0, 0), status, DamangeColor);
                }


                UIManager.Instance.SetDamageNumber(collision.gameObject, _damage);

                if (isSplit && GameManager.Instance.playerController.shootType ==
                     PlayerController.ShootType.normal)
                {
                    Split(collision.gameObject);
                    isSplit = false;
                }
                if (isDisable == true && isPenetation_monster == false)
                {
                    if (animator != null)
                    {
                        rb.velocity = new Vector2(0, 0);
                        animator.SetTrigger("Hit");
                    }
                    else
                    {
                        EZ_Pooling.EZ_PoolManager.Despawn(transform);
                    }
                    StopBullet();
                    //EZ_Pooling.EZ_PoolManager.Despawn(transform);                    
                }

                Hit = true;
            }

            if (collision.tag == "Obstacle" && collision.gameObject != PrevTarget)
            {
                if (isSplit && GameManager.Instance.playerController.shootType ==
                     PlayerController.ShootType.normal)
                {
                    Split(collision.gameObject);
                    isSplit = false;
                }
                if (isDisable == true && isPenetration_object == false)
                {
                    if (animator != null)
                    {
                        rb.velocity = new Vector2(0, 0);
                        animator.SetTrigger("Hit");
                    }
                    else
                    {
                        EZ_Pooling.EZ_PoolManager.Despawn(transform);
                    }
                    StopBullet();
                    //EZ_Pooling.EZ_PoolManager.Despawn(transform);                    
                }

                OnObstacleEnterLaser(collision.ClosestPoint(transform.position));
                Hit = true;
            }
        }
        else
        {
            if (collision.tag == "Player")
            {
                int _damagePlayer = (int)Power;

                Vector2 direction = (collision.transform.position - GameManager.Instance.Player.transform.position).normalized;
                float randPower = Random.Range(1.5f, 3f);
                Vector2 knocback = direction * randPower;

                collision.gameObject.GetComponent<PlayerController>().Knockback(transform.position, -_damagePlayer);

                if (isDisable == true)
                {
                    if (animator != null)
                    {
                        rb.velocity = new Vector2(0, 0);
                        animator.SetTrigger("Hit");
                    }
                    else
                    {
                        EZ_Pooling.EZ_PoolManager.Despawn(transform);
                    }
                    StopBullet();
                    //EZ_Pooling.EZ_PoolManager.Despawn(transform);
                }
                Hit = true;
            }
            if (collision.tag == "Obstacle")
            {
                if (isDisable == true && isPenetration_object == false)
                {
                    if (animator != null)
                    {
                        rb.velocity = new Vector2(0, 0);
                        animator.SetTrigger("Hit");
                    }
                    else
                    {
                        EZ_Pooling.EZ_PoolManager.Despawn(transform);
                    }
                    StopBullet();
                    //EZ_Pooling.EZ_PoolManager.Despawn(transform);
                }
            }
            Hit = true;
        }

        
    }
    public bool isStopMovement = false;
    public void StopBullet()
    {
        isStopMovement = true;
    }

    Vector2 SetLIner(GameObject target, float margin)
    {
        float y1 = initPos.y;
        float x1 = initPos.x;
        float y2 = target.transform.position.y;
        float x2 = target.transform.position.x;
        float a, b;

        a = (y2 - y1) / (x2 - x1);
        b = (x2 * y1 - x1 * y2) / (x2 - x1);

        float temp = (x2 + margin) * a + b;

        return new Vector2(x2 + margin, temp);
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

        Vector3 moveVec = direction + direction_split;

        Transform temp = EZ_Pooling.EZ_PoolManager.Spawn(bullet.transform, target.transform.position, new Quaternion());
        temp.GetComponent<Bullet>().SetAttackMethods(GetComponent<Bullet>().m_attackMethods, GetComponent<Bullet>().m_Target);
        temp.GetComponent<Bullet>().bulletDirection = GetComponent<Bullet>().bulletDirection;
        temp.GetComponent<Bullet>().SetSplit(target);
        temp.GetComponent<Bullet>().isSplit = false;
        temp.transform.localScale = temp.transform.localScale / 2;
        temp.GetComponent<Rigidbody2D>().velocity = moveVec * GetComponent<Bullet>().speed;

        Vector2 dir = temp.GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        temp.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);




        moveVec = direction - direction_split;

        Transform temp_2 = EZ_Pooling.EZ_PoolManager.Spawn(bullet.transform, target.transform.position, Quaternion.AngleAxis(angle - 90, Vector3.forward));
        temp_2.GetComponent<Bullet>().SetAttackMethods(GetComponent<Bullet>().m_attackMethods, GetComponent<Bullet>().m_Target);
        temp_2.GetComponent<Bullet>().bulletDirection = GetComponent<Bullet>().bulletDirection;
        temp_2.GetComponent<Bullet>().SetSplit(target);

        temp_2.transform.localScale = temp_2.transform.localScale / 2;
        temp_2.GetComponent<Bullet>().isSplit = false;
        temp_2.GetComponent<Rigidbody2D>().velocity = moveVec * GetComponent<Bullet>().speed;

        dir = temp_2.GetComponent<Rigidbody2D>().velocity;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        temp_2.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public bool CheckStern()
    {
        bool isStern = false;

        double sternProbability = (GameManager.Instance.luck * 0.033d) + 0.1d;
        if (GameManager.Instance.FindProbability(sternProbability))
        {
            isStern = true;
        }
        return isStern;
    }
    public bool CheckFascination()
    {
        bool isFascination = false;
        double FascinationProbability = (GameManager.Instance.luck * 0.033d) + 0.1d;
        if (GameManager.Instance.FindProbability(FascinationProbability))
        {
            isFascination = true;
        }
        return isFascination;
    }

    public bool CheckCritical()
    {
        bool isCirtical = false;
        double criticlaValue = (GameManager.Instance.luck + 1) * 0.1d;
        if (GameManager.Instance.FindProbability(criticlaValue))
        {
            isCirtical = true;
        }
        return isCirtical;
    }
    public bool CheckPosion()
    {
        bool isPosion = false;
        double PosionProbability = (GameManager.Instance.luck * 0.05d) + 0.625d;
        if (GameManager.Instance.FindProbability(PosionProbability))
        {
            isPosion = true;
        }
        return isPosion;
    }
    public bool CheckSlow()
    {
        bool isSlow = false;
        double slowProbability = (GameManager.Instance.luck * 0.05d) + 0.25d;
        if (GameManager.Instance.FindProbability(slowProbability))
        {
            isSlow = true;
        }
        return isSlow;
    }
    bool CheckAllFalse = false;
    public void CheckEffectBullet()
    {
        status = Monster.Status.Normal;
        knockbackForce = defaultKncokback;
        isPenetration_object = false;
        isSplit = false;
        isCritical = false;
        for (int i = 0; i < m_attackTypes.Count; i++)
        {
            if (m_attackTypes[i] == true)
            {
                switch (i)
                {
                    case (int)GameManager.AttackType.critical:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.critical] = false;
                        if (CheckCritical())
                        {
                            m_attackTypes_Bullets[(int)GameManager.AttackType.critical] = true;
                        }
                        break;
                    case (int)GameManager.AttackType.fascination:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.fascination] = false;
                        if (CheckFascination())
                        {
                            m_attackTypes_Bullets[(int)GameManager.AttackType.fascination] = true;
                        }
                        break;
                    case (int)GameManager.AttackType.penetration_object: isPenetration_object = true; break;
                    case (int)GameManager.AttackType.plus_2_random:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.plus_2_random] = false;
                        m_attackTypes_Bullets[(int)GameManager.AttackType.plus_2_random] = true;
                        break;
                    case (int)GameManager.AttackType.posion:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.posion] = false;
                        if (CheckPosion())
                        {
                            m_attackTypes_Bullets[(int)GameManager.AttackType.posion] = true;
                        }
                        break;
                    case (int)GameManager.AttackType.slow:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.slow] = false;
                        if (CheckSlow())
                        {
                            m_attackTypes_Bullets[(int)GameManager.AttackType.slow] = true;
                        }
                        break;
                    case (int)GameManager.AttackType.split: isSplit = true; break;
                    case (int)GameManager.AttackType.stern:
                        m_attackTypes_Bullets[(int)GameManager.AttackType.stern] = false;
                        if (CheckStern())
                        {
                            m_attackTypes_Bullets[(int)GameManager.AttackType.stern] = true;
                        }
                        break;
                    case (int)GameManager.AttackType.Poly:
                        if(GameManager.Instance.playerController.shootType == PlayerController.ShootType.normal)
                        {
                            transform.localScale = initScale * 2;
                        }                            
                        else if(GameManager.Instance.playerController.shootType == PlayerController.ShootType.laser)
                        {
                            //transform.localScale = initScale * 4;
                        }
                        break;
                    case (int)GameManager.AttackType.penetration_monster: isPenetation_monster = true; knockbackForce = 0; break;
                }
            }
        }
        for (int i = 0; i < m_attackTypes_Bullets.Count; i++)
        {
            if (m_attackTypes_Bullets[i] == true)
            {
                if (m_attackTypes_Bullets_before[i])
                {
                    CheckAllFalse = true;
                }
                else
                {
                    CheckAllFalse = false;
                }
            }
        }
        if (CheckAllFalse)
        {
            for (int i = 0; i < m_attackTypes_Bullets_before.Count; i++)
            {
                m_attackTypes_Bullets_before[i] = false;
            }
        }
        for (int i = 0; i < m_attackTypes_Bullets.Count; i++)
        {
            if (m_attackTypes_Bullets[i] == true)
            {
                if (i == (int)GameManager.AttackType.posion && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Posion;
                    m_attackTypes_Bullets_before[i] = true;
                    //SetColor(PosionColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.slow && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Slow;
                    m_attackTypes_Bullets_before[i] = true;
                    //SetColor(SlowColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.critical && m_attackTypes_Bullets_before[i] == false)
                {
                    isCritical = true;
                    //SetColor(CriticalColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.fascination && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Fascination;
                    m_attackTypes_Bullets_before[i] = true;
                    //SetColor(FascinationColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.stern && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Stren;
                    m_attackTypes_Bullets_before[i] = true;                   
           
                    return;
                }

            }
        }
    }
}
