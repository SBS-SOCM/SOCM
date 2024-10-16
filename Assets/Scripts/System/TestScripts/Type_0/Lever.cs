using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    // ����° ���������� ��Ÿ���� �ð�
    [TabGroup("Global")] public float interactionTime;
    
    // �ʿ��� �ð�
    [TabGroup("Global")] public float needTime;

    // ������ UI
    [TabGroup("Inagame")] public Image gauge;
    [TabGroup("Inagame")] public Image gaugeBackGround;

    // ������ ������ �ɸ��� �ð�
    // [TabGroup("Global")] public float holdingTime;

    // �ֺ� ENPC ���� Warning �ִ� �Ÿ�
    // [TabGroup("Global")] public float findEnemyRange;

    // ���� ����
    [TabGroup("Global")] public bool isOn;

    // ����
    [TabGroup("Global")]
    public GameObject switchLever;

    // ������ �������� �� , �������� �ϴ� Ÿ��
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

        // �ֺ� ������ ���� (��ġ ����)

        // �ֺ� ��Ӱ�
        darkBG.gameObject.SetActive(true);

        // �ֺ� ENPC �þ� ����
        Collider[] hits = FindEnemy();
        DecreaseSight(hits);

        // ������ ������ �ɸ��� �ð� or Event
        float holdingTimeRemain = holdingTime;

        while(holdingTimeRemain > 0)
        {
            holdingTimeRemain -= Time.deltaTime;
            yield return null;
        }

        // ������ ����
        darkBG.gameObject.SetActive(false);

        // �ֺ� ENPC �þ� ����
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
