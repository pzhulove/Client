using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComInputSkillExternalRoot : MonoBehaviour
{
    public GameObject WeaponButton;
    public GameObject DrinkDrugButton;

    public Vector3 GetWeaponButtonPosition()
    {
        if (null == WeaponButton)
        {
            return Vector3.zero;
        }

        return WeaponButton.transform.position;
    }

    public Vector3 GetDrinkButtonPosition()
    {
        if (null == DrinkDrugButton)
        {
            return Vector3.zero;
        }

        return DrinkDrugButton.transform.position;
    }
    
}
