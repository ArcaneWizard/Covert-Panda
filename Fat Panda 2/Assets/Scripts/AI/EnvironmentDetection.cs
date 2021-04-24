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


        //A|B represents: type of check | position check starts
        //R = right, L = left, then U = Upper, D = Lower

        //G for ground check
        sendRaycast(Vector2.down, 6, new EnvKey('G', 0, 0), Color.magenta, new Vector2(eX, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', 1, 0), Color.magenta, new Vector2(eX + 1.5f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', 2, 0), Color.magenta, new Vector2(eX + 3f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', 3, 0), Color.magenta, new Vector2(eX + 4.5f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', 4, 0), Color.magenta, new Vector2(eX + 6f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', 5, 0), Color.magenta, new Vector2(eX + 7.5f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', -1, 0), Color.magenta, new Vector2(eX - 1.5f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', -2, 0), Color.magenta, new Vector2(eX - 3f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', -3, 0), Color.magenta, new Vector2(eX - 4.5f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', -4, 0), Color.magenta, new Vector2(eX - 6f, eY));
        sendRaycast(Vector2.down, 6, new EnvKey('G', -5, 0), Color.magenta, new Vector2(eX - 7.5f, eY));

        //L for left wall check
        sendRaycast(Vector2.left, 9, new EnvKey('L', 0, 0), Color.blue, new Vector2(eX, eY));

        //R for right wall check
        sendRaycast(Vector2.right, 9, new EnvKey('R', 0, 0), Color.green, new Vector2(eX, eY));

        //C for ceiling check
        sendRaycast(Vector2.up, 6, new EnvKey('C', 0, 0), Color.cyan, new Vector2(eX, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', 1, 0), Color.cyan, new Vector2(eX + 1.5f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', 2, 0), Color.cyan, new Vector2(eX + 3f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', 3, 0), Color.cyan, new Vector2(eX + 4.5f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', 4, 0), Color.cyan, new Vector2(eX + 6f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', 5, 0), Color.cyan, new Vector2(eX + 7.5f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', -1, 0), Color.cyan, new Vector2(eX - 1.5f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', -2, 0), Color.cyan, new Vector2(eX - 3f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', -3, 0), Color.cyan, new Vector2(eX - 4.5f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', -4, 0), Color.cyan, new Vector2(eX - 6f, eY));
        sendRaycast(Vector2.up, 6, new EnvKey('C', -5, 0), Color.cyan, new Vector2(eX - 7.5f, eY));

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
