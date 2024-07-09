using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour
{
    public GameObject player;

    Vector3 startLoc;
    Quaternion startRot;

    public float distance;
    public float duration;

    public bool isActive;
    void Start()
    {
        startLoc = transform.position;
        startRot = transform.rotation;

        isActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ClueOn();
        }
        else if (Input.anyKeyDown)
        {
            ClueOff();
        }
    }

    void ClueOn()
    {
        Vector3 targetPos = player.transform.position + player.transform.forward * distance;
        Quaternion targetRot = player.transform.rotation;

        transform.DOMove(targetPos, duration);
        transform.DORotateQuaternion(targetRot, duration);
    }

    void ClueOff()
    {
        transform.DOMove(startLoc, duration);
        transform.DORotateQuaternion(startRot, duration);
    }
}
