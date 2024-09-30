using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Matching : MonoBehaviour
{
    [TabGroup("Array")] public GameObject[] leftBtnBG;
    [TabGroup("Array")] public GameObject[] rightBtnBG;

    [TabGroup("Array")] public GameObject[] leftBtn;
    [TabGroup("Array")] public GameObject[] rightBtn;

    [TabGroup("Array")] public GameObject[] leftPos;
    [TabGroup("Array")] public GameObject[] rightPos;

   
    public int[] nowClicked;

    // 0: 왼쪽 / 1 : 오른쪽
    public bool[] isClick;

    public Transform linesParent;

    public GameObject linePrefab;

    public int slotCount;

    // 정답 테이블
    [TabGroup("Answer")] public int[] answerTable;

    // 정답 입력 가능 횟수
    [TabGroup("Answer")] public int answerChance;

    // 왼쪽과 오른쪽의 몇번쨰가 몇번쨰와 연결되어 있는지 확인하는 테이블
    public int[] matchingTable;
    
    // 각 칸이 이미 매칭되서 선이 존재하는지 여부
    public bool[] isMatched;

    // 라인 오브젝트들 모음 (왼쪽 기준)
    [HideInInspector] public GameObject[] lineObjects;

    [TabGroup("UI")] public Image lifeBar;
    [TabGroup("UI")] public Text infoText;


    void Start()
    {
        isMatched = new bool[2 * slotCount];

        matchingTable = new int[slotCount];
        Array.Fill(matchingTable, -1);

        lineObjects = new GameObject[slotCount];
    }


    void Update()
    {
        
    }

    public void LeftBtnClick(int num)
    {
        if (!isClick[0])
        {
            // 왼쪽 처음 클릭
            nowClicked[0] = num;
            isClick[0] = true;
            leftBtnBG[num].SetActive(true);
            Check();
        }
        else
        {
            // 같은 쪽 같은 버튼 클릭 == 취소
            if (nowClicked[0] == num)
            {
                nowClicked[0] = -1;
                isClick[0] = false;
                leftBtnBG[num].SetActive(false);
                return;
            }

            // 같은 쪽 버튼 클릭 == 변경
            leftBtnBG[nowClicked[0]].SetActive(false);
            nowClicked[0] = num;
            leftBtnBG[num].SetActive(true);
        }
        
    }

    public void RightBtnClick(int num)
    {
        if (!isClick[1])
        {

            // 오른쪽 처음 클릭
            nowClicked[1] = num;
            isClick[1] = true;
            rightBtnBG[num].SetActive(true);
            Check();
        }
        else
        {
            // 같은 쪽 같은 버튼 클릭 == 취소
            if (nowClicked[1] == num)
            {
                nowClicked[1] = -1;
                isClick[1] = false;
                rightBtnBG[num].SetActive(false);
                return;
            }

            // 같은 쪽 버튼 클릭 == 변경
            rightBtnBG[nowClicked[1]].SetActive(false);
            nowClicked[1] = num;
            rightBtnBG[num].SetActive(true);
            
        }
    }

    public void Check()
    {
        if (isClick[0] && isClick[1])
        {
            leftBtnBG[nowClicked[0]].SetActive(false);
            rightBtnBG[nowClicked[1]].SetActive(false);

            // 연결되어 있는 선이 있나 확인
            CheckIsMatched(nowClicked[0] , nowClicked[1]);

            // 선 연결
            DrawLine(nowClicked[0], nowClicked[1]);

            // 연결 테이블 작성
            matchingTable[nowClicked[0]] = nowClicked[1];
            isMatched[nowClicked[0]] = true;
            isMatched[nowClicked[1] + slotCount] = true;

            // 클릭되어 있는 정보 초기화
            isClick[0] = false;
            isClick[1] = false;
            nowClicked[0] = -1;
            nowClicked[1] = -1;
        }
    }

    public void CheckIsMatched(int left , int right)
    {   
        // 왼쪽것과 연결되어 있는 것들 초기화
        if (isMatched[left])
        {
            isMatched[matchingTable[left] + slotCount] = false;
            isMatched[left] = false;
            matchingTable[left] = -1;
            Destroy(lineObjects[left]);
        }

        // 오른쪽과 연결되어 있는 것들 초기화
        if (isMatched[right + slotCount])
        {
            // 오른쪽과 연결되어 있는 왼쪽 탐색 (왼쪽이 기준)
            int tmp = -1;

            for (int i = 0; i < slotCount; i++)
            {
                if (matchingTable[i] == right)
                {
                    tmp = i;
                }
            }

            isMatched[tmp] = false;
            isMatched[right + slotCount] = false;
            matchingTable[tmp] = -1;
            Destroy(lineObjects[tmp]);
        }
    }

    public void DrawLine(int left, int right)
    {

        // 선으로 사용할 Image 프리팹을 생성
        GameObject line = Instantiate(linePrefab, linesParent);

        // 선의 RectTransform을 가져옴
        RectTransform lineRect = line.GetComponent<RectTransform>();

        // 시작점과 끝점의 중간 위치로 선을 이동
        Vector3 startPos = leftPos[left].transform.position;
        Vector3 endPos = rightPos[right].transform.position;
        Vector3 centerPos = (startPos + endPos) / 2;

        lineRect.position = centerPos;

        // 선의 크기와 각도 설정
        float distance = Vector3.Distance(startPos, endPos);
        lineRect.sizeDelta = new Vector2(distance, 5f); // 선의 길이와 두께

        // 두 점 사이의 각도를 구해서 회전
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // 생성된 선을 리스트에 추가
        lineObjects[left] = line;

    }

    public int CheckAnswer()
    {
        int answerCount = 0;

        for (int i = 0; i < slotCount; i++)
        {
            if (answerTable[i] == matchingTable[i])
            {
                answerCount++;
            }
        }

        return answerCount;
    }

    public void ClickAnswer()
    {
        

        int ansCount = CheckAnswer();

        if (ansCount < slotCount)
        {
            infoText.text = ansCount.ToString();

            answerChance--;

            if (answerChance <= 0)
            {
                //! Warning

                ClearAll();
            }

            lifeBar.fillAmount = answerChance / 5.0f;

        }
        else
        {
            //! Right
        }

        Clear();
    }

    public void Clear()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Destroy(lineObjects[i]);
            matchingTable[i] = -1;
            isMatched[i] = false;
            isMatched[i + slotCount] = false;

            rightBtnBG[i].SetActive(false);
            rightBtnBG[i].SetActive(false);

        }

        isClick[0] = false;
        isClick[1] = false;

        nowClicked[0] = -1;
        nowClicked[1] = -1;

    }

    public void ClearAll()
    {
        Clear();
        answerChance = 5;
        lifeBar.fillAmount = 1;
        infoText.text = "";

        Singleton.instance.GetComponentInChildren<UIManager>().ESC();
    }
}
