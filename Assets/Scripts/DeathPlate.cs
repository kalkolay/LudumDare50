using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlate : MonoBehaviour
{
    public bool isKillZone = true;
    private bool isDead = false;
    public event System.Action OnDead;
    public bool DeactiveteObstaclesOnTouch = false;
    public Menu menu;
    public DeathRend deathRend;
    private int score = 0;
    
    void Start()
    {
        isDead = false;
        if (isKillZone)
            OnDead += menu.Death;
    }

    void Update()
    {
        
    }

    public void onNewGame()
    {
        isDead = false;
        score = 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            score += (int)rb.mass;
        } 
        else
        {
            score += 1;
        }
        if (DeactiveteObstaclesOnTouch && col.tag == "Stone")
        {
            col.gameObject.SetActive(false);
        }
        
        if (col.tag == "Player" && !isDead)
        {
            if (isKillZone)
            {
                Debug.Log("You Die");
                deathRend.SetScore(score);
                isDead = true;
            }
            OnDead?.Invoke();
        }
    }

    public void Restart()
    {
        isDead = false;
    }
}
