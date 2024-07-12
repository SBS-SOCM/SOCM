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
                case "Knife":
                    id = 0;
                    explain = "짧은 단검";
                    break;

                case "Pistol":
                    id = 1;
                    explain = "권총";
                    break;
                case "Coin":
                    id = 2;
                    explain = "동전";
                    break;

            }
        }

    }

    public int inventoryCount;

    public Item[] inventory;

    public Image[] itemImages;

    public int nowItem;
    public int ItemCount;

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
                ItemCount++;
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
        int startMax = ItemCount;
        int startLot = 0;

        while (startLot < startMax)
        {
            int idMin = inventory[startLot].id;
    

            for (int i = startLot + 1; i < inventoryCount; i++)
            {
                if (idMin > inventory[i].id)
                {
                }
            }
        }
        

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

    public void DropItem(int itemId)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            if (inventory[i].id == itemId)
            {
                inventory[i] = null;
                SortInventory();

                return;
            }
        }
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
            
            itemImages[i].sprite = Resources.Load<Sprite>("Item/" + inventory[i].name);

            if (i == nowItem)
            {
                itemImages[i].transform.localScale = new Vector3(1.5f, 1.5f, 1);
            }
            else
            {
                itemImages[i].transform.localScale = new Vector3(1, 1, 1);
            }
            
        }
    }

}
