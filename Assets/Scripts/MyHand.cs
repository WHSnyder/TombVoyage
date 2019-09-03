﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MyHand : MonoBehaviour
{
    public SteamVR_Action_Boolean grab = null;
    private SteamVR_Behaviour_Pose pose = null;

    private GameObject pointerSphere = null;
    private GameObject joystick = null;
    public GameObject player = null;
    private GameObject ship = null;

    private FixedJoint joint = null;

    public MyHand other;

    float facepalm = -1;
    bool flip = false;

    private bool holding = false;
    private bool driving = false;


    // Update is called once per frame
    void Update(){        

        if (grab.GetStateUp(pose.inputSource)){
            Drop();
            holding = false;
            driving = false;
        }
        
        if (!SteamVR_Input.GetState("Point", SteamVR_Input_Sources.RightHand) && !SteamVR_Input.GetState("GrabPinch", SteamVR_Input_Sources.RightHand)){

            GameObject coll = updatePointer();

            pointerSphere.SetActive(coll != null); 
                
            if (coll != null && SteamVR_Input.GetState("Select", SteamVR_Input_Sources.RightHand)){
                player.transform.rotation = Quaternion.LookRotation(-1 * coll.transform.right, ship.transform.forward);
            }
        }
        else { 
            pointerSphere.SetActive(false);
        }


        /*
         
         JoyStick control... perhaps checkout upgrading to circular drive with to axes..
         
         */
        if (driving){
            if (grab.GetState(pose.inputSource)){
                Vector3 difference = joystick.transform.position - transform.position;

                if (Vector3.Magnitude(difference) < 1){
                    Quaternion rot = Quaternion.LookRotation(-1 * difference, Vector3.Cross(Vector3.right, difference));
                    joystick.transform.rotation = rot;
                }
                else{
                    driving = false;
                    joystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else {
                driving = false;
                joystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }



    private GameObject updatePointer(){
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward + (-1 * transform.up), out hit,Mathf.Infinity, Physics.DefaultRaycastLayers)){
            pointerSphere.transform.position = hit.point;
            return hit.collider.gameObject;
        }
        return null;
    }



    private void Awake(){
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();

        pointerSphere = GameObject.Find("PR_Sphere");
        joystick = GameObject.Find("/ship/StickRoot/JoyStick");
        //player = GameObject.Find("Player");
        ship = GameObject.Find("ship");
    }

    

    private void OnTriggerStay(Collider other) {
        /*print("triggered");
        if (!holding && other.gameObject.CompareTag("Interactable")){
            if (grab.GetState(pose.inputSource)){
                curr = GameObject.FindGameObjectWithTag("Interactable").GetComponent<MyInter>();
                Pickup();
            }
        }*/

        if (!driving && other.gameObject.CompareTag("Joy")){
            if (grab.GetState(pose.inputSource)){
                //GetComponent<SteamVR_Skeleton_Poser>().GetBlendedPose();
                driving = true;
            }
        }
    }



    public void Pickup() {
        /*print("picked");

        holding = true;

        curr.transform.position = transform.position;

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = false;
        joint.connectedBody = targ;

        curr.curHand1 = this;*/
    }
  

    public void Drop(){
        /*
        if (!curr) { return; }

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = true;
        joint.connectedBody = null;
        curr = null;*/
    }
}