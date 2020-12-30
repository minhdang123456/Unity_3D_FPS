using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerBtn : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject player;    

    private IDoor doorA;
    private Rigidbody rb;
    private void Awake(){
        doorA = door.GetComponent<IDoor>();
        rb=player.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.GetComponent<Rigidbody>()!=null)
        {
            doorA.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.GetComponent<Rigidbody>()!=null)
        {
            doorA.CloseDoor();
        }
    }
}
