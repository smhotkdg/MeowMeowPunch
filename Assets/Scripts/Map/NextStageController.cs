using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageController : MonoBehaviour
{
    public enum NextStageType
    {
        Normal
    }
    public NextStageType stageType = NextStageType.Normal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            switch(stageType)
            {
                case NextStageType.Normal:
                    GameManager.Instance.Stage++;
                    MapMaker.Instance.MakeMap();
                    break;
            }
        }
    }
}
