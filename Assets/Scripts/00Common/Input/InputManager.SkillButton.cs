using System;
using System.Collections.Generic;
using GameClient;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能按钮相关（新）
/// </summary>
public partial class InputManager
{
    public struct SkillSlotData
    {
        public int slot;
        public int skillId;
    }

    private GameObject mObETCButtons = null;
    private GameObject mObETCButtonRoot = null;
    private GameObject mObETCEffectRoot = null;
    public GameObject ETCButtons{ get { return mObETCButtons; } }
    public GameObject ETCEffectRoot{ get { return mObETCEffectRoot; } }
    private Dictionary<int, ETCButton> mButtonMap = new Dictionary<int, ETCButton>();
    public Dictionary<int, ETCButton> ButtonSlotMap { get { return mButtonMap; } }

    private List<SkillSlotData> _skillSlotList;
    //private Dictionary<int, int> _skillSlotDic;

    private int _buffSkillStartIndex;

    const int BUFF_SLOT_INDEX = 16;
    const int AWAKE_SLOT_INDEX = 19;//觉醒技能槽
    const int QTE_SLOT_INDEX = 20;
    const int RUN_ATTACK_INDEX = 21;

    private bool _needCheckPrejob = false;

    private void _LoadSkillButton(int pid, Dictionary<int, int> skillSlotDic)
    {
        mPid = pid;
        var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(mPid);
        if (jobData != null)
            _needCheckPrejob = jobData.JobType == 0;
        _ClearSkillButtonData();
        _UnLoadSkillButtonUI();
        if (skillSlotDic != null)
        {
            var enumerator = skillSlotDic.GetEnumerator();
            while (enumerator.MoveNext())
            {

                var current = enumerator.Current;
                
                SkillSlotData skillSlotData = new SkillSlotData();
                skillSlotData.slot = current.Key;
                skillSlotData.skillId = current.Value;

                _skillSlotList.Add(skillSlotData);
            }
        }
        else
            _skillSlotList = _GetBattleSkillSlotInfo();
        if (SwitchFunctionUtility.IsOpen(69))
            SortBuffSkillSlot(_skillSlotList);

        mObETCButtons = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/ETCInput/ETCButtonsModeNormalNew");

        if (mObETCButtons == null)
        {
            Logger.LogErrorFormat("没有加载到技能按钮界面");
            return;
        }

        mObETCButtonRoot = mObETCButtons.transform.Find("ButtonRoot").gameObject;
        mObETCEffectRoot = mObETCButtons.transform.Find("EffectRoot").gameObject;

        if (mObETCButtonRoot == null)
            return;

        Utility.AttachTo(mObETCButtons, ClientSystemManager.instance.MiddleLayer);
        mObETCButtons.transform.SetAsLastSibling();

        for (int i = 0; i < mObETCButtonRoot.transform.childCount; i++)
        {
            var transform = mObETCButtonRoot.transform.GetChild(i);
            var etcButton = transform.GetComponent<ETCButton>();

            int slot = i + 1;
            if (!mButtonMap.ContainsKey(slot))
                mButtonMap.Add(slot, etcButton);
            etcButton.SetSkillBtnVisible(slot == 1 || slot == 3);
            etcButton.InPutEffectRoot = mObETCEffectRoot;

            // marked by ckm
            stateDic[i] = etcButton.gameObject.activeSelf;
            // stateDic[slot] = etcButton.gameObject.activeSelf;
            _InitSingleEtcButtonData(slot, etcButton);
        }
    }

    public void ResetETCEffectTrans()
    {
        if (mObETCButtonRoot != null)
        {
            for (int i = 0; i < mObETCButtonRoot.transform.childCount; i++)
            {
                var transform = mObETCButtonRoot.transform.GetChild(i);
                var etcButton = transform.GetComponent<ETCButton>();

                if (etcButton != null && etcButton.EffectRoot != null)
                {
                    etcButton.EffectRoot.transform.localPosition = etcButton.transform.localPosition;
                    etcButton.EffectRoot.transform.localScale = etcButton.transform.localScale;
                }
            }
        }
    }

    private void _ClearSkillButtonData()
    {
        if (mButtonMap != null)
            mButtonMap.Clear();
        if (_skillSlotList != null)
            _skillSlotList.Clear();
    }

    /// <summary>
    /// 初始化单个技能按钮的数据
    /// </summary>
    private void _InitSingleEtcButtonData(int slot, ETCButton skillBtn)
    {
        int index = FindSlotIndexInList(_skillSlotList, slot);
        if (index < 0) return;

        int slotSkillId = _skillSlotList[index].skillId;

        skillBtn.onDown.AddListener(() =>
        {
            OnSkillButonPress(slot, slotSkillId, false);
        });

        skillBtn.onUp.AddListener(() =>
        {
            OnSkillButonPress(slot, slotSkillId, true);
        });

        if (!mDictSkillID.ContainsKey(slotSkillId))
            mDictSkillID.Add(slotSkillId, skillBtn);

        var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(slotSkillId);
        if (skillData == null)
            return;

        skillBtn.SetFgImage(skillData.Icon, false);
        
        if (_needCheckPrejob)
        {
            bool ret = ChangePrejobSkillImage(slotSkillId, skillBtn);
            if (ret)
                _needCheckPrejob = false;
        }

        AddButtonEffect(slotSkillId, skillBtn);

        skillBtn.SetSkillBtnVisible(slot < QTE_SLOT_INDEX);
    }

    private bool ChangePrejobSkillImage(int skillID, ETCButton button)
    {
        var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID);
        if (skillData != null && skillData.IsPreJob > 0)
        {
            //替换图标

            var com = button.gameObject.GetComponent<Image>();
            if (com != null)
            {
                var pic = AssetLoader.instance.LoadRes("UI/Image/Icon/Icon_skillIcon/Common/Common_skill button_yuzhanzhi.png:Common_skill button_yuzhanzhi", typeof(Sprite), false);
                if (pic != null && pic.obj != null)
                {
                    button.normalSprite = pic.obj as Sprite;

                    var color = button.normalColor;
                    //color.a = 1.0f;

                    button.normalColor = color;

                    com.SetNativeSize();

                    return true;
                }
            }
        }

        return false;
    }

    private void AddButtonEffect(int slotSkillId,ETCButton skillBtn)
    {
        if (null == controllActor) return;
        var skill = controllActor.GetSkill(slotSkillId);
        if (skill == null) return;

        if (skill.charge && !skill.hideSpellBar)
        {
            skillBtn.AddEffect(ETCButton.eEffectType.onCharge);
        }
        else if (skill.isQTE || skill.isRunAttack)
        {
            skillBtn.AddEffect(ETCButton.eEffectType.onContinue);
            skillBtn.SetSkillBtnVisible(false);
        }
        else if (skill.canSlide)
        {
            skillBtn.AddEffect(ETCButton.eEffectType.onSlide);
        }

        if (SkillDataManager.GetInstance().IsSkillNewForBattle(skill.skillID))
        {
            skillBtn.AddEffect(ETCButton.eEffectType.onNew);
        }
#if !LOGIC_SERVER
        skill.button = skillBtn;
#endif
    }

    private void _UnLoadSkillButtonUI()
    {
        if (mObETCButtons != null)
        {
            GameObject.Destroy(mObETCButtons);
            mObETCButtons = null;
        }

        if (mObETCButtonRoot != null)
        {
            mObETCButtonRoot = null;
        }
        
        if (mObETCEffectRoot != null)
        {
            mObETCEffectRoot = null;
        }
    }

    public void OnSkillButonPress(int slot, int skillId, bool isUp)
    {
        //普攻 跳 后跳 
        int sendSkillId = skillId;
        if (slot >= 1 && slot <= 3)
        {
            switch (slot)
            {
                case 1:
                    sendSkillId = (int)SpecialSkillID.NORMAL_ATTACK;
                    break;
                case 2:
                    sendSkillId = (int)SpecialSkillID.JUMP;
                    break;
                case 3:
                    sendSkillId = (int)SpecialSkillID.JUMP_BACK;
                    break;
            }
            CreateSkillFrameCommand(sendSkillId, new SkillFrameCommand.SkillFrameData{isUp = isUp});
            return;
        }

        BeActor actor = mControllActor;
        if (actor == null) return;
        var skill = actor.GetSkill(skillId);
        if (skill == null) return;

        if (!skill.isCooldown)
        {
            if (!skill.canRemoveJoystick)
            {
                // 该模式在技能释放后创建摇杆，摇杆UI覆盖技能按键会触发技能按键抬起，过滤掉抬起帧
                if (isUp)
                    sendSkillId = -1;
            }
            else if (skill.joystickMode == SkillJoystickMode.FREE)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp);
            }
            else if (skill.joystickMode == SkillJoystickMode.SPECIAL || skill.joystickMode == SkillJoystickMode.FORWARDBACK)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp);
                sendSkillId = -1;
            }
            else if (skill.joystickMode == SkillJoystickMode.SELECTSEAT && BeUtility.GetPlayerCount() > 1)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp, SkillJoystickMode.SELECTSEAT);
                sendSkillId = -1;
            }
            else if (skill.joystickMode == SkillJoystickMode.DIRECTION)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp);
            }
			else if (skill.joystickMode == SkillJoystickMode.MODESELECT)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp, SkillJoystickMode.MODESELECT);
                sendSkillId = -1;
            }
            else if (skill.joystickMode == SkillJoystickMode.ACTIONSELECT)
            {
                LoadButtonJoystick(slot, actor, skillId, isUp, SkillJoystickMode.ACTIONSELECT);
                sendSkillId = -1;
            }
        }
        else if (skill.joystickMode == SkillJoystickMode.DIRECTION)
        {
            LoadButtonJoystick(slot, actor, skillId, isUp);
        }
		else if (skill.joystickMode == SkillJoystickMode.MODESELECT )
        { 
            if (skill is IModeSelectSkill modeSkill && modeSkill.CanJoystickOnCD())
            {
                LoadButtonJoystick(slot, actor, skillId, isUp, SkillJoystickMode.MODESELECT);
                sendSkillId = -1;
            }
        }

        if (sendSkillId > 0)
        {
            CreateSkillFrameCommand(sendSkillId, new SkillFrameCommand.SkillFrameData{isUp = isUp});
        }
    }

    public void HiddenJump()
    {
        if (mButtonMap.ContainsKey(2))
            mButtonMap[2].SetSkillBtnVisible(false);
        if (mButtonMap.ContainsKey(3))
            mButtonMap[3].SetSkillBtnVisible(false);
    }

    public void ShowJump()
    {
        if (mButtonMap.ContainsKey(2))
            mButtonMap[2].SetSkillBtnVisible(false);
        if (mButtonMap.ContainsKey(2))
            mButtonMap[3].SetSkillBtnVisible(true);
    }


#region 初始化技能槽位信息数据

    /// <summary>
    /// 技能与槽位对应关系
    /// </summary>
    public List<SkillSlotData> _GetBattleSkillSlotInfo()
    {
        List<SkillSlotData> skillSlotList = new List<SkillSlotData>();

        if (BattleMain.instance == null) return skillSlotList;

        var battletype = BattleMain.battleType;
        _buffSkillStartIndex = 0;

        if (Global.Settings.startSystem != EClientSystem.Battle)
        {
            if (battletype == BattleType.TrainingPVE)
                _InitTrainingBattleSkillSlot(false, skillSlotList);
            else if (battletype == BattleType.Training)
                _InitTrainingBattleSkillSlot(true, skillSlotList);
            else if (battletype == BattleType.NewbieGuide)
                _InitTrainingBattleSkillSlot(false, skillSlotList);
            else if (battletype == BattleType.InputSetting)
                _InitTrainingBattleSkillSlot(false, skillSlotList);
            else if (battletype == BattleType.ChangeOccu)
                _InitChangeOccuBattleSkillSlot(skillSlotList);
            else
                _InitBattleSyncSkillSlotDic(skillSlotList);
        }
        else
            _InitBattleTestSkillSlotDic(skillSlotList);

        return skillSlotList;
    }

    /// <summary>
    /// 获取服务器下发的技能按钮和槽位对应关系
    /// </summary>
    private void _InitBattleSyncSkillSlotDic(List<SkillSlotData> skillSlotDic)
    {
        if (BattleDataManager.GetInstance() == null) return;

        RacePlayerInfo localPlayer = BattleDataManager.GetInstance().GetLocalPlayerInfo();

        if (localPlayer == null) return;

        _AddCommonSkillSlot(skillSlotDic);

        for (int i = 0; i < localPlayer.skills.Length; i++)
        {
            var skill = localPlayer.skills[i];
            _AddSingleSkillSlot(skill.id, skillSlotDic, skill.slot + 3);
        }
    }

    /// <summary>
    /// 初始化转职体验战斗模式下的技能按钮和槽位对应关系
    /// </summary>
    private void _InitChangeOccuBattleSkillSlot(List<SkillSlotData> skillSlotDic)
    {
        if (BattleMain.instance == null)
            return;;
        if (BattleMain.instance.GetPlayerManager() == null)
            return;
        var battlePlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (battlePlayer == null || battlePlayer.playerInfo == null)
            return;
        int roleId = battlePlayer.playerInfo.occupation;
        var jobData = TableManager.instance.GetTableItem<ProtoTable.JobTable>(roleId);
        if (jobData == null) return;

        //List<int> skillList = new List<int>();
        _AddCommonSkillSlot(skillSlotDic);
        
        int buffCount = 0;
        int skillCount = 0;
        for (int i = 0; i < jobData.ChangeOccuSkillsLength; ++i)
        {
            var skillId = jobData.ChangeOccuSkills[i];
            var skillData = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(skillId);
            if (skillData == null)
            {
                continue;
            }

            int slot = -1;

            if (skillData.IsBuff > 0)
            {
                if (buffCount < 4)
                {
                    slot = (byte)(BUFF_SLOT_INDEX + buffCount);
                    buffCount++;
                }
            }
            else
            {
                if (skillData.SkillType == ProtoTable.SkillTable.eSkillType.ACTIVE)
                {
                    if (skillCount < BUFF_SLOT_INDEX)
                    {
                        slot = skillCount;
                    }
                    else
                    {
                        slot = skillCount + 4;
                    }
                    skillCount++;
                }
            }

            if (slot > -1)
            {
                _AddSingleSkillSlot(skillId, skillSlotDic, slot);
            }
        }
        
    }
    
    /// <summary>
    /// 初始化单机战斗模式下的技能按钮和槽位对应关系
    /// </summary>
    private void _InitBattleTestSkillSlotDic(List<SkillSlotData> skillSlotDic)
    {
        int roleId = (int)Global.Settings.iSingleCharactorID;
        var jobData = TableManager.instance.GetTableItem<ProtoTable.JobTable>(roleId);
        if (jobData == null) return;

        List<int> skillList = new List<int>();

        if (jobData.BattleTestUseSkills.Length <= 0)
        {
            _AddCommonSkillSlot(skillSlotDic);

            var allSkills = TableManager.instance.GetSkillInfoByPid(roleId).Keys.ToList<int>();
            

            for(int i=allSkills.Count-1; i>=0; --i)
            {
                var skillid = allSkills[i];
                if (skillid > 5000) continue;
                var skillData = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(skillid);
                if (skillData == null || skillData.SkillType == ProtoTable.SkillTable.eSkillType.PASSIVE)
                    continue;
                if (skillData.SkillCategory != 3 && skillData.SkillCategory != 4)
                    continue;
                skillList.Add(skillid);
            }
        }
        else 
            skillList = jobData.BattleTestUseSkills.ToList<int>();

       

        int localSkillSlot = 4;

        for (int i = 0; i < skillList.Count; i++)
        {
            int skillId = skillList[i];
            int slot = localSkillSlot;

            if (_AddSingleSkillSlot(skillId, skillSlotDic, slot))
            {
                if (slot >= BUFF_SLOT_INDEX)
                    ;
                else
                    localSkillSlot++;
            }
                
        }
    }

    /// <summary>
    /// 初始化练习模式下的技能槽位配置(因为不走服务器开始开始战斗协议)
    /// </summary>
    private void _InitTrainingBattleSkillSlot(bool isPvp, List<SkillSlotData> skillSlotList)
    {
        if (SkillDataManager.GetInstance() == null) return;

        _AddCommonSkillSlot(skillSlotList);

        var skillBarGridList = SkillDataManager.GetInstance().GetSkillConfiguration(isPvp);
        for (int i = 0; i < skillBarGridList.Count; i++)
        {
            _AddSingleSkillSlot(skillBarGridList[i].id, skillSlotList, skillBarGridList[i].slot + 3);
        }

        var buffSkillList = SkillDataManager.GetInstance().GetBuffSkillID(isPvp);
        for (int i = 0; i < buffSkillList.Count; i++)
        {
            _AddSingleSkillSlot(buffSkillList[i], skillSlotList);
        }

        var qteSkillID = SkillDataManager.GetInstance().GetQTESkillID(isPvp);
        _AddSingleSkillSlot(qteSkillID, skillSlotList);

        var runAttackSkillID = SkillDataManager.GetInstance().GetRunAttackSkillID(isPvp);
        _AddSingleSkillSlot(runAttackSkillID, skillSlotList);

        var awakeSkillId = PlayerBaseData.GetInstance().GetAwakeSkillID();
        _AddSingleSkillSlot(awakeSkillId, skillSlotList);
    }

    /// <summary>
    /// 添加普攻 跳 后跳等基本技能按钮槽位信息
    /// </summary>
    private void _AddCommonSkillSlot(List<SkillSlotData> skillSlotList)
    {
        var jobTable = TableManager.instance.GetTableItem<ProtoTable.JobTable>(mPid);
        if (jobTable != null)
        {
            int normalAttackId = BattleMain.IsChiji() ? jobTable.ChijiNormalAttackID : jobTable.NormalAttackID;
            
            SkillSlotData data1 = ConstructionSkillSlot(1, normalAttackId);

            skillSlotList.Add(data1);
        }
        else
        {
            Logger.LogErrorFormat("职业Id错误 Id:{0}", mPid);

            SkillSlotData data1 = ConstructionSkillSlot(1,-1);
            skillSlotList.Add(data1);
        }

        SkillSlotData data2 = ConstructionSkillSlot(2, -1);
        skillSlotList.Add(data2);

        SkillSlotData data3 = ConstructionSkillSlot(3, -1);
        skillSlotList.Add(data3);
    }

    /// <summary>
    /// 添加单个技能的槽位信息
    /// </summary>
    private bool _AddSingleSkillSlot(int skillId, List<SkillSlotData> skillSlotList, int slot = 0)
    {
        bool ret = true;
        int realSlot = slot;
        TableManager.SkillType type = TableManager.instance.GetSkillType(skillId);
        switch (type)
        {
            case TableManager.SkillType.BuffSkill:
                realSlot = _buffSkillStartIndex + BUFF_SLOT_INDEX;
                _buffSkillStartIndex++;
                ret = false;
                break;
            case TableManager.SkillType.QteSkill:
                realSlot = QTE_SLOT_INDEX;
                ret = false;
                break;
            case TableManager.SkillType.RunAttackSkill:
                realSlot = RUN_ATTACK_INDEX;
                ret = false;
                break;
            case TableManager.SkillType.AwakeSkill:
                realSlot = AWAKE_SLOT_INDEX;
                ret = false;
                break;
        }
        if (realSlot <= 0) return false;
        if(FindSlotIndexInList(skillSlotList,realSlot)>0)
        {
            if (realSlot > 3)
                Logger.LogErrorFormat("已经包含槽位技能信息，槽位:{0} 技能:{1}", realSlot, skillId);
            return false;
        }
        var data = ConstructionSkillSlot(realSlot, skillId);
        skillSlotList.Add(data);

        return ret;
    }

    /// <summary>
    /// 构造一个技能槽位数据
    /// </summary>
    /// <param name="slot">槽位</param>
    /// <param name="skillId">技能Id</param>
    private SkillSlotData ConstructionSkillSlot(int slot, int skillId)
    {
        SkillSlotData data = new SkillSlotData();
        data.slot = slot;
        data.skillId = skillId;
        return data;
    }
    
    private int FindSlotIndexInList(List<SkillSlotData> list,int slot)
    {
        for(int i=0;i< list.Count; i++)
        {
            var skillSlotData = list[i];
            if (skillSlotData.slot == slot) return i;
        }
        return -1;
    }

    private void SortBuffSkillSlot(List<SkillSlotData> list)
    {
        var indexList = GetBuffSkillSlotIndexs(list);
        List<int> levelList = new List<int>();
        List<int> skillIDList = new List<int>();
        for (int i = 0; i < indexList.Count; ++i)
        {
            int skillID = list[indexList[i]].skillId;
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID);
            if (data != null && data.IsBuff > 0 && skillID > 0)
            {
                var index = levelList.BinarySearch(data.LevelLimit);
                if (index < 0)
                {
                    levelList.Insert(~index, data.LevelLimit);
                    skillIDList.Insert(~index, skillID);
                }
            }
        }

        if (skillIDList.Count != indexList.Count) return;
        for(int i = 0; i < indexList.Count; ++i)
        {
            list[indexList[i]] = ConstructionSkillSlot(list[indexList[i]].slot, skillIDList[i]);
        }
    }

    private List<int> GetBuffSkillSlotIndexs(List<SkillSlotData> list)
    {
        List<int> ans = new List<int>();
        for(int i = 0; i < list.Count; ++i)
        {
            var skillSlotData = list[i];
            if (skillSlotData.slot >= BUFF_SLOT_INDEX && skillSlotData.slot < AWAKE_SLOT_INDEX)
            {
                ans.Add(i);
            }
        }
        return ans;
    }
    #endregion
}