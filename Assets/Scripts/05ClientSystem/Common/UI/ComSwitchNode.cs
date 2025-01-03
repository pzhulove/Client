using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class ComSwitchNode : ComBaseComponet
{
    public enum eState
    {
        Open,
        Close,
    }

    public eState state
    {
        get; private set;
    }

    
#region ExtraUIBind
    private Toggle mSwitch = null;
    private GameObject mContent = null;
    private ToggleGroup mSwitchgroup = null;
    private GameObject mStateoff = null;
    private GameObject mStateon = null;

    protected override void _bindExUI()
    {
        mSwitch = mBind.GetCom<Toggle>("switch");
        mSwitch.onValueChanged.AddListener(_onSwitchToggleValueChange);
        mContent = mBind.GetGameObject("content");
        mSwitchgroup = mBind.GetCom<ToggleGroup>("switchgroup");
        mStateoff = mBind.GetGameObject("stateoff");
        mStateon = mBind.GetGameObject("stateon");
    }

    protected override void _unbindExUI()
    {
        mSwitch.onValueChanged.RemoveListener(_onSwitchToggleValueChange);
        mSwitch = null;
        mContent = null;
        mSwitchgroup = null;
        mStateoff = null;
        mStateon = null;
    }
#endregion    

#region Callback
    private void _onSwitchToggleValueChange(bool changed)
    {
        /* put your code in here */

        state = changed ? eState.Open : eState.Close;

        if (null != mContent)
        {
            mContent.SetActive(changed);
        }

        if (null != mStateon)
        {
            mStateon.SetActive(changed);
        }

        if (null != mStateoff)
        {
            mStateoff.SetActive(!changed);
        }
    }
#endregion
    public void Reset()
    {
        ClearSubItem();
        _onSwitchToggleValueChange(false);
    }

    public ComCommonBind AddOneSubItem()
    {
        string path = mBind.GetPrefabPath("unit");

        ComCommonBind bind = null;

        if (!string.IsNullOrEmpty(path))
        {
            bind = mBind.LoadExtraBind(path);

            if (null != bind)
            {
                Utility.AttachTo(bind.gameObject, mContent);
            }
        }

        return bind;
    }

    public void ClearSubItem()
    {
        string path = mBind.GetPrefabPath("unit");

        if (!string.IsNullOrEmpty(path))
        {
            mBind.ClearCacheBinds(path);
        }
    }
}
