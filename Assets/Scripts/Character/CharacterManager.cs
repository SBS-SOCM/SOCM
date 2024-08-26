using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.Windows;

public partial class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    //Character State
    public float hp = 100.0f;
    public bool isVisible = true;
    public bool isSilence = false;
    public bool isUsingVisibleSkill = false;
    private float visibleModeTime = 10.0f;
    public float willPower = 100.0f;
    private float visibleSKillCool = 1.0f;
    private float visibleReturnTime = 2.0f;
    private float increaseWillPowerTime = 60.0f;
    public bool isCharacterDie = false;
    private StarterAssetsInputs _input;
    public bool isMoving = false;
    public bool isFire = false;

    public List<Transform> Enemies = new List<Transform>();
    public LayerMask targetMask; // 적을 검색하기 위한 레이어마스크
    public LayerMask walllMask; // 장애물 마스크
    public float checkRange = 10.0f;

    //UI
    [SerializeField] private Image willPowerImage;
    [SerializeField] private Image hpImage;
    [SerializeField] private Image visibleSkillImage;
    [SerializeField] private Text willPowerText;


    //
    [SerializeField] private Material characterMat;
    [SerializeField] private Material invisibleMat;
    [SerializeField] private GameObject characterGO;
     

    private void Awake()
    {
        instance = this;
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (hp <= 0) CharacterDie();

        //스킬 사용
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && isUsingVisibleSkill && visibleSKillCool <= 0.0f)
        {
            OffVisible();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.R) && isUsingVisibleSkill && visibleReturnTime <= 0.0f)
        {
            visibleSKillCool = 20.0f;
            OnVisible();
        }

        CharacterMoveCheck();
        CheckEnemy();
        SkillUiUpdate();
        VisibleModeCheck();

        willPowerText.text = Mathf.RoundToInt(willPower).ToString();
        willPowerImage.fillAmount = (willPower / 100.0f) * 0.75f;
        hpImage.fillAmount = (hp / 100.0f) * 0.75f;
        if (willPower >= 100.0f) willPower = 100.0f;

        increaseWillPowerTime -= Time.deltaTime;
        if (increaseWillPowerTime <= 0.0f)
        {
            willPower += 10.0f;
            increaseWillPowerTime = 60.0f;
        }
    }
    private void CharacterMoveCheck()
    {

        if (_input.move == Vector2.zero)
        {
            isMoving = false;
        }
        else isMoving = true;
    }
    private void CharacterDie()
    {
        isCharacterDie = true;
    }
    public void OnVisible()
    {
        isUsingVisibleSkill = false;
        isUsingVisibleSkill = true;
    }
    public void OffVisible()
    {
        isVisible = false;
        isUsingVisibleSkill = true;
        visibleReturnTime = 2.0f;
    }
    private void SkillUiUpdate()
    {
        visibleSKillCool -= Time.deltaTime;
        if (visibleSKillCool <= 0.0f) visibleSKillCool = 0.0f;
        visibleSkillImage.fillAmount = visibleSKillCool/20.0f;
    }
    private void VisibleModeCheck()
    {
        visibleReturnTime -= Time.deltaTime;
        if (!isVisible)
        {
            visibleModeTime -= Time.deltaTime;
            if(visibleModeTime <= 0.0f)willPower -= Time.deltaTime;
        }
        else visibleModeTime = 10.0f;

        if (isUsingVisibleSkill) characterGO.GetComponent<SkinnedMeshRenderer>().material = characterMat;
        else if (isUsingVisibleSkill) characterGO.GetComponent<SkinnedMeshRenderer>().material = invisibleMat;
    }

    private void CheckEnemy()
    {
        Enemies.Clear();
        Collider[] results = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, checkRange, results, targetMask);

        for (int i = 0; i < size; ++i)
        {
            Transform enemy = results[i].transform;

            Vector3 dirToTarget = (enemy.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 90 / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, enemy.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, walllMask))
                {
                    if (CharacterManager.instance.isVisible)
                    {
                        Enemies.Add(enemy);
                    }
                }
            }
        }
        if(Enemies.Count > 0)
        {
            willPower -= Time.deltaTime * Enemies.Count * 0.5f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            isVisible = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            isVisible = true;
        }
    }



}
