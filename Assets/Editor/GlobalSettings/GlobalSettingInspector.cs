using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using GameClient;

[CustomEditor(typeof(GlobalSetting))]
public class GlobalSettingInspector : ComEditor
{
#if !LOGIC_SERVER
    [MenuItem("全局配置/显示全局配置")]
    public static void Create()
    {
        //FileTools.CreateAsset<GlobalSetting>("GlobalSettings");
        //return ;

        var pinObject = Resources.Load<GlobalSetting>(PathUtil.EraseExtension(Global.PATH)) as GlobalSetting;
        if (pinObject == null)
        {
            // TODO save to the Global.PATH
            pinObject = FileTools.CreateAsset<GlobalSetting>(Global.PATH);
        }
        EditorGUIUtility.PingObject(pinObject);
        Selection.activeObject = pinObject;

    }

   // [MenuItem("全局配置/属性操作有Z轴速度的帧Length替换成1")]
    public static void ChangeLengthToOne()
    {
        Object[] list = Selection.GetFiltered(typeof(DSkillData), SelectionMode.DeepAssets);

        for (int i = 0; i < list.Length; i++)
        {
            DSkillData data = list[i] as DSkillData;
            if (data.properModify.Length >= 1)
            {
                for (int j = 0; j < data.properModify.Length; j++)
                {
                    DSkillPropertyModify modify = data.properModify[j];
                    if ((modify.modifyfliter == DSkillPropertyModifyType.SPEED_Z || modify.modifyfliter == DSkillPropertyModifyType.SPEED_ZACC) && modify.length > 1)
                    {
                        modify.length = 1;
                    }
                }
            }
            EditorUtility.DisplayProgressBar("Simple Progress Bar", i + "/" + list.Length, (float)i / list.Length);
        }
        AssetDatabase.SaveAssets();
        UnityEditor.EditorUtility.ClearProgressBar();
    }

    [MenuItem("全局配置/查找所有多个帧标签的文件")]
    public static void CheckAllMultipleTagsDSkillData()
    {
        Object[] list = Selection.GetFiltered(typeof(DSkillData), SelectionMode.DeepAssets);

        for (int i = 0; i < list.Length; i++)
        {
            DSkillData data = list[i] as DSkillData;
            var frameTags = data.frameTags;
            for (int j = 0; j < frameTags.Length; j++)
            {
                var tag = (int)frameTags[j].tag;
                switch (tag)
                {
                    case 1:
                        frameTags[j].tag = DSFFrameTags.TAG_NEWDAMAGE;
                        break;
                    case 2:
                        frameTags[j].tag = DSFFrameTags.TAG_LOCKZSPEED;
                        break;
                    case 4:
                        frameTags[j].tag = DSFFrameTags.TAG_LOCKZSPEEDFREE;
                        break;
                    case 8:
                        frameTags[j].tag = DSFFrameTags.TAG_IGNORE_GRAVITY;
                        break;
                    case 16:
                        frameTags[j].tag = DSFFrameTags.TAG_RESTORE_GRAVITY;
                        break;
                    case 32:
                        frameTags[j].tag = DSFFrameTags.TAG_SET_TARGET_POS_XY;
                        break;
                    case 64:
                        frameTags[j].tag = DSFFrameTags.TAG_CURFRAME;
                        break;
                    case 128:
                        frameTags[j].tag = DSFFrameTags.TAG_CHANGEFACE;
                        break;
                    case 256:
                        frameTags[j].tag = DSFFrameTags.TAG_CHANGEFACEBYDIR;
                        break;
                    default:
                        frameTags[j].tag = DSFFrameTags.TAG_NEWDAMAGE;
                        break;
                }
            }
            data.frameTags = frameTags;
            EditorUtility.DisplayProgressBar("Simple Progress Bar", i + "/" + list.Length, (float)i / list.Length);
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }

    private List<string> serverList = new List<string>();
    private bool bShowServerList = false;
    private string[] pidString;
    private System.Array pidValues;

    private bool mIsShowCommon = true;
    private bool mIsShowFilter = false;
    private bool mIsShowBattleDetail = false;
    private bool mIsShowShockSection = false;
    private bool mIsShowEquipRate = false;
    private bool mIsShowGameTest = false;

    private bool mIsOpenMark = true;
    private bool mIsReplayMode = false;
    private string mRecordMarkPath = string.Empty;
    private string mMarkReadableFilePath = string.Empty;

    private bool mCreamaSettingFoldout = false;

    enum PageEnum
    {
        Base,
        Game,
        Testing,
        Battle,
        EquipmentSorce,
        ConvertTable,
        Num
    }

    void _commonSetting(GlobalSetting data)
    {
		data.mainSDKChannel = (SDKChannel)EditorGUILayout.EnumPopup("主要SDK渠道选择", data.mainSDKChannel);
        data.sdkChannel = (SDKChannel)EditorGUILayout.EnumPopup("接入SDK选择", data.sdkChannel);
        data.isPaySDKDebug = EditorGUILayout.Toggle("是否开启SDK支付DEBUG", data.isPaySDKDebug);
        //data.isBanShuVersion = EditorGUILayout.Toggle("是否是版属版本", data.isBanShuVersion);
        data.isDebug = EditorGUILayout.Toggle("是否是DEBUG", data.isDebug);
        data.isRecordPVP = EditorGUILayout.Toggle("是否开启PVP录像", data.isRecordPVP);
        data.showDialog = EditorGUILayout.Toggle("是否显示调试弹窗", data.showDialog);

        data.recordResFile = EditorGUILayout.Toggle("是否记录加载文件", data.recordResFile);
        data.profileAssetLoad = EditorGUILayout.Toggle("是否统计文件加载耗时", data.profileAssetLoad);

        data.isGuide = EditorGUILayout.Toggle("是否启用引导", data.isGuide);
        data.isAnimationInto = EditorGUILayout.Toggle("是否启用按钮解锁飞入动画", data.isAnimationInto);

#if UNITY_EDITOR
        data.CloseLoginPushFrame = EditorGUILayout.Toggle("# 禁用进入主城时的弹窗 #",data.CloseLoginPushFrame);
#endif
        data.displayHUD = EditorGUILayout.Toggle("是否显示调试信息", data.displayHUD);
        data.CloseTeamCondition = EditorGUILayout.Toggle("是否关闭组队条件限制", data.CloseTeamCondition);

        data.showDebugBox = EditorGUILayout.Toggle("是否显示碰撞盒", data.showDebugBox);
        data.isTestTeam = EditorGUILayout.Toggle("是否开启组队测试", data.isTestTeam);

        data.frameLock = EditorGUILayout.IntField("锁定帧率", data.frameLock);
        data.fallgroundHitFactor = EditorGUILayout.FloatField("倒地击飞距离缩短百分比", data.fallgroundHitFactor);

        data.defaultHitEffect = EditorGUILayout.TextField("默认击中特效", data.defaultHitEffect);
        data.defaultProjectileHitEffect = EditorGUILayout.TextField("默认投射物击中特效", data.defaultProjectileHitEffect);
        data.defualtHitSfx = EditorGUILayout.TextField("默认击中音效", data.defualtHitSfx);

        data.fallProtectGravityAddFactor = EditorGUILayout.FloatField("浮空保护重力增加因子", mGlobalSettingTable.GetFloatValue1000("fallProtectGravityAddFactor"));
        data.defaultFloatHurt = EditorGUILayout.FloatField("浮空保护触发百分比", mGlobalSettingTable.GetFloatValue10000("defaultFloatHurt"));
        data.defaultFloatLevelHurat = EditorGUILayout.FloatField("浮空保护触发后再触发", mGlobalSettingTable.GetFloatValue10000("defaultFloatLevelHurat"));
        data.defaultGroundHurt = EditorGUILayout.FloatField("倒地保护触发百分比", mGlobalSettingTable.GetFloatValue10000("defaultGroundHurt"));
        data.defaultStandHurt = EditorGUILayout.FloatField("站立保护触发百分比", mGlobalSettingTable.GetFloatValue10000("defaultStandHurt"));
        data.protectClearDuration = (int)(EditorGUILayout.FloatField("保护重置间隔", mGlobalSettingTable.GetFloatValue("protectClearDuration", 1) / 1000f) * 1000);

        //data.zDimFactor = EditorGUILayout.FloatField("z轴通用缩放系数", data.zDimFactor);
        data.snapDuration = EditorGUILayout.FloatField("残影存留时间", data.snapDuration);

        data.bullteScale = EditorGUILayout.Slider("子弹时间时间缩放系数", data.bullteScale, 0.01f, 0.99f);
        data.bullteTime = EditorGUILayout.IntSlider("子弹时间持续时间", data.bullteTime, 1000, 5000);

        data.PVPHPScale = EditorGUILayout.IntField("PVP模式HP缩放", data.PVPHPScale);

        data.minHurtTime = EditorGUILayout.IntField("被击最小时间", data.minHurtTime);
        data.maxHurtTime = EditorGUILayout.IntField("被击最大时间", data.maxHurtTime);
        data.frozenPercent = EditorGUILayout.FloatField("僵直系数", data.frozenPercent);

        var spd = mGlobalSettingTable.GetValue<Vec3>("walkSpeed");
        //var spd = data.walkSpeed;
        var spd2 = Vector3.zero;

        spd = data.townWalkSpeed;
        spd2 = EditorGUILayout.Vector3Field("城镇行走速度", new Vector3(spd.x, spd.y, spd.z));
        data.townWalkSpeed = new Vec3(spd2.x, spd2.y, spd.z);

        spd = data.townRunSpeed;
        spd2 = EditorGUILayout.Vector3Field("城镇奔跑速度", new Vector3(spd.x, spd.y, spd.z));
        data.townRunSpeed = new Vec3(spd2.x, spd2.y, spd.z);

        data.drift = EditorGUILayout.FloatField("网络:drift",data.drift);
        data.logicFrameStepDelta = EditorGUILayout.IntField("网络:logicFrameStepDelta", data.logicFrameStepDelta);
        data.logicFrameStep = (uint)EditorGUILayout.IntField("网络:logicFrameStep", (int)data.logicFrameStep);
        data.gateReconnectTimeOut = EditorGUILayout.FloatField("GateServer超时时长:", data.gateReconnectTimeOut);
        data.gateReconnectSendTryCount = EditorGUILayout.IntField("GateServer重发次数:", data.gateReconnectSendTryCount);
        data.relayReconnectTimeOut = EditorGUILayout.FloatField("RelayServer超时时长:", data.relayReconnectTimeOut);
        data.relayReconnectSendTryCount = EditorGUILayout.IntField("RelayServer重发次数:", data.relayReconnectSendTryCount);

        data.townActionSpeed = EditorGUILayout.FloatField("城镇奔跑播放速度", data.townActionSpeed);

        data.townPlayerRun = EditorGUILayout.Toggle("城镇角色是否奔跑", data.townPlayerRun);

        data.jumpBackSpeed = EditorGUILayout.Vector2Field("后跳速度", data.jumpBackSpeed);

        data.jumpForce = EditorGUILayout.FloatField("弹跳力", data.jumpForce);
        data.clickForce = EditorGUILayout.FloatField("点击力", data.clickForce);
        data.enemyHasAI = EditorGUILayout.Toggle("怪物是否有AI", data.enemyHasAI);
        //data.monsterWalkSpeedFactor = EditorGUILayout.FloatField("怪物全局速度调整倍率", data.monsterWalkSpeedFactor);
        data.monsterSightFactor = EditorGUILayout.FloatField("怪物全局视野调整倍率", mGlobalSettingTable.GetFloatValue1000("monsterSightFactor"));

        data.gravity = EditorGUILayout.FloatField("全局重力", mGlobalSettingTable.GetFloatValue("gravity", 1));
        data.fallGravityReduceFactor = EditorGUILayout.FloatField("击飞掉落重力百分比", mGlobalSettingTable.GetFloatValue10000("fallGravityReduceFactor"));
        data.skillHasCooldown = EditorGUILayout.Toggle("是否有技能CD", data.skillHasCooldown);

        data.cameraInRange = EditorGUILayout.Vector2Field("场景相机跟随缓存", data.cameraInRange);

        data.avatarLightDir = EditorGUILayout.Vector3Field("角色界面灯光方向", data.avatarLightDir);
        data.shadowLightDir = EditorGUILayout.Vector3Field("角色阴影灯光方向", data.shadowLightDir);

        data.buttonType = (InputManager.ButtonMode)EditorGUILayout.EnumPopup("按钮模式选择", data.buttonType);

        data.bgmStart = EditorGUILayout.FloatField("开始背景音", Mathf.Clamp01(data.bgmStart));
        data.bgmTown = EditorGUILayout.FloatField("城镇背景音", Mathf.Clamp01(data.bgmTown));
        data.bgmBattle = EditorGUILayout.FloatField("战斗背景音", Mathf.Clamp01(data.bgmBattle));


        data.startVel = EditorGUILayout.Vector3Field("开始速度", data.startVel);
        data.endVel = EditorGUILayout.Vector3Field("结束速度", data.endVel);
        data.offset = EditorGUILayout.Vector3Field("offset", data.offset);
        data.TimeAccerlate = EditorGUILayout.FloatField("TimeAccerlate", data.TimeAccerlate);
        data.TotalTime = EditorGUILayout.IntField("TotalTime", data.TotalTime);
        data.TotalDist = EditorGUILayout.IntField("TotalDist", data.TotalDist);

        data.enableAssetInstPool = EditorGUILayout.Toggle("启用资源实例池", data.enableAssetInstPool);
        data.heightAdoption = EditorGUILayout.Toggle("摄像机高度适配", data.heightAdoption);
        mCreamaSettingFoldout = EditorGUILayout.Foldout(mCreamaSettingFoldout, "相机默认参数设置（编辑器用）");
        if(mCreamaSettingFoldout)
        {
            data.battleCameraNearClip = EditorGUILayout.FloatField("战斗相机默认近裁剪", data.battleCameraNearClip);
            data.battleCameraFarClip = EditorGUILayout.FloatField("战斗相机默认远裁剪", data.battleCameraFarClip);
            data.townCameraNearClip = EditorGUILayout.FloatField("城镇相机默认近裁剪", data.townCameraNearClip);
            data.townCameraFarClip = EditorGUILayout.FloatField("城镇相机默认远裁剪", data.townCameraFarClip);
        }

        data.notifyItemTimeLess = EditorGUILayout.IntField("提示道具过期时间（秒）", data.notifyItemTimeLess);

		data.petDialogLife = EditorGUILayout.FloatField("宠物冒泡持续时间", data.petDialogLife);
		data.petDialogShowInterval = EditorGUILayout.FloatField("宠物冒泡时间间隔", data.petDialogShowInterval);
		data.petSpecialIdleInterval = EditorGUILayout.FloatField("宠物特殊待机时间间隔", data.petSpecialIdleInterval);
        data.defualtChannel = EditorGUILayout.TextField("默认渠道", data.defualtChannel);
    }

    private void _loginSetting(GlobalSetting data)
    {
        data.showBattleInfoPanel = EditorGUILayout.Toggle("是否开启战斗信息界面", data.showBattleInfoPanel);

#region ip list

        serverList.Clear();
        for (int i = 0; i < data.serverList.Length; i++)
        {
            serverList.Add(string.Format("[{0}]{1}", i, data.serverList[i].ToString()));
        }

        data.ipSelectedIndex = EditorGUILayout.Popup("服务器列表", data.ipSelectedIndex, serverList.ToArray());

        bShowServerList = EditorGUILayout.Foldout(bShowServerList, "服务器列表详情");
        if (bShowServerList)
        {
            int arrayLen = data.serverList.Length;
            int ipCount = EditorGUILayout.IntField("登录服务器列表", arrayLen);

            if (ipCount != arrayLen)
            {
                List<GlobalSetting.Address> ipList = new List<GlobalSetting.Address>(data.serverList);

                if (ipCount > arrayLen)
                {
                    for (int i = arrayLen; i < ipCount; i++)
                    {
                        var item = data.serverList[arrayLen - 1];
                        ipList.Add(new GlobalSetting.Address() { ip = item.ip, port = item.port });
                    }
                }
                else
                {
                    ipList.RemoveRange(ipCount, arrayLen - ipCount);
                }
                data.serverList = ipList.ToArray();
            }

            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < ipCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        data.serverList[i].ip = EditorGUILayout.TextField("登录服务器地址", data.serverList[i].ip);
                        GUILayoutUtility.GetRect(1, 1);
                        data.serverList[i].port = (ushort)EditorGUILayout.IntField(data.serverList[i].port, GUILayout.Width(80.0f));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

        }
#endregion
    }
    private void _gameSettingGUI(GlobalSetting data)
    {
        _logFilter(data);
        EditorGUILayout.LabelField("游戏配置", fontStyle);
        data.startSystem = (GameClient.EClientSystem)EditorGUILayout.EnumPopup("初始状态选择", data.startSystem);

        EditorGUI.indentLevel++;
        switch (data.startSystem)
        {
            case GameClient.EClientSystem.Login:
                _loginSetting(data);
                break;
            case GameClient.EClientSystem.Town:
                break;
            case GameClient.EClientSystem.Battle:
                _battleSetting(data);
                break;
            default:
                break;
        }

        EditorGUILayout.LabelField("打包热更新设置", fontStyle);
        data.loadFromPackage = EditorGUILayout.Toggle("从AssetBundle加载资源", data.loadFromPackage);
        data.enableHotFix = EditorGUILayout.Toggle("启用热更新流程", data.enableHotFix);
        if (data.enableHotFix)
        {
            data.hotFixUrlDebug = EditorGUILayout.Toggle("是否是调试热更新地址", data.hotFixUrlDebug);
        }
        if (data.loadFromPackage)
        {
            data.isRecordAB = EditorGUILayout.Toggle("是否记录AB包加载记录", data.isRecordAB);
        }


        EditorGUILayout.LabelField("进度条加载", fontStyle);
        data.loadingProgressStepInEditor = EditorGUILayout.IntField("进度条更新步长（编辑器下）", data.loadingProgressStepInEditor);
        data.loadingProgressStep = EditorGUILayout.IntField("进度条更新步长", data.loadingProgressStep);
        EditorGUILayout.Space();

        Texture2D old = data.globalRamp;
        data.globalRamp = EditorGUILayout.ObjectField("全局Ramp图", data.globalRamp, typeof(Texture2D)) as Texture2D;
        data.serverListUrl = EditorGUILayout.TextField("登录服务器http服务器地址", data.serverListUrl);

        EditorGUI.indentLevel--;
    }
    PageEnum current;

    void PageShow(PageEnum key, GUIContent contentshow)
    {
        bool bCheck = ToggleButton(current == key, contentshow);
        if (bCheck)
        {
            current = key;
        }
    }

    public override void OnInspectorGUI()
    {
        OnBaseInspectorGUI();

        if (current < PageEnum.Base || current >= PageEnum.Num)
        {
            current = PageEnum.Base;
        }

        // show the path of the data

        // show the data button

        GlobalSetting data = target as GlobalSetting;

        if (data == null)
        {
            return;
            //data = target as GlobalData;
            //Global.Settings = data;
        }

        using (new GUILayout.HorizontalScope("GroupBox"))
        {
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField("配置路径", Global.PATH);
                mGlobalSettingTable.OnGUIWithID(1);
            }
            // if (GUILayout.Button("定位"))
            // {
            //     EditorGUIUtility.PingObject(data);
            //     Selection.activeObject = data;
            // }
        }


        EditorGUILayout.Space();
        DrawBox(Color.white, 3);
        GUILayout.BeginHorizontal();
        PageShow(PageEnum.Base, new GUIContent("基础", "基础信息配置"));
        PageShow(PageEnum.Game, new GUIContent("游戏启动", "游戏启动配置"));
        PageShow(PageEnum.Testing, new GUIContent("测试", "测试配置"));
        PageShow(PageEnum.Battle, new GUIContent("战斗", "战斗细节配置"));
        PageShow(PageEnum.EquipmentSorce, new GUIContent("装备评分", "装备评分配置"));
        PageShow(PageEnum.ConvertTable, new GUIContent("转表", "转表配置"));
        GUILayout.EndHorizontal();

        switch (current)
        {
            case PageEnum.Base:
                {
                    _commonSetting(data);
                }
                break;
            case PageEnum.Game:
                {
                    _gameSettingGUI(data);
                }
                break;
            case PageEnum.Testing:
                {
                    _gameTestSetting(data);
                }
                break;
            case PageEnum.Battle:
                {
                    _battleDetailSetting(data);
                }
                break;
            case PageEnum.EquipmentSorce:
                {
                    _equipRateScoreSetting(data);
                }
                break;
            case PageEnum.ConvertTable:
                {
                    _convertTableSetting(data);
                }
                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }

    }

    private bool _textRegexFiled(string name, ref string str)
    {
        EditorGUI.BeginChangeCheck();
        var s = EditorGUILayout.TextField(name, str);

        if (EditorGUI.EndChangeCheck())
        {
            try
            {
                var r = new Regex(@s);
                str = s;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        return false;
    }

    private void _logFilter(GlobalSetting data)
    {
        mIsShowFilter = EditorGUILayout.BeginToggleGroup("LOG正则过滤", mIsShowFilter);

        if (mIsShowFilter)
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            GUI.color = Color.yellow;
            _textRegexFiled("函数名", ref data.loggerFilter[0]);
            GUI.color = Color.green;
            _textRegexFiled("文件名", ref data.loggerFilter[1]);
            GUI.color = Color.magenta;
            _textRegexFiled("类名", ref data.loggerFilter[2]);
            GUI.color = Color.cyan;
            _textRegexFiled("模块名", ref data.loggerFilter[3]);
            GUI.color = Color.white;
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(data);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndToggleGroup();

    }


    private void _battleSetting(GlobalSetting data)
    {
        try
        {
            if (pidString == null)
            {
                pidString = typeof(GameClient.ActorOccupation).GetDescriptions();
                pidValues = typeof(GameClient.ActorOccupation).GetValues();
            }

            int iIndex = System.Array.BinarySearch(pidValues, (GameClient.ActorOccupation)data.iSingleCharactorID);
            if (iIndex < 0 || iIndex >= pidString.Length)
            {
                iIndex = 0;
                data.iSingleCharactorID = (int)pidValues.GetValue(iIndex);
            }

            iIndex = EditorGUILayout.Popup("单机战斗默认角色ID", iIndex, pidString);

            data.iSingleCharactorID = (int)pidValues.GetValue(iIndex);
        }
        catch
        {

        }

        data.TestLevel = EditorGUILayout.IntField("测试等级", data.TestLevel);
        data.testPlayerNum = EditorGUILayout.IntField("测试玩家数量", data.testPlayerNum);

        data.isGiveEquips = EditorGUILayout.Toggle("是否给角色装备", data.isGiveEquips);
        if (data.isGiveEquips)
        {
            data.equipList = EditorGUILayout.TextField("装备列表(,隔开)", data.equipList);
            data.switchEquipList = EditorGUILayout.TextField("切换武器列表(,隔开)", data.switchEquipList);
        }

        data.debugDrawBlock = EditorGUILayout.Toggle("显示地面阻挡", data.debugDrawBlock);

        data.showBattleInfoPanel = EditorGUILayout.Toggle("是否开启战斗信息界面", data.showBattleInfoPanel);
        if (data.showBattleInfoPanel)
        {
            data.defaultMonsterID = EditorGUILayout.IntField("默认创建怪物ID", data.defaultMonsterID);
        }

        data.isCreateMonsterLocal = EditorGUILayout.Toggle("单机是否创建怪物", data.isCreateMonsterLocal);

        data.isLocalDungeon = EditorGUILayout.Toggle("是否启用单机关卡", data.isLocalDungeon);
        data.localDungeonID = mDungeonTable.OnGUIWithID(data.localDungeonID);
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (GUILayout.Button(string.Format("加载关卡:{0}", data.localDungeonID),"minibutton"))
            {
                ClientSystemManager.instance.SwitchSystem<ClientSystemBattle>(null,null,true);
            }
        }
        else
        {
            if (GUILayout.Button(string.Format("开始"), "minibutton"))
            {
                EditorApplication.isPlaying = true;
            }
        }
       
        data.scenePath = EditorGUILayout.TextField("战斗场景默认配置文件路径", data.scenePath);
    }

    private TableInspetor<ProtoTable.DungeonTable> mDungeonTable = new TableInspetor<ProtoTable.DungeonTable>("单机关卡ID(地下城表)", true);

    private TableInspetor<ProtoTable.GlobalSettingTable> mGlobalSettingTable = new TableInspetor<ProtoTable.GlobalSettingTable>("客户端全局配置表", true);

    private void _equipRateScoreSetting(GlobalSetting data)
    {

        GUILayout.Label("属性分值设定:");
        for (int i = 0; i < Global.equipPropName.Count; ++i)
        {
            AttributeType at = (AttributeType)System.Enum.Parse(typeof(AttributeType), Global.equipPropName[i]);
            data.equipPropFactorValues[i] = EditorGUILayout.FloatField(Utility.GetEnumDescription<AttributeType>(at).Replace("{0}", ""), data.equipPropFactorValues[i]);
        }

        GUILayout.Label("部位系数设定:");
        /*
        var array = System.Enum.GetNames(typeof(ProtoTable.ItemTable.eThirdType) );
        foreach(var field in array)
        {
            if (data.quipThirdTypeFactors.ContainsKey(field))
                data.quipThirdTypeFactors[field] = EditorGUILayout.FloatField(Global.equipThirdTypeNames[field], data.quipThirdTypeFactors[field]);
        }*/
        for (int i = 0; i < Global.equipThirdTypeNamesList.Count; ++i)
        {
            string field = Global.equipThirdTypeNamesList[i];
            data.quipThirdTypeFactorValues[i] = EditorGUILayout.FloatField(Global.equipThirdTypeNames[field], data.quipThirdTypeFactorValues[i]);
        }



    }

    private void _convertTableSetting(GlobalSetting data)
    {
        GUILayout.Label("服务器转表设置:");
        data.serverCodePath = EditorGUILayout.TextField("服务器代码路径", data.serverCodePath);
    }

    private void _battleDetailSetting(GlobalSetting data)
    {

        data.testFashionEquip = EditorGUILayout.Toggle("是否开启城镇玩家穿时装", data.testFashionEquip);
        data.aiHotReload = EditorGUILayout.Toggle("是否动态加载AI", data.aiHotReload);
        data.damageNoRange = EditorGUILayout.Toggle("伤害不要浮动", data.damageNoRange);
        data.sceneDark = EditorGUILayout.Toggle("场景变黑", data.sceneDark);

        data.openBossShow = EditorGUILayout.Toggle("是否播BOSS出场动画", data.openBossShow);

        data.doorEffPrefix = EditorGUILayout.TextField("传送门特效名称前缀(用|分开)", data.doorEffPrefix);

        data.isTrainingAIOpen = EditorGUILayout.Toggle("练习场木桩AI是否开启",data.isTrainingAIOpen);
        if (data.isTrainingAIOpen)
        {
            data.trainingAIConfigId = EditorGUILayout.IntField("机器人AI表ID", data.trainingAIConfigId);
            data.trainingRobotId = EditorGUILayout.IntField("机器人配置表ID", data.trainingRobotId);
        }

        mIsShowShockSection = EditorGUILayout.BeginToggleGroup("展开抖动设置", mIsShowShockSection);
        if (mIsShowShockSection)
        {
            showShockData("主角被击抖动:", ref data.playerBeHitShockData);
            showShockData("主角CD中抖动:", ref data.playerSkillCDShockData);
            showShockData("怪物被击抖动:", ref data.monsterBeHitShockData);
            showShockData("人物高度掉落震动:", ref data.playerHighFallTouchGroundShockData);
            data.highFallHight = EditorGUILayout.FloatField("掉落震动高度设置", data.highFallHight);
        }
        EditorGUILayout.EndToggleGroup();

        data.critialDeadEffect = EditorGUILayout.TextField("暴击死亡特效", data.critialDeadEffect);
        data.immediateDeadEffect = EditorGUILayout.TextField("秒杀死亡特效", data.immediateDeadEffect);
        data.normalDeadEffect = EditorGUILayout.TextField("普通死亡特效", data.normalDeadEffect);
        data.enableEffectLimit = EditorGUILayout.Toggle("开启特效限制", data.enableEffectLimit);
        data.effectLimitCount = EditorGUILayout.IntField("特效限制数目上线", data.effectLimitCount);
        data.immediateDeadHPPercent = EditorGUILayout.IntField("秒杀血量百分比(100)", data.immediateDeadHPPercent);

        data.deadWhiteTime = EditorGUILayout.FloatField("死亡变白时间", data.deadWhiteTime);
        data.hitTime = EditorGUILayout.FloatField("默认击中高亮时间", data.hitTime);

        data.shooterFitPercent = EditorGUILayout.FloatField("枪手向下射击Y范围削减因子", data.shooterFitPercent);

        GUILayout.Label("随从设置:");
        data.disappearDis = EditorGUILayout.Vector3Field("随从消失距离", data.disappearDis);
        data.keepDis = EditorGUILayout.FloatField("随从出现距离", data.keepDis);
        data.accompanyRunTime = EditorGUILayout.FloatField("随从跑动时间", data.accompanyRunTime);

        GUILayout.Label("AI设置:");
        data.aiWanderRange            = EditorGUILayout.IntField("默认游荡范围",           mGlobalSettingTable.GetValue<int>("aiWanderRange"));
        data.aiWAlkBackRange          = EditorGUILayout.IntField("默认后退范围",           mGlobalSettingTable.GetValue<int>("aiWAlkBackRange"));
        data.aiMaxWalkCmdCount        = EditorGUILayout.IntField("进战最大连续行走指令",   mGlobalSettingTable.GetValue<int>("aiMaxWalkCmdCount"));
        data.aiMaxWalkCmdCount_RANGED = EditorGUILayout.IntField("远程最大连续行走指令",   mGlobalSettingTable.GetValue<int>("aiMaxWalkCmdCount_RANGED"));
        data.aiMaxIdleCmdcount        = EditorGUILayout.IntField("最大连续IDLE指令",       mGlobalSettingTable.GetValue<int>("aiMaxIdleCmdcount"));
        data.aiSkillAttackPassive     = EditorGUILayout.IntField("技能攻击被动目标的概率", mGlobalSettingTable.GetValue<int>("aiSkillAttackPassive"));

        GUILayout.Label("角色参数：");
        data.dunFuTime    = EditorGUILayout.FloatField("PVE蹲伏最大时间", mGlobalSettingTable.GetFloatValue1000("dunFuTime"));
        data.pvpDunFuTime = EditorGUILayout.FloatField("PVP蹲伏最大时间", mGlobalSettingTable.GetFloatValue1000("pvpDunFuTime"));

        data.jumpAttackLimitHeight = EditorGUILayout.FloatField("角色跳跃攻击最低高度", data.jumpAttackLimitHeight);
        data.charScale = EditorGUILayout.FloatField("战斗中角色缩放", data.charScale);
        data.rollSpeed = EditorGUILayout.Vector2Field("怪物滚的速度", data.rollSpeed);
        data.rollRand = EditorGUILayout.FloatField("版边怪物滚的概率", data.rollRand);
        data.normalRollRand = EditorGUILayout.FloatField("平时怪物滚的概率", data.normalRollRand);

        data.monsterGetupBatiFactor = EditorGUILayout.FloatField("怪物起身霸体概率(0-1)", mGlobalSettingTable.GetFloatValue1000("monsterGetupBatiFactor"));


        GUILayout.Label("自动战斗:");
        data.forceUseAutoFight = EditorGUILayout.Toggle("强制开启自动战斗", data.forceUseAutoFight);
        data.afThinkTerm              = EditorGUILayout.IntField("攻击判定间隔(ms)",                 mGlobalSettingTable.GetValue<int>("afThinkTerm"));
        data.afFindTargetTerm         = EditorGUILayout.IntField("寻找攻击目标间隔(ms)",             mGlobalSettingTable.GetValue<int>("afFindTargetTerm"));
        data.afChangeDestinationTerm  = EditorGUILayout.IntField("寻路间隔(ms)",                     mGlobalSettingTable.GetValue<int>("afChangeDestinationTerm"));
        data.autoCheckRestoreInterval = EditorGUILayout.IntField("摇杆松开后恢复自动战斗的间隔(ms)", mGlobalSettingTable.GetValue<int>("autoCheckRestoreInterval"));

        data.loadAutoFight            = EditorGUILayout.Toggle("自否自动加载上次设置", data.loadAutoFight);

        GUILayout.Label("技能：");
        data.skillCancelLimitTime = EditorGUILayout.FloatField("技能取消最小时间", data.skillCancelLimitTime);

        GUILayout.Label("摇杆配置:");

        data.doublePressCheckDuration = EditorGUILayout.IntField("摇杆双击判定间隔(ms)", data.doublePressCheckDuration);
        data.changeFaceStop = EditorGUILayout.Toggle("转向是否停止奔跑", data.changeFaceStop);
        //var spd = data.walkSpeed;
        var spd = mGlobalSettingTable.GetValue<Vec3>("walkSpeed");
        var spd2 = Vector3.zero;

        List<string> actionList = new List<string>() { "ActionType_WALK", "ActionType_RUN" };

        data.walkAction = EditorGUILayout.Popup("行走动画选择", (data.walkAction == ActionType.ActionType_WALK ? 0 : 1), actionList.ToArray()) == 0 ? ActionType.ActionType_WALK : ActionType.ActionType_RUN;
        spd2 = EditorGUILayout.Vector3Field("行走速度", new Vector3(spd.x, spd.y, spd.z));
        data.walkAniFactor = EditorGUILayout.FloatField("行走动画播放速度", data.walkAniFactor);
        data.walkSpeed = new Vec3(spd2.x, spd2.y, spd.z);


        data.runAction = EditorGUILayout.Popup("奔跑动画选择", data.runAction == ActionType.ActionType_WALK ? 0 : 1, actionList.ToArray()) == 0 ? ActionType.ActionType_WALK : ActionType.ActionType_RUN;

        spd = mGlobalSettingTable.GetValue<Vec3>("runSpeed");
        //spd = data.runSpeed;

        spd2 = EditorGUILayout.Vector3Field("奔跑速度", new Vector3(spd.x, spd.y, spd.z));
        data.runAniFactor = EditorGUILayout.FloatField("奔跑动画播放速度", data.walkAniFactor);
        data.runSpeed = new Vec3(spd2.x, spd2.y, spd.z);


        data.hasDoubleRun = EditorGUILayout.Toggle("是否开启双击跑", data.hasDoubleRun);

        data.useNewHurtAction = EditorGUILayout.Toggle("是否使用新版被击动作流程", data.useNewHurtAction);

        data.useNewGravity = EditorGUILayout.Toggle("是否使用新版重力计算", data.useNewGravity);
        if (data.useNewGravity)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            var cnt = data.speedAnchorArray.Length;
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("添加区间"))
                {
                    cnt++;

                    var aa = new int[cnt];
                    aa[0] = 0;
                    for (int i = 0; i < data.speedAnchorArray.Length; i++)
                    {
                        aa[i + 1] = data.speedAnchorArray[i];
                    }
                    data.speedAnchorArray = aa;

                    var bb = new int[cnt];
                    bb[0] = 0;
                    for (int i = 0; i < data.gravityRateArray.Length; i++)
                    {
                        bb[i + 1] = data.gravityRateArray[i];
                    }
                    data.gravityRateArray = bb;
                }
                if (GUILayout.Button("删除区间") && cnt > 1)
                {
                    cnt--;

                    var aa = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    {
                        aa[i] = data.speedAnchorArray[i + 1];
                    }
                    data.speedAnchorArray = aa;

                    var bb = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    {
                        bb[i] = data.gravityRateArray[i + 1];
                    }
                    data.gravityRateArray = bb;
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < cnt; i++)
            {
                EditorGUILayout.BeginHorizontal();
                data.speedAnchorArray[i] = EditorGUILayout.IntField($"速度区间 {i}", data.speedAnchorArray[i]);
                data.gravityRateArray[i] = EditorGUILayout.IntField("重力比率", data.gravityRateArray[i]);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("添加区间"))
                {
                    cnt++;
                }
                if (GUILayout.Button("删除区间") && cnt > 1)
                {
                    cnt--;
                }
                var aa = new int[cnt];
                for (int i = 0; i < data.speedAnchorArray.Length && i < cnt; i++)
                {
                    aa[i] = data.speedAnchorArray[i];
                }
                data.speedAnchorArray = aa;

                var bb = new int[cnt];
                for (int i = 0; i < data.gravityRateArray.Length && i < cnt; i++)
                {
                    bb[i] = data.gravityRateArray[i];
                }
                data.gravityRateArray = bb;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        GUILayout.Label("录像设置:", fontStyle);
        mIsOpenMark = EditorGUILayout.Toggle("是否开启水印", mIsOpenMark);
        if (mIsOpenMark)
        {
            mIsReplayMode = EditorGUILayout.Toggle("是否开启不同步方式播放", mIsReplayMode);
        }
        GUILayout.Label("PVP测试设置:", fontStyle);
        data.isLogRecord = EditorGUILayout.Toggle("是否打开录像日志", data.isLogRecord);
        data.pvpDefaultSesstionID = EditorGUILayout.TextField("默认sessionID设置", data.pvpDefaultSesstionID);
        if (GUILayout.Button("播放录像"))
        {
            ReplayServer.GetInstance().StartReplay(data.pvpDefaultSesstionID, ReplayPlayFrom.VIDEO_FRAME, true, mIsOpenMark, mIsReplayMode);
        }
        GUILayout.Label("PVE测试设置:", fontStyle);

        data.mPveRecordPath = EditorGUILayout.TextField("录像路径", data.mPveRecordPath);

        if (GUILayout.Button("播放PVE录像"))
        {
            ReplayServer.GetInstance().StartPVEReplay(data.mPveRecordPath, ReplayPlayFrom.VIDEO_FRAME, mIsOpenMark, mIsReplayMode);
        }

        if (GUILayout.Button("校验水印重复"))
        {
            HeroGo.UtilityTools.CheckMarkRepeat();
        }
        if (GUILayout.Button("导出水印描述"))
        {
            HeroGo.UtilityTools.ExportReadableMark();
        }
        mRecordMarkPath = EditorGUILayout.TextField("录像水印路径", mRecordMarkPath);
        if (GUILayout.Button("生成水印日志"))
        {
            DumpProcessFiles(mRecordMarkPath);
        }


        Global.screenOrientation = (ScreenOrientation)EditorGUILayout.EnumPopup("屏幕模式修改", Global.screenOrientation);

        spd = mGlobalSettingTable.GetValue<Vec3>("monsterWalkSpeed");
        spd2 = EditorGUILayout.Vector3Field("怪物走路速度", new Vector3(spd.x, spd.y, spd.z));
        data.monsterWalkSpeed = new Vec3(spd2.x, spd2.y, spd.z);

        spd = mGlobalSettingTable.GetValue<Vec3>("monsterRunSpeed");
        spd2 = EditorGUILayout.Vector3Field("怪物跑步速度", new Vector3(spd.x, spd.y, spd.z));
        data.monsterRunSpeed = new Vec3(spd2.x, spd2.y, spd.z);

        GUILayout.Label("竞技场天平参数:");
        data.pkDamageAdjustFactor = EditorGUILayout.FloatField("技能伤害调整参数", mGlobalSettingTable.GetFloatValue1000("pkDamageAdjustFactor"));
        data.pkHPAdjustFactor = EditorGUILayout.FloatField("技能HP调整参数", mGlobalSettingTable.GetFloatValue1000("pkHPAdjustFactor"));
        data.pkUseMaxLevel = EditorGUILayout.Toggle("是否用最大等级调整天平", data.pkUseMaxLevel);

        GUILayout.Label("战斗测试:");
        data.playerHP = EditorGUILayout.IntField("玩家HP设置", data.playerHP);
        data.playerRebornHP = EditorGUILayout.IntField("玩家复活HP设置", data.playerRebornHP);
        data.monsterHP = EditorGUILayout.IntField("怪物HP设置", data.monsterHP);


		GUILayout.Label("宠物测试:", fontStyle);
		data.petID = EditorGUILayout.IntField("宠物ID", data.petID);
		data.petLevel = EditorGUILayout.IntField("宠物等级", data.petLevel);
		data.petHunger = EditorGUILayout.IntField("宠物饥饿度", data.petHunger);
		data.petSkillIndex = EditorGUILayout.IntField("宠物技能index", data.petSkillIndex);

		data.petXMovingDis = EditorGUILayout.FloatField("X方向最大跟随距离", data.petXMovingDis);
		data.petXMovingv1 = EditorGUILayout.FloatField("X方向启动速度", data.petXMovingv1);
		data.petXMovingv2 = EditorGUILayout.FloatField("X方向最大速度", data.petXMovingv2);

		data.petYMovingDis = EditorGUILayout.FloatField("Y方向最大跟随距离", data.petYMovingDis);
		data.petYMovingv1 = EditorGUILayout.FloatField("Y方向启动速度", data.petYMovingv1);
		data.petYMovingv2 = EditorGUILayout.FloatField("Y方向最大速度", data.petYMovingv2);

    }

    RecordMarkSystem sys;
    private void DumpProcessFiles(string path)
    {
     //   DirectoryInfo folder = new DirectoryInfo(path);
        sys = new RecordMarkSystem(RECORD_MODE.REPLAY, null, string.Empty, string.Empty, BattleType.None, eDungeonMode.None);
        sys.DumpProcessFileFromMark(path);
        //foreach (FileInfo nextFile in folder.GetFiles("*.mark"))
        //{
        //    sys.DumpProcessFileFromMark(nextFile.FullName);
        //}
        //foreach (DirectoryInfo nextFolder in folder.GetDirectories())
        //{
        //    DumpProcessFiles(nextFolder.FullName);
        //}
        sys = null;
    }

    uint itemSn = 0;
    System.Text.StringBuilder mRequestRewardBuilder = new System.Text.StringBuilder();
    private void _gameTestSetting(GlobalSetting data)
    {
        EditorGUILayout.LabelField("游戏测试脚本", fontStyle);
        mIsShowGameTest = EditorGUILayout.BeginToggleGroup("开启测试", mIsShowGameTest);
        if (mIsShowGameTest)
        {
            data.qualityAdjust.bIsOpen = EditorGUILayout.Toggle("装备品级调整", data.qualityAdjust.bIsOpen);
            if (data.qualityAdjust.bIsOpen)
            {
                data.qualityAdjust.fInterval = EditorGUILayout.FloatField("调整间隔", data.qualityAdjust.fInterval);
                data.qualityAdjust.iTimes = EditorGUILayout.IntField("执行次数", data.qualityAdjust.iTimes);
                itemSn = (uint)EditorGUILayout.IntField("掉落序号", (int)itemSn);
                if (GUILayout.Button("战斗中掉落作弊测试"))
                {
                    Protocol.SceneDungeonRewardReq req = new Protocol.SceneDungeonRewardReq();
                    req.dropItemIds = new uint[1] { itemSn };
                    mRequestRewardBuilder.Clear();

                    for (int i = 0; i < req.dropItemIds.Length; ++i)
                    {
                        mRequestRewardBuilder.Append(req.dropItemIds[i]);
                    }
                    req.md5 = GameClient.DungeonUtility.GetBattleEncryptMD5(mRequestRewardBuilder.ToString());
                    if (Network.NetManager.instance != null)
                    {
                        Network.NetManager.instance.SendCommand(Network.ServerType.GATE_SERVER, req);
                    }
                }

                data.qualityAdjust.SelectedIndex = -1;
                data.qualityAdjust.mScroll = EditorGUILayout.BeginScrollView(data.qualityAdjust.mScroll);
                for (int i = 0; i < data.qualityAdjust.Equipments.Count; ++i)
                {
                    var current = data.qualityAdjust.Equipments[i];
                    if (current != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        data.qualityAdjust.toggles[i] = EditorGUILayout.Toggle(current.Name, data.qualityAdjust.toggles[i]);
                        if (data.qualityAdjust.toggles[i])
                        {
                            data.qualityAdjust.SelectedIndex = i;
                        }
                        GUILayout.Label(string.Format("品级:{0}", current.SubQuality));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();

                GameClient.ItemData currentItem = null;
                if (data.qualityAdjust.SelectedIndex >= 0 && data.qualityAdjust.SelectedIndex < data.qualityAdjust.Equipments.Count)
                {
                    currentItem = data.qualityAdjust.Equipments[data.qualityAdjust.SelectedIndex];
                    for (int i = 0; i < data.qualityAdjust.Equipments.Count; ++i)
                    {
                        data.qualityAdjust.toggles[i] = i == data.qualityAdjust.SelectedIndex;
                    }
                }

                if (currentItem == null)
                {
                    GUILayout.Label("请选择一件装备!!!");
                }
                else
                {
                    GUILayout.Label("所选装备:");
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label(string.Format("装备名称:{0}", currentItem.Name));
                    GUILayout.Label(string.Format("品级:{0}", currentItem.SubQuality));
                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("刷新"))
                {
                    data.qualityAdjust.Dirty = true;
                }

                if (GUILayout.Button("开始调整"))
                {
                    if (currentItem != null)
                    {
                        var targetItem = GameClient.ItemDataManager.GetInstance().GetItem(currentItem.GUID);
                        if (targetItem != null)
                        {
                            //GameClient.ComFunctionAdjust.ExecuteCmdQualityChange(targetItem, data.qualityAdjust.fInterval,data.qualityAdjust.iTimes);
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndToggleGroup();
    }

    private void showShockData(string label, ref ShockData sd)
    {
        GUILayout.Label(label);
        sd.time = EditorGUILayout.FloatField("时间(ms)", sd.time);
        sd.speed = EditorGUILayout.FloatField("速度", sd.speed);
        sd.xrange = EditorGUILayout.FloatField("x轴幅度", sd.xrange);
        sd.yrange = EditorGUILayout.FloatField("y轴幅度", sd.yrange);
    }
#endif
}



