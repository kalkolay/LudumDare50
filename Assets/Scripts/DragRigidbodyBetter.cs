using System;
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
    public float fSlipCoef = 0.1f;

    public int[] aLimbConnected = new int[4];
    public event Action onDedFall;
    public bool bFalling = false;

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
        onDedFall += () => { Debug.Log("Ded is falling"); };
        bFalling = false;
        GameState.instance.AddOnDeadCallback(DedFall);
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

    private void SlipJoints()
    {
        for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
        {
            if (connectedJoints[iLimbIdx] != null)
            {
                var spring = connectedJoints[iLimbIdx].GetComponent<SpringJoint2D>();
                spring.transform.Translate(new Vector3(0.0f, -fSlipCoef * Time.deltaTime, 0.0f));
            }
        }
    }

    public void DedFall()
    {
        if (!bFalling)
        {
            bFalling = true;
            for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
            {
                ReleaseSpring(grLimbs[iLimbIdx], iLimbIdx);
            }
            onDedFall?.Invoke();
        }
    }

    private void CheckFalling()
    {
        int iConnectionCount = 0;
        for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
        {
            if (iLimbIdx == cureentDragJoint)
            {
                aLimbConnected[iLimbIdx] = 0;
                continue;
            }

            aLimbConnected[iLimbIdx] = 0;
            if (connectedJoints[iLimbIdx] != null)
            {
                var spring = connectedJoints[iLimbIdx].GetComponent<SpringJoint2D>();

                bool bLimbConnected = (spring != null && spring.connectedBody != null);

                if (bLimbConnected)
                {
                    aLimbConnected[iLimbIdx] = spring.transform.position.x < 0 ? -1 : 1;
                    iConnectionCount++;
                }
            }
        }

        if (iConnectionCount < 2)
        {
            DedFall();
        }

        if (iConnectionCount == 2)
        {
            int iWallInfo = 0;
            for (int iLimbIdx = RightHand; iLimbIdx <= LeftLeg; iLimbIdx++)
            {
                iWallInfo += aLimbConnected[iLimbIdx];
            }
            if (iWallInfo == 2 || iWallInfo == -2)
            {
                DedFall();
            }
        }
    }

    private void Update()
    {
        UpdateDedExtension();
        UpdateDedSpringJoint();
        SlipJoints();

        if (!_isInitialized)
        {
            TryGrab(initRightHandGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initLeftHandGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initLeftLegGrabObject.transform.position);
            TryRelease(true);
            TryGrab(initRightLegGrabObject.transform.position);
            TryRelease(true);
            _isInitialized = true;
        }

        UpdatePinnedSprings();
        CheckFalling();

        if (Input.GetMouseButtonDown(0) && !bFalling)
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
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(new Vector2(pos.x, pos.y),
            Vector2.zero, Mathf.Infinity);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];

            if (!hit)
                continue;
            Rigidbody2D rb = hit.rigidbody;
            // We need to hit a rigidbody that is not kinematic
            if (!rb || rb.isKinematic)
                continue;

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
                continue;

            currentGrabber = rb.gameObject.GetComponentInChildren<Grabber>();

            if (currentGrabber == null) continue;
            ReleaseSpring(currentGrabber, hitBodyIndex);
            CreateSpring(hit, currentGrabber, hitBodyIndex);
            UpdatePinnedSprings();
            cureentDragJoint = hitBodyIndex;
            _dragCoroutine = StartCoroutine(DragObject(hit.distance));
            return;
        }
    }

    private void TryRelease(bool force = false)
    {
        if (currentGrabber != null)
        {
            if (!(_dragCoroutine is null))
                StopCoroutine(_dragCoroutine);
            var springJoint = cureentDragJoint == -1 ? null : connectedJoints[cureentDragJoint];
            if ((IsCloseToWall() || force) && !(springJoint is null))
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
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var closestPosition = GameState.instance.GetConnectToWallPosition(mousePosition);
            if (IsCloseToWall())
                draggedJoint.transform.position = closestPosition;
            else
                draggedJoint.transform.position = mousePosition;

            yield return null;
        }
    }

    private bool IsCloseToWall()
    {
        var isLeftJoint = cureentDragJoint == LeftHand || cureentDragJoint == LeftLeg;
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var isCorrectJointToWall = isLeftJoint && mousePosition.x <= 0 || !isLeftJoint && mousePosition.x >= 0;
        var closestPosition = GameState.instance.GetConnectToWallPosition(mousePosition);
        return isCorrectJointToWall && Mathf.Abs(mousePosition.x - closestPosition.x) < 0.8;
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

    public void ReInitDeda(GameObject GrandDed)
    {
        playerScript = GrandDed.transform.Find("Controller").GetComponent<PlayerScript>();

        initRightHandGrabObject = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-right/char-hand-002/char-hand-003-right").GetComponent<Rigidbody2D>();
        initLeftHandGrabObject = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-left/char-hand-002/char-hand-003-left").GetComponent<Rigidbody2D>();
        initRightLegGrabObject = GrandDed.transform.Find("char-body-lower/right-leg-001/char-leg-002/char-leg-003-right").GetComponent<Rigidbody2D>();
        initLeftLegGrabObject = GrandDed.transform.Find("char-body-lower/left-leg-001/char-leg-002/char-leg-003-left").GetComponent<Rigidbody2D>();

        RightHandRootBone = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-right").GetComponent<CapsuleCollider2D>();
        LeftHandRootBone = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-left").GetComponent<CapsuleCollider2D>();
        RightLegRootBone = GrandDed.transform.Find("char-body-lower/right-leg-001").GetComponent<CapsuleCollider2D>();
        LeftLegRootBone = GrandDed.transform.Find("char-body-lower/left-leg-001").GetComponent<CapsuleCollider2D>();

        RightHandRootBoneJoint = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-right").GetComponent<HingeJoint2D>();
        LeftHandRootBoneJoint = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-left").GetComponent<HingeJoint2D>();
        RightLegRootBoneJoint = GrandDed.transform.Find("char-body-lower/right-leg-001").GetComponent<HingeJoint2D>();
        LeftLegRootBoneJoint = GrandDed.transform.Find("char-body-lower/left-leg-001").GetComponent<HingeJoint2D>();

        RightHandBoneJoint = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-right/char-hand-002/char-hand-003-right").GetComponent<HingeJoint2D>();
        LeftHandBoneJoint = GrandDed.transform.Find("char-body-lower/char-body-upper/char-hand-left/char-hand-002/char-hand-003-left").GetComponent<HingeJoint2D>();
        RightLegBoneJoint = GrandDed.transform.Find("char-body-lower/right-leg-001/char-leg-002/char-leg-003-right").GetComponent<HingeJoint2D>();
        LeftLegBoneJoint = GrandDed.transform.Find("char-body-lower/left-leg-001/char-leg-002/char-leg-003-left").GetComponent<HingeJoint2D>();

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
        onDedFall += () => { Debug.Log("Ded is falling"); };
        bFalling = false;

        _isInitialized = false;
    }
}