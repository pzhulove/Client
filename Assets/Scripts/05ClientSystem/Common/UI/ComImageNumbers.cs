using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

// 美术字组成的数字
public class ComImageNumbers : MonoBehaviour 
{
    [SerializeField]
    string numPathPrefix = "";

    [SerializeField]
    Image numTemplate = null;

    [SerializeField]
    GameObject numsRoot = null;

    [SerializeField]
    uint number = 0;   

    private void Awake()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        
    }

    public void SetValue(uint value)
    {
        number = value;
        UpdateUI();
    }

    uint GetValue()
    {
        return number;
    }

    void UpdateUI()
    {
        if(numsRoot == null)
        {
            return;
        }

        if(numTemplate == null)
        {
            return;
        }

        for (int i = 0; i < numsRoot.transform.childCount; ++i)
        {
            GameObject.Destroy(numsRoot.transform.GetChild(i).gameObject);
        }

        string strNumber = number.ToString();
        for(int i = 0;i < strNumber.Length;i++)
        {
            GameObject go = GameObject.Instantiate(numTemplate.gameObject);
            if(go == null)
            {
                continue;
            }

            Image img = go.GetComponent<Image>();
            if(img == null)
            {
                return;
            }

            img.SafeSetImage(numPathPrefix + Utility.ToInt(strNumber[i].ToString()), true);
            img.transform.SetParent(numsRoot.transform, false);
            img.CustomActive(true);
        }
    }
}
