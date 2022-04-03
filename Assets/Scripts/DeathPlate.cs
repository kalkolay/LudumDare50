using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlate : MonoBehaviour
{
    private bool isDead = false;
    public event System.Action OnDead;
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onNewGame()
    {
        isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDead)
        {
            Debug.Log("You Die");
            OnDead?.Invoke();
            isDead = true;
        }
    }
}
