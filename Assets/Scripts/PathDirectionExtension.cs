using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathDirection
{
    North,
    East,
    South,
    West
}

public enum DirectionChange
{
    None,
    TurnRight,
    Turneft,
    TurnAround
}

public static class PathDirectionExtension
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f,90f,0f),
        Quaternion.Euler(0f,180f,0f),
        Quaternion.Euler(0f,270f,0f)
    };

    static Vector3[] halfVectors =
    {
        Vector3.forward * .5f,
        Vector3.right * .5f,
        Vector3.back * .5f,
        Vector3.left * .5f
    };

    public static Quaternion GetRotation(this PathDirection a_direction)
    {
        return rotations[(int)a_direction];
    }

    public static DirectionChange GetDirectionChange(this PathDirection a_current, PathDirection a_next)
    {
        if (a_current == a_next)
        {
            return DirectionChange.None;
        }
        else if (a_current + 1 == a_next || a_current - 3 == a_next)
        {
            return DirectionChange.TurnRight;
        }
        else if (a_current - 1 == a_next || a_current + 3 == a_next)
        {
            return DirectionChange.Turneft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this PathDirection a_direction)
    {
        return (float)a_direction * 90f;
    }

    public static Vector3 GetHalfVector(this PathDirection a_direction)
    {
        return halfVectors[(int)a_direction];
    }
}
