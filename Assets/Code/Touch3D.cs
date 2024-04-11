using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Touch3D : MonoBehaviour
{
    public LayerMask castLayer;
    private float raycastDist = 50;

    public XRNode handRole = XRNode.LeftHand;
    bool triggerState = false;

    void Update()
    {
        InputDevices.GetDeviceAtXRNode(handRole).TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);

        if (trigger && !triggerState) // on trigger down
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDist, castLayer))
            {
                Transform hitTrans = hit.transform;
                switch (hitTrans.name) //switch is like a more effeciant if else block
                {
                    case "JukeBtn0":
                        hitTrans.parent.GetComponent<Jukebox>().PlaySong(0);
                        break;
                    case "JukeBtn1":
                        hitTrans.parent.GetComponent<Jukebox>().PlaySong(1);
                        break;
                    default:
                        break;
                }
            }
        }
        triggerState = trigger;
    }
}