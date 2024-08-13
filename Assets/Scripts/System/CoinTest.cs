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
            if(Singleton.instance.GetComponentInChildren<Inventory>().GetItemInfo().Equals("coin"))
            {

            }

        }
    }
}
