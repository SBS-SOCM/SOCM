using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform vfxHitZombie;
    [SerializeField] private Transform vfxHit;


    public float rayHitDistance = 0.1f;
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

    // Update is called once per frame
    void FixedUpdate()
    {
        //DrawBulletRay();
    }

    /*private RaycastHit bulletRayHit;
    private void DrawBulletRay()
    {
        Debug.DrawRay(transform.position, transform.forward * rayHitDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out bulletRayHit, rayHitDistance))
        {

            Destroy(this.gameObject);
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }


}
