using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plasmaBullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            gameObject.SetActive(false);

        else if (transform.parent.parent.name == "Player weapon ammo" && col.gameObject.layer == 9)
        {
            dmgEnemy(col);
            gameObject.SetActive(false);
        }

        else if (transform.parent.parent.name == "Alien weapon ammo" && col.gameObject.layer == 12)
        {
            HP.playerHP -= 5;
            gameObject.SetActive(false);
        }
    }

    private void dmgEnemy(Collision2D col)
    {
        Debug.Log(col.gameObject);
        if (col.transform.parent.GetComponent<EnemyHP>() != null)
            col.transform.parent.GetComponent<EnemyHP>().hp -= 10;
        else if (col.transform.parent.parent.parent.GetComponent<EnemyHP>() != null)
            col.transform.parent.parent.parent.GetComponent<EnemyHP>().hp -= 10;
        else
            Debug.LogError("Can't find enemy hp script");
    }
}
