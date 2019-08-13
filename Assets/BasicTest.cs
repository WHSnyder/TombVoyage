using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BasicTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftHand, rightHand;

    public SteamVR_Will leftHandler;
    public SteamVR_Will rightHandler;


    Vector3 rightPos = Vector3.zero;


    bool holding = false;
    bool apply = false;


    bool rightHold = false;
    bool leftHold = false;


    void Start()
    {
        //leftHand = transform.Find("Player/SteamVRObjects/LeftHand").gameObject;
        // rightHand = transform.Find("Player/SteamVRObjects/RightHand").gameObject;
        //GetComponent<Rigidbody>().velocity = Vector3.forward;

        leftHandler = leftHand.GetComponent<SteamVR_Will>();
        rightHandler = rightHand.GetComponent<SteamVR_Will>();

    }

    // Update is called once per frame
    void Update(){

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand)){
            rightHold = false;
            rightHandler.contact = false;
        }

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand))
        {
            leftHold = false;
            leftHandler.contact = false;
        }

        if (!leftHold && !rightHold)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider coll = collision.contacts[0].thisCollider;
        Collider other = collision.contacts[0].otherCollider;
        
        if (other.gameObject.CompareTag("Climb") && coll.gameObject.CompareTag("Hand"))
        {
            if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand) && coll.gameObject.name[0] == 'R')
            {
                rightHold = true;
                GetComponent<Rigidbody>().isKinematic = true;
                rightHandler.contact = true;
            }

            if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand))
            {
                leftHold = true;
                GetComponent<Rigidbody>().isKinematic = true;
                leftHandler.contact = true;
            }
        }
    }
}
