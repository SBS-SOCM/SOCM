using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Inventory inventory;
    public float interactionDistance;
    public float interactionAngle = 45f; // ��ȣ�ۿ� ������ ���� (45���� ����)

    Collider[] hits;

    public Image interactionImage;

    GameObject hitObject;
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
        hits = Physics.OverlapSphere(CharacterManager.instance.gameObject.transform.position, interactionDistance, 1 << 9);

        if (hits.Length > 0)
        {
            Collider validHit = GetValidHit(hits);
            if (validHit != null)
            {
                Debug.Log("DO Interaction");

                ReceiveInteraction(validHit);
            }
        }
    }

    public void CheckInteraction()
    {
        if (CharacterManager.instance.gameObject == null)
        {
            return;
        }

        hits = Physics.OverlapSphere(CharacterManager.instance.gameObject.transform.position, interactionDistance, 1 << 9);

        Collider validHit = GetValidHit(hits);

        if (validHit != null)
        {
            interactionImage.gameObject.SetActive(true);

            
            // OutlineController�� ã�Ƽ� �ܰ��� Ȱ��ȭ
            OutlineController outlineController = validHit.GetComponent<OutlineController>();

            if (hitObject != null && hitObject != validHit.gameObject)
            {
                hitObject.GetComponent<OutlineController>().RemoveOutline();
            }

            hitObject = validHit.gameObject;
            
            if (outlineController != null)
            {
                outlineController.ApplyOutline();  // �ܰ��� ����
            }
            
        }
        else
        {
            interactionImage.gameObject.SetActive(false);

            
            if (hitObject != null && hitObject.GetComponent<OutlineController>() != null)
            {
                // ������ �ܰ����� ����� ������Ʈ�� �ܰ����� ����
                hitObject.GetComponent<OutlineController>().RemoveOutline();

            }
            

        }
    }

    public void ReceiveInteraction(Collider interactionObject)
    {
        string interactionName = interactionObject.name;

        Debug.Log("��ȣ�ۿ� : " + interactionName);



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

            case "Door":
                interactionObject.GetComponent<Door>().Open();
                break;

            case "Chest":
                interactionObject.GetComponent<Chest>().Open();
                break;

            case "HideObject":
                interactionObject.GetComponent<HideObject>().InteractionSend();
                break;

            case "EndObject":
                interactionObject.GetComponent<EndObject>().DelEndBlock();
                break;
        }
    }

    private Collider GetValidHit(Collider[] hitsColl)
    {
        foreach (var hit in hitsColl)
        {
            Vector3 directionToTarget = (hit.transform.position - CharacterManager.instance.gameObject.transform.position).normalized;
            float angle = Vector3.Angle(CharacterManager.instance.gameObject.transform.forward, directionToTarget);

            if (angle < interactionAngle)
            {
                return hit;
            }
        }

        return null;
    }
}
