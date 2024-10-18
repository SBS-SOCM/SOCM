using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
    
public class UIManager : SerializedMonoBehaviour
{

    /// <summary>
    /// UI Panel들 / 0 : Setting / 1 : Inventory / 2 : Pause
    /// </summary>
    [TabGroup("System")] public GameObject[] UIPanels;
    [TabGroup("System")] public Inventory inventory;

    [TabGroup("System"), OdinSerialize] public Stack<int> UIStack = new Stack<int>();
    [TabGroup("System"), ReadOnly] public string nowSceneName;

    /// <summary>
    /// 인게임 내의 UI / 0 : now Item / 1 : before Item / 2 : next Item / 3 : HP Image / 4 : MP Image / 5 : MP Text / 6 : before 2 item / 7 : after 2 item
    ///                  8,9 : SkillIcon
    /// </summary>
    [TabGroup("Ingame")] public GameObject[] ingameUIObjects;

    /// <summary>
    /// 인게임 내의 패널들
    /// </summary>
    [TabGroup("Ingame")] public GameObject[] ingameUIPanels;

    [TabGroup("Ingame")] public Image hpImage;
    [TabGroup("Ingame")] public Image mpImage;

    [Range(0.0f, 0.75f), TabGroup("Ingame")] public float hpTest;
    [Range(0.0f, 0.75f), TabGroup("Ingame")] public float mpTest;


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        nowSceneName = scene.name;

        if (nowSceneName.StartsWith("Ingame"))
        {
            for (int i = 0; i < ingameUIPanels.Length; i++)
            {
                ingameUIPanels[i].SetActive(true);
            }

            Debug.Log("Mouse Off with Scene Load");
            CharacterManager.instance.OffMouseActive();
        }
        else
        {
            for (int i = 0; i < ingameUIPanels.Length; i++)
            {
                ingameUIPanels[i].SetActive(false);
            }
        }
    }


    void Start()
    {

    }
    void Update()
    {
        GetKeyboadInput();

        /// Only For Test
        if (SceneManager.GetActiveScene().name.StartsWith("Ingame"))
        {
            RenewConditionUI();
        }
        
        if (Input.GetKeyDown(KeyCode.F12))
        {
            OpenPuzzle();
        }

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
            if (inventory.itemCount == 0)
            {
                return;
            }

            inventory.nowItem = mod(inventory.nowItem + (int) wheelValue, inventory.itemCount);

            if (inventory.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                inventory.RenewInventoryUI();
            }

            CharacterManager.instance.SetItemType(inventory.inventory[inventory.nowItem].id);
            RenewItemUI();
        }
    }

    public void ESC()
    {
        if (UIStack.Count == 0)
        {
            if (!SceneManager.GetActiveScene().name.StartsWith("Ingame"))
            {
                return;
            }

            UIPanels[2].SetActive(true);
            OpenUI();
            UIStack.Push(2);
            
        }
        else
        {
            if (UIStack.Count == 1 && CharacterManager.instance.gameObject != null)
            {
                CharacterManager.instance.gameObject.GetComponent<CharacterManager>().OffMouseActive();
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
        if (CharacterManager.instance.gameObject == null)
        {
            
            return;
        }

        Debug.Log("Mouse On in Open UI");
        CharacterManager.instance.gameObject.GetComponent<CharacterManager>().OnMouseActive();
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

    public void RenewSkillIcon()
    {
        // 8 ,9


    }

    public void RenewItemUI()
    {
        
        int nowItem = mod(inventory.nowItem , inventory.itemCount);
        int beforeItem = mod(inventory.nowItem - 1, inventory.itemCount);
        int before2Item = mod(inventory.nowItem - 2, inventory.itemCount);
        int afterItem = mod(inventory.nowItem + 1, inventory.itemCount);
        int after2Item = mod(inventory.nowItem + 2, inventory.itemCount);

        Debug.Log("now : " + nowItem.ToString() + " before : " + beforeItem.ToString() + " after : " + afterItem.ToString());

        Debug.Log("Item/" + inventory.inventory[nowItem].name);
     
        /*
        ingameUIObjects[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[nowItem].name);
        ingameUIObjects[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[afterItem].name);
        ingameUIObjects[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[beforeItem].name);
        */

        if (inventory.itemCount > 3)
        {
            ingameUIObjects[7].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[after2Item].name);
        }
        
        if (inventory.itemCount > 4)
        {
            ingameUIObjects[6].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/" + inventory.inventory[before2Item].name);
        }
    }

    public void RenewConditionUI()
    {
        ingameUIObjects[3].GetComponent<Image>().fillAmount = hpTest;
        ingameUIObjects[4].GetComponent<Image>().fillAmount = mpTest;
        ingameUIObjects[5].GetComponent<Text>().text = ((int) (mpTest / 3 * 400)).ToString();

        if (mpTest < 0.075 )
        {
            ingameUIObjects[4].GetComponent<Image>().color = new Color(180/255f, 88 / 255f, 0);
        }
        else if (mpTest < 0.225)
        {
            ingameUIObjects[4].GetComponent<Image>().color = new Color(215 / 255f, 150 / 255f, 0);
        }
        else
        {
            ingameUIObjects[4].GetComponent<Image>().color = new Color(255 / 255f, 212 / 255f, 0);
        }
    }

    public void OpenPuzzle()
    {
        UIPanels[3].SetActive(true);
        UIStack.Push(3);
        OpenUI();
    }

    public IEnumerator OpenEnd()
    {
        UIPanels[4].SetActive(true);
        UIStack.Push(4);
        OpenUI();

        yield return new WaitForSeconds(2f);

        Singleton.instance.GetComponentInChildren<SceneManagement>().LoadSceneDirect(0);
        Singleton.instance.DistroySIngleton();
    }
}
