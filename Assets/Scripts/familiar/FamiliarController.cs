using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamiliarController : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float minDistance = 1f;

    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetTarget(GameObject _target)
    {
        target = _target;
    }
    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    void Update()
    {
        if(target !=null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > minDistance)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);
                transform.position = newPosition;
            }
            CheckFlip();
        }      
    }
    public void CheckFlip()
    {
        if(transform.position.x > target.transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}
