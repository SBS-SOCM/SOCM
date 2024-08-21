using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletRay : MonoBehaviour
{
    public float speed = 70.0f;
    public float rayDist = 5f;

    private Rigidbody bulletRigid;
    private RaycastHit hit;

    private void Awake()
    {
        bulletRigid = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        bulletRigid.velocity = transform.forward * speed;

        Destroy(gameObject, 4.0f);
    }
    private void Update()
    {
        CheckRay();
    }

    private void CheckRay()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDist))
        {
            if(hit.transform.name == "Head")
            {
                hit.transform.GetComponentInParent<MonsterCtrl>().monsterHP -= 2;
            }
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<MonsterCtrl>().monsterHP -= 1;
            }
            Destroy(this.gameObject);
            
        }

    }



}
