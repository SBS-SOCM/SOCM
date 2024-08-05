using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    private float aimingTime = 5.0f;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
    }

    private void Update()
    {
        Shoot();
        if (starterAssetsInputs.aim)
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
    }

    private void Shoot()
    {
        if (starterAssetsInputs.aim)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }
        }
    }
    private void Fire()
    {
        Debug.Log("Fire");
        Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
    }
}
