using System.Collections;

using UnityEngine;

public class CoroutineWithData
{
    public Coroutine Coroutine { get; private set; }
    public object Result;
    private IEnumerator target;

    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        Coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext()) {
            Result = target.Current;
            yield return Result;
        }
    }
}