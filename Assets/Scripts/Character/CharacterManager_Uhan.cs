using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterManager
{
    /// <summary>
    /// 현재 손에 들고있는 아이템 타입
    /// 0 : 단검 / 1 : 권총 / 2 : 동전
    /// </summary>
    [TabGroup("Uhan")] [ReadOnly] public int itemType;

    public void SetItemType(int type)
    {
        itemType = type;
        
    }

    public void OnMouseActive()
    {
        _input.cursorLocked = false;
        _input.cursorInputForLook = false;
        _input.SetCursorState(false);
    }

    public void OffMouseActive()
    {
        Debug.Log("Mouse Off");

        _input.cursorLocked = true;
        _input.cursorInputForLook = true;
        _input.SetCursorState(true);
    }


}
