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
    }

    public Collider[] FindEnemy()
    {
        Collider[] hits;

        hits = Physics.OverlapSphere(Singleton.instance.player.transform.position, findEnemyRange, LayerMask.NameToLayer("Enemy"));

        return(hits);
    }

    public void DecreaseSight(Collider[] hits)
    {
        foreach (Collider hit in hits)
        {
            if (hit != null)
            {
                // hit.GetComponent<MonsterCtrl>().SetViewRange();
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
