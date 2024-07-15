using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public Inventory inventory;
    public float interactionDistance;

    Collider[] hits;

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            SendInteraction();
        }
    }

    public void SendInteraction()
    {
        hits = Physics.OverlapSphere(player.transform.position, interactionDistance, 1<<6);

        if (hits.Length > 0)
        {
            ReceiveInteraction(hits[0]);
        }
    }

    public void ReceiveInteraction(Collider interactionObject)
    {
        string interactionName = interactionObject.name;

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
        }
    }
}
