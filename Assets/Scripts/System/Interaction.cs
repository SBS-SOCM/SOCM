using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Inventory inventory;
    public float interactionDistance;

    Collider[] hits;

    public Image interactionImage;

    

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
        hits = Physics.OverlapSphere(Singleton.instance.player.transform.position, interactionDistance, 1<<9);

        if (hits.Length > 0)
        {
            ReceiveInteraction(hits[0]);
        }
    }

    public void CheckInteraction()
    {
        hits = Physics.OverlapSphere(Singleton.instance.player.transform.position, interactionDistance, 1 << 9);

        if (hits.Length > 0)
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

        Debug.Log("상호작용 : "+ interactionName);

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
}
