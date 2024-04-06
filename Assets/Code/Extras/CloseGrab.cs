using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CloseGrab : MonoBehaviour
{
    public LayerMask grabbableLayer;
    public Transform holdPoint;

    private float launchForce = 10;
    private float grabDist = .15f;

    private Transform grabbableObject = null;
    private Transform heldObject = null;
    private Rigidbody heldRigidbody = null;

    public XRNode handRole = XRNode.LeftHand;
    bool triggerState = false;

    void Update()
    {
        InputDevices.GetDeviceAtXRNode(handRole).TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);

        if (trigger && !triggerState) // on trigger down
        {
            if (heldObject == null)
            {
                CheckForPickUp();
            }
            else
            {
                LaunchObject();
            }
        }

        triggerState = trigger;
    }

    void CheckForPickUp()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, grabDist, grabbableLayer);
        if (hitColliders.Length > 0)
        {
            if (ReferenceEquals(hitColliders[0].transform.parent, null))
            {
                StartCoroutine(PickUpObject(hitColliders[0].transform));
            }

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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, grabDist, grabbableLayer);
        if (hitColliders.Length > 0)
        {
            if (grabbableObject != hitColliders[0].transform)
            {
                if (!ReferenceEquals(grabbableObject, null))
                {
                    grabbableObject.GetComponent<Renderer>().material.color = Color.white;
                }
                grabbableObject = hitColliders[0].transform;
                grabbableObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            if (!ReferenceEquals(grabbableObject, null))
            {
                grabbableObject.GetComponent<Renderer>().material.color = Color.white;
                grabbableObject = null;
            }
        }
    }
}