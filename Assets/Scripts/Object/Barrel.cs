using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject hideCamera;   // ī�޶� ������Ʈ
    public bool isInteracion;       // ��ȣ�ۿ� ����

    Vector3 playerOriginVec;        // �÷��̾� ���� ��ġ

    // ���콺 �Է��� ���� ī�޶� ȸ���� �ʿ��� ������
    public float CameraAngleOverride = 0.0f;  // ī�޶� ���� ����
    private float _cinemachineTargetYaw;    // ī�޶� �¿� ȸ�� ��
    private const float _threshold = 0.01f; // ȸ���� �ּ� ������
    public bool LockCameraPosition = false; // ī�޶� ���� ����

    private void Start()
    {
        // �ʱ�ȭ
        _cinemachineTargetYaw = hideCamera.transform.rotation.eulerAngles.y;

        // ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Ŀ�� �����
    }

    void Update()
    {
        if (isInteracion)
        {
            CameraRotation();  // ��ȣ�ۿ� ���̸� ī�޶� ȸ��
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
        // ī�޶� Ȱ��ȭ
        hideCamera.SetActive(true);

        // ��ȣ�ۿ� ���� ����
        isInteracion = true;

        // �÷��̾� ��Ȱ��ȭ (�����)
        CharacterManager.instance.gameObject.SetActive(false);
    }

    public void InteractionOff()
    {
        // ī�޶� ��Ȱ��ȭ
        hideCamera.SetActive(false);

        // ��ȣ�ۿ� ���� ����
        isInteracion = false;

        // �÷��̾� �ٽ� Ȱ��ȭ (���̱�)
        CharacterManager.instance.gameObject.SetActive(true);
    }

    // ���콺 �����ӿ� ���� ī�޶� ȸ�� �Լ� (�¿� ȸ���� ����)
    private void CameraRotation()
    {
        // ���콺 ������ �� �ޱ� (X�ุ ���)
        float mouseX = Input.GetAxis("Mouse X");

        // ����� �α׷� ���콺 �Է� Ȯ��
        Debug.Log("Mouse X: " + mouseX);

        // ���콺 �Է��� ������ ī�޶� ȸ�� ����
        if (Mathf.Abs(mouseX) >= _threshold && !LockCameraPosition)
        {
            // �¿� ȸ�� (Yaw)�� ����
            _cinemachineTargetYaw += mouseX;

            // ���� ���� ������ �ʿ� ���� (���� ������ ����)
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);

            // ī�޶� ȸ�� ���� (Pitch ����, Yaw�� ����)
            transform.GetChild(0).rotation = Quaternion.Euler(CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
    }

    // ���� ���� �Լ�
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
