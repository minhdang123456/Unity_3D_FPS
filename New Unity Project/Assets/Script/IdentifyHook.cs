using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdentifyHook : MonoBehaviour
{
    GameObject player;

    RaycastHit sphereCastInfo;
    [HideInInspector] public bool found;

    public float maxDistance = 60f;

    [HideInInspector] public Vector3 attachPoint;


    public Image attachPointCrosshair;
    public RectTransform canvasRect;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.FindWithTag("Player");
        attachPointCrosshair.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sphereCastOrigin = new Vector3(player.transform.position.x,player.transform.position.y+1f,player.transform.position.z+1f);
        bool hitSomething = Physics.SphereCast(sphereCastOrigin,2f,transform.forward,out sphereCastInfo,maxDistance);
        if(hitSomething && sphereCastInfo.collider.tag=="Ground" && Vector3.Distance(player.transform.position, sphereCastInfo.point) > 5f)
        {
            found = true;
            attachPointCrosshair.gameObject.SetActive(true);
            attachPoint = sphereCastInfo.point;
            attachPointCrosshair.transform.position = Camera.main.WorldToScreenPoint(attachPoint);
        }
        else
        {
            found = false;
            attachPointCrosshair.gameObject.SetActive(false);
        }
    }
}
