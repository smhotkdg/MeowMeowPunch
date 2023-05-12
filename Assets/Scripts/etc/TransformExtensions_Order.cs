using UnityEngine;

public static class TransformExtensions_Order
{
    public static int GetSortingOrder(this Transform transfrom,float yOffset =0)
    {
        return -(int)((transfrom.position.y+yOffset) * 100);
    }
}
