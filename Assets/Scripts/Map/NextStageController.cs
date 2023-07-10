using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class NextStageController : MonoBehaviour
{
    public CircleCollider2D initColider;
    public Transform playerPos;
    public GameObject EvObject;
    Animator animator;
    public enum NextStageType
    {
        Normal
    }
    public NextStageType stageType = NextStageType.Normal;
    float defaultNextTime;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        EvObject.SetActive(false);
        initColider.enabled = false;
        defaultNextTime = NextTime;
        spriteRenderer.sortingLayerName = "Foreground";
        isOn = false;
    }
    bool isOn = false;
    [Button]
    public void OpenNext()
    {
        if(isOn ==false)
        {
            isOn = true;
            spriteRenderer.sortingLayerName = "Characters";
            animator.Play("hole_open_animation");
            initColider.enabled = true;
        }
        

    }
    public float NextTime = 0.5f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            defaultNextTime -= Time.deltaTime;
            if(defaultNextTime<=0)
            {
                StartNext();
            }
        }
    }
    public void ChangeLayerUP()
    {
        GameManager.Instance.gameStatus = GameManager.GameStatus.DO_FORCE;
        spriteRenderer.sortingLayerName = "Above";        
    }
    public void StartNextMap()
    {
        GameManager.Instance.ChangePlayerSprite(false);
        StartCoroutine(MakeMapRoutine());
    }
    IEnumerator MakeMapRoutine()
    {
        yield return new WaitForSeconds(.5f);
        EvObject.SetActive(true);
        float y = transform.position.y;
        y = y + 10;
        EvObject.transform.DOMoveY(y, 5f);
        animator.Play("hole_open");
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.Stage++;
        MapMaker.Instance.MakeMap();
    }
    void StartNext()
    {
        GameManager.Instance.gameStatus = GameManager.GameStatus.DO_FORCE;
        animator.Play("elevator_close");

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.tag == "Player")
        //{
        //    switch(stageType)
        //    {
        //        case NextStageType.Normal:
        //            GameManager.Instance.Stage++;
        //            MapMaker.Instance.MakeMap();
        //            break;
        //    }
        //}
    }
}
