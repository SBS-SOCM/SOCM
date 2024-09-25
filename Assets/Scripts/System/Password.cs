using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Password : MonoBehaviour
{
    public int nowSlot;

    public Text[] pressedNumText;

    public void ClickBtn(int num)
    {
        if (nowSlot > 2)
        {
            return;
        }

        pressedNumText[nowSlot].text = num.ToString();
        nowSlot++;

        if (nowSlot >= 3)
        {
            Invoke("CheckPassword",1f);
        }
        
    }

    public void CheckPassword()
    {
        if (pressedNumText[0].text == 0.ToString() &&
            pressedNumText[1].text == 0.ToString() &&
            pressedNumText[2].text == 0.ToString())
        {
            Debug.Log("Right");
        }
        else
        {
            Debug.Log("Wrong");
        }

        nowSlot = 0;

        for (int i = 0; i < pressedNumText.Length; i++)
        {
            pressedNumText[i].text = "";
        }

    }
}
