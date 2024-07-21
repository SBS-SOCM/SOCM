using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrapButtonTest : MonoBehaviour
{
    public GameObject[] traps;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider != null)
        {
            Debug.Log(collider.name);
        }

        if (collider.gameObject.name.Equals("PlayerArmature"))
        {
            for (int i = 0; i < traps.Length; i++)
            {
                traps[i].GetComponent<GroundTrapTest>().TrapOn();
            }
        }
    }

}
