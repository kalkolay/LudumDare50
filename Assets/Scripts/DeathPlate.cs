using UnityEngine;

public class DeathPlate : MonoBehaviour
{
    public bool isKillZone = true;
    private bool isDead = false;
    public event System.Action OnDead;
    public event System.Action OnWin;
    public event System.Action OnSpawnDed;
    public bool DeactiveteObstaclesOnTouch = false;
    
    void Start()
    {
        isDead = false;
    }

    void Update()
    {
        
    }

    public void onNewGame()
    {
        isDead = false;
        GameState.instance.Score = 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            GameState.instance.Score += (int)rb.mass;
        } 
        else
        {
            GameState.instance.Score += 1;
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
                isDead = true;
                if (GameState.instance.IsWin)
                    OnWin?.Invoke();
                else
                    OnDead?.Invoke();
            }
            else
            {
                GameState.instance.IsWin = true;
                OnSpawnDed?.Invoke();
            }
        }
    }

    public void Restart()
    {
        isDead = false;
    }

    public void AddMenu(Menu menu)
    {
        OnDead += menu.Death;
        OnWin += menu.Win;
    }
}
