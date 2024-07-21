using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public float fadeCount = 1;

    void Start()
    {
            Color targetColor = meshRenderer.material.color;
        targetColor.a = 0;

        meshRenderer.material.DOColor(targetColor, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
