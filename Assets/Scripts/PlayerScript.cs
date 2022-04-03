using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Transform BodyTransform;
    public DragRigidbodyBetter rigidBody;

    private float _prevBodyPosition;
    private float _amountToMove = 0;
    private bool isMoving = false;

    void Start()
    {
        _prevBodyPosition = BodyTransform.position.y;
    }

    public void OnGrabTrigger()
    {
        var travelDistance = BodyTransform.position.y - _prevBodyPosition;
        Debug.Log($"Travelled {travelDistance}");
        if (travelDistance > 0)
        {
            _prevBodyPosition = BodyTransform.position.y;
            _amountToMove += travelDistance;
            if (!isMoving)
                StartCoroutine(EnqeueWallMove());
        }
    }

    private IEnumerator EnqeueWallMove()
    {
        while (_amountToMove > Mathf.Epsilon)
        {
            yield return new WaitForEndOfFrame();
            var travelDistance = Mathf.Min(GameState.instance.GetSettings().testScrollSpeed, _amountToMove);
            GameState.instance.MoveWalls(travelDistance);
            rigidBody.MoveAllSprings(-travelDistance);
            _amountToMove -= travelDistance;
            if (_amountToMove < Mathf.Epsilon)
                _amountToMove = 0;
        }
        isMoving = false;
    }
}
