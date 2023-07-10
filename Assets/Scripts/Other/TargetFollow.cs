using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    GameObject target;
 
    public GameObject target_effect;

    Vector2 pos;
    public void SetTarget(GameObject _target)
    {
        target = _target;
        pos = _target.GetComponent<Monster>().effect_target.transform.position;
        target_effect.transform.localScale = _target.GetComponent<Monster>().effectSacle;
        //target = _target;  
    }
    void Update()
    {
        if(target==null)
        {
            return;
        }
        transform.position = pos;
    }
}
