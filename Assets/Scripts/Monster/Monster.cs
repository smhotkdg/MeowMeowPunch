using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;
public class Monster : MonoBehaviour
{
    private readonly int OutLineColor = Shader.PropertyToID("_OutlineColor");
    private readonly int OutLineAlpha = Shader.PropertyToID("_OutlineAlpha");
    [Title("몬스터 상태")]
    public enum Status
    {
        Normal,
        Slow,
        Posion
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

    public GameObject Status_Sprite;
    float defaultHp;
    Material material;
    float defaultSpeed;
    private void Awake()
    {
        defaultSpeed = moveSpeed;
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        material = GetComponent<SpriteRenderer>().material;
    }
    private void Start()
    {
        SetHP();
     
    }
    private void OnEnable()
    {
        Status_Sprite.SetActive(false);
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
        material.SetColor(OutLineColor, _Color);
        material.SetFloat(OutLineAlpha, 1);
        moveSpeed = moveSpeed / 2;
        yield return new WaitForSeconds(2.5f);        
        moveSpeed = defaultSpeed;
        material.SetFloat(OutLineAlpha, 0);
        material.SetColor(OutLineColor, new Color(1,1,1,1));
        status = Status.Normal;
    }
    public bool isPosion = false;
    IEnumerator PosionRoutine()
    {
        isPosion = true;
        material.SetColor(OutLineColor, _Color);
        material.SetFloat(OutLineAlpha, 1);        
        yield return new WaitForSeconds(2f);                
        material.SetFloat(OutLineAlpha, 0);
        material.SetColor(OutLineColor, new Color(1, 1, 1, 1));
        status = Status.Normal;
        isPosion = false;
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
                    StartCoroutine(SlowRoutine());
                    break;
                case Status.Posion:
                    StartCoroutine(PosionRoutine());                  
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
                StartCoroutine(KnockbackRoutine(randVector));
                isKnockback = true;
            }
          
        }
        
        if(animator!=null)
        {
            animator.SetTrigger("hit");
        }        
        if(isPosion)
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
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Player")
        {            
            collision.gameObject.GetComponent<PlayerController>().Knockback(transform.position);
        }
    }
    void MoveObject()
    {
        if (Target == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, moveSpeed * Time.deltaTime);        
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
