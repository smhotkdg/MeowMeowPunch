using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LineAttackController : MonoBehaviour
{
    public float interval = 1.0f; // 호출 간격
    private float timer = 0.0f;

    public List<SpriteShape> spriteShapes;
    int boundsHash = 0;
    public float tangentLength = 1.0f;
    
    public List<GameObject> TestObjectList;

    public SpriteShapeController spriteShapeController;
    public SpriteShapeRenderer spriteShapeRenderer;
        
    [Button]
    public void AddLIne()
    {        
        Spline spline = spriteShapeController.spline;
        spline.Clear();
        Vector3 pos = GameManager.Instance.playerController.shootController.transform.position;
        Vector3 dir = (GameManager.Instance.playerController.shootController.transform.position - TestObjectList[0].transform.position).normalized;
        Vector3 newLInePoint = pos + dir * -5;        

        spline.InsertPointAt(0, GameManager.Instance.playerController.shootController.transform.position);
        spline.InsertPointAt(1, newLInePoint);
      
        //spline.InsertPointAt(1, new Vector3(0, 0));
        //spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        //spline.SetRightTangent(1,new Vector3(Random.Range(0f,0.2f), Random.Range(-0.15f, 0.3f) )* tangentLength);
        //spline.SetLeftTangent(1, new Vector3(Random.Range(-0.25f, 0f), Random.Range(-0.25f, 0.25f)) * tangentLength);
        
    }
    private void Update()
    {
        if (timer >= interval)
        {
            spriteShapeController.spriteShape = spriteShapes[Random.Range(0, spriteShapes.Count)];
            timer = 0.0f; 
        }

        timer += Time.deltaTime; 
    }
}

