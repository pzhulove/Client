using UnityEngine.UI;
using UnityEngine;
using GameClient;
using ProtoTable;
using UnityEditor;
using Protocol;
using System;
using System.Collections.Generic;
using Network;
using UnityEngine.EventSystems;

class ComSkillInfo : MonoBehaviour
{
    [SerializeField] private Text mTextNameLv;
    [SerializeField] private Text mTextCostItem;
    [SerializeField] private List<GameObject> mTypeList;
    [SerializeField] private List<Text> mTextTypeList;
    [SerializeField] private Text mTextSkillType;
    [SerializeField] private Text mTextSkillLvLimit;
    [SerializeField] private Text mTextSkillCd;
    [SerializeField] private Text mTextMpCost;
    [SerializeField] private Text mTextSkillDesp;
    [SerializeField] private List<SkillAttrItem> mAttrItemList;
    [SerializeField] private Text mTextNeedSp;
    [SerializeField] private Text mTextNeedLv;
    [SerializeField] private Text mTextSkillMaxLvDesp;
    [SerializeField] private Image mImgSkillMaxMask;
    [SerializeField] private UIGray mGrayUp;
    [SerializeField] private UIGray mGrayDown;
    [SerializeField] private Toggle mToggleDesp;
    [SerializeField] private Toggle mToggleAttr;
    [SerializeField] private Button mBtnTalent;
    [SerializeField] private RawImage mPreviewSkill;
    [SerializeField] private GameObject mObjNoPreview;
    [SerializeField] private Text mTextNoPreview;
    [SerializeField] private float DelayInitTime = 0.35f;
    [SerializeField] private float mDelayPreviewTime = 0.9f;

    private int mCurSkillId = 0;
    private SkillTable mSkillTable = null;

    private int mCurLv = 0;
    private int mAddLv = 0;
    private float mTimer = 0.0f;
    private bool bDelayInit = false;
    private bool mIsinitPreview = false;
    private bool mIsPreviewSkill = false;

    void Start()
    {
        _BindUIEvent();
        _Init();
    }

    void OnDestroy()
    {
        _UnBindUIEvent();
        BattlePreview.GetInstance().Stop();
        mCurSkillId = 0;
        mCurLv = 0;
        mAddLv = 0;
    }

    private void _BindUIEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePreviewCreateFinish, _OnSkillPreCreateFinish);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnChooseSkill, _OnChooseSkill);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedLevelChanged);
    }

    private void _UnBindUIEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePreviewCreateFinish, _OnSkillPreCreateFinish);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnChooseSkill, _OnChooseSkill);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedLevelChanged);
    }

    private void _OnChooseSkill(UIEvent uiEvent)
    {
        int id = (int)uiEvent.Param1;
        if(id <= 0)
        {
            return;
        }

        // CurSelectedSkillId与tableData要保证赋值上的一致性
        mSkillTable = TableManager.GetInstance().GetTableItem<SkillTable>(id);
        if(mSkillTable == null)
        {
            return;
        }

        mCurSkillId = id;
        mCurLv = (int)uiEvent.Param2;
        mAddLv = (int)uiEvent.Param3;

        if(bDelayInit)
        {
            _UpdateInfo();
        }
    }

    public void OnClickShowInfo(bool value)
    {
        if (value)
        {
            if (null == mSkillTable)
                return;
            _SetDesp();//技能描述
            _SetMpCost();//技能魔耗
            _SetSkillCd();//技能cd
            _SetSkillType();//技能类型
            _SetSkillLvLimit();//技能等级上限

            _SetAttrList();//技能属性说明
        }
    }

    //sp发生变化
    private void _OnSpChanged(UIEvent uiEvent)
    {
        _SetNeedLevelPoint();//需要等级与技能点
        _SetBtnGray();//设置按钮灰化
    }

    //设置按钮灰化
    private void _SetBtnGray()
    {
        //技能等级为0 无法降级
        mGrayDown.enabled = (!SkillDataManager.GetInstance().CheckLvDown(mCurLv, mSkillTable, false));
        //技能等级已满 或者无法学习
        mGrayUp.enabled = (!SkillDataManager.GetInstance().CheckLvUp(mCurLv, mSkillTable, false));
    }

    private void _OnSkillLearnedLevelChanged(UIEvent uiEvent)
    {
        Skill skill = uiEvent.Param1 as Skill;

        if (skill == null)
        {
            return;
        }

        if (mCurSkillId != 0 && skill.id == mCurSkillId)
        {
            mCurLv = skill.level;
            _SetSkillNameLv();//名称与等级
            _SetCostItem();//消耗道具
            _SetSkillCd();//技能cd
            _SetMpCost();//技能魔耗

            _SetAttrList();//技能属性说明

            _SetNeedLevelPoint();//需要等级与技能点
            _SetSkillMaxLvDesp();//技能满级描述和满级标记
            _SetBtnGray();//设置按钮灰化
        }
    }

    private void _Init()
    {
        mTimer = 0.0f;
        bDelayInit = false;
    }

    private void _UpdateInfo()
    {
        if (null == mSkillTable)
        {
            Logger.LogErrorFormat("技能表中找不到id = {0}的技能 找策划问问", mCurSkillId);
            return;
        }
        _SetSkillNameLv();//名字+等级
        _SetTypeList();//技能标签
        _SetSkillPreview();//技能预览
        _SetSkillTalent();//技能天赋

        _SetDesp();//技能描述
        _SetSkillType();//技能类型
        _SetSkillLvLimit();//技能等级上限

        _SetCostItem();//消耗道具
        _SetSkillCd();//技能cd
        _SetMpCost();//技能魔耗
        _SetAttrList();//技能属性说明
        _SetNeedLevelPoint();//需要等级与技能点
        _SetSkillMaxLvDesp();//技能满级描述和满级标记
        _SetBtnGray();//设置按钮灰化  
    }

    //技能天赋按钮
    private void _SetSkillTalent()
    {
        mBtnTalent.CustomActive(SkillDataManager.GetInstance().IsSkillHaveTalent(mCurSkillId));
    }

    //技能预览
    private void _InitSkillPreview()
    {
        mTextNoPreview.SafeSetText(TR.Value("skill_preview_no_content"));
        mObjNoPreview.CustomActive(false);
        _SetSkillPreview();
    }
    bool mIsStartPreview = false;
    private void _SetSkillPreview()
    {
        if (!mIsinitPreview)
            return;
        //技能预览表中有数据才能预览
        var table = TableManager.instance.GetTableItem<ProtoTable.SkillInputTable>(mCurSkillId);
        if (table == null)
        {
            mIsPreviewSkill = false;
            mObjNoPreview.CustomActive(true);
            mPreviewSkill.CustomActive(false);
            return;
        }
        mIsPreviewSkill = true;
        if (!mIsStartPreview)
        {
            mIsStartPreview = true;
            BattlePreview.instance.Start(mPreviewSkill.texture as RenderTexture, mCurSkillId);
        }
        else
        {
            mObjNoPreview.CustomActive(false);
            mPreviewSkill.CustomActive(true);
            BattlePreview.instance.UseSkill(mCurSkillId);
        }
    }
    //第一次打开时需要等待消息回来再显示
    private void _OnSkillPreCreateFinish(UIEvent uiEvent)
    {
        if (!mIsinitPreview)
            return;
        //技能预览表中有数据才能预览
        var table = TableManager.instance.GetTableItem<ProtoTable.SkillInputTable>(mCurSkillId);
        if (table == null)
        {
            mIsPreviewSkill = false;
            mObjNoPreview.CustomActive(true);
            mPreviewSkill.CustomActive(false);
            return;
        }
        mObjNoPreview.CustomActive(false);
        mPreviewSkill.CustomActive(true);
    }

    private void _SetSkillNameLv()
    {
        if (mCurLv <= 0)
            mTextNameLv.SafeSetText(TR.Value("skill_info_name_no_learn", mSkillTable.Name));
        else if(mAddLv <= 0)
            mTextNameLv.SafeSetText(TR.Value("skill_info_name_lv", mSkillTable.Name, mCurLv));
        else
            mTextNameLv.SafeSetText(TR.Value("skill_info_name_lv_add_lv", mSkillTable.Name, mCurLv, mAddLv));
    }

    //消耗晶石数量
    private void _SetCostItem()
    {
        if (null != mSkillTable)
        {
            float CrystalCost = (float)TableManager.GetValueFromUnionCell(mSkillTable.CrystalCost, mCurLv + mAddLv, false);
            if (CrystalCost > 0)
                mTextCostItem.SafeSetText(TR.Value("skill_cost_item_count", CrystalCost));
            else
                mTextCostItem.SafeSetText("");
        }
    }

    //类型列表
    private void _SetTypeList()
    {
        if (null != mSkillTable)
        {
            int index = 0;
            for (; index < mSkillTable.SkillEffect.Count; ++index)
            {
                if (index >= mTextTypeList.Count)
                {
                    break;
                }

                string tempStr = getSkillTypeText((byte)mSkillTable.SkillEffect[index]);
                if(tempStr == "")
                {
                    continue;
                }

                mTextTypeList[index].SafeSetText(tempStr);
                mTypeList[index].CustomActive(true);
            }

            for(; index < mTextTypeList.Count; ++index)
            {
                mTypeList[index].CustomActive(false);
            }
        }
    }
    //获取类型文本
    string getSkillTypeText(byte effectIndex)
        {
            string resultStr = "";
            switch(effectIndex)
            {
                case (byte)ProtoTable.SkillTable.eSkillEffect.START_SKILL:
                    resultStr = TR.Value("skill_start");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.CONTINUOUS_SKILL:
                    resultStr = TR.Value("skill_continuous");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.HURT_SKILL:
                    resultStr = TR.Value("skill_hurt");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.DISPLACEMENT_SKILL:
                    resultStr = TR.Value("displacement_skilll");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.CONTROL_SKILL:
                    resultStr = TR.Value("control_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.GRAB_SKILL:
                    resultStr = TR.Value("grab_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.DEFENSE_SKILL:
                    resultStr = TR.Value("defense_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.ASSISTANT_SKILL:
                    resultStr = TR.Value("assistant_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.PHYSICAL_SKILL:
                    resultStr = TR.Value("physical_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.MAGIC_SKILL:
                    resultStr = TR.Value("magic_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.NEAR_SKILL:
                    resultStr = TR.Value("near_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.FAR_SKILL:
                    resultStr = TR.Value("far_skill");
                    break;
            }
            return resultStr;
        }

    //技能类型
    private void _SetSkillType()
    {
        if (!mToggleDesp.isOn)
            return;
        if (null != mSkillTable)
        {
            mTextSkillType.SafeSetText(SkillDataManager.GetInstance().GetSkillType(mSkillTable));
        }
    }

    //技能等级上限
    private void _SetSkillLvLimit()
    {
        if (!mToggleDesp.isOn)
            return;
        mTextSkillLvLimit.SafeSetText(string.Format("Lv.{0}", mSkillTable.TopLevelLimit));
    }

    //技能cd
    private void _SetSkillCd()
    {
        if (!mToggleDesp.isOn)
            return;
        var lv = mCurLv + mAddLv;
        if (0 == lv)
            lv = 1;
        var times = SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP ? mSkillTable.RefreshTimePVP : mSkillTable.RefreshTime;
        float time = TableManager.GetValueFromUnionCell(times, lv) / 1000f;
        mTextSkillCd.SafeSetText(TR.Value("skill_info_cd_time", time));
    }

    //魔耗
    private void _SetMpCost()
    {
        if (!mToggleDesp.isOn)
            return;
        var lv = mCurLv +mAddLv;
        if (0 == lv)
            lv = 1;
        mTextMpCost.SafeSetText(((float)TableManager.GetValueFromUnionCell(mSkillTable.MPCost, lv, false)).ToString());
    }

    //技能描述
    private void _SetDesp()
    {
        if (!mToggleDesp.isOn)
            return;
        var desp = SkillDataManager.GetInstance().GetSkillDescription(mSkillTable);
        var height = StaticUtility.GetTextPreferredHeight(mTextSkillDesp, desp);
        mTextSkillDesp.text = desp;
        var size = mTextSkillDesp.rectTransform.sizeDelta;
        size.y = height + 10f;
        mTextSkillDesp.rectTransform.sizeDelta = size;
    }

    //属性说明列表
    private void _SetAttrList()
    {
        if (!mToggleAttr.isOn)
            return;
        var label = SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE ? SkillFrameTabType.PVE : SkillFrameTabType.PVP;
        var curlist = SkillDataManager.GetInstance().GetSkillDesList(mCurSkillId, (byte)(mCurLv + mAddLv), label);
        var nameList = new List<string>();
        var curList = new List<string>();
        var nextList = new List<string>();
        foreach (var str in curlist)
        {
            var strArr = str.Split(':');
            if (strArr.Length > 0)
                nameList.Add(strArr[0]);
            if (strArr.Length > 1 && mCurLv + mAddLv > 0)
                curList.Add(strArr[1]);
            else
                curList.Add("0");
        }
        if (mCurLv + mAddLv + 1 > (byte)mSkillTable.TopLevel)
        {
            foreach (var str in curlist)
                nextList.Add("MAX");
        }
        else
        {
            var nextlist = SkillDataManager.GetInstance().GetSkillDesList(mSkillTable.ID, (byte)(mCurLv + mAddLv + 1), label);
            foreach (var str in nextlist)
            {
                var strArr = str.Split(':');
                if (strArr.Length > 1)
                    nextList.Add(strArr[1]);
                else
                    nextList.Add("0");
            }
        }
        int index = 0;
        for (; index < nameList.Count; ++index)
        {
            if (index >= mAttrItemList.Count)
                break;
            mAttrItemList[index].CustomActive(true);
            mAttrItemList[index].OnInit(index, nameList[index], curList[index], nextList[index]);
        }
        for (; index < mAttrItemList.Count; ++index)
        {
            mAttrItemList[index].CustomActive(false);
        }
    }

    //需要等级与技能点
    private void _SetNeedLevelPoint()
    {
        //满级隐藏
        if (mCurLv >= mSkillTable.TopLevelLimit)
        {
            mTextNeedSp.CustomActive(false);
            mTextNeedLv.CustomActive(false);
        }
        else
        {
            mTextNeedSp.CustomActive(true);
            mTextNeedLv.CustomActive(true && SkillFrame.frameParam.frameType != SkillFrameType.FairDuel);
            {
                mTextNeedSp.SafeSetText(mSkillTable.LearnSPCost.ToString());
                var comColor = mTextNeedSp.GetComponent<ComSetTextColor>();
                if (null != comColor)
                {
                    if (SkillDataManager.GetInstance().GetCurSp() < mSkillTable.LearnSPCost)
                    {
                        //技能点不足
                        comColor.SetColor(0);
                    }
                    else
                    {
                        //技能点充足
                        comColor.SetColor(1);
                    }
                }
            }
            if (PlayerBaseData.GetInstance().Level < mSkillTable.LevelLimit && mSkillTable.IsPreJob == 0)
            {
                mTextNeedLv.text = string.Format(TR.Value("skill_open_level_need_tip"), mSkillTable.LevelLimit);
                var comColor = mTextNeedLv.GetComponent<ComSetTextColor>();
                if (null != comColor)
                {
                    //学习技能等级不足
                    comColor.SetColor(2);
                }
            }
            else
            {
                int tempLevel = SkillDataManager.GetInstance().GetSkillNextOpenNeedRoleLv(mSkillTable, mCurLv);
                mTextNeedLv.text = string.Format(TR.Value("skill_up_level_need_tip"), tempLevel);
                var comColor = mTextNeedLv.GetComponent<ComSetTextColor>();
                if (null != comColor)
                {
                    if (tempLevel > PlayerBaseData.GetInstance().Level)
                    {
                        //已经学会的技能 升级所需等级不足
                        comColor.SetColor(0);
                    }
                    else
                    {
                        //已经学会的技能 升级所需等级满足
                        comColor.SetColor(1);
                    }
                }
            }
        }
    }

    //技能满级描述和满级标记
    private void _SetSkillMaxLvDesp()
    {
        mImgSkillMaxMask.CustomActiveAlpha(mCurLv >= mSkillTable.TopLevelLimit);
        if (mCurLv >= mSkillTable.TopLevelLimit)
            mTextSkillMaxLvDesp.SafeSetText(TR.Value("skill_max_des", mSkillTable.TopLevelLimit, mSkillTable.TopLevel));
        else
            mTextSkillMaxLvDesp.SafeSetText("");
    }


    //降级
    public void OnClickLvDown()
    {
        if (!SkillDataManager.GetInstance().CheckLvDown(mCurLv, mSkillTable, true))
        {
            return;
        }
        //有天赋的技能降到0级会重置
        if (1 == mCurLv && SkillDataManager.GetInstance().IsSkillHaveTalent(mCurSkillId))
        {
            SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("skill_skill_down_level_to_zero_tip"), null, OnClickLvUp);
        }
        _SendSkillLevelChanged(false);
    }    
    //升级
    public void OnClickLvUp()
    {
        if (!SkillDataManager.GetInstance().CheckLvUp(mCurLv, mSkillTable, true))
        {
            return;
        }
        _SendSkillLevelChanged(true);
    }
    //发送升降级请求
    private void _SendSkillLevelChanged(bool isUp)
    {
        if (mCurSkillId <= 0)
        {
            return;
        }

        if (SkillFrame.frameParam == null)
        {
            Logger.LogError("SkillFrame.frameParam == null");
            return;
        }

        SceneChangeSkillsReq req = new SceneChangeSkillsReq();

        req.skills = new ChangeSkill[1];
        req.skills[0] = new ChangeSkill();

        if (isUp)
        {
            req.skills[0].dif = 1;
        }
        else
        {
            req.skills[0].dif = 255;
        }

        req.skills[0].id = (UInt16)mCurSkillId;

        if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
        {
            req.configType = (byte)SkillConfigType.SKILL_CONFIG_EQUAL_PVP;
        }
        else
        {
            if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
            {
                req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVE;
            }
            else
            {
                req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVP;
            }
        }

        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
    }
    
    //打开技能天赋界面
    public void OnClickOpenTalentFrame()
    {
        if (0 == mCurLv)
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("skill_talent_open_err"));
            return;
        }
        ClientSystemManager.GetInstance().OpenFrame<SkillTalentFrame>(FrameLayer.Middle, mCurSkillId);
    }

    void Update()
    {
        mTimer += Time.deltaTime;

        if(mTimer > DelayInitTime && !bDelayInit)
        {
            // 延迟初始化
            _UpdateInfo();
            bDelayInit = true;
        }
        if (mTimer > mDelayPreviewTime && !mIsinitPreview)
        {
            mIsinitPreview = true;
            _InitSkillPreview();
        }
        if (mIsinitPreview && mIsPreviewSkill)
            BattlePreview.instance.Update(Time.deltaTime);
    }
}