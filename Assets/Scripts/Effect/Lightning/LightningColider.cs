using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningColider : DamageColider
{
    public bool setInit = false;
    EdgeCollider2D edgeCollider;
    List<Vector2> edges = new List<Vector2>();


    
    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        OnObstacleEnterEventHandler += LightningColider_OnObstacleEnterEventHandler;
        edgeCollider.enabled = false;
    }

    private void LightningColider_OnObstacleEnterEventHandler(Vector3 collisionPos)
    {        
       
    }
  
    List<LineRenderer> mylineRenderers = new List<LineRenderer>();
    List<LightningBolt> mylightningBolts = new List<LightningBolt>();
    private void OnEnable()
    {   
        setInit = false;
        edges.Clear();
        edgeCollider.SetPoints(edges);
        mylineRenderers.Clear();
        edgeCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        CheckColider();
    }
    float enableTime = 0.1f;
    void CheckColider()
    {
        edges.Clear();
        for (int LineCount = 0; LineCount < mylineRenderers.Count; LineCount++)
        {
            LineRenderer lineRenderer = mylineRenderers[LineCount];
            if(lineRenderer.enabled)
            {
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    Vector3 lineRendererPoint = lineRenderer.GetPosition(i);
                    bool isSame = false;

                    for (int k = 0; k < edges.Count; k++)
                    {
                        if (edges[k].x == lineRendererPoint.x && edges[k].y == lineRendererPoint.y)
                        {
                            isSame = true;
                        }
                    }

                    if (isSame == false)
                    {
                        edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
                    }

                }
            }
           
        }

        edgeCollider.SetPoints(edges);
        enableTime -= Time.deltaTime;
        if(enableTime <=0)
        {
            edgeCollider.enabled = true;
            enableTime = 0.1f;
        }   
        
        setInit = true;
    }
    public void SetEdgeColider(List<LineRenderer> lineRenderers,List<LightningBolt> lightningBolts)
    {
        edgeCollider.enabled = false;
        if (setInit)
            return;
        mylineRenderers = lineRenderers;
        mylightningBolts = lightningBolts;
    }  
}
