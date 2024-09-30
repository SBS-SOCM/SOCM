using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchTowerLight : MonoBehaviour
{
    Collider[] hits;

    public float interactionAngle;
    public bool isPlayer;

    // ���� ���� ���� �߰�
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

    // ���� ������ �� �信 ǥ���ϴ� �޼���
    private void OnDrawGizmosSelected()
    {
        // ��� ������ ��ü�� ���� ������ �ð������� ǥ��
        Gizmos.color = new Color(0, 1, 0, 0.3f);  // ���, 30% ����
        Gizmos.DrawSphere(transform.position, detectionRadius);

        // ����� ������ Interaction Angle ������ �ð������� ǥ��
        Vector3 forwardDirection = transform.forward;

        // �¿� ���
        Vector3 rightBoundary = Quaternion.Euler(0, interactionAngle, 0) * forwardDirection * detectionRadius;
        Vector3 leftBoundary = Quaternion.Euler(0, -interactionAngle, 0) * forwardDirection * detectionRadius;

        // ���� ���
        Vector3 upBoundary = Quaternion.Euler(interactionAngle, 0, 0) * forwardDirection * detectionRadius;
        Vector3 downBoundary = Quaternion.Euler(-interactionAngle, 0, 0) * forwardDirection * detectionRadius;

        Gizmos.color = Color.yellow;

        // �¿� ��輱 �׸���
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        // ���� ��輱 �׸���
        Gizmos.DrawLine(transform.position, transform.position + upBoundary);
        Gizmos.DrawLine(transform.position, transform.position + downBoundary);
    }
}
