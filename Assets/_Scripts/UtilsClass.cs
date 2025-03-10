using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
    public static class UtilsClass
    {

        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);

        public const int sortingOrderDefault = 5000;

        // Get Sorting order to set SpriteRenderer sortingOrder, higher position = lower sortingOrder
        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
        {
            return (int)(baseSortingOrder - position.y) + offset;
        }
public static float GetAngleFromVectorFloat(Vector3 dir)
{
    dir = dir.normalized;
    float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    if (n < 0) n += 360;

    return n;
}

}
