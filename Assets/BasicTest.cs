using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BasicTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftHand, rightHand;

    Vector3 rightPos = Vector3.zero;


    bool holding = false;


    void Start()
    {
        //leftHand = transform.Find("Player/SteamVRObjects/LeftHand").gameObject;
        // rightHand = transform.Find("Player/SteamVRObjects/RightHand").gameObject;
        GetComponent<Rigidbody>().AddForce(Vector3.forward, ForceMode.Acceleration);//  ..velocity = Vector3.forward;
    }

    // Update is called once per frame
    void Update(){

        if (!SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand)){
           // holding = false;
           // GetComponent<Rigidbody>().isKinematic = false;
        }


        if (holding){

            //rightHand.transform.position = Vector3.zero;

            transform.position += rightHand.transform.position - rightPos;

            rightHand.transform.position = rightPos;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Collider coll = collision.contacts[0].thisCollider;
        //Collider other = collision.contacts[0].otherCollider;
        
       /* if (other.gameObject.CompareTag("Climb") && !holding)// && coll.gameObject.CompareTag("Hand"))
        {
            if (SteamVR_Input.GetState("GrabGrip", SteamVR_Input_Sources.RightHand))
            {
                holding = true;
                rightPos = rightHand.transform.position;

                Debug.Log("rightHand Position: " + rightPos.ToString());

                GetComponent<Rigidbody>().isKinematic = true;
            }
        }*/

    }
}
