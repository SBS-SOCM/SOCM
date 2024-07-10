using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    private StarterAssets.ThirdPersonController thirdPersonController;
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
    }

    private void Update()
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            //thirdPersonController.SetRotateOnMove(false);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            //thirdPersonController.SetRotateOnMove(false);
        }
    }

}
