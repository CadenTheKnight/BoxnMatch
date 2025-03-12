using UnityEngine;

public enum AbilityDirection 
{ 
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
}

//Extension method for defining a function for an enum, so rotation is the same everywhere
public static class DirectionExtensions
{
    private static readonly int directionCount = 4;
    public static void Rotate(this ref AbilityDirection dir, int quartersClockwise)
    {
        int tmpDir = (int)dir + quartersClockwise;

        //handle for negative special cases cause c# modulus does it weird
        if (tmpDir < 0) tmpDir = directionCount + tmpDir;
        tmpDir %= directionCount;

        dir = (AbilityDirection)tmpDir;
    }

    public static Vector3 GetUnitDirection(this AbilityDirection dir)
    {
        switch (dir)
        {
            case AbilityDirection.NORTH:
                return Vector3.up;
            case AbilityDirection.EAST:
                return Vector3.right;
            case AbilityDirection.SOUTH:
                return Vector3.down;
            case AbilityDirection.WEST:
                return Vector3.left;
        }
        return Vector3.zero;
    }

    public static float GetRotationZ(this AbilityDirection dir)
    {
        switch (dir)
        {
            case AbilityDirection.NORTH:
                return 0;
            case AbilityDirection.EAST:
                return 270;
            case AbilityDirection.SOUTH:
                return 180;
            case AbilityDirection.WEST:
                return 90;
        }
        return 0;
    }
}
