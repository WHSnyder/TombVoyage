using System.Collections;
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
    private GameObject player = null;
    private GameObject ship = null;

    private FixedJoint joint = null;

    private MyInter curr = null;

    private List<MyInter> contacts = new List<MyInter>();

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
            //fix this workaround soon...
            if (!gameObject.name.Contains("Left")){

                GameObject coll = updatePointer();

                pointerSphere.SetActive(coll != null); 
                
                if (coll != null && SteamVR_Input.GetState("Select", SteamVR_Input_Sources.RightHand))
                {
                    
                    player.transform.position = coll.transform.position - ship.transform.up;
                    player.transform.rotation = Quaternion.LookRotation(-1 * coll.transform.right, ship.transform.up);
                    
                }
            }
        }
        else
        {
            pointerSphere.SetActive(false);
        }



        if (driving)
        {
            Vector3 difference = joystick.transform.position - transform.position;


            if (Vector3.Magnitude(difference) < 1)
            {
                
                flip = !flip;

                if (flip)
                {
                    facepalm = facepalm * -1;
                }

                Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.forward, difference), -1*difference);
                //print("Joyright: " + facepalm*joystick.transform.right);
                
                joystick.transform.rotation = rot;

                //print(rot.ToString());

            }
            else
            {
                driving = false;
            }
        }
    }

    private GameObject updatePointer()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward + (-1 * transform.up), out hit,Mathf.Infinity, Physics.DefaultRaycastLayers))
        {
            pointerSphere.transform.position = hit.point;
            //print("Cast success");
            return hit.collider.gameObject;
        }
        
        //print("Cast failed");
        return null;
    }



    private void Awake(){
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();

        pointerSphere = GameObject.Find("PR_Sphere");
        joystick = GameObject.Find("JoyStick");
        player = GameObject.Find("Player");
        ship = GameObject.Find("ship_test");
    }

    

    private void OnTriggerStay(Collider other) {
        //print("triggered");
        if (!holding && other.gameObject.CompareTag("Interactable")){
            if (grab.GetState(pose.inputSource)){
                curr = GameObject.FindGameObjectWithTag("Interactable").GetComponent<MyInter>();
                Pickup();
            }
        }

        else if (!driving && other.gameObject.CompareTag("Joy")){
            if (grab.GetState(pose.inputSource)){
                driving = true;
            }
        }
    }



    public void Pickup() {
        //print("picked");

        holding = true;

        curr.transform.position = transform.position;

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = false;
        joint.connectedBody = targ;

        curr.curHand1 = this;
    }
  

    public void Drop(){

        if (!curr) { return; }

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = true;
        joint.connectedBody = null;
        curr = null;
    }
}
