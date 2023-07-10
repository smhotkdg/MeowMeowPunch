using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagentController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Product")
        {
            //if(GameManager.Instance.room)
            collision.GetComponent<BounceDue>().StartMovePlayer();
        }
    }    
}
