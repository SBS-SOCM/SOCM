using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Renderer objectRenderer;
    public float objectMinHeight;
    public float objectMaxHeight;

    void Start()
    {
        ApplyHeightSettings();
    }

    void ApplyHeightSettings()
    {
        Material material = objectRenderer.material;
        material.SetFloat("_ObjectMinHeight", objectMinHeight);
        material.SetFloat("_ObjectMaxHeight", objectMaxHeight);
    }
}
