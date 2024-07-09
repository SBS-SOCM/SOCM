using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float[] soundsVolume;

    public void ChangeVolume(float volume, int type)
    {
        soundsVolume[type] = volume;
    }
}
