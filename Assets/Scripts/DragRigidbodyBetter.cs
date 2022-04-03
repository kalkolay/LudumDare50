using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DragRigidbodyBetter : MonoBehaviour
{

    private Camera mainCamera;
    private Grabber currentGrabber;

    private List<Grabber> grabbers;
    public List<Rigidbody2D> bodies;
    public float maxDistance = 5f;

    private void Awake()
    {
        mainCamera = Camera.main;
        grabbers = new List<Grabber>(FindObjectsOfType<Grabber>());
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y),
                Vector2.zero, Mathf.Infinity);
            if (!hit)
            {
                return;
            }
            Rigidbody2D rb = hit.rigidbody;
            // We need to hit a rigidbody that is not kinematic
            if (!rb || rb.isKinematic)
            {
                return;
            }

            currentGrabber = rb.gameObject.GetComponentInChildren<Grabber>();

            if (currentGrabber == null) return;
            SetMass(150);
            currentGrabber.Release();
            CreateSpring(hit, currentGrabber);



        }
        if (Input.GetMouseButton(0))
        {
            if (currentGrabber == null) return;
            if (currentGrabber.connectedSpring == null) return;
            var newPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            foreach (var g in grabbers)
            {
                if (g != currentGrabber && g.isGrabbing)
                {
                    //Debug.Log(Vector3.Distance(newPos, g.transform.position));
                    if (Vector3.Distance(newPos, g.transform.position) > maxDistance)
                    {

                        newPos = g.transform.position - (g.transform.position - newPos).normalized * maxDistance * 0.9f;
                    }
                };
            }


            currentGrabber.connectedSpring.transform.position = newPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentGrabber != null)
            {
                SetMass(10);
                if (!currentGrabber.CanGrab)
                {
                    currentGrabber.Release();
                }

            }

        }


    }
    private void SetMass(float mass)
    {
        foreach (var body in bodies)
        {
            body.mass = mass;
        }
    }
    private SpringJoint2D CreateSpring(RaycastHit2D hit, Grabber g)
    {
        //if (!m_SpringJoint)
        //{
        var connectedSpring = new GameObject("Spring");
        connectedSpring.transform.parent = transform;
        connectedSpring.transform.localPosition = Vector3.zero;
        Rigidbody2D body = connectedSpring.AddComponent<Rigidbody2D>();
        SpringJoint2D m_SpringJoint = connectedSpring.AddComponent<SpringJoint2D>();
        g.Grab(m_SpringJoint);
        body.isKinematic = true;

        //}

        m_SpringJoint.transform.position = hit.point;
        m_SpringJoint.anchor = Vector3.zero;


        m_SpringJoint.dampingRatio = 1;
        m_SpringJoint.autoConfigureDistance = false;
        m_SpringJoint.distance = 0;
        m_SpringJoint.frequency = 50;
        m_SpringJoint.connectedBody = g.GetComponent<Rigidbody2D>();



        return m_SpringJoint;
    }


}
