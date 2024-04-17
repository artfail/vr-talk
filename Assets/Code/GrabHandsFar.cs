using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHandsFar : MonoBehaviour
{
    public LayerMask grabbableLayer;
    public Transform holdPoint;

    private float launchForce = 10;
    private float sphereCastDist = 50;
    private float sphereCastRadius = .2f;


    public Material[] handMats;
    public Renderer handRenderer;
    private Transform grabbableObject = null;
    private Transform heldObject = null;
    private Rigidbody heldRigidbody = null;

    public LineRenderer line;


    public void GestureOn(int type)
    {
        handRenderer.material = handMats[type];

        if (type == 1) //grip
        {
            line.enabled = true;
        }

        else if (type == 2 && heldObject == null) //full fist
        {
            CheckForPickUp();
        }
    }

    public void GestureOff(int type)
    {
        handRenderer.material = handMats[0];
        line.enabled = false;

        if (type == 2 && heldObject != null) //full fist
        {
            LaunchObject();
        }
    }

    void CheckForPickUp()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out hit, sphereCastDist, grabbableLayer))
        {
            StartCoroutine(PickUpObject(hit.transform));
        }
    }

    IEnumerator PickUpObject(Transform transform)
    {
        heldObject = transform;
        heldRigidbody = heldObject.GetComponent<Rigidbody>();
        heldRigidbody.isKinematic = true;

        float t = 0;
        while (t < .4f) //snap to position when close
        {
            heldRigidbody.position = Vector3.Lerp(heldRigidbody.position, holdPoint.position, t);
            t += Time.deltaTime;
            yield return null;
        }
        SnapToHand();
    }

    void SnapToHand()
    {
        heldObject.position = holdPoint.position;
        heldObject.parent = transform;
    }

    void LaunchObject()
    {
        StopAllCoroutines();
        SnapToHand();

        heldRigidbody.isKinematic = false;
        heldRigidbody.AddForce(transform.forward * launchForce, ForceMode.VelocityChange);
        heldObject.parent = null;
        StartCoroutine(LetGo());
    }

    IEnumerator LetGo()
    {
        yield return new WaitForSeconds(.1f);
        heldObject = null;
    }


    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out hit, sphereCastDist, grabbableLayer))
        {
            if (grabbableObject != hit.transform)
            {
                if (grabbableObject != null)
                {
                    grabbableObject.GetComponent<Renderer>().material.color = Color.white;
                }
                grabbableObject = hit.transform;
                grabbableObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            if (grabbableObject != null)
            {
                grabbableObject.GetComponent<Renderer>().material.color = Color.white;
                grabbableObject = null;
            }
        }
    }
}