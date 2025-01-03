using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;

class AdjustResultFrame : ClientFrame
{
    public class AdjustResultFrameData
    {
        public string desc;
        public UnityEngine.Events.UnityAction callback;
    }

    AdjustResultFrameData data;

    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/SmithShop/AdjustResultFrameEx";
    }

    Text m_kDesc;

    [UIControl("ok", typeof(Button))]
    private Button mOkBtn;

    [UIEventHandle("ok")]
    void OnClickOk()
    {
        if(data != null && data.callback != null)
        {
            data.callback.Invoke();
        }

        if (mOkBtn != null)
        {
            mOkBtn.enabled = false;

            InvokeMethod.Invoke(this, 1.00f, () =>
            {
                if (mOkBtn != null)
                {
                    mOkBtn.enabled = true;
                }
            });
        }

        frameMgr.CloseFrame(this);
    }

    protected override void _OnOpenFrame()
    {
        m_kDesc = Utility.FindComponent<Text>(frame, "middle/Text");
        data = userData as AdjustResultFrameData;
        m_kDesc.text = data.desc;
    }

    protected override void _OnCloseFrame()
    {
        
    }

    [UIEventHandle("close")]
    void OnClickClose()
    {
        frameMgr.CloseFrame(this);
    }
}
