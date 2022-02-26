using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utils
{
    public static bool IsDistanceGreater(Vector3 point1, Vector3 point2, float distance)
    {
        return (point2 - point1).sqrMagnitude > distance * distance;
    }
}
