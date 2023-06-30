using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Camera minimapCamera;
    [SerializeField]
    float maxMinimapSize = 20;
    [SerializeField]
    float minMinimapSize = 5;
    private void OnEnable()
    {
        minimapCamera.orthographicSize = maxMinimapSize;
    }
    private void OnDisable()
    {
        minimapCamera.orthographicSize = minMinimapSize;
    }

}
