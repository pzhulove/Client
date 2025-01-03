using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using UnityEngine.UI;
public class SkillComboDesFrame : ClientFrame
{
    private Button btnClick;
    private Image background;
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/BattleUI/SkillComboDesFrame";
    }

    protected override void _bindExUI()
    {
        base._bindExUI();
        btnClick = mBind.GetCom<Button>("btnClick");
        background = mBind.GetCom<Image>("background");
    }

    protected override void _OnOpenFrame()
    {
        base._OnOpenFrame();
        btnClick.onClick.RemoveAllListeners();
        btnClick.onClick.AddListener(()=> { Close(); });
    }

    protected override void _OnCloseFrame()
    {
        base._OnCloseFrame();
    }

    protected override void _unbindExUI()
    {
        base._unbindExUI();
    }

}
