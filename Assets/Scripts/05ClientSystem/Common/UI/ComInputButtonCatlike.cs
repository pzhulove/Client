using UnityEngine;
using System.Collections;
using System;

[ExecuteAlways]
public class ComInputButtonCatlike : MonoBehaviour {
    public ETCButton[] mButtonGroupUse = new ETCButton[0];
    public ETCButton[] mButtonGroupTip = new ETCButton[0];

    private void _swithETCButtons(ETCButton srcButton, ETCButton dstButton)
    {
        var srcRectSize = srcButton.rectTransform().rect.width;
        var dstRectSize = dstButton.rectTransform().rect.width;

        srcButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dstRectSize);
        srcButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dstRectSize);

        dstButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, srcRectSize);
        dstButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, srcRectSize);

        var srcOff = new Vector2(srcButton.anchorOffet.x, srcButton.anchorOffet.y);
        var dstOff = new Vector2(dstButton.anchorOffet.x, dstButton.anchorOffet.y);
        srcButton.anchorOffet = dstOff;
        dstButton.anchorOffet = srcOff;
    }

    private bool mSwitchStatus = true;
    public void Switch()
    {
        int iUseLen = mButtonGroupUse.Length;
        int iTipLen = mButtonGroupTip.Length;

        if (iUseLen != iTipLen)
        {
            Logger.LogErrorFormat("the swap array mButtonGroupUse & mButtonGroupTips has different size");
            return;
        }

        for (int i = 0; i < iUseLen; i++)
        {
            _swithETCButtons(mButtonGroupTip[i], mButtonGroupUse[i]);
            mButtonGroupTip[i].activated = mSwitchStatus;
            mButtonGroupUse[i].activated = !mSwitchStatus;
        }

        mSwitchStatus = !mSwitchStatus;
    }

	void Start () {
        mSwitchStatus = true;
        Switch();
        Switch();
	}

#if UNITY_EDITOR
    [System.NonSerialized]
    public bool mTestUse = false;
    private bool mUse = false;
#endif

	void Update () {
#if UNITY_EDITOR
        if (mTestUse != mUse)
        {
            mUse = mTestUse;
            Switch();
        }
#endif
    }
}
