using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;

class RelationFrameTest : ClientFrame
{
    
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Test/TitleImage";
    }


    protected override void _OnOpenFrame()
    {
       
    }

    protected override void _OnCloseFrame()
    {
        
    }

    [UIEventHandle("Panel/Exist/Button")]
    void OnClickClose()
    {
        frameMgr.CloseFrame(this);
    }

    [UIEventHandle("Bottom/Button")]
    void OnOpen1()
    {
        frameMgr.OpenFrame<RelationFrameDialogTest>(FrameLayer.Middle);
    }

    [UIEventHandle("Bottom/ButtonList")]
    void OnOpen2()
    {
       frameMgr.OpenFrame<RelationFramePopupTest>(FrameLayer.Middle);
    }
}


class RelationFramePopupTest : ClientFrame
{
    
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Test/PopupDialog";
    }


    protected override void _OnOpenFrame()
    {
       
    }

    protected override void _OnCloseFrame()
    {
        
    }

    [UIEventHandle("Title/Button")]
    void OnClickClose()
    {
        frameMgr.CloseFrame(this);
    }
    
}


class RelationFrameDialogTest : ClientFrame
{
    
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Test/Dialog";
    }


    protected override void _OnOpenFrame()
    {
       
    }

    protected override void _OnCloseFrame()
    {
        
    }

    [UIEventHandle("Ok")]
    void OnClickClose()
    {
        frameMgr.CloseFrame(this);
    }
}