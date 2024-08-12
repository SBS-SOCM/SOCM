using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterManager : MonoBehaviour
{
    [TabGroup("Uhan")] [ReadOnly]
    public int itemType;

    public void SetItemType(int type)
    {
        itemType = type;
    }
}
