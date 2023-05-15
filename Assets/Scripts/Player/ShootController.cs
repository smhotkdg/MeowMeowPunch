using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public GameObject Gun;
    public List<bool> m_attackTypes = new List<bool>();
    public List<bool> m_attackMethods = new List<bool>();
    [SerializeField]
    private float BulletRange = 20;
    [SerializeField]
    private float Rotationspeed = 1;
    [SerializeField]
    float rangle_bullet = 0;
    public float speed;
    [SerializeField]
    
    public int BulletCount = 1;
    public GameObject Target;
    public GameObject Bullet;
    public GameObject BulletPos;
    private float defaultShootSpeed;
    public bool shoot = false;
    public float Damage;
    private void Awake()
    {
        defaultShootSpeed = speed;
        for (int i =0; i< System.Enum.GetValues(typeof(GameManager.AttackType)).Length; i++)
        {
            m_attackTypes.Add(false);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackMethod)).Length; i++)
        {
            m_attackMethods.Add(false);
        }
    }
    public void SetBulletAttackType(List<GameManager.AttackType> attackTypes)
    {
        for(int i=0; i< attackTypes.Count; i++)
        {
            m_attackTypes[(int)attackTypes[i]] = true;
        }
    }
    public void SetBulletMethodType(List<GameManager.AttackMethod> attackMethods)
    {
        for (int i = 0; i < attackMethods.Count; i++)
        {
            m_attackMethods[(int)attackMethods[i]] = true;
        }
    }
    public void SetShootSpeed(float _speed)
    {
        speed = defaultShootSpeed + _speed;
    }
    private void Update()
    {
        if (Target != null)
        {          
            //Vector3 TargetVec = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            Vector3 TargetVec = new Vector3(0, 0, 0);
            TargetVec.x += Target.transform.position.x;
            TargetVec.y += Target.transform.position.y;
            Vector2 direction = TargetVec - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);            

            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * Rotationspeed);
            if(Gun.transform.localScale.y <0)
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            
            if (shoot)
            {
                //float BulletRand = Random.Range(-0.5f, 0.5f);
                //BulletRand += BulletRange;
                for (int i = 0; i < BulletCount; i++)
                {
                    float range = 90;
                    
                    if (i % 2 == 0)
                    {
                        range = 90 + (BulletRange * ((i + 1) / 2) * (1));
                        rangle_bullet = (BulletRange * (i + 1) / 2) * (1);
                    }
                    else
                    {
                        range = 90 + (BulletRange * ((i + 1) / 2) * (-1));
                        rangle_bullet = (BulletRange * (i + 1) / 2) * (-1);
                    }

                    // 90 110 70 130 50 
                    Vector2 moveVec;
                    //Vector2 moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2))*speed;
                    if(i >0)
                    {
                        moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2)) * speed;
                    }
                    else
                    {
                        moveVec = (BulletPos.transform.up) * speed;
                    }
                    

                    Transform object_transfrom =  EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range, Vector3.forward));
                    object_transfrom.gameObject.SetActive(false);
                    object_transfrom.GetComponent<Bullet>().SetAttackType(m_attackTypes);
                    object_transfrom.GetComponent<Bullet>().SetAttackMethods(m_attackMethods,Target);
                    object_transfrom.GetComponent<Bullet>().SetSpeed(speed);
                    object_transfrom.GetComponent<Bullet>().Power = Damage;

                    //object_transfrom.GetComponent<Rigidbody2D>().velocity = BulletPos.transform.up * speed;
                    object_transfrom.gameObject.SetActive(true);
                    object_transfrom.GetComponent<Rigidbody2D>().velocity = moveVec;
                    object_transfrom.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.forward;
                    object_transfrom.GetComponent<Bullet>().SetVelocity();

                    Vector2 dir = object_transfrom.GetComponent<Rigidbody2D>().velocity;
                    float angle_bullets = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    object_transfrom.transform.rotation = Quaternion.AngleAxis(angle_bullets - 90, Vector3.forward);

                    if (m_attackTypes[(int)GameManager.AttackType.plus_2_random])
                    {
                        StartCoroutine(ShootPlus(rangle_bullet, angle, range));
                    }

                }
                float range_temp = 90;
                if (m_attackTypes[(int)GameManager.AttackType.back])
                {
                    bool canBack = false;
                    double BackProbability = (GameManager.Instance.luck * 0.1d) + 0.5d;
                    if (GameManager.Instance.FindProbability(BackProbability))
                    {
                        canBack = true;                       
                    }                    
                    if (canBack)
                    {
                        Transform object_transfrom_back = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range_temp, Vector3.forward));
                        object_transfrom_back.gameObject.SetActive(false);
                        object_transfrom_back.GetComponent<Bullet>().SetAttackType(m_attackTypes);
                        object_transfrom_back.GetComponent<Bullet>().SetAttackMethods(m_attackMethods,Target);
                        object_transfrom_back.GetComponent<Bullet>().SetSpeed(speed);
                        object_transfrom_back.GetComponent<Bullet>().Power = Damage;
                        object_transfrom_back.gameObject.SetActive(true);
                        object_transfrom_back.GetComponent<Rigidbody2D>().velocity = -BulletPos.transform.up * speed;
                        object_transfrom_back.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.back;
                        object_transfrom_back.GetComponent<Bullet>().SetVelocity();

                    }
                }
                if (m_attackTypes[(int)GameManager.AttackType.cross])
                {
                    bool canCross = false;
                    double CrossProbability = (GameManager.Instance.luck * 0.05d) + 0.25d;
                    if (GameManager.Instance.FindProbability(CrossProbability))
                    {
                        canCross = true;
                    }
                    if (canCross)
                    {
                        Transform object_transfrom_back = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range_temp, Vector3.forward));
                        object_transfrom_back.gameObject.SetActive(false);
                        object_transfrom_back.GetComponent<Bullet>().SetAttackType(m_attackTypes);
                        object_transfrom_back.GetComponent<Bullet>().SetAttackMethods(m_attackMethods,Target);
                        object_transfrom_back.GetComponent<Bullet>().Power = Damage;
                        object_transfrom_back.GetComponent<Bullet>().SetSpeed(speed);
                        object_transfrom_back.gameObject.SetActive(true);
                        object_transfrom_back.GetComponent<Rigidbody2D>().AddForce(-BulletPos.transform.up * speed, ForceMode2D.Impulse);
                        object_transfrom_back.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.back;
                        object_transfrom_back.GetComponent<Bullet>().SetVelocity();



                        Transform object_transfrom_Right = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
                        object_transfrom_Right.gameObject.SetActive(false);
                        object_transfrom_Right.GetComponent<Bullet>().SetAttackType(m_attackTypes);
                        object_transfrom_Right.GetComponent<Bullet>().SetAttackMethods(m_attackMethods,Target);
                        object_transfrom_Right.GetComponent<Bullet>().SetSpeed(speed);
                        object_transfrom_Right.GetComponent<Bullet>().Power = Damage;
                        object_transfrom_Right.gameObject.SetActive(true);
                        object_transfrom_Right.GetComponent<Rigidbody2D>().AddForce(BulletPos.transform.right * speed, ForceMode2D.Impulse);
                        object_transfrom_Right.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.back;
                        object_transfrom_Right.GetComponent<Bullet>().SetVelocity();

                        Transform object_transfrom_Left = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - 180, Vector3.forward));
                        object_transfrom_Left.gameObject.SetActive(false);
                        object_transfrom_Left.GetComponent<Bullet>().SetAttackType(m_attackTypes);
                        object_transfrom_Left.GetComponent<Bullet>().SetAttackMethods(m_attackMethods,Target);
                        object_transfrom_Left.GetComponent<Bullet>().SetSpeed(speed);
                        object_transfrom_Left.GetComponent<Bullet>().Power = Damage;
                        object_transfrom_Left.gameObject.SetActive(true);
                        object_transfrom_Left.GetComponent<Rigidbody2D>().AddForce(-BulletPos.transform.right * speed, ForceMode2D.Impulse);
                        object_transfrom_Left.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.back;
                        object_transfrom_Left.GetComponent<Bullet>().SetVelocity();
                    }
                }
                
                shoot = false;
            }
        }
    }
    IEnumerator ShootPlus(float rangle_bullet,float angle,float range)
    {
        yield return new WaitForSeconds(.1f);
        Vector2 moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2)) * speed;

        Transform object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range, Vector3.forward));
        
        object_transfrom.GetComponent<Bullet>().SetAttackType(m_attackTypes);
        object_transfrom.GetComponent<Bullet>().SetAttackMethods(m_attackMethods, Target);
        object_transfrom.GetComponent<Bullet>().SetSpeed(speed);
        object_transfrom.GetComponent<Bullet>().Power = 2f;
        object_transfrom.transform.localScale = Bullet.transform.localScale * 0.75f;
        object_transfrom.gameObject.SetActive(true);
        object_transfrom.GetComponent<Rigidbody2D>().velocity = moveVec;
        object_transfrom.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.forward;
        object_transfrom.GetComponent<Bullet>().SetVelocity();
    }
}
