using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{

    public GameObject hideCamera;

    public bool isInteracion;

    public Transform hidePlayerPos;

    Vector3 hidePlayerVec;

    Vector3 playerOriginVec;
    

    void Start()
    {
        // �� ī�޶󿡴� �÷��̾ ������ �ʰ� ����
//        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        hidePlayerVec = hidePlayerPos.position;
    }

   
    void Update()
    {
        
    }

    public void InteractionSend()
    {
        if (!isInteracion)
        {
            InteractionOn();
        }
        else
        {
            InteractionOff();
        }
    }

    public void InteractionOn()
    {
        // ī�޶� ��ȯ
        hideCamera.SetActive(true);

        // ���� ���� ����
        isInteracion = true;

        /*
        // ���� ��ġ ����
        playerOriginVec = CharacterManager.instance.gameObject.transform.position;
        
        // ��ġ �̵�
        CharacterManager.instance.gameObject.transform.position = hidePlayerVec;
        
        // �÷��̾� ������ �ʰ� ����
        CharacterManager.instance.isVisible = false;
        */

        CharacterManager.instance.gameObject.SetActive(false);
    }

    public void InteractionOff()
    {
        // ī�޶� ��ȯ
        hideCamera.SetActive(false);
        
        // ���� ���� ����
        isInteracion = false;

        /*
        // ����� ��ġ�� �̵�
        CharacterManager.instance.gameObject.transform.position = playerOriginVec;

        // �÷��̾ �ٽ� ���̰� ����
        CharacterManager.instance.isVisible = true;
        */

        CharacterManager.instance.gameObject.SetActive(true);

    }
}
