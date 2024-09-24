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
    [SerializeField] private GameObject bulletSpecialGO;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private Transform gunFireSpawnPos;
    [SerializeField] private GameObject gunFire;
    [SerializeField] private Transform vfxBlood;
    [SerializeField] private Transform vfxGroundHit;

    private AudioSource audioSource;
    private float aimingTime = 5.0f;
    private float fireTerm = 0.5f;
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    public bool isForceAim;

    Vector3 mouseWorldPositon;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        mouseWorldPositon = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPositon = raycastHit.point;
        }
        if (isForceAim)
        {
            starterAssetsInputs.aim = true;
        }

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
        //AimIK();

        if(Input.GetMouseButtonDown(0) && fireTerm <= 0.0f && starterAssetsInputs.aim) StartCoroutine(Shoot(ActionCtrl.weaponType));
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
    private IEnumerator Shoot(int weaponType)
    {
        Vector3 aimDir = (mouseWorldPositon - bulletSpawnPos.position).normalized;
        switch (weaponType)
        {
            case 0: // Hand
                break;
            case 1: // Knife
                break;
            case 2: // Pistol
                CharacterManager.instance.isFire = true;
                fireTerm = 0.5f;
                audioSource.PlayOneShot(fireSound);
                Instantiate(gunFire, gunFireSpawnPos.position, gunFireSpawnPos.rotation);
                GameObject bulletPistol = Instantiate(bulletGO, bulletSpawnPos.position, Quaternion.LookRotation(aimDir, Vector3.up));

                yield return new WaitForSeconds(0.1f);
                CharacterManager.instance.isFire = false;
                break;
            case 3: // Rifle
                CharacterManager.instance.isFire = true;
                fireTerm = 0.2f;
                audioSource.PlayOneShot(fireSound);
                Instantiate(gunFire, gunFireSpawnPos.position, gunFireSpawnPos.rotation);
                GameObject bulletRifle = Instantiate(bulletGO, bulletSpawnPos.position, Quaternion.LookRotation(aimDir, Vector3.up));

                yield return new WaitForSeconds(0.1f);
                CharacterManager.instance.isFire = false;
                break;
            case 4: // Special
                CharacterManager.instance.isFire = true;
                fireTerm = 0.5f;
                GameObject bulletSpecial = Instantiate(bulletSpecialGO, bulletSpawnPos.position, Quaternion.LookRotation(aimDir, Vector3.up));

                yield return new WaitForSeconds(0.1f);
                CharacterManager.instance.isFire = false;
                break;
        }

        
    }

}
