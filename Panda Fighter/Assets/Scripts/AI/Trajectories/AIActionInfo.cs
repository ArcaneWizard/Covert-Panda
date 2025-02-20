using UnityEngine;

// Houses the information required to execute any type of complex AI action. Not all parameters need to be filled

[System.Serializable]
public class AIActionInfo : MonoBehaviour
{
    public int DirX { get; private set; }
    public Vector2 Speed { get; private set; }
    public Vector2 TimeB4Change { get; private set; }
    public Vector2 ChangedSpeed { get; private set; }
    public Vector2 TimeB4SecondChange { get; private set; }
    public Vector2 SecondChangedSpeed { get; private set; }
    public Vector2 Bounds { get; private set; }

    public AIActionInfo(int direction, Vector2 speed, Vector2 timeB4Change, Vector2 changedSpeed,
        Vector2 timeB4SecondChange, Vector2 secondChangedSpeed, Vector2 bounds, Vector3 trajectoryPos)
    {
        DirX = direction;
        Speed = speed;
        TimeB4Change = timeB4Change;
        ChangedSpeed = changedSpeed;
        Bounds = new Vector2(trajectoryPos.x + bounds.x, trajectoryPos.x + bounds.y);
        TimeB4SecondChange = timeB4SecondChange;
        SecondChangedSpeed = secondChangedSpeed;
    }

    public AIActionInfo(int dir) { DirX = dir; }

    public override string ToString()
    {
        return $"DirX: {DirX}, speed: {Speed}, timeB4CHange: {TimeB4Change} + ChangedSpeed: {ChangedSpeed}" +
            $"timeb4SecondChange: {TimeB4SecondChange} + SecondChangedSpeed: {SecondChangedSpeed} + Bounds: {Bounds}";
    }
}
