using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    Rigidbody rb;

    GameObject joystick = null;
    GameObject throttle = null;

    Vector3 joy_rest = Vector3.up;
    Vector3 joy_curr = Vector3.zero;

    Vector3 ang_vel;
    Vector3 speed;

    Quaternion rot;

    AudioSource engine;


    float side_vel, up_vel;


    void Start(){
        rb = GetComponent<Rigidbody>();
        engine = GetComponent<AudioSource>();
    }

    void Awake(){
        joystick = GameObject.Find("/ship/StickRoot/JoyStick");
        throttle = GameObject.Find("/ship/ThrottleRoot/Throttle");
    }

    // Update is called once per frame
    void Update(){

        joy_curr = joystick.transform.forward;

        float side_bet = Vector3.Angle(-1 * this.transform.up, joy_curr);
        float up_bet = Vector3.Angle(this.transform.right, joy_curr);

        side_vel = Mathf.Deg2Rad * (90 - side_bet);
        up_vel = Mathf.Deg2Rad * (90 - up_bet); 

        ang_vel = new Vector3(side_vel, 0, up_vel);





    }

    private void FixedUpdate(){

        rb.centerOfMass = Vector3.zero;
        rb.angularVelocity = rb.transform.right*side_vel + rb.transform.up*up_vel;



        float degs = throttle.transform.localRotation.eulerAngles.y;

        Debug.Log("throttle rot: " + degs);

        
        if (degs > 200)
        {
            degs -= 360;
        }
        
        degs *= -1;
        

        degs += 60.0f;


        degs = Mathf.Clamp(degs, 0.0f, 100.0f) / 100.0f;
        


        engine.pitch = .9f + .4f*degs;
        rb.velocity = transform.right * (-5 - 6 * degs);
    }
}
