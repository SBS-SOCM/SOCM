using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //Character State
    public bool isVisible = true;
    public bool isSilence = false;
    
    public static CharacterManager instance;
    private void Awake()
    {
        instance = this;
    }




}
