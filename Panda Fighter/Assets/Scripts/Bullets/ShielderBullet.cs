using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeToGrow = 1.5f;
    private float timer;
    public Vector2 startSize = new Vector2(0.5f, 0.5f);
    public Vector2 endSize = new Vector2(3f, 3f);

    void OnEnable()
    {
        transform.localScale = startSize;
        timer = timeToGrow;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            transform.localScale = (endSize - startSize) * (1 - timer / timeToGrow) + startSize;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            gameObject.SetActive(false);
    }
}
