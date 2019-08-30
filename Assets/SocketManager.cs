using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketManager : MonoBehaviour
{

    public GameObject otherSocket;
    public GameObject colliders;

    private bool relaxed = true;

    /*
    void OnCollisionEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherSocket.GetComponent<SocketManager>().relax();
        }
    }
    */

    public void relax(){
        colliders.SetActive(false);
    }
}
