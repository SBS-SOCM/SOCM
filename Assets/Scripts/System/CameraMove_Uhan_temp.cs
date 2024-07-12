using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove_Uhan_temp : MonoBehaviour
{
    public Transform originPos;
    public Transform playerPos;

    RaycastHit hit;
    bool isWall = false;

    Vector3 hitPoint;

    Vector3 dir;
    float dist;
    private void Start()
    {

    }

    private void Update()
    {
        dir =  playerPos.position - originPos.position;
        dist = Vector3.Distance(originPos.position, playerPos.position);

        FindObejct();

        
    }



    private void FixedUpdate()
    {
        transform.DORotateQuaternion(playerPos.rotation, 0.1f);

        hitPoint = new Vector3(hitPoint.x, originPos.position.y, hitPoint.z);

        if (isWall)
        {
            transform.DOMove(hitPoint, 0.1f);

        }
        else
        {
            transform.DOMove(originPos.position, 0.1f);
        }
    }

    public void FindObejct()
    {
        
        
        if (Physics.Raycast(originPos.position, dir, out hit, dist, 1 << 7 ))
        {
            hitPoint = hit.point;
            isWall = true;
        }
        else
        {
            isWall = false;
        }


    }
}
