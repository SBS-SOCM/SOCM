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
        CharacterManager.instance.gameObject.SetActive(false);
    }

    public void InteractionOff()
    {
        hideCamera.SetActive(false);
        isInteracion = false;
        CharacterManager.instance.gameObject.SetActive(true);
    }
}
