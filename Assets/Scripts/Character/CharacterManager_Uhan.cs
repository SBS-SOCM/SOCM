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
}
