using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MecTimeScale : MonoBehaviour
{
    public bool loadDiffScene;
    public float timeScale = 1.0f;
    private int currSceneIdx = 0;

    void Start()
    {
        float a = 1;
        Timing.RunSafeCoroutine(timer(a), gameObject);
    }

    void OnValidate()
    {
        if (loadDiffScene)
        {
            loadDiffScene = false;
            currSceneIdx = (++currSceneIdx) % 2;
            SceneManager.LoadScene(currSceneIdx);
        }
        Time.timeScale = timeScale;
    }

    private IEnumerator<float> timer(float a)
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(a);
            Debug.Log("ow");
        }

    }
}
