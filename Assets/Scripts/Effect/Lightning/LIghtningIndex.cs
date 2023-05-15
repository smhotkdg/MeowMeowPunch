using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LIghtningIndex : MonoBehaviour
{
    public float LineLenght;
    LineRenderer lineRenderer;    
    public Vector3 startline;
    public Vector3 direction;
    public float SegmentLength = 0.2f;
    public int MaxIndex =1000;
    public delegate void isOk(bool flag,bool isDisable, int index);
    public event isOk isOkEventHandler;
    int m_index = 0;    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        MaxIndex = 1000;
        
    }
    Vector3 dir;
    bool isDrawEnd = false;
    
    public void SetLine(Vector3 sPos,Vector3 dPos,int index)
    {
        startline = sPos;
        direction = dPos;
        m_index = index;
        Raycasting();
      
        DrawLightning(startline, direction);
    }
 
 
    public void DrawLightning(Vector2 source, Vector2 target)
    {
        //Calculated amount of Segments      
         if(MaxIndex >= m_index)
        {
            float distance = Vector2.Distance(source, target);
            int segments = 5;
            if (distance > SegmentLength)
            {
                segments = Mathf.FloorToInt(distance / SegmentLength) + 2;
            }
            else
            {
                segments = 4;
            }            
            lineRenderer.SetVertexCount(segments);
            lineRenderer.SetPosition(0, source);
            Vector2 lastPosition = source;
            for (int j = 1; j < segments - 1; j++)
            {
                //Go linear from source to target
                Vector2 tmp;

                tmp = Vector2.Lerp(source, target, (float)j / (float)segments);
                //Add randomness
                lastPosition = new Vector2(tmp.x + Random.Range(-0.1f, 0.1f), tmp.y + Random.Range(-0.1f, 0.1f));
                //Set the calculated position
                lineRenderer.SetPosition(j, lastPosition);
            }
            lineRenderer.SetPosition(segments - 1, target);       

            Color lightColor = new Color(0.5647f, 0.58823f, 1f, Random.Range(0.2f, 1f));
            lineRenderer.SetColors(lightColor, lightColor);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }        
        isDrawEnd = false;
    }    
    void Raycasting()
    {
        float distance = Vector2.Distance(startline, direction);
        dir = (direction - startline).normalized;


        bool isObstacle = false;
        RaycastHit2D[] Hits = Physics2D.RaycastAll(startline, dir,distance);
        List<Transform> PointList = new List<Transform>();
        for (int i =0; i< Hits.Length; i++)
        {
            if(Hits[i].collider.tag == "Obstacle")
            {
                //lineRenderer.SetPosition(lineRenderer.positionCount - 1, Hits[i].point);
                //startline = Hits[i].point;
                PointList.Add(Hits[i].transform);
                
                isObstacle = true;
            }
        }
        Vector2 NowPoint = new Vector2(0,0);
        if (PointList.Count >1)
        {
            Transform[] trees = new Transform[PointList.Count];
            float shortDis = Vector3.Distance(startline,PointList[0].position);            

            for (int i = 0; i < PointList.Count; i++) 
            {
                trees[i] = PointList[i];                
            }
            trees = trees.OrderBy((d) => (d.position - startline).sqrMagnitude).ToArray();
            direction = trees[0].position;
        }
        else
        {
            if(PointList.Count >0)
            {
                direction = PointList[0].position;
            }            
        }



        //direction = Hits[i].point;
        
        isOkEventHandler?.Invoke(isObstacle,isObstacle, m_index);
        Debug.DrawRay(startline, dir*distance, Color.red);    
    }
    private void OnDisable()
    {
        lineRenderer.positionCount = 0;
        isDrawEnd = false;
        isOkEventHandler?.Invoke(false,false, m_index);
    }
}
