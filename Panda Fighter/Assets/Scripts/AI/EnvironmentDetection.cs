using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDetection
{
    private LayerMask map = (1 << 11);

    private Dictionary<EnvKey, EnvInfo> info = new Dictionary<EnvKey, EnvInfo>();

    //cast multiple raycasts to gather info about the environment
    public void scanEnvironment(Transform entity)
    {
        info.Clear();

        float eX = entity.transform.position.x;
        float eY = entity.transform.position.y;

        //G for ground check, 
        for (int i = -7; i <= 7; i++)
            sendRaycast(Vector2.down, 6, new EnvKey('G', i, 0), Color.magenta, new Vector2(eX + 1.5f * i, eY));

        //C for ceiling check
        for (int j = -7; j <= 7; j++)
            sendRaycast(Vector2.up, 6, new EnvKey('C', j, 0), Color.cyan, new Vector2(eX + 1.5f * j, eY));

        //L for left wall check, R for right wall check
        sendRaycast(Vector2.left, 9, new EnvKey('L', 0, 0), Color.blue, new Vector2(eX, eY));
        sendRaycast(Vector2.right, 9, new EnvKey('R', 0, 0), Color.green, new Vector2(eX, eY));

        /* foreach (KeyValuePair<string, EnvInfo> kvp in info)
            Debug.LogFormat("{0}: {1}, {2}", kvp.Key, kvp.Value.getHit(), kvp.Value.getHitPoint()) ;
        */
    }

    //request the info about the environment
    public Dictionary<EnvKey, EnvInfo> requestEnvironmentInfo()
    {
        return info;
    }

    private void sendRaycast(Vector2 dir, float distance, EnvKey nameOfCheck, Color color, Vector2 entityPos)
    {
        //send Raycast, check if it hit a collider, and draw ray visually too
        RaycastHit2D hit = Physics2D.Raycast(entityPos, dir, distance, map);
        Vector2 hitPos = (hit.collider != null) ? hit.point : entityPos + dir * distance;
        Debug.DrawLine(entityPos, hitPos, color, 2f);

        //return info
        info.Add(nameOfCheck, new EnvInfo(hitPos, hit.collider));
    }
}
