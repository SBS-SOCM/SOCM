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
        // ����Ƽ �����Ϳ��� ���� ���� ���
        EditorApplication.isPlaying = false;
#else
        // ����� .exe ���Ͽ��� ���� ���� ���
        Application.Quit();
#endif
    }
}
