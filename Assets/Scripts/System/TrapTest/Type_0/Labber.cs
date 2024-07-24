using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Labber : MonoBehaviour
{
    public float interactionTime;
    public float needTime;

    public Image gauge;
    public Image gaugeBackGround;

    public GameObject[] traps;

    public void InteractionOn()
    {
        StartCoroutine(Interaction());
    }

    IEnumerator Interaction()
    {
        gauge.gameObject.SetActive(true);
        gaugeBackGround.gameObject.SetActive(true);

        while (interactionTime < needTime)
        {
            interactionTime += Time.deltaTime;
            gauge.fillAmount = interactionTime / needTime;
            yield return null;
        }

        gauge.gameObject.SetActive(false);
        gaugeBackGround.gameObject.SetActive(false);

        for (int i = 0; i < traps.Length; i++)
        {
            traps[i].GetComponent<GroundTrapTest>().TrapOff();
        }
    }
}
