using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    public Collider[] arrColliders;
    public Transform crosshair;
    public Transform player;
    public LayerMask Layer;
    public float Radius; 
    private float maxDistance = 10f;
    private void FixedUpdate()
    {
        CheckClosest();
    }

    private void CheckClosest()
    {
        RaycastHit hit;
        if(Physics.Raycast(crosshair.position,transform.forward,out hit,Layer))
        {
            Physics.OverlapSphere(player.position,Radius);
        }
    }

}
