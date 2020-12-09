using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
   public GameObject elevatorPanel;

    enum States { up, down, pause };
    States states;


    public Transform OriginSpot;
    public Transform Destination;

    public float smooth;
    Vector3 newPos;

    bool hasRider;
    // Start is called before the first frame update
    void Start()
    {
        elevatorPanel.SetActive(false);
        states = States.pause;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && hasRider)
        {
            states = States.up;
        }
        if (Input.GetKeyDown(KeyCode.R) && hasRider)
        {
            states = States.down;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            elevatorPanel.SetActive(true);
            other.transform.parent = gameObject.transform;
            hasRider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            elevatorPanel.SetActive(false);
            other.transform.parent = null;
            hasRider = false;
        }
    }
    void FMS()
    {
        if(states == States.down)
        {
            newPos = Destination.position;
            transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
        }
        if (states == States.up)
        {
            newPos = OriginSpot.position;
            transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
        }
        if (states == States.pause)
        {
            
        }
    }
}
