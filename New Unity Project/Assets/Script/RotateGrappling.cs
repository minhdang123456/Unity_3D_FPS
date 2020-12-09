using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGrappling : MonoBehaviour
{
    private Grappling grappling;
    private Quaternion desiredRotation;
    private float rotationSpeed = 7f;

    private void Awake()
    {
        grappling = GetComponent<Grappling>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!grappling.IsGrappling())
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}
