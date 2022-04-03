using System.Collections;
using UnityEngine;

public class DragRigidbodyBetter : MonoBehaviour
{
    public PlayerScript playerScript;
    public float Damper = 1f;

    public Rigidbody2D initRightHandGrabObject;
    public Rigidbody2D initLeftHandGrabObject;
    public Rigidbody2D initRightLegGrabObject;
    public Rigidbody2D initLeftLegGrabObject;

    public CapsuleCollider2D RightHandRootBone;
    public CapsuleCollider2D LeftHandRootBone;
    public CapsuleCollider2D RightLegRootBone;
    public CapsuleCollider2D LeftLegRootBone;

    public HingeJoint2D RightHandRootBoneJoint;
    public HingeJoint2D LeftHandRootBoneJoint;
    public HingeJoint2D RightLegRootBoneJoint;
    public HingeJoint2D LeftLegRootBoneJoint;

    public HingeJoint2D RightHandBoneJoint;
    public HingeJoint2D LeftHandBoneJoint;
    public HingeJoint2D RightLegBoneJoint;
    public HingeJoint2D LeftLegBoneJoint;

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

    private float dedExtension;

    private float[] fDistStd = new float[4];
    private float[] fDistCurrent = new float[4];
    private float[] fExtensionCurrentSqr = new float[4];
    private Rigidbody2D[] rbLimbs = new Rigidbody2D[4];
    private Grabber[] grLimbs = new Grabber[4];


    const int iCapsulePerLimb = 3;
    private CapsuleCollider2D[] ccCapsules = new CapsuleCollider2D[4 * iCapsulePerLimb];

    public float fLimbExtentionMax = 0.4f;

    CapsuleCollider2D[] getAllLimbsCapsuleColliders()
    {
        var result = new CapsuleCollider2D[12];

        result[0] = RightHandRootBone.GetComponentInChildren<CapsuleCollider2D>();
        result[1] = result[0].GetComponentInChildren<CapsuleCollider2D>();
        result[2] = result[1].GetComponentInChildren<CapsuleCollider2D>();

        result[3] = LeftHandRootBone.GetComponentInChildren<CapsuleCollider2D>();
        result[4] = result[3].GetComponentInChildren<CapsuleCollider2D>();
        result[5] = result[4].GetComponentInChildren<CapsuleCollider2D>();

        result[6] = RightLegRootBone.GetComponentInChildren<CapsuleCollider2D>();
        result[7] = result[6].GetComponentInChildren<CapsuleCollider2D>();
        result[8] = result[7].GetComponentInChildren<CapsuleCollider2D>();

        result[9] = LeftLegRootBone.GetComponentInChildren<CapsuleCollider2D>();
        result[10] = result[9].GetComponentInChildren<CapsuleCollider2D>();
        result[11] = result[10].GetComponentInChildren<CapsuleCollider2D>();

        return result;
    }

    private void Start()
    {
        var rhDistVec = RightHandBoneJoint.anchor - RightHandRootBoneJoint.anchor;
        fDistStd[RightHand] = Mathf.Sqrt(rhDistVec.x * rhDistVec.x + rhDistVec.y * rhDistVec.y);

        var lhDistVec = LeftHandBoneJoint.anchor - LeftHandRootBoneJoint.anchor;
        fDistStd[LeftHand] = Mathf.Sqrt(lhDistVec.x * lhDistVec.x + lhDistVec.y * lhDistVec.y);

        var rlDistVec = RightLegBoneJoint.anchor - RightLegRootBoneJoint.anchor;
        fDistStd[RightLeg] = Mathf.Sqrt(rlDistVec.x * rlDistVec.x + rlDistVec.y * rlDistVec.y);

        var llDistVec = LeftLegBoneJoint.anchor - LeftLegRootBoneJoint.anchor;
        fDistStd[LeftLeg] = Mathf.Sqrt(llDistVec.x * llDistVec.x + llDistVec.y * llDistVec.y);

        rbLimbs[RightHand] = initRightHandGrabObject;
        rbLimbs[LeftHand] = initLeftHandGrabObject;
        rbLimbs[RightLeg] = initRightLegGrabObject;
        rbLimbs[LeftLeg] = initLeftLegGrabObject;

        grLimbs[RightHand] = initRightHandGrabObject.GetComponentInChildren<Grabber>();
        grLimbs[LeftHand] = initLeftHandGrabObject.GetComponentInChildren<Grabber>();
        grLimbs[RightLeg] = initRightLegGrabObject.GetComponentInChildren<Grabber>();
        grLimbs[LeftLeg] = initLeftLegGrabObject.GetComponentInChildren<Grabber>();

        ccCapsules = getAllLimbsCapsuleColliders();
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    private float get2dDistanceFrom3DVectors(Vector3 a, Vector3 b)
    {
        Vector2 vDist = new Vector2(b.x - a.x, b.y - a.y);
        return vDist.sqrMagnitude;
    }

    private void UpdateDedExtension()
    {
        /*
        var rhDistVec = RightHandBoneJoint.anchor - RightHandRootBoneJoint.anchor;
        fDistCurrent[RightHand] = Mathf.Sqrt(rhDistVec.x* rhDistVec.x + rhDistVec.y * rhDistVec.y);
        fExtensionCurrentSqr[RightHand] = (fDistCurrent[RightHand] - fDistStd[RightHand]) * (fDistCurrent[RightHand] - fDistStd[RightHand]);

        var lhDistVec = LeftHandBoneJoint.anchor - LeftHandRootBoneJoint.anchor;
        fDistCurrent[LeftHand] = Mathf.Sqrt(lhDistVec.x * lhDistVec.x + lhDistVec.y * lhDistVec.y);
        fExtensionCurrentSqr[LeftHand] = (fDistCurrent[LeftHand] - fDistStd[LeftHand]) * (fDistCurrent[LeftHand] - fDistStd[LeftHand]);

        var rlDistVec = RightLegBoneJoint.anchor - RightLegRootBoneJoint.anchor;
        fDistCurrent[RightLeg] = Mathf.Sqrt(rlDistVec.x * rlDistVec.x + rlDistVec.y * rlDistVec.y);
        fExtensionCurrentSqr[RightLeg] = (fDistCurrent[RightLeg] - fDistStd[RightLeg]) * (fDistCurrent[RightLeg] - fDistStd[RightLeg]);

        var llDistVec = LeftLegBoneJoint.anchor - LeftLegRootBoneJoint.anchor;
        fDistCurrent[LeftLeg] = Mathf.Sqrt(llDistVec.x * llDistVec.x + llDistVec.y * llDistVec.y);
        fExtensionCurrentSqr[LeftLeg] = (fDistCurrent[LeftLeg] - fDistStd[LeftLeg]) * (fDistCurrent[LeftLeg] - fDistStd[LeftLeg]);
        */

        /*for (int iLimbIdx = RightHand; iLimbIdx < LeftLeg; iLimbIdx++)
        {
            float fCurLimbExt = 0.0f;
            for (int iCapsuleIdx = 0; iCapsuleIdx < iCapsulePerLimb; iCapsuleIdx++)
            {
                Vector2 vExt = ccCapsules[iLimbIdx * iCapsulePerLimb + iCapsuleIdx].bounds.extents;
                fCurLimbExt += vExt.SqrMagnitude();
            }
            fExtensionCurrentSqr[iLimbIdx] = fCurLimbExt;
        }*/

        for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
        {
            if (connectedJoints[iLimbIdx] != null)
            {
                var spring = connectedJoints[iLimbIdx].GetComponent<SpringJoint2D>();
                if (!spring.connectedBody)
                {
                    fExtensionCurrentSqr[iLimbIdx] = 0.0f;
                    continue;
                }
                var connectedPosition = spring.connectedBody.transform.TransformPoint(spring.connectedAnchor);
                fExtensionCurrentSqr[iLimbIdx] = get2dDistanceFrom3DVectors(spring.transform.position, connectedPosition);
            }
            else
            {
                fExtensionCurrentSqr[iLimbIdx] = 0.0f;
            }
        }
    }

    private void UpdateDedSpringJoint()
    {
        for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
        {
            if (fExtensionCurrentSqr[iLimbIdx] > fLimbExtentionMax && cureentDragJoint != iLimbIdx && connectedJoints[iLimbIdx] != null)
            {
                ReleaseSpring(grLimbs[iLimbIdx], iLimbIdx);
            }
        }
    }

    private void Update()
    {
        UpdateDedExtension();
        UpdateDedSpringJoint();

        if (!_isInitialized)
        {
            TryGrab(initRightHandGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initLeftHandGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initLeftLegGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initRightHandGrabObject.transform.position);
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


        // We need to actually hit an object


    }

    private void TryGrab(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y),
            Vector2.zero, Mathf.Infinity);
        if (!hit)
            return;
        Rigidbody2D rb = hit.rigidbody;
        // We need to hit a rigidbody that is not kinematic
        if (!rb || rb.isKinematic)
            return;


        int hitBodyIndex = -1;
        if (rb == initRightHandGrabObject)
            hitBodyIndex = 0;
        if (rb == initLeftHandGrabObject)
            hitBodyIndex = 1;
        if (rb == initRightLegGrabObject)
            hitBodyIndex = 2;
        if (rb == initLeftLegGrabObject)
            hitBodyIndex = 3;
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
                springJoint.transform.position = GameState.instance.GetConnectToWallPosition(springJoint.transform.position);
                currentGrabber.Grab(springJoint);
                playerScript.OnGrabTrigger();
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
        springJoint.frequency = 10;
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