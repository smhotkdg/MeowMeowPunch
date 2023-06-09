using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsLastSibling : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
