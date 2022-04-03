using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    Vector3 prevPosition;
    SpriteRenderer renderer;
    Rigidbody2D rigidbodier;
    CircleCollider2D collider;
    float prevDrag = 0.0f;
    float prevAngularDrag = 0.0f;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        prevPosition = transform.position;
        renderer = GetComponent<SpriteRenderer>();
        rigidbodier = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: NO HARDCODE
        if (transform.position.y <= -5.45f)
            gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collided = true;

        prevDrag = rigidbodier.drag;
        prevAngularDrag = rigidbodier.angularDrag;

        rigidbodier.drag = 12.0f;
        rigidbodier.angularDrag = 20.0f;

        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        int iContactCount = collision.GetContacts(contacts);
        int bp = 1;

#if rertert
        rigidbodier.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        FixedJoint2D joint = gameObject.GetComponent<FixedJoint2D>();

        if (joint == null)
        {
            joint = gameObject.AddComponent<FixedJoint2D>();
        }

        joint.connectedBody = collision.attachedRigidbody;
#endif
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    Debug.Log("OnCollisionStay2D");
    //    List<ContactPoint2D> contacts = new List<ContactPoint2D>();
    //    int iContactCount = collision.GetContacts(contacts);

    //    ContactPoint2D point = contacts[0];

    //    rigidbodier.AddForce(-point.normal * 10.0f);
    //}

    private void OnCollisionExit2D(Collision2D collision)
    {

        rigidbodier.drag = prevDrag;
        rigidbodier.angularDrag = prevAngularDrag;

        collided = false;


        rigidbodier.drag = 0f;
        rigidbodier.angularDrag = 0.05f;
#if rertert
        FixedJoint2D joint = gameObject.GetComponent<FixedJoint2D>();

        if (joint != null)
        {
            joint.connectedBody = null;
        }
#endif
    }
}
