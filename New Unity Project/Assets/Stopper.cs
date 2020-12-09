using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    private Transform target;

    private void OnTriggerStay(Collider other)
    {
        target = other.transform;
        if(Input.GetKeyDown("s"))
        {
            target.Translate(Vector3.up*Time.deltaTime,Space.World);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        target = other.transform;
        if (Input.GetKeyDown("s"))
        {
            target.Translate(Vector3.up * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
    }
}
