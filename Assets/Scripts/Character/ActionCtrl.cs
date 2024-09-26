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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckEnemy();
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
        if(Enemies.Count != 0)
        {
            if(Enemies[0].GetComponent<MonsterCtrl>() != null && !Enemies[0].GetComponent<MonsterCtrl>().isDie)
            {
                StartCoroutine(Assasinate(Enemies[0].GetComponent<MonsterCtrl>()));
            }
            
            
        }
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
    private IEnumerator Assasinate(MonsterCtrl monster)
    {
        CharacterManager.instance.canMove = false;
        _animator.SetTrigger("Stabbing");
        yield return new WaitForSeconds(1.0f);
        monster.monsterHP = 0;
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
