using GameClient;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VipSetting : MonoBehaviour {
    public  GeUISwitchButton switchReborn = null;
    //public GeUISwitchButton switchCrystal = null;

    public GameObject mReborn = null;
    //public GameObject mCrystalSkill = null;
    // Use this for initialization
    void Start () {
        if (null != switchReborn)
        {
            switchReborn.onValueChanged.AddListener(_onRebornToggleValueChange);
        }
        //if (null != switchCrystal)
        //{
        //    switchCrystal.onValueChanged.AddListener(_onCrystalSkillToggleValueChange);
        //}
        InitRebornSetting();
        //InitUseCrystalSkillSetting();
        if (!BeUtility.CheckVipFuncOpen(SettingManager.vipRebornTableId))
            mReborn.CustomActive(false);
        //if (!BeUtility.CheckVipFuncOpen(SettingManager.vipUseCrystalTableId) || !CheckUseCrystal())
        //    mCrystalSkill.CustomActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //protected bool CheckUseCrystal()
    //{
    //    SwitchClientFunctionTable data = TableManager.instance.GetTableItem<SwitchClientFunctionTable>(SettingManager.vipUseCrystalTableId);
    //    for (int i = 0; i < data.ValueDLength; i++)
    //    {
    //        if (data.ValueDArray(i) == PlayerBaseData.GetInstance().JobTableID)
    //            return true;
    //    }
    //    return false;
    //}
    private void _onRebornToggleValueChange(bool changed)
    {
        SettingManager.GetInstance().SetVipSettingData(SettingManager.STR_VIPREBORN, changed);
    }

    //private void _onCrystalSkillToggleValueChange(bool changed)
    //{
    //    SettingManager.GetInstance().SetVipSettingData(SettingManager.STR_VIPCRYSTAL, changed);
    //}

    //初始化自动复活
    protected void InitRebornSetting()
    {
        int tableId = SettingManager.vipRebornTableId;
        if (!CheckVipLevel(tableId))
            return;

        bool flag = GetDefault(tableId, SettingManager.STR_VIPREBORN);
        switchReborn.SetSwitch(flag);
    }

    //初始化自动化使用无色技能
    //protected void InitUseCrystalSkillSetting()
    //{
    //    int tableId = SettingManager.vipUseCrystalTableId;
    //    if (!CheckVipLevel(tableId))
    //        return;
    //    bool flag = GetDefault(tableId, SettingManager.STR_VIPCRYSTAL);
    //    switchCrystal.SetSwitch(flag);
    //}

    private bool CheckVipLevel(int id)
    {
        int vipLevel = PlayerBaseData.GetInstance().VipLevel;
        int limitVipLevel = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(id).ValueA;
        return vipLevel > limitVipLevel;
    }

    private string GetRoleId()
    {
        return PlayerBaseData.GetInstance().RoleID.ToString();
    }

    private bool GetDefault(int tableId, string vipType)
    {
        string realyKey = string.Format("{0}{1}", vipType, GetRoleId());
        bool flag = false;
        if (PlayerLocalSetting.GetValue(realyKey) == null)
        {
            SwitchClientFunctionTable switchClientTableData = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(tableId);
            flag = switchClientTableData.ValueB == 1 ? true : false;

        }
        else
        {
            flag = SettingManager.GetInstance().GetVipSettingData(vipType, GetRoleId());
        }
        return flag;
    }
    void OnDestroy()
    {
        mReborn = null;
        if (null != switchReborn)
        {
            switchReborn.onValueChanged.RemoveListener(_onRebornToggleValueChange);
        }
        switchReborn = null;

        //mCrystalSkill = null;
        //if (null != switchCrystal)
        //{
        //    switchCrystal.onValueChanged.RemoveListener(_onCrystalSkillToggleValueChange);
        //}
    }
}
