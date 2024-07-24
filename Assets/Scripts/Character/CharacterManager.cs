using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    //Character State
    public int hp = 5;
    public bool isVisible = true;
    public bool isSilence = false;
    private float visibleModeTime = 10.0f;
    public float willPower = 100.0f;
    private float visibleSKillCool = 5.0f;
    private float visibleReturnTime = 2.0f;
    private float characterHP = 100.0f;
    private float increaseWillPowerTime = 60.0f;

 
    public List<Transform> Enemies = new List<Transform>();
    public LayerMask targetMask; // 적을 검색하기 위한 레이어마스크
    public LayerMask walllMask; // 장애물 마스크
    public float checkRange = 10.0f;

    //UI
    [SerializeField] private Image willPowerImage;
    [SerializeField] private Image visibleSkillImage;

    //
    [SerializeField] private Material characterMat;
    [SerializeField] private Material invisibleMat;
    [SerializeField] private GameObject characterGO;
    

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isVisible && visibleSKillCool <= 0.0f)
        {
            OffVisible();
        }
        if (Input.GetKeyDown(KeyCode.R) && !isVisible && visibleReturnTime <= 0.0f)
        {
            visibleSKillCool = 20.0f;
            OnVisible();
        }


        CheckEnemy();
        SkillUiUpdate();
        VisibleModeChheck();
        willPowerImage.fillAmount = willPower / 100.0f;
        increaseWillPowerTime -= Time.deltaTime;
        if (increaseWillPowerTime <= 0.0f)
        {
            willPower += 10.0f;
            increaseWillPowerTime = 60.0f;
        }
    }
    public void OnVisible()
    {
        isVisible = true;
    }
    public void OffVisible()
    {
        isVisible = false;
        visibleReturnTime = 2.0f;
    }
    private void SkillUiUpdate()
    {
        visibleSKillCool -= Time.deltaTime;
        if (visibleSKillCool <= 0.0f) visibleSKillCool = 0.0f;
        visibleSkillImage.fillAmount = visibleSKillCool/20.0f;
    }
    private void VisibleModeChheck()
    {
        visibleReturnTime -= Time.deltaTime;
        if (!isVisible)
        {
            visibleModeTime -= Time.deltaTime;
            if(visibleModeTime <= 0.0f)willPower -= Time.deltaTime;
        }
        else visibleModeTime = 10.0f;

        if (isVisible) characterGO.GetComponent<SkinnedMeshRenderer>().material = characterMat;
        else characterGO.GetComponent<SkinnedMeshRenderer>().material = invisibleMat;
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



}
