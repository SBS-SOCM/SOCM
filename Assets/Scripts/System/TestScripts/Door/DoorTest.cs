using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DoorTest : MonoBehaviour
{
    public float rootY;

    // Start is called before the first frame update
    void Start()
    {
        rootY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        if (transform.rotation.eulerAngles.y == rootY)
        {
            
            transform.DORotate(new Vector3(0, rootY + 120, 0), 1f);
            // transform.GetChild(i).gameObject.SetActive(false);
        }

        else
        {
            transform.DORotate(new Vector3(0, rootY, 0), 1f);
        }
        
    }
}
