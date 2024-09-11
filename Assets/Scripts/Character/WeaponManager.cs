using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int weaponType = 0;

    private void Update()
    {
        WeaponChange();
    }

    private void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Hand
        {
            weaponType = 0;
        }else if (Input.GetKeyDown(KeyCode.Alpha2)) // Knife
        {
            weaponType = 1;
        }else if (Input.GetKeyDown(KeyCode.Alpha3)) // Pistol
        {
            weaponType = 2;
        }else if (Input.GetKeyDown(KeyCode.Alpha4)) // Rifle
        {
            weaponType = 3;
        }else if (Input.GetKeyDown(KeyCode.Alpha5)) // Special
        {
            weaponType = 4;
        }
    }


}
