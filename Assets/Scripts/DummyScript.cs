using UnityEngine;

public class DummyScript : MonoBehaviour
{
    public DragRigidbodyBetter controller;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerPart") && !controller.bFalling)
            controller.DedFall();
    }
}
