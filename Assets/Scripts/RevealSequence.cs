using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealSequence : MonoBehaviour
{

    GameObject[] guards;
    GameObject[] roids;

    bool slowDown = false;

    // Start is called before the first frame update
    void Start(){
        guards = GameObject.FindGameObjectsWithTag("GuardRocks");

        int i = 0;

        foreach (GameObject guard in guards){
            guard.GetComponent<Rigidbody>().detectCollisions = false;
            Debug.Log("tracking " + i + "th guard");
        }

        roids = GameObject.FindGameObjectsWithTag("Roid");

        foreach (GameObject roid in roids){
            roid.GetComponent<Rigidbody>().AddTorque(new Vector3(20 * Random.value, 20 * Random.value, 20 * Random.value));
        }

        GameObject testRoid = GameObject.Find("TestRoid");
        testRoid.GetComponent<Rigidbody>().velocity = -1 * testRoid.transform.forward;

    }


    void Update()
    {
        if (slowDown)
        {

            foreach (GameObject guard in guards)
            {
                guard.GetComponent<Rigidbody>().velocity /= 5;
            }



            slowDown = false;
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Roid"))
        {

            Debug.Log("sequence activated...");

            foreach (GameObject guard in guards)
            {
                guard.GetComponent<Rigidbody>().detectCollisions = true;
            }

            slowDown = true;
        }
    }
}
