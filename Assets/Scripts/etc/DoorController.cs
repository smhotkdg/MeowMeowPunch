using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator Orther;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public bool isItem = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isItem == false)
            return;
        if(collision.gameObject.tag =="Player")
        {            
            if(GameManager.Instance.Key >=1)
            {                
                GameManager.Instance.Key -= 1;
                UIManager.Instance.SetKeyText();
                //animator.SetTrigger("Open");
                animator.Play("Treasure_Open");
                //Orther.SetTrigger("Open");
                Orther.Play("Treasure_Open");
                //여기 열쇠 몇번 가져가는지 확인                
            }
        }
    }
}
