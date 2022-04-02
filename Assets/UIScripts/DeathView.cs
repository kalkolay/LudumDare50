using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer youDiedRend;
    [SerializeField] private Text score;

    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float youDiedAppear = 0.5f;

    private bool clicked;
    private bool isAppear = false;

    private float transparency = 0f; 

    private Color youDiedColor;
    private Color scoreColor;

    private int scoreVal = 1488;

    void Start()
    {
        InitDeathView(scoreVal);
    }

    void InitDeathView(int scoreValue)
    {
        score.text += scoreValue;

        StartCoroutine(Appear());
    }

    void Update()
    {
        clicked = Input.GetMouseButtonDown(0);

        if (isAppear && clicked)
        {
            StartCoroutine(Fade());
        }
    }
    
    private IEnumerator Appear() 
    {
        youDiedColor = youDiedRend.color;
        scoreColor = score.color;

        while (transparency < 1f) 
        { 
            transparency += Time.deltaTime * youDiedAppear; 
            
            youDiedColor.a = transparency;
            youDiedRend.color = youDiedColor; 
            
            scoreColor.a = transparency;
            score.color = scoreColor;
            yield return 0; 
        }

        isAppear = true;
    }

    private IEnumerator Fade() 
    {
        youDiedColor = youDiedRend.color;
        scoreColor = score.color;

        while (transparency > 0f) 
        { 
            transparency -= Time.deltaTime * fadeTime; 
            
            youDiedColor.a = transparency;
            youDiedRend.color = youDiedColor; 
            
            scoreColor.a = transparency;
            score.color = scoreColor;
            yield return 0; 
        }

        SceneManager.LoadScene("SampleScene");
    }
}
