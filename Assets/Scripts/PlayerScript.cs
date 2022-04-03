﻿using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Transform BodyTransform;
    public Camera MainCmera;

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
            GameState.instance.OnCameraMove(travelDistance);
            MainCmera.transform.Translate(new Vector3(0, travelDistance, 0));
            _amountToMove -= travelDistance;
            if (_amountToMove < Mathf.Epsilon)
                _amountToMove = 0;
        }
        isMoving = false;
    }
}
