using DungeonMaker;
using SensorToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject MagnetObj;
    public GameObject ShieldObject;
    public enum ShootType
    {
        normal,
        laser
    }
    
    public GameObject GunObject;

    public ShootType shootType = ShootType.normal;
    public ChainLightning chainLightning;

    public TestScript testScript;

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

    private GameObject enemy;
    private Animator animator;
    private TargetFollow targetFollow;

    SpriteRenderer spriteRenderer;
    SpriteRenderer gunsprite;
    private Animator GunAnim;
    float defaultSpeed;
    IEnumerator CheckShieldRoutine;
    private void Awake()
    {
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

    public List<bool> m_ItemOthers = new List<bool>();
    private void ItemController_OnChangeDamageEvnetHandler(float damage,float tps, int shootCount, List<GameManager.AttackMethod> attackMethod, 
        List<GameManager.AttackType> attackTypes,float range,float shootSpeed,float moveSpeed,List<GameManager.ItemOthers> ItemOthers)
    {
        Damage = damage;
        defaultAttackDelay = tps /20;
        shootController.BulletCount = shootCount;
        chainLightning.lightnings = shootCount;
        ChangeRange(range);
        chainLightning.Power = Damage;
        shootController.SetBulletAttackType(attackTypes);
        shootController.SetBulletMethodType(attackMethod);
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
        StopCoroutine(CheckShieldRoutine);
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
        moveValue = Joystick.Horizontal + Joystick.Vertical;
        
        CheckAttack();
        //if(moveValue >0)
        //{
        //    animator.Play("Blend Tree");
        //}
        UpdateChainEnemy();
        UpdateGun();
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
            //gunsprite.flipY = !spriteRenderer.flipX;
            if(spriteRenderer.flipX)
            {
                GunObject.transform.localScale = new Vector3(-1, -1, 1);
            }
            else
            {
                GunObject.transform.localScale = new Vector3(-1, 1, 1);
            }
            gunsprite.flipX = false;
        }
        else
        {

            Quaternion rotation = Quaternion.AngleAxis(180, Vector3.forward);
            GunObject.transform.rotation = Quaternion.Lerp(GunObject.transform.rotation, rotation, Time.deltaTime * 8);
            gunsprite.flipX = spriteRenderer.flipX;
            //gunsprite.flipY = true;
            GunObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        

    }
    bool isKnockback = false;
    public void Knockback(Vector3 value,int HitDamage)
    {
        if(isKnockback ==false)
        {
            isKnockback = true;
            StartCoroutine(KnockbackRoutine(value, HitDamage));
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
                ChainIndex = -1;
                chainLightning.Power = Damage;
                chainLightning.BuildChain(EnmeyVecList);
                break;
        }
    }
    int ChainIndex = -1;
    public bool isExitObstacles = false;
   
    void UpdateChainEnemy()
    {        
        if (EnmeyVecList.Count > 0)
        {
            chainLightning.UpdateChain(EnmeyVecList);
        }      
        
    }
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
                        spriteRenderer.flipX = true;
                    }
                    else
                    {
                        //transform.localScale = new Vector3(1, 1, 1);
                        spriteRenderer.flipX = false;
                    }
                }
                else
                {
                    if (isFlip == false)
                    {
                        //transform.localScale = new Vector3(1, 1, 1);
                        spriteRenderer.flipX = false;
                    }
                    else
                    {
                        //transform.localScale = new Vector3(-1, 1, 1);
                        spriteRenderer.flipX = true;
                    }
                }
            }
        }
        else
        {
            if(enemy.transform.position.x < transform.position.x)
            {
                //transform.localScale = new Vector3(1, 1, 1);
                spriteRenderer.flipX = false;
            }
            else
            {
                //transform.localScale = new Vector3(-1, 1, 1);
                spriteRenderer.flipX = true;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Room_wall")
        {
            if (collision.transform.parent.parent.parent.parent.GetComponent<DungeonController>().IsOpen)
            {
                if (collision.transform.parent.GetComponent<Rule>().NextMap.name == "ItemRoom")
                {
                    if(GameManager.Instance.Key >=1)
                    {
                        collision.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().isCome = true;
                        testScript.ChangeMap(collision.transform.parent.GetComponent<Rule>().NextMap, collision.transform.parent.GetComponent<Rule>().NextPosition);
                        GameManager.Instance.Key -= 1;
                        UIManager.Instance.SetKeyText();
                    }
                }
                else
                {
                    collision.transform.parent.GetComponent<Rule>().NextMap.gameObject.GetComponent<DungeonController>().isCome = true;
                    testScript.ChangeMap(collision.transform.parent.GetComponent<Rule>().NextMap, collision.transform.parent.GetComponent<Rule>().NextPosition);
                }                
            }
        }
    }   
}
