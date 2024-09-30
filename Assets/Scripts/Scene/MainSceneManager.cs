using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public GameObject[] chapters;
    public string[] missionMapRoot;
    public string[] missionExplain;

    public GameObject chapterSelectPanel;
    public Text missonExplainText;
    public Text chapText;

    [HideInInspector]
    public int nowChap;

    void Start()
    {
        // chapters[0].transform.Find("Map").GetComponent<Image>().sprite = "";
        missonExplainText.text = missionExplain[nowChap];

    }

    private void Update()
    {
        GetKeyboardInput();
    }

    public void NextChpater()
    {
        nowChap = mod(nowChap+1, missionExplain.Length);

        missonExplainText.text = missionExplain[nowChap];
        chapText.text = String.Format("Chapter {0:D2}.",nowChap+1);
    }


    public void BeforeChpater()
    {
        nowChap = mod(nowChap - 1, missionExplain.Length);

        missonExplainText.text = missionExplain[nowChap];
        chapText.text = String.Format("Chapter {0:D2}.", nowChap + 1);
    }

    public void OpenChapterSelectPanel()
    {
        chapterSelectPanel.SetActive(true);
    }

    public void CloseChapterSelectPanel()
    {
        chapterSelectPanel.SetActive(false);
    }

    void GetKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (chapterSelectPanel.activeSelf)
            {
                CloseChapterSelectPanel();
            }
        }
    }

    public void StartGame()
    {
        GetComponent<SceneManagement>().LoadScene(4);
    }
    int mod(int x, int m)
    {
        if (m == 0)
        {
            Debug.Log("Error : Devide with Zero");
            return 0;
        }

        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
