using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //Monster AI
    private NavMeshAgent nav;
    private RaycastHit monsterHitInfo;

    //Monster State
    public bool isPlayerChecked = false;
    public bool isWarning = false;

    [SerializeField] private GameObject targetTr;
    [SerializeField] private float soundRange;
    [SerializeField] private float viewRange;
    [SerializeField] private Text stateText;


    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckSound();
        CheckView();
        CheckState();
    }
    private void CheckState()
    {
        if (isPlayerChecked) 
        {
            stateText.text = "!";
            stateText.color = Color.red;
        }
        else if (isWarning)
        {
            stateText.text = "?";
            stateText.color = Color.yellow;
        }
        else if(!isPlayerChecked && !isWarning)
        {
            stateText.text = "";
        }
    }
    private void CheckView()
    {
        Vector3 monsterRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);

        if (Physics.Raycast(monsterRay, targetRay - monsterRay, out monsterHitInfo, viewRange))
        {
            if (monsterHitInfo.transform.tag == "Player")
            {
                nav.SetDestination(targetTr.transform.position);
                isWarning = false;
                isPlayerChecked = true;
            }else if (monsterHitInfo.transform.tag !="Player" && isWarning)
            {
                isWarning = true;
                isPlayerChecked = false;
            }
            else
            {
                isWarning = false;
                isPlayerChecked = false;
            }
        }
    }
    private void CheckSound()
    {
        float targetDist = Vector3.Distance(targetTr.transform.position, this.transform.position);
        if (targetDist <= soundRange)
        {
            nav.SetDestination(targetTr.transform.position);
            if (!isPlayerChecked)
            {
                isWarning = true;
            }
        }
    }

}
