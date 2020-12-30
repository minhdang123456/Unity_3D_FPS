using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject elevator;
    [SerializeField] private GameObject player;

    private IElevator IE;
    private Rigidbody rb;

    private void Awake(){
        IE = elevator.GetComponent<IElevator>();
        rb = player.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.GetComponent<Rigidbody>()!=null)
        {
            IE.Up();
            
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.GetComponent<Rigidbody>()!=null)
        {
            IE.Down();
        }
    }
}
