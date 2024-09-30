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

    // 0: ���� / 1 : ������
    public bool[] isClick;

    public Transform linesParent;

    public GameObject linePrefab;

    public int slotCount;

    // ���� ���̺�
    [TabGroup("Answer")] public int[] answerTable;

    // ���� �Է� ���� Ƚ��
    [TabGroup("Answer")] public int answerChance;

    // ���ʰ� �������� ������� ������� ����Ǿ� �ִ��� Ȯ���ϴ� ���̺�
    public int[] matchingTable;
    
    // �� ĭ�� �̹� ��Ī�Ǽ� ���� �����ϴ��� ����
    public bool[] isMatched;

    // ���� ������Ʈ�� ���� (���� ����)
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
            // ���� ó�� Ŭ��
            nowClicked[0] = num;
            isClick[0] = true;
            leftBtnBG[num].SetActive(true);
            Check();
        }
        else
        {
            // ���� �� ���� ��ư Ŭ�� == ���
            if (nowClicked[0] == num)
            {
                nowClicked[0] = -1;
                isClick[0] = false;
                leftBtnBG[num].SetActive(false);
                return;
            }

            // ���� �� ��ư Ŭ�� == ����
            leftBtnBG[nowClicked[0]].SetActive(false);
            nowClicked[0] = num;
            leftBtnBG[num].SetActive(true);
        }
        
    }

    public void RightBtnClick(int num)
    {
        if (!isClick[1])
        {

            // ������ ó�� Ŭ��
            nowClicked[1] = num;
            isClick[1] = true;
            rightBtnBG[num].SetActive(true);
            Check();
        }
        else
        {
            // ���� �� ���� ��ư Ŭ�� == ���
            if (nowClicked[1] == num)
            {
                nowClicked[1] = -1;
                isClick[1] = false;
                rightBtnBG[num].SetActive(false);
                return;
            }

            // ���� �� ��ư Ŭ�� == ����
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

            // ����Ǿ� �ִ� ���� �ֳ� Ȯ��
            CheckIsMatched(nowClicked[0] , nowClicked[1]);

            // �� ����
            DrawLine(nowClicked[0], nowClicked[1]);

            // ���� ���̺� �ۼ�
            matchingTable[nowClicked[0]] = nowClicked[1];
            isMatched[nowClicked[0]] = true;
            isMatched[nowClicked[1] + slotCount] = true;

            // Ŭ���Ǿ� �ִ� ���� �ʱ�ȭ
            isClick[0] = false;
            isClick[1] = false;
            nowClicked[0] = -1;
            nowClicked[1] = -1;
        }
    }

    public void CheckIsMatched(int left , int right)
    {   
        // ���ʰͰ� ����Ǿ� �ִ� �͵� �ʱ�ȭ
        if (isMatched[left])
        {
            isMatched[matchingTable[left] + slotCount] = false;
            isMatched[left] = false;
            matchingTable[left] = -1;
            Destroy(lineObjects[left]);
        }

        // �����ʰ� ����Ǿ� �ִ� �͵� �ʱ�ȭ
        if (isMatched[right + slotCount])
        {
            // �����ʰ� ����Ǿ� �ִ� ���� Ž�� (������ ����)
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

        // ������ ����� Image �������� ����
        GameObject line = Instantiate(linePrefab, linesParent);

        // ���� RectTransform�� ������
        RectTransform lineRect = line.GetComponent<RectTransform>();

        // �������� ������ �߰� ��ġ�� ���� �̵�
        Vector3 startPos = leftPos[left].transform.position;
        Vector3 endPos = rightPos[right].transform.position;
        Vector3 centerPos = (startPos + endPos) / 2;

        lineRect.position = centerPos;

        // ���� ũ��� ���� ����
        float distance = Vector3.Distance(startPos, endPos);
        lineRect.sizeDelta = new Vector2(distance, 5f); // ���� ���̿� �β�

        // �� �� ������ ������ ���ؼ� ȸ��
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // ������ ���� ����Ʈ�� �߰�
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
