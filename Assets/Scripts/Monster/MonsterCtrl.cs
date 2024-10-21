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
    public bool isDie = false;
    public int monsterHP = 2;
    public bool courseMove = false;
    private float courMoveTiem = 5.0f;
    public float courseMoveTerm = 8.0f;

    private Transform targetTr;
    [SerializeField] private float soundRange;
    [SerializeField] private float viewRange;
    [SerializeField] private Text stateText;
    [SerializeField] private Transform movePos1;
    [SerializeField] private Transform movePos2;
    [SerializeField] private Text stabbingText;
    [SerializeField] public Transform neckPos;

    //Search Player
    [Range(0, 360)]
    public float Angle;
    private float normalAngle = 65.0f;
    public LayerMask targetMask;
    public LayerMask allyMask;
    public LayerMask walllMask;
    public List<Transform> Enemies = new List<Transform>();
    public List<Transform> Allys = new List<Transform>();
    public float notInViewTime = 6.0f;
    private float playerChaseTime = 0.5f;

    //Check Enemy
    public float enemyCheckRange = 30.0f;
    private float outRangeTime = 10.0f;
    private float playerY = 1.0f;
    private float gunFireCheckRange = 40.0f;
    private float voiceTerm = 0.0f;

    //Battle
    public float attackRange = 2.0f;
    private float attackTerm = 1.0f;
    private bool isAttacking = false;
    private AudioSource audioSource;
    [SerializeField] private AudioClip RifleAudioClip;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPos;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        targetTr = GameObject.Find("Geometry").transform;
    }

    private void Update()
    {
        voiceTerm -= Time.deltaTime;
        courMoveTiem -= Time.deltaTime;

        if (isWarning || isPlayerChecked) Angle = normalAngle * 2f;
        else Angle = normalAngle;

        if (ThirdPersonController.isProne) playerY = 0.2f;
        else playerY = 1.0f;

        //Ontrigger로 player가 들어왔을떄만 실행 -> 최적화
        if (!isSleeping && !isDie && !isAttacking)
        {
            if(CharacterManager.instance.isMoving)
            {
                CheckSound();
            }
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
                        courMoveTiem = Random.Range(60.0f,100.0f);
                        StartCoroutine(CourseMove());
                    }
                }
                
            }
            else canRandomMoveTime = 6.0f;
            
        }else if (isSleeping)
        {
            CheckSound();
        }
        /*if (courseMove)
        {
            StopCourseMove();
        }*/
        if (isPlayerChecked)
        {
            this.transform.LookAt(targetTr.transform.position);
        }


        CheckSleeping();
        CheckDie();
    }
    /*private void TurnSmooth()
    {
        Vector2 forward = new Vector2(transform.position.z, transform.position.x);
        Vector2 steeringTarget = new Vector2(nav.steeringTarget.z, nav.steeringTarget.x);

        //방향을 구한 뒤, 역함수로 각을 구한다.
        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //방향 적용
        transform.eulerAngles = Vector3.up * angle;

    }*/

    private IEnumerator Stabbing()
    {
        nav.ResetPath();
        stabbingText.text = "Q";
        yield return new WaitForSeconds(2f);
        stabbingText.text = "";
    }
    public void StabbingCtrl()
    {
        if(!isDie) StartCoroutine(Stabbing());
    }
    IEnumerator CourseMove()
    {
        Debug.Log("CourseMove");
        nav.SetDestination(movePos2.position);
        _animator.SetBool("Walk", true);

        yield return new WaitForSeconds(courseMoveTerm);
        nav.SetDestination(movePos1.position);
        _animator.SetBool("Walk", true);

        yield return new WaitForSeconds(courseMoveTerm + 2.0f);
        nav.SetDestination(movePos1.position);
        _animator.SetBool("Walk", false);


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
        if(monsterHP <= 0 && !isDie)
        {
            isDie = true;
            isWarning = false;
            isPlayerChecked = false;
            stabbingText.text = "";
            stateText.text = "";
            nav.ResetPath();
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            StopAllCoroutines();
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);
            _animator.SetTrigger("Die");
        }

    }
    void CheckAttack()
    {
        float targetDist = Vector3.Distance(targetTr.position, this.transform.position);
        if (targetDist <= attackRange && !isAttacking) //Attack
        {
            Vector3 dirToTarget = (targetTr.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 15)
            {
                this.transform.LookAt(targetTr);
                StartCoroutine(Attack());
            }

            
        }
    }
    IEnumerator Attack()
    {
        isAttacking = true;
        float tempSpeed = nav.speed;
        nav.speed = 0.0f;
        _animator.SetBool("Walk", false);
        _animator.SetBool("Run", false);

        Vector3 dir = targetTr.position - bulletSpawnPos.position; dir.y = 0f;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        Instantiate(bulletPrefab, bulletSpawnPos.position, rot);
        audioSource.PlayOneShot(RifleAudioClip, 0.7f);
        CharacterManager.instance.InstantiateBloodVfx(1);

        yield return new WaitForSeconds(0.5f);
        nav.speed = tempSpeed;
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;

    }
    void CheckRandomMove()
    {
        timeSinceLastUpdate += Time.deltaTime; // 시간 값을 갱신합니다.

        if (timeSinceLastUpdate >= updateInterval) // 설정한 시간 간격이 지났는지 확인합니다.
        {
            timeSinceLastUpdate = 0f; // 시간 값을 초기화합니다.
            Vector3 randomPosition = GetRandomPositionOnNavMesh(); // NavMesh 위의 랜덤한 위치를 가져옵니다.
            Vector3 nowPosition = transform.position;
            float randomDistance = Vector3.Distance(randomPosition, nowPosition);
            if (randomDistance >= 10.0f)
            {
                nav.SetDestination(randomPosition); // NavMeshAgent의 목표 위치를 랜덤 위치로 설정합니다.
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
        Vector3 randomDirection = Random.insideUnitSphere * 25.0f; // 원하는 범위 내의 랜덤한 방향 벡터를 생성합니다.
        randomDirection += transform.position; // 랜덤 방향 벡터를 현재 위치에 더합니다.

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 25.0f, NavMesh.AllAreas)) // 랜덤 위치가 NavMesh 위에 있는지 확인합니다.
        {
            return hit.position; // NavMesh 위의 랜덤 위치를 반환합니다.
        }
        else
        {
            return transform.position; // NavMesh 위의 랜덤 위치를 찾지 못한 경우 현재 위치를 반환합니다.
        }
    }

    private void CheckState()
    {
        if (isPlayerChecked)
        {
            _animator.SetBool("Aiming",true);
            stateText.text = "!";
            stateText.color = Color.red;
        }
        else if (isWarning)
        {
            _animator.SetBool("Aiming", true);
            stateText.text = "?";
            stateText.color = Color.yellow;
        }
        else if(!isPlayerChecked && !isWarning)
        {
            _animator.SetBool("Aiming", false);
            stateText.text = "";
        }
    }

    private void SpreadWarning()
    {
        Allys.Clear();
        Collider[] results = new Collider[100];
        var size = Physics.OverlapSphereNonAlloc(transform.position, enemyCheckRange, results, allyMask);
        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;
            if (enemy.GetComponent<MonsterCtrl>() != null)
            {
                enemy.GetComponent<MonsterCtrl>().isWarning = true;
            }
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
        else if (isSleeping) soundCheckRange = soundRange / 2;
        else soundCheckRange = soundRange;
        if (CharacterManager.instance.isFire) soundCheckRange = gunFireCheckRange;
        if (isWarning || isPlayerChecked)
        {
            isSleeping = false;
            soundCheckRange *= 1.3f;
        }
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z),
            new Vector3(targetTr.position.x, targetTr.position.y + playerY, targetTr.position.z) - new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z)
            , out hit))
        {
            if(hit.transform.CompareTag("Wall") && soundCheckRange!= gunFireCheckRange)
            {
                soundCheckRange *= 0.1f;
            }
        }
        float targetDist = Vector3.Distance(targetTr.position, this.transform.position);

        if (targetDist <= soundCheckRange)
        {
            if(voiceTerm <= 0.0f)
            {
                audioSource.PlayOneShot(audioClips[0], 0.5f);
                voiceTerm = 10.0f;
            }
            
            nav.SetDestination(targetTr.position);
            if (!isPlayerChecked)
            {
                isWarning = true;
                outRangeTime = 10.0f;
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
                outRangeTime = 10.0f;
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
            RaycastHit hit;

            Transform enemy = results[i].transform;
            Vector3 dirToTarget = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < Angle)
            {
                float dstToTarget = Vector3.Distance(transform.position, enemy.position);
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z),
            new Vector3(targetTr.position.x, targetTr.position.y + playerY, targetTr.position.z) - new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z)
            , out hit))
                {
                    if(hit.transform.CompareTag("Player"))
                    {
                        Enemies.Add(enemy);
                    }
                    
                }
            }
        }

        Collider[] allyResults = new Collider[10];
        var allySize = Physics.OverlapSphereNonAlloc(transform.position, checkViewRange, allyResults, allyMask);
        for (int j = 0; j < allySize; ++j)
        {
            Transform dieAlly = allyResults[j].transform;
            Vector3 dirToTarget = (dieAlly.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < Angle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, dieAlly.position);
                if(!Physics.Raycast(transform.position, dirToTarget,distToTarget, walllMask))
                {
                    if (dieAlly.GetComponent<MonsterCtrl>())
                    {
                        if (dieAlly.GetComponent<MonsterCtrl>().isDie)
                        {
                            SpreadWarning();
                        }
                    }
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
                notInViewTime = 6.0f;
                isWarning = false;
                isPlayerChecked = true;
                canRandomMove = false;
                courseMove = false;
                nav.SetDestination(targetTr.position);
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
}
