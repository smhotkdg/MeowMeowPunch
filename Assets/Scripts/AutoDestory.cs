using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestory : MonoBehaviour
{
    public bool isDespawn = true;
  
    [SerializeField]
    float DestoryTime = 0.2f;

    float defaultTime;
    private void Awake()
    {
        defaultTime = DestoryTime;
    }
    private void OnEnable()
    {

        DestoryTime = defaultTime;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Despawn()
    {
        EZ_PoolManager.Despawn(transform);
    }
    // Update is called once per frame
    void Update()
    {
        DestoryTime -= Time.deltaTime;
        if(DestoryTime<=0)
        {
            if(isDespawn)
            {
                EZ_PoolManager.Despawn(transform);
            }
            else
            {
                gameObject.SetActive(false);
            }
            
        }
    }
}
