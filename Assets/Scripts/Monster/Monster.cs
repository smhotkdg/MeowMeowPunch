using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;
using Pathfinding;

public class Monster : MonoBehaviour
{
    public int MonsterHitDamage = 1;
    private readonly int OutLineColor = Shader.PropertyToID("_OutlineColor");
    private readonly int OutLineAlpha = Shader.PropertyToID("_OutlineAlpha");
    AIDestinationSetter Aisetter;
    [Title("몬스터 상태")]
    public enum Status
    {
        Normal,
        Slow,
        Posion,        
        Fascination,
        Stren
    }
    public Status status = Status.Normal;
    [Title("이동 타입")]
    public enum MovementType
    {
        //추적
        Tracking,
        //대각선
        Diagonally,
        //랜덤
        Random,
        //상하좌우
        UpDown_LeftRight,
        //두더지
        mole
    }
    public MovementType movementType;
    [ShowIf("movementType", MovementType.Tracking)]
    public bool infinityTracking = false;

    [Title("공격 타입")]
    public enum AttackType
    {
        None,
        //미사일
        Missile,
        //장판
        Floor
    }
    public AttackType attackType;
    [ShowIf("attackType", AttackType.Missile)]
    public bool isRandomAttack = false;
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

    private void Awake()
    {
        defaultSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        material = GetComponent<SpriteRenderer>().material;
        Aisetter = GetComponent<AIDestinationSetter>();
        iLerp = GetComponent<AILerp>();
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
    private void Update()
    {
        TargetCheck();
        MoveObject();
        CheckFlip();
        CheckStatus();
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
    }
    void MoveObject()
    {
        iLerp.SearchPath();
        if (StatusBool[(int)Status.Stren])
        {
            iLerp.canMove = false;
            return;
        }
        if (StatusBool[(int)Status.Fascination])
        {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position, -moveSpeed/2 * Time.deltaTime);
            //iLerp.SearchPath();
            iLerp.canMove = false;
        }       
        if (Target == null)
            return;
        if(StatusBool[(int)Status.Fascination] ==false && isKnockback ==false)
        {
            //transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, moveSpeed * Time.deltaTime);
            Aisetter.target = Target.transform;
            iLerp.canMove = true;
            iLerp.speed = moveSpeed;
            //iLerp.SearchPath();            
        }        
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
    void CheckFlip()
    {
        if (Target == null)
        {
            return;
        }
        else
        {
            if (Target.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                healthBar.parent.localScale = new Vector3(-1, 1, 1);
                healthBarPrev.parent.localScale = new Vector3(-1,1,1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                healthBar.parent.localScale = new Vector3(1, 1, 1);
                healthBarPrev.parent.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
