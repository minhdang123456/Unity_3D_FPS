using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorAnimation : MonoBehaviour,IElevator
{
    private Animator animator;
   
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Origin()
    {
        animator.SetBool("Origin",true);
    }

    public void Up()
    {
        animator.SetBool("Active",true);
    }
    public void Down()
    {
        animator.SetBool("Active",false);
    }

}
