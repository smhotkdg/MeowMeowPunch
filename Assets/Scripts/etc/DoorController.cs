using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum Pos
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }
    public Pos pos = Pos.BOTTOM;
    public Animator Orther;
    Animator animator;
    private Vector2 CenterPosition;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
    }
    public bool isItem = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isItem == false)
            return;
        if(collision.gameObject.tag =="Player")
        {

            CenterPosition.x = (transform.position.x + Orther.transform.position.x) / 2f;
            CenterPosition.y = (transform.position.y + Orther.transform.position.y) / 2f;
            if (GameManager.Instance.Key >=1)
            {
                GameManager.Instance.KeyEventHandler += Instance_KeyEventHandler;
                GameManager.Instance.Key -= 1;
                UIManager.Instance.SetKeyText();               
                switch(pos)
                {
                    case Pos.TOP:
                        Vector2 keTopPos = CenterPosition;
                        keTopPos.y = keTopPos.y - 0.1f;
                        GameManager.Instance.Spawn(GameManager.SpawnType.Key_Top, keTopPos, 1);
                        break;
                    case Pos.BOTTOM:
                        Vector2 keyBottomPos = CenterPosition;
                        keyBottomPos.y = keyBottomPos.y - 0.1f;
                        GameManager.Instance.Spawn(GameManager.SpawnType.Key_Bottom, keyBottomPos, 1);
                        break;
                    case Pos.LEFT:
                        Vector2 keyLeftPos = CenterPosition;
                        keyLeftPos.x = keyLeftPos.x + 0.26f;
                        GameManager.Instance.Spawn(GameManager.SpawnType.Key_Left, keyLeftPos, 1);
                        break;
                    case Pos.RIGHT:
                        Vector2 keyRightPos = CenterPosition;
                        keyRightPos.x = keyRightPos.x - 0.26f;
                        GameManager.Instance.Spawn(GameManager.SpawnType.Key_Right, keyRightPos, 1);
                        break;
                }
            }
        }
    }

    private void Instance_KeyEventHandler()
    {
        GameManager.Instance.KeyEventHandler -= Instance_KeyEventHandler;
        animator.Play("Treasure_Open");
        Orther.Play("Treasure_Open");
    }
}
