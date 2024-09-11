using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionCtrl : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController noWeaponAnim;
    [SerializeField] RuntimeAnimatorController weaponAnim;


    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator _animator;

    //State

    private float targetAiming = 0f;
    private float aimingBlend = 0f;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    private void Update()
    {
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
        else
        {
            _animator.runtimeAnimatorController = noWeaponAnim;
            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            rigBuilder.enabled = false;
            targetAiming = 0f;
        }

        aimingBlend = Mathf.Lerp(aimingBlend, targetAiming, Time.deltaTime * 10f);
        _animator.SetFloat("Aiming", aimingBlend);
    }
    

}
