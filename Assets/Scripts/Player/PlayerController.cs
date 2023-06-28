using DungeonMaker;
using SensorToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rigidbody2;
    public bool isFly = false;
    [SerializeField]
    Vector2 NormalGunPos;
    [SerializeField]
    Vector2 FlyGunPos;
    public GameObject MagnetObj;
    public GameObject ShieldObject;
    public enum ShootType
    {
        normal,
        laser
    }
    public Transform LaserAttack;
    public GameObject GunObject;

    public ShootType shootType = ShootType.normal;    

    public ShootController shootController;

    [SerializeField]
    GameObject Target;
    [SerializeField]
    RangeSensor2D targetSensor;
    [SerializeField]
    RangeSensor2D attackSensor;
    public GameObject SensroViewer;

    public float Damage;
    [SerializeField]
    float speed = 1;
    [SerializeField]
    float defaultAttackDelay = 1;
        
    float attackTime = 1;

    public FloatingJoystick Joystick;

    public GameObject enemy;
    private Animator animator;
    private TargetFollow targetFollow;

    SpriteRenderer spriteRenderer;
    SpriteRenderer gunsprite;
    private Animator GunAnim;
    float defaultSpeed;
    IEnumerator CheckShieldRoutine;
    CapsuleCollider2D capsuleCollider;

    List<GameManager.AttackMethod> m_attackMethod;
    List<GameManager.AttackType> m_attackTypes;

    List<bool> bAttackTypes = new List<bool>();
    List<bool> bAttackMethod = new List<bool>();

    public delegate void PlayerMove(bool flag);
    public event PlayerMove PlayerMoveEventHandler;

    public GameObject LaserRayObject;


    private void Awake()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackType)).Length; i++)
        {
            bAttackTypes.Add(false);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackMethod)).Length; i++)
        {
            bAttackMethod.Add(false);
        }        
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        defaultSpeed = speed;       
        animator = GetComponent<Animator>();
        targetFollow = Target.GetComponent<TargetFollow>();
        attackTime = defaultAttackDelay;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gunsprite = GunObject.GetComponent<SpriteRenderer>();
        GunAnim = GunObject.GetComponent<Animator>();
        MagnetObj.SetActive(false);
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.ItemOthers)).Length; i++)
        {
            m_ItemOthers.Add(false);
        }        
        ItemController.Instance.OnChangeDamageEvnetHandler += ItemController_OnChangeDamageEvnetHandler;
        ItemController.Instance.GetDamage();
        SetFly();
      
    }
    
    void SetFly()
    {
        if (isFly)
        {
            GunObject.transform.localPosition = FlyGunPos;
            animator.Play("Fly");
        }
        else
        {
            GunObject.transform.localPosition = NormalGunPos;
            animator.Play("Noraml");
        }
        
    }
    private void OnEnable()
    {
        bStartShield = false;
    }
    float m_shootSpeed;
    bool bStartShield = false;
    IEnumerator ShieldRoutine()
    {        
        yield return new WaitForSeconds(2f);
        double probability = 0.2d + (GameManager.Instance.luck * 0.29d);       
        if(GameManager.Instance.FindProbability(probability))
        {
            ShieldObject.SetActive(true);
            ShieldObject.GetComponent<ShieldController>().OnCompleteEvnetHander += PlayerController_OnCompleteEvnetHander;
        }
        else
        {
            CheckShieldRoutine = ShieldRoutine();
            StartCoroutine(CheckShieldRoutine);
        }
    }

    private void PlayerController_OnCompleteEvnetHander()
    {
        ShieldObject.GetComponent<ShieldController>().OnCompleteEvnetHander -= PlayerController_OnCompleteEvnetHander;
        ShieldObject.SetActive(false);
        CheckShieldRoutine = ShieldRoutine();
        StartCoroutine(CheckShieldRoutine);
    }
    public int m_shootCount;
    public List<bool> m_ItemOthers = new List<bool>();
    private void ItemController_OnChangeDamageEvnetHandler(float damage,float tps, int shootCount, List<GameManager.AttackMethod> attackMethod, 
        List<GameManager.AttackType> attackTypes,float range,float shootSpeed,float moveSpeed,List<GameManager.ItemOthers> ItemOthers,bool fly)
    {
        shootType = ShootType.normal;

        m_attackMethod = attackMethod;
        m_attackTypes = attackTypes;

        for (int i = 0; i < m_attackTypes.Count; i++)
        {
            bAttackTypes[(int)m_attackTypes[i]] = true;
        }
        for (int i = 0; i < m_attackMethod.Count; i++)
        {
            bAttackMethod[(int)m_attackMethod[i]] = true;
        }

        isFly = fly;
        SetFly();
        Damage = damage;
        defaultAttackDelay = tps /20;
        shootController.BulletCount = shootCount;
        m_shootCount = shootCount;
        ChangeRange(range);
        
        shootController.SetBulletAttackType(attackTypes);
        shootController.SetBulletMethodType(attackMethod);
        for(int i=0;i< attackMethod.Count; i++)
        {
            if (attackMethod[i] == GameManager.AttackMethod.laser)
            {
                shootType = ShootType.laser;
            }
        }
        
        
        for (int i = 0; i < ItemOthers.Count; i++)
        {
            m_ItemOthers[(int)ItemOthers[i]] = true;
        }
        MagnetObj.SetActive(m_ItemOthers[(int)GameManager.ItemOthers.followItem]);
        if(m_ItemOthers[(int)GameManager.ItemOthers.shield_7]==true && bStartShield ==false)
        {
            CheckShieldRoutine = ShieldRoutine();
            StartCoroutine(CheckShieldRoutine);
            bStartShield = true;
        }
        shootController.SetShootSpeed(shootSpeed);
     
        if(moveSpeed>0)
        {
            speed = defaultSpeed + moveSpeed;
        }
    }
    private void OnDisable()
    {
        if(CheckShieldRoutine !=null)
        {
            StopCoroutine(CheckShieldRoutine);
        }        
    }
    void ChangeRange(float range)
    {
        float defaultRange = 0.6f;
        if(range>0)
        {
            range = (range / 5) + defaultRange;
            attackSensor.SensorRange = range;
            targetSensor.SensorRange = range;
            SensroViewer.transform.localScale = new Vector3(range * 2, range * 2, range * 2);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public List<Vector2> EnmeyVecList = new List<Vector2>();

    void TargetCheck()
    {
        if (targetSensor.DetectedObjects.Count <= 0)
        {
            enemy = null;
            Target.SetActive(false);
        }
        else
        {          
            float shortDis = Vector3.Distance(transform.position, targetSensor.DetectedObjects[0].transform.position);

            enemy = targetSensor.DetectedObjects[0];
            trees = new Transform[targetSensor.DetectedObjects.Count];
            int index = 0;
            foreach (GameObject found in targetSensor.DetectedObjects)
            {
                float Distance = Vector3.Distance(transform.position, found.transform.position);

                if(found.tag != "Obstacles")
                {
                    if (Distance < shortDis)
                    {
                        enemy = found;
                        shortDis = Distance;
                    }
                    trees[index] = found.transform;
                    index++;
                }                
            }
            
            trees = trees.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToArray();
            targetFollow.SetTarget(enemy);     

            EnmeyVecList.Clear();           
            for (int i = 0; i < trees.Length; i++)
            {
                EnmeyVecList.Add(trees[i].gameObject.transform.position);
            }

            Target.SetActive(true);
        }
    }
    public Transform[] trees;


    void CheckAttack()
    {
        attackTime -= Time.deltaTime;
        if(attackTime <=0)
        {
            attackTime = 0;
        }
        if(enemy ==null)
        {
            return;
        }
        if(attackSensor.DetectedObjects.Count <=0)
        {
            return;
        }
        float shortDis = Vector3.Distance(transform.position, attackSensor.DetectedObjects[0].transform.position);

        GameObject attackTarget = attackSensor.DetectedObjects[0];

        foreach (GameObject found in attackSensor.DetectedObjects)
        {
            float Distance = Vector3.Distance(transform.position, found.transform.position);

            if (Distance < shortDis)
            {
                attackTarget = found;
                shortDis = Distance;
            }
        }
        if(attackTarget == enemy)
        {
            if(attackTime <=0 )
            {
                Attack();
                attackTime = defaultAttackDelay;
            }
        }

    }
    void Update()
    {        
        if(GameManager.Instance.gameStatus != GameManager.GameStatus.NOTING)
        {
            return;
        }
        moveValue =  Mathf.Abs(Joystick.Horizontal) +Mathf.Abs(Joystick.Vertical);
        if(moveValue>0)
        {
            PlayerMoveEventHandler?.Invoke(true);
        }
        else
        {
            PlayerMoveEventHandler?.Invoke(false);
        }
        CheckAttack();  
        UpdateGun();
        rigidbody2.velocity = new Vector2(0, 0);
    }
    public float GunRotationSpeed = 10f;
    void UpdateGun()
    {
        if (enemy != null)
        {
            Vector2 direction = enemy.transform.position - GunObject.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GunObject.transform.rotation = Quaternion.Lerp(GunObject.transform.rotation, rotation, Time.deltaTime * GunRotationSpeed);
          
            if (GunObject.transform.localScale.y > 0)
            {
                //gunsprite.flipX = true;
            }
            else
            {
                gunsprite.flipX = false;
            }

        }
        else
        {
            GunAnim.Play("idle");
            Quaternion rotation = Quaternion.AngleAxis(180, Vector3.forward);
            GunObject.transform.rotation = Quaternion.Lerp(GunObject.transform.rotation, rotation, Time.deltaTime * 8);
            gunsprite.flipX = !spriteRenderer.flipX;
            //gunsprite.flipY = true;
            GunObject.transform.localScale = new Vector3(1, 1, 1);
        }
        if (spriteRenderer.flipX)
        {
            GunObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            GunObject.transform.localScale = new Vector3(-1, -1, 1);
        }
        

    }
    bool isKnockback = false;
    IEnumerator knockbackRoutine;
    public void Knockback(Vector3 value,int HitDamage)
    {
        if(isKnockback ==false && canKnockback ==true)
        {
            isKnockback = true;
            knockbackRoutine = KnockbackRoutine(value, HitDamage);
            StartCoroutine(knockbackRoutine);
            //StartCoroutine(KnockbackRoutine(value, HitDamage));
        }
    }
  
    
    public float knockbackForce=2f;
    IEnumerator KnockbackRoutine(Vector3 dir, int HitDamage)
    {
        GameManager.Instance.gameStatus = GameManager.GameStatus.KNOCKBACK;
        //GameManager.Instance.Hp -= HitDamage;
        GameManager.Instance.AddHp(HitDamage);
        //UIManager.Instance.SetHp();
        float ctime = 0;
        Vector2 direction = (transform.position-dir).normalized;
        Vector2 knockVec = direction * knockbackForce;
        float moveSpeed = 1;
        while(ctime <.2f)
        {
            //transform.Translate(Vector2.left * moveSpeed * Time.deltaTime*1);
            transform.Translate(knockVec * moveSpeed * Time.deltaTime);
            ctime += Time.deltaTime;            
            yield return null;
        }

        GameManager.Instance.gameStatus = GameManager.GameStatus.NOTING;
        yield return new WaitForSeconds(.5f);
        isKnockback = false;
        
    }
    public void Shoot()
    {
        switch(shootType)
        {
            case ShootType.normal:
                if (enemy != null)
                {
                    shootController.Target = enemy;
                    shootController.shoot = true;
                    shootController.Damage = Damage;
                }
                break;
            case ShootType.laser:
                for(int i =0; i< m_shootCount; i++)
                {
                    
                    if (i==0)
                    {
                        Transform tempLaser = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, m_attackMethod, LineAttackController.LineType.None, false, 0);
                    }
                    else
                    {
                        float angle = 30;
                        if (i % 2 == 0)
                        {
                            angle = (30 * ((i + 1) / 2) * (1));
                        }
                        else
                        {
                            angle = (30 * ((i + 1) / 2) * (-1));
                           
                        }
                        Transform tempLaser2;
                        tempLaser2 = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser2.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, 
                            m_attackMethod, LineAttackController.LineType.None, true, angle);
                    }
                }
                
                if (bAttackTypes[(int)GameManager.AttackType.cross])
                {
                    bool canCross = false;
                    double CrossProbability = (GameManager.Instance.luck * 0.05d) + 0.25d;
                    if (GameManager.Instance.FindProbability(CrossProbability))
                    {
                        canCross = true;
                    }
                    if (canCross)
                    {
                        Transform tempLaser_Back = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser_Back.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, m_attackMethod, LineAttackController.LineType.Back, false, 0);

                        Transform tempLaser_Left = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser_Left.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, m_attackMethod, LineAttackController.LineType.Left, false, 0);

                        Transform tempLaser_Right = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser_Right.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, m_attackMethod, LineAttackController.LineType.Right, false, 0);
                    }
                }
                if (bAttackTypes[(int)GameManager.AttackType.back])
                {
                    bool canBack = false;
                    double BackProbability = (GameManager.Instance.luck * 0.1d) + 0.5d;
                    if (GameManager.Instance.FindProbability(BackProbability))
                    {
                        canBack = true;
                    }
                    if (canBack)
                    {
                        Transform tempLaser_Back = EZ_Pooling.EZ_PoolManager.Spawn(LaserAttack, new Vector3(0, 0, 0), new Quaternion());
                        tempLaser_Back.GetComponent<LineAttackController>().Shoot(Damage, enemy, m_attackTypes, m_attackMethod, LineAttackController.LineType.Back, false, 0);
                    }
                }

                //TargetPosition = GetRotateVector(TargetPosition,GameManager.Instance.playerController.shootController.transform.position, angle);

                break;
        }
    }
 
    int ChainIndex = -1;
    public bool isExitObstacles = false;

    public void Attack()
    {
        //animator.SetTrigger("Attack");       
        GunAnim.SetTrigger("Attack");
    }
    bool isFlip = false;
    float moveValue = 0;
    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(0, 0, 0);
        direction = new Vector3((Joystick.Horizontal / 100)*speed, (Joystick.Vertical / 100) * speed);
        transform.position += direction;
        //float moveSpeed = (Mathf.Abs(variableJoystick.Horizontal) + Mathf.Abs(variableJoystick.Vertical)) * speed;
        animator.SetFloat("MotionX", (Joystick.Horizontal));
        animator.SetFloat("MotionY", (Joystick.Vertical));        
        CheckFlip();
        TargetCheck();
    }
    void CheckFlip()
    {        
        if(enemy == null)
        {
            if (Joystick.Horizontal != 0)
            {
                if (Joystick.Horizontal > 0)
                {
                    if (isFlip == false)
                    {
                        //transform.localScale = new Vector3(-1, 1, 1);
                        spriteRenderer.flipX = false;
                    }
                    else
                    {
                        //transform.localScale = new Vector3(1, 1, 1);
                        spriteRenderer.flipX = true;
                    }
                }
                else
                {
                    if (isFlip == false)
                    {
                        //transform.localScale = new Vector3(1, 1, 1);
                        spriteRenderer.flipX = true;
                    }
                    else
                    {
                        //transform.localScale = new Vector3(-1, 1, 1);
                        spriteRenderer.flipX = false;
                    }
                }
            }
        }
        else
        {
            if(enemy.transform.position.x < transform.position.x)
            {
                //transform.localScale = new Vector3(1, 1, 1);
                spriteRenderer.flipX = true;
            }
            else
            {
                //transform.localScale = new Vector3(-1, 1, 1);
                spriteRenderer.flipX = false;
            }
        }
    }
    bool canKnockback = true;
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "Room_wall")
        {
            canKnockback = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "Room_wall")
        {
            if (knockbackRoutine != null)
            {
                StopCoroutine(knockbackRoutine);
                isKnockback = false;
                GameManager.Instance.gameStatus = GameManager.GameStatus.NOTING;
            }
            canKnockback = false;
        }
        if (collision.gameObject.tag == "Room_wall")
        {
            if (collision.transform.parent.parent.parent.parent.GetComponent<DungeonController>().IsOpen)
            {
                if (collision.transform.parent.GetComponent<Rule>().NextMap.name == "ItemRoom")
                {
                    //if(GameManager.Instance.Key >=1)
                    {
                        collision.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().isCome = true;
                        MapMaker.Instance.ChangeMap(collision.transform.parent.GetComponent<Rule>().NextMap, collision.transform.parent.GetComponent<Rule>().NextPosition);
                        //GameManager.Instance.Key -= 1;
                        //UIManager.Instance.SetKeyText();
                    }
                }
                else
                {
                    collision.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().isCome = true;
                    MapMaker.Instance.ChangeMap(collision.transform.parent.GetComponent<Rule>().NextMap, collision.transform.parent.GetComponent<Rule>().NextPosition);
                }
                collision.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().StartMonster();
            }
        }
        if(collision.gameObject.tag == "Obstacle")
        {
            if(isFly)
            {
                Physics2D.IgnoreCollision(capsuleCollider, collision.collider);
            }
        }
        if (collision.gameObject.tag == "Product" )
        {
            if (isFly)
            {                
                Physics2D.IgnoreCollision(capsuleCollider, collision.collider);                
            }
        }
    }   
}
