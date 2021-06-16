using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HP : MonoBehaviour
{
    public static int playerHP = 100;
    private static int maxPlayerHP = 100;

    public Image hpBar;

    void Awake()
    {
        playerHP = maxPlayerHP;
    }

    void Update()
    {
        //update health bar visually 
        hpBar.fillAmount = (float)playerHP / (float)maxPlayerHP;

        if (playerHP < 0)
            deathAnimation();
    }

    private void deathAnimation()
    {
        //death animation 
        SceneManager.LoadScene(1);
    }
}
