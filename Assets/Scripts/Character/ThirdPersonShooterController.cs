using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private GameObject bulletGO;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private AudioClip fireSound;

    private AudioSource audioSource;
    private float aimingTime = 5.0f;
    private float fireTerm = 0.5f;
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        fireTerm -= Time.deltaTime;
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

        if(Input.GetMouseButtonDown(0) && fireTerm <= 0.0f && starterAssetsInputs.aim) Shoot();
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
    private void Shoot()
    {
        fireTerm = 0.5f;
        audioSource.PlayOneShot(fireSound);
        Instantiate(bulletGO, bulletSpawnPos.position, bulletSpawnPos.rotation);
    }

}
