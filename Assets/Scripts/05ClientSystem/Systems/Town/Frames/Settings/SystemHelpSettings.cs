using GameClient;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

public class SystemHelpSettings : SettingsBindUI
{
    InputField inputLevel;
    Button btnLevelUp;
    Button btnMaxLevel;
    InputField inputOccu;
    Button btnChangeOccu;
    Button btnGetStronger;
    Button btnFinishTasks;
    Button btnAddFatigue;
    InputField inputSuitId;
    Text textSuitName;
    InputField inputSuitStrength;
    Button btnGetSuit;
    InputField inputItemId;
    Text textItemName;
    InputField inputItemNumber;
    InputField inputEquipStrength;
    InputField inputEquipQuality;
    InputField inputEquipType;
    InputField inputEnhanceType;
    Button btnGetItem;
    InputField inputCMD;
    Button btnSendCMD;
    Toggle mSelectTestToggle;

    public SystemHelpSettings(GameObject root, ClientFrame frame) : base(root, frame)
    {

    }

    protected override string GetCurrGameObjectPath()
    {
        return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/systemHelp";
    }

    protected override void InitBind()
    {
        //升级
        inputLevel = mBind.GetCom<InputField>("InputLevel");
        btnLevelUp = mBind.GetCom<Button>("ButtonLevelUp");
        if (btnLevelUp != null && inputLevel != null)
        {
            btnLevelUp.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!uplevel num=" + inputLevel.text);
            });
        }
        btnMaxLevel = mBind.GetCom<Button>("ButtonMaxLevel");
        if (btnMaxLevel != null)
        {
            btnMaxLevel.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!uplevel num=60");
            });
        }

        //转职
        inputOccu = mBind.GetCom<InputField>("InputOccu");
        btnChangeOccu = mBind.GetCom<Button>("ButtonChangeOccu");
        if (btnChangeOccu != null && inputOccu != null)
        {
            inputOccu.text = PlayerBaseData.GetInstance().JobTableID.ToString();
            btnChangeOccu.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!changeoccu occu=" + inputOccu.text);
            });
        }

        //一键变强
        btnGetStronger = mBind.GetCom<Button>("ButtonGetStronger");
        if (btnGetStronger != null)
        {
            btnGetStronger.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!addpoint num=999999");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!addgold num=9999999");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!additem id=600000006 num=999");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!additem id=200001001 num=999");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!additem id=200000003 num=9990");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!additem id=200000004 num=9990");
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!additem id=300000106 num=9990");

                var suitId = 4000 + PlayerBaseData.GetInstance().JobTableID;
                var data = TableManager.instance.GetTableItem<OneKeyWearTable>(suitId);
                if (data == null)//没有套装就用初始职业套装
                {
                    suitId = suitId - suitId % 10;
                }
                Utility.UseOneKeySuitWear(suitId, 10);
            });
        }

        //完成所有任务
        btnFinishTasks = mBind.GetCom<Button>("ButtonFinishTasks");
        if (btnFinishTasks != null)
        {
            btnFinishTasks.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!fatask");
            });
        }

        //回满体力
        btnAddFatigue = mBind.GetCom<Button>("ButtonAddFatigue");
        if (btnAddFatigue != null)
        {
            btnAddFatigue.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!addfatigue num=150");
            });
        }

        //获取套装
        inputSuitId = mBind.GetCom<InputField>("InputSuitID");
        textSuitName = mBind.GetCom<Text>("TextSuitName");
        if (inputSuitId != null && textSuitName != null)
        {
            inputSuitId.onValueChanged.AddListener(text =>
            {
                int suitId = 0;
                int.TryParse(text, out suitId);
                var data = TableManager.instance.GetTableItem<OneKeyWearTable>(suitId);
                if (data != null)
                {
                    textSuitName.text = data.Name;
                }
                else
                {
                    textSuitName.text = "无";
                }
            });
        }
        inputSuitStrength = mBind.GetCom<InputField>("InputSuitStrength");
        btnGetSuit = mBind.GetCom<Button>("ButtonGetSuit");
        if (inputSuitId != null && inputSuitStrength != null && btnGetSuit != null)
        {
            btnGetSuit.onClick.AddListener(() =>
            {
                int suitId = 0;
                int.TryParse(inputSuitId.text, out suitId);
                int strength = 0;
                int.TryParse(inputSuitStrength.text, out strength);

                Utility.UseOneKeySuitWear(suitId, strength);
            });
        }

        //获取道具（装备）
        inputItemId = mBind.GetCom<InputField>("InputItemID");
        textItemName = mBind.GetCom<Text>("TextItemName");
        if (inputItemId != null && textItemName != null)
        {
            inputItemId.onValueChanged.AddListener(text =>
            {
                int itemId = 0;
                int.TryParse(text, out itemId);
                var data = TableManager.instance.GetTableItem<ItemTable>(itemId);
                if (data != null)
                {
                    textItemName.text = data.Name;
                }
                else
                {
                    textItemName.text = "无";
                }
            });
        }
        inputItemNumber = mBind.GetCom<InputField>("InputItemNumber");
        inputEquipStrength = mBind.GetCom<InputField>("InputEquipStrength");
        inputEquipQuality = mBind.GetCom<InputField>("InputEquipQuality");
        inputEquipType = mBind.GetCom<InputField>("InputEquipType");
        inputEnhanceType = mBind.GetCom<InputField>("InputEnhanceType");
        btnGetItem = mBind.GetCom<Button>("ButtonGetItem");
        if (btnGetItem != null &&
            inputItemId != null &&
            inputItemNumber != null &&
            inputEquipStrength != null &&
            inputEquipQuality != null &&
            inputEquipType != null &&
            inputEnhanceType != null)
        {
            btnGetItem.onClick.AddListener(() =>
            {
                if (IsEquip(inputItemId.text))
                {
                    if (inputEquipType.text == "0")
                    {
                        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1} str={2} ql={3}", inputItemId.text, inputItemNumber.text, inputEquipStrength.text, inputEquipQuality.text));
                    }
                    else
                    {
                        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addee id={0} num={1} str={2} ql={3} et={4} ent={5}", inputItemId.text, inputItemNumber.text, inputEquipStrength.text, inputEquipQuality.text, inputEquipType.text, inputEnhanceType.text));
                    }
                }
                else
                {
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", inputItemId.text, inputItemNumber.text));
                }
            });
        }

        //发送作弊指令
        inputCMD = mBind.GetCom<InputField>("InputCMD");
        btnSendCMD = mBind.GetCom<Button>("ButtonSendCMD");
        if (inputCMD != null && btnSendCMD != null)
        {
            btnSendCMD.onClick.AddListener(() =>
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, inputCMD.text);
            });
        }

        mSelectTestToggle = mBind.GetCom<Toggle>("SelectTestToggle");
        if (mSelectTestToggle != null)
            mSelectTestToggle.onValueChanged.AddListener(_OnSelectTestToggle);
    }

    private void _OnSelectTestToggle(bool value)
    {
        Global.Settings.isDebug = value;
    }

    protected override void UnInitBind()
    {
        if (btnLevelUp != null)
        {
            btnLevelUp.onClick.RemoveAllListeners();
        }
        if (btnMaxLevel != null)
        {
            btnMaxLevel.onClick.RemoveAllListeners();
        }
        if (btnChangeOccu != null)
        {
            btnChangeOccu.onClick.RemoveAllListeners();
        }
        if (btnGetStronger != null)
        {
            btnGetStronger.onClick.RemoveAllListeners();
        }
        if (inputSuitId != null)
        {
            inputSuitId.onValueChanged.RemoveAllListeners();
        }
        if (btnGetSuit != null)
        {
            btnGetSuit.onClick.RemoveAllListeners();
        }
        if (btnFinishTasks != null)
        {
            btnFinishTasks.onClick.RemoveAllListeners();
        }
        if (btnAddFatigue != null)
        {
            btnAddFatigue.onClick.RemoveAllListeners();
        }
        if (inputItemId != null)
        {
            inputItemId.onValueChanged.RemoveAllListeners();
        }
        if (btnGetItem != null)
        {
            btnGetItem.onClick.RemoveAllListeners();
        }
        inputLevel = null;
        btnLevelUp = null;
        btnMaxLevel = null;
        inputOccu = null;
        btnChangeOccu = null;
        btnGetStronger = null;
        inputSuitId = null;
        btnGetSuit = null;
        btnFinishTasks = null;
        inputSuitStrength = null;
        textSuitName = null;
        btnAddFatigue = null;
        inputItemId = null;
        textItemName = null;
        inputItemNumber = null;
        inputEquipStrength = null;
        inputEquipQuality = null;
        inputEquipType = null;
        inputEnhanceType = null;
        btnGetItem = null;
        if (mSelectTestToggle != null)
        {
            mSelectTestToggle.onValueChanged.RemoveAllListeners();
            mSelectTestToggle = null;
        }
    }

    bool IsEquip(string id)
    {
        var itemTable = TableManager.instance.GetTable<ItemTable>();
        foreach (var item in itemTable)
        {
            ItemTable table = item.Value as ItemTable;
            if (table.Type == ItemTable.eType.EQUIP && table.ID.ToString() == id)
                return true;
        }
        return false;
    }
}
