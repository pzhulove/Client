using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ComDungeonCharactorBarEnergy: MonoBehaviour
{
    public Image transBar;
    public Image imageBar;

    protected float curValue;
    protected float maxValue;

    public void InitData(float max)
    {
        maxValue = max;
        curValue = max;

        RefreshData(curValue);
    }
    
    public void RefreshData(float value)
    {
        if(Math.Abs(maxValue) < 0.1f)
            return;
        
        curValue = value;
        imageBar.fillAmount = curValue / maxValue;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
