using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;
    public int clearMissionNum = 0;
    [SerializeField] private GameObject[] missions;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        MissionCheck();
    }
    private void MissionCheck()
    {
        switch (clearMissionNum)
        {
            case 0:
                missions[0].active = true;
                break;
            case 1:
                missions[1].active = true;
                break;
            case 2:
                missions[2].active = true;
                break;
            case 3:
                missions[3].active = true;
                break;
        }
    }

}
