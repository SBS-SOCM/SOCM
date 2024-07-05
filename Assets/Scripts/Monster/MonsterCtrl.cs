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
    [SerializeField] private float checkRange;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        CheckPlayer();
    }


    private void CheckPlayer()
    {
        Vector3 monsterRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);

        if (Physics.Raycast(monsterRay, targetRay - monsterRay, out monsterHitInfo, checkRange + 5.0f))
        {
            if (monsterHitInfo.transform.name == "Player")
            {
                
            }
        }

        nav.SetDestination(targetTr.transform.position);
    }

}
