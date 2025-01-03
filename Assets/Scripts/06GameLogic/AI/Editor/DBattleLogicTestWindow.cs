using UnityEditor;
using UnityEngine;
using System;
using GameClient;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Battle;

public class DBattleLogicTestWindow : EditorWindow
{
    [MenuItem("[TM工具集]/战斗调试器/打开战斗调试器",false, 1)]
    static void Init()
    {
        var window = GetWindow<DBattleLogicTestWindow>();
        window.position = new Rect(50, 50, 250, 60);
        window.Show();
    }

    private TableInspetor<ProtoTable.UnitTable> mMonsterTable = new TableInspetor<ProtoTable.UnitTable>("怪物表ID", "Name");
    private TableInspetor<ProtoTable.SkillTable> mSkillTable = new TableInspetor<ProtoTable.SkillTable>("技能表ID", "Name");
    private TableInspetor<ProtoTable.ResTable> mResTable = new TableInspetor<ProtoTable.ResTable>("模型资源表ID", true);
    private TableInspetor<ProtoTable.BuffInfoTable> mbuffInfoTable = new TableInspetor<ProtoTable.BuffInfoTable>("buff信息表ID", true);
    private TableInspetor<ProtoTable.BuffTable> mbuffTable = new TableInspetor<ProtoTable.BuffTable>("buff表ID", true);
    private TableInspetor<ProtoTable.MechanismTable> mMechanismTable = new TableInspetor<ProtoTable.MechanismTable>("机制表ID", true);


    private GUIStyle fontStyle;
    private ComActorInfoDebug activeCom;
	private BeActor currentPlayer = null;
    private BeProjectile currentProjectile = null;

    private bool projectile1;
    private bool projectile2;
    Vector2 projectileScrollPos;
    Vector2 projectileScrollPos2;

	private bool monster1;
	private bool monster2;
	private bool monster3;
	private bool monster4;
	private bool monster5;
	private bool monster6;
	private bool monster7;
    private bool monster8;
    private bool monster9;
    private bool monster10;

    private bool actor1;
	private bool actor2;
	private bool actor3;
	private bool actor4;
	private bool actor5;
	private bool actor6;
	private bool actor7;
	private bool actor8;
	private bool actor9;
	private bool actor10;
	private bool actor11;
    private bool actor12;
    private bool actor13;
	private bool actor14;
    private bool actor15;
    private bool actor16;
    private bool actor17;
    private float timeScale;

    private int recordTrailSkillId;
    private bool recordTrail;
    private bool showRecordPosition;
    private BeActor recordActor;
    private BeActor recordTarget;
    private DelayCallUnitHandle recordCaller;
    private IBeEventHandle recordSkillHandle1;
    private IBeEventHandle recordSkillHandle2;
    private VInt3 recordStartPostion;
    private List<VInt3> recordPositionList = new List<VInt3>();

    private bool battle1;

	private int _buffId = 0;
	private int _buffLevel = 60;
	private int _buffProb = 1000;
	private int _buffDur = 5000;
    private int _buffAttack = 2000;

	private int _buffInfoID = 0;
	private int _mechanismID = 0;

    private int[] selectValues = new int[100];

    //private InputUserData inputD

    private bool folderAIInfo = false;
    private bool folderAIParameterInfo = false;
    
    private bool showBattleInfo = true;
    private bool showMonsterInfo = true;
    private bool showProjectileInfo = true;
    private bool showFunction = true;
    private bool showPlayerInfo = true;
    private bool showLevel = true;

    private string _scenraioTreeName = "";
    
    void CreateFontStyle()
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = (Font)EditorGUIUtility.Load("PingFangBold.TTF");
            fontStyle.fontSize = 22;
            fontStyle.alignment = TextAnchor.MiddleCenter;
            fontStyle.normal.textColor = Color.green;
            fontStyle.hover.textColor = Color.green;
            
			timeScale = Time.timeScale;
        }
    }

    void Callback(object obj)
    {
        Debug.Log("Selected: " + obj);
    }


	void CreateMonsterInBattleTest(int monsterID, int num)
    {
        if (monsterID == 0)
            return;

        if(Application.isPlaying == false)
        {
            return;
        }

        if(BattleMain.instance == null)
        {
            return;
        }


		for(int i = 0; i < num; ++i)
		{
			var target = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
            var enemy = BattleMain.instance.Main.CreateMonster(monsterID + monsterCreateLevel * 100);
			if (enemy != null)
			{
				//enemy.StartAI(target);
				var pos = target.GetPosition();
				Vec3 targetPos = new Vec3(pos.fx + UnityEngine.Random.Range(-2, 2), pos.fy + 1, pos.fz);
				//targetPos.x = -18;
				//targetPos.y = 8;
				enemy.SetPosition(new VInt3(targetPos));

				var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);
				if (monsterData != null)
                {
                    if (monsterData.BornAI == ProtoTable.UnitTable.eBornAI.Start)
                        enemy.StartAI(target);
                    if (monsterData.FloatValue > 0)
                        enemy.SetFloating(VInt.NewVInt(monsterData.FloatValue, 1000));
				}
			}
		}

    }

    private string monsterText;

    public void ClearAllCharacter()
    {
        if(Application.isPlaying == false)
        {
            return;
        }

        if (BattleMain.mode == eDungeonMode.SyncFrame) return;

        BattleMain.instance.Main.ClearAllCharacter();
    }

    bool monsterCreateUseID = false;
    int  monsterCreateLevel = 0;
    
    Vector2 InfoScrollPos;
    Vector2 infoScrollPos7;

    enum MonsterDifficulty
    {
        Normal = 1,
        Middle,
        Hard,
        hero
    }

    MonsterDifficulty monsterdiff = MonsterDifficulty.Normal;
	int monsterNum = 1;
 
	void OnInspectorUpdate()
	{
		Repaint();
	}

    void OnGUI()
    {
        
        GUIControls.UIStyles.UpdateEditorStyles();
        CreateFontStyle();
        //EditorGUILayout.BeginVertical("GroupBox");

        InfoScrollPos = EditorGUILayout.BeginScrollView(InfoScrollPos);

        ShowBattleInfo();

        ShowMonsterInfo();

        ShowAllProjectiles();

		ShowFunction();

		ShowPlayerInfo();

		_ShowLevel();
        EditorGUILayout.EndScrollView();
        
        //EditorGUILayout.EndVertical();
    }

    int EditorGUILayout_EnumPopup(string text,Enum value)
    {
       return Convert.ToInt32(EditorGUILayout.EnumPopup(text,value));
    }

	//private string[] skillNames = null;
	int selectSkillIndex = 0;
    int selectSkillIndex2 = 0;

	#region monsterInfo
	void ShowMonsterInfo()
	{
		//GUILayout.Label("怪物信息", fontStyle);
		EditorGUILayout.BeginVertical("GroupBox");

        showMonsterInfo = EditorGUILayout.Foldout(showMonsterInfo, "怪物信息");
        if (showMonsterInfo)
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            monsterText = EditorGUILayout.TextField(monsterCreateUseID ? "ID" : "名称", monsterText);
            monsterCreateUseID = GUILayout.Toggle(monsterCreateUseID, "使用ID创建", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            monsterCreateLevel = EditorGUILayout.IntField("等级", monsterCreateLevel);
            monsterNum = EditorGUILayout.IntField("数量", monsterNum);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("创建", "minibutton"))
            {
                var unitTable = TableManager.instance.GetTable<ProtoTable.UnitTable>();
                if (unitTable != null)
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var item in unitTable)
                    {
                        var current = item.Value as ProtoTable.UnitTable;
                        if (monsterCreateUseID)
                        {
                            int.TryParse(monsterText, out int id);
                            CreateMonsterInBattleTest(id, monsterNum);
                        }
                        else if (System.Text.RegularExpressions.Regex.IsMatch(current.Name, monsterText, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            menu.AddItem(new GUIContent(current.Name + "ID:" + current.ID), false, (object obj) =>
                            {
                                CreateMonsterInBattleTest((int)obj, monsterNum);
                            }, current.ID);
                        }
                    }
                    menu.ShowAsContext();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("MenuItem1"), false, Callback, "item 1");
                    menu.AddItem(new GUIContent("MenuItem2"), false, Callback, "item 2");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("SubMenu/MenuItem3"), false, Callback, "item 3");
                    menu.ShowAsContext();
                }

            }
            if (GUILayout.Button("清除怪物", "minibutton"))
            {
                ClearAllCharacter();
            }
            if (GUILayout.Button("选择怪物...", "minibutton"))
            {
                GenericMenu menu = new GenericMenu();
                var objList = Resources.FindObjectsOfTypeAll(typeof(ComActorInfoDebug));
                int count = 0;
                foreach (var item in objList)
                {

                    menu.AddItem(new GUIContent(count.ToString() + " " + item.name), false, (object obj) =>
                    {
                        activeCom = obj as ComActorInfoDebug;
                        monster1 = false;
                        monster2 = false;
                    }, item);
                    count++;
                }
                menu.ShowAsContext();
            }

            if (activeCom != null && activeCom.actor != null)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                GUILayout.Label(activeCom.actor.GetEntityData().name);
                if (GUILayout.Button("在场景中选中", GUILayout.Width(200)))
                {
                    Selection.activeObject = activeCom.gameObject;
                }
                GUILayout.Space(10);

                var ai = activeCom.actor.aiManager as BeActorAIManager;

                var tableData = ai.tableData;

                //GUILayout.Label("AI表格数据:");

                //EditorGUILayout.IntField("单位ID", tableData.ID);

                
#if !USE_FB_TABLE
            tableData.AIIsAPC =  EditorGUILayout.Toggle("是否APC",tableData.AIIsAPC == 1) ? 1 : 0;
			tableData.AISight =  (tableData.AISight = EditorGUILayout.IntField("境界视野",tableData.AISight))== 0 ? 4000 : tableData.AISight;
			tableData.AIChase = (tableData.AIChase =  EditorGUILayout.IntField("追击视野",tableData.AIChase)) == 0 ? (int)(tableData.AISight * 1.5f) : tableData.AIChase;
			tableData.AIWarlike = (tableData.AIWarlike =  EditorGUILayout.IntField("好战度",tableData.AIWarlike)) == 0 ? 50 : tableData.AIWarlike;
			tableData.AIAttackDelay = (tableData.AIAttackDelay = EditorGUILayout.IntField("攻击延迟",tableData.AIAttackDelay)) == 0 ? 2000 : tableData.AIAttackDelay;
			tableData.AIThinkTargetTerm = (tableData.AIThinkTargetTerm = EditorGUILayout.IntField("选择目标延迟",tableData.AIThinkTargetTerm)) == 0 ? 2000 : tableData.AIThinkTargetTerm;
			tableData.AIDestinationChangeTerm = (tableData.AIDestinationChangeTerm = EditorGUILayout.IntField("选择行走目标地间隔",tableData.AIDestinationChangeTerm)) == 0 ? 3000 : tableData.AIDestinationChangeTerm;
			tableData.AIKeepDistance = EditorGUILayout.IntField("保持距离",tableData.AIKeepDistance);

			tableData.AIIdleRand = (tableData.AIIdleRand=EditorGUILayout.IntField("Idle概率",tableData.AIIdleRand))==0?50:tableData.AIIdleRand;
			tableData.AIEscapeRand = (tableData.AIEscapeRand=EditorGUILayout.IntField("远离概率",tableData.AIEscapeRand))==0?50:tableData.AIEscapeRand;
			tableData.AIWanderRand = (tableData.AIWanderRand=EditorGUILayout.IntField("Wander概率",tableData.AIWanderRand))==0?50:tableData.AIWanderRand;
			if (tableData.AICombatType == 1)
				tableData.AIYFirstRand = (tableData.AIYFirstRand=EditorGUILayout.IntField("Y轴优先概率",tableData.AIYFirstRand))==0?50:tableData.AIYFirstRand;

			tableData.AIIdleDuration = (tableData.AIIdleDuration=EditorGUILayout.IntField("Idle时间",tableData.AIIdleDuration))==0?1000:tableData.AIIdleDuration;
			tableData.AIWanderRange = (tableData.AIWanderRange=EditorGUILayout.IntField("Wander范围",tableData.AIWanderRange))==0?Global.Settings.aiWanderRange:tableData.AIWanderRange;
			tableData.AIWalkBackRange = (tableData.AIWalkBackRange=EditorGUILayout.IntField("后退范围",tableData.AIWalkBackRange))==0?Global.Settings.aiWAlkBackRange:tableData.AIWalkBackRange;

			tableData.WalkSpeed = (tableData.WalkSpeed=EditorGUILayout.IntField("移动速度(%)",tableData.WalkSpeed));
			tableData.WalkAnimationSpeedPerent = (tableData.WalkAnimationSpeedPerent=EditorGUILayout.IntField("移动动画速度(%)",tableData.WalkAnimationSpeedPerent));

			int defaultWalkCmdCount = tableData.AICombatType==1?Global.Settings.aiMaxWalkCmdCount_RANGED:Global.Settings.aiMaxWalkCmdCount;

			tableData.AIMaxWalkCmdCount = (tableData.AIMaxWalkCmdCount=EditorGUILayout.IntField("最大连续移动数量",tableData.AIMaxWalkCmdCount))==0?defaultWalkCmdCount:tableData.AIMaxWalkCmdCount;
			tableData.AIMaxIdleCmdCount = (tableData.AIMaxIdleCmdCount=EditorGUILayout.IntField("最大连续IDLE数量",tableData.AIMaxIdleCmdCount))==0?Global.Settings.aiMaxIdleCmdcount:tableData.AIMaxIdleCmdCount;



			tableData.AITargetType[0] = EditorGUILayout_EnumPopup("目标类型",(BeAIManager.TargetType)(int)tableData.AITargetType[0]);

			//(BeAIManager.TargetType)data.AITargetType[0];
			tableData.AICombatType = EditorGUILayout_EnumPopup("AICombatType",(BeAIManager.AIType)(int)tableData.AICombatType);
			tableData.AIIdleMode   = EditorGUILayout_EnumPopup("AIIdleMode",(BeAIManager.IdleMode)(int)tableData.AIIdleMode);
			//tableData.AIAttackKind = EditorGUILayout_EnumPopup("AIAttackKind",);

			GUILayout.Label("AI行为树:");
			tableData.AIActionPath = EditorGUILayout.TextField("Action",tableData.AIActionPath);
			tableData.AIDestinationSelectPath = EditorGUILayout.TextField("Destination",tableData.AIDestinationSelectPath);
			tableData.AIEventPath = EditorGUILayout.TextField("Event",tableData.AIEventPath);

#endif
            GUILayout.BeginVertical("GroupBox");
                GUILayout.Label("拥有技能:");
                if (ai.attackInfos != null)
                {
                    for (int i = 0; i < ai.attackInfos.Count; ++i)
                    {
                        var info = ai.attackInfos[i];

                        var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(info.skillID);
                        string str = string.Format("ID:{0} {1} 概率:{2} 范围({3})", info.skillID, skillData.Name, info.prob, info.front);

                        EditorGUILayout.BeginHorizontal();

                        info.enable = GUILayout.Toggle(info.enable, str);
                        GUILayoutUtility.GetRect(1, 1);

                        GUILayout.Label("概率");
                        info.prob = EditorGUILayout.IntField("", info.prob, GUILayout.Width(50));
                        GUILayoutUtility.GetRect(1, 1);
                        GUILayout.Label("范围:前");
                        info.front.i = EditorGUILayout.IntField("", info.front.i, GUILayout.Width(50));
                        GUILayoutUtility.GetRect(1, 1);
                        GUILayout.Label("后");
                        info.back.i = EditorGUILayout.IntField("", info.back.i, GUILayout.Width(50));
                        GUILayoutUtility.GetRect(1, 1);
                        GUILayout.Label("左");
                        info.top.i = EditorGUILayout.IntField("", info.top.i, GUILayout.Width(50));
                        GUILayoutUtility.GetRect(1, 1);
                        GUILayout.Label("右");
                        info.down.i = EditorGUILayout.IntField("", info.down.i, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if (GUILayout.Button("技能重新排序", "minibutton", GUILayout.Width(100)))
                    {
                        ai.ReorderAttackInfo();
                    }
                }
            GUILayout.EndVertical();
                GUILayout.BeginVertical("GroupBox");
                {   
                    var monster = activeCom.actor; 
                    if (GUILayout.Button("重新加载所有技能", "minibutton", GUILayout.Width(200)))
                    {
                        
                        #if !LOGIC_SERVER
                                                
						BeActionFrameMgr.Clear();

                        BeActionFrameMgr.ClearSkillObjectCache();
                        SkillFileListCache.Clear(true);
                        #endif
                        monster.ChangeSkill(tableData);
                    }

                    List<int> skills = new List<int>();
                    List<string> names = new List<string>();

                    foreach(var skill in monster.skillController.GetSkillList())
                    {
                       names.Add(string.Format("[{0}]", skill.skillID) + skill.skillData.Name);
                       skills.Add(skill.skillID);
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    selectSkillIndex2 = EditorGUILayout.Popup(selectSkillIndex2, names.ToArray(), GUILayout.Width(300));

                   
                 
                    if (GUILayout.Button("释放技能", "minibutton", GUILayout.Width(100)))
                    {
                        monster.UseSkill(skills[selectSkillIndex2], true);
                    }
                    if (GUILayout.Button("释放技能并暂停", "minibutton", GUILayout.Width(100)))
                    {
                        Debug.Break();
                        monster.UseSkill(skills[selectSkillIndex2], true);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (selectSkillIndex2 < skills.Count && skills[selectSkillIndex2] > 0)
                        mSkillTable.OnGUIWithID(skills[selectSkillIndex2]);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("放个技能试试？", GUILayout.Width(100));
                    caskSkillId = EditorGUILayout.IntField("", caskSkillId, GUILayout.Width(100));
                    if (GUILayout.Button("释放技能", "minibutton", GUILayout.Width(100)))
                    {
                        activeCom.actor.UseSkill(caskSkillId);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("造成指定伤害", GUILayout.Width(100));
                    monsterDoHurtValue = EditorGUILayout.IntField("", monsterDoHurtValue, GUILayout.Width(100));
                    if (GUILayout.Button("造成伤害", "minibutton", GUILayout.Width(100)))
                    {
                        activeCom.actor.DoHurt(monsterDoHurtValue);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                ShowMonsterInfo(activeCom.actor);
                UpdateDropTest(activeCom.actor);

                if (GUILayout.Button("更新", "minibutton"))
                {
                    ai.SetAIInfo(tableData, true);
                }

                if (GUILayout.Button("杀死", "minibutton"))
                {
                    activeCom.actor.DoDead();
                }

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();
	}

	void ShowFunction()
	{
		//GUILayout.Label("调试功能", fontStyle);
		EditorGUILayout.BeginVertical("GroupBox");

        showFunction = EditorGUILayout.Foldout(showFunction, "调试功能");
        if (showFunction)
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            timeScale = EditorGUILayout.FloatField("TimeScale设置", timeScale, GUILayout.Width(200));
            if (GUILayout.Button("应用", "minibutton", GUILayout.Width(50)))
            {
                Time.timeScale = timeScale;
            }
            if (GUILayout.Button("1倍", "minibutton", GUILayout.Width(50)))
                Time.timeScale = 1.0f;
            if (GUILayout.Button("2倍", "minibutton", GUILayout.Width(50)))
                Time.timeScale = 2.0f;
            if (GUILayout.Button("3倍", "minibutton", GUILayout.Width(50)))
                Time.timeScale = 3.0f;
            if (GUILayout.Button("5倍", "minibutton", GUILayout.Width(50)))
                Time.timeScale = 5.0f;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("随机一下", "minibutton"))
            {
                if (BattleMain.instance != null)
                {
                    var battle = BattleMain.instance.GetBattle() as BaseBattle;
                    if (battle != null && battle.FrameRandom != null)
                        battle.FrameRandom.Range100();
                }
            }

            recordTrail = EditorGUILayout.Toggle("记录被击方移动轨迹", recordTrail);
            if (recordTrail)
            {
                if (recordActor == null)
                {
                    if (BattleMain.instance != null)
                    {
                        var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                        if (player != null)
                        {
                            recordActor = player.playerActor;
                        }
                    }
                }
                if (recordActor != null && recordSkillHandle1 == null)
                {
                    recordSkillHandle1 = recordActor.RegisterEventNew(BeEventType.onHitOther, param =>
                    {
                        if (recordTarget != null)
                        {
                            return;
                        }

                        if (param.m_Int2 == recordTrailSkillId)
                        {
                            recordTarget = param.m_Obj as BeActor;
                            if (recordTarget != null)
                            {
                                //Logger.LogError("start record...");

                                recordStartPostion = recordActor.GetPosition();

                                recordPositionList.Clear();
                                recordPositionList.Add(recordTarget.GetPosition());

                                recordCaller = recordTarget.delayCaller.RepeatCall(32, () =>
                                {
                                    recordPositionList.Add(recordTarget.GetPosition());
                                });

                                recordSkillHandle2 = recordTarget.RegisterEventNew(BeEventType.onTouchGround, _param =>
                                {
                                    recordPositionList.Add(recordTarget.GetPosition());

                                    recordTarget = null;
                                    recordCaller.SetRemove(true);
                                    recordSkillHandle2.Remove();
                                    recordSkillHandle2 = null;

                                    //Logger.LogError("end record...");
                                });
                            }
                        }
                    });

                }

                EditorGUILayout.BeginHorizontal();
                recordTrailSkillId = EditorGUILayout.IntField("记录技能ID", recordTrailSkillId);
                if (GUILayout.Button("释放技能", "minibutton"))
                {
                    if (recordActor != null)
                    {
                        recordActor.UseSkill(recordTrailSkillId);
                    }
                }
                EditorGUILayout.EndHorizontal();

                showRecordPosition = EditorGUILayout.Foldout(showRecordPosition, "显示轨迹信息");
                if (showRecordPosition)
                {
                    EditorGUILayout.BeginVertical("GroupBox");

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("清空轨迹点", "minibutton"))
                    {
                        recordPositionList.Clear();
                    }
                    if (GUILayout.Button("保存到技能配置文件", "minibutton"))
                    {
                        Logger.LogError("TODO 保存到技能配置文件");

                        //Global.Settings.recordPositionList.Clear();
                        //for (int i = 0; i < recordPositionList.Count; i++)
                        //{
                        //    Global.Settings.recordPositionList.Add(recordPositionList[i] - recordStartPostion);
                        //}
                    }
                    EditorGUILayout.EndHorizontal();

                    var offsetPos = VInt3.zero;
                    if (recordPositionList.Count > 0)
                    {
                        offsetPos = recordPositionList[0] - recordStartPostion;
                        offsetPos.y = 0;
                    }
                    var pos = EditorGUILayout.Vector3Field("位置偏移", new Vector3(offsetPos.x, offsetPos.y, offsetPos.z));
                    offsetPos = new VInt3((int)pos.x, (int)pos.y, (int)pos.z);
                    if (recordPositionList.Count > 0)
                    {
                        recordStartPostion = recordPositionList[0] - offsetPos;

                        GUILayout.Space(10);
                        int flag = -1;
                        for (int i = 0; i < recordPositionList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label((recordPositionList[i] - recordStartPostion).ToString());
                            if (GUILayout.Button("删除", "minibutton", GUILayout.Width(100)))
                            {
                                flag = i;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (flag != -1)
                        {
                            recordPositionList.RemoveAt(flag);
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                recordActor = null;
                recordTarget = null;
                recordCaller.SetRemove(true);
                if (recordSkillHandle1 != null)
                {
                    recordSkillHandle1.Remove();
                    recordSkillHandle1 = null;
                }
                if (recordSkillHandle2 != null)
                {
                    recordSkillHandle2.Remove();
                    recordSkillHandle2 = null;
                }
            }

        }

        EditorGUILayout.EndVertical();
    }



	#endregion

    List<int> histroyProjectileIDList = new List<int>();
    List<string> histroyProjectileInfoList = new List<string>();

    void ShowAllProjectiles()
    {
        if (BattleMain.instance == null)
        {
            histroyProjectileIDList.Clear();
            histroyProjectileInfoList.Clear();
            return;
        }
            

        EditorGUILayout.BeginVertical("GroupBox");
        showProjectileInfo = EditorGUILayout.Foldout(showProjectileInfo, "子弹信息");
        if (showProjectileInfo)
        {
            if (GUILayout.Button("选择子弹...", "minibutton"))
            {
                GenericMenu menu = new GenericMenu();
                //if (BattleMain.instance != null)
                {
                    var projectiles = GetProjects();
                    foreach (var projectile in projectiles)
                    {
                        int count = 0;
                        
                        menu.AddItem(new GUIContent(count.ToString() + " [" + projectile.m_iResID+"]"+ projectile.GetName()), false, (object obj) =>
                        {
                            var proj = obj as BeProjectile;
                            if (proj != null)
                                currentProjectile = proj;
                        }, projectile);
                        count++;
                    }
                        menu.ShowAsContext();
                }
            }

            {
                var projectiles = GetProjects();
                EditorGUILayout.BeginVertical("GroupBox");
                projectileScrollPos = EditorGUILayout.BeginScrollView(projectileScrollPos, GUILayout.Height(100));

                
                
                int count = 0;
                GUI.color = Color.green;
                foreach(var proj in projectiles)
                {
                    string entityName = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(proj.m_iResID).Name;

                    if (!histroyProjectileIDList.Contains(proj.GetPID()))
                    {
                        histroyProjectileIDList.Add(proj.GetPID());
                        histroyProjectileInfoList.Add(string.Format("[{0}][RESID:{1}][PID:{2}]{3}", histroyProjectileIDList.Count, proj.m_iResID, proj.GetPID(), entityName));
                    }

                    if(GUILayout.Button(string.Format("{0}[{1}][PID{2}]{3}({4}/{5})", count, proj.m_iResID, proj.GetPID(), entityName, proj.timeAcc, proj.m_fLifes),"minibutton"))
                    {
                        currentProjectile = proj;
                    }
                    count++;
                }

                GUI.color = Color.white;
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            
            
            if (currentProjectile != null)
            {
                ShowProjectileInfo(currentProjectile);
            }

            projectile2 = EditorGUILayout.Toggle("显示历史子弹发射信息", projectile2);
            if (projectile2)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                projectileScrollPos2 = EditorGUILayout.BeginScrollView(projectileScrollPos2, GUILayout.Height(100));
                
                for(int i=0; i<histroyProjectileInfoList.Count; ++i)
                {
                    GUILayout.Label(histroyProjectileInfoList[i]);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
        }        
        EditorGUILayout.EndVertical();
    }

    void ShowProjectileInfo(BeProjectile projectile)
    {
        if (projectile == null)
            return;
        
        EditorGUILayout.BeginVertical("GroupBox");

        mResTable.OnGUIWithID(projectile.m_iResID);

        string entityName = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(projectile.m_iResID).Name;
		var baseInfo = string.Format("[{1}/{2}]实体ID:{0} {5} 攻击次数:{3} 比例:{4} 距离:{7} hurtID:{6}",
		projectile.m_iResID, projectile.timeAcc, projectile.m_fLifes, projectile.totoalHitCount, projectile.GetScale(), entityName, projectile.hurtID, projectile.distance);
        GUILayout.Label(baseInfo);

        if (GUILayout.Button("在场景中选中", GUILayout.Width(200)))
        {
            Selection.activeObject = projectile.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        }

        projectile1 = EditorGUILayout.Toggle("显示加载的技能配置文件", projectile1);
        if (projectile1)
        {
            ShowLoadedSkillConfig(projectile.m_cpkEntityInfo, 50);
        }

        EditorGUILayout.EndVertical();
    }

    List<BeProjectile> GetProjects()
    {
        var list = new List<BeProjectile>();
        if (BattleMain.instance != null && BattleMain.instance.Main != null)
        {
            var all = BattleMain.instance.Main.GetEntities();
            foreach(var entity in all)
            {
                if (entity == null)
                    continue;
                var projectile = entity as BeProjectile;
                if (projectile == null)
                    continue;
                list.Add(projectile);
            }
        }

        return list;
    }

	#region PlayerInfo
	void ShowPlayerInfo()
	{
        //GUILayout.Label("玩家信息", fontStyle);
        EditorGUILayout.BeginVertical("GroupBox");

        showPlayerInfo = EditorGUILayout.Foldout(showPlayerInfo, "玩家信息");
        if (showPlayerInfo)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("选择玩家...", "minibutton"))
            {
                GenericMenu menu = new GenericMenu();
                //var objList = Resources.FindObjectsOfTypeAll(typeof(ComActorInfoDebug));


                if (BattleMain.instance == null)
                {
                    var actor = BeUtility.GetMainPlayerActor(/*BattleMain.IsModePvP(BattleMain.battleType)*/);
                    int count = 0;
                    menu.AddItem(new GUIContent("查看玩家信息"), false, (object obj) =>
                       {
                           currentPlayer = actor;
                       }, actor);
                    menu.ShowAsContext();
                }
                else
                {
                    var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();

                    int count = 0;
                    foreach (var player in players)
                    {
                        menu.AddItem(new GUIContent(count.ToString() + " " + player.playerInfo.name), false, (object obj) =>
                            {
                                BeActor actor = obj as BeActor;
                                if (actor != null)
                                    currentPlayer = actor;


                            }, player.playerActor);
                        count++;
                    }
                    menu.ShowAsContext();
                }

            }

            /*
            if (currentPlayer == null)
            {
                if (BattleMain.instance != null)
                {
                    var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
                    if (players.Count > 0)
                        currentPlayer = players[0].playerActor;
                }
            }*/

            if (currentPlayer != null)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                GUILayout.Label(currentPlayer.GetEntityData().name);
                if (GUILayout.Button("在场景中选中", GUILayout.Width(200)))
                {
                    if (currentPlayer.m_pkGeActor != null)
                    {
                        Selection.activeObject = currentPlayer.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                    }
                }
                GUILayout.Space(10);


                ShowCastSkills();

                ShowPlayerInfo(currentPlayer);

                UpdateDropTest(currentPlayer);

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();
    }

    public string DropInfo;

    public void UpdateDropTest(BeEntity owner)
    {
        DropInfo = EditorGUILayout.TextField("掉落测试", DropInfo);
        if (!GUILayout.Button("掉落", "minibutton"))
        {
            return;
        }

        if (string.IsNullOrEmpty(DropInfo))
        {
            return;
        }

        var drops = DropInfo.Split(',');
        if (drops.Length <= 0)
        {
            return;
        }

        List<DungeonDropItem> list = new List<DungeonDropItem>();
        foreach (var curDrop in drops)
        {
            DungeonDropItem item = new DungeonDropItem();

            var pair = curDrop.Split(':');
            int id = 0;
            if (pair.Length > 0)
            {
                int.TryParse(pair[0], out id);
            }
            item.typeId = id;

            if (id > 0)
            {
                list.Add(item);
            }

            int num = 1;
            if (pair.Length > 1)
            {
                int.TryParse(pair[1], out num);
            }
            item.num = num;
        }

        if (list.Count <= 0)
        {
            return;
        }

        if (null == owner)
        {
            return;
        }

        var scene = owner.CurrentBeScene;
        if (null == scene)
        {
            return;
        }


        scene.DropItems(list, owner.GetPosition());
    }




	public void ShowPlayerInfo(BeActor actor)
	{
		ShowGeneralInfo(actor);

        if (GUILayout.Button("中断当前技能", "minibutton"))
        {
            var currentSkill = actor.GetCurrentSkill();
            if(currentSkill != null)
            {
                currentSkill.Cancel();
            }
        }
        

        actor1 = EditorGUILayout.Toggle("显示玩家面板属性",actor1);
		if (actor1)
		{
			ShowActorPanelAttribute(actor.GetEntityData());
		}

		actor2 =  EditorGUILayout.Toggle("显示玩家属性",actor2);
		if (actor2)
		{
			ShowActorAttribute(actor.GetEntityData());
		}

        actor15 = EditorGUILayout.Toggle("显示玩家能力", actor15);
        if (actor15)
        {
            ShowAbility(actor);
        }

		//显示装备信息
		actor4 = EditorGUILayout.Toggle("显示装备信息", actor4);
		if (actor4)
		{
			ShowEquipInfo(actor);
		}

		//显示技能信息
		actor3 = EditorGUILayout.Toggle("显示技能信息", actor3);
		if (actor3)
		{
			ShowSkillInfo(actor);
		}

		//显示buff信息
		actor5 = EditorGUILayout.Toggle("显示buff信息", actor5);
		if (actor5)
		{
			ShowBuffInfo(actor);
		}

		actor6 = EditorGUILayout.Toggle("显示条件Buff信息", actor6);
		if (actor6)
		{
			ShowTriggerbuffInfo(actor);
		}

		actor10 = EditorGUILayout.Toggle("显示机制信息", actor10);
		if (actor10)
		{
			ShowMechanismInfo(actor);
		}

		actor7 = EditorGUILayout.Toggle("显示随从信息", actor7);
		if (actor7)
		{
			ShowAccompanyInfo(actor);
		}

		actor11 = EditorGUILayout.Toggle("显示投射物!", actor11);
		if (actor11)
		{
			ShowProjectile();
		}

        actor12 = EditorGUILayout.Toggle("显示技能释放信息", actor12);
        if (actor12)
        {
            ShowUseSkillInfo(actor);
        }

        actor16 = EditorGUILayout.Toggle("显示触发效果攻击到目标列表", actor16);
        if (actor16)
        {
            ShowHurtIdInfo(actor);
        }

        actor17 = EditorGUILayout.Toggle("显示事件监听列表", actor17);
        if (actor17)
        {
            ShowEventHandleList(actor);
        }

		actor8 = EditorGUILayout.Toggle("显示状态切换记录", actor8);
		if (actor8)
		{
			ShowActorStatesRecord(actor);
		}

		actor9 = EditorGUILayout.Toggle("显示功能", actor9);
		if (actor9)
		{
			ShowFunction(actor);
		}

		actor14 = EditorGUILayout.Toggle("显示加载的技能配置文件", actor14);
		if (actor14)
		{
			ShowLoadedSkillConfig(actor.m_cpkEntityInfo);
		}

        ShowAIInfo(actor);

        if (currentPlayer != null && GUILayout.Button("杀死", "minibutton"))
        {
            currentPlayer.DoDead();
        }
	}

	void ShowLoadedSkillConfig(BDEntityRes entityRes, int height = 300)
	{
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.BeginVertical();

        infoScrollPos7 = EditorGUILayout.BeginScrollView(infoScrollPos7, GUILayout.Height(height));

        for(int i=0; i<2; ++i)
        {
            foreach (var info in entityRes._vkActionsMap)
            {
                var name = info.Key;
                
                bool run = name.Equals(entityRes.GetCurrentActionName());
                if (i==0&&run || i==1&&!run)
                {
                    if (i == 0)
                        GUI.color = Color.green;
                    
                    var skillid = entityRes.GetSkillIDByActionName(name);

                    if(GUILayout.Button((run?"[运行中]":"") +(skillid>0?string.Format("[{0}]", skillid):"")+ name,"minibutton"))
                    {
                        var actionPath = entityRes._vkActionsMap[name].key;
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Resources/"+actionPath+".asset", typeof(DSkillData) ));
                    }
                    if (i == 0)
                        GUI.color = Color.white;
                }
                else
                    continue;
            }
        }
        
       

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
		for (int i = 0; i < entityRes.tagActionInfo.Length; ++i)
        {
            var tagActionInfo = entityRes.tagActionInfo[i];
            if (tagActionInfo != null)
            {
                foreach (var moveName in tagActionInfo.weaponTypeSkillDataInfo)
                {
                    EditorGUILayout.BeginVertical("GroupBox");
                    GUILayout.Label("WeaponType:" + moveName.Key + "  WeaponTag:" + i);
                    for (int k = 0; k < moveName.Value.Count; ++k)
                    {
                        GUILayout.Label(moveName.Value[k]);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    void ShowAbility(BeActor actor)
    {
        EditorGUILayout.BeginVertical("GroupBox");

        if (actor.stateController != null)
        {
            CrypticInt32[] ability = actor.stateController.Ability;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < ability.Length; i++)
            {
                if (i % 2 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (i == 0 || i == 9 || i == 45) continue;
                GUILayout.Label(((AbilityType)i).ToString(), GUILayout.Width(200));
                EditorGUILayout.Toggle("", ability[i] > 0, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    public enum AbilityType
    {
        //基本能力
        可以移动 = 1,    //可以移动
        可以攻击 = 2,    //可以攻击
        可以倒地 = 3,    //可以倒地
        可以浮 = 4,    //可以浮空
        可以被抓取 = 5,    //可以被抓取
        可以被破招 = 6,    //可以被破招
        可以背击 = 7,   //be hit
        能被选为攻击对象 = 8,  //能不能被选为攻击对象

        //异常状态能力
        感电 = 10,
        流血 = 11,
        燃烧 = 12,
        中毒 = 13,
        失明 = 14,
        眩晕 = 15,
        石化 = 16,
        冰冻 = 17,
        睡眠 = 18,
        混乱 = 19,
        束缚 = 20,
        减速 = 21,
        诅咒 = 22,

        受阻挡 = 23,   //受阻挡
        受重力影响 = 24,   //受重力影响
        可以转向 = 25,  //可以转向
        可以分身 = 26,  //可以分身
        可以x轴移动 = 27,  //可以x轴移动
        可以y轴移动 = 28,  //可以y轴移动
        可以z轴移动 = 29,  //可以z轴移动
        可以上buff = 30, //可以上buff

        不免伤 = 31,//默认不免伤

        混乱状态不能攻击队友默认是不拥有这个能力 = 32,     //混乱状态 不能攻击队友           (默认是不拥有这个能力)
        能被攻击但是不造成伤害默认是不拥有这个能力 = 33,  //能被攻击 但是不造成伤害   (默认是不拥有这个能力)
        免疫远程伤害 = 34,      //免疫远程伤害                    (默认是不拥有这个能力)
        可以被破招强制 = 35,    //可以被破招(强制）
        可以上异常buff = 36,//可以上异常buff
        不能被黑洞机制吸过去默认不拥有这个能力= 37,//不能被黑洞机制吸过去 (默认不拥有这个能力)
        只能被近身攻击伤害默认不拥有这个能力 = 38,   //只能被近身攻击伤害 (默认不拥有这个能力)
        不可以抓取 = 39,//不可以抓取
        箭头消失PVP隐身 = 40,//箭头消失（PVP隐身）
        过门的时候不移除 = 41,//过门的时候不移除
        可以攻击队友 = 42,//可以攻击队友
        超级霸体    =43,//免疫buff的TargetState
        可以顿帧    = 44,
        CHANGE_ACTION   =  45,
        可以攻击友方和敌方 = 46,
        可以背击_表中填了表示不能被背击 = 47,
        X轴位置需要刷新_默认不填表示需要刷新 = 48,
        Y轴位置需要刷新_默认不填表示需要刷新 = 49,
        Z轴位置需要刷新_默认不填表示需要刷新 = 50,
        COUNT = 51,
    }

	void ShowAccompanyInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");
		var accData = actor.accompanyData;
		if (accData.id > 0)
		{
			var mData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(accData.id);
			string content = string.Format("主随从:{0} {1} skill:{2} lv:{3}", mData.ID, mData.Name, accData.skillID, accData.level);
			GUILayout.Label(content);
		}

		EditorGUILayout.EndVertical();
	}
			
	void ShowEquipInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");

		var attribute = actor.GetEntityData();
		var equipList= attribute.itemProperties;
            
		foreach(var item in equipList)
		{
			var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(item.itemID);
			if (itemData != null)
			{
				string content = "";
				content += string.Format("道具:{0} {1} ", itemData.ID, itemData.Name);
				for(int i=0; i<(int)AttributeType.baseAtk; ++i)
				{
					var at = (AttributeType)i;
					string fieldName = Global.GetAttributeString(at);
					string name = Utility.GetEnumDescription(at);
					name = name.Replace("+",":");
					name = name.Replace("-",":");
					name = name.Replace("%","");

                    try
                    {
                        var value = Utility.GetValue(typeof(ItemProperty), item, fieldName);
                        if (value != 0)
                        {
                            content += string.Format(name + " ", value);
                        }
                    }
                    catch(System.Exception e)
                    {
                        string error = e.ToString();
                    }
					
				}

				if (BattleMain.IsModePvP(BattleMain.battleType))
				{
					if (item.attachPVPBuffIDs.Count > 0 && item.attachPVPBuffIDs[0] > 0)
					{
						content += " attachPVPBuffIDs:";
						for(int j=0; j<item.attachPVPBuffIDs.Count; ++j)
							content += item.attachPVPBuffIDs[j] + " ";
					}

					if (item.attachPVPMechanismIDs.Count > 0 && item.attachPVPMechanismIDs[0] > 0)
					{
						content += " attachPVPMechanismIDs:";
						for(int j=0; j<item.attachPVPMechanismIDs.Count; ++j)
							content += item.attachPVPMechanismIDs[j] + " ";
					}

				}
				else 
				{
					if (item.attachBuffIDs.Count > 0 && item.attachBuffIDs[0] > 0)
					{
						content += " attachBuffIDs:";
						for(int j=0; j<item.attachBuffIDs.Count; ++j)
							content += item.attachBuffIDs[j] + " ";
					}
					if (item.attachMechanismIDs.Count > 0 && item.attachMechanismIDs[0] > 0)
					{
						content += " attachMechanismIDs:";
						for(int j=0; j<item.attachMechanismIDs.Count; ++j)
							content += item.attachMechanismIDs[j] + " ";
					}

				}

				GUILayout.Label(content);
			}
		}

		EditorGUILayout.EndVertical();
	}

	void ShowProjectile()
	{
		if (BattleMain.instance == null)
			return;
        
		EditorGUILayout.BeginVertical("GroupBox");
        
        //if (showProjectileInfo)
        {
            var bescene = BattleMain.instance.GetDungeonManager().GetBeScene();
            if (bescene == null)
                return;
            var entities = bescene.GetEntities();
            foreach(var entity in entities)
            {
                BeProjectile projectile = entity as BeProjectile;
                if (projectile != null)
                {
                    string entityName = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(projectile.m_iResID).Name;
                    string strAddBuff = "";
                    /*for(int i=0; i<projectile.hurtAddBuffs.Count; ++i)
                        strAddBuff += projectile.hurtAddBuffs[i] + " ";*/
                    var content = string.Format("[{1}/{2}]实体ID:{0} {5} 攻击次数:{3} 比例:{4} 距离:{7} hurtID:{6} addbuff:{8}",
                        projectile.m_iResID, projectile.timeAcc, projectile.m_fLifes, projectile.totoalHitCount, projectile.GetScale(), /*projectile.GetName()*/entityName, projectile.hurtID, projectile.distance, strAddBuff);
                    GUILayout.Label(content);
                }
            }
        }
		EditorGUILayout.EndVertical();
	}



    int caskSkillId = 0;
    int monsterDoHurtValue = 0;
	void ShowSkillInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");

        var skillList = actor.GetSkills();
		foreach(var skill in skillList)
		{
			var beskill = skill.Value;

			if (beskill.skillData.SkillCategory == 1)
				continue;
			
			string pre = "";
			if (beskill.skillData.SkillCategory == 2)
				pre = "通用技能";
			else if (beskill.skillData.SkillCategory == 3)
				pre = "职业技能";
			else if (beskill.skillData.SkillCategory == 4)
				pre = "觉醒技能";
			else if (beskill.skillData.SkillCategory == 5)
				pre = "公会技能";

			var content = string.Format("{10}:技能ID:{0} {1} Lv:{2} CD:{3} 是否行走:{4} 是否蓄力:{5} 速度增加:{6} 命中增加:{7} 暴击增加:{8} 攻击力增加:{9}", 
				beskill.skillID, beskill.skillData.Name, beskill.level, beskill.GetCurrentCD(), 
				beskill.walk, beskill.charge, beskill.speedAddRate, beskill.hitRateAdd,
				beskill.criticalHitRateAdd, beskill.attackAddRate, pre);
			GUILayout.Label(content);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("--等级", GUILayout.Width(50));
			beskill.level = EditorGUILayout.IntField("", beskill.level, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal ();

		}
        
        EditorGUILayout.EndVertical();
	}

	void ShowBuffInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");
		var buffList = actor.buffController.GetBuffList();
		foreach(var buff in buffList)
		{
			string skillInfo = "";
            if(null == buff)
            {
                continue;
            }
			if (buff.skillIDs != null && buff.skillIDs.Count > 0 && buff.skillIDs[0] > 0)
			{
				for(int i=0; i<buff.skillIDs.Count; ++i)
				{
					var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(buff.skillIDs[i]);
                    if(null != skillData)
                    {
					skillInfo += string.Format("{0} {1}", buff.skillIDs[i], skillData.Name);
                    }
                    else
                    {
                        Logger.LogErrorFormat("buff:{0}的技能id为:{1}的技能在技能表找不到",buff.buffID,buff.skillIDs[i]);
                    }
				}
					
			}
			var content = "";
            if(null != buff.buffData)
            {
                if (buff.duration < int.MaxValue)
                    content = string.Format("[{3}/{6}]buffID:{0} {1} Lv:{2} 异常lv:{7} {5} buffAttack:{4}",
                    buff.buffData.ID, buff.buffData.Name, buff.level, buff.GetLeftTime(), buff.buffAttack, skillInfo, buff.duration, buff.abnormalLevel);
                else
                    content = string.Format("[永久]buffID:{0} {1} Lv:{2} 异常lv:{3} buffAttack:{4} skillInfo:{5}",
                        buff.buffData.ID, buff.buffData.Name, buff.level, buff.abnormalLevel, buff.buffAttack, skillInfo);
            }
			GUILayout.Label(content);
		}

		EditorGUILayout.EndVertical();
	}
		
	void ShowMechanismInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");
		var mlist = actor.MechanismList;
		foreach(var m in mlist)
		{
			var content = string.Format("ID:{0}, mechanismID:{3} level:{1} name:{2}", m.mechianismID, m.level, m.data.Description, m.data.Index);
			GUILayout.Label(content);
		}

		EditorGUILayout.EndVertical();
	}

	void ShowTriggerbuffInfo(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");
		var triggerBuffList = actor.buffController.GetTriggerBuffList();
		foreach(var item in triggerBuffList)
		{
			var condition = (BuffCondition)item.Key;
			var buffInfoList = item.Value;
			GUILayout.Label(string.Format("条件:{0}", condition));
			for(int i=0; i<buffInfoList.Count; ++i)
			{
				var buffInfo = buffInfoList[i];
				var buffInfoData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfo.buffInfoID);
				if (buffInfoData != null)
				{
					var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffInfo.buffID);

					var content = string.Format("--[{10}]buff信息表ID:{0} lv:{2} 目标:{3} 概率:{4} 时间:{5} CD:{6}  buff攻击力:{7} 触发buff:{8} {9} 效果:{1}", 
						buffInfo.buffInfoID, buffInfoData.Description[0], buffInfo.level, buffInfo.target, 
						buffInfo.prob, buffInfo.duration, buffInfo.CD, buffInfo.attack, buffInfo.buffID, buffData.Name,
						buffInfo.IsCD()?string.Format("CD中:{0}/{1}", buffInfo.GetCDAcc(), buffInfo.CD):"");	
					GUILayout.Label(content);
					EditorGUILayout.BeginHorizontal();
					int width = 100;
					GUILayout.Label("-----修改概率", GUILayout.Width(width));
					var prob = EditorGUILayout.IntField("", buffInfo.prob);
					GUILayoutUtility.GetRect(1, 1);
					GUILayout.Label("修改时间", GUILayout.Width(width));
					var dur = EditorGUILayout.IntField("", buffInfo.duration);
					GUILayoutUtility.GetRect(1, 1);
					GUILayout.Label("修改CD", GUILayout.Width(width));
					var cd = EditorGUILayout.IntField("", buffInfo.CD);
					GUILayoutUtility.GetRect(1, 1);
					GUILayout.Label("修改触发buffID", GUILayout.Width(width));
					var buffID = EditorGUILayout.IntField("", buffInfo.buffID);
					GUILayoutUtility.GetRect(1, 1);
					if(true/*GUILayout.Button("执行修改","minibutton", GUILayout.Width(100))*/)
					{
						buffInfo.prob = prob;
						buffInfo.duration = dur;
						buffInfo.buffID = buffID;
						buffInfo.CD = cd;
					}
					EditorGUILayout.EndHorizontal();
				}

			}
		}
		EditorGUILayout.EndVertical();
	}

    IBeEventHandle handle1;
    IBeEventHandle handle2;
    IBeEventHandle handle3;
    private int skillTimes = 10;
    private int count = 0;
    private float totalTime = 0;
    private float tempTime = 0;
    private int totalFrame = 0;
    private int tempFrame = 0;
    public void ShowCastSkills()
	{
		if (InputManager.instance != null)
		{
             if (currentPlayer != null && GUILayout.Button("重新加载所有技能", "minibutton", GUILayout.Width(200)))
                {
                    #if !LOGIC_SERVER
                                        
					BeActionFrameMgr.Clear();

                    BeActionFrameMgr.ClearSkillObjectCache();
                    SkillFileListCache.Clear(true);
                    #endif
                    currentPlayer.ChangeSkillForPlayer();
                }

			List<int> skills = new List<int>();
			List<string> names = new List<string>();
			var tmpSkills = TableManager.GetInstance().GetSkillInfoByPid(InputManager.instance.GetJobID()).Keys;
			foreach(var key in tmpSkills)
			{
				var tableData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(key);
				if (tableData != null && tableData.SkillCategory >= 3)
				{
					names.Add(string.Format("[{0}]", tableData.ID) + tableData.Name);
					skills.Add(key);
				}
			}
            
            EditorGUILayout.BeginHorizontal();
			selectSkillIndex = EditorGUILayout.Popup(selectSkillIndex, names.ToArray(), GUILayout.Width(300));
			GUILayout.Label("技能ID: " + skills[selectSkillIndex]);
            EditorGUILayout.EndHorizontal();

            //这个功能没用，先注掉
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("连续释放技能次数：");
            //skillTimes = EditorGUILayout.IntField("", skillTimes, GUILayout.Width(100));
            //if (GUILayout.Button("释放技能", "minibutton", GUILayout.Width(200)))
            //{
            //    count = 0;
            //    totalTime = 0;
            //    tempTime = 0;
            //    totalFrame = 0;
            //    tempFrame = 0;
            //    int skillIndex = selectSkillIndex;
            //    var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
            //    if (actor != null && !actor.IsDead())
            //    {
            //        if (null == handle1)
            //        {
            //            handle1 = actor.RegisterEventNew(BeEventType.onCastSkill, args =>
            //            {
            //                Debug.LogFormat("skill begin time:{0} frame:{1}", Time.time, Time.frameCount);
            //                tempTime = Time.time;
            //                tempFrame = Time.frameCount;
            //            });
            //        }
            //        if (null == handle2)
            //        {
            //            handle2 = actor.RegisterEventNew(BeEventType.onCastSkillFinish, param =>
            //            {
            //                Debug.LogFormat("skill end time:{0} frame:{1}", Time.time, Time.frameCount);
            //                var skill = actor.GetSkill(skills[skillIndex]);
            //                if (skill != null)
            //                    skill.ResetCoolDown();
            //                totalTime += Time.time - tempTime;
            //                totalFrame += Time.frameCount - tempFrame;
            //            });
            //        }
            //        actor.UseSkill(skills[skillIndex], true);
            //        ++count;
            //        if (null == handle3)
            //        {
            //            handle3 = actor.RegisterEventNew(BeEventType.onStateChange, (GameClient.BeEvent.BeEventParam param) =>
            //            {
            //                if (count >= skillTimes)
            //                {
            //                    handle1.Remove();
            //                    handle2.Remove();
            //                    handle3.Remove();
            //                    handle1 = null;
            //                    handle2 = null;
            //                    handle3 = null;
            //                    Debug.LogWarning("==============================================================================");
            //                    Debug.LogWarningFormat("释放技能：{0}  技能ID：{1}", names[skillIndex], skills[skillIndex]);
            //                    Debug.LogWarningFormat("释放次数：{0}  总时间：{1}  总帧数：{2}  平均时间：{3}  平均帧数：{4}", skillTimes, totalTime, totalFrame, totalTime / skillTimes, (float)totalFrame / skillTimes);
            //                    return;
            //                }
            //                var state = (ActionState)param.m_Int;
            //                if (state == ActionState.AS_IDLE)
            //                {
            //                    actor.UseSkill(skills[skillIndex], true);
            //                    ++count;
            //                }
            //            });
            //        }
            //    }
            //}
            //EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("中断技能", "minibutton"))
            {
                if (handle1 != null)
                {
                    return;
                }

                if (currentPlayer != null)
                {
                    //currentPlayer.Locomote(new BeStateData((int)ActionState.AS_HURT, 0, 0, 0, 0, 0, 300), true);
                    currentPlayer.Locomote(new BeStateData((int)ActionState.AS_HURT) { _timeout = 300 }, true);
                }
            }

            if (GUILayout.Button("释放技能","minibutton"))
			{
                if (handle1 != null)
                {
                    return;
                }

                if (currentPlayer != null)
				{
                    currentPlayer.UseSkill(skills[selectSkillIndex], true);
				}
			}

            if(GUILayout.Button("释放技能并暂停","minibutton"))
			{
                if (handle1 != null)
                {
                    return;
                }
                Debug.Break();

                if (currentPlayer != null)
				{
                    currentPlayer.UseSkill(skills[selectSkillIndex], true);
				}
			}

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("释放技能ID：");
            caskSkillId = EditorGUILayout.IntField("", caskSkillId, GUILayout.Width(100));
            if (GUILayout.Button("释放技能", "minibutton", GUILayout.Width(200)))
            {
                if (currentPlayer != null)
                {
                    currentPlayer.UseSkill(caskSkillId, true);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("突然触发LowLevel", "minibutton"))
			{
				//GeGraphicSetting.instance.DoSetLowLevel();

				GeGraphicSetting.instance.SetGraphicLevel(GraphicLevel.LOW);
			}

            if (GUILayout.Button("武器切换", "minibutton"))
            {
                var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
                if (actor != null)
                {
                    actor.ChangeWeapon(0, 10);
                }
            }

            if (GUILayout.Button("复活", "minibutton") || HasKeyDown(KeyCode.Y))
            {
                var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
                if (actor != null && actor.IsDead())
                {
                    actor.Reborn();
                }
            }

            if (GUILayout.Button("检查改变技能等级变化的武器", "minibutton"))
            {
                Dictionary<int, int> needModifySkillIDs = new Dictionary<int, int>();
                Dictionary<int, int> uniqueMechanism = new Dictionary<int, int>();


                var equipAttriTable = TableManager.GetInstance().GetTable<ProtoTable.EquipAttrTable>();
                foreach (var equipData in equipAttriTable)
                {
                    var propertyData = (ProtoTable.EquipAttrTable)equipData.Value;
                    for (int i = 0; i < propertyData.AttachMechanismIDs.Count; ++i)
                    {
                        var mechanismData = TableManager.GetInstance().GetTableItem<ProtoTable.MechanismTable>(propertyData.AttachMechanismIDs[i]);
                        if (mechanismData != null)
                        {
                            if (mechanismData.Index == 1)
                                Logger.LogErrorFormat("--{0} {1}", propertyData.ID, propertyData.Name);

                            if (!uniqueMechanism.ContainsKey(mechanismData.Index))
                                uniqueMechanism.Add(mechanismData.Index, 1);
                        }
                    }

                    for(int i=0; i<propertyData.AttachBuffInfoIDs.Count; ++i)
                    {
                        BuffInfoData infoData = new BuffInfoData(propertyData.AttachBuffInfoIDs[i]);
                        if (infoData != null && (infoData.condition == BuffCondition.ENTERBATTLE || infoData.condition == BuffCondition.NONE))
                        {
                            var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(infoData.buffID);
                            if (buffData != null && buffData.Type == -7)
                            {
                                foreach(var mid in buffData.MechanismID)
                                {
                                    Logger.LogErrorFormat("8888888 buffid:{0}", buffData.ID);
                                    var mechanismData = TableManager.GetInstance().GetTableItem<ProtoTable.MechanismTable>(mid);
                                    if (mechanismData == null)
                                        continue;

                                    if (!uniqueMechanism.ContainsKey(mechanismData.Index))
                                        uniqueMechanism.Add(mechanismData.Index, 1);
                                }
                            }
                        }
                    }
                }

                string str2 = "";
                foreach (var item in uniqueMechanism)
                {
                    str2 += item.Key + " ";
                }
                Logger.LogErrorFormat("----需要看的技能 {0}", str2);

                uniqueMechanism.Clear();
                var itemTable = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
                var enumerator = itemTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ProtoTable.ItemTable data = (ProtoTable.ItemTable)enumerator.Current.Value;
                    if (data != null && data.SubType == ProtoTable.ItemTable.eSubType.WEAPON)
                    {
                        var propertyData = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(data.EquipPropID);
                        if (propertyData != null)
                        {

                            
                            for (int i = 0; i < propertyData.AttachMechanismIDs.Count; ++i)
                            {
                               

                                var mechanismData = TableManager.GetInstance().GetTableItem<ProtoTable.MechanismTable>(propertyData.AttachMechanismIDs[i]);
                                if (mechanismData != null)
                                {
                                    Logger.LogErrorFormat("---{0} {1} {2}", data.ID, data.Name, mechanismData.Index);

                                    if (!uniqueMechanism.ContainsKey(mechanismData.Index))
                                        uniqueMechanism.Add(mechanismData.Index, 1);
                                }
                            }

                           

                            //                            for (int i = 0; i < propertyData.AttachBuffInfoIDs.Count; ++i)
                            //                             {
                            //                                 var buffInfoData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(propertyData.AttachBuffInfoIDs[i]);
                            //                                 if (buffInfoData != null && 
                            //                                     (buffInfoData.BuffID == 1000001 ||
                            //                                     buffInfoData.BuffID == 1000002 ||
                            //                                     buffInfoData.BuffID == 1000003 ||
                            //                                     buffInfoData.BuffID == 1000004 ||
                            //                                     buffInfoData.BuffID == 1000005 ||
                            //                                     buffInfoData.BuffID == 1200459 ||
                            //                                     buffInfoData.BuffID == 1200461))
                            //                                 {
                            //                                     foreach(int skillid in buffInfoData.SkillID)
                            //                                     {
                            //                                         string skillName = "Skill" + skillid;
                            //                                         Type skillType = TypeTable.GetType(skillName);
                            //                                         bool hasScript = skillType != null;
                            // 
                            //                                         var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillid);
                            // 
                            // 
                            // 
                            //                                         if (/*!hasScript && */
                            //                                             (   HasValue(skillData.HitEffectIDs) ||
                            //                                                 HasValue(skillData.HitEffectIDsPVP) ||
                            //                                                 HasValue(skillData.BuffInfoIDs) ||
                            //                                                 HasValue(skillData.MechismIDs)))
                            //                                         {
                            //                                             if (!needModifySkillIDs.ContainsKey(skillid))
                            //                                                 needModifySkillIDs.Add(skillid, 1);
                            //                                             Logger.LogErrorFormat("----{0} {1} {3} hasScript={4} {5} {2}", data.ID, data.Name, buffInfoData.Description[0], skillid, hasScript, skillData.Name);
                            // 
                            // 
                            //                                         }
                            //                                             
                            // 
                            //                                     }
                            // 
                            //                                     
                            //                                 }
                            //                             }
                        }
                    }
                }
                
//                 string str = "";
//                 foreach(var item in needModifySkillIDs)
//                 {
//                     str += item.Key + " ";
//                 }
// 
//                 Logger.LogErrorFormat("需要需改的技能 {0}", str);
// 
                 string str3 = "";
                foreach (var item in uniqueMechanism)
                {
                    str3 += item.Key + " ";
                }
                Logger.LogErrorFormat("需要看的武器配的机制 {0}", str3);

            }

        }
	}

    bool HasValue(List<int> lst)
    {
        for(int i=0; i<lst.Count; ++i)
        {
            if (lst[i] != 0)
                return true;
        }

        return false;
    }

    public bool HasKeyDown(KeyCode kc)
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.KeyDown:
                {
                    if (Event.current.keyCode == kc)
                    {
                        return true;
                    }
                    break;
                }
        }

        return false;
    }

	public void ShowMonsterInfo(BeActor actor)
	{
		ShowGeneralInfo(actor);

		monster1 = EditorGUILayout.Toggle("显示怪物面板属性",monster1);
		if (monster1)
		{
			ShowMonsterPanelAttribute(actor.GetEntityData());
		}

		monster2 =  EditorGUILayout.Toggle("显示怪物属性",monster2);
		if (monster2)
		{
			ShowActorAttribute(actor.GetEntityData());
		}

        monster9 = EditorGUILayout.Toggle("显示怪物能力", monster9);
        if (monster9)
        {
            ShowAbility(actor);
        }

        monster7 = EditorGUILayout.Toggle("显示怪物技能", monster7);
		if (monster7)
		{
			ShowSkillInfo(actor);
		}

		monster3 =  EditorGUILayout.Toggle("显示怪物buff信息",monster3);
		if (monster3)
		{
			ShowBuffInfo(actor);
		}

		monster6 = EditorGUILayout.Toggle("显示机制信息", monster6);
		if (monster6)
		{
			ShowMechanismInfo(actor);
		}

		monster4 = EditorGUILayout.Toggle("显示怪物条件buff信息", monster4);
		if (monster4)
		{
			ShowTriggerbuffInfo(actor);
		}

		monster5 = EditorGUILayout.Toggle("显示怪物状态切换记录", monster5);
		if (monster5)
		{
			ShowActorStatesRecord(actor);
		}


        monster8 = EditorGUILayout.Toggle("显示功能", monster8);
        if (monster8)
        {
            ShowFunction(actor);
        }

        monster10 = EditorGUILayout.Toggle("显示加载的技能配置文件", monster10);
		if (monster10)
		{
			ShowLoadedSkillConfig(actor.m_cpkEntityInfo);
		}

        ShowAIInfo(actor);
    }

	void ShowGeneralInfo(BeActor actor)
	{
		if (actor == null)
			return;
        EditorGUILayout.BeginVertical("GroupBox");

        GUILayout.Label("表格快捷操作:");

        mbuffInfoTable.OnGUIWithID(0);
        mbuffTable.OnGUIWithID(0);
        mMechanismTable.OnGUIWithID(0);

		GUILayout.Label("基本信息:");
        if (actor.aiManager != null)
        {
            GUILayout.Label("AI好战度:" + actor.aiManager.warlike);
        }

        GUILayout.Label("等级:" + actor.GetEntityData().level.ToString());
        //状态
        string state = "";
		bool stand = true;
		if (actor.HasTag((int)AState.ACS_FALL))
		{
			stand = false;
			state += "浮空,";
		}
		if (actor.HasTag((int)AState.AST_FALLGROUND))
		{
			stand = false;
			state += "倒地,";
		}
		if (stand)
			state += "站立";
		
		GUILayout.Label(state);

		string tmp = "";
		tmp = string.Format("speed:({0},{1},{2})", actor.moveXSpeed, actor.moveYSpeed, actor.moveZSpeed);
		GUILayout.Label(tmp);

		tmp = string.Format("camp:{0} id:{1} resid:{2}", actor.m_iCamp, actor.m_iID, actor.m_iResID);
		GUILayout.Label(tmp);

        if (actor.IsMonster())
        {
            mMonsterTable.OnGUIWithID(actor.GetEntityData().monsterID);
        }

        mResTable.OnGUIWithID(actor.m_iResID);

        _showLastSkillInfo(actor);

		tmp = string.Format("pos:({0},{1},{2})", actor.GetPosition().fx, actor.GetPosition().fy, actor.GetPosition().fz);
		GUILayout.Label(tmp);
		tmp = string.Format("isFloating:{0}", actor.isFloating);

        GUILayout.EndVertical();
	}
    
    void _showLastSkillInfo(BeActor actor)
    {
        if (actor.GetCurrentSkill() != null)
        {
            EditorGUILayout.BeginHorizontal("GroupBox");
            var skill = actor.GetCurrentSkill();
            var tmp = string.Format("技能名:{0},攻速:{1},技能行走速度:{2}", skill.skillData.Name, skill.skillSpeed,skill.walkSpeed);
            GUILayout.Label(tmp);
        
            GUILayoutUtility.GetRect(1, 1);
            var skillconfig = actor.GetSkillActionInfo(actor.GetCurrentSkill().skillID);
            if (skillconfig != null)
            {
                var actionPath = skillconfig.key;
                if (skillconfig != null  && GUILayout.Button(string.Format("打开[{0}]", skillconfig.moveName), GUILayout.Width(200)))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Resources/"+actionPath+".asset", typeof(DSkillData) ));
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

	private GUIStyle linkLabelStyle;
	void ShowActorStatesRecord(BeActor actor)
	{
#if !LOGIC_SERVER
		if ( actor.GetStateGraph() == null)
			return;
		EditorGUILayout.BeginVertical("GroupBox");
		var records = actor.GetStateGraph().statesChangeRecord.ToArray();
		for(int i=0; i<records.Length; ++i)
		{
			BeStatesGraph.StateRecordInfo info = records[i];
			info.isShow = EditorGUILayout.Foldout(info.isShow, info.content);
			if(info.isShow)
			{
				EditorGUILayout.BeginVertical();
				for (int j = 0; j < info.frames.Length; j++)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(" ", GUILayout.Width(10));
					var frame = info.frames[j];
					string content = frame.GetMethod().DeclaringType + "." + frame.GetMethod().Name + "()(" +
					                 frame.GetFileName() + ":" + frame.GetFileLineNumber() + ")";
					if(HeroGo.CustomGUIUtility.LinkLabel(content))
					{
						string fileName = frame.GetFileName();
						string fileAssetPath = fileName.Substring(fileName.IndexOf("Assets"));  
						AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(fileAssetPath), frame.GetFileLineNumber());  
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
		}
		EditorGUILayout.EndVertical();
#endif
	}


	
	void ShowActorPanelAttribute(BeEntityData attribute)
	{
		var actorAttributeDisplay = BeEntityData.GetActorAttributeForDisplay(attribute);
		actorAttributeDisplay.hp = attribute.GetHP();
		actorAttributeDisplay.mp = attribute.GetMP();
		_showPanelAttribute(actorAttributeDisplay);
	}

	void ShowMonsterPanelAttribute(BeEntityData attribute)
	{
		var monsterAttributeDisplay = BeEntityData.GetMonsterAttributeForDisplay(attribute);
		monsterAttributeDisplay.hp = attribute.GetHP();
		monsterAttributeDisplay.mp = attribute.GetMP();
		_showPanelAttribute(monsterAttributeDisplay);
	}

	void ShowActorAttribute(BeEntityData attribute)
	{
		EditorGUILayout.BeginVertical("GroupBox");
		GUILayout.Label(string.Format("当前HP:{0}", attribute.GetHP()));
		GUILayout.Label(string.Format("当前MP:{0}", attribute.GetMP()));
        GUILayout.Label(string.Format("血条血量最大值:{0}", attribute.battleData.maxHp));

		for(int i=0; i<(int)AttributeType.hpGrow; ++i)
		{
			var at = (AttributeType)i;
			string name = Utility.GetEnumDescription(at);
			name = name.Replace("+",":");
			name = name.Replace("-",":");
			name = name.Replace("%","");

            int value = attribute.GetAttributeValue(at);

            EditorGUILayout.BeginHorizontal();

            string content = string.Format(name, value);
			GUILayout.Label(content, GUILayout.Width(200));

            //GUILayoutUtility.GetRect(0.01f, 0.01f);

            int tmpValue = EditorGUILayout.IntField("", value, GUILayout.Width(200));

            if (tmpValue != value)
            {
                attribute.SetAttributeValue(at, tmpValue);
            }
            //GUILayoutUtility.GetRect(0.01f, 0.01f);
            EditorGUILayout.EndHorizontal();
		}

        string element = "攻击属性:";
        string elementAttack = "属强:";
        string elementDefence = "属抗:";
        for(int i=1; i<(int)MagicElementType.MAX; ++i)
        {
            string name = Utility.GetEnumDescription((MagicElementType)i);

            if (attribute.battleData.magicELements[i] > 0)
            {    
                element += string.Format("{0}:{1} ", name, attribute.battleData.magicELements[i]);
            }

            if (attribute.battleData.magicElementsAttack[i] != 0)
            {
                elementAttack += string.Format("{0}:{1} ", name, attribute.battleData.magicElementsAttack[i]);
            }

            if (attribute.battleData.magicElementsDefence[i] != 0)
            {
                elementDefence += string.Format("{0}:{1} ", name, attribute.battleData.magicElementsDefence[i]);
            }
        }


        string[] elementLabels = new string[]
        {
            element,
            elementAttack,
            elementDefence,
        };

        for(int j=0; j<3; ++j)
        {
            
            GUILayout.Label(elementLabels[j]);
            //GUILayoutUtility.GetRect(1, 1);
            EditorGUILayout.BeginHorizontal();
            for (int i = 1; i < (int)MagicElementType.MAX; ++i)
            {
                string name = Utility.GetEnumDescription((MagicElementType)i);
                GUILayout.Label(name, GUILayout.Width(50));
                //GUILayoutUtility.GetRect(1, 1);

                if (j == 0)
                {
                    int value = EditorGUILayout.IntField("", attribute.battleData.magicELements[i], GUILayout.Width(50));
                    if (value != attribute.battleData.magicELements[i])
                        attribute.battleData.magicELements[i] = value;
                }
                else if (j == 1)
                {
                    int value = EditorGUILayout.IntField("", attribute.battleData.magicElementsAttack[i], GUILayout.Width(50));
                    if (value != attribute.battleData.magicElementsAttack[i])
                        attribute.battleData.magicElementsAttack[i] = value;
                }
                else if (j == 2)
                {
                    int value = EditorGUILayout.IntField("", attribute.battleData.magicElementsDefence[i], GUILayout.Width(50));
                    if (value != attribute.battleData.magicElementsDefence[i])
                        attribute.battleData.magicElementsDefence[i] = value;
                }

                //GUILayoutUtility.GetRect(1, 1);
            }

            EditorGUILayout.EndHorizontal();
        }

        string abnormal = "异常抗性:";
        for(int i= (int)BuffType.FLASH; i<=(int)BuffType.CURSE; ++i)
        {
            int index = i - (int)BuffType.FLASH;
            string name = Utility.GetEnumDescription((BuffType)i);

            if (attribute.battleData.abnormalResists[index] > 0)
                abnormal += string.Format("{0}:{1} ", name, attribute.battleData.abnormalResists[index]);
        }

        

        GUILayout.Label(abnormal);
        EditorGUILayout.BeginHorizontal();
        //GUILayoutUtility.GetRect(1, 1);
        for (int i = (int)BuffType.FLASH, j = 1; i <= (int)BuffType.CURSE; ++i, ++j) 
        {
            int index = i - (int)BuffType.FLASH;
            string name = Utility.GetEnumDescription((BuffType)i);

            GUILayout.Label(name, GUILayout.Width(50));
            int value = EditorGUILayout.IntField("", attribute.battleData.abnormalResists[index], GUILayout.Width(50));
            if (value != attribute.battleData.abnormalResists[index])
                attribute.battleData.abnormalResists[index] = value;
            if (j % 4 == 0 ) 
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        string resistContent = string.Format("抗魔值:{0}", attribute.battleData.resistMagic);
        GUILayout.Label(resistContent, GUILayout.Width(200));
        //GUILayoutUtility.GetRect(1, 1);
        int tmpResist = EditorGUILayout.IntField("", attribute.battleData.resistMagic, GUILayout.Width(200));

        if (tmpResist != attribute.battleData.resistMagic)
        {
            attribute.battleData.resistMagic = tmpResist;
            attribute.ChangeMaxHpByResist();
        }
        EditorGUILayout.EndHorizontal();


        string[] damageTexts = new string[]
       {
            "附加伤害固定值",
            "附加伤害百分比",
            "增加伤害固定值",
            "增加伤害百分比",
            "减伤固定值",
            "减伤百分比",
            "反伤固定值",
            "反伤百分比",
            "附加减伤百分比",
            "魅影减伤百分比",
            "格挡减伤百分比",
            "职业技能增加物理伤害百分比",
            "职业技能增加魔法伤害百分比",
       };

        List<AddDamageInfo>[] damageValues = new List<AddDamageInfo>[]
        {
            attribute.battleData.attachAddDamageFix,
            attribute.battleData.attachAddDamagePercent,
            attribute.battleData.addDamageFix,
            attribute.battleData.addDamagePercent,
            attribute.battleData.reduceDamageFix,
            attribute.battleData.reduceDamagePercent,
            attribute.battleData.reflectDamageFix,
            attribute.battleData.reflectDamagePercent,
            attribute.battleData.reduceExtraDamagePercent,
            attribute.battleData.reduceMeiyingDamagePercent,
            attribute.battleData.reduceGeDangDamagePercent,
            attribute.battleData.skillAddDamagePercent,
            attribute.battleData.skillAddMagicDamagePercent,
        };

        for(int i=0; i<damageTexts.Length; ++i)
        {
            string text = damageTexts[i];

            var damageValue = damageValues[i];

            for (int j=0; j< damageValue.Count; ++j)
            {
                text += string.Format(" [{0}]{1}", damageValue[j].attackType, damageValue[j].value);
            }
            if (damageValue.Count == 0)
                text += " 无";

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(text, GUILayout.Width(300));
            //GUILayoutUtility.GetRect(1, 1);

            if (damageValue.Count > 0)
            {
                List<string> tmp = new List<string>();

                for (int j = 0; j < damageValue.Count; ++j)
                {
                    
                    tmp.Add(string.Format("[{0}]{1}", damageValue[j].attackType, damageValue[j].value));
                }

                int value = EditorGUILayout.IntField("", damageValue[selectValues[i]].value, GUILayout.Width(50));
                if (value != damageValue[selectValues[i]].value)
                {
                    var t = damageValue[selectValues[i]];
                    t.value = value;
                    damageValue[selectValues[i]] = t;
                }
                    

                selectValues[i] = EditorGUILayout.Popup(selectValues[i], tmp.ToArray(), GUILayout.Width(200));
                //GUILayoutUtility.GetRect(1, 1);


            }


            EditorGUILayout.EndHorizontal();
        }
                                                                                                                                                                                                                                                                        

        string crystal = "无色水晶数量:";
        crystal += attribute.GetCrystalNum();
        GUILayout.Label(crystal);

        EditorGUILayout.EndVertical();
	}
		
	void _showPanelAttribute(DisplayAttribute display)
	{
		GUILayout.Label("面板属性:");
		EditorGUILayout.BeginVertical("GroupBox");

		GUILayout.Label(string.Format("当前HP:{0}", display.hp));
		GUILayout.Label(string.Format("当前MP:{0}", display.mp));
		for(int i=0; i<(int)AttributeType.hpGrow; ++i)
		{
			var at = (AttributeType)i;
			string fieldName = Global.GetAttributeString(at);
			string name = Utility.GetEnumDescription(at);
			name = name.Replace("+",":");
			name = name.Replace("-",":");

			Type type = typeof(DisplayAttribute);
			var fieldInfo = type.GetField(fieldName);
			if (fieldInfo != null)
			{
				float value = (float)fieldInfo.GetValue(display);
				string content = string.Format(name, value);
				GUILayout.Label(content);
			}
		}



		EditorGUILayout.EndVertical();
	}

	void ShowFunction(BeActor actor)
	{
		EditorGUILayout.BeginVertical("GroupBox");

		//添加buff
		EditorGUILayout.BeginHorizontal();
		int width = 50;
		GUILayout.Label("ID", GUILayout.Width(width));
		_buffId = EditorGUILayout.IntField("", _buffId, GUILayout.Width(width));
		GUILayout.Label("时间", GUILayout.Width(width));
		_buffDur = EditorGUILayout.IntField("", _buffDur, GUILayout.Width(width));
		GUILayout.Label("概率", GUILayout.Width(width));
		_buffProb = EditorGUILayout.IntField("", _buffProb,GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
		GUILayout.Label("等级", GUILayout.Width(width));
		_buffLevel = EditorGUILayout.IntField("", _buffLevel, GUILayout.Width(width));
        GUILayout.Label("异常攻击", GUILayout.Width(width));
        _buffAttack = EditorGUILayout.IntField("", _buffAttack, GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加BUFF", "minibutton", GUILayout.Width(150)))
		{
			if (actor != null)
            {
                actor.buffController.TryAddBuff(_buffId, _buffDur, _buffLevel, _buffProb, _buffAttack,false,null,0,0,actor);
            }
		}
        if (GUILayout.Button("删除BUFF", "minibutton", GUILayout.Width(150)))
        {
            if (actor != null)
            {
                actor.buffController.RemoveBuff(_buffId);
            }
        }
        EditorGUILayout.EndHorizontal();

		//添加buff信息表ID
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("ID", GUILayout.Width(width));
        _buffInfoID = EditorGUILayout.IntField("", _buffInfoID, GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("添加BUFF信息表ID","minibutton", GUILayout.Width(150)))
		{
			if (actor != null)
            {
				actor.buffController.AddBuffInfoByID(_buffInfoID);
            }
		}
        if (GUILayout.Button("删除BUFF信息表ID", "minibutton", GUILayout.Width(150)))
        {
            if (actor != null)
            {
                actor.buffController.RemoveBuffByBuffInfoID(_buffInfoID);
            }
        }
		EditorGUILayout.EndHorizontal();

		//添加机制表ID
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("ID", GUILayout.Width(width));
        _mechanismID = EditorGUILayout.IntField("", _mechanismID, GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加机制表ID", "minibutton", GUILayout.Width(150)))
        {
            if (actor != null)
            {
                actor.AddMechanism(_mechanismID);
            }
        }
        if (GUILayout.Button("删除机制表ID", "minibutton", GUILayout.Width(150)))
        {
            if (actor != null)
            {
                actor.RemoveMechanism(_mechanismID);
            }
        }
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginVertical("GroupBox");
		GUILayout.Label("剧情AI", GUILayout.Width(width));
		_scenraioTreeName = EditorGUILayout.TextField("剧情AI名字:", _scenraioTreeName);
		if (GUILayout.Button("开始剧情AI", "minibutton", GUILayout.Width(150)))
		{
			if (actor != null && actor.aiManager != null)
			{
				(actor.aiManager as BeActorAIManager).StartScenario(_scenraioTreeName);
			}
		}
		if (GUILayout.Button("停止当前剧情AI", "minibutton", GUILayout.Width(150)))
		{
			if (actor != null && actor.aiManager != null)
			{
				(actor.aiManager as BeActorAIManager).StopScenario();
			}
		}
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndVertical();
	}

    //显示决斗场机器人使用技能信息
    void ShowUseSkillInfo(BeActor actor)
    {
#if !LOGIC_SERVER
        if (actor == null)
            return;
        EditorGUILayout.BeginVertical("GroupBox");
        if(actor == null || actor.GetStateGraph()== null)
        {
            return;
        }
        var skillUseRecord = actor.GetStateGraph().skillUseRecord;
        foreach (var skillUseLog in skillUseRecord)
        {
            var content = skillUseLog;
            GUILayout.Label(content);
        }
        EditorGUILayout.EndVertical();
#endif
    }

    //显示攻击到目标的触发效果ID
    void ShowHurtIdInfo(BeActor actor)
    {
        if (actor == null)
            return;
        EditorGUILayout.BeginVertical("GroupBox");
        var recordHurtIdList = actor.RecordHurtIdList;
        foreach (var log in recordHurtIdList)
        {
            var content = log;
            GUILayout.Label(content);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    ///显示事件监听列表
    /// </summary>
    void ShowEventHandleList(BeActor actor)
    {
        if (actor == null)
            return;
        EditorGUILayout.BeginVertical("GroupBox");
        //新的事件列表
        if (actor.EventManager != null)
        {
            Dictionary<int, List<BeEvent.BeEventHandleNew>> data = actor.EventManager.GetNewEventHandleList();
            if (data != null)
            {
                int num = 0;
                GUILayout.Label("新事件列表:");
                var enumorator = data.GetEnumerator();
                while (enumorator.MoveNext())
                {
                    var value = enumorator.Current.Value;
                    for (int i = 0; i < value.Count; i++)
                    {
                        var handle = value[i];
                        
                        string content = String.Format("类型:{0}, 状态:{1}({2}.{3})", (BeEventType)handle.m_EventType, handle.m_CanRemove ? "死亡" : "存活", handle.m_Fn.Method.ReflectedType, handle.m_Fn.Method.Name);
                        GUILayout.Label(content);
                        num++;
                    }
                }
                string count = string.Format("当前新事件数量:{0}", num);
                GUILayout.Label(count);
            }
        }
        EditorGUILayout.EndVertical();
    }

    void ShowAIInfo(BeActor actor)
    {
        folderAIInfo = EditorGUILayout.Foldout(folderAIInfo, "AI信息");
        if (!folderAIInfo)
            return;

        if (actor.aiManager != null)
        {
            BeActorAIManager ai = (BeActorAIManager)actor.aiManager;

            EditorGUILayout.BeginVertical("GroupBox");

            actor.pauseAI = EditorGUILayout.Toggle("是否暂停AI", actor.pauseAI);

            if (ai.actionAgent != null)
            {
                GUILayout.Label("攻击：使用了行为树 " + ai.actionAgent.GetName() + "(" + ai.actionAgent.path + ")");
            }

            if (ai.destinationSelectAgent != null)
            {
                GUILayout.Label("寻路：使用了行为树 " + ai.destinationSelectAgent.GetName() + "(" + ai.destinationSelectAgent.path + ")");
            }

            if (ai.eventAgent != null)
            {
                GUILayout.Label("事件：使用了行为树 " + ai.eventAgent.GetName() + "(" + ai.eventAgent.path + ")");
            }

            if (ai.scenarioAgent != null)
            {
                GUILayout.Label("剧情AI：使用了行为树 " + ai.scenarioAgent.GetName() + "(" + ai.scenarioAgent.path + ")");
            }

            GUILayout.Label("计时器:");
            GUILayout.Label(string.Format("选择攻击目标计时器:{0}/{1}", ai.updateFindTargetAcc, ai.findTargetTerm));
            GUILayout.Label(string.Format("选择攻击计时器:{0}/{1}", ai.updateActionTimeAcc, ai.thinkTerm));
            GUILayout.Label(string.Format("选择目的地间隔:{0}/{1}", ai.updateDestionTimeAcc, ai.changeDestinationTerm));
            GUILayout.Label(string.Format("选择执行事件间隔:{0}/{1}", ai.updateEventTimeAcc, ai.eventTerm));
            GUILayout.Label(string.Format("剧情AI事件间隔:{0}/{1}", ai.updateScenarioTimeAcc, ai.scenarioTerm));

            ShowAIParameterInfo(ai);

            if (ai.scenarioAgent != null)
            {
                if (GUILayout.Button("重置行为树", "minibutton", GUILayout.Width(150)))
                {
                    ai.scenarioAgent.Reset();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

    void ShowAIParameterInfo(BeActorAIManager ai)
    {
        folderAIParameterInfo = EditorGUILayout.Foldout(folderAIParameterInfo, "AI参数信息");
        if (!folderAIParameterInfo)
        {
            return;
        }

        if (ai == null)
        {
            return;
        }
        ai.thinkTerm = EditorGUILayout.IntField("选择攻击间隔(ms)", ai.thinkTerm);
        ai.findTargetTerm = EditorGUILayout.IntField("选择攻击目标间隔(ms)", ai.findTargetTerm);
        ai.changeDestinationTerm = EditorGUILayout.IntField("选择目的地间隔(ms)", ai.changeDestinationTerm);
        ai.eventTerm = EditorGUILayout.IntField("选择执行事件间隔(ms)", ai.eventTerm);
        ai.scenarioTerm = EditorGUILayout.IntField("选择剧情AI间隔(ms)", ai.scenarioTerm);
        
        //ai.idleMode = (IdleMode)EditorGUILayout.EnumPopup("idle时的模式", ai.idleMode);

        ai.warlike = EditorGUILayout.IntField("好战度", ai.warlike);

        ai.sight = EditorGUILayout.IntField("视野", ai.sight);
        ai.chaseSight = EditorGUILayout.IntField("追击视野", ai.chaseSight);

        ai.skIntMaxRunAwayDisX = EditorGUILayout.IntField("最大逃跑距离X", ai.skIntMaxRunAwayDisX.i);
        ai.skIntMaxRunAwayDisY = EditorGUILayout.IntField("最大逃跑距离Y", ai.skIntMaxRunAwayDisY.i);
        ai.skIntKeepDis_TableX = EditorGUILayout.IntField("保持距离_读表X", ai.skIntKeepDis_TableX.i);
        ai.skIntKeepDis_TableY = EditorGUILayout.IntField("保持距离_读表Y", ai.skIntKeepDis_TableY.i);
        ai.skIntFrontFaceAndZigZagYDis = EditorGUILayout.IntField("Y移动距离（Z字，避免正面）", ai.skIntFrontFaceAndZigZagYDis.i);
        ai.wanderRange = EditorGUILayout.IntField("游荡范围（自身为中心）", ai.wanderRange);
        ai.wanderByTargetRange = EditorGUILayout.IntField("游荡范围（目标为中心）", ai.wanderByTargetRange);

        ai.destinationTypeTest = (BeAIManager.DestinationType)EditorGUILayout.EnumPopup("寻路方式", ai.destinationTypeTest);
    }

    #endregion

    #region battleInfo

    void ShowBattleInfo()
	{
        //GUILayout.Space(10);
        //GUILayout.Label("战斗信息", fontStyle);
		EditorGUILayout.BeginVertical("GroupBox");

        showBattleInfo = EditorGUILayout.Foldout(showBattleInfo, "战斗信息");
        if (showBattleInfo)
        {
            GUILayout.Space(10);

            string fps = string.Format("FPS:{0}", ComponentFPS.instance.GetFPS());
            GUILayout.Label(fps);

            string lastAverageFps = string.Format("Last Average FPS:{0} ({1}/{2})", ComponentFPS.instance.GetLastAverageFPS(), ComponentFPS.instance.frameCount, ComponentFPS.instance.watchFrames);
            GUILayout.Label(lastAverageFps);
            string needProtomoted = string.Format("Need Protomoted:{0}", GeGraphicSetting.instance.needPromoted);
            GUILayout.Label(needProtomoted);

            battle1 = EditorGUILayout.Toggle("显示Pool信息(剩余/全部)", battle1);
            if (battle1)
            {
                ShowPoolInfo();
            }

            BeActor.useNewLoadingSkill = EditorGUILayout.Toggle("使用区分武器类型的技能加载模式", BeActor.useNewLoadingSkill);

            ComActorInfoDebug.bShowDebugInfo = EditorGUILayout.Toggle("调试字符串", ComActorInfoDebug.bShowDebugInfo);
            ComActorInfoDebug.bShowSight = EditorGUILayout.Toggle("视野框", ComActorInfoDebug.bShowSight);
            ComActorInfoDebug.bShowLevelDetail = EditorGUILayout.Toggle("等级信息", ComActorInfoDebug.bShowLevelDetail);
        }

        EditorGUILayout.EndVertical ();
	}

	void ShowPoolInfo()
	{
		var info = CPoolManager.GetInstance ().GetPoolsInfo ();
		GUILayout.Label (info);
	}

    #endregion

    #region levelInfo
    private TableInspetor<ProtoTable.DungeonTable> mDungeonTable = new TableInspetor<ProtoTable.DungeonTable>("单机关卡ID(地下城表)", true);
    private void _ShowLevel()
    {
	    EditorGUILayout.BeginVertical("GroupBox");
	    showLevel = EditorGUILayout.Foldout(showLevel, "关卡");
	    if (showLevel)
	    {
		    Global.Settings.localDungeonID = mDungeonTable.OnGUIWithID(Global.Settings.localDungeonID);
		    if (EditorApplication.isPlayingOrWillChangePlaymode)
		    {
			    if (GUILayout.Button(string.Format("加载关卡:{0}",  Global.Settings.localDungeonID),"minibutton"))
			    {
				    ClientSystemManager.instance.SwitchSystem<ClientSystemBattle>(null,null,true);
				    currentPlayer = null;
			    }
		    }
		    else
		    {
			    if (GUILayout.Button(string.Format("开始"), "minibutton"))
			    {
				    EditorApplication.isPlaying = true;
			    }
		    }
	    }
	    EditorGUILayout.EndVertical();
    }
    

    #endregion
}
