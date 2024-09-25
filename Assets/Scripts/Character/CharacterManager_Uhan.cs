using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterManager
{
    /// <summary>
    /// ���� �տ� ����ִ� ������ Ÿ��
    /// 0 : �ܰ� / 1 : ���� / 2 : ����
    /// </summary>
    [TabGroup("Uhan")] [ReadOnly] public int itemType;

    public void SetItemType(int type)
    {
        itemType = type;
        
    }

    public void OnMouseActive()
    {
        if (_input.cursorLocked == true)
        {
            _input.cursorLocked = !_input.cursorLocked;
            _input.SetCursorState(false);
        }
        
    }

    public void OffMouseActive()
    {
        if (_input.cursorLocked == false)
        {
            _input.cursorLocked = !_input.cursorLocked;
            _input.SetCursorState(true);
        }
    }


}
