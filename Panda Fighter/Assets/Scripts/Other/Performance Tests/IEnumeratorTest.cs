using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEnumeratorTest : MonoBehaviour
{
    Coroutine storage;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started at " + Time.time);
        timer = 10f;
        storage = StartCoroutine(writeCheese());
        Debug.Log("ended at " + Time.time);
    }

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
        else
        {
            Debug.Log("timer finished at " + Time.time);
            StopCoroutine(storage);
            timer = 100f;
        }

        Debug.Log("storage is " + storage + ", " + storage == null);
    }

    private IEnumerator writeCheese()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("cheese");
        }
    }
}
