using GameClient;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHelpSettings : SettingsBindUI
{
    Toggle toggleSkillCD;
    Toggle toggleShowBox;
    Toggle toggleAutoFight;
    Button btnKillMonsters;
    Button btnButtonInputSetting;

    public BattleHelpSettings(GameObject root, ClientFrame frame) : base(root, frame)
    {

    }

    protected override string GetCurrGameObjectPath()
    {
        return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/battleHelp";
    }

    protected override void InitBind()
    {
        toggleSkillCD = mBind.GetCom<Toggle>("ToggleSkillCD");
        if (toggleSkillCD != null)
        {
            toggleSkillCD.isOn = Global.Settings.skillHasCooldown;
            toggleSkillCD.onValueChanged.AddListener(isOn =>
            {
                Global.Settings.skillHasCooldown = isOn;
            });
        }

        toggleShowBox = mBind.GetCom<Toggle>("ToggleShowBox");
        if (toggleShowBox != null)
        {
            toggleShowBox.isOn = Global.Settings.showDebugBox;
            toggleShowBox.onValueChanged.AddListener(isOn =>
            {
                Global.Settings.showDebugBox = isOn;
            });
        }

        toggleAutoFight = mBind.GetCom<Toggle>("ToggleAutoFight");
        if (toggleAutoFight != null)
        {
            toggleAutoFight.isOn = Global.Settings.forceUseAutoFight;
            toggleAutoFight.onValueChanged.AddListener(isOn =>
            {
                Global.Settings.forceUseAutoFight = isOn;
            });
        }

        btnKillMonsters = mBind.GetCom<Button>("ButtonKillMonsters");
        if (btnKillMonsters != null)
        {
            btnKillMonsters.onClick.AddListener(() =>
            {
                if (BattleMain.mode != eDungeonMode.SyncFrame)
                {
                    BattleMain.instance.Main.ClearAllCharacter();
                }
            });
        }

        btnButtonInputSetting = mBind.GetCom<Button>("ButtonInputSetting");
        if (btnButtonInputSetting != null)
        {
            btnButtonInputSetting.onClick.AddListener(() =>
            {
                BattleMain.OpenBattle(BattleType.InputSetting, eDungeonMode.LocalFrame, 0, "1000");
                ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
                //ClientSystemManager.GetInstance().OpenFrame<BattleInputFrame>();
            });
        }
    }

    protected override void UnInitBind()
    {
        if (toggleSkillCD != null)
        {
            toggleSkillCD.onValueChanged.RemoveAllListeners();
        }
        if (toggleShowBox != null)
        {
            toggleShowBox.onValueChanged.RemoveAllListeners();
        }
        if (toggleAutoFight != null)
        {
            toggleAutoFight.onValueChanged.RemoveAllListeners();
        }
        if (btnKillMonsters != null)
        {
            btnKillMonsters.onClick.RemoveAllListeners();
        }
        toggleSkillCD = null;
        toggleShowBox = null;
        toggleAutoFight = null;
        btnKillMonsters = null;
    }
}
