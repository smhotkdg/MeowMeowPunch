using UnityEngine;
using System.Collections;
using EZ_Pooling;
using System.Collections.Generic;

public class LightningBolt : MonoBehaviour {

    
	public LineRenderer[] lineRenderer{get; set;}
	public LineRenderer lightRenderer{get; set;}
	
	public float SegmentLength{get; set;}
	public int Index{get; private set;}
	public bool IsActive{get; private set;}
    LightningColider lightningColider;

    public LightningBolt(float segmentLength, int index){
		SegmentLength=segmentLength;
		Index=index;
	}

    private void OnEnable()
    {
        //isStop = false;
    }
    ChainLightning m_chainLightning;
    public void Init(int lineRendererCount, GameObject lineRendererPrefab, GameObject lightRendererPrefab,Vector2 Target,LightningColider _colider,int index,ChainLightning chainLightning)
    {
        m_chainLightning = chainLightning;
        isChangeTarget = false;
        setInit = true;        
        lightningColider = _colider;
        lineRenderer =new LineRenderer[lineRendererCount];
		for(int i=0;i<lineRendererCount;i++)
        {
            lineRenderer[i] = EZ_PoolManager.Spawn(lineRendererPrefab.transform,
                new Vector3(0, 0, 0), Quaternion.AngleAxis(0, new Vector3(0, 0, 0))).gameObject.GetComponent<LineRenderer>();            
            lineRenderer[i].enabled=false;
		}
        IsActive =false;
	}


    public void Activate(){
		//Active this LightningBolt with all of its LineRenderers
		for(int i=0;i<lineRenderer.Length;i++){
			lineRenderer[i].enabled=true;
        }
		//lightRenderer.enabled=true;
		IsActive=true;
	}
    public void DeActivate()
    {
        for (int i = 0; i < lineRenderer.Length; i++)
        {
            lineRenderer[i].GetComponent<AutoDestory>().Despawn();
        }
        //lightRenderer.GetComponent<AutoDestory>().Despawn();
    }

    bool setInit = true;
    public bool isChangeTarget =false;
    public Vector2 m_Target;
    public bool isStop = false;
    
	public void DrawLightning(Vector2 source, Vector2 target,int index)
    {
        Activate();
        for (int i=0;i<lineRenderer.Length;i++){
            

            lineRenderer[i].GetComponent<LIghtningIndex>().SetLine(source, target,index);
            lineRenderer[i].GetComponent<LIghtningIndex>().isOkEventHandler += LightningBolt_isOkEventHandler;
        }
    }
    public delegate void isOk(bool flag, bool isDisable,int index);
    public event isOk isOkEventHandler;
    
    private void LightningBolt_isOkEventHandler(bool flag,bool isDisable,int index)
    {        
        for (int i = 0; i < lineRenderer.Length; i++)
        {   
            lineRenderer[i].GetComponent<LIghtningIndex>().isOkEventHandler -= LightningBolt_isOkEventHandler;
        }
        isOkEventHandler.Invoke(flag, isDisable,index);
    }
    public void SetMinIndex(int index)
    {
        for (int i = 0; i < lineRenderer.Length; i++)
        {
            lineRenderer[i].GetComponent<LIghtningIndex>().MaxIndex = index;            
        }
    }
}
