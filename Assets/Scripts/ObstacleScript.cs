using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    Rigidbody2D rigidBody;
    bool isMultiplayed = false;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rigidBody.velocity.magnitude > GameState.instance.GetSettings().obstacleMaxSpeed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * GameState.instance.GetSettings().obstacleMaxSpeed;
        }
    }

    private void OnEnable()
    {
        isMultiplayed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerPart") && !isMultiplayed)
        {
            rigidBody.velocity = new Vector3(0, 0, 0);
            isMultiplayed = true;
        }
    }
}
