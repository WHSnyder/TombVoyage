using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LeftHand : MonoBehaviour
{
    public SteamVR_Action_Boolean leftSelect = null;
    public SteamVR_Action_Boolean grab = null;
    public SteamVR_Action_Boolean pull = null;
    private SteamVR_Behaviour_Pose pose = null;
    public SteamVR_Action_Boolean grip = null;

    private GameObject pointerSphere = null;
    private GameObject joystick = null;
    private GameObject player = null;
    private GameObject ship = null;
    private GameObject gun = null;

    public GameObject rightHand;

    private GameObject asteroid = null;
    private Rigidbody roidrb = null;

    private FixedJoint joint = null;

    private Transform pointer;

    private MyInter curr = null;

    private List<MyInter> contacts = new List<MyInter>();

    //public MyHand other;

    float facepalm = -1;
    bool flip = false;

    private bool holding = false;
    private bool driving = false;
    private bool pulling = false;

    public Transform vrCam;


    private GameObject reach;
    private Reach reachHandle;
    private bool reachLinked = false;


    private Vector3 lastPosition = Vector3.zero;


    // Update is called once per frame
    void Update(){

        pointer = transform.Find("LeftRenderModel(Clone)/vr_glove_left(Clone)/vr_glove_model/Root/wrist_r/finger_index_meta_r/finger_index_0_r/finger_index_1_r/");

        if (pointer == null){
            reachLinked = false;
            pulling = false;
        }

        if (!reachLinked && pointer){
            print("reaching");
            linkReach();
        }

        if (grab.GetStateUp(pose.inputSource)){
            Drop();
            holding = false;
            driving = false;
        }

        if (leftSelect.GetState(pose.inputSource))
        {
            changeToFree();
        }


        Vector3 dif = Vector3.zero;
        Vector3 towards = pointer.right * -1; 

        if (reachLinked && pull.GetState(pose.inputSource) && !pulling){

            if (reachHandle.collided){

                asteroid = reachHandle.collided;
                if (!asteroid.name.Contains("Cu")){

                    asteroid = null;
                    pulling = false;
                }
                else {
                    
                    pulling = true;
                    roidrb = asteroid.GetComponent<Rigidbody>();
                }
            }
        }
        else {
            if (!pull.GetState(pose.inputSource)){
                if (pulling){

                    /* Vector3 dest = transform.position + 100 * reach.transform.forward;

                     roidrb.velocity = .5f * (dest - roidrb.gameObject.transform.position);*/

                    Vector3 handVel, nada;

                    pose.GetEstimatedPeakVelocities(out handVel, out nada);
                    RaycastHit hit;

                    //if (Physics.Raycast(vrCam.position, vrCam.forward, out hit, 1000, 1 << 8))
                    //{
                        Debug.Log("Hand Speed: " + Vector3.Magnitude( handVel ));
                        roidrb.velocity = Vector3.Magnitude( handVel ) * 10 * Vector3.Normalize(reach.GetComponent<Reach>().collided.transform.position - roidrb.transform.position);
                   /* }
                    else
                    {
                        Debug.Log("no hit found....");
                        roidrb.velocity = handVel;
                    }*/




                    roidrb = null;
                }
                pulling = false;
            }
        }

        if (pulling && pointer){

            dif = this.transform.position - asteroid.transform.position;

            

            if (grip.GetState(pose.inputSource)){
                roidrb.velocity = 8.0f * Vector3.Normalize(dif);
            }
            else {

                //
            }
        }





        if (driving){

            Vector3 difference = joystick.transform.position - transform.position;

            if (Vector3.Magnitude(difference) < 1){

                flip = !flip;

                if (flip)
                {
                    facepalm = facepalm * -1;
                }

                Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.forward, difference), -1 * difference);
                //print("Joyright: " + facepalm*joystick.transform.right);

                joystick.transform.rotation = rot;

                //print(rot.ToString());

            }
            else{
                driving = false;
            }
        }

        lastPosition = transform.position;
    }

    

    private void Awake()
    {
        reach = GameObject.Find("reach");
        reachHandle = reach.GetComponent<Reach>();

        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();

        pointerSphere = GameObject.Find("PR_Sphere");
        joystick = GameObject.Find("JoyStick");
        player = GameObject.Find("Player");
        ship = GameObject.Find("ship_test");

        gun = GameObject.Find("gun");

        pointer = transform.Find("LeftRenderModel(Clone)/vr_glove_left(Clone)/vr_glove_model/Root/wrist_r/finger_inex_meta_r/finger_index_0_r/finger_index_1_r/");


    }



    private void OnTriggerStay(Collider other)
    {
        
        //print("triggered");
        if (!holding && other.gameObject.CompareTag("Interactable"))
        {
            if (grab.GetState(pose.inputSource))
            {
                curr = GameObject.FindGameObjectWithTag("Interactable").GetComponent<MyInter>();
                Pickup();
            }
        }

        else if (!driving && other.gameObject.CompareTag("Joy"))
        {
            if (grab.GetState(pose.inputSource))
            {
                driving = true;
            }
        }
    }



    public void Pickup()
    {
        //print("picked");

        holding = true;

        curr.transform.position = transform.position;

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = false;
        joint.connectedBody = targ;

        //curr.curHand1 = this;
    }


    public void Drop()
    {

        if (!curr) { return; }

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = true;
        joint.connectedBody = null;
        curr = null;
    }


    public void linkReach()
    {
        reach.transform.position = pointer.transform.position;

        Quaternion rot = Quaternion.LookRotation(pointer.right * -1, pointer.forward);

        reach.transform.rotation = rot;
        reachLinked = true;

        reach.transform.SetParent(pointer);
        //reach.GetComponent<MeshRenderer>().enabled = false;
    }


    void changeToFree()
    {
        GetComponent<SteamVR_Will>().enabled = true;
        GetComponent<SteamVR_Behaviour_Pose>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        rightHand.GetComponent<SteamVR_Will>().enabled = true;
        rightHand.GetComponent<SteamVR_Behaviour_Pose>().enabled = false;
        rightHand.GetComponent<Rigidbody>().isKinematic = true;

        player.transform.SetParent(null);

        player.GetComponent<BoxCollider>().enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<BasicTest>().enabled = true;

        player.GetComponent<Rigidbody>().velocity = 2 * ship.GetComponent<Rigidbody>().velocity;
    }
}
