using System.Collections;

using UnityEngine;

public class IEnumeratorTest : MonoBehaviour
{
    Coroutine storage;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 10f;
        storage = StartCoroutine(writeCheese());
    }

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
        else {
            StopCoroutine(storage);
            timer = 100f;
        }
    }

    private IEnumerator writeCheese()
    {
        while (true) {
            yield return new WaitForSeconds(1f);
            Debug.Log("cheese");
        }
    }
}
