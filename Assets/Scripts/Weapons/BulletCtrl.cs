using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public int damage = 20;
    public float speed = 70.0f;

    private Rigidbody bulletRigid;

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

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
