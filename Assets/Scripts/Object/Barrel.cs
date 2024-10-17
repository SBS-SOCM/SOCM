using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject hideCamera;   // 카메라 오브젝트
    public bool isInteracion;       // 상호작용 상태

    Vector3 playerOriginVec;        // 플레이어 원래 위치

    // 마우스 입력을 통한 카메라 회전에 필요한 변수들
    public float CameraAngleOverride = 0.0f;  // 카메라 각도 보정
    private float _cinemachineTargetYaw;    // 카메라 좌우 회전 값
    private const float _threshold = 0.01f; // 회전할 최소 움직임
    public bool LockCameraPosition = false; // 카메라 고정 여부

    private void Start()
    {
        // 초기화
        _cinemachineTargetYaw = hideCamera.transform.rotation.eulerAngles.y;

        // 마우스 커서 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 커서 숨기기
    }

    void Update()
    {
        if (isInteracion)
        {
            CameraRotation();  // 상호작용 중이면 카메라 회전
        }
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
        // 카메라 활성화
        hideCamera.SetActive(true);

        // 상호작용 상태 변경
        isInteracion = true;

        // 플레이어 비활성화 (숨기기)
        CharacterManager.instance.gameObject.SetActive(false);
    }

    public void InteractionOff()
    {
        // 카메라 비활성화
        hideCamera.SetActive(false);

        // 상호작용 상태 변경
        isInteracion = false;

        // 플레이어 다시 활성화 (보이기)
        CharacterManager.instance.gameObject.SetActive(true);
    }

    // 마우스 움직임에 따른 카메라 회전 함수 (좌우 회전만 적용)
    private void CameraRotation()
    {
        // 마우스 움직임 값 받기 (X축만 사용)
        float mouseX = Input.GetAxis("Mouse X");

        // 디버그 로그로 마우스 입력 확인
        Debug.Log("Mouse X: " + mouseX);

        // 마우스 입력이 없으면 카메라 회전 없음
        if (Mathf.Abs(mouseX) >= _threshold && !LockCameraPosition)
        {
            // 좌우 회전 (Yaw)만 적용
            _cinemachineTargetYaw += mouseX;

            // 각도 제한 설정은 필요 없음 (상하 움직임 없앰)
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);

            // 카메라 회전 적용 (Pitch 제거, Yaw만 적용)
            transform.GetChild(0).rotation = Quaternion.Euler(CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
    }

    // 각도 제한 함수
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
