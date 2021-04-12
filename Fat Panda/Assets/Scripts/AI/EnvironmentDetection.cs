using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDetection : MonoBehaviour
{
    private LayerMask map = (1 << 11);

    private Dictionary<string, EnvInfo> info = new Dictionary<string, EnvInfo>();

    //cast multiple raycasts to gather info about the environment
    public void scanEnvironment(Transform entity) {

        info.Clear();

        float eX = entity.transform.position.x;
        float eY = entity.transform.position.y;


        //A|B represents: type of check | position check stars
        //R = right, L = left, then U = Upper, D = Lower

        //G for ground check
        sendRaycast(Vector2.down, 6, "G", Color.magenta, new Vector2(eX, eY));
        sendRaycast(Vector2.down, 6, "G|R1", Color.magenta, new Vector2(eX + 3, eY));
        sendRaycast(Vector2.down, 6, "G|R2", Color.magenta, new Vector2(eX + 6, eY));
        sendRaycast(Vector2.down, 6, "G|R3", Color.magenta, new Vector2(eX + 9, eY));
        sendRaycast(Vector2.down, 6, "G|R4", Color.magenta, new Vector2(eX + 12, eY));
        sendRaycast(Vector2.down, 6, "G|R5", Color.magenta, new Vector2(eX + 15, eY));
        sendRaycast(Vector2.down, 6, "G|L1", Color.magenta, new Vector2(eX - 3, eY));
        sendRaycast(Vector2.down, 6, "G|L2", Color.magenta, new Vector2(eX - 6, eY));
        sendRaycast(Vector2.down, 6, "G|L3", Color.magenta, new Vector2(eX - 9, eY));
        sendRaycast(Vector2.down, 6, "G|L4", Color.magenta, new Vector2(eX - 12, eY));
        sendRaycast(Vector2.down, 6, "G|L5", Color.magenta, new Vector2(eX - 15, eY));

        //LW for left wall check
        sendRaycast(Vector2.left, 14, "LW", Color.blue, new Vector2(eX, eY));
        sendRaycast(Vector2.left, 14, "LW|L1U1", Color.blue, new Vector2(eX, eY + 2.5f));
        sendRaycast(Vector2.left, 14, "LW|L1D1", Color.blue, new Vector2(eX, eY - 2.5f));
        sendRaycast(Vector2.left, 14, "LD|L1U2", Color.blue, new Vector2(eX, eY + 5f));
        sendRaycast(Vector2.left, 14, "LW|L1D2", Color.blue, new Vector2(eX, eY - 5f));
        sendRaycast(Vector2.left, 14, "LW|L2U1", Color.blue, new Vector2(eX - 2.5f, eY + 2.5f));
        sendRaycast(Vector2.left, 14, "LW|L2D1", Color.blue, new Vector2(eX - 2.5f, eY - 2.5f));
        sendRaycast(Vector2.left, 14, "LW|L2U2", Color.blue, new Vector2(eX-2.5f, eY + 5f));
        sendRaycast(Vector2.left, 14, "LW|L2D2", Color.blue, new Vector2(eX-2.5f, eY - 5f));

        //RW for right wall check
        sendRaycast(Vector2.right, 14, "RW", Color.green, new Vector2(eX, eY));
        sendRaycast(Vector2.right, 14, "RW|RU", Color.green, new Vector2(eX, eY + 2.5f));
        sendRaycast(Vector2.right, 14, "RW|RD", Color.green, new Vector2(eX, eY - 2.5f));
        sendRaycast(Vector2.right, 14, "RW|RU2", Color.green, new Vector2(eX, eY + 5f));
        sendRaycast(Vector2.right, 14, "RW|RD2", Color.green, new Vector2(eX, eY - 5f));
        sendRaycast(Vector2.right, 14, "RW|R2U", Color.green, new Vector2(eX + 2.5f, eY + 2.5f));
        sendRaycast(Vector2.right, 14, "RW|R2D", Color.green, new Vector2(eX + 2.5f, eY - 2.5f));
        sendRaycast(Vector2.right, 14, "RW|R2U2", Color.green, new Vector2(eX + 2.5f, eY + 5f));
        sendRaycast(Vector2.right, 14, "RW|R2D2", Color.green, new Vector2(eX + 2.5f, eY - 5f));

        //C for ceiling check
        sendRaycast(Vector2.up, 6, "C", Color.cyan, new Vector2(eX, eY));
        sendRaycast(Vector2.up, 6, "C|R1", Color.cyan, new Vector2(eX + 3, eY));
        sendRaycast(Vector2.up, 6, "C|R2", Color.cyan, new Vector2(eX + 6, eY));
        sendRaycast(Vector2.up, 6, "C|R3", Color.cyan, new Vector2(eX + 9, eY));
        sendRaycast(Vector2.up, 6, "C|R4", Color.cyan, new Vector2(eX + 12, eY));
        sendRaycast(Vector2.up, 6, "C|R5", Color.cyan, new Vector2(eX + 15, eY));
        sendRaycast(Vector2.up, 6, "C|L1", Color.cyan, new Vector2(eX - 3, eY));
        sendRaycast(Vector2.up, 6, "C|L2", Color.cyan, new Vector2(eX - 6, eY));
        sendRaycast(Vector2.up, 6, "C|L3", Color.cyan, new Vector2(eX - 9, eY));
        sendRaycast(Vector2.up, 6, "C|L4", Color.cyan, new Vector2(eX - 12, eY));
        sendRaycast(Vector2.up, 6, "C|L5", Color.cyan, new Vector2(eX - 15, eY));

        /* foreach (KeyValuePair<string, EnvInfo> kvp in info)
            Debug.LogFormat("{0}: {1}, {2}", kvp.Key, kvp.Value.getHit(), kvp.Value.getHitPoint()) ;
        */

    }

    //request the info about the environment
    public Dictionary<string, EnvInfo> requestEnvironmentInfo()
    {
        return info;
    }

    private void sendRaycast(Vector2 dir, float distance, string nameOfCheck, Color color, Vector2 entityPos)
    {
        //send Raycast, check if it hit a collider, and draw ray visually too
        RaycastHit2D hit = Physics2D.Raycast(entityPos, dir, distance, map);
        Vector2 hitPos = (hit.collider != null) ? hit.point : entityPos + dir * distance;
        Debug.DrawLine(entityPos, hitPos, color, 4f);

        //return info
        info.Add(nameOfCheck, new EnvInfo(hitPos, hit.collider != null));
    }
}
