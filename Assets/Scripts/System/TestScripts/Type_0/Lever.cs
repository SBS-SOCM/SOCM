using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    // 몇초째 진행중인지 나타내는 시간
    [TabGroup("Global")] public float interactionTime;
    
    // 필요한 시간
    [TabGroup("Global")] public float needTime;

    // 게이지 UI
    [TabGroup("Inagame")] public Image gauge;
    [TabGroup("Inagame")] public Image gaugeBackGround;

    // 분전반 복구에 걸리는 시간
    // [TabGroup("Global")] public float holdingTime;

    // 주변 ENPC 한테 Warning 주는 거리
    // [TabGroup("Global")] public float findEnemyRange;

    // 현재 상태
    [TabGroup("Global")] public bool isOn;

    // 래버
    [TabGroup("Global")]
    public GameObject switchLever;

    // 래버가 내려졌을 떄 , 없어져야 하는 타워
    [TabGroup("Inagame")] public GameObject[] watchTowerLights;


    public void InteractionOn()
    {
        StartCoroutine(Interaction());
    }

    IEnumerator Interaction()
    {
        interactionTime = 0;
        isOn = false;

        gauge.gameObject.SetActive(true);
        gaugeBackGround.gameObject.SetActive(true);

        while (interactionTime < needTime)
        {
            interactionTime += Time.deltaTime;
            gauge.fillAmount = interactionTime / needTime;
            yield return null;
        }

        switchLever.transform.DORotate(new Vector3(23.967f, 177.146f, -1.16f),0.2f);


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

    /*
    public Collider[] FindEnemy()
    {
        Collider[] hits;

        hits = Physics.OverlapSphere(CharacterManager.instance.gameObject.transform.position, findEnemyRange, LayerMask.NameToLayer("Enemy"));

        return(hits);
    }

    */

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
