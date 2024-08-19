using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform cam;

    private void Awake()
    {
        if(cam == null)
        {
            cam = GameObject.Find("Main Camera").transform;
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }


}
