using GameClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

enum InputSettingItemChangeTypeFlag
{
    none = 0x00,
    position = 0x01,
    scale = 0x02,
    alpha = 0x04,
}
public class InputSettingItem
{
    public Vector3 position;
    public Vector3 scale;
    public float alpha;

    public InputSettingItem()
    {
        position = Vector3.zero;
        scale = Vector3.one;
        alpha = 1.0f;
    }
    
    public InputSettingItem(Vector3 _position, Vector3 _scale, float _alpha)
    {
        position = _position;
        scale = _scale;
        alpha = _alpha;
    }

    public bool IsSameData(InputSettingItem item)
    {
        if (item == null)
            return false;
        if (position.x == item.position.x
            && position.y == item.position.y
            && position.z == item.position.z
            && scale.x == item.scale.x
            && scale.y == item.scale.y
            && scale.z == item.scale.z
            && alpha == item.alpha)
            return true;
        return false;
    }
    
    public void SetData(Vector3 _position, Vector3 _scale, float _alpha)
    {
        position = _position;
        scale = _scale;
        alpha = _alpha;
    }

    public void SetDataPosition(Vector3 _position)
    {
        position = _position;
    }
    
    public void SetDataScale(Vector3 _scale)
    {
        scale = _scale;
    }
    
    public void SetDataAlpha(float _alpha)
    {
        alpha = _alpha;
    }

    public void DecodeData(string str)
    {
        if (string.IsNullOrEmpty(str)) return;
        string[] strArry = str.Split(',');
        if (strArry != null && strArry.Length == 7)
        {
            if(float.TryParse(strArry[0], out var data0))
                position.x = data0;
            if(float.TryParse(strArry[1], out var data1))
                position.y = data1;
            if(float.TryParse(strArry[2], out var data2))
                position.z = data2;
            if(float.TryParse(strArry[3], out var data3))
                scale.x = data3;
            if(float.TryParse(strArry[4], out var data4))
                scale.y = data4;
            if(float.TryParse(strArry[5], out var data5))
                scale.z =  data5;
            if(float.TryParse(strArry[6], out var data6))
                alpha = data6;
        }
    }
    
    public string IncodeData()
    {
        string result = string.Empty;
        result += position.x + "," + position.y + "," + position.z + "," + scale.x + "," + scale.y + "," + scale.z + "," + alpha;
        return result;
    }
}

public class InputSettingBattleItemList
{
    public InputSettingItem mJoystick;
    public InputSettingItem mBattleUIDrug;
    public InputSettingItem mBattleUISwitchWeaAndEquip;
    public InputSettingItem mETCButtons;
    public List<InputSettingItem> mETCButtonlist;

    public InputSettingBattleItemList()
    {
        mJoystick = new InputSettingItem();
        mBattleUIDrug = new InputSettingItem();
        mBattleUISwitchWeaAndEquip = new InputSettingItem();
        mETCButtons = new InputSettingItem();
        mETCButtonlist = new List<InputSettingItem>();
    }
    
    public void SetData(InputSettingBattleItemList inputSettingBattleItemListData)
    {
        if(inputSettingBattleItemListData == null)
            return;;
        mJoystick.SetData(
            inputSettingBattleItemListData.mJoystick.position,
            inputSettingBattleItemListData.mJoystick.scale,
            inputSettingBattleItemListData.mJoystick.alpha
        );
        mBattleUIDrug.SetData(
            inputSettingBattleItemListData.mBattleUIDrug.position,
            inputSettingBattleItemListData.mBattleUIDrug.scale,
            inputSettingBattleItemListData.mBattleUIDrug.alpha
        );
        mBattleUISwitchWeaAndEquip.SetData(
            inputSettingBattleItemListData.mBattleUISwitchWeaAndEquip.position,
            inputSettingBattleItemListData.mBattleUISwitchWeaAndEquip.scale,
            inputSettingBattleItemListData.mBattleUISwitchWeaAndEquip.alpha
        );
        mETCButtons.SetData(
            inputSettingBattleItemListData.mETCButtons.position,
            inputSettingBattleItemListData.mETCButtons.scale,
            inputSettingBattleItemListData.mETCButtons.alpha
        );
            
        mETCButtonlist.Clear();
        foreach (var item in inputSettingBattleItemListData.mETCButtonlist)
        {
            var data = new InputSettingItem(item.position,item.scale,item.alpha);
            ETCButtonlistAdd(data);
        }
    }

    public void ETCButtonlistAdd(InputSettingItem data)
    {
        if (mETCButtonlist.Count < InputSettingBattleManager.InputSettingBattleItemProETCButtonlistMaxCount)
        {
            mETCButtonlist.Add(data);
        }
        else
        {
            Logger.LogErrorFormat("ETCButtonlistAdd Error");
        }
    }

    public bool IsSameData(InputSettingBattleItemList inputSettingBattleItemListData)
    {
        if (inputSettingBattleItemListData == null)
            return false;
        if (mJoystick.IsSameData(inputSettingBattleItemListData.mJoystick)
            && mBattleUIDrug.IsSameData(inputSettingBattleItemListData.mBattleUIDrug)
            && mBattleUISwitchWeaAndEquip.IsSameData(inputSettingBattleItemListData.mBattleUISwitchWeaAndEquip)
            && mETCButtons.IsSameData(inputSettingBattleItemListData.mETCButtons)
            && mETCButtonlist.Count == inputSettingBattleItemListData.mETCButtonlist.Count)
        {
            for (int i = 0; i < mETCButtonlist.Count; i++)
            {
                if (!mETCButtonlist[i].IsSameData(inputSettingBattleItemListData.mETCButtonlist[i]))
                    return false;
            }

            return true;
        }

        return false;
    }

    void ClearData()
    {
        if(mJoystick != null)
            mJoystick = null;
        if(mBattleUIDrug != null)
            mBattleUIDrug = null;
        if(mBattleUISwitchWeaAndEquip != null)
            mBattleUISwitchWeaAndEquip = null;
        if(mETCButtons != null)
            mETCButtons = null;
        if (mETCButtonlist != null)
        {
            mETCButtonlist.Clear();
            mETCButtonlist = null;
        }
    }

    ~InputSettingBattleItemList()
    {
        ClearData();
    }
    
    public void DecodeData(string str)
    {
        if (string.IsNullOrEmpty(str)) return;
        string[] strArry = str.Split(';');
        if (strArry != null && strArry.Length == 5)
        {
            mJoystick.DecodeData(strArry[0]);
            mBattleUIDrug.DecodeData(strArry[1]);
            mBattleUISwitchWeaAndEquip.DecodeData(strArry[2]);
            mETCButtons.DecodeData(strArry[3]);
            string tmpStr = strArry[4];
            if (!string.IsNullOrEmpty(tmpStr))
            {
                string[] strArry2 = tmpStr.Split('|');
                if (strArry2 != null)
                {
                    mETCButtonlist.Clear();
                    for (int i = 0; i < strArry2.Length; i++)
                    {
                        var data = new InputSettingItem();
                        data.DecodeData(strArry2[i]);
                        ETCButtonlistAdd(data);
                    }
                }
            }
        }
    }
    
    public string IncodeData()
    {
        string result = string.Empty;
        result += mJoystick.IncodeData() + ";";
        result += mBattleUIDrug.IncodeData() + ";";
        result += mBattleUISwitchWeaAndEquip.IncodeData() + ";";
        result += mETCButtons.IncodeData() + ";";
        foreach (var item in mETCButtonlist)
        {
            result += item.IncodeData() + "|";
        }
        return result;
    }
}

public enum InputSettingBattleProgramType
{
    
    none = -1,
    [DescriptionAttribute("自定义方案1")]
    InputSettingBattleProgramType1,
    [DescriptionAttribute("自定义方案2")]
    InputSettingBattleProgramType2,
    [DescriptionAttribute("自定义方案3")]
    InputSettingBattleProgramType3,
    Max,
} 

public class InputSettingBattleManager : Singleton<InputSettingBattleManager>
{
    public readonly static int InputSettingBattleItemProETCButtonlistMaxCount = 21;
    private Dictionary<InputSettingBattleProgramType, InputSettingBattleItemList> mInputSettingBattleProgramDic = new Dictionary<InputSettingBattleProgramType, InputSettingBattleItemList>();
    private Dictionary<InputSettingBattleProgramType, string> mInputSettingBattleProgramNameDic = new Dictionary<InputSettingBattleProgramType, string>();
    private InputSettingBattleItemList mInputSettingBattleItemProgramOrigin = new InputSettingBattleItemList();
    private InputSettingBattleProgramType mCurrInputSettingBattleProgramType = InputSettingBattleProgramType.none;
    public bool isInitOrigin = false;
    public bool isInitOrigin_BattleUIDrug = false;
    public bool isInitOrigin_BattleUISwitchWeaAndEquip = false;
    public override void Init()
    {
        base.Init();
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleIdChanged, _OnRoleIdChanged);
        loadData();
    }

    public override void UnInit()
    {
        base.UnInit();
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleIdChanged, _OnRoleIdChanged);
        mInputSettingBattleProgramDic.Clear();
        mInputSettingBattleProgramNameDic.Clear();
        mInputSettingBattleItemProgramOrigin = null;
        isInitOrigin = false;
        isInitOrigin_BattleUIDrug = false;
        isInitOrigin_BattleUISwitchWeaAndEquip = false;
    }
    
    void _OnRoleIdChanged(UIEvent uiEvent)
    {
        mInputSettingBattleProgramDic.Clear();
        mInputSettingBattleProgramNameDic.Clear();
        isInitOrigin = false;
        isInitOrigin_BattleUIDrug = false;
        isInitOrigin_BattleUISwitchWeaAndEquip = false;
        loadData();
    }

    void loadData()
    {
        mInputSettingBattleProgramDic.Clear();
        mInputSettingBattleProgramNameDic.Clear();
        for (int i = 0; i < (int)InputSettingBattleProgramType.Max; i++)
        {
            var item = PlayerPrefs.GetString(((InputSettingBattleProgramType)i).ToString() + "Type" + PlayerBaseData.GetInstance().RoleID);
            if (!string.IsNullOrEmpty(item))
            {
                var inputSettingBattlePro = new InputSettingBattleItemList();
                inputSettingBattlePro.DecodeData(item);
                mInputSettingBattleProgramDic.Add((InputSettingBattleProgramType) i, inputSettingBattlePro);
            }
            var itemName = PlayerPrefs.GetString(((InputSettingBattleProgramType)i).ToString() + "Name"+ PlayerBaseData.GetInstance().RoleID);
            mInputSettingBattleProgramNameDic.Add((InputSettingBattleProgramType) i, itemName);
        }

        var type = PlayerPrefs.GetInt("CurrInputSettingBattleProgram" + PlayerBaseData.GetInstance().RoleID, -1);
        if (type >= (int) InputSettingBattleProgramType.none && type < (int) InputSettingBattleProgramType.Max)
            mCurrInputSettingBattleProgramType = (InputSettingBattleProgramType) type;
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.InputSettingBattleProNameChange);
    }

    public InputSettingBattleItemList GetInputSettingBattleProgram(InputSettingBattleProgramType type)
    {
        if (mInputSettingBattleProgramDic.ContainsKey(type))
        {
            return mInputSettingBattleProgramDic[type];
        }
        return null;
    }
    
    public void SaveInputSettingBattleItemProgramData(InputSettingBattleProgramType type, InputSettingBattleItemList data)
    {
        var str = data.IncodeData();
        if (mInputSettingBattleProgramDic.ContainsKey(type))
        {
           mInputSettingBattleProgramDic[type].SetData(data);
        }
        else
        {
            var inputSettingBattlePro = new InputSettingBattleItemList();
            inputSettingBattlePro.SetData(data);
            mInputSettingBattleProgramDic.Add(type, inputSettingBattlePro);
        }

        PlayerPrefs.SetString(type.ToString() + "Type" + PlayerBaseData.GetInstance().RoleID, str);
    }
    
    public string GetInputSettingBattleProgramName(InputSettingBattleProgramType type)
    {
        if (mInputSettingBattleProgramNameDic.ContainsKey(type))
        {
            return mInputSettingBattleProgramNameDic[type];
        }
        return "";
    }
    
    public void SaveInputSettingBattleItemProgramNameData(InputSettingBattleProgramType type, string data)
    {
        if (mInputSettingBattleProgramNameDic.ContainsKey(type))
        {
            mInputSettingBattleProgramNameDic[type] = data;
        }
        else
        {
            mInputSettingBattleProgramNameDic.Add(type, data);
        }

        PlayerPrefs.SetString(type.ToString() + "Name"+ PlayerBaseData.GetInstance().RoleID, data);
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.InputSettingBattleProNameChange);
    }

    public void SaveInputSettingBattleProgramType(InputSettingBattleProgramType type)
    {
        if (type >= InputSettingBattleProgramType.none && type < InputSettingBattleProgramType.Max)
        {
            mCurrInputSettingBattleProgramType = type;
            PlayerPrefs.SetInt("CurrInputSettingBattleProgram" + PlayerBaseData.GetInstance().RoleID, (int)type); 
        }
    }
    
    public InputSettingBattleItemList GetCurrInputSettingBattleProgram()
    {
        return GetInputSettingBattleProgram(mCurrInputSettingBattleProgramType);
    }

    public InputSettingBattleProgramType GetInputSettingBattleProgramType()
    {
        return mCurrInputSettingBattleProgramType;
    }

    public void InitOriginData(InputSettingBattleItemList inputSettingBattleItemListData)
    {
        if (!isInitOrigin)
        {
            mInputSettingBattleItemProgramOrigin.SetData(inputSettingBattleItemListData);
            isInitOrigin = true;
        }
    }
    
    public void InitOriginData_BattleUIDrug(InputSettingItem battleUIDrug)
    {
        if (!isInitOrigin_BattleUIDrug)
        {
            mInputSettingBattleItemProgramOrigin.mBattleUIDrug.SetData(battleUIDrug.position, battleUIDrug.scale,
                battleUIDrug.alpha);
            isInitOrigin_BattleUIDrug = true;
        }
    }
    
    public void InitOriginData_BattleUISwitchWeaAndEquip(InputSettingItem battleUISwitchWeaAndEquip)
    {
        if (!isInitOrigin_BattleUISwitchWeaAndEquip)
        {
            mInputSettingBattleItemProgramOrigin.mBattleUISwitchWeaAndEquip.SetData(battleUISwitchWeaAndEquip.position,
                battleUISwitchWeaAndEquip.scale, battleUISwitchWeaAndEquip.alpha);
            isInitOrigin_BattleUISwitchWeaAndEquip = true;
        }
    }

    public bool IsInitSuccess()
    {
        return isInitOrigin && isInitOrigin_BattleUIDrug && isInitOrigin_BattleUISwitchWeaAndEquip;
    }
    
    public InputSettingBattleItemList GetInputSettingBattleItemProgramOrigin()
    {
        return mInputSettingBattleItemProgramOrigin;
    }
}
