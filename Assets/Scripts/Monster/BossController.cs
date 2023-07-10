using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public delegate void onCompleteEnable();
    public event onCompleteEnable onCompleteEnableHandler;
    public List<GameObject> Boss;

    private void OnEnable()
    {
        GameObject BossObj = Instantiate(Boss[Random.Range(0, Boss.Count)]);
        //GameObject BossObj = Instantiate(Boss[0]);
        BossObj.transform.SetParent(transform.parent);
        BossObj.transform.position = transform.position;
        onCompleteEnableHandler?.Invoke();
    }
}
