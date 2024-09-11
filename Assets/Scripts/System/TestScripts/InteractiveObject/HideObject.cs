using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{

    public GameObject hideCamera;

    public bool isInteracion;

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void InteractionSend()
    {
        if (!isInteracion)
        {
            InteractionOn();
        }
        else
        {
            InteractionOff();
        }
    }

    public void InteractionOn()
    {
        hideCamera.SetActive(true);
        isInteracion = true;
        Singleton.instance.player.SetActive(false);
    }

    public void InteractionOff()
    {
        hideCamera.SetActive(false);
        isInteracion = false;
        Singleton.instance.player.SetActive(true);
    }
}
