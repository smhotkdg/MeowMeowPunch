using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetController : MonoBehaviour
{
    public RectTransform CanvasRect;
    public RectTransform UI_Element;
    public float y_Margin = 0;
    public float x_Margin = 0;    

    private void OnEnable()
    {
        UI_Element.gameObject.SetActive(true);
        //FIndWorldPos();
    }

    private void Update()
    {       
    }
    private void OnDisable()
    {
        UI_Element.gameObject.SetActive(false);
    }
  
    void FIndWorldPos()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + x_Margin,
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + y_Margin);
        //now you can set the position of the ui element
        UI_Element.anchoredPosition = WorldObject_ScreenPosition;
    }
    private void LateUpdate()
    {
        FIndWorldPos();
    }
}
