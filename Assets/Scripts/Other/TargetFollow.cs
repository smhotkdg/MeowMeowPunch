using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    GameObject target;
 

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }
    void Update()
    {
        if(target==null)
        {
            return;
        }
        transform.position = target.transform.position;
    }
}
