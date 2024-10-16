using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Inventory inventory;
    public float interactionDistance;
    public float interactionAngle = 45f; // 상호작용 가능한 각도 (45도로 설정)

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

            
            // OutlineController를 찾아서 외곽선 활성화
            OutlineController outlineController = validHit.GetComponent<OutlineController>();

            if (hitObject != null && hitObject != validHit.gameObject)
            {
                hitObject.GetComponent<OutlineController>().RemoveOutline();
            }

            hitObject = validHit.gameObject;
            
            if (outlineController != null)
            {
                outlineController.ApplyOutline();  // 외곽선 적용
            }
            
        }
        else
        {
            interactionImage.gameObject.SetActive(false);

            
            if (hitObject != null && hitObject.GetComponent<OutlineController>() != null)
            {
                // 이전에 외곽선이 적용된 오브젝트의 외곽선을 제거
                hitObject.GetComponent<OutlineController>().RemoveOutline();

            }
            

        }
    }

    public void ReceiveInteraction(Collider interactionObject)
    {
        string interactionName = interactionObject.name;

        Debug.Log("상호작용 : " + interactionName);



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
