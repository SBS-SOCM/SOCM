using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
                    explain = "짧은 단 검";
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

    public int inventorySize;

    public Item[] inventory;

    public Image[] itemImages;

    public int nowItem;
    public int itemCount;

    public float diameter;

    private void Start()
    {
        inventory = new Item[inventorySize];

        diameter = Vector3.Distance(itemImages[0].gameObject.transform.position, transform.GetChild(0).position);
}
    public void GetItem(Item item)
    {
        Debug.Log(item.name);

        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                itemCount++;
                SortInventory();
                break;
            }

            else if (inventory[i].name == item.name)
            {
                inventory[i].count++;
                // break;

            }
            
        }

        RenewInventoryUI();
    }

    public void SortInventory()
    {
        int startMax = itemCount;
        int startLot = 0;

        while (startLot < startMax)
        {
            if (inventory[startLot] == null)
            {
                for (int i = startLot + 1; i < inventorySize; i++)
                {
                    if (inventory[i] != null)
                    {
                        inventory[startLot] = inventory[i];
                        inventory[i] = null;
                    }
                }
            }

            int idMin = inventory[startLot].id;
    

            for (int i = startLot + 1; i < inventorySize; i++)
            {
                if (inventory[i] == null)
                {
                    continue;
                }

                if (idMin > inventory[i].id)
                {
                    Item temp = inventory[i];
                    inventory[i] = inventory[startLot];
                    inventory[startLot] = temp;

                    idMin = inventory[startLot].id;
                }
            }

            startLot++;
        }
        

        for (int i = 0; i < inventorySize - 1; i++)
        {
            if (inventory[i] == null)
            {
                break;
            }

            for (int j = i + 1; j < inventorySize; j++)
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
        for (int i = 0; i < inventorySize; i++)
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
        for (int i = 0; i < itemCount; i++)
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
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i] == null)
            {
                break;
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

        Vector3 center = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z);

        for (int i = 0; i < inventorySize; i++)
        {
            if (i < itemCount)
            {
                itemImages[i].gameObject.SetActive(true);

                float angle = (360 / itemCount) * i;

                Vector3 addLot = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0) * diameter;
                itemImages[i].gameObject.transform.position = addLot + center;
            }
            else
            {
                itemImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void UseItem(Item item)
    {

    }

    public void UseItem(int itemId)
    {

    }

    public string GetItemInfo()
    {
        return inventory[nowItem].name;
    }

}
