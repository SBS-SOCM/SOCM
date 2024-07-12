using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// UI Panel들 / 0 : Setting / 1 : Inventory / 2 : Pause
    /// </summary>
    public GameObject[] UIPanels;

    /// <summary>
    /// 인게임 내의 UI / 0 : now Item , 1 : before Item , 2 : next Item
    /// </summary>
    public GameObject[] ingameUIPanels;

    Stack<int> UIStack = new Stack<int>();

    public Inventory inventory;
    void Start()
    {
#if !UNITY_EDITOR
        // disable WebGLInput.stickyCursorLock so if the browser unlocks the cursor (with the ESC key) the cursor will unlock in Unity
        WebGLInput.stickyCursorLock = false;
#endif
    }
    void Update()
    {
        GetKeyboadInput();
    }

    public void GetKeyboadInput()
    {
        float wheelValue = Input.GetAxis("Mouse ScrollWheel") * 10;
        wheelValue = Mathf.Clamp(wheelValue, -1, 1);

        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESC();
        }
        else if (wheelValue != 0)
        {
            if (inventory.ItemCount == 0)
            {
                return;
            }

            inventory.nowItem = mod(inventory.nowItem + (int) wheelValue, inventory.ItemCount);

            if (inventory.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                inventory.RenewInventoryUI();
            }
            
            RenewItemUI();

        }
    }

    public void ESC()
    {
        Debug.Log(UIStack.Count);

        if (UIStack.Count == 0)
        {
            UIPanels[2].SetActive(true);
            OpenUI();
            UIStack.Push(2);
        }
        else
        {
            if (UIStack.Count == 1)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            UIPanels[UIStack.Pop()].SetActive(false);
        }

        if (UIPanels[2].activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void OpenUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public void OpenInventory()
    {
        if (UIPanels[1].activeSelf)
        {
            ESC();
        }
        else
        {
            UIPanels[1].SetActive(true);
            OpenUI();
            UIStack.Push(1);
        }
    }

    public void ChangeItem(InputValue value)
    {
        float wheelValue = value.Get<float>();

        if (wheelValue != 0)
        {
            Debug.Log(wheelValue);
        }
    }

    public void RenewIngameUI()
    {
        
    }

    public void OpenSettingUI()
    {
        OpenUI();
        UIStack.Push(0);

        UIPanels[0].SetActive(true);
    }

    public void RenewItemUI()
    {
        
        int nowItem = mod(inventory.nowItem , inventory.ItemCount);
        int beforeItem = mod(inventory.nowItem - 1, inventory.ItemCount);
        int afterItem = mod(inventory.nowItem + 1, inventory.ItemCount);

        Debug.Log("now : " + nowItem.ToString() + " before : " + beforeItem.ToString() + " after : " + afterItem.ToString());

        Debug.Log("Item/" + inventory.inventory[nowItem].name);

        ingameUIPanels[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[nowItem].name);
        ingameUIPanels[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[beforeItem].name);
        ingameUIPanels[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[afterItem].name);
    }
}
