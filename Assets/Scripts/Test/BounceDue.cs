using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BounceDue : MonoBehaviour
{
    public bool isBounce = true;
    public int maxBounce;
    public float xForce;
    public float yForce;
    public float gravity;

    private Vector2 direction;
    private int currentBounce = 0;
    public bool isGrounded = true;
    
    private float maxHeight;
    private float currentHeight;

    public Transform sprite;
    public Transform shadow;
    public bool isMovePlayer = false;
    GetItemController itemController;
    public void StartMovePlayer()
    {
        if(isMovePlayer ==false)
        {  
            switch (itemController.itemType)
            {
                case GetItemController.GetItemType.coin:
                    if (GameManager.Instance.CheckCoin(itemController.ItemCount))
                    {
                        isMovePlayer = true;
                    }                   
                    break;

                case GetItemController.GetItemType.key:
                    if (GameManager.Instance.CheckKey(itemController.ItemCount))
                    {
                        isMovePlayer = true;
                    }                    
                    break;
                case GetItemController.GetItemType.Hp:
                    if (GameManager.Instance.CheckHp(itemController.ItemCount))
                    {
                        isMovePlayer = true;
                    }
                    
                    break;
                case GetItemController.GetItemType.MaxHp:
                    if (GameManager.Instance.CheckMaxHp(itemController.ItemCount))
                    {
                        isMovePlayer = true;
                    }                   
                    break;
            }
            
        }
    }
    private void Awake()
    {
        itemController = GetComponent<GetItemController>();
    }
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        if(isBounce ==false)
        {
            return;
        }
        isMovePlayer = false;
        currentBounce = 0;
        currentHeight = Random.Range(yForce - 1, yForce);
        maxHeight = currentHeight;
        Initialize(new Vector2(Random.Range(-xForce, xForce), Random.Range(-xForce, xForce)));
        shadow.localScale = Vector2.one;
    }   
    private void FixedUpdate()
    {
        if(isBounce ==false)
        {
            return;
        }
        if(!isGrounded)
        {
            currentHeight += -gravity + Time.deltaTime;
            sprite.position += new Vector3(0, currentHeight, 0) * Time.deltaTime;
            transform.position += (Vector3)direction * Time.deltaTime;

            float totalVelocity = Mathf.Abs(currentHeight) + Mathf.Abs(maxHeight);
            float scaleXY = Mathf.Abs(currentHeight) / totalVelocity;
            shadow.localScale = Vector2.one * Mathf.Clamp(scaleXY, 0.5f, 1.0f);

            CheckGroundHit();
        }
        if(isMovePlayer==true && isGrounded ==true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.Player.transform.position, Time.deltaTime*2f);
        }
    }
    private void Initialize(Vector2 _direction)
    {
        isGrounded = false;
        maxHeight /= 1.5f;
        direction = _direction;
        currentHeight = maxHeight;
        currentBounce++;
    }
    void CheckGroundHit()
    {
        if(sprite.position.y < shadow.position.y)
        {
            sprite.position = shadow.position;
            shadow.localScale = Vector2.zero;
            if(currentBounce < maxBounce)
            {
                Initialize(direction / 1.5f);
            }
            else
            {
                isGrounded = true;
                shadow.transform.localScale = Vector2.zero;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag =="wall" || collision.tag =="Room_wall")
        {
            isBounce = false;
            sprite.position = shadow.position;
            shadow.transform.localScale = Vector2.zero;
        }
    }
}