using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using SensorToolkit;
using System.Linq;

public class Bullet : DamageColider
{
    public bool TargetPlayerAttack = false;
    public bool isDirectShoot = true;

    public float ShootWaitTime = 0;

    private float m_shootWaitTime;
    public Transform MissileTarget;
    public TrailRenderer m_TrailRenderer; 
    public Color SlowColor;
    public Color PosionColor;
    public Color CriticalColor;
    public Color Plus_2_randColor;
    public Color FascinationColor;
    public Color SternColor;
    private readonly int ColorGradient = Shader.PropertyToID("_GradTopLeftCol");

    [SerializeField]
    CapsuleCollider2D boxCollider;
    public GameObject m_Target;

    //public List<bool> m_attackMethods = new List<bool>();

    public float LifeTime =3;

    public float speed = 5;

    
    public float boomerangTime = 1f;
    Material material;
    float deltaTime;
    RangeSensor2D rangeSensor2D;
    float defaultboomerangTime;
    public enum BulletDirection
    {
        forward,
        back,
        right,
        left,
        cross,
        Split
    }   
    public BulletDirection bulletDirection = BulletDirection.forward;
    float defaultSpeed;


    bool isTargetingMissile = false;
    public Vector2 Target_Boss;
    public Transform t;
    public void SetTargeting(Vector2 targetPos)
    {
        Target_Boss = targetPos;
        isTargetingMissile = true;
        t = EZ_PoolManager.Spawn(MissileTarget, Target_Boss, new Quaternion());
    }
    
    void TargetingMissile()
    {
        if (isTargetingMissile == false)
            return;
        float rotateAmount = 0;        
        Vector2 direction;
        Vector3 tart3 = Target_Boss;
        Vector2 dir = tart3 - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //== 타겟 방향으로 회전함 ==//
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);


        direction = Target_Boss - rb.position;
        direction.Normalize();
        rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -AngleSpeed * rotateAmount;
        rb.velocity = transform.up * speed;
        if(Mathf.Abs(Vector2.Distance(Target_Boss,transform.position))<=0.05f)
        {
            animator.SetTrigger("Hit");
            EZ_PoolManager.Despawn(t);
            isTargetingMissile = false;
        }
       
    }
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_attackMethods.Clear();
        m_attackTypes.Clear();
        m_attackTypes_Bullets_before.Clear();
        m_attackTypes_Bullets.Clear();
        defaultboomerangTime = boomerangTime;
        rangeSensor2D = GetComponent<RangeSensor2D>();
        defaultHorming = hormingTime;
        material = GetComponent<SpriteRenderer>().material;
        defaultKncokback = knockbackForce;
        initScale = transform.localScale;
        defaultSpeed = speed;
        boxCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        isDisable = true;
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackType)).Length; i++)
        {
            m_attackTypes.Add(false);
            m_attackTypes_Bullets.Add(false);
            m_attackTypes_Bullets_before.Add(false);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackMethod)).Length; i++)
        {
            m_attackMethods.Add(false);
        }
        bulletTransform = transform;
    }
    
    public void SetAttackType(List<bool> _attackType)
    {
        m_attackTypes = _attackType;
    }
    Vector2 velocity;
    public void SetVelocity()
    {
        velocity = rb.velocity;
    }

    public void SetAttackMethods(List<bool> _attackMethod,GameObject target)
    {       
        m_attackMethods = _attackMethod;
        m_Target = target;
    }
    
    public void CheckAttackType()
    {
        CheckEffectBullet();   

    }
   
    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
 
    private void OnEnable()
    {
        //RandColor();
        m_TrailRenderer.Clear();
        Hit = false;
        boomerangTime = defaultboomerangTime;
        if(bulletType == BulletType.player)
        {
            SetColor(new Color(1, 1, 1, 1));
        }
        else if(bulletType == BulletType.monster)
        {
            SetColor(new Color(1, 0, 0, 1));
        }
        
        boxCollider.enabled = true;
        initPos = transform.position;
        transform.localScale = initScale;
        boxCollider.enabled = true;        
        deltaTime = LifeTime;
        CheckAttackType();
        hormingTime = defaultHorming;
        isEnable = true;
        isBoomerang = false;
        m_shootWaitTime = ShootWaitTime;
        Shoot_Manual = false;
        isStopMovement = false;
    }  
    void SetColor(Color color)
    {
        
        //material.SetColor("Top Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        material.SetColor(ColorGradient, color);
    }
    bool Shoot_Manual =false;
    Vector2 Manualdir;
    void Update()
    {
        //deltaTime -= Time.deltaTime;
        //if(deltaTime <=0)
        //{
        //    EZ_PoolManager.Despawn(transform);
        //}
        if(isDirectShoot)
        {
            TargetingMissile();
            Homing();
            boomerang();
        }
        else
        {
            m_shootWaitTime -= Time.deltaTime;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            if(m_shootWaitTime<=0)
            {
                if(isStopMovement)
                {
                    return;
                }
                if (Shoot_Manual == false)
                {
                    Manualdir = GameManager.Instance.GetTwoPointDistanceVector(
                    transform.position, GameManager.Instance.Player.transform.position, 100);                    
                    Shoot_Manual = true;
                }
                
                //velocity.y = 2f;
                //rb.velocity = velocity;                
                rb.constraints = RigidbodyConstraints2D.None;
                transform.position = Vector3.MoveTowards(transform.position, Manualdir, speed * Time.deltaTime);
            }
        }
    }
    public float AngleSpeed = 1;    
    public float hormingTime = 1f;
    float defaultHorming;
    public Transform[] trees;    
    GameObject findMin()
    {
        GameObject tempObject;
        float shortDis = Vector3.Distance(transform.position, rangeSensor2D.DetectedObjects[0].transform.position);

        tempObject = rangeSensor2D.DetectedObjects[0];
        trees = new Transform[rangeSensor2D.DetectedObjects.Count];
        int index = 0;
        foreach (GameObject found in rangeSensor2D.DetectedObjects)
        {
            float Distance = Vector3.Distance(transform.position, found.transform.position);

            if (found.tag != "Obstacles")
            {
                if (Distance < shortDis)
                {
                    tempObject = found;
                    shortDis = Distance;
                }
                trees[index] = found.transform;
                index++;
            }
        }

        trees = trees.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToArray();

        return trees[0].gameObject;
    }
    void Homing()
    {   
        if(Hit)
        {
            return;
        }
      
        if(m_attackMethods[(int)GameManager.AttackMethod.homing])
        {
            float rotateAmount = 0;
            hormingTime -= Time.deltaTime;
            Vector2 direction;
            GameObject temp = null;
            if(rangeSensor2D.DetectedObjects.Count >0)
            {
                if(PrevTarget !=null)
                {
                    if(findMin() != PrevTarget)
                    {
                        temp = findMin();
                    }
                }
                else
                {
                    temp = findMin();
                }

                if (temp == null)
                    return;
            }
            else
            {
                return;
            }
            if (hormingTime <= 0)
            {
                return;
            }
            //transform.position = Vector3.MoveTowards(transform.position, temp.transform.position, speed*Time.deltaTime);
            Vector3 dir = temp.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //== 타겟 방향으로 회전함 ==//
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);


            direction = (Vector2)temp.transform.position - rb.position;
            direction.Normalize();
            rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -AngleSpeed * rotateAmount;
            rb.velocity = transform.up * speed;      
        }
    }
    bool isBoomerang = false;
    void boomerang()
    {        
        if (m_attackMethods[(int)GameManager.AttackMethod.boomerang])
        {
            boomerangTime -= Time.deltaTime;
            if (boomerangTime <= 0 && isBoomerang == false)
            {
                isBoomerang = true;
                Vector2 randPos = new Vector2();
                switch (bulletDirection)
                {
                    case BulletDirection.forward:
                        randPos =new Vector2(Random.Range(-transform.right.x/2f, transform.right.x/2f), Random.Range(-transform.right.y/2f, transform.right.y/2f));
                        break;
                    case BulletDirection.back:
                        randPos = new Vector2(Random.Range(-transform.right.x/2f, transform.right.x/2f), Random.Range(-transform.right.y, transform.right.y/2f));
                        break;
                    case BulletDirection.left:
                        randPos = new Vector2(Random.Range(-transform.up.x/2f, transform.up.x/2f), Random.Range(-transform.up.y/2f, transform.up.y/2f));
                        break;
                    case BulletDirection.right:
                        randPos = new Vector2(Random.Range(-transform.up.x/2f, transform.up.x/2f), Random.Range(-transform.up.y/2f, transform.up.y/2f));
                        break;
                }
                float randVelocity = Random.Range(1, 2f);
                rb.velocity = -rb.velocity* randVelocity + randPos;
                boomerangTime = defaultboomerangTime;

                Vector2 dir = rb.velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }        
    }
 
    public void EndAnimation()
    {
        EZ_PoolManager.Despawn(transform);
    }
    public void SetSplit(GameObject _prevTarget)
    {
        PrevTarget = _prevTarget;
    }
    private void OnDisable()
    {
        isEnable = false;
        PrevTarget = null;
        if(t !=null && isTargetingMissile ==true)
        {
            EZ_PoolManager.Despawn(t);
            isTargetingMissile = false;
        }
        
    }
}

