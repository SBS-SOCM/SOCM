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

    public float findEnemyRange;

    public GameObject[] watchTowerLights;
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

        for (int i = 0; i < watchTowerLights.Length; i++)
        {
            watchTowerLights[i].SetActive(false);
        }    




        /*

        // 주변 적에게 전달 (위치 유도)

        // 주변 어둡게
        darkBG.gameObject.SetActive(true);

        // 주변 ENPC 시야 감소
        Collider[] hits = FindEnemy();
        DecreaseSight(hits);

        // 분전반 복구에 걸리는 시간 or Event
        float holdingTimeRemain = holdingTime;

        while(holdingTimeRemain > 0)
        {
            holdingTimeRemain -= Time.deltaTime;
            yield return null;
        }

        // 분전반 복구
        darkBG.gameObject.SetActive(false);

        // 주변 ENPC 시야 복구
        rollbackSight(hits);
        */
    }

    public Collider[] FindEnemy()
    {
        Collider[] hits;

        hits = Physics.OverlapSphere(CharacterManager.instance.gameObject.transform.position, findEnemyRange, LayerMask.NameToLayer("Enemy"));

        return(hits);
    }

    public void DecreaseSight(Collider[] hits)
    {
        foreach (Collider hit in hits)
        {
            if (hit != null)
            {
                // hit.GetComponent<MonsterCtrl>().SetViewRange();
                // hit.GetComponent<MonsterCtrl>().isWarning = true;
            }
        }
    }

    public void rollbackSight(Collider[] hits)
    {
        foreach (Collider hit in hits)
        {
            if (hit != null)
            {
                // hit.GetComponent<MonsterCtrl>().SetViewRange();
            }
        }
    }
}
