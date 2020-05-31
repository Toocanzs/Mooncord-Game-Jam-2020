using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Vector2Extentions
{
    public static float GetAngle(Vector2 dir)
    {
        return math.atan2(dir.y, dir.x);
    }
}
