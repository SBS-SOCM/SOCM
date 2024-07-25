using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;
using UnityEditor;
using UnityEngine.UIElements;
using StarterAssets;
using System.Runtime.CompilerServices;

public class MonsterCtrl : MonoBehaviour
{
    //Monster AI
    private NavMeshAgent nav;
    private Animator _animator;
    public float updateInterval = 12.0f;
    public float timeSinceLastUpdate;
    public float canRandomMoveTime = 5.0f;

    //Monster State
    public bool isPlayerChecked = false;
    public bool isWarning = false;
    public bool isSleeping = false;
    public bool canRandomMove = true;
    private bool isDie = false;
    public float monsterHP = 100.0f;
    private bool isMoveing = false;
    public bool courseMove = false;
    private float courMoveTiem = 5.0f;

    [SerializeField] private GameObject targetTr;
    [SerializeField] private float soundRange;
    [SerializeField] private float viewRange;
    [SerializeField] private Text stateText;
    [SerializeField] private Transform movePos1;
    [SerializeField] private Transform movePos2;


    //Search Player
    [Range(0, 360)]
    public float Angle;
    private float normalAngle = 65.0f;
    public LayerMask targetMask;
    public LayerMask walllMask;
    public List<Transform> Enemies = new List<Transform>();
    public List<Transform> Allys = new List<Transform>();
    private StarterAssetsInputs _input;
    public float notInViewTime = 3.0f;
    private float playerChaseTime = 0.5f;


    //Check Enemy
    public float enemyCheckRange = 30.0f;
    public LayerMask enemyMask;
    private float outRangeTime = 5.0f;

    //Battle
    public float attackRange = 2.0f;
    public float attackTerm = 1.0f;
    private bool isAttacking = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        courMoveTiem -= Time.deltaTime;

        if (isWarning || isPlayerChecked) Angle = normalAngle * 2f;
        else Angle = normalAngle;
        Debug.DrawRay(transform.position, Vector3.forward, Color.red, 10.5f);

        //Ontrigger�� player�� ���������� ���� -> ����ȭ
        if (!isSleeping && !isDie)
        {
            CheckSound();
            CheckView();
            CheckState();
            if (!isWarning && !isPlayerChecked)
            {
                if (canRandomMove)
                {
                    canRandomMoveTime -= Time.deltaTime;
                    if (canRandomMoveTime <= 0.0f) CheckRandomMove();
                }
                else if(courseMove)
                {
                    if (courMoveTiem <= 0.0f && courseMove)
                    {
                        courMoveTiem = 60.0f;
                        StartCoroutine(CourseMove());
                    }
                }
                
            }
            else canRandomMoveTime = 6.0f;
            
        }
        else
        {
            stateText.text = "";
        }
        if (courseMove)
        {
            StopCourseMove();
        }
        
        CheckSleeping();
        CheckDie();

    }
    IEnumerator CourseMove()
    {
        nav.SetDestination(movePos1.position);
        _animator.SetBool("Walk", true);
        Debug.Log(Vector3.Distance(this.transform.position, movePos1.position));
        yield return new WaitForSeconds(5.0f);


        yield return new WaitForSeconds(5.0f);
        nav.SetDestination(movePos2.position);
        _animator.SetBool("Walk", true);


    }
    private void StopCourseMove()
    {
        if(Vector3.Distance(this.transform.position, movePos2.position) <= 1.0f ||
            Vector3.Distance(this.transform.position, movePos1.position) <= 1.0f)
        {
            nav.ResetPath();
            _animator.SetBool("Walk",false);
        }
    }
    void CheckSleeping()
    {
        if (isSleeping)
        {
            _animator.SetBool("Sleeping", true);
        }
        else
        {
            _animator.SetBool("Sleeping", false);
        }
    }
    void CheckDie()
    {
        if(monsterHP <= 0.0f && !isDie)
        {
            isDie = true;
            nav.ResetPath();
            StopAllCoroutines();
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);
            _animator.SetTrigger("Die");
        }
    }
    void CheckAttack()
    {
        float targetDist = Vector3.Distance(targetTr.transform.position, this.transform.position);
        if (targetDist <= attackRange && !isAttacking) //Attack
        {
            nav.enabled = false;
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);
            _animator.SetTrigger("Attack");
            CharacterManager.instance.hp -= 1;

            nav.enabled = true;
        }
    }
    void CheckRandomMove()
    {
        timeSinceLastUpdate += Time.deltaTime; // �ð� ���� �����մϴ�.

        if (timeSinceLastUpdate >= updateInterval) // ������ �ð� ������ �������� Ȯ���մϴ�.
        {
            timeSinceLastUpdate = 0f; // �ð� ���� �ʱ�ȭ�մϴ�.
            Vector3 randomPosition = GetRandomPositionOnNavMesh(); // NavMesh ���� ������ ��ġ�� �����ɴϴ�.
            Vector3 nowPosition = transform.position;
            float randomDistance = Vector3.Distance(randomPosition, nowPosition);
            if (randomDistance >= 10.0f)
            {
                nav.SetDestination(randomPosition); // NavMeshAgent�� ��ǥ ��ġ�� ���� ��ġ�� �����մϴ�.
                _animator.SetBool("Walk", true);
            }
        }
        if (timeSinceLastUpdate >= 6.0f)
        {
            nav.ResetPath();
            _animator.SetTrigger("idle");
        }
    }
    Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 25.0f; // ���ϴ� ���� ���� ������ ���� ���͸� �����մϴ�.
        randomDirection += transform.position; // ���� ���� ���͸� ���� ��ġ�� ���մϴ�.

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 25.0f, NavMesh.AllAreas)) // ���� ��ġ�� NavMesh ���� �ִ��� Ȯ���մϴ�.
        {
            return hit.position; // NavMesh ���� ���� ��ġ�� ��ȯ�մϴ�.
        }
        else
        {
            return transform.position; // NavMesh ���� ���� ��ġ�� ã�� ���� ��� ���� ��ġ�� ��ȯ�մϴ�.
        }
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

    private void SpreadWarning()
    {
        Allys.Clear();
        Collider[] results = new Collider[100];
        var size = Physics.OverlapSphereNonAlloc(transform.position, enemyCheckRange, results, enemyMask);

        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;
            enemy.GetComponent<MonsterCtrl>().isWarning = true;
            Allys.Add(enemy);
        }
    }

    private void StopAnimator()
    {
        if (!canRandomMove && !courseMove)
        {
            _animator.SetBool("Run", false);
            _animator.SetBool("Walk", false);
            nav.ResetPath();
        }
        
    }
    private void CheckSound()
    {
        float soundCheckRange;
        if (CharacterManager.instance.isSilence) soundCheckRange = 0.0f;
        else soundCheckRange = soundRange;
        if(isWarning || isPlayerChecked)
        {
            soundCheckRange *= 1.3f;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetTr.transform.position - transform.position, out hit))
        {
            if(hit.transform.CompareTag("Wall"))
            {
                soundCheckRange *= 0.1f;
            }
        }
        float targetDist = Vector3.Distance(targetTr.transform.position, this.transform.position);

        if (targetDist <= soundCheckRange)
        {
            nav.SetDestination(targetTr.transform.position);
            if (!isPlayerChecked)
            {
                isWarning = true;
                outRangeTime = 5.0f;
                _animator.SetBool("Run", false);
                _animator.SetBool("Walk", true);
            }
        }else if(targetDist >= soundCheckRange)
        {
            outRangeTime -= Time.deltaTime;
            if(outRangeTime <= 0.0f)
            {
                StopAnimator();
                isWarning = false;
                outRangeTime = 5.0f;
            }
        }
    }

    void CheckView()
    {
        float checkViewRange;
        if (CharacterManager.instance.isVisible) checkViewRange = viewRange;
        else checkViewRange = 2f;

        if (isWarning || isPlayerChecked) checkViewRange *= 1.3f;
        Enemies.Clear();
        Collider[] results = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, checkViewRange, results, targetMask);

        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;

            Vector3 dirToTarget = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < Angle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, enemy.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, walllMask)) 
                {
                    Enemies.Add(enemy);
                }
            }
        }

        // 3 State
        if (Enemies.Count > 0) //Player in View Range
        {
            playerChaseTime -= Time.deltaTime;
            if(playerChaseTime <= 0.0f)
            {
                SpreadWarning();
                notInViewTime = 3.0f;
                isWarning = false;
                isPlayerChecked = true;
                canRandomMove = false;
                courseMove = false;
                nav.SetDestination(targetTr.transform.position);
                _animator.SetBool("Walk", false);
                _animator.SetBool("Run", true);

                CharacterManager.instance.OnVisible();
                CheckAttack();
            }
        }
        else if(Enemies.Count == 0 && isWarning)
        {
            playerChaseTime = Random.Range(0.3f, 1.0f);
            notInViewTime -= Time.deltaTime;
            if(notInViewTime <= 0.0f)
            {
                isWarning = true;
                isPlayerChecked = false;
            }
            
        }
        else
        {
            playerChaseTime = Random.Range(0.3f, 1.0f);
            notInViewTime -= Time.deltaTime;
            if (notInViewTime <= 0.0f)
            {
                isWarning = false;
                isPlayerChecked = false;
                StopAnimator();
            }
            
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, soundRange);


        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRange);
        float angle = -Angle * 0.5f + transform.eulerAngles.y;
        float angle2 = Angle * 0.5f + transform.eulerAngles.y;
        Vector3 AngleLeft = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        Vector3 AngleRight = new Vector3(Mathf.Sin(angle2 * Mathf.Deg2Rad), 0, Mathf.Cos(angle2 * Mathf.Deg2Rad)); // ������

        Handles.color = Color.green;

        Handles.DrawLine(transform.position, transform.position + AngleLeft * viewRange);
        Handles.DrawLine(transform.position, transform.position + AngleRight * viewRange);
        Handles.color = Color.red;
        foreach (Transform enemy in Enemies)
        {
            Handles.DrawLine(transform.position, enemy.position);
        }
    }*/
}
