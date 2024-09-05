using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletCtrl : MonoBehaviour
{
    [SerializeField] GameObject bulletHitPrefab;
    [SerializeField] Transform vfxBlood;

    public float speed = 70.0f;
    public float rayDist = 5f;
    public float damage = 10.0f;

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
            if (hit.transform.CompareTag("Player"))
            {
                Instantiate(vfxBlood, hit.point, Quaternion.identity);
                CharacterManager.instance.hp -= damage;
            }

            Destroy(this.gameObject);
        }

    }
}
