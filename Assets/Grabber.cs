using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Grabber : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    [HideInInspector]public Joint2D connectedSpring;

    public bool isGrabbing;
    private bool canGrab = false;



    public bool CanGrab
    {
        get
        {
            return canGrab;
        }
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    //public bool CanGrab()
    //{
    //    return true;
    //}

    public void Grab(Joint2D spring)
    {
        connectedSpring = spring;
        isGrabbing = true;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Release()
    {
        rigidbody.constraints = RigidbodyConstraints2D.None;
        isGrabbing = false;
        if (connectedSpring)
        {
            Destroy(connectedSpring.gameObject);
            connectedSpring = null;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Finish"))
        {
            canGrab = true;
        }
       
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        canGrab = false;
    }
}
