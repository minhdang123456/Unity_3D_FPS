using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerBtn : MonoBehaviour
{
    [SerializeField] private DoorAnimation door;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            door.OpenDoor();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            door.CloseDoor();
        }
    }
}
