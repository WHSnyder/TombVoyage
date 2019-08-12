using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = 5 * transform.forward;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
