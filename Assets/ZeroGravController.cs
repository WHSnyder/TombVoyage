using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ZeroGravController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftHand, rightHand;

    public SteamVR_Will leftHandler;
    public SteamVR_Will rightHandler;


    public GameObject caveDest;

    private Vector3 delayedVel = Vector3.zero;


    Vector3 rightPos = Vector3.zero;

    Vector3 lastPos = Vector3.zero;


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

        GetComponent<Rigidbody>().velocity = transform.forward;

    }

    // Update is called once per frame
    void Update()
    {

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand))
        {
            rightHold = false;
            rightHandler.hold = false;
            //rightHandler.contact = false;
        }

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand))
        {
            leftHold = false;
            leftHandler.hold = false;
            // leftHandler.contact = false;
        }

        if (!leftHold && !rightHold)
        {
            //GetComponent<Rigidbody>().isKinematic = false;
        }

        lastPos = transform.position;

        //Debug.Log("Player vel: " + GetComponent<Rigidbody>().velocity);

        if (SteamVR_Input.GetState("Select", SteamVR_Input_Sources.LeftHand))
        {
            transform.position = caveDest.transform.position;
            GetComponent<Rigidbody>().velocity = .3f * caveDest.transform.right;
        }
    }

    void OnCollisionEnter(Collision collision){
        Collider coll = collision.contacts[0].thisCollider;
        Collider other = collision.contacts[0].otherCollider;

        Debug.Log(coll.gameObject.name + " collided at normal: " + collision.GetContact(0).normal.ToString());

        if (other.gameObject.CompareTag("Climb") && coll.gameObject.CompareTag("Hand")){
            if (coll.gameObject.name[0] == 'R'){
                Debug.Log("right hand collision,...");

                rightHandler.contact = true;
                rightHandler.contactNormal = collision.GetContact(0).normal;
            }
            else{
                Debug.Log("left hand collision,...");

                leftHandler.contact = true;
                leftHandler.contactNormal = collision.GetContact(0).normal;
            }
        }
    }


    void OnCollisionExit(Collision other){
        Vector3 dummy = Vector3.zero;
        Vector3 handVel = Vector3.zero;
        if (rightHandler.contact){

            rightHandler.pushVel = Vector3.zero;
            rightHandler.GetEstimatedPeakVelocities(out handVel , out dummy);
            GetComponent<Rigidbody>().velocity = -1 * handVel;

            Debug.Log("right hand exit...");

            rightHold = false;
            rightHandler.hold = false;
            rightHandler.contact = false;
        }
        else if (leftHandler.contact){

            leftHandler.pushVel = Vector3.zero;
            leftHandler.GetEstimatedPeakVelocities(out handVel, out dummy);
            GetComponent<Rigidbody>().velocity = -1 * handVel;

            Debug.Log("left hand exit...");

            leftHold = false;
            leftHandler.hold = false;
            leftHandler.contact = false;
        }       
    }
}
