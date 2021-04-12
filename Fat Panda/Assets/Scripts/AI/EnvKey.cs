using System.Collections;
using System.Collections.Generic;
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
}
