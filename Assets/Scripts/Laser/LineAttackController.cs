using SensorToolkit;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class LineAttackController : DamageColider
{
    public enum LineType
    {
        None,
        Right,
        Left,
        Back
    }
    public int BulletCount = 1;
    public int MaxHomingTarget = 3;
    public Transform LaserCollisionEffect;
    public LineType lineType = LineType.None;
    public RaySensor2D raySensor2D;
    public RaySensor2D collsionRay;
    public float interval = 1.0f; // 호출 간격
    private float timer = 0.0f;

    float defalutInterval;
    public List<SpriteShape> spriteShapes;
    public float tangentLength = 1.0f;

    public List<GameObject> TestObjectList;

    public SpriteShapeController spriteShapeController;
    public SpriteShapeRenderer spriteShapeRenderer;

    Spline spline;
    GameObject m_target;

    List<bool> m_attackTypes = new List<bool>();
    List<bool> m_attackMethods = new List<bool>();


    List<Vector2> RightRandomAngle = new List<Vector2>();
    List<Vector2> LeftRandomAngle = new List<Vector2>();
    EdgeCollider2D edgeCollider;
    Vector2 Init_0 = new Vector2();
    Vector2 Init_1 = new Vector2();
    
    private void Awake()
    {
        spline = spriteShapeController.spline;
        defalutInterval = interval;
        edgeCollider = GetComponent<EdgeCollider2D>();
        Init_0 = spline.GetPosition(0);
        Init_1 = spline.GetPosition(1);
    }


    private void CollsionRay_OnSensorUpdate()
    {
        //collsionRay.OnSensorUpdate -= CollsionRay_OnSensorUpdate;

        //bInit = true;
        //InitShoot();
        //CheckHoming();
    }

    [Button]
    public void AddLIne(int index)
    {
        int pos = spline.GetPointCount() - 1;
        //if (Vector2.Distance(spline.GetPosition(pos), Target.transform.position) > 0.05f)
        for (int i = 0; i < spline.GetPointCount(); i++)
        {
            if (Vector2.Distance(spline.GetPosition(i), EnmeyVecList[index]) < 0.1f)
                return;
        }
        if (Vector2.Distance(spline.GetPosition(pos), EnmeyVecList[index]) > 0.1f)
        {
            //Debug.Log(Vector2.Distance(spline.GetPosition(pos), TargetList[TargetList.Count - 1].transform.position));
            spline.InsertPointAt(pos, EnmeyVecList[index]);
            spline.SetTangentMode(pos, ShapeTangentMode.Continuous);
            spline.SetRightTangent(pos, RightRandomAngle[index] * tangentLength);
            spline.SetLeftTangent(pos, LeftRandomAngle[index] * tangentLength);
            //spline.SetRightTangent(pos, new Vector3(Random.Range(0f, 0.2f), Random.Range(-0.15f, 0.3f)) * tangentLength);
            //spline.SetLeftTangent(pos, new Vector3(Random.Range(-0.25f, 0f), Random.Range(-0.25f, 0.25f)) * tangentLength);
        }

    }
    Vector3 TargetPosition;
    List<Transform> EffectList = new List<Transform>();
    public override void OnObstacleEnterLaser(Vector3 HitPoint)
    {
        Transform tempEffect = EZ_Pooling.EZ_PoolManager.Spawn(LaserCollisionEffect, HitPoint, new Quaternion());
        bool isSame = false;
        for (int i = 0; i < EffectList.Count; i++)
        {
            if (EffectList[i] == tempEffect)
            {
                isSame = true;
            }
        }
        if (isSame == false)
        {
            EffectList.Add(tempEffect);
        }
        tempEffect.GetComponent<AutoDestory>().DestoryTime = GetComponent<AutoDestory>().DestoryTime;
    }
    
    void InitShoot()
    {
        if (m_target != null)
        {
            Vector3 pos = GameManager.Instance.playerController.shootController.transform.position;
            Vector3 dir = (GameManager.Instance.playerController.shootController.transform.position - TargetPosition).normalized;
            switch(lineType)
            {
                case LineType.None:
                    if (isRotateAngle)
                    {
                        dir = GetRotateVector(dir, angle_rotate);
                        collsionRay.transform.localRotation = Quaternion.Euler(0, 0, angle_rotate);
                    }
                    else
                    {
                        collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }                    
                    break;
                case LineType.Back:
                    dir = GetRotateVector(dir, 180);
                    collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    break;
                case LineType.Left:
                    dir = GetRotateVector(dir, 90);
                    collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case LineType.Right:
                    dir = GetRotateVector(dir, -90);
                    collsionRay.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
            }   
            Vector3 newLInePoint = pos + dir;

            if (collsionRay.DetectedObjects.Count == 0)
            {
                newLInePoint = pos + dir * -5;
            }
            else
            {
                newLInePoint = collsionRay.DetectedObjects[0].transform.position;
            }          

          
            spline.SetPosition(0, newLInePoint);                       
        
            spline.SetPosition(spline.GetPointCount() - 1, GameManager.Instance.playerController.shootController.transform.position);

            raySensor2D.transform.position = spline.GetPosition(spline.GetPointCount() - 1);
            raySensor2D.Length = Vector2.Distance(spline.GetPosition(0), spline.GetPosition(spline.GetPointCount() - 1));
            raySensor2D.Direction = (spline.GetPosition(0) - spline.GetPosition(spline.GetPointCount() - 1)).normalized;



            pos = GameManager.Instance.playerController.shootController.transform.position;
            dir = (GameManager.Instance.playerController.shootController.transform.position - m_target.transform.position).normalized;
            newLInePoint = pos + dir * -5;

            collsionRay.transform.position = pos;
            collsionRay.Direction = (newLInePoint - pos).normalized;
            collsionRay.Length = Vector2.Distance(newLInePoint, pos);    
        }
        bInit = true;
    }
    private void OnDisable()
    {
        raySensor2D.Length = 0.1f;
        collsionRay.Length = 0.1f;
        removeLine();
        spline.SetPosition(0, Init_0);
        spline.SetPosition(1, Init_1);
        spriteShapeController.RefreshSpriteShape();
        spriteShapeController.autoUpdateCollider = false;

        raySensor2D.transform.position = spline.GetPosition(spline.GetPointCount() - 1);        
        raySensor2D.Direction = (spline.GetPosition(0) - spline.GetPosition(spline.GetPointCount() - 1)).normalized;       
        Vector2[] newPoints;
        newPoints = edgeCollider.points;
        for (int i =0; i< newPoints.Length; i++)
        {
            if (i == 0)
                newPoints[i] = Init_0;
            else
            {
                newPoints[i] = Init_1;
            }
        }
        edgeCollider.points = newPoints;
        interval = defalutInterval;
        for (int i = 0; i < EffectList.Count; i++)
        {
            EZ_Pooling.EZ_PoolManager.Despawn(EffectList[i]);
        }
    }
    private void OnEnable()
    {
        switch (lineType)
        {
            case LineType.None:
                if (isRotateAngle)
                {
                    collsionRay.transform.localRotation = Quaternion.Euler(0, 0, angle_rotate);
                }
                else
                {
                    collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }                
                break;
            case LineType.Back:                
                collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case LineType.Left:                
                collsionRay.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case LineType.Right:                
                collsionRay.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
        }
        
        collsionRay.OnSensorUpdate += CollsionRay_OnSensorUpdate;

        m_attackTypes.Clear();
        m_attackMethods.Clear();
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackMethod)).Length; i++)
        {
            m_attackMethods.Add(false);
        }
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackType)).Length; i++)
        {
            m_attackTypes.Add(false);
        }
        spriteShapeController.autoUpdateCollider = true;
        EnmeyVecList.Clear();
    }
    bool bInit = false;
    bool isRotateAngle = false;
    float angle_rotate;
    public void Shoot(float _damage, GameObject _Target, List<GameManager.AttackType> attackTypes, List<GameManager.AttackMethod> attackMethods, LineType line, bool _rotate, float _rotateAngle)
    {
        removeLine();
        BulletCount = GameManager.Instance.playerController.m_shootCount;
        isRotateAngle = _rotate;
        angle_rotate = _rotateAngle;
        RightRandomAngle.Clear();
        LeftRandomAngle.Clear();
        for (int i = 0; i < MaxHomingTarget; i++)
        {
            RightRandomAngle.Add(new Vector3(Random.Range(0f, 0.2f), Random.Range(-0.15f, 0.3f)));
            LeftRandomAngle.Add(new Vector3(Random.Range(-0.25f, 0f), Random.Range(-0.25f, 0.25f)));

        }
        lineType = line;
        for (int i = 0; i < attackMethods.Count; i++)
        {
            m_attackMethods[(int)attackMethods[i]] = true;
        }
        for (int i = 0; i < attackTypes.Count; i++)
        {
            m_attackTypes[(int)attackTypes[i]] = true;
        }
        m_target = _Target;
        Power = _damage;
        TargetPosition = m_target.transform.position;
        if (m_attackMethods[(int)GameManager.AttackMethod.homing])
        {
            CheckHoming();
        }
        InitShoot();
        

    }
    public Vector2 GetRotateVector(Vector2 v, float degrees)
    {
        float _x = v.x * Mathf.Cos(Mathf.Deg2Rad * degrees) - v.y * Mathf.Sin(Mathf.Deg2Rad * degrees);
        float _y = v.x * Mathf.Sin(Mathf.Deg2Rad * degrees) + v.y * Mathf.Cos(Mathf.Deg2Rad * degrees);
        return new Vector2(_x, _y);
    }
    void removeLine()
    {
        if (spline.GetPointCount() > 2)
        {
            while (true)
            {
                spline.RemovePointAt(1);
                if (spline.GetPointCount() == 2)
                {
                    break;
                }
            }
        }
    }
    void removeAll()
    {
                
    }
    public List<Vector2> EnmeyVecList = new List<Vector2>();

    void TargetCheck()
    {

        if (raySensor2D.DetectedObjects.Count <= 0)
        {

        }
        else
        {
            float shortDis = Vector3.Distance(transform.position, raySensor2D.DetectedObjects[0].transform.position);


            //trees = new Transform[raySensor2D.DetectedObjects.Count-1];
            trees = new Transform[raySensor2D.DetectedObjects.Count];
            int index = 0;
            foreach (GameObject found in raySensor2D.DetectedObjects)
            {

                float Distance = Vector3.Distance(GameManager.Instance.Player.transform.position, found.transform.position);

                if (found.tag != "Obstacles")
                {
                    if (Distance < shortDis)
                    {
                        shortDis = Distance;
                    }
                    trees[index] = found.transform;
                    index++;
                }
            }
            if (index > 0)
            {
                trees = trees.OrderBy((d) => (d.position - GameManager.Instance.Player.transform.position).sqrMagnitude).ToArray();


                EnmeyVecList.Clear();
                int treeCount = trees.Length;
                if (treeCount > MaxHomingTarget)
                {
                    treeCount = MaxHomingTarget;
                }
                for (int i = 0; i < treeCount; i++)
                {
                    EnmeyVecList.Add(trees[i].gameObject.transform.position);
                }
            }
        }
    }
    public Transform[] trees;


    void CheckHoming()
    {
        //TargetList.Clear();

        if (m_attackMethods[(int)GameManager.AttackMethod.homing])
        {

            TargetCheck();

            removeLine();

            for (int i = EnmeyVecList.Count() - 1; i >= 0; i--)
            {
                AddLIne(i);
            }

        }
    }
    private void Update()
    {
        collsionRay.Pulse();
        raySensor2D.Pulse();
        spriteShapeController.RefreshSpriteShape();
        spriteShapeController.BakeCollider();
        if (timer >= interval)
        {
            spriteShapeController.spriteShape = spriteShapes[Random.Range(0, spriteShapes.Count)];

            raySensor2D.transform.position = spline.GetPosition(0);
            raySensor2D.Length = Vector2.Distance(spline.GetPosition(0), spline.GetPosition(spline.GetPointCount() - 1));
            raySensor2D.Direction = (spline.GetPosition(spline.GetPointCount() - 1) - spline.GetPosition(0)).normalized;

            timer = 0.0f;
        }
    
        if (bInit)
        {
            CheckHoming();
            InitShoot();            
        }
        timer += Time.deltaTime;
    }
}
