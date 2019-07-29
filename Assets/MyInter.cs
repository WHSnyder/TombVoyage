using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInter : MonoBehaviour
{

    public MyHand curHand1, curHand2;

    public Vector3 originalScale;

    public GameObject obj;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        obj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
