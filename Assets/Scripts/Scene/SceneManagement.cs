    using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static int nextScene;

    public enum SceneList
    {
        SampleScene, TestScene1, TestScene2
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadScene(SceneList scene)
    {
        nextScene = (int) scene;

        SceneManager.LoadScene(1);
    }

    public void LoadScene(int scene)
    {
        nextScene = scene;

        SceneManager.LoadScene(1);
    }

    public void LoadSceneDirect(SceneList scene)
    {
        SceneManager.LoadScene((int)scene);
    }

    public void LoadSceneDirect(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
