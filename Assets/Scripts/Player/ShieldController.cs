using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public delegate void OnComplete();
    public event OnComplete OnCompleteEvnetHander;

    float defaultTime = 7;
    float deltaTime = 0;
    bool isEnd = false;
    private void OnEnable()
    {
        deltaTime = defaultTime;
        isEnd = false;
    }
    private void Update()
    {
        deltaTime -= Time.deltaTime;
        if(deltaTime <=0 && isEnd ==false)
        {
            isEnd = true;
            OnCompleteEvnetHander?.Invoke();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            float randPower = Random.Range(0, 0);
            Vector2 knocback = direction * randPower;            
            collision.gameObject.GetComponent<Monster>().SetDamage(0, new Vector2(0,0),Monster.Status.Stren,new Color(1,1,1));
        }
    }
}
