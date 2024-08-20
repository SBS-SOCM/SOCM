using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    /// <summary>
    /// 0 : SampleScene / 1 : TitleScene / 2 : MainScene / 3 : LoadingScene / 4 : IntroScene / 5 : Ingame_test
    /// </summary>

    public static int nextScene;

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
    public void LoadScene(int scene)
    {
        nextScene = scene;

        SceneManager.LoadScene(3);
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
