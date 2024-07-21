using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrapTest : MonoBehaviour
{
    public Vector3 MoveDir;
    [Range(0.1f, 5f)] public float speed;

    void Start()
    {

    }

    void Update()
    {

    }

    public void TrapOn()
    {
        transform.DOMove(transform.position + MoveDir, speed);
    }

    public void TrapOff()
    {
        transform.DOMove(transform.position - MoveDir, speed*5);
    }
}
