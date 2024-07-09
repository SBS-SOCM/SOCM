using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

    /// <summary>
    /// 로딩을 포함한 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(SceneList scene)
    {
        nextScene = (int) scene;

        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// 로딩을 포함한 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(int scene)
    {
        nextScene = scene;

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 로딩을 포함하지 않는 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    /// 
    public void LoadSceneDirect(SceneList scene)
    {
        SceneManager.LoadScene((int)scene);
    }
    /// <summary>
    /// 로딩을 포함하지 않는 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    public void LoadSceneDirect(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
