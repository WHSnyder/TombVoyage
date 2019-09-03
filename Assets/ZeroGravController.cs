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


    private bool endSeq;
    public GameObject bloomed;

    Vector3 rightPos = Vector3.zero;

    Vector3 lastPos = Vector3.zero;


    bool holding = false;
    bool apply = false;

    GameObject currObjL = null;
    GameObject currObjR = null;

    bool leftObj = false;
    bool rightObj = false;
    Vector3 currLook = Vector3.zero;


    bool rightHold = false;
    bool leftHold = false;

    void Start(){

        leftHandler = leftHand.GetComponent<SteamVR_Will>();
        rightHandler = rightHand.GetComponent<SteamVR_Will>();
    }

    void Update(){

        if (rightHandler.contact && !currObjR){
            if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand)){
                rightHandler.hold = true;
                //rightHandler.contact = false;
            }
        }
        else if (leftHandler.contact && !currObjL){
            if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand)){
                leftHandler.hold = true;
                // leftHandler.contact = false;
            }
        }

        if (currObjL) {
            Vector3 newUp = leftHand.transform.TransformPoint(currLook);
            newUp = currObjL.transform.InverseTransformPoint(newUp);
            newUp.z = 0;

            Quaternion rot = Quaternion.LookRotation(currObjL.transform.forward, currObjL.transform.TransformDirection(newUp));
            currObjL.transform.rotation = rot;

            leftHand.GetComponent<Collider>().enabled = false;
        }

        if (currObjR){
            Vector3 newUp = rightHand.transform.TransformPoint(currLook);
            newUp = currObjR.transform.InverseTransformPoint(newUp);
            newUp.z = 0;

            Quaternion rot = Quaternion.LookRotation(currObjR.transform.forward, currObjR.transform.TransformDirection(newUp));
            currObjR.transform.rotation = rot;

            rightHand.GetComponent<Collider>().enabled = false;
        }


        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand)){
            rightHandler.hold = false;
        }

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand)){
            leftHandler.hold = false;
        }


        if (currObjR && !SteamVR_Input.GetState("GrabPinch", SteamVR_Input_Sources.RightHand)){
            currObjR.GetComponent<Collider>().enabled = true;
            currObjR = null;
            rightHand.GetComponent<Collider>().enabled = true;
        }

        if (currObjL && !SteamVR_Input.GetState("GrabPinch", SteamVR_Input_Sources.LeftHand)){
            currObjL.GetComponent<Collider>().enabled = true;
            currObjL = null;
            leftHand.GetComponent<Collider>().enabled = true;
        }

        /*if (!leftHold && !rightHold){
            GetComponent<Rigidbody>().isKinematic = false;
        }*/

        lastPos = transform.position;

        if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.LeftHand) && !leftHold){
            GetComponent<Rigidbody>().velocity = 2*leftHand.transform.forward;
        }

        if (SteamVR_Input.GetState("Select", SteamVR_Input_Sources.LeftHand)){
           // transform.position = GameObject.Find("HandleTest").transform.position;
        }

        if (endSeq){
        }


    }

    void OnCollisionEnter(Collision collision){
        Collider coll = collision.contacts[0].thisCollider;
        Collider other = collision.contacts[0].otherCollider;

        if (coll.gameObject.CompareTag("Hand")) {
            if (other.gameObject.CompareTag("Climb")) {
                if (coll.gameObject.name[0] == 'R') {
                    rightHandler.contact = true;
                    rightHandler.contactNormal = collision.GetContact(0).normal;
                }
                else {
                    leftHandler.contact = true;
                    leftHandler.contactNormal = collision.GetContact(0).normal;
                }
            }
            else if (other.gameObject.CompareTag("Handle")) {
                if (coll.gameObject.name[0] == 'R'){
                    currObjR = other.gameObject;
                    currObjR.GetComponent<Collider>().enabled = false;
                    rightObj = true;
                    currLook = rightHand.transform.worldToLocalMatrix * (currObjR.transform.position + currObjR.transform.up);
                    rightHold = true;
                }
                else {
                    currObjL = other.gameObject;
                    currObjL.GetComponent<Collider>().enabled = false;
                    leftObj = true;
                    currLook = leftHand.transform.worldToLocalMatrix * (currObjL.transform.position + currObjL.transform.up);
                    leftHold = true;
                }
            }
        }
    }


    void OnCollisionExit(Collision other){
        Vector3 dummy = Vector3.zero;
        Vector3 handVel = Vector3.zero;
        if (rightHandler.contact){

            rightHandler.pushVel = Vector3.zero;

            //Debug.Log("RIGHT EXIT! Player velocity is now: " + GetComponent<Rigidbody>().velocity);

            rightHold = false;
            rightHandler.hold = false;
            rightHandler.contact = false;
        }
        else if (leftHandler.contact){

            leftHandler.pushVel = Vector3.zero;

            //Debug.Log("LEFT EXIT! Player velocity is now: " + GetComponent<Rigidbody>().velocity);

            leftHold = false;
            leftHandler.hold = false;
            leftHandler.contact = false;
        }
        /*
        else if (leftObj){
            leftObj = false;
            currObj = null;
        }
        else if (rightObj){
            rightObj = false;
            currObj = null;

        }*/
        

    }
}
