using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragRigidbodyBetter : MonoBehaviour
{

    public float Damper = 1f;
    public Rigidbody2D initRightHandGrabObject;
    public Rigidbody2D initLeftHandGrabObject;
    public Rigidbody2D initRightLegGrabObject;
    public Rigidbody2D initLeftLegGrabObject;

    private int m_SpringCount = 1;
    private LineRenderer m_SpringRenderer;
    private Camera mainCamera;
    private Grabber currentGrabber;

    private SpringJoint2D[] connectedJoints = new SpringJoint2D[4];
    private bool _isInitialized = false;
    private Coroutine _dragCoroutine;

    private const int RightHand = 0;
    private const int LeftHand = 1;
    private const int RightLeg = 2;
    private const int LeftLeg = 3;
    private int cureentDragJoint = -1;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    private void Update()
    {
        if (!_isInitialized)
        {
            TryGrab(initRightHandGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initLeftHandGrabObject.transform.position);
            TryRelease(true);
            _isInitialized = true;
        }

        UpdatePinnedSprings();

        if (Input.GetMouseButtonDown(0))
        {
            TryGrab(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButtonUp(0))
        {
            TryRelease();
        }
    }

    private void TryGrab(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y),
            Vector2.zero, Mathf.Infinity);
        if (!hit)
            return;
        Rigidbody2D rb = hit.rigidbody;
        if (!rb || rb.isKinematic)
            return;


        int hitBodyIndex = -1;
        if (rb == initRightHandGrabObject)
            hitBodyIndex = RightHand;
        if (rb == initLeftHandGrabObject)
            hitBodyIndex = LeftHand;
        if (rb == initRightLegGrabObject)
            hitBodyIndex = RightLeg;
        if (rb == initLeftLegGrabObject)
            hitBodyIndex = LeftLeg;
        if (hitBodyIndex == -1)
            return;

        currentGrabber = rb.gameObject.GetComponentInChildren<Grabber>();

        if (currentGrabber == null) return;
        ReleaseSpring(currentGrabber, hitBodyIndex);
        CreateSpring(hit, currentGrabber, hitBodyIndex);
        UpdatePinnedSprings();
        cureentDragJoint = hitBodyIndex;
        _dragCoroutine = StartCoroutine(DragObject(hit.distance));
    }

    private void TryRelease(bool force = false)
    {
        if (currentGrabber != null)
        {
            if (!(_dragCoroutine is null))
                StopCoroutine(_dragCoroutine);
            var springJoint = cureentDragJoint == -1 ? null : connectedJoints[cureentDragJoint];
            if ((currentGrabber.CanGrab || force) && !(springJoint is null))
            {
                currentGrabber.Grab(springJoint);
            }
            else if (cureentDragJoint != -1)
            {
                ReleaseSpring(currentGrabber, cureentDragJoint);
                m_SpringRenderer = null;
            }
            cureentDragJoint = -1;
        }
    }

    private void CreateSpring(RaycastHit2D hit, Grabber g, int hitBodyIndex)
    {
        var springJoint = connectedJoints[hitBodyIndex];
        if (springJoint is null)
        {
            var connectedSpring = new GameObject("Spring-" + m_SpringCount);
            connectedSpring.transform.parent = transform;
            connectedSpring.transform.localPosition = Vector3.zero;
            Rigidbody2D body = connectedSpring.AddComponent<Rigidbody2D>();
            springJoint = connectedJoints[hitBodyIndex] = connectedSpring.AddComponent<SpringJoint2D>();
            body.isKinematic = true;
            m_SpringCount++;
        }

        springJoint.transform.position = hit.point;
        springJoint.anchor = Vector3.zero;


        springJoint.dampingRatio = Damper;
        springJoint.autoConfigureDistance = false;
        springJoint.distance = 0;
        springJoint.frequency = 100000;
        springJoint.connectedBody = g.GetComponent<Rigidbody2D>();


        if (m_SpringRenderer)
        {
            m_SpringRenderer.enabled = true;
        }
    }

    private void ReleaseSpring(Grabber g, int hitBodyIndex)
    {
        g.Release();
        SpringJoint2D spring = connectedJoints[hitBodyIndex];

        if (spring != null)
        {
            GameObject.Destroy(spring.gameObject);
            connectedJoints[hitBodyIndex] = null;
        }
    }


    private IEnumerator DragObject(float distance)
    {
        while (Input.GetMouseButton(0))
        {
            var draggedJoint = cureentDragJoint == -1 ? null : connectedJoints[cureentDragJoint];
            if (draggedJoint is null) yield break;
            draggedJoint.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            yield return null;
        }
    }


    private void UpdatePinnedSprings()
    {
        foreach (Transform child in transform)
        {
            var spring = child.GetComponent<SpringJoint2D>();
            var renderer = child.GetComponentInChildren<LineRenderer>();

            if (!spring.connectedBody)
                continue;

            var connectedPosition = spring.connectedBody.transform.TransformPoint(spring.connectedAnchor);

            if (renderer && renderer.positionCount >= 2)
            {
                renderer.SetPosition(0, spring.transform.position);
                renderer.SetPosition(1, connectedPosition);
            }
        }
    }
}
