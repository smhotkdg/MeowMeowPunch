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
	private List<Vector2> Targets{get; set;}

    float disableTime_Delta;
	void Awake(){
		LightningBolts=new List<LightningBolt>();
		Targets=new List<Vector2>();
        playerController = GetComponent<PlayerController>();

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
            for (int i = 0; i < Targets.Count; i++)
            {
                if (i == 0)
                {
                    LightningBolts[i].DrawLightning(playerController.shootController.transform.position, Targets[i], i);
                }
                else
                {
                    LightningBolts[i].DrawLightning(Targets[i - 1], Targets[i], i);
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
