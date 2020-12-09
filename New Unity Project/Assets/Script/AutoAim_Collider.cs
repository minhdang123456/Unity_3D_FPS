using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim_Collider : MonoBehaviour
{
    public Transform Crosshair;
    private GameObject target;
    private Grappling grappling;

    private void Awake()
    {
        grappling = GetComponent<Grappling>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            target = other.gameObject;
            //  Debug.Log(target.name);
            //   Debug.Log(target.transform.position);
            
         
        }
        else
        {
            //this.Grappling.position = Grappling.position;
        }
    }
}  
