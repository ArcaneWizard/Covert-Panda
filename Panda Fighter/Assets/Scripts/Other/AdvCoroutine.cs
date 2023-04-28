using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvCoroutine : MonoBehaviour
{
    public Coroutine coroutine { get; private set; }
    public bool IsCoroutineActive { get; private set; }

    private IEnumerator target;
    private IEnumerator startCoroutine;
    private MonoBehaviour owner;

    public AdvCoroutine(MonoBehaviour owner, IEnumerator target)
    {
        this.owner = owner;
        this.target = target;
    }

    public void StartCoroutine()
    {
        startCoroutine = startCoroutineHelper();
        owner.StartCoroutine(startCoroutine);
    }

    private IEnumerator startCoroutineHelper()
    {
        IsCoroutineActive = true;
        yield return owner.StartCoroutine(target);
        IsCoroutineActive = false;
    }

    public void StopCoroutine()
    {
        owner.StopCoroutine(target);
        owner.StopCoroutine(startCoroutine);
        IsCoroutineActive = false;
    }

    public IEnumerator WaitTillCoroutineIsOver()
    {
        while (IsCoroutineActive)
            yield return null;
    }
}
