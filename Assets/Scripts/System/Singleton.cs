using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour
{
    public static Singleton instance;
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }


    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player != null)
        {
            return;
        }

        if (scene.name.StartsWith("Ingame") || scene.name.StartsWith("SampleScene"))
        {
            player = GameObject.Find("PlayerArmature");
        }
    }


}


