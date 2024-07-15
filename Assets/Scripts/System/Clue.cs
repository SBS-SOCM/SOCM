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
        if (Input.anyKeyDown && isActive)
        {
            ClueOff();
        }
    }

    public void ActiveTrue()
    {
        isActive = true;
    }

    public void ClueOn()
    {
        if (isActive)
        {
            return;
        }

        Vector3 targetPos = player.transform.position + player.transform.forward * distance;
        Quaternion targetRot = player.transform.rotation;

        transform.DOMove(targetPos, duration);
        transform.DORotateQuaternion(targetRot, duration);
        Invoke("ActiveTrue", duration);
    }

    public void ClueOff()
    {
        isActive = false;

        transform.DOMove(startLoc, duration);
        transform.DORotateQuaternion(startRot, duration);
    }
}
