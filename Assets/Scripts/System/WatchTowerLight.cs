using DG.Tweening;
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
            

            // �������� ���� �÷��̾� ����
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
            // �������� ���� �÷��̾� ����
            isPlayer = false;
        }
    }

    IEnumerator TracePlayer()
    {
        isCoroutine = true;

        float remainTime = 2;
        

        // player�� Ž���Ǵ� ����
        while (remainTime > 0)
        {
            if (isPlayer)
            {
                remainTime = 2;
            }

            Vector3 targetVec = player.transform.position - transform.position; // playerTransform ���

            // ��ǥ ȸ�� ���
            Quaternion targetRotation = Quaternion.LookRotation(targetVec);

            // ���� ȸ���� ��ǥ ȸ�� ���� ���� ���� ���
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // �ִ� ȸ�� �ӵ��� �����Ͽ� ���� ����
            float rotationSpeed = Mathf.Min(maxAngleSpeed * Time.deltaTime, angleDifference);

            // ȸ�� ����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

            remainTime -= Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
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
