using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    private SphereCollider collider;
    public LayerMask LayerMask;
    public Transform aimPosition;
    public GameObject currentGun;
    GameObject currentTarget;
    public float distance = 10f;

    bool IsAiming;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentGun.transform.position = aimPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTarget();
        if (IsAiming) AutoAiming();
    }
    private void CheckTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward, out hit, distance))
        {
            if(hit.transform.gameObject.tag=="Ground")
            {
                if(!IsAiming)
                {
                    Debug.Log("Target Acquired");
                }
                currentTarget = hit.transform.gameObject;
                IsAiming = true;
            }
            else
            {
               
                currentTarget = null;
                IsAiming = false;
            }
        }
    }

    public void AutoAiming()
    {
        if (collider.radius < distance)
        {
            currentGun.transform.LookAt(currentTarget.transform);
        }
        else
        {
            currentGun.transform.position = aimPosition.position;
        }
    }

}
