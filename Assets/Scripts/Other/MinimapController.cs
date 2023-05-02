using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Camera minimapCamera;

    private void OnEnable()
    {
        minimapCamera.orthographicSize = 15;
    }
    private void OnDisable()
    {
        minimapCamera.orthographicSize = 5;
    }

}
