using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isLight;


    public void ExitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중인 경우
        EditorApplication.isPlaying = false;
#else
        // 빌드된 .exe 파일에서 실행 중인 경우
        Application.Quit();
#endif
    }
}
