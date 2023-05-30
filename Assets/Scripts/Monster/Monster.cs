using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;
using Pathfinding;
using EZ_Pooling;

public class Monster : MonoBehaviour
{
    public bool IsAnimationAttack = true;
    public GameObject pObject;
    public int MonsterHitDamage = 1;
    private readonly int OutLineColor = Shader.PropertyToID("_OutlineColor");
    private readonly int OutLineAlpha = Shader.PropertyToID("_OutlineAlpha");
    public bool isStartMonster = false;
    AIDestinationSetter Aisetter;
    Patrol patrol_Setter;
    [Title("비행")]
    public bool isFly = false;
    [Title("몬스터 상태")]
    public enum Status
    {
        Normal,
        Slow,
        Posion,        
        Fascination,
        Stren
    }
    [Serializable]
    public class MyPattern_Movement
    {
        [TableList]
        public MovementType movementTypes;
        [TableList]        
        public float Value;
    }
   

    public enum Pattern
    {
        single,
        Combination
    }
    [Title("몬스터 패턴")]
    public Pattern pattern = Pattern.single;
    public Status status = Status.Normal;


    [Title("패턴 리스트")]
    [ShowIf("pattern", Pattern.Combination)]
    [Searchable]
    public List<MyPattern_Movement> myPatterns;

    [Title("이동 타입")]
    public enum MovementType
    {
        None,
        //추적
        Tracking,        
        patrol,
        //랜덤
        Random,
        Reflect,
        mole
    }
    [ShowIf("pattern", Pattern.single)]
    public MovementType movementType;
    [ShowIf("@pattern == Pattern.single &&  movementType == MovementType.Tracking")]    
    public bool infinityTracking = false;
    [ShowIf("@pattern == Pattern.single")]
    public bool isRush = false;
    [ShowIf("@pattern == Pattern.single &&  isRush == true")]
    public float Rush_WaitTIme= 1f;
    [ShowIf("@pattern == Pattern.single &&  isRush == true")]
    public float RushSpeed = 1f;

    [ShowIf("movementType", MovementType.patrol)]
    public float PatrolWaitTime =1f;
    [ShowIf("movementType", MovementType.patrol)]
    public float PatrolDelay = 1f;

    [ShowIf("movementType", MovementType.patrol)]
    public Transform[] PatrolList;

    [ShowIf("movementType", MovementType.Random)]
    public float wiatTIme = 0.2f;
    [ShowIf("movementType", MovementType.Random)]
    public float MaxWait = 2f;

    [Title("공격 타입")]
    public enum AttackType
    {
        None,
        //미사일
        Missile,
        patternMissile,
        //장판
        Floor,
        SpawnMonster,
    }
    public enum PatternType
    {
        cross,
        directions_6,
        directions_8
    }
    [ShowIf("pattern",Pattern.single)]
    public AttackType attackType;    
    [ShowIf("@pattern == Pattern.single &&  attackType == AttackType.Missile")]
    public bool isRandomAttack = false;
    [ShowIf("attackType", AttackType.Missile)]
    public int BulletCount = 1;
    [ShowIf("@(BulletCount >1 && attackType == AttackType.Missile) || attackType == AttackType.patternMissile")]
    public bool isSIngleWay = false;
    [ShowIf("@(BulletCount >1 && attackType == AttackType.Missile) || attackType == AttackType.patternMissile")]
    public float DealyEachBullet = 0.1f;
    [ShowIf("attackType", AttackType.Missile)]
    public int BulletRange = 30;
    [ShowIf("attackType", AttackType.Missile)]
    public Transform BulletPos;
    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public float Shootspeed = 1;

    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public int MonsterBulletDamage= 1;

    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public Transform Bullet;
    //[ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    
    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public float defaultShootTImer;
    [ShowIf("attackType", AttackType.Missile)]
    public GameObject GunObject;
    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public bool InfinityAttack = false;
    [ShowIf("@attackType == AttackType.Missile || attackType == AttackType.patternMissile")]
    public bool dontNeedTarget = false;

    [ShowIf("attackType", AttackType.SpawnMonster)]
    public GameObject SpawanObject;
    [ShowIf("attackType", AttackType.SpawnMonster)]
    public float SpawnTime;



    [ShowIf("attackType", AttackType.patternMissile)]
    public PatternType patternType = PatternType.cross;


    [Title("사망 타입")]
    public enum DeathType
    {
        None,
        SpawnMonster,
        patternMissile,
    }
    [ShowIf("pattern", Pattern.single)]
    public DeathType deathType = DeathType.None;
        
    [ShowIf("@pattern == Pattern.single &&  deathType == DeathType.SpawnMonster")]
    public List<GameObject> DeathSpawnMonsterList = new List<GameObject>();
    [ShowIf("deathType", DeathType.SpawnMonster)]
    public int DeathSpawnCount;
    [ShowIf("@deathType==DeathType.SpawnMonster && DeathSpawnCount>0")]
    public bool DeathSpawnRandom =false;
    [Title("================================")]
    public float BaseHp =10;
    public float StageHp =0;

    [SerializeField]
    float Hp;
    [SerializeField]
    RangeSensor2D targetSensor;
    [SerializeField]
    RangeSensor2D attackSensor;
    public GameObject Target;
    [Title("넉백")]
    public bool enableKncokBack = true;

    public int MonsterIndex;
    private Animator animator;
    public delegate void DestoryEvent(int position);
    public event DestoryEvent DestoryEventHandler;
    public float ShootTimer;
    public CapsuleCollider2D capsuleCollider;
    public Transform healthBar;
    public Transform healthBarPrev;
    
    float defaultHp;
    Material material;
    float defaultSpeed;
    [SerializeField]
    //여기를 받아오고 Bool List로 삭제 안하도록 변경
    public List<GameObject> StatusList = new List<GameObject>();
    List<bool> StatusBool = new List<bool>();
    List<GameObject> StatusTemp = new List<GameObject>();
    AILerp iLerp;
    GameObject RandomSpawn;
    float spawnDeltaTime;
    SpriteRenderer spriteRenderer;
    IEnumerator CheckMoveRoutine;

    float defaultRushTime =0;
    private void Awake()
    {
        defaultRushTime = Rush_WaitTIme;
        spawnDeltaTime = SpawnTime;
        ShootTimer = defaultShootTImer;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultShootTImer = ShootTimer;
        RandomSpawn = transform.Find("RandomSpawn").gameObject;
        defaultSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        material = GetComponent<SpriteRenderer>().material;
        Aisetter = GetComponent<AIDestinationSetter>();
        patrol_Setter = GetComponent<Patrol>();
        iLerp = GetComponent<AILerp>();
        CheckMoveRoutine = CheckMoveCoRoutine();
        StartCoroutine(CheckMoveRoutine);
    }
    private void OnDisable()
    {
        StopCoroutine(CheckMoveRoutine);
    }
    bool isMove =false;
    IEnumerator CheckMoveCoRoutine()
    {        
        while (true)
        {
            Vector3 lastPos = transform.position;
            yield return new WaitForSeconds(0.1f);
            if(lastPos == transform.position)
            {
                isMove = false;
            }
            else
            {
                if(forceAttack ==false)
                    isMove = true;
            }            
        }
    }
    private void Start()
    {
        SetHP();
        for(int i=0; i< StatusList.Count; i++)
        {
            StatusBool.Add(false);
        }
    }
    private void OnEnable()
    {
        isStartMonster = false;
    }
    [Button]
    public void SetHP()
    {
        float temp_hp;
        float bound = 0.8f * Mathf.Min(GameManager.Instance.Stage - 5, 5);
        if(bound >0)
        {
            temp_hp = BaseHp + (Mathf.Min(4, GameManager.Instance.Stage) + bound) * StageHp;
        }
        else
        {
            temp_hp = BaseHp + (Mathf.Min(4, GameManager.Instance.Stage))*StageHp;
        }
        Hp = temp_hp;
        defaultHp = temp_hp;
    }
    public void SetBarSize(float sizeNormalized)
    {        
        //healthBar.localScale = new Vector3(sizeNormalized, 1f);
        healthBar.DOScale(new Vector3(sizeNormalized, 1f), 0.1f).SetEase(Ease.InSine);
        healthBarPrev.DOScale(new Vector3(sizeNormalized, 1f), 0.1f).SetEase(Ease.InSine).SetDelay(0.1f);
    }
    Color _Color;
    
    IEnumerator SlowRoutine()
    {
        StatusBool[(int)Status.Slow] = true;
        moveSpeed = moveSpeed / 2;
        yield return new WaitForSeconds(2.5f);        
        moveSpeed = defaultSpeed;
        StatusBool[(int)Status.Slow] = false;
        status = Status.Normal;
        for (int i =0; i< StatusTemp.Count; i++)
        {
            if(StatusTemp[i].name == "slow")
            {
                StatusTemp[i].SetActive(false);
                StatusTemp.RemoveAt(i);
                yield return null;
            }
        }
       
    }    
    IEnumerator PosionRoutine()
    {        
        StatusBool[(int)Status.Posion] = true;
        yield return new WaitForSeconds(2f);           
        status = Status.Normal;
        StatusBool[(int)Status.Posion] = false;
        for (int i = 0; i < StatusTemp.Count; i++)
        {
            if (StatusTemp[i].name == "posion")
            {
                StatusTemp[i].SetActive(false);
                StatusTemp.RemoveAt(i);
                yield return null;
            }
        }
    }

    IEnumerator fascinationRoutine()
    {
        StatusBool[(int)Status.Fascination] = true;
        yield return new WaitForSeconds(1f);
        status = Status.Normal;
        StatusBool[(int)Status.Fascination] = false;
        for (int i = 0; i < StatusTemp.Count; i++)
        {
            if (StatusTemp[i].name == "Fascination")
            {
                StatusTemp[i].SetActive(false);
                StatusTemp.RemoveAt(i);
                yield return null;
            }
        }
    }
    IEnumerator SternRoutine()
    {
        StatusBool[(int)Status.Stren] = true;
        yield return new WaitForSeconds(2f);
        status = Status.Normal;
        StatusBool[(int)Status.Stren] = false;
        for (int i = 0; i < StatusTemp.Count; i++)
        {
            if (StatusTemp[i].name == "Stren")
            {
                StatusTemp[i].SetActive(false);
                StatusTemp.RemoveAt(i);
                yield return null;
            }
        }        
    }
    [SerializeField]
    float moveSpeed = 1;
    bool isKnockback =false;
    public void SetDamage(float damage,Vector2 knockback,Status _status,Color changeColor)
    {
        if(status != _status)
        {
            _Color = changeColor;
            status = _status;
            switch (status)
            {
                case Status.Normal:
                    break;
                case Status.Slow:
                    if(StatusBool[(int)Status.Slow] == false)
                    {                       
                        StartCoroutine(SlowRoutine());
                        StatusList[(int)Status.Slow].name = "slow";
                        StatusTemp.Add(StatusList[(int)Status.Slow]);
                    }                  
                    break;
                case Status.Posion:
                    if(StatusBool[(int)Status.Posion] == false)
                    {
                        StartCoroutine(PosionRoutine());
                        StatusList[(int)Status.Posion].name = "posion";
                        StatusTemp.Add(StatusList[(int)Status.Posion]);
                    }                  
                    break;              
                case Status.Fascination:
                    if (StatusBool[(int)Status.Fascination] == false)
                    {
                        StartCoroutine(fascinationRoutine());
                        StatusList[(int)Status.Fascination].name = "Fascination";
                        StatusTemp.Add(StatusList[(int)Status.Fascination]);
                    }
                    break;
                case Status.Stren:
                    if (StatusBool[(int)Status.Stren] == false)
                    {
                        StartCoroutine(SternRoutine());
                        StatusList[(int)Status.Stren].name = "Stren";
                        StatusTemp.Add(StatusList[(int)Status.Stren]);
                    }
                    break;
            }
        }
      
        if(isKnockback ==false)
        {
            Vector2 randVector;
            float randx = UnityEngine.Random.Range(-0.25f, 0.25f);
            float randy = UnityEngine.Random.Range(-0.25f, 0.25f);
            randVector.x = knockback.x + randx;
            randVector.y = knockback.y + randy;
            if(enableKncokBack)
            {
                iLerp.canMove = false;
                StartCoroutine(KnockbackRoutine(randVector));
                isKnockback = true;
            }
          
        }
        
        if(animator!=null)
        {
            animator.SetTrigger("hit");
        }        
        if(StatusBool[(int)Status.Posion])
        {
            StartCoroutine(PosionAttackRoutine(damage));
        }
        Hp -= damage;
        SetBarSize(Hp / defaultHp);
        if (Hp<=0)
        {
            Death();
            DestoryEventHandler?.Invoke(MonsterIndex);
            Destroy(this.gameObject);
        }
        
    }
    IEnumerator PosionAttackRoutine(float _damage)
    {
        yield return new WaitForSeconds(0.1f);
        Hp -= _damage;
        SetBarSize(Hp / defaultHp);
        UIManager.Instance.SetDamageNumber(gameObject, _damage);
    }
    IEnumerator KnockbackRoutine(Vector3 knockback)
    {        
        float ctime = 0;     
        while (ctime < .2f)
        {
            //transform.Translate(Vector2.left * moveSpeed * Time.deltaTime*1);
            transform.Translate(knockback * moveSpeed * Time.deltaTime);
            ctime += Time.deltaTime;
            yield return null;
        }        
        isKnockback = false;  
    }
    private void LateUpdate()
    {
        CheckFlip();
    }
    private void Update()
    {

        if (isStartMonster == false)
            return;
        TargetCheck();
        MoveObject();
        
        CheckStatus();
        //UpdateBulletPos();
        FindAttackTarget();
        if(status != Status.Stren)
        {
            if (attackType == AttackType.Missile || attackType == AttackType.patternMissile)
            {
                CheckAttack();
            }                
            if(attackType == AttackType.SpawnMonster)
            {
                CheckSpwan();
            }            
        }   
        if(isRush)
        {
            Rush_WaitTIme -= Time.deltaTime;
        }
        if(movementType == MovementType.Reflect)
        {            
            transform.position = Vector2.MoveTowards(transform.position, RandomSpawn.transform.position, moveSpeed * Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {       
        if(isMove)
        {
            animator.SetFloat("MotionX", 1);
            animator.SetFloat("MotionY", 1);
        }
        else
        {
            animator.SetFloat("MotionX", 0);
            animator.SetFloat("MotionY", 0);
        }
    }
    void FindAttackTarget()
    {
        if(attackType == AttackType.Missile || attackType == AttackType.patternMissile)
        {
            if (attackSensor.DetectedObjects.Count <= 0)
            {
                if(!InfinityAttack)
                {
                    AttackTarget = null;
                }
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
            AttackTarget = attackTarget;
        }       
    }
    void UpdateBulletPos()
    {
        if (AttackTarget != null)
        {
            Vector2 direction = AttackTarget.transform.position - GunObject.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GunObject.transform.rotation = Quaternion.Lerp(GunObject.transform.rotation, rotation, Time.deltaTime * 5000);
            //gunsprite.flipY = !spriteRenderer.flipX;
            //if (Target.transform.position.x < transform.position.x)
            //{
            //    GunObject.transform.localScale = new Vector3(-1, -1, 1);
            //}
            //else
            //{
            //    GunObject.transform.localScale = new Vector3(-1, 1, 1);
            //}
            //gunsprite.flipX = false;
        }
        else
        {

            //Quaternion rotation = Quaternion.AngleAxis(180, Vector3.forward);
            //GunObject.transform.rotation = Quaternion.Lerp(GunObject.transform.rotation, rotation, Time.deltaTime * 8);
            ////gunsprite.flipX = spriteRenderer.flipX;
            ////gunsprite.flipY = true;
            //GunObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    bool shoot = false;
    void Shoot(float angle, bool isSingleShoot, bool randomShoot)
    {
        if (isSingleShoot)
        {
            for (int i = 0; i < BulletCount; i++)
            {
                float range = 90;
                float rangle_bullet = 0;
                if (i % 2 == 0)
                {
                    range = 90 + (BulletRange * ((i + 1) / 2) * (1));
                    rangle_bullet = ((i + 1) / 2) * (1);
                }
                else
                {
                    range = 90 + (BulletRange * ((i + 1) / 2) * (-1));
                    rangle_bullet = ((i + 1) / 2) * (-1);
                }

                // 90 110 70 130 50 
                Vector2 moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2)) * Shootspeed;
                Transform object_transfrom;
                if (randomShoot)
                {
                    float range_temp = 90;
                    range_temp = UnityEngine.Random.Range(0f, 361);
                    GunObject.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0.0f, 360.0f));
                    object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(range_temp, Vector3.forward));
                }
                else
                {
                    object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range, Vector3.forward));
                }
                object_transfrom.gameObject.SetActive(false);
                object_transfrom.GetComponent<Bullet>().bulletType = DamageColider.BulletType.monster;
                object_transfrom.GetComponent<Bullet>().SetSpeed(Shootspeed);
                object_transfrom.GetComponent<Bullet>().Power = MonsterBulletDamage;

                //object_transfrom.GetComponent<Rigidbody2D>().velocity = BulletPos.transform.up * speed;
                object_transfrom.gameObject.SetActive(true);
                object_transfrom.GetComponent<Rigidbody2D>().velocity = moveVec;
                object_transfrom.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.forward;
                object_transfrom.GetComponent<Bullet>().SetVelocity();               

                Vector2 dir = object_transfrom.GetComponent<Rigidbody2D>().velocity;
                float angle_reset = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                object_transfrom.transform.rotation = Quaternion.AngleAxis(angle_reset - 90, Vector3.forward);
               
            }
        }
        else
        {           
            StartCoroutine(BulletDelayRoutine(randomShoot,angle));            
        }
    }
    IEnumerator BulletDelayRoutine(bool randomShoot,float angle)
    {
        for (int i = 0; i < BulletCount; i++)
        {
            if (status == Status.Stren)
            {
                break;
            }
            float range = 90;
            float rangle_bullet = 0;
            if (isSIngleWay == false)
            {
                if (i % 2 == 0)
                {
                    range = 90 + (BulletRange * ((i + 1) / 2) * (1));
                    rangle_bullet = ((i + 1) / 2) * (1);
                }
                else
                {
                    range = 90 + (BulletRange * ((i + 1) / 2) * (-1));
                    rangle_bullet = ((i + 1) / 2) * (-1);
                }
            }          
            
            // 90 110 70 130 50 
            Vector2 moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2)) * Shootspeed;
            Transform object_transfrom;
            if (randomShoot)
            {
                float range_temp = 90;
                range_temp = UnityEngine.Random.Range(0f, 361);
                GunObject.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0.0f, 360.0f));
                object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(range_temp, Vector3.forward));
            }
            else
            {
                object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(angle - range, Vector3.forward));
            }
            object_transfrom.gameObject.SetActive(false);
            object_transfrom.GetComponent<Bullet>().bulletType = DamageColider.BulletType.monster;
            object_transfrom.GetComponent<Bullet>().SetSpeed(Shootspeed);
            object_transfrom.GetComponent<Bullet>().Power = MonsterBulletDamage;

            //object_transfrom.GetComponent<Rigidbody2D>().velocity = BulletPos.transform.up * speed;
            object_transfrom.gameObject.SetActive(true);
            object_transfrom.GetComponent<Rigidbody2D>().velocity = moveVec;
            object_transfrom.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.forward;
            object_transfrom.GetComponent<Bullet>().SetVelocity();
            if(randomShoot)
            {

                Vector2 dir = object_transfrom.GetComponent<Rigidbody2D>().velocity;
                float angle_reset = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                object_transfrom.transform.rotation = Quaternion.AngleAxis(angle_reset - 90, Vector3.forward);
            }
            yield return new WaitForSeconds(DealyEachBullet);
        }
    }
    bool forceAttack = false;
    void CheckAttack()
    {
        if(shoot ==false)
        {
            ShootTimer -= Time.deltaTime;
        }            
        if (ShootTimer <= 0)
        {
            shoot = true;
            ShootTimer = defaultShootTImer;
        }
        if (shoot)
        {
            shoot = false;            
            if (IsAnimationAttack)
            {
                iLerp.canMove = false;
                forceAttack = true;
                animator.SetTrigger("Attack");
            }
            else
            {
                AnimAttack();                
            }
        }      
    }
    IEnumerator PatternBulletDelayRoutine(float count)
    {
        float range_temp = 360f/count;        
        for (int i = 0; i < count; i++)
        {
            if (status == Status.Stren)
            {
                break;
            }
            float rangle_bullet = 0;
            float gunAngle = range_temp * i;
            GunObject.transform.rotation = Quaternion.Euler(0, 0, gunAngle);

            Vector2 moveVec = (BulletPos.transform.up + (BulletPos.transform.right * rangle_bullet / 2)) * Shootspeed;
            Transform object_transfrom;                      
            
            object_transfrom = EZ_PoolManager.Spawn(Bullet.transform, BulletPos.transform.position, Quaternion.AngleAxis(0,new Vector3(0,0,0)));
           
          
            object_transfrom.gameObject.SetActive(false);
            object_transfrom.GetComponent<Bullet>().bulletType = DamageColider.BulletType.monster;
            object_transfrom.GetComponent<Bullet>().SetSpeed(Shootspeed);
            object_transfrom.GetComponent<Bullet>().Power = MonsterBulletDamage;

            //object_transfrom.GetComponent<Rigidbody2D>().velocity = BulletPos.transform.up * speed;
            object_transfrom.gameObject.SetActive(true);
            object_transfrom.GetComponent<Rigidbody2D>().velocity = moveVec;
            object_transfrom.GetComponent<Bullet>().bulletDirection = global::Bullet.BulletDirection.forward;
            object_transfrom.GetComponent<Bullet>().SetVelocity();


            Vector2 dir = object_transfrom.GetComponent<Rigidbody2D>().velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            object_transfrom.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            yield return new WaitForSeconds(DealyEachBullet);
        }
    }
    void PatternShot()
    {
        shoot = false;
        Vector3 TargetVec;
        TargetVec = new Vector3(0, 0, 0);
        //TargetVec.x += AttackTarget.transform.position.x;
        //TargetVec.y += AttackTarget.transform.position.y;
        if (!dontNeedTarget)
        {
            if(AttackTarget !=null)
            {
                TargetVec.x += AttackTarget.transform.position.x;
                TargetVec.y += AttackTarget.transform.position.y;
            }
            else
            {
                return;
            }
        }
        
       
        Vector2 direction = TargetVec - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        if (GunObject.transform.localScale.y < 0)
        {
            BulletPos.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            BulletPos.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        
        switch (patternType)
        {
            case PatternType.cross:
                StartCoroutine(PatternBulletDelayRoutine(4));
                break;
            case PatternType.directions_6:
                StartCoroutine(PatternBulletDelayRoutine(6));
                break;
            case PatternType.directions_8:
                StartCoroutine(PatternBulletDelayRoutine(8));
                break;
        }
        
    }
    void NoramlMisslie()
    {
        if (AttackTarget != null)
        {
            Vector3 TargetVec;

            TargetVec = new Vector3(0, 0, 0);

            TargetVec.x += AttackTarget.transform.position.x;
            TargetVec.y += AttackTarget.transform.position.y;
            Vector2 direction = TargetVec - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * Rotationspeed);
            if (GunObject.transform.localScale.y < 0)
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, 90);
            }                          
            if (isRandomAttack == false)
            {
                UpdateBulletPos();
                if (BulletCount == 1)
                {
                    Shoot(angle, true, false);
                }
                else
                {
                    Shoot(angle, false, false);
                }
            }
            else
            {
                if (BulletCount == 1)
                {
                    Shoot(angle, true, true);
                }
                else
                {
                    Shoot(angle, false, true);
                }
            }            
        }
        else if (dontNeedTarget)
        {
            Vector3 TargetVec;

            TargetVec = new Vector3(0, 0, 0);
            Vector2 direction = TargetVec - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * Rotationspeed);
            if (GunObject.transform.localScale.y < 0)
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                BulletPos.transform.localEulerAngles = new Vector3(0, 0, 90);
            }          
            if (isRandomAttack == false)
            {
                UpdateBulletPos();
                if (BulletCount == 1)
                {
                    Shoot(angle, true, false);
                }
                else
                {
                    Shoot(angle, false, false);
                }
            }
            else
            {
                if (BulletCount == 1)
                {
                    Shoot(angle, true, true);
                }
                else
                {
                    Shoot(angle, false, true);
                }
            }            
        }
    }
    void CheckStatus()
    {
        Vector2 pos= StatusList[0].transform.localPosition;
        for (int i =0; i< StatusTemp.Count; i++)
        {
            pos.x = 0.15f * i;
            StatusTemp[i].SetActive(true);
            StatusTemp[i].transform.localPosition = pos;
        }
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if(collision.gameObject.tag =="Player")
        {            
            collision.gameObject.GetComponent<PlayerController>().Knockback(transform.position,-MonsterHitDamage);
        }
        if(movementType == MovementType.Reflect)
        {
            Vector3 dir = Vector3.Reflect(iLerp.velocity, collision.GetContact(0).normal);
            dir = dir * 10;
            RandomSpawn.transform.position = dir;            
        }        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    void MoveObject()
    {
        MoveTypeChecker();        
        if (StatusBool[(int)Status.Stren])
        {
            iLerp.canMove = false;
            return;
        }
        if (StatusBool[(int)Status.Fascination])
        {
            if(movementType != MovementType.None)
            {
                transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position, -moveSpeed / 2 * Time.deltaTime);
            }            
            //iLerp.SearchPath();
            iLerp.canMove = false;
        }       
        
        if(StatusBool[(int)Status.Fascination] ==false && isKnockback ==false)
        {            
            if(Target !=null)
            {
                Aisetter.target = Target.transform;
            }                
            if(forceAttack ==false)
                iLerp.canMove = true;            
        }        
    }

    GameObject AttackTarget;
    bool isMoveRandom = false;
    public Vector2 RandomMoveVector;
    IEnumerator RandomMoveRoutine()
    {        
        isMoveRandom = true;
        Vector3 pos = transform.position;
        pos.x += UnityEngine.Random.Range(-1f,1f);
        pos.y += UnityEngine.Random.Range(-1f, 1f);
        NNInfo info = AstarPath.active.GetNearest(pos);
        GraphNode node = info.node;
        
        Vector3 randomPoint = node.RandomPointOnSurface();
        RandomMoveVector = randomPoint;
        RandomSpawn.transform.SetParent(transform.parent);
        RandomSpawn.transform.position = randomPoint;        
        Aisetter.target = RandomSpawn.transform;
        Target = RandomSpawn;
        float endWiat = MaxWait;
        while (!iLerp.reachedDestination)
        {
            endWiat -= Time.deltaTime;
            iLerp.SearchPath();
            if(endWiat <=0)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(wiatTIme);
        isMoveRandom = false;
        RandomSpawn.transform.SetParent(transform);
    }
    IEnumerator RandomMoveCoRoutine;
    IEnumerator PatrolCoRoutine;
    IEnumerator RushRoutine()
    {
        // 여기를 러쉬 로
        Vector3 pos = GameManager.Instance.Player.transform.position;    
        NNInfo info = AstarPath.active.GetNearest(pos);
        GraphNode node = info.node;        
        iLerp.speed = RushSpeed;     
        RandomSpawn.transform.SetParent(transform.parent);
        RandomSpawn.transform.position = pos;
        Aisetter.target = RandomSpawn.transform;
        float endWiat = MaxWait;
        while (!iLerp.reachedDestination)
        {
            endWiat -= Time.deltaTime;
            iLerp.SearchPath();
            if (endWiat <= 0)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(wiatTIme);        
        RandomSpawn.transform.SetParent(transform);
        //
        Rush_WaitTIme = defaultRushTime;
        iLerp.SearchPath();
        isStartRush = false;        
    }
    bool isStartRush = false;
    void MoveTypeChecker()
    {
        if (isRush == true && Rush_WaitTIme <= 0)
        {
            if(isStartRush ==false)
            {
                //   transform.DOMove(GameManager.Instance.Player.transform.position, Vector2.Distance(GameManager.Instance.Player.transform.position, transform.position)).SetEase(Ease.OutSine)
                //.OnComplete(CompleteRush);
                StartCoroutine(RushRoutine());
                //iLerp.canMove = false;
                CheckPlayerFlip();
                if (RandomMoveCoRoutine != null)
                {
                    StopCoroutine(RandomMoveCoRoutine);
                    isMoveRandom = false;
                }
                if(PatrolCoRoutine !=null)
                {
                    Aisetter.enabled = true;
                    patrol_Setter.enabled = false;
                    StopCoroutine(PatrolCoRoutine);
                    isStartPatrol = false;
                }
                    
                isStartRush = true;
            }
         
        }
        else
        {
            iLerp.speed = moveSpeed;
            switch (movementType)
            {
                case MovementType.None:
                    Aisetter.enabled = true;
                    patrol_Setter.enabled = false;
                    break;
                case MovementType.Random:
                    Aisetter.enabled = true;
                    patrol_Setter.enabled = false;
                    if (isMoveRandom == false)
                    {
                        RandomMoveCoRoutine = RandomMoveRoutine();
                        StartCoroutine(RandomMoveCoRoutine);
                    }
                    break;
                case MovementType.Tracking:
                    Aisetter.enabled = true;
                    patrol_Setter.enabled = false;
                    if (Target == null)
                    {
                        iLerp.canMove = false;
                    }
                    else
                    {
                        iLerp.SearchPath();
                    }
                    break;
                case MovementType.patrol:
                    if(isStartPatrol ==false)
                    {
                        PatrolCoRoutine = PatrolRoutine();
                        StartCoroutine(PatrolCoRoutine);
                    }                    
                    break;
                case MovementType.Reflect:
                    if(bStartReflect ==false)
                    {                        
                        Aisetter.enabled = false;
                        patrol_Setter.enabled = false;
                        StartReflect();
                        bStartReflect = true;
                    }                 
                    //iLerp.SearchPath();                    
                    break;
            }
        }
    }
    
    
    bool bStartReflect = false;
    public void StartReflect()
    {        
        Vector3 pos = transform.position;
        pos.x += UnityEngine.Random.Range(-50f, 50f);
        pos.y += UnityEngine.Random.Range(-50f, 50f);

        RandomMoveVector = pos;
        RandomSpawn.transform.SetParent(transform.parent);
        RandomSpawn.transform.position = pos;
        Aisetter.target = RandomSpawn.transform;
        Target = RandomSpawn;
        //iLerp.SearchPath();
    }
    bool isStartPatrol = false;
    
    IEnumerator PatrolRoutine()
    {
        Aisetter.enabled = false;
        patrol_Setter.enabled = true;
        isStartPatrol = true;
        if (PatrolList.Length >0)
        {
            patrol_Setter.delay = PatrolDelay;
            patrol_Setter.targets = PatrolList;
            
            yield return new WaitForSeconds(PatrolWaitTime);              
            
            while (!iLerp.reachedDestination)
            {                
                iLerp.SearchPath();
                Target = patrol_Setter.GetTarget().gameObject;
                if (iLerp.reachedDestination)
                {
                    break;
                }
                yield return null;
            }
            Target = null;
            yield return new WaitForSeconds(0.1f);                      
            iLerp.SearchPath();            
        }
        
        isStartPatrol = false;
        Aisetter.enabled = true;
        patrol_Setter.enabled = false;
  
    }
    void TargetCheck()
    {
        
        if(movementType != MovementType.Tracking)
        {
            return;
        }
        if (targetSensor.DetectedObjects.Count <= 0)
        {
            if(infinityTracking == false)
                Target = null;
            return;
        }
        if(attackSensor.DetectedObjects.Count <=0)
        {
            if (infinityTracking == false)
                Target = null;
            return;
        }
        else
        {
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
            Target = attackTarget;
        }
    }
    void CheckPlayerFlip()
    {
      
        if (GameManager.Instance.Player.transform.position.x < transform.position.x)
        {
            //transform.localScale = new Vector3(-1, 1, 1);
            spriteRenderer.flipX = true;
            //healthBar.parent.localScale = new Vector3(-1, 1, 1);
            //healthBarPrev.parent.localScale = new Vector3(-1,1,1);
        }
        else
        {
            //transform.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
            //healthBar.parent.localScale = new Vector3(1, 1, 1);
            //healthBarPrev.parent.localScale = new Vector3(1, 1, 1);
        }
        
    }
    void CheckFlip()
    {
        if(isStartRush)
        {
            CheckPlayerFlip();
        }
        else
        {
            if (Target != null)
            {
                if (Target.transform.position.x < transform.position.x)
                {
                    //transform.localScale = new Vector3(-1, 1, 1);
                    spriteRenderer.flipX = true;
                    //healthBar.parent.localScale = new Vector3(-1, 1, 1);
                    //healthBarPrev.parent.localScale = new Vector3(-1,1,1);
                }
                else
                {
                    //transform.localScale = new Vector3(1, 1, 1);
                    spriteRenderer.flipX = false;
                    //healthBar.parent.localScale = new Vector3(1, 1, 1);
                    //healthBarPrev.parent.localScale = new Vector3(1, 1, 1);
                }
            }

            if (AttackTarget != null)
            {
                if (AttackTarget.transform.position.x < transform.position.x)
                {
                    //transform.localScale = new Vector3(-1, 1, 1);
                    spriteRenderer.flipX = true;
                    //healthBar.parent.localScale = new Vector3(-1, 1, 1);
                    //healthBarPrev.parent.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    //transform.localScale = new Vector3(1, 1, 1);
                    spriteRenderer.flipX = false;
                    //healthBar.parent.localScale = new Vector3(1, 1, 1);
                    //healthBarPrev.parent.localScale = new Vector3(1, 1, 1);
                }
            }

            if (Target == null && AttackTarget == null)
            {
                //if (RandomMoveVector.x < transform.position.x)
                //{
                //    spriteRenderer.flipX = true;
                //}
                //else
                //{
                //    spriteRenderer.flipX = false;
                //}
            }
        }
       
    }
    bool isSpawn =false;
    void CheckSpwan()
    {
        if (isSpawn == false)
        {
            spawnDeltaTime -= Time.deltaTime;
        }
        if (spawnDeltaTime <= 0)
        {
            isSpawn = true;
            spawnDeltaTime = SpawnTime;
        }
        if (isSpawn)
        {
            isSpawn = false;
            if (IsAnimationAttack)
            {
                iLerp.canMove = false;
                forceAttack = true;
                animator.SetTrigger("Attack");
            }
            else
            {
                Spawn();
            }
        }
    }
    void Death()
    {
        GameManager.Instance.SetHp13Count();
        RandomSpawn.transform.SetParent(transform);
        switch (deathType)
        {
            case DeathType.SpawnMonster:               
                for(int i =0; i< DeathSpawnCount; i++)
                {
                    int randomCount = i;
                    if (DeathSpawnRandom)
                    {
                        randomCount = UnityEngine.Random.Range(0, DeathSpawnMonsterList.Count);
                    }
                    GameObject temp = Instantiate(DeathSpawnMonsterList[randomCount]);

                    temp.transform.SetParent(transform.parent);
                    temp.transform.localScale = new Vector3(1, 1, 1);
                    temp.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - 0.3f, transform.position.x + 0.3f),
                        UnityEngine.Random.Range(transform.position.y - 0.3f, transform.position.y + 0.3f));
                    temp.GetComponent<Monster>().isStartMonster = true;
                    if (pObject != null)
                    {
                        pObject.GetComponent<DungeonController>().AddObject(temp);
                    }
                }                 
                
                break;
        }
    }
    void Spawn()
    {
        GameObject temp = Instantiate(SpawanObject);
        temp.transform.SetParent(transform.parent);
        temp.transform.localScale = new Vector3(1, 1, 1);
        temp.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - 0.3f, transform.position.x + 0.3f),
            UnityEngine.Random.Range(transform.position.y - 0.3f, transform.position.y + 0.3f));
        temp.GetComponent<Monster>().isStartMonster = true;
        if (pObject != null)
        {
            pObject.GetComponent<DungeonController>().AddObject(temp);
        }
    }
    public void AnimAttack()
    {        
        switch (attackType)
        {
            case AttackType.Missile:
                NoramlMisslie();
                break;
            case AttackType.patternMissile:
                PatternShot();
                break;
            case AttackType.SpawnMonster:
              
                break;
        }        
    }
    public void EndAttack()
    {
        forceAttack = false;
        iLerp.canMove = true;
    }
}
