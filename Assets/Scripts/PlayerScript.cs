using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public DragRigidbodyBetter dragBody;

    void Start()
    {
    }

    void Update()
    {
        //if (dragBody.currentGrabber is null || !dragBody.currentGrabber.isGrabbing)
        //    return;
        //dragBody.currentGrabber.gameObject.transform.Translate(0f, GameState.instance.GetSettings().slideDownSpeed, 0f);
    }
}
