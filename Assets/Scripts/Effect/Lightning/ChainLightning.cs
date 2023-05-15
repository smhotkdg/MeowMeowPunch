using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;

public class ChainLightning : MonoBehaviour{

    PlayerController playerController;
    public LightningColider lightningColider;
	[Header("Prefabs")]
	public GameObject lineRendererPrefab;
	public GameObject lightRendererPrefab;

	[Header("Config")]
	public float chainLength;
	public int lightnings;

	private float nextRefresh;
    [SerializeField]
	private float segmentLength=0.1f;
    public float Power;
	private List<LightningBolt> LightningBolts{get; set;}
    public List<int> minCounts = new List<int>();
	private List<Vector3> Targets{get; set;}

    float disableTime_Delta;
	void Awake(){
		LightningBolts=new List<LightningBolt>();
		Targets=new List<Vector3>();
        playerController = GetComponent<PlayerController>();
        for (int i = 0; i < System.Enum.GetValues(typeof(GameManager.AttackMethod)).Length; i++)
        {
            m_attackMethods.Add(false);
        }
        //BuildChain();

    }
    private void Start()
    {
        
    }

    public void UpdateChain(List<Vector2> TargetList)
    {
        Targets.Clear();
        for(int i=0; i< TargetList.Count; i++)
        {
            Targets.Add(TargetList[i]);
        }
        
    }
    public List<bool> m_attackMethods = new List<bool>();
    public void SetMethodType(List<GameManager.AttackMethod> attackMethods)
    {
        for (int i = 0; i < attackMethods.Count; i++)
        {
            m_attackMethods[(int)attackMethods[i]] = true;
        }
    }
    Vector2 GetLinePoint(Vector2 p1,Vector2 p2,int _x)
    {
        Vector2 newPoint = new Vector2();
        newPoint.y = (p2.y - p1.y) / (p2.x - p1.x) * _x + (p2.x * p1.y - p1.x * p2.y) / (p2.x - p1.x);
        newPoint.x = _x;
        return newPoint;
    }
    public void BuildChain(List<Vector2> TargetList){
        //Build a chain, in a real project this might be enemies ;)
        bInit = false;
        //disableEvent(TargetList.Count);
        Targets.Clear();
        LightningBolt tmpLightningBolt;
        LightningBolts.Clear();
        minCounts.Clear();
        lightningColider.gameObject.SetActive(true);
        lightningColider.Power = Power;
        for (int i=0;i< TargetList.Count; i++)
        {
            minCounts.Add(1000);
            tmpLightningBolt = new LightningBolt(segmentLength, i);
            tmpLightningBolt.isOkEventHandler += TmpLightningBolt_isOkEventHandler;
            
            tmpLightningBolt.Init(lightnings, lineRendererPrefab, lightRendererPrefab, TargetList[i], lightningColider,i,GetComponent<ChainLightning>());
            //tmpLightningBolt.Init(lightnings, lineRendererPrefab, lightRendererPrefab, NewLinePoint, lightningColider, i, GetComponent<ChainLightning>());

            LightningBolts.Add(tmpLightningBolt);           
            Targets.Add(TargetList[i]);            
            //LightningBolts[i].Activate();
        }   
    }
    int minIndex = 1000;    
    private void TmpLightningBolt_isOkEventHandler(bool flag,bool isDisable, int index)
    {      
        if (isDisable)
        {
            minCounts[index] = index;        
            minCounts.Sort();
            if (minIndex > index)
            {
                minIndex = index;
            }
            if(minCounts.Count == LightningBolts.Count)
            {
                
                for (int i = 0; i < LightningBolts.Count; i++)
                {
                    if(minCounts[0] >0)
                        LightningBolts[i].SetMinIndex(minCounts[0]);
                }
            }            
        }     
            
    }

    bool bInit = false;
    List<LineRenderer> lineRenderers;
    Transform transform_lightningColider;
    public void InitDraw()
    {
        if (bInit)
            return;        
        lineRenderers = new List<LineRenderer>();
        for(int i =0; i < LightningBolts.Count; i++)
        {   
            for(int k =0; k < LightningBolts[i].lineRenderer.Length;k++ )
            {
                lineRenderers.Add(LightningBolts[i].lineRenderer[k]);
            }
            
        }

        transform_lightningColider = EZ_PoolManager.Spawn(lightningColider.transform, new Vector3(0, 0, 0), new Quaternion());
        transform_lightningColider.gameObject.GetComponent<LightningColider>().SetEdgeColider(lineRenderers, LightningBolts);
        bInit = true;
    }
 
    void ShowLightning()
    {
        if (LightningBolts.Count <= 0)
        {
            return;
        }
        if (Targets.Count != LightningBolts.Count)
        {
            return;
        }
        if (Time.time > nextRefresh)
        { 
            if(Targets.Count ==1)
            {
                Vector3 pos = playerController.shootController.transform.position;
                Vector3 dir = (playerController.shootController.transform.position - Targets[0]).normalized;
                Vector3 newLInePoint = pos + dir * -3;
                LightningBolts[0].DrawLightning(playerController.shootController.transform.position, newLInePoint, 0);
            }
            else
            {
                for (int i = 0; i < Targets.Count; i++)
                {
                    if (i == 0)
                    {
                        if (m_attackMethods[(int)GameManager.AttackMethod.homing])
                        {
                            //if(Targets.Count >1)
                            {
                                LightningBolts[i].DrawLightning(playerController.shootController.transform.position, Targets[i], i);
                            }
                            //else
                            //{
                            //    Vector3 pos = playerController.shootController.transform.position;
                            //    Vector3 dir = (playerController.shootController.transform.position - Targets[i]).normalized;
                            //    Vector3 newLInePoint = pos + dir * -3;
                            //    LightningBolts[i].DrawLightning(playerController.shootController.transform.position, newLInePoint, i);
                            //}

                        }
                        else
                        {
                            Vector3 pos = playerController.shootController.transform.position;
                            Vector3 dir = (playerController.shootController.transform.position - Targets[i]).normalized;
                            Vector3 newLInePoint = pos + dir * -3;

                            LightningBolts[i].DrawLightning(playerController.shootController.transform.position, newLInePoint, i);
                        }

                    }
                    else
                    {
                        if (m_attackMethods[(int)GameManager.AttackMethod.homing])
                        {
                            LightningBolts[i].DrawLightning(Targets[i - 1], Targets[i], i);
                        }
                    }
                }
            }
           
            nextRefresh = Time.time + 0.01f;
            InitDraw();
        }
    }
   
    void Update()
    {
        ShowLightning();
    }
}
