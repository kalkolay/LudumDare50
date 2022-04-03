﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDrag : MonoBehaviour
{
    public GameObject selectedObject;
    Vector3 offset;
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject)
            {
                selectedObject = targetObject.transform.gameObject;
                offset = selectedObject.transform.position - mousePosition;
            }
        }
        if (selectedObject)
        {
            selectedObject.transform.position = mousePosition + offset;
        }
        //if (Input.GetMouseButtonUp(0) && selectedObject)
        //{
        //    Rigidbody2D rigidbodier = selectedObject.GetComponent<Rigidbody2D>();
        //    rigidbodier.constraints = 0;
        //    selectedObject = null;
        //}
    }
}
