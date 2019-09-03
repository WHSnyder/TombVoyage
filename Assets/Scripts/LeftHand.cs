using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LeftHand : MonoBehaviour{

    public SteamVR_Action_Boolean leftSelect = null;
    public SteamVR_Action_Boolean grab = null;
    public SteamVR_Action_Boolean pull = null;
    private SteamVR_Behaviour_Pose pose = null;
    public SteamVR_Action_Boolean grip = null;

    public GameObject socket;
    public GameObject monument;
    public GameObject faceCollider;

    private GameObject pointerSphere;
    private GameObject joystick = null;
    private GameObject player = null;
    private GameObject ship = null;
    private GameObject throttle = null;

    public GameObject rightHand;

    private GameObject asteroid = null;
    private Rigidbody roidrb = null;

    private FixedJoint joint = null;

    private Transform pointer;

    /*
    private MyInter curr = null;

    private List<MyInter> contacts = new List<MyInter>();
    

    public MyHand other;
    */

    float facepalm = -1;
    bool flip = false;

    private bool holding = false;
    private bool driving = false;
    private bool pulling = false;
    private bool flooring = false;

    public Transform vrCam;


    private GameObject reach;
    private Reach reachHandle;
    private bool reachLinked = false;


    private Vector3 lastPosition = Vector3.zero;

    private bool eject = false;

    private void Start(){
        player.transform.localPosition = new Vector3(-0.03362f, -0.00162f, -0.00385f);
        player.transform.localRotation = Quaternion.Euler(-85.175f, -90.00001f, -90.00001f);
    }


    // Update is called once per frame
    void Update(){

        pointer = transform.Find("LeftRenderModel(Clone)/vr_glove_left(Clone)/vr_glove_model/Root/wrist_r/finger_index_meta_r/finger_index_0_r/finger_index_1_r/");

        if (pointer == null){
            reachLinked = false;
            pulling = false;
        }

        if (!reachLinked && pointer){
            linkReach();
        }

        if (grab.GetStateUp(pose.inputSource)){
            Drop();
            holding = false;
        }

        if (leftSelect.GetStateDown(pose.inputSource)){

            if (eject)
            {
                changeToFree();
            }
            else
            {
                //player.transform.localPosition = new Vector3(-0.029f, -0.0003f, -0.0105f);
                eject = true;
            }
        }


        Vector3 dif = Vector3.zero;
        Vector3 towards = pointer.right * -1; 



        /*
         * 
         *   Asteroid physics logic - need to refine..
         *   
         */

        if (reachLinked && pull.GetState(pose.inputSource) && !pulling){

            if (reachHandle.collided){

                asteroid = reachHandle.collided;

                if (asteroid.CompareTag("Roid")){
                    pulling = true;
                    roidrb = asteroid.GetComponent<Rigidbody>();
                    roidrb.angularVelocity = new Vector3(2, 2, 2);
                }
                else {

                    asteroid = null;
                    pulling = false;
                }
            }
        }
        else {
            if (!pull.GetState(pose.inputSource)){
                if (pulling){

                    Vector3 handVel, nada;

                    pose.GetEstimatedPeakVelocities(out handVel, out nada);

                    handVel = player.transform.TransformDirection(handVel);

                    Debug.Log("Hand vel: " + handVel );

                    //if (Vector3.Magnitude(handVel) > 1){

                        roidrb.AddForce(3.0f * handVel, ForceMode.VelocityChange);
                    //}
                    
                    roidrb = null;
                }
                pulling = false;
            }
        }

        if (pulling && pointer){

            dif = this.transform.position - asteroid.transform.position;

            if (grip.GetState(pose.inputSource)){

                if (Vector3.Magnitude(dif) > 15.0f){
                    roidrb.velocity = Vector3.Normalize(dif)/10;
                }
            }
        }



        /*
         
         Steering Logic
         
         
        if (driving){
            if (grab.GetState(pose.inputSource)){
                Vector3 difference = joystick.transform.position - transform.position;

                if (Vector3.Magnitude(difference) < 1){

                    Quaternion rot = Quaternion.LookRotation(-1 * difference, Vector3.Cross(Vector3.right, difference));
                    joystick.transform.rotation = rot;
                }
                else {
                    driving = false;
                    joystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else{
                driving = false;
                joystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        */


        /*
         
         Accel Logic
         
         */
        if (flooring){

            if (grab.GetState(pose.inputSource)){

                Vector3 difference = transform.position - throttle.transform.position;

                if (Vector3.Magnitude(difference) < 2){

                    difference = throttle.transform.worldToLocalMatrix * difference;

                    float yrot = throttle.transform.localRotation.eulerAngles.y;

                    //if (yrot > -40 && yrot < 60) {
                    Vector3 lookAt = throttle.transform.localToWorldMatrix * new Vector3(difference.x, 0, difference.z);

                    Quaternion rot = Quaternion.LookRotation(lookAt, throttle.transform.up);
                    throttle.transform.rotation = rot;
                    //}
                }
                else{
                    flooring = false;
                }
            }
            else{
                flooring = false;
            }
        }
        lastPosition = transform.position;
    }





    private void Awake(){
        reach = GameObject.Find("reach");
        reachHandle = reach.GetComponent<Reach>();

        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();

        pointerSphere = GameObject.Find("PR_Sphere");
        joystick = GameObject.Find("JoyStick");
        throttle = GameObject.Find("Throttle");
        player = GameObject.Find("Player");
        ship = GameObject.Find("ship");

        pointer = transform.Find("LeftRenderModel(Clone)/vr_glove_left(Clone)/vr_glove_model/Root/wrist_r/finger_inex_meta_r/finger_index_0_r/finger_index_1_r/");
    }



    private void OnTriggerStay(Collider other){
        /*if (!holding && other.gameObject.CompareTag("Interactable")){
            if (grab.GetState(pose.inputSource)){
                curr = GameObject.FindGameObjectWithTag("Interactable").GetComponent<MyInter>();
                Pickup();
            }
        }*/
        if (!flooring && other.gameObject.CompareTag("Throttle")){
            if (grab.GetState(pose.inputSource)){
                flooring = true;
            }
        }
    }



    public void Pickup()
    {
        /*print("picked");

        holding = true;

        curr.transform.position = transform.position;

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = false;
        joint.connectedBody = targ;

        curr.curHand1 = this;*/
    }


    public void Drop()
    {
        /*
        if (!curr) { return; }

        Rigidbody targ = curr.GetComponent<Rigidbody>();
        targ.isKinematic = true;
        joint.connectedBody = null;
        curr = null;*/
    }


    public void linkReach()
    {
        reach.transform.position = pointer.transform.position;

        Quaternion rot = Quaternion.LookRotation(pointer.right * -1, pointer.forward);

        reach.transform.rotation = rot;
        reachLinked = true;

        reach.transform.SetParent(transform);
        reach.GetComponent<MeshRenderer>().enabled = false;
    }


    void changeToFree(){

        faceCollider.GetComponent<MeshCollider>().enabled = false;
        monument.GetComponent<SphereCollider>().enabled = false;

        freeHand(this.gameObject);
        freeHand(rightHand);
        
        player.transform.SetParent(null);
        Rigidbody rb = player.AddComponent( typeof(Rigidbody) ) as Rigidbody;
        rb.isKinematic = false;
        rb.useGravity = false;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        player.GetComponent<ZeroGravController>().enabled = true;

        player.GetComponent<Rigidbody>().velocity =  (socket.transform.position - player.transform.position) / 10;
        ship.GetComponent<AudioSource>().enabled = false;

        reach.gameObject.SetActive(false);

        enabled = false;
        rightHand.GetComponent<MyHand>().enabled = false;
    }

    private void freeHand(GameObject hand)
    {
        hand.GetComponent<SteamVR_Will>().enabled = true;
        hand.GetComponent<SteamVR_Behaviour_Pose>().enabled = false;
        hand.GetComponent<SphereCollider>().isTrigger = false;
        Destroy(hand.GetComponent<Rigidbody>());
    }
}