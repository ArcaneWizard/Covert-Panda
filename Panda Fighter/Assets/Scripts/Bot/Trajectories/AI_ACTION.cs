using UnityEngine;

[System.Serializable]
public struct AI_ACTION
{
    public int dirX { get; private set; }
    public Vector2 speed { get; private set; }
    public Vector2 timeB4Change { get; private set; }
    public Vector2 changedSpeed { get; private set; }
    public Vector2 timeB4SecondChange { get; private set; }
    public Vector2 secondChangedSpeed { get; private set; }
    public string actionName { get; private set; }
    public Vector2 jumpBounds { get; private set; }

    public AI_ACTION(string action, int direction, Vector2 speed, Vector2 timeB4Change, Vector2 changedSpeed, 
        Vector2 timeB4SecondChange, Vector2 secondChangedSpeed, Vector2 jumpBounds, Vector3 trajectoryPos)
    {
        this.actionName = action;
        this.dirX = direction;
        this.speed = speed;
        this.timeB4Change = timeB4Change;
        this.changedSpeed = changedSpeed;
        this.jumpBounds = new Vector2(trajectoryPos.x + jumpBounds.x, trajectoryPos.x + jumpBounds.y);
        this.timeB4SecondChange = timeB4SecondChange;
        this.secondChangedSpeed = secondChangedSpeed;
    }

    public override string ToString()
    {
        return $"action: {actionName}, dirX: {dirX}, speed: {speed}, timeB4CHange: {timeB4Change} + changedSpeed: {changedSpeed}" + 
            $"timeb4SecondChange: {timeB4SecondChange} + secondChangedSpeed: {secondChangedSpeed} + jumpBounds: {jumpBounds}";
    }
}

