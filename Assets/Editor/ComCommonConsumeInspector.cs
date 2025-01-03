using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using GameClient;

[CustomEditor(typeof(ComCommonConsume))]
public class ComCommonConsumeInspector : Editor
{
    /*
     * 600000001	金币
     * 600000002	点券
     * 600000003	经验
     * 600000005	决斗币
     * 600000006	复活币
     * 600000007	绑定金币
     * 600000008	绑定点券
     * 600000009	公会贡献
     * */

    public enum eItemType
    {
        None,

        Gold,
        BindGold,
        Ticket,
        BindTicket,
        Exp,
        ResurrectionCcurrency,
        WarriorSoul,
        DuelCoin,
        GuildContri,
        BindHellTicket,
        HellTicket,
        BindYuanGuTicket,
        YuanGuTicket,
        NoColorCrystal,
        ColorCrystal,
        StrengthMergeTool,
        StrengtheMergeToolHigh,
        BlessCrystal,
        PassBlessDrug,
        Bounty,
    }

    protected eItemType mType;

    private int _getIDByType(eItemType type)
    {
        switch (type)
        {
            case eItemType.Gold:
                return 600000001;
            case eItemType.BindGold:
                return 600000007;
            case eItemType.Ticket:
                return 600000002;
            case eItemType.BindTicket:
                return 600000008;
            case eItemType.Exp:
                return 600000003;
            case eItemType.ResurrectionCcurrency:
                return 600000006;
            case eItemType.WarriorSoul:
                return 300001101;
            case eItemType.DuelCoin:
                return 600000005;
            case eItemType.GuildContri:
                return 600000009;
            case eItemType.HellTicket:
                return 200000004;
            case eItemType.BindHellTicket:
                return 200000002;
            case eItemType.YuanGuTicket:
                return 200000003;
            case eItemType.BindYuanGuTicket:
                return 200000001;
            case eItemType.NoColorCrystal:
                return 300000106;
            case eItemType.ColorCrystal:
                return 300000105;
            case eItemType.StrengthMergeTool:
                return 330000213;
            case eItemType.StrengtheMergeToolHigh:
                return 330000214;
            case eItemType.BlessCrystal:
                return 600002535;
            case eItemType.PassBlessDrug:
                return 600002536;
            case eItemType.Bounty:
                return 600002538;
        }

        return 0;
    }


    private eItemType _getTypeByID(int id)
    {
        eItemType type = eItemType.None;

        type = eItemType.Gold;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.BindGold;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.Ticket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.BindTicket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.Exp;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.ResurrectionCcurrency;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.WarriorSoul;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.DuelCoin;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.GuildContri;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.BindHellTicket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.BindYuanGuTicket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.HellTicket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.YuanGuTicket;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.NoColorCrystal;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.ColorCrystal;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.StrengthMergeTool;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.StrengtheMergeToolHigh;
        if( _getIDByType(type) == id) { return type;}

        type = eItemType.BlessCrystal;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.PassBlessDrug;
        if (_getIDByType(type) == id) { return type; }

        type = eItemType.Bounty; 
        if (_getIDByType(type) == id) { return type; }

        return eItemType.None;
    }

  
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ComCommonConsume consume = target as ComCommonConsume;

        GUI.color = Color.green;

        if (null != consume)
        {
            mType = _getTypeByID(consume.mItemID);
            mType = (eItemType)EditorGUILayout.EnumPopup("选择显示类型：", mType);
            int tmp = _getIDByType(mType);
            if (tmp != 0 && tmp != consume.mItemID)
            {
                consume.mItemID = tmp;
            }
        }

        GUI.color = Color.white;
    }
}
