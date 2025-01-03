using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PayRewardLevelTab : MonoBehaviour
{
    bool isInited = false;
    bool isManualActive = false;
    #region Bind Data
    public int PayRewardLevelIndex { get; private set; }

    public void SetPayRewardLevelIndex(int index)
    {
        PayRewardLevelIndex = index;
    }

    #endregion

    #region Base Info

    public Toggle toggle;
    public Text toggleText;

    public void Initialize()
    {
        toggle = this.GetComponent<Toggle>();
        if (toggle)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        toggleText = Utility.GetComponetInChild<Text>(this.gameObject, "TabText");

        isInited = true;
    }

    public void Clear()
    {
        if (toggle)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggle = null;
        }
        toggleText = null;

        onPayRewardLevelTabChanged = null;
        isInited = false;
        isManualActive = false;
    }

    public void SetTabText(string desc)
    {
        if (toggleText)
        {
            toggleText.text = desc;
        }
    }

    public void SetTabActive(bool isOn)
    {
        if (toggle)
        {
            toggle.isOn = isOn;
        }
    }

    #endregion

    #region UIEventSystem
    public delegate void OnPayRewardLevelTabChanged();

    public OnPayRewardLevelTabChanged onPayRewardLevelTabChanged;

    public void OnToggleValueChanged(bool isOn)
    {
        if (isInited == false)
        {
            return;
        }
        if (isOn)
        {
            if (onPayRewardLevelTabChanged != null)
            {
                onPayRewardLevelTabChanged();
            }
        }
    }
    #endregion
}
