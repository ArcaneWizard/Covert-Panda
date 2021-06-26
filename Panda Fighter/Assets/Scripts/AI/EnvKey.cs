
using UnityEngine;

public struct EnvKey
{
    public int x;
    public int y;
    public char direction;

    public EnvKey(char direction, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
    }

    public void print()
    {
        Debug.LogFormat("{0}: {1}, {2}", this.direction, this.x, this.y);
    }

    public void printColored()
    {
        Debug.LogFormat("<color=cyan>{0}: {1}, {2}</color>", this.direction, this.x, this.y);
    }

}

public struct Jump
{
    private string type;
    private float jumpSpeed;
    private float delay;
    private float midAirSpeed;
    private Vector3 landing;

    public Jump(string type, float jumpSpeed, float delay, float midAirSpeed, Vector3 landing)
    {
        this.type = type;
        this.jumpSpeed = jumpSpeed;
        this.delay = delay;
        this.midAirSpeed = midAirSpeed;
        this.landing = landing;
    }

    public string getType()
    {
        return type;
    }

    public float getJumpSpeed()
    {
        return jumpSpeed;
    }

    public float getDelay()
    {
        return delay;
    }

    public float getMidAirSpeed()
    {
        return midAirSpeed;
    }

    public Vector3 getLandingPosition()
    {
        return landing;
    }
}

public struct WallChecker
{
    public bool wallNearby;
    public string floorOpening;
    public string ceilingOpening;

    public WallChecker(bool wallNearby, string floorOpening, string ceilingOpening)
    {
        this.wallNearby = wallNearby;
        this.floorOpening = floorOpening;
        this.ceilingOpening = ceilingOpening;
    }
}

public struct locationExists
{
    private bool exists;
    private Vector3 location;

    public locationExists(bool exists, Vector3 location)
    {
        this.exists = exists;
        this.location = location;
    }

    public bool doesExist()
    {
        return exists;
    }

    public Vector3 endLocation()
    {
        return location;
    }
}