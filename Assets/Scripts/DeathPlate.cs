using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlate : MonoBehaviour
{
    private bool isDead = false;
    public event System.Action OnDead;
    public bool DeactiveteObstaclesOnTouch = false;
    public Menu menu;
    public DeathRend deathRend;
    private int score = 0;
    
    void Start()
    {
        isDead = false;
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
            Debug.Log("You Die");
            deathRend.SetScore(score);
            OnDead?.Invoke();
            isDead = true;
        }
    }
}
