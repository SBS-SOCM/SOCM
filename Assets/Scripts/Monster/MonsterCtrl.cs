using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //Monster AI
    private NavMeshAgent nav;
    private RaycastHit monsterHitInfo;


    [SerializeField] private GameObject targetTr;
    [SerializeField] private float soundRange;
    [SerializeField] private float viewRange;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        CheckView();
        CheckSound();
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
            }
        }
    }
    private void CheckSound()
    {
        float targetDist = Vector3.Distance(targetTr.transform.position, this.transform.position);
        if (targetDist <= viewRange)
        {
            nav.SetDestination(targetTr.transform.position);
        }
    }

}
