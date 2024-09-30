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

    // Start is called before the first frame update
    void Start()
    {

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
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }
    }

    private Collider GetValidHit(Collider[] hitsColl)
    {
        foreach (var hit in hitsColl)
        {
            Vector3 directionToTarget = (hit.transform.position - CharacterManager.instance.gameObject.transform.position).normalized;
            float angle = Vector3.Angle(gameObject.transform.forward, directionToTarget);

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
}
