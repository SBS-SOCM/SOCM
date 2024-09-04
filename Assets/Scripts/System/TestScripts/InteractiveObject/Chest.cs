using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Chest : MonoBehaviour
{
    public float rootZ;
    public Vector3 rootVec;
    public bool isAction;

    public bool isItem;
    public float distanceForward;
    public float distanceRight;
    public float distanceDown;

    public GameObject item;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rootVec = transform.rotation.eulerAngles;
        rootZ = transform.rotation.eulerAngles.z;

        isItem = true;
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

        StartCoroutine(GetItem());

    }

    public IEnumerator GetItem()
    {
        isAction = true;

        
        transform.DORotate(new Vector3(0, 0, rootZ + 60) + rootVec, 1f);
        // transform.GetChild(i).gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // 아이템 획득 하는 코드 추가
        ItemOn();

        yield return new WaitForSeconds(1.5f);

        ItemOff();

        yield return new WaitForSeconds(1f);

        item.SetActive(false);
        isItem = false;

        
        transform.DORotate(new Vector3(0, 0 , rootZ) + rootVec, 1f);

        yield return new WaitForSeconds(1f);
        isAction = false;
        
    }

    public void ItemOn()
    {
        Vector3 targetPos = player.transform.position + player.transform.forward * distanceForward + new Vector3(1,1,0);
        Quaternion targetRot = player.transform.rotation;

        item.transform.DOMove(targetPos, 1f);
        item.transform.DORotateQuaternion(targetRot, 1f);
        Invoke("ActiveTrue", 1f);
    }

    public void ItemOff()
    {
        Vector3 targetPos = player.transform.position + player.transform.right * distanceRight + transform.transform.up * -1 * distanceDown;
        Quaternion targetRot = player.transform.rotation;

        item.transform.DOMove(targetPos, 1f);
        item.transform.DORotateQuaternion(targetRot, 1f);
    }
}
