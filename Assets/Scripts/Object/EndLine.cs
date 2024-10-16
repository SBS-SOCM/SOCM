using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLine : MonoBehaviour
{


    public void EndGame()
    {
        StartCoroutine(Singleton.instance.GetComponentInChildren<UIManager>().OpenEnd());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            EndGame();
        }
    }
}
