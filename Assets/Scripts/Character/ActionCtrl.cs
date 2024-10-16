using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionCtrl : MonoBehaviour
{
    //[SerializeField] RuntimeAnimatorController noWeaponAnim;
    [SerializeField] RuntimeAnimatorController weaponAnim;
    [SerializeField] private Transform vfxBlood;
    public LayerMask enemyMask;
    public LayerMask wallMask;

    public List<Transform> Enemies = new List<Transform>();
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator _animator;

    private float stabbingRange = 1f;

    //State
    private float targetAiming = 0f;
    private float aimingBlend = 0f;
    public static int weaponType = 0;

    //Mission
    [SerializeField] private LayerMask missionLayer;

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

        CheckEnemy();
        CheckMission();

        //stabbingWeightBlend = Mathf.Lerp(stabbingWeightBlend, isActiveStabbing ? 1f : 0f, Time.deltaTime * 100f);
        stabbingWeightBlend = isActiveStabbing ? 0.5f : 0f;

    }
    private void CheckMission()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if(hit.collider.gameObject.CompareTag("Mission"))
                {
                    Mission mission = hit.collider.gameObject.GetComponent<Mission>();
                    if (mission.active)
                    {
                        MissionManager.instance.clearMissionNum = mission.missionNum;
                        mission.active = false;
                    }
                    
                }
            }
        }
    }
    private void CheckEnemy()
    {
        Enemies.Clear();
        Collider[] results = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, stabbingRange, results, enemyMask);

        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;

            Vector3 dirToTarget = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 90 / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, enemy.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, wallMask))
                {
                    Enemies.Add(enemy);
                }
            }
        }
        if (Enemies.Count != 0)
        {
            if (Enemies[0].GetComponent<MonsterCtrl>() != null && !Enemies[0].GetComponent<MonsterCtrl>().isDie)
            {
                Enemies[0].GetComponent<MonsterCtrl>().StabbingCtrl();
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    //Stabbing Hand IK
                    if (Enemies[0].GetComponent<MonsterCtrl>() != null && !Enemies[0].GetComponent<MonsterCtrl>().isDie)
                    {
                        isActiveStabbing = true;
                        var monster = Enemies[0].GetComponent<MonsterCtrl>();
                        stabbingTarget = monster.neckPos;
                        StartCoroutine(StabbingHandIK(monster));
                    }
                }
            }
        }
    }

    bool isActiveStabbing = false;
    private Transform stabbingTarget;
    private float stabbingWeightBlend = 0f;

    private void OnAnimatorIK(int layerIndex)
    {
        /*if (isActiveStabbing)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.2f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.2f);

            _animator.SetIKPosition(AvatarIKGoal.RightHand, stabbingTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.forward));
        }*/
    }

    private IEnumerator StabbingHandIK(MonsterCtrl monster)
    {
        //_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        //_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        //_animator.SetIKPosition(AvatarIKGoal.RightHand, monster.neckPos.position);
        //_animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.forward));

        CharacterManager.instance.willPower -= 5.0f;
        StartCoroutine(Assasinate(monster));
        yield return new WaitForSeconds(0.4f);
        Instantiate(vfxBlood, monster.neckPos);

        yield return new WaitForSeconds(1.2f);
        isActiveStabbing = false;
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);


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
            RigBuilder rigBuilder = GetComponent<RigBuilder>();
            rigBuilder.enabled = false;
            targetAiming = 0f;
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
    private IEnumerator Assasinate(MonsterCtrl monster)
    {
        CharacterManager.instance.canMove = false;
        _animator.SetTrigger("Stabbing");
        yield return new WaitForSeconds(0.2f);
        monster.monsterHP = 0;
        yield return new WaitForSeconds(1.0f);
        CharacterManager.instance.canMove = true;
    }
    private void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Pistol
        {
            weaponType = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Rifle
        {
            weaponType = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // Special
        {
            weaponType = 2;
        }
    }


}
