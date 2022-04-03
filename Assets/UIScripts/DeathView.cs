using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathView : MonoBehaviour
{
    [SerializeField] private GameObject youDied;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject score;
    [SerializeField] private Transform mainCamera;

    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float youDiedAppear = 0.5f;

    private SpriteRenderer youDiedRend;
    private SpriteRenderer backgroundRend;
    private Text scoreText;

    private bool clicked;
    private bool isAppear = false;

    private float transparency = 0f; 

    private Color youDiedColor;
    private Color backgroundColor;
    private Color scoreColor;

    private int scoreVal = 1488;

    void Start()
    {
        youDiedRend = youDied.GetComponent<SpriteRenderer>();
        backgroundRend = background.GetComponent<SpriteRenderer>();
        scoreText = score.GetComponent<Text>();

        Vector3 cameraPos = mainCamera.transform.position;

        youDied.transform.position = new Vector3(cameraPos.x, cameraPos.y, 1);
        background.transform.position = new Vector3(cameraPos.x, cameraPos.y, 1);
        score.transform.position = new Vector3(cameraPos.x, cameraPos.y - 1, 0);

        //InitDeathView(scoreVal);
    }

    void InitDeathView(int scoreValue)
    {
        scoreText.text += scoreValue;

        StartCoroutine(Appear());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))  // For testing purposes
        {
            InitDeathView(scoreVal);
        }

        clicked = Input.GetMouseButtonDown(0);

        if (isAppear && clicked)
        {
            StartCoroutine(Fade());
        }
    }
    
    private IEnumerator Appear() 
    {
        youDiedColor = youDiedRend.color;
        backgroundColor = backgroundRend.color;
        scoreColor = scoreText.color;

        while (transparency < 1f) 
        { 
            transparency += Time.deltaTime * youDiedAppear; 
            
            youDiedColor.a = transparency;
            youDiedRend.color = youDiedColor; 

            backgroundColor.a = transparency;
            backgroundRend.color = backgroundColor;
            
            scoreColor.a = transparency;
            scoreText.color = scoreColor;
            yield return 0; 
        }

        isAppear = true;
    }

    private IEnumerator Fade() 
    {
        youDiedColor = youDiedRend.color;
        backgroundColor = backgroundRend.color;
        scoreColor = scoreText.color;

        while (transparency > 0f) 
        { 
            transparency -= Time.deltaTime * fadeTime; 
            
            youDiedColor.a = transparency;
            youDiedRend.color = youDiedColor; 

            backgroundColor.a = transparency;
            backgroundRend.color = backgroundColor;
            
            scoreColor.a = transparency;
            scoreText.color = scoreColor;
            yield return 0; 
        }

        SceneManager.LoadScene("GameScene");
    }
}
