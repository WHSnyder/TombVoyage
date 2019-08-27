﻿using System.Collections;
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

        GetComponent<Rigidbody>().velocity = 1 * transform.forward;

    }

    // Update is called once per frame
    void Update(){

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand)){
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
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider coll = collision.contacts[0].thisCollider;
        Collider other = collision.contacts[0].otherCollider;

        Debug.Log(coll.gameObject.name + " collided at normal: " + collision.GetContact(0).normal.ToString());
        
        if (other.gameObject.CompareTag("Climb") && coll.gameObject.CompareTag("Hand")){
            //if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.Any)){
            if (coll.gameObject.name[0] == 'R'){
                //rightHold = true;
                //GetComponent<Rigidbody>().isKinematic = true;
                //rightHandler.hold = true;

                Debug.Log("right hand collision,...");

                rightHandler.contact = true;
                rightHandler.contactNormal = collision.GetContact(0).normal;
            }
            else{
                //leftHold = true;
                // GetComponent<Rigidbody>().isKinematic = true;
                //leftHandler.hold = true;

                leftHandler.contact = true;
                leftHandler.contactNormal =  collision.GetContact(0).normal;
            }             
            //}
        }
    }


    void OnCollisionExit(Collision other){

        //if (other.gameObject.CompareTag("Climb") && coll.gameObject.CompareTag("Hand"))
        //{
        //if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.Any)){
        if (rightHandler.contact){
            //GetComponent<Rigidbody>().velocity = rightHandler.pushVel;
            rightHandler.pushVel = Vector3.zero;

            Debug.Log("right hand exit...");

        }
        else if (leftHandler.contact){
           // GetComponent<Rigidbody>().velocity = leftHandler.pushVel;
            leftHandler.pushVel = Vector3.zero;

            Debug.Log("right hand exit...");
        }
        //}
        //}
        rightHold = false;
        rightHandler.hold = false;
        rightHandler.contact = false;

        leftHold = false;
        leftHandler.hold = false;
        leftHandler.contact = false;
    }
}