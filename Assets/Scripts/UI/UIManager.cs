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
    /// UI Panel들 / 0 : Setting / 1 : Inventory
    /// </summary>
    public GameObject[] UIPanels;

    /// <summary>
    /// 인게임 내의 UI / 0 : now Weapon , 1 : before Weapon , 2 : next Weapon
    /// </summary>
    public GameObject[] ingameUIPanels;

    Stack<int> UIStack = new Stack<int>();

    void Start()
    {

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
            Debug.Log(wheelValue);
        }
    }

    public void ESC()
    {
        if (UIStack.Count == 0)
        {

        }
        else
        {
            if (UIStack.Count == 1)
            {
                Time.timeScale = 1f;
            }

            UIPanels[UIStack.Pop()].SetActive(false);
        }
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
            UIStack.Push(1);
        }
    }

    public void ChangeWeapon(InputValue value)
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
        UIStack.Push(0);

        UIPanels[0].SetActive(true);
    }
}
