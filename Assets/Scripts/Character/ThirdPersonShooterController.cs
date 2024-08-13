using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    private float aimingTime = 5.0f;
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
    }

    private void Update()
    {
        if (starterAssetsInputs.aim && CharacterManager.instance.hp > 0)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            aimingTime -= Time.deltaTime;
            if(aimingTime <= 0.0f)
            {
                // Decrease WillPower
                CharacterManager.instance.willPower -= 1.0f;
                aimingTime = 5.0f;
            }
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            aimingTime = 5.0f;
        }
        AimIK();
    }
    private void AimIK()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
        }
    }

}
