using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpawner : MonoBehaviour
{
    public List<BoxCollider2D> spawnArea;
    public int numberRandomPositions = 10;


    private void Awake()
    {
        //spawnArea = GetComponent<BoxCollider2D>();
    }


    private void Start()
    {
        //for (int i =0; i < numberRandomPositions;i++)
        //{
        //    float randomX = Random.Range(ranges[0], ranges[1]);
        //    float randomY = Random.Range(ranges[2], ranges[3]);

        //    Vector2 randomPos = new Vector2(randomX, randomY);
        //    GameObject go = Instantiate(Temp, randomPos, Quaternion.identity);
        //    Debug.Log(IsInside(spawnArea, go.transform.position));
        //}        
    }

    public Vector2 GetRandomPosition()
    {
        int rand = Random.Range(0, spawnArea.Count);
        Bounds colliderBounds = spawnArea[rand].bounds;
        Vector3 colliderCenter = colliderBounds.center;

        float spawnableItemSizeX = 1 / 2;
        float spawnableItemSizeY = 1 / 2;

        float[] ranges = {
            (colliderCenter.x - colliderBounds.extents.x) + spawnableItemSizeX,
            (colliderCenter.x + colliderBounds.extents.x) - spawnableItemSizeX,
            (colliderCenter.y - colliderBounds.extents.y) + spawnableItemSizeY,
            (colliderCenter.y + colliderBounds.extents.y) - spawnableItemSizeY,
        };

        float randomX = Random.Range(ranges[0], ranges[1]);
        float randomY = Random.Range(ranges[2], ranges[3]);

        Vector2 randomPos = new Vector2(randomX, randomY);
        return randomPos;
    }

    public bool IsInside(Vector3 point)
    {
        Vector3 closest = new Vector3();
        for (int i =0; i< spawnArea.Count; i++)
        {
            closest = spawnArea[0].ClosestPoint(point);
            if(closest == point)
            {
                return true;
            }
        }         
        // Because closest=point if point is inside - not clear from docs I feel
        return false;
    }
}
