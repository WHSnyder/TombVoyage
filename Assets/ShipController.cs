using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    Rigidbody rb;

    GameObject joystick = null;

    Vector3 joy_rest = Vector3.up;
    Vector3 joy_curr = Vector3.zero;

    Vector3 ang_vel;
    Vector3 speed;

    Quaternion rot;

    float side_vel, up_vel;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void Awake()
    {
        joystick = GameObject.Find("JoyStick");
    }

    // Update is called once per frame
    void Update()
    {
        //rot = Quaternion.LookRotation(-1*joystick.transform.right, joystick.transform.up);
        //print("Joyright: " + facepalm*joystick.transform.right);
        //this.transform.rotation = rot;
        //print(rot);

        joy_curr = joystick.transform.up;

        float side_bet = Vector3.Angle(this.transform.forward, joy_curr);
        float up_bet = Vector3.Angle(this.transform.right, joy_curr);
        //print(between);

        //print("Up angle: " + up_bet + " Side angle: " + side_bet);

        side_vel = Mathf.Deg2Rad * (90 - side_bet);
        up_vel = Mathf.Deg2Rad * (90 - up_bet); 

        ang_vel = new Vector3(side_vel, 0, up_vel);
        //print(vel * Mathf.Rad2Deg);

    }

    private void FixedUpdate()
    {


        rb.centerOfMass = Vector3.zero;

        rb.angularVelocity = rb.transform.right*side_vel + rb.transform.forward*-1*up_vel;

        //print("Rot speed: " + rb.angularVelocity.ToString());

        rb.velocity = transform.right * -5;
    }


    public void fire(Vector3 target)
    {

    }
}
