using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DoorTest : MonoBehaviour
{
    public float rootY;
    public bool isAction;

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
        if (isAction)
        {
            return;
        }

        if (transform.rotation.eulerAngles.y == rootY)
        {

            isAction = true;
            transform.DORotate(new Vector3(0, rootY + 120, 0), 1f).OnComplete(EndAction);
            // transform.GetChild(i).gameObject.SetActive(false);
        }

        else
        {
            isAction = true;
            transform.DORotate(new Vector3(0, rootY, 0), 1f).OnComplete(EndAction);
        }
        
    }

    public void EndAction()
    {
        isAction = false;
    }
}
