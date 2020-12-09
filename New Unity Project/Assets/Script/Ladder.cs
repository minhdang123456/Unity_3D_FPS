using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    GameObject playerObject;
    bool ladderClimb=false;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    private void OnTriggerEnter(Collider other1)
    {
        if(other1.gameObject.tag=="Player")
        {
            ladderClimb = true;
            playerObject = other1.gameObject;
        }
    }
    private void OnTriggerExit(Collider other2)
    {
        if (other2.gameObject.tag == "Player")
        {
            ladderClimb = false;
            playerObject = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(ladderClimb)
        {
            if(Input.GetKey("w"))
            {
                playerObject.transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime*speed);
            }
            if(Input.GetKey("s"))
            {
                playerObject.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * speed);
            }
        }
    }
}
