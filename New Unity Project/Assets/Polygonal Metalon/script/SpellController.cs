using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject player;
    public float lifeTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeTime>0){
            lifeTime-=Time.deltaTime;
        }
        else{
            enabled=false;
            Destroy(gameObject,0.5f);    
        }
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.name == "Player")
        {
            enabled=false;
            Destroy(gameObject,0.5f);
        }
        else
        {
            enabled=false;
            Destroy(gameObject,1f);
        }
        
    }
}
