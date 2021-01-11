using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject player;
    public GameObject breakable;
    public TimeManager time;
    private float breakForce = 2.5f;
    private Rigidbody rb;
    private void Awake()
    {
        rb = player.GetComponent<Rigidbody>();
    }

   private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent< Rigidbody >()!= null) {
            Instantiate(breakable, transform.position,breakable.transform.rotation);
            Destroy(gameObject);
            time.SlowTime();
        }
        
 
    }

   
}
