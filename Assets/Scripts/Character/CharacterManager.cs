using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    //Character State
    public bool isVisible = true;
    public bool isSilence = false;
    private float visibleModeTime = 10.0f;
    private float willPower = 100.0f;
 
    public List<Transform> Enemies = new List<Transform>();
    public LayerMask targetMask; // 적을 검색하기 위한 레이어마스크
    public LayerMask walllMask; // 장애물 마스크
    public float checkRange = 10.0f;

    //UI
    [SerializeField] private Image willPowerImage;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        CheckEnemy();
        VisibleModeChheck();
        willPowerImage.fillAmount = willPower / 100.0f;
    }
    private void VisibleModeChheck()
    {
        if (isVisible)
        {
            visibleModeTime -= Time.deltaTime;
            if(visibleModeTime <= 0.0f)willPower -= Time.deltaTime;
        }
        else visibleModeTime = 10.0f;
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
