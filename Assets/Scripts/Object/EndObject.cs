using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndObject : MonoBehaviour
{
    public GameObject EndBlock;
    
    public void DelEndBlock()
    {
        EndBlock.SetActive(false);
    }
}
