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
    public Image darkBG;

    public GameObject[] traps;

    public float holdingTime;

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

        
        // 주변 적에게 전달 (위치 유도)

        // 주변 어둡게
        darkBG.gameObject.SetActive(true);


        float holdingTimeRemain = holdingTime;

        while(holdingTimeRemain > 0)
        {
            holdingTimeRemain -= Time.deltaTime;
            yield return null;
        }

        darkBG.gameObject.SetActive(false);
    }
}
