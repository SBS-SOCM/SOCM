using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    Color[] tempColor;

    public GameObject[] chapters;

    public GameObject chapterSelectPanel;

    void Start()
    {
        tempColor = new Color[3];

        tempColor[0] = chapters[0].GetComponent<Image>().color; 
        tempColor[1] = chapters[1].GetComponent<Image>().color; 
        tempColor[2] = chapters[2].GetComponent<Image>().color; 
    }

    private void Update()
    {
        GetKeyboardInput();
    }

    public void NextChpater()
    {
        Color temp = chapters[0].GetComponent<Image>().color;
        chapters[0].GetComponent<Image>().color = chapters[1].GetComponent<Image>().color;
        chapters[1].GetComponent<Image>().color = chapters[2].GetComponent<Image>().color;
        chapters[2].GetComponent<Image>().color = temp;
    }


    public void BeforeChpater()
    {
        Color temp = chapters[1].GetComponent<Image>().color;
        chapters[1].GetComponent<Image>().color = chapters[0].GetComponent<Image>().color;
        chapters[0].GetComponent<Image>().color = chapters[2].GetComponent<Image>().color;
        chapters[2].GetComponent<Image>().color = temp;
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

}
