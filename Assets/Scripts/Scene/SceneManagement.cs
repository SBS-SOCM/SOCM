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
    /// 0 : TitleScene / 1 : IntroScene / 2 : MainScene / 3 : LoadingScene / 4 : Ingame / 5 : SampleScene
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
        Debug.Log("Scene Move");

        // 마우스 커서를 해제하고 자유롭게 움직이게 함
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        nextScene = scene;

        SceneManager.LoadScene(3);
    }

    /// <summary>
    /// 로딩을 포함하지 않는 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    public void LoadSceneDirect(int scene)
    {
        Debug.Log("Scene Direct Move");

        // 마우스 커서를 해제하고 자유롭게 움직이게 함
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(scene);
    }
}
