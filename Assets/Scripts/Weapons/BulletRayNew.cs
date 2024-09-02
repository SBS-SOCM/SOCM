using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRayNew : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;

    public float moveSpeed = 200.0f;

    private Vector3 targetPosition;

    public void SetUp(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public void Update()
    {
        float distanceBefore = Vector3.Distance(transform.position, targetPosition);

        Vector3 moveDir = (targetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        if(distanceBefore < distanceAfter)
        {
            Instantiate(vfxHit, targetPosition, Quaternion.identity);
            //transform.Find("Trail").SetParent(null);
            Destroy(gameObject);
        }

    }

}
