using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAttack : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject stab;
    public GameObject appearPoint;
    public GameObject Spell;
    public float upwardForce;
    public float forwardForce;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void activateAtk()
    {
        stab.GetComponent<SphereCollider>().enabled = true;
    }

    public void deactivateAtk()
    {
        stab.GetComponent<SphereCollider>().enabled = false;
    }
    public void SpellSpaw()
    {
        Rigidbody rb = Instantiate(Spell,appearPoint.transform.position,Quaternion.identity).GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upwardForce, ForceMode.Impulse);
    }
}
