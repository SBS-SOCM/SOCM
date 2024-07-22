using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionCtrl : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController noWeaponAnim;
    [SerializeField] RuntimeAnimatorController weaponAnim;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator _animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
        starterAssetsInputs = GetComponent<StarterAssets.StarterAssetsInputs>();
    }
    private void Update()
    {
        CheckWeaponType();
    }
    private void CheckWeaponType()
    {
        if (starterAssetsInputs.aim)
        {
            _animator.runtimeAnimatorController = weaponAnim;
        }
        else
        {
            _animator.runtimeAnimatorController = noWeaponAnim;
        }

    }

}
