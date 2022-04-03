using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private void Awake()
    {
    }

    void Update()
    {
        transform.Translate(new Vector3(0, -GameState.instance.GetSettings().obstaclesFallSpeed, 0));
        if (transform.position.y <= -5.45f)
            gameObject.SetActive(false);
    }
}
