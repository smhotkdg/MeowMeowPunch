using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DynamicYSort : MonoBehaviour
{
    private int _baseSoringOrder;
    private float _ySoringOffset;
    [SerializeField]
    private SortableSprite[] _sortableSprites;
    [SerializeField]
    private Transform _sortOffsetMarker;
    private void Start()
    {
        _ySoringOffset = _sortOffsetMarker.position.y;
    }
    void Update()
    {
        _baseSoringOrder = transform.GetSortingOrder(_ySoringOffset);
        foreach(var sortableSprite in _sortableSprites)
        {
            sortableSprite.spriteRenderer.sortingOrder = _baseSoringOrder + sortableSprite.relativeOrder;
        }
    }
    [Serializable]
    public struct SortableSprite
    {
        public SpriteRenderer spriteRenderer;
        public int relativeOrder;
    }
    
}
