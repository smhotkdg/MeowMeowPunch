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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Player")
        {
            if(GameManager.Instance.Key >=1)
            {
                GameManager.Instance.Key -= 1;
                UIManager.Instance.SetKeyText();
                animator.SetTrigger("Open");
                Orther.SetTrigger("Open");
            }
        }
    }
}
