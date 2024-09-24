using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionCtrl : MonoBehaviour
{
    //[SerializeField] RuntimeAnimatorController noWeaponAnim;
    [SerializeField] RuntimeAnimatorController weaponAnim;


    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator _animator;

    //State

    private float targetAiming = 0f;
    private float aimingBlend = 0f;
    public static int weaponType = 0;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    private void Update()
    {
        _animator.runtimeAnimatorController = weaponAnim;
        WeaponChange();
        CheckWeaponType();
    }

    private void CheckWeaponType()
    {
        if (starterAssetsInputs.aim && CharacterManager.instance.hp > 0)
        {
            _animator.runtimeAnimatorController = weaponAnim;
            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            rigBuilder.enabled = true;
            targetAiming = 1f;
        }
        /*else
        {
            _animator.runtimeAnimatorController = noWeaponAnim;
            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            rigBuilder.enabled = false;
            targetAiming = 0f;
        }*/

        aimingBlend = Mathf.Lerp(aimingBlend, targetAiming, Time.deltaTime * 10f);
        _animator.SetFloat("Aiming", aimingBlend);
    }
    private void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Hand
        {
            weaponType = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Knife
        {
            weaponType = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Pistol
        {
            weaponType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // Rifle
        {
            weaponType = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // Special
        {
            weaponType = 4;
        }
    }


}
