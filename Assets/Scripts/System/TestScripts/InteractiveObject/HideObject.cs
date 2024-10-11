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
        // 이 카메라에는 플레이어가 보이지 않게 설정
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
        // 카메라 전환
        hideCamera.SetActive(true);

        // 현재 상태 저장
        isInteracion = true;

        /*
        // 현재 위치 저장
        playerOriginVec = CharacterManager.instance.gameObject.transform.position;
        
        // 위치 이동
        CharacterManager.instance.gameObject.transform.position = hidePlayerVec;
        
        // 플레이어 보이지 않게 설정
        CharacterManager.instance.isVisible = false;
        */

        CharacterManager.instance.gameObject.SetActive(false);
    }

    public void InteractionOff()
    {
        // 카메라 전환
        hideCamera.SetActive(false);
        
        // 현재 상태 저장
        isInteracion = false;

        /*
        // 저장된 위치로 이동
        CharacterManager.instance.gameObject.transform.position = playerOriginVec;

        // 플레이어가 다시 보이게 설정
        CharacterManager.instance.isVisible = true;
        */

        CharacterManager.instance.gameObject.SetActive(true);

    }
}
