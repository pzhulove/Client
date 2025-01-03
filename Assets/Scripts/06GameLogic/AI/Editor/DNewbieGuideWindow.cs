using GameClient;
using UnityEditor;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System.Collections.Generic;

public class DNewbieGuideWindow : EditorWindow
{
    #region data
    NewbieGuideTable.eNewbieGuideTask NewbieGuideTask = NewbieGuideTable.eNewbieGuideTask.None;
    NewbieGuideTable.eNewbieGuideTask ResetForceGuideID = NewbieGuideTable.eNewbieGuideTask.None;
    NewbieGuideTable.eNewbieGuideTask ResetWeakGuideID = NewbieGuideTable.eNewbieGuideTask.None;
    
    string ItemName = "深渊票";
    string ItemID = "200000004";
    string ItemNum = "10";
    string SuitId = "1";
    string StrengthLevel = "10";
    string FuHuoBiID = "600000006";
    string FuHuoBiNum = "100";
    string Uplevel = "30";
    string fatask = "!!fatask";
    string FatigueNum = "120";
    string AcceptMissionID = "2271";
    string FinishMissionID = "2271";
    string Exp = "15088649782";
    string PointNum = "881103";
    string GoldNum = "881103";
    string strengthLv = "0";
    string quality = "0";
    ActorOccupation JobID = ActorOccupation.KuangZhan;
    bool breathEquipTog;
    string equipType = "0";
    string attrType = "0";
    #endregion

    #region Init
    [MenuItem("[TM工具集]/新手引导编辑器/打开新手引导编辑器")]
    static void Init()
    {
        var window = GetWindow<DNewbieGuideWindow>();
        window.position = new Rect(20, 170, 340, 600);
        window.Show();
    }
    #endregion

    #region Edit UI
    void OnGUI()
    {
        GUIControls.UIStyles.UpdateEditorStyles();

        GUILayout.Space(20);

        NewbieGuideTask = (NewbieGuideTable.eNewbieGuideTask)EditorGUILayout.EnumPopup("选择引导名称:", NewbieGuideTask);

        GUILayout.Space(20);

        if (GUILayout.Button("创建引导", "button"))
        {
            CreateNewbieGuide();
        }

        if (GUILayout.Button("断开Gate", "button"))
        {
            Network.NetManager.instance.Disconnect(ServerType.GATE_SERVER);
            var net = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.IClientNet;
            if (net != null)
            {
                net.OnDisconnect(ServerType.GATE_SERVER);
            }
        }

        if (GUILayout.Button("断开Relay", "button"))
        {
            Network.NetManager.instance.Disconnect(ServerType.RELAY_SERVER);
            var net = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.IClientNet;
            if (net != null)
            {
                net.OnDisconnect(ServerType.RELAY_SERVER);
            }

        }

        if (GUILayout.Button("退出到登陆", "button"))
        {
            ClientSystemManager.instance._QuitToLoginImpl();
        }

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        {
            ResetForceGuideID = (NewbieGuideTable.eNewbieGuideTask)EditorGUILayout.EnumPopup("重置强引导至步骤:", ResetForceGuideID);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMResetForceGuide((int)ResetForceGuideID);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            ResetWeakGuideID = (NewbieGuideTable.eNewbieGuideTask)EditorGUILayout.EnumPopup("重置弱引导:", ResetWeakGuideID);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMResetWeakGuide((int)ResetWeakGuideID);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(40);

        EditorGUILayout.BeginHorizontal();
        {
            Uplevel = EditorGUILayout.TextField("提升等级至:", Uplevel);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMUpLevel(Uplevel);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Exp = EditorGUILayout.TextField("添加经验值:", Exp);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMAddExp(Exp);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            PointNum = EditorGUILayout.TextField("添加点券数量:", PointNum);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMAddPoint(PointNum);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            GoldNum = EditorGUILayout.TextField("添加金币数量:", GoldNum);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMAddGold(GoldNum);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            AcceptMissionID = EditorGUILayout.TextField("强制接取任务ID:", AcceptMissionID);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMForceAcceptMission(AcceptMissionID);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            FinishMissionID = EditorGUILayout.TextField("强制完成任务ID:", FinishMissionID);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMForceFinishMission(FinishMissionID);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            fatask = EditorGUILayout.TextField("完成当前已接取的任务:", fatask);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMFaTask(fatask);
            }
        }
        EditorGUILayout.EndHorizontal();

        
        
        ItemID = EditorGUILayout.TextField("添加道具ID(默认深渊票):", ItemID);
        breathEquipTog = EditorGUILayout.Toggle("创建(气息装备或者红字装备)",breathEquipTog);
        if (IsEquip(ItemID))
        {
            strengthLv = EditorGUILayout.TextField("强化等级:", strengthLv);
            quality = EditorGUILayout.TextField("品级:", quality);
        }
        
        if (IsEquip(ItemID) && breathEquipTog)
        {
            equipType = EditorGUILayout.TextField("装备类型(0：普通装备 1：气息装备 2：红字装备)", equipType);
            attrType = EditorGUILayout.TextField("红字装备属性类型", attrType);
        }

        EditorGUILayout.BeginHorizontal();
        {
            ItemNum = EditorGUILayout.TextField("道具数量:", ItemNum);

            if (GUILayout.Button("确定", "button"))
            {
                if (IsEquip(ItemID) && !breathEquipTog)
                {
                    if (NetManager.Instance() != null)
                        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1} str={2} ql={3}", ItemID, ItemNum, strengthLv, quality));
                }
                else if (IsEquip(ItemID) && breathEquipTog)
                {
                    if (NetManager.Instance() != null)
                        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addee id={0} num={1} ql={2} str={3} et={4} ent={5}", ItemID, ItemNum, quality, strengthLv, equipType, attrType));
                }
                else
                {
                    SendGMAddItem(ItemID, ItemNum);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        ItemName = EditorGUILayout.TextField("道具名", ItemName);
        if (GUILayout.Button("搜索并发送道具", "button", GUILayout.Width(150)))
        {
            CreateProp("additem id={0} num={1}", ItemName, new object[]{null, ItemNum});
        }
        
        OneKeyWearTable onKeyWearData = null;
        SuitId = EditorGUILayout.TextField("套装ID:", SuitId);
        if (!string.IsNullOrEmpty(SuitId))
            onKeyWearData = TableManager.GetInstance().GetTableItem<OneKeyWearTable>(int.Parse(SuitId));

        if (onKeyWearData != null)
        {
            string suitInfo = string.Format("适用职业:{0}  套装名称:{1}", onKeyWearData.JobName, onKeyWearData.Name);
            EditorGUILayout.LabelField(suitInfo);

            EditorGUILayout.BeginHorizontal();
            {
                StrengthLevel = EditorGUILayout.TextField("套装强化等级:", StrengthLevel);
                if (GUILayout.Button("确定", "button"))
                {
                    Utility.UseOneKeySuitWear(int.Parse(SuitId), int.Parse(StrengthLevel), (int)onKeyWearData.EquipType, (int)onKeyWearData.EnhanceType);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        {
            FuHuoBiNum = EditorGUILayout.TextField("添加复活币数量:", FuHuoBiNum);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMAddItem(FuHuoBiID, FuHuoBiNum);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            FatigueNum = EditorGUILayout.TextField("添加体力值:", FatigueNum);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMFatigue(FatigueNum);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            JobID = TMEnumHelper.GetChangeEnumType("转职:", JobID);

            if (GUILayout.Button("确定", "button"))
            {
                SendGMChangeJob(((int)JobID).ToString());
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    protected void CreateProp(string format, string parameterContent, object[] objects)
    {
        if (format == null)
            return;

        if (parameterContent == null)
            return;

        if (objects == null)
            return;

        if (objects.Length <= 0)
            return;

        var itemTable = TableManagerEditor.GetInstance().GetTable<ProtoTable.ItemTable>();
        if (itemTable != null)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var item in itemTable)
            {
                var current = item.Value as ProtoTable.ItemTable;
                if (System.Text.RegularExpressions.Regex.IsMatch(current.Name, parameterContent, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    menu.AddItem(new GUIContent(current.Name + "ID:" + current.ID), false, (object obj) =>
                    {
                        objects[0] = obj;
                        var command = string.Format(format, objects);
                        ChatToolWindow._sendGM(command);
                    }, current.ID);
                }

            }
            menu.ShowAsContext();
        }
    }

    #region Create Guide
    void CreateNewbieGuide()
    {
        NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTask);
        if (tabledata == null)
        {
            Logger.LogErrorFormat("无法创建引导---引导ID错误:{0}", NewbieGuideTask);
            return;
        }

        NewbieGuideManager.GetInstance().DoGuideByEditor(tabledata);
    }
    #endregion

    #region GM commond
    void SendGMAddItem(string param0, string param1)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", param0, param1));
    }

    void SendGMUpLevel(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!uplevel num={0}", param0));
    }

    void SendGMFaTask(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("{0}", param0));
    }

    void SendGMFatigue(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addfatigue num={0}", param0));
    }

    void SendGMForceAcceptMission(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!accepttask id={0} force=1", param0));
    }

    void SendGMForceFinishMission(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!submittask id={0} force=1", param0));
    }

    void SendGMAddExp(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addexp num={0}", param0));
    }

    void SendGMAddPoint(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addpoint num={0}", param0));
    }

    void SendGMAddGold(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addgold num={0}", param0));
    }

    void SendGMResetForceGuide(int param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!setboot id={0}", param0));
    }

    void SendGMResetWeakGuide(int param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!resetbootflag id={0}", param0));
    }

    void SendGMChangeJob(string param0)
    {
        ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!changeoccu occu={0}", param0));
    }
    #endregion

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
