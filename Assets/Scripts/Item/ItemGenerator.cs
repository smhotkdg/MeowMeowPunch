using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Vector3 GeneratorPosition = new Vector3(0, 0, 0);
    private void OnEnable()
    {
        ItemController.Instance.MakeItem(Random.Range(0, ItemController.Instance.GetMaxItemCount()), GeneratorPosition, transform);
    }
}
