using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Vector3 GeneratorPosition = new Vector3(0, 0, 0);
    private void OnEnable()
    {
        int itemIndex = 0;
        while (true) 
        {
            itemIndex = Random.Range(0, ItemController.Instance.GetMaxItemCount());
            if (ItemController.Instance.CanMakeItem(itemIndex))
            {
                break;
            }
        }
        ItemController.Instance.MakeItem(itemIndex, GeneratorPosition, transform);
    }
}
