using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
public class Bullet : DamageColider
{
    public Color SlowColor;
    public Color PosionColor;
    private readonly int ColorGradient = Shader.PropertyToID("_GradTopLeftCol");
    public List<bool> m_attackTypes = new List<bool>();
    public List<bool> m_attackTypes_Bullets = new List<bool>();
    public List<bool> m_attackTypes_Bullets_before = new List<bool>();
    [SerializeField]
    BoxCollider2D boxCollider;    

    public float LifeTime =3;

    public float speed = 5;

    private Rigidbody2D rb;

    Material material;
    float deltaTime;
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
        material = GetComponent<SpriteRenderer>().material;
        defaultKncokback = knockbackForce;
        initScale = transform.localScale;
        defaultSpeed = speed;
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        isDisable = true;
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackType)).Length; i++)
        {
            m_attackTypes.Add(false);
            m_attackTypes_Bullets.Add(false);
            m_attackTypes_Bullets_before.Add(false);
        }
        
    }
    public void SetAttackType(List<bool> _attackType)
    {
        m_attackTypes = _attackType;
    }
    bool CheckAllFalse = false;
    public void CheckAttackType()
    {
        status = Monster.Status.Normal;
        knockbackForce = defaultKncokback;
        isPenetration_object = false;
        isSplit = false;
        for (int i = 0; i < m_attackTypes.Count; i++)
        {            
            if (m_attackTypes[i] == true)
            {
                switch (i)
                {                    
                    case (int)GameManager.AttackType.critical: break;                    
                    case (int)GameManager.AttackType.fascination: break;
                    case (int)GameManager.AttackType.penetration_object: isPenetration_object = true; break;
                    case (int)GameManager.AttackType.plus_2_random: break;
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
                    case (int)GameManager.AttackType.stern: break;
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
                    DamangeColor = PosionColor;
                    m_attackTypes_Bullets_before[i] = true;
                    SetColor(PosionColor);
                    //Debug.Log("독 공격");
                    return;
                }
                if (i == (int)GameManager.AttackType.slow && m_attackTypes_Bullets_before[i] == false)
                {
                    status = Monster.Status.Slow;
                    DamangeColor = SlowColor;
                    m_attackTypes_Bullets_before[i] = true;
                    SetColor(SlowColor);
                    //Debug.Log("슬로우 공격");
                    return;
                }
              
            }
        }

    }
    bool CheckPosion()
    {
        bool isPosion = false;
        if (GameManager.Instance.luck == 0)
        {
            if (Random.Range(0, 5) == 0)
            {
                isPosion = true;                
            }
        }
        else if (Random.Range(GameManager.Instance.luck + 5, 21) >= Random.Range(0, 21))
        {            
            isPosion = true;
        }
        return isPosion;
    }
    bool CheckSlow()
    {
        bool isSlow = false;
        if (GameManager.Instance.luck == 0)
        {
            if (Random.Range(0, 5) == 0)
            {
                isSlow = true;                
            }
        }
        else if (Random.Range(GameManager.Instance.luck + 5, 21) >= Random.Range(0, 21))
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
        SetColor(new Color(1, 1, 1, 1));
        boxCollider.enabled = true;
        initPos = transform.position;
        transform.localScale = initScale;
        boxCollider.enabled = true;
        deltaTime = LifeTime;
        CheckAttackType();
        isEnable = true;
        
    }  
    void SetColor(Color color)
    {
        //material.SetColor("Top Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        material.SetColor(ColorGradient, color);
    }
    void Update()
    {     
        deltaTime -= Time.deltaTime;
        if(deltaTime <=0)
        {
            EZ_PoolManager.Despawn(transform);
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

