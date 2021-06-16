using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding 
{
    public static float distance = 2;
    public static Vector2 nodeHead;

    public static void drawPotentialPaths(Vector2 rootPosition, int numberOfRays, Vector2 endPosition)
    {
        while (numberOfRays > 0)
        {
            numberOfRays--;

            if (endPosition.x > -4 + rootPosition.x)
            {
                Debug.DrawRay(rootPosition, Vector2.right * distance, Color.red, 3f, true);
                drawPotentialPaths(rootPosition + Vector2.right * distance, numberOfRays, endPosition);
            }

            if (endPosition.x < rootPosition.x + 4)
            {
                Debug.DrawRay(rootPosition, Vector2.left * distance, Color.red, 3f, true);
                drawPotentialPaths(rootPosition + Vector2.left * distance, numberOfRays, endPosition);
            }

            if (endPosition.y > -4 + rootPosition.y)
            {
                Debug.DrawRay(rootPosition, Vector2.up * distance, Color.red, 3f, true);
                drawPotentialPaths(rootPosition + Vector2.up * distance, numberOfRays, endPosition);
            }

            if (endPosition.y < rootPosition.y + 4)
            {
                Debug.DrawRay(rootPosition, Vector2.down * distance, Color.red, 3f, true);
                drawPotentialPaths(rootPosition + Vector2.down * distance, numberOfRays, endPosition);
            }
        }
    }
}
