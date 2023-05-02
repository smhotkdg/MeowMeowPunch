using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Canvas Maincanvas;
    protected override void Start()
    {
        base.Start();        
        
        //background.gameObject.SetActive(false);
    }
    private void Awake()
    {
        Maincanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }    

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //background.gameObject.SetActive(false);
        //Debug.Log(background.transform.localPosition);
        background.anchoredPosition = new Vector3(Maincanvas.GetComponent<RectTransform>().rect.width / 2,300,0);

        base.OnPointerUp(eventData);
    }
    public void ChangeCanvas()
    {
        if(Maincanvas ==null)
            Maincanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        StartCoroutine(ChangeCanvasRoutine());        
    }
    IEnumerator ChangeCanvasRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        background.anchoredPosition = new Vector3(Maincanvas.GetComponent<RectTransform>().rect.width / 2, 300, 0);
        
    }
}