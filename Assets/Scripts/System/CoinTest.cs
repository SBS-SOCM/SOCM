using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTest : MonoBehaviour
{
    public GameObject coin;

    [Range(0.0f, 20f)] public float power;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject coinObj = Instantiate(coin, Singleton.instance.GetComponentInChildren<Interaction>().player.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            coinObj.GetComponent<Rigidbody>().AddForce(Singleton.instance.GetComponentInChildren<Interaction>().player.transform.forward * power, ForceMode.Impulse);
        }
    }
}
