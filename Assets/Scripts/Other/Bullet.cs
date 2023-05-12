using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using SensorToolkit;
using System.Linq;

public class Bullet : DamageColider
{
  
    public Color SlowColor;
    public Color PosionColor;
    public Color CriticalColor;
    public Color Plus_2_randColor;
    public Color FascinationColor;
    public Color SternColor;
    private readonly int ColorGradient = Shader.PropertyToID("_GradTopLeftCol");
    public List<bool> m_attackTypes = new List<bool>();
    public List<bool> m_attackTypes_Bullets = new List<bool>();
    public List<bool> m_attackTypes_Bullets_before = new List<bool>();
    [SerializeField]
    CapsuleCollider2D boxCollider;
    public GameObject m_Target;

    public List<bool> m_attackMethods = new List<bool>();

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
    Vector2 initScale;
    
    float defaultKncokback;
 
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
    bool CheckAllFalse = false;
    public void CheckAttackType()
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
                    case (int)GameManager.AttackType.Poly: transform.localScale = initScale * 2; break;
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
                    SetColor(PosionColor);                    
                    return;
                }
                if (i == (int)GameManager.AttackType.slow && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Slow;                    
                    m_attackTypes_Bullets_before[i] = true;
                    SetColor(SlowColor);                    
                    return;
                }
                if (i == (int)GameManager.AttackType.critical && m_attackTypes_Bullets_before[i] == false)
                {
                    isCritical = true;
                    SetColor(CriticalColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.fascination && m_attackTypes_Bullets_before[i] == false)
                {       
                    status = Monster.Status.Fascination;                    
                    m_attackTypes_Bullets_before[i] = true;
                    SetColor(FascinationColor);
                    return;
                }
                if (i == (int)GameManager.AttackType.stern && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Stren;
                    m_attackTypes_Bullets_before[i] = true;
                    SetColor(SternColor);
                    return;
                }

            }
        }

    }
    bool CheckStern()
    {
        bool isStern = false;

        double sternProbability = (GameManager.Instance.luck * 0.033d) + 0.1d;
        if (GameManager.Instance.FindProbability(sternProbability))
        {
            isStern = true;            
        }      
        return isStern;
    }
    bool CheckFascination()
    {
        bool isFascination = false;
        double FascinationProbability = (GameManager.Instance.luck * 0.033d) + 0.1d;
        if (GameManager.Instance.FindProbability(FascinationProbability))
        {          
            isFascination = true;            
        }        
        return isFascination;
    }

    bool CheckCritical()
    {
        bool isCirtical= false;
        double criticlaValue = (GameManager.Instance.luck+1) * 0.1d;
        if (GameManager.Instance.FindProbability(criticlaValue))
        {
            isCirtical = true;
        }
        return isCirtical;
    }
    bool CheckPosion()
    {
        bool isPosion = false;
        double PosionProbability = (GameManager.Instance.luck * 0.05d) + 0.625d;
        if (GameManager.Instance.FindProbability(PosionProbability))
        {
            isPosion = true;                            
        }
        return isPosion;
    }
    bool CheckSlow()
    {
        bool isSlow = false;
        double slowProbability = (GameManager.Instance.luck * 0.05d) + 0.25d;
        if (GameManager.Instance.FindProbability(slowProbability))
        {
            isSlow = true;                           
        }        
        return isSlow;
    }
    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
 
    private void OnEnable()
    {
        //RandColor();
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
    }  
    void SetColor(Color color)
    {
        
        //material.SetColor("Top Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        material.SetColor(ColorGradient, color);
    }
    void Update()
    {     
        //deltaTime -= Time.deltaTime;
        //if(deltaTime <=0)
        //{
        //    EZ_PoolManager.Despawn(transform);
        //}
        Homing();
        boomerang();
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
        if(TriggerEnter)
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
    }
}

