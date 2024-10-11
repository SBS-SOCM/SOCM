using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchTowerLight : MonoBehaviour
{
    Collider[] hits;

    public float interactionAngle;
    public bool isPlayer;

    // 감지 범위 변수 추가
    public float detectionRadius = 100f;

    public float angle;

    public float normalSpeed;

    GameObject player;

    public float maxAngleSpeed;

    float rotateX;
    float rotateY;
    float rotateZ;

    public bool isCoroutine; 

    // Start is called before the first frame update
    void Start()
    {
        rotateLeft();

        rotateX = transform.rotation.eulerAngles.x;
        rotateY = transform.rotation.eulerAngles.y;
        rotateZ = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
    }

    public void FindPlayer()
    {
        hits = Physics.OverlapSphere(gameObject.transform.position, detectionRadius, 1 << 6);

        Collider validHit = GetValidHit(hits);

        if (validHit != null)
        {
            

            // 감지범위 내에 플레이어 존재
            if (!isCoroutine)
            {
                if (!CharacterManager.instance.isVisible)
                {
                    return;
                }

                player = validHit.gameObject;

                isPlayer = true;
               
                DOTween.KillAll();
                CancelInvoke("rotateRight");
                CancelInvoke("rotateLeft");
                StopCoroutine(TracePlayer());
                StartCoroutine(TracePlayer());
            }

            
        }
        else
        {
            // 감지범위 내에 플레이어 부재
            isPlayer = false;
        }
    }

    IEnumerator TracePlayer()
    {
        isCoroutine = true;

        float remainTime = 2;
        

        // player가 탐지되는 동안
        while (remainTime > 0)
        {
            if (isPlayer)
            {
                remainTime = 2;
            }

            Vector3 targetVec = player.transform.position - transform.position; // playerTransform 사용

            // 목표 회전 계산
            Quaternion targetRotation = Quaternion.LookRotation(targetVec);

            // 현재 회전과 목표 회전 간의 각도 차이 계산
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // 최대 회전 속도를 적용하여 각도 변경
            float rotationSpeed = Mathf.Min(maxAngleSpeed * Time.deltaTime, angleDifference);

            // 회전 적용
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

            remainTime -= Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        isCoroutine = false;

        yield return new WaitForSeconds(1);

        transform.DORotate(new Vector3(rotateX, rotateY, rotateZ) , normalSpeed);

        yield return new WaitForSeconds(normalSpeed);

        rotateLeft();

        
    }


    private Collider GetValidHit(Collider[] hitsColl)
    {
        foreach (var hit in hitsColl)
        {
            Vector3 directionToTarget = (hit.transform.position - gameObject.transform.position).normalized;
            angle = Vector3.Angle(gameObject.transform.forward, directionToTarget);

            if (angle < interactionAngle)
            {
                return hit;
            }
        }

        return null;
    }

    // 감지 범위를 씬 뷰에 표시하는 메서드
    private void OnDrawGizmosSelected()
    {
        // 녹색 반투명 구체로 감지 범위를 시각적으로 표시
        Gizmos.color = new Color(0, 1, 0, 0.3f);  // 녹색, 30% 투명
        Gizmos.DrawSphere(transform.position, detectionRadius);

        // 노란색 선으로 Interaction Angle 범위를 시각적으로 표시
        Vector3 forwardDirection = transform.forward;

        // 좌우 경계
        Vector3 rightBoundary = Quaternion.Euler(0, interactionAngle, 0) * forwardDirection * detectionRadius;
        Vector3 leftBoundary = Quaternion.Euler(0, -interactionAngle, 0) * forwardDirection * detectionRadius;

        // 상하 경계
        Vector3 upBoundary = Quaternion.Euler(interactionAngle, 0, 0) * forwardDirection * detectionRadius;
        Vector3 downBoundary = Quaternion.Euler(-interactionAngle, 0, 0) * forwardDirection * detectionRadius;

        Gizmos.color = Color.yellow;

        // 좌우 경계선 그리기
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        // 상하 경계선 그리기
        Gizmos.DrawLine(transform.position, transform.position + upBoundary);
        Gizmos.DrawLine(transform.position, transform.position + downBoundary);
    }

    public void rotateLeft()
    {
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0,90,0), normalSpeed);

        Invoke("rotateRight", normalSpeed + 1);
    }

    public void rotateRight()
    {
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, -90, 0), normalSpeed);

        Invoke("rotateLeft", normalSpeed + 1);
    }
}
