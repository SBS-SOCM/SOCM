using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;
using UnityEditor;

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


    //Search Player
    [Range(0, 360)] //인스펙터에서 슬라이더로 표시 
    public float Angle = 90f; //기본값 90도
    public LayerMask targetMask; // 적을 검색하기 위한 레이어마스크
    public LayerMask walllMask; // 장애물 마스크
    public List<Transform> Enemies = new List<Transform>(); // 범위안에있는 적들


    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.forward, Color.red, 10.5f);
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
    /*private void CheckView()
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
    }*/
    private void CheckSound()
    {
        float targetDist = Vector3.Distance(targetTr.transform.position, this.transform.position);
        if (targetDist <= soundRange && !CharacterManager.instance.isSilence)
        {
            nav.SetDestination(targetTr.transform.position);
            if (!isPlayerChecked)
            {
                isWarning = true;
            }
        }
    }

    void CheckView()
    {
        Enemies.Clear();
        Collider[] results = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, viewRange, results, targetMask);

        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;

            Vector3 dirToTarget = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < Angle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, enemy.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, walllMask)) 
                {
                    if (CharacterManager.instance.isVisible)
                    {
                        Enemies.Add(enemy);
                    }
                }
            }
        }

        // 3 State
        if (Enemies.Count > 0) //Player in View Range
        {
            isWarning = false;
            isPlayerChecked = true;
            nav.SetDestination(targetTr.transform.position);
        }
        else if(Enemies.Count == 0 && isWarning)
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

    private void OnDrawGizmosSelected()
    {
        CheckView();
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRange);
        float angle = -Angle * 0.5f + transform.eulerAngles.y;
        float angle2 = Angle * 0.5f + transform.eulerAngles.y;
        Vector3 AngleLeft = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        Vector3 AngleRight = new Vector3(Mathf.Sin(angle2 * Mathf.Deg2Rad), 0, Mathf.Cos(angle2 * Mathf.Deg2Rad)); // 오른쪽

        Handles.color = Color.green;

        Handles.DrawLine(transform.position, transform.position + AngleLeft * viewRange);
        Handles.DrawLine(transform.position, transform.position + AngleRight * viewRange);
        Handles.color = Color.red;
        foreach (Transform enemy in Enemies)
        {
            Handles.DrawLine(transform.position, enemy.position);
        }
    }
}
