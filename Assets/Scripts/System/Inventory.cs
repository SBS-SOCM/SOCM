using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public class Item
    {
        public string name;
        public int id;
        public int count;
        public string explain;

        public Item(string name)
        {
            this.name = name;
            switch (name)
            {
                case "knife":
                    id = 0;
                    explain = "짧은 단검";
                    break;

                case "pistol":
                    id = 1;
                    explain = "권총";
                    break;
            }
        }

    }

    public int inventoryCount;

    public Item[] inventory;

    public GameObject inventoryPanel;
    public Button[] inventoryItemButtons;
    public Text explainText;

    public int nowWeapon;

    private void Start()
    {
        inventory = new Item[inventoryCount];
}
    public void GetItem(Item item)
    {
        Debug.Log(item.name);

        for (int i = 0; i < inventoryCount; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                inventoryItemButtons[i].interactable = true;
                SortInventory();
                break;
            }

            else if (inventory[i].name == item.name)
            {
                inventory[i].count++;
            }
        }

        RenewInventoryUI();
    }

    public void SortInventory()
    {
        for (int i = 0; i < inventoryCount - 1; i++)
        {
            if (inventory[i] == null)
            {
                break;
            }

            for (int j = i + 1; j < inventoryCount; j++)
            {
                if (inventory[j] == null)
                {
                    break;
                }

                if (inventory[i].id > inventory[j].id)
                {
                     Item temp = inventory[i];
                    inventory[i] = inventory[j];
                    inventory[j] = temp;
                }
            }
        }
    }

    public void ClickItem(int index)
    {
        explainText.text = inventory[index].explain;
    }

    public void ClickUseButton()
    {
        // !! 아이템 사용
    }

    public  Item FindItem(string name)
    {
        foreach ( Item item in inventory)
        {
            if (item != null && item.name == name)
            {
                return item;
            }
        }
        return null;
    }

    public void Test()
    {
        for (int i = 0; i < inventoryCount; i++)
        {
            if (inventory[i] != null)
            {
                Debug.Log(inventory[i].name);
            }
        }
        RenewInventoryUI();
    }

    public  Item FindItem(int itemId)
    {
        foreach ( Item item in inventory)
        {
            if (item.id == itemId)
            {
                return item;
            }
        }
        return null;
    }

    public void RenewInventoryUI()
    {
        for (int i = 0; i < inventoryCount; i++)
        {
            if (inventory[i] == null)
            {
                return;
            }
            switch (inventory[i].name)
            {
                // 이미지 불러오기
                case "":
                    inventoryItemButtons[i].image.sprite = Resources.Load<Sprite>("");
                    break;
            }
        }
    }
}
