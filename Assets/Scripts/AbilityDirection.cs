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
}
