using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] private ElevatorAnimation elevator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            elevator.Up();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            elevator.Down();
        }
    }
}
