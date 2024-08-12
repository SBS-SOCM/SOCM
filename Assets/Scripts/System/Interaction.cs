using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public Inventory inventory;

    public float detectionRadius = 5f;  // ���� �ݰ�
    public float detectionAngle = 45f;  // ���� ����
    public LayerMask targetLayerMask;   // 9�� ���̾ �����ϴ� ���̾� ����ũ
    public Color gizmoColor = Color.green;  // ����� ���� ����

    Collider[] hits;

    public Image interactionImage;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Ingame"))
        {
            player = GameObject.Find("PlayerArmature");
        }
    }

    private void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E))
        {
            SendInteraction();
        }
    }

    public void SendInteraction()
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, detectionRadius, targetLayerMask);

        foreach (Collider hit in hits)
        {
            // ������Ʈ�� ���� ���͸� ���
            Vector3 directionToTarget = hit.transform.transform.position - player.transform.position;
            directionToTarget.Normalize();

            // ������Ʈ�� �÷��̾��� �չ��� ���� ���� ������ ���
            float angleToTarget = Vector3.Angle(player.transform.forward, directionToTarget);

            // ���� ���� ���� �ִ��� Ȯ��
            if (angleToTarget <= detectionAngle)
            {
                ReceiveInteraction(hits[0]);
                
            }
        }
    }

    public void CheckInteraction()
    {
        bool isInteractive = false;

        Collider[] hits = Physics.OverlapSphere(player.transform.position, detectionRadius, targetLayerMask);

        foreach (Collider hit in hits)
        {
            // ������Ʈ�� ���� ���͸� ���
            Vector3 directionToTarget = hit.transform.transform.position - player.transform.position;
            directionToTarget.Normalize();

            // ������Ʈ�� �÷��̾��� �չ��� ���� ���� ������ ���
            float angleToTarget = Vector3.Angle(player.transform.forward, directionToTarget);

            // ���� ���� ���� �ִ��� Ȯ��
            if (angleToTarget <= detectionAngle)
            {
                isInteractive = true;
            }
        }

        if (isInteractive)
        {
            interactionImage.gameObject.SetActive(true);
        }

        else
        {
            interactionImage.gameObject.SetActive(false);
        }
    }

    public void ReceiveInteraction(Collider interactionObject)
    {
        string interactionName = interactionObject.name;

        Debug.Log("��ȣ�ۿ� : "+ interactionName);

        switch (interactionName)
        {
            case "StartItem":
                Inventory.Item item1 = new Inventory.Item("Knife");
                inventory.GetItem(item1);

                Inventory.Item item2 = new Inventory.Item("Pistol");
                inventory.GetItem(item2);

                Inventory.Item item3 = new Inventory.Item("Coin");
                inventory.GetItem(item3);
                break;

            case "Map":
                interactionObject.GetComponent<Clue>().ClueOn();
                break;

            case "Labber":
                interactionObject.GetComponent<Labber>().InteractionOn();
                break;
        }
    }

     // ����� �׷��ִ� �޼���
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // �÷��̾� ��ġ�� �������� ������ �׸��� ���� ������ ���
        Vector3 forward = player.transform.forward * detectionRadius;
        Vector3 start = player.transform.position;

        for (float i = -detectionAngle; i <= detectionAngle; i += 5.0f) // 5���� ������ �ּ� ������ ������ �׸��ϴ�.
        {
            // ������ ���� ������ ���
            Vector3 direction = Quaternion.Euler(0, i, 0) * forward;
            Gizmos.DrawRay(start, direction);
        }
        
        // ���� �ݰ��� �ð�ȭ�ϱ� ���� ���� �׸�
        Gizmos.DrawWireSphere(player.transform.position, detectionRadius);
    }
}
