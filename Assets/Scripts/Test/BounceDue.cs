using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BounceDue : MonoBehaviour
{
        
    public UnityEvent onGroundHitEvent;
    private Transform trnsObject;
    
    public float gravity = -10;
    public Vector2 groundVelocity;
    public float verticalVelocity;
    private float lastVerticalVelocity;
    public bool isGrounded;
    private float randomYDrop;
    float firstYPos;


    public float yDropMin;
    public float yDropMax;
    public float randX_Min;
    public float randX_Max;
    public float randY_Min;
    public float randY_Max;
    public float PowerX;
    public float PowerY;

    //float groundRand;
    bool moveTop = false;
    Vector3 targetPos;
    float distance;
    CircleCollider2D collider2D;
    private void Awake()
    {
        //trnsBody = transform;
        trnsObject = transform;
        collider2D = GetComponent<CircleCollider2D>();
    }
    private void OnEnable()
    {
        collider2D.enabled = false;
        moveTop = false;
        transform.localPosition = new Vector3(0, 0, 0);
        randomYDrop = Random.Range(yDropMin, yDropMax);
        firstYPos = transform.position.y;
        Set(Vector3.right * Random.Range(randX_Min, randX_Max) * Random.Range(randY_Min, randY_Max), Random.Range(PowerX, PowerY));
        if(randomYDrop <0)
        {
            targetPos = trnsObject.position;
            targetPos.y = firstYPos - randomYDrop;
            distance = Vector3.Distance(trnsObject.position, targetPos);
            moveTop = true;            
        }
        //groundRand = Random.Range(0.2f, .7f);
    }    

    void Update()
    {
        if (moveTop==false)
        {
            UPosition();
            CheckGroundHit();
        }
        else
        {
            moveTopPos();
        }        
    }
    void moveTopPos()
    {        
        trnsObject.position = Vector3.MoveTowards(trnsObject.position, targetPos, distance*0.2f);
        if(targetPos.y <= trnsObject.position.y)
        {
            moveTop = false;
            collider2D.enabled = true;
        }
    }
    public void Set(Vector2 groundVelocity, float verticalVelocity)
    {
        isGrounded = false;
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
        lastVerticalVelocity = verticalVelocity;

    }
    public void UPosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
            trnsObject.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;            
        }
        trnsObject.position += (Vector3)groundVelocity * Time.deltaTime;
    }
    
    void CheckGroundHit()
    {
        if (trnsObject.position.y < firstYPos - randomYDrop && !isGrounded)
        {
            trnsObject.position = new Vector2(trnsObject.position.x, firstYPos - randomYDrop);
            //trnsObject.position = new Vector2(trnsObject.position.x, firstYPos);            
            isGrounded = true;
            GroundHit();
        }
    }
    void GroundHit()
    {
        onGroundHitEvent.Invoke();
    }
    public void Bounce(float division)
    {            
        Set(groundVelocity, lastVerticalVelocity / division);
        collider2D.enabled = true;
    }
    public void SlowDownVelocity(float division)
    {
        groundVelocity = groundVelocity / division;
    }
}