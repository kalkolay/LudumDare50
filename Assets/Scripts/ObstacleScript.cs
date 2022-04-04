using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    Rigidbody2D rigidBody;
    bool isMultiplayed = false;
    public float speedModifier = 1;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var isVelocityUp = rigidBody.velocity.y > 0;
        var maxSpeed = isVelocityUp && isMultiplayed ? GameState.instance.GetSettings().obstacleMaxSpeed * speedModifier : GameState.instance.GetSettings().obstacleMaxSpeed;
        if (rigidBody.velocity.magnitude > maxSpeed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
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
