using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float remainTime = 600;

    Text remainTimeText;

    void Start()
    {
        remainTimeText = GetComponent<Text>();
    }

    void Update()
    {
        remainTime -= Time.deltaTime;

        SetColor();

        remainTimeText.text = TimeToString();
    }

    string TimeToString()
    {
        string str= "";

        int remainMin = (int) remainTime / 60;
        int remainSec = (int) remainTime % 60;

        str = remainMin.ToString() + ":" + remainSec.ToString();

        return str;
    }

    void SetColor()
    {
        if (remainTime < 590 && remainTimeText.color != Color.red)
        {
            remainTimeText.color = Color.red;
        }
    }
}
