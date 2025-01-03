using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComChangeFashionIcon : MonoBehaviour 
{
    const string normalIconPath = "Icon";
    const string checkedIconPath = "Icon/Icon (1)";
    public GameObject fashionIcon;
    Image normalIcon;
    Image checkedIcon;
    Toggle toggle;

    void Start()
    {
        if (fashionIcon == null)
            return;
        normalIcon = Utility.GetComponetInChild<Image>(fashionIcon, normalIconPath);
        checkedIcon = Utility.GetComponetInChild<Image>(fashionIcon, checkedIconPath);
        
        toggle = this.GetComponent<Toggle>();
        if (toggle)
        {
            toggle.onValueChanged.AddListener(OnToggleOn);
            SetIconChecked(toggle.isOn);
        }
    }

    void OnDestroy()
    {
        if(toggle)
        {
            toggle.onValueChanged.RemoveListener(OnToggleOn);
            toggle = null;
        }
    }

    void OnToggleOn(bool isOn)
    {
        SetIconChecked(isOn);
    }

    void SetIconChecked(bool isChecked)
    {
        if (normalIcon)
            normalIcon.enabled = !isChecked;
        if (checkedIcon)
            checkedIcon.enabled = isChecked;
    }
}
