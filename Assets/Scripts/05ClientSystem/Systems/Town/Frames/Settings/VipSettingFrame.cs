using _Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using UnityEngine.UI;
using ProtoTable;
using System;

public class VipSettingFrame :ClientFrame
{
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/SettingPanel/vipSettingFrame";
    }

    protected override bool _isLoadFromPool()
    {
        
        return BattleMain.instance==null?false:true;
    }

    #region ExtraUIBind

  
    private ItemDrugSetting[] itemPrefabs = new ItemDrugSetting[3];
    private Button btn;
    protected override void _bindExUI()
    {
        itemPrefabs[0] = mBind.GetCom<ItemDrugSetting>("ItemPrefab1");
        itemPrefabs[1] = mBind.GetCom<ItemDrugSetting>("ItemPrefab2");
        itemPrefabs[2] = mBind.GetCom<ItemDrugSetting>("ItemPrefab3");
        btn = mBind.GetCom<Button>("setBtn");

        btn.onClick.AddListener(()=> 
        {
            ClientSystemManager.instance.OpenFrame<ChapterBattlePotionSetFrame>();
        });
    }

    #endregion

    protected override void _OnOpenFrame()
    {
        InitUI();
        InitDrugItem(null);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ClosePotionSetFrame, InitDrugItem);
    }

    GameObject vipsetting = null;
    protected void InitUI()
    {
        vipsetting = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/SettingPanel/VipSetting");
        if (vipsetting != null)
        {
            Utility.AttachTo(vipsetting,frame);
        }
    }

    

    private void InitDrugItem(UIEvent uiEvent)
    {
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            itemPrefabs[i].SetItemInfo(PlayerBaseData.GetInstance().GetPotionID((PlayerBaseData.PotionSlotType)i), PlayerBaseData.GetInstance().GetPotionPercent((PlayerBaseData.PotionSlotType)i), (PlayerBaseData.PotionSlotType)i);
        }

    }

    protected override void _OnCloseFrame()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ClosePotionSetFrame, InitDrugItem);
        base._OnCloseFrame();
    }
}

