using System;
using System.Collections.Generic;
using System.Collections;
///////删除linq
using System.Text;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;


namespace GameClient
{
    public struct UpLevelSkillData
    {
        public string name;
        public string iconpath;
    }

    class LevelUpNotify : ClientFrame
    {
        const int iShowSkillNum = 5;
        const int iNewFuncNum = 4;

        ushort currentLevel = 0;

        List<UpLevelSkillData> NewSkillList = new List<UpLevelSkillData>();
        private bool haveNewSkill = false;
        private bool haveNewFunc = false;

        protected override void _OnOpenFrame()
        {
            currentLevel = PlayerBaseData.GetInstance().Level;
            InitPanel();

            var destroyDelay = frame.GetComponent<DestroyDelay>();
            StartCoroutine(closeDelay(destroyDelay.Delay));
            destroyDelay.enabled = false;
            if (PlayerBaseData.GetInstance().NewUnlockFuncList.Count > 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPlayerFunctionUnlockAnimation);
            }
        }

        IEnumerator closeDelay(float time)
        {
            yield return new  WaitForSeconds(time);
            Close();
            yield break;
        }

        protected override void _OnCloseFrame()
        {
            ClearData();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UplevelFrameClose);
            if (PlayerBaseData.GetInstance().bNeedShowAwakeFrame)
            {
                ClientSystemManager.GetInstance().OpenFrame<AwakeFrame>(FrameLayer.Middle);
            }

            for (int i = 0; i < iShowSkillNum; i++)
            {
                if (NewSkillRoot[i] != null)
                {
                    var ani = NewSkillRoot[i].GetComponent<DOTweenAnimation>();
                    if (ani != null)
                    {
                        ani.onComplete.RemoveAllListeners();
                    }
                }
            }
        }

        void ClearData()
        {
            currentLevel = 0;
            NewSkillList.Clear();
            haveNewSkill = false;
            haveNewFunc = false;
            // CanUpLvSkillList.Clear();
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/UpLevelNotifyNew";
        }

//         struct Attr
//         {
//             public Text OldValue;
//             public Text NewValue;
//         }

        void InitPanel()
        {
//             StringBuilder builder = StringBuilderCache.Acquire();
// 
//             if (mAttr != null)
//             {
//                 for (int i = 0; i < 4; ++i)
//                 {
//                     builder.Clear();
//                     builder.AppendFormat("Panel/Attr{0}/OldValue", i);
//                     attr[i].OldValue = Utility.FindComponent<Text>(mAttr, builder.ToString());
//                     builder.Clear();
//                     builder.AppendFormat("Panel/Attr{0}/NewValue", i);
//                     attr[i].NewValue = Utility.FindComponent<Text>(mAttr, builder.ToString());
//                 }
//             }

            //InitAttr();
            InitSkillPoint();
            InitLearnSkill();
            InitFuncUnLock();
            
            mTopLevel.SafeSetText(currentLevel.ToString());
            string curLevel = string.Format("<color=#FFD800>{0}</color>", currentLevel);
            mUpToText.SafeSetText(string.Format(TR.Value("player_level_upto"),curLevel));
            
            var maxLevel = 60;
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
            if(null != systemValue)
            {
                maxLevel = systemValue.Value;
            }
            
            if (currentLevel == maxLevel)
            {
                mGXSJ.SafeSetText(TR.Value("congratulations_lv_up_max"));
            }
            else
            {
                mGXSJ.SafeSetText(TR.Value("congratulations_lv_up"));
            }
            

            if (!haveNewSkill && !haveNewFunc)
            {
                mBottomBg.CustomActive(false);
            }
        }

        void InitAttr()
        {
//             int occuID = PlayerBaseData.GetInstance().JobTableID;
// 
//             var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occuID);
//             if (jobData == null)
//             {
//                 Logger.LogErrorFormat("找不到职业ID为{0}的职业", occuID);
//                 return;
//             }
// 
//             var fightData = TableManager.GetInstance().GetTableItem<ProtoTable.FightPackageTable>(jobData.FightID);
// 
//             int CurrentValue;
//             int OldValue;
// 
//             CurrentValue = (fightData.Power + (currentLevel - 1) * fightData.PowerLevel) / 1000;
//             OldValue = CurrentValue - fightData.PowerLevel / 1000;
//             if (OldValue < 0) OldValue = 0;
// 
//             attr[0].OldValue.text = OldValue.ToString();
//             attr[0].NewValue.text = CurrentValue.ToString();
// 
//             CurrentValue = (fightData.Intellect + (currentLevel - 1) * fightData.IntellectLevel) / 1000;
//             OldValue = CurrentValue - fightData.IntellectLevel / 1000;
//             if (OldValue < 0) OldValue = 0;
// 
//             attr[1].OldValue.text = OldValue.ToString();
//             attr[1].NewValue.text = CurrentValue.ToString();
// 
//             CurrentValue = (fightData.Streangth + (currentLevel - 1) * fightData.StrengthLevel) / 1000;
//             OldValue = CurrentValue - fightData.StrengthLevel / 1000;
//             if (OldValue < 0) OldValue = 0;
// 
//             attr[2].OldValue.text = OldValue.ToString();
//             attr[2].NewValue.text = CurrentValue.ToString();
// 
//             CurrentValue = (fightData.Spirit + (currentLevel - 1) * fightData.SpiritLevel) / 1000;
//             OldValue = CurrentValue - fightData.SpiritLevel / 1000;
//             if (OldValue < 0) OldValue = 0;
// 
//             attr[3].OldValue.text = OldValue.ToString();
//             attr[3].NewValue.text = CurrentValue.ToString();
        }

        void InitSkillPoint()
        {
             var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.ExpTable>(currentLevel);
             if (jobData != null)
             {
                 mSpCount.SafeSetText(string.Format("获得技能点{0}",jobData.Sp));
             }
        }

        void InitLearnSkill()
        {
            Dictionary<int, int> skilllist = TableManager.GetInstance().GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);
            var emu = skilllist.GetEnumerator();

            bool HasNewSkill = false;

            while(emu.MoveNext())
            {
                var skillTable = TableManager.GetInstance().GetTableItem<SkillTable>(emu.Current.Key);

                if (skillTable != null)
                {
                    // 筛选掉普攻
                    if(skillTable.SkillCategory == 1)
                    {
                        continue;
                    }

                    if (skillTable.LevelLimit == currentLevel)
                    {
                        var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);

                        if (jobTable == null)
                        {
                            continue;
                        }

                        if (currentLevel > 10 && jobTable.JobType == 0)
                        {
                            continue;
                        }
                        UpLevelSkillData data = new UpLevelSkillData();
                        data.name = skillTable.Name;
                        data.iconpath = skillTable.Icon;

                        NewSkillList.Add(data);
                    }
                }
            }

            if(NewSkillList.Count <= 0)
            {
                mNewSkill.CustomActive(false);
                return;
                // mSkillTitle.gameObject.SetActive(false);
                // mSkillTitleBack.gameObject.SetActive(false);
            }
            else
            {
                mNewSkill.CustomActive(true);
                haveNewSkill = true;
            }

            for(int i = 0; i < NewSkillList.Count && i < iShowSkillNum; i++)
            {
                AddLearnSkill(NewSkillList[i].name, NewSkillList[i].iconpath, i);
                lvupImg[i].gameObject.SetActive(false);
                AddSkillEffectListener(i);
            }

            int iFinishIndex = NewSkillList.Count;

            if (currentLevel == 45)
            {
                mSkillTitle.text = "开启觉醒任务";
            }

            for (int i = iFinishIndex; i < iShowSkillNum; i++)
            {
                NewSkillRoot[i].gameObject.SetActive(false);
                var ani = NewSkillRoot[i].GetComponent<DOTweenAnimation>();
                if (ani!=null)
                {
                    ani.onComplete.RemoveAllListeners();
                }
            }
        }


        void AddSkillEffectListener(int i)
        {
            if (i < 0 || i >= skillEffects.Length)
            {
                return;
            }
            var skillRoot = NewSkillRoot[i];
            if (skillRoot != null)
            {
                var ani = skillRoot.GetComponent<DOTweenAnimation>();
                if (ani != null)
                {
                    if (ani.onComplete != null)
                    {
                        ani.onComplete.RemoveAllListeners();
                        ani.onComplete.AddListener(() => { skillEffects[i].CustomActive(true);});   
                    }
                }
            }
        }
        
        void AddFuncEffectListener(int i)
        {
            if (i < 0 || i >= funcEffects.Length)
            {
                return;
            }
            var funcRoot = NewFuncRoot[i];
            if (funcRoot != null)
            {
                var ani = funcRoot.GetComponent<DOTweenAnimation>();
                if (ani != null)
                {
                    if (ani.onComplete != null)
                    {
                        ani.onComplete.RemoveAllListeners();
                        ani.onComplete.AddListener(() => { funcEffects[i].CustomActive(true);});   
                    }
                }
            }
        }

        void AddLearnSkill(string name, string iconpath, int iIndex)
        {
            if(iIndex < 0 || iIndex >= NewSkillImg.Length)
            {
                return;
            }

            //var pic = AssetLoader.instance.LoadRes(iconpath, typeof(Sprite), false);

            //NewSkillImg[iIndex].sprite = pic.obj as Sprite;
            ETCImageLoader.LoadSprite(ref NewSkillImg[iIndex], iconpath);
            NewSkillText[iIndex].text = name;

//             GameObject obj = mNewSkillItem;
//             var newObj = GameObject.Instantiate(obj, obj.transform.parent) as GameObject;
//             newObj.GetComponent<Image>().sprite = pic.obj as Sprite;
//             newObj.transform.GetChild(0).GetComponent<Text>().text = name;
//             newObj.CustomActive(true);
        }

        void InitFuncUnLock()
        {
            var funcUnLock = TableManager.GetInstance().GetTable<FunctionUnLock>();
            var emu = funcUnLock.GetEnumerator();

            int iIndex = 0;
            while (emu.MoveNext())
            {
                FunctionUnLock curItem = emu.Current.Value as FunctionUnLock;

                if (curItem != null && (curItem.ShowFunctionOpen == 1))
                {
                    if (curItem.FinishLevel == currentLevel)
                    {
                        //如果是帐号绑定的功能 并且不在解锁动画播放列表中 则不展示这个icon了
                        if (curItem.FuncType == FunctionUnLock.eFuncType.AdventureTeam &&
                            curItem.BindType == FunctionUnLock.eBindType.BT_AccBind &&
                            PlayerBaseData.GetInstance().NewUnlockFuncList.Contains(curItem.ID) == false)
                        {
                            continue;
                        }
                        if (curItem.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason &&
                            curItem.BindType == FunctionUnLock.eBindType.BT_AccBind &&
                            PlayerBaseData.GetInstance().NewUnlockFuncList.Contains(curItem.ID) == false)
                        {
                            continue;
                        }                        
#if APPLE_STORE					
                        //add by mjx for IOS appstore
                        if (CheckIsSevenAwardClose(curItem))
                        {
                            continue;
                        }
#endif
                        AddUnLockFunction(curItem.Name, curItem.IconPath, iIndex);
                        iIndex++;
                    }
                }
            }

            if (iIndex == 0)
            {
                mFunctionUnlockRoot.CustomActive(false);
                // mFuncTitle.CustomActive(false);
                // mFuncTitleBack.CustomActive(false);
            }
            else
            {
                mFunctionUnlockRoot.CustomActive(true);
                haveNewFunc = true;
            }

            for (int i = iIndex; i < iNewFuncNum; i++)
            {
                NewFuncRoot[i].gameObject.SetActive(false);
            }
        }

        void AddUnLockFunction(string name, string iconpath, int iIndex)
        {
            if (iIndex < 0 || iIndex >= NewFunctionImg.Length)
            {
                return;
            }

            //var pic = AssetLoader.instance.LoadRes(iconpath, typeof(Sprite), false);

            //NewFunctionImg[iIndex].sprite = pic.obj as Sprite;
            ETCImageLoader.LoadSprite(ref NewFunctionImg[iIndex], iconpath);
            NewFunctionText[iIndex].text = name;
            AddFuncEffectListener(iIndex);
//             GameObject obj = mUnLockItem;
//             var newObj = GameObject.Instantiate(obj, obj.transform.parent) as GameObject;
//             newObj.GetComponent<Image>().sprite = pic.obj as Sprite;
//             newObj.transform.GetChild(0).GetComponent<Text>().text = name;
//             newObj.CustomActive(true);
        }

        bool CheckIsSevenAwardClose(FunctionUnLock curItem)
        {
            if (curItem == null)
                return false;


            //for ios appstore !
            int sevenAward = 32;
            if (curItem.ID == sevenAward)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SEVEN_AWARDS))
                {
                    return true;
                }
            }
            return false;
        }

        [UIControl("Back/NewSkill/Panel/Image{0}", typeof(RectTransform), 1)]
        RectTransform[] NewSkillRoot = new RectTransform[iShowSkillNum];

        [UIControl("Back/FunctionUnlock/Panel/Image{0}", typeof(RectTransform), 1)]
        RectTransform[] NewFuncRoot = new RectTransform[iNewFuncNum];

        [UIControl("Back/NewSkill/Panel/Image{0}/Image", typeof(Image), 1)]
        Image[] NewSkillImg = new Image[iShowSkillNum];

        [UIControl("Back/NewSkill/Panel/Image{0}/Text", typeof(Text), 1)]
        Text[] NewSkillText = new Text[iShowSkillNum];

        [UIControl("Back/FunctionUnlock/Panel/Image{0}/Icon", typeof(Image), 1)]
        Image[] NewFunctionImg = new Image[iNewFuncNum];

        [UIControl("Back/FunctionUnlock/Panel/Image{0}/Text", typeof(Text), 1)]
        Text[] NewFunctionText = new Text[iNewFuncNum];

        [UIControl("Back/NewSkill/Panel/Image{0}/lvup", typeof(Image), 1)]
        Image[] lvupImg = new Image[iShowSkillNum];
        
        [UIControl("Back/NewSkill/Panel/Image{0}/Skill_UI_Shengji_icon", typeof(Transform), 1)]
        Transform[] skillEffects = new Transform[iShowSkillNum];
        
        [UIControl("Back/FunctionUnlock/Panel/Image{0}/Skill_UI_Shengji_icon02", typeof(Transform), 1)]
        Transform[] funcEffects = new Transform[iNewFuncNum];

        #region ExtraUIBind
        private Text mSkillTitle = null;
        private Text mFuncTitle = null;
        private RectTransform mSkillTitleBack = null;
        private RectTransform mFuncTitleBack = null;
        private RectTransform mNewSkill = null;
        private RectTransform mSpRoot = null;
        private TextEx mSpCount = null;
        private TextEx mTopLevel = null;
        private TextEx mUpToText = null;
        private RectTransform mBottomBg = null;
        private TextEx mGXSJ = null;
        private RectTransform mFunctionUnlockRoot = null;


        protected override void _bindExUI()
        {
            mSkillTitle = mBind.GetCom<Text>("SkillTitle");
            mFuncTitle = mBind.GetCom<Text>("FuncTitle");
            mSkillTitleBack = mBind.GetCom<RectTransform>("SkillTitleBack");
            mFuncTitleBack = mBind.GetCom<RectTransform>("FuncTitleBack");
            mNewSkill = mBind.GetCom<RectTransform>("NewSkill");
            mSpRoot = mBind.GetCom<RectTransform>("SpRoot");
            mSpCount = mBind.GetCom<TextEx>("spCount");
            mTopLevel = mBind.GetCom<TextEx>("TopLevel");
            mUpToText = mBind.GetCom<TextEx>("UpToText");
            mBottomBg = mBind.GetCom<RectTransform>("BottomBg");
            mGXSJ = mBind.GetCom<TextEx>("GXSJ");
            mFunctionUnlockRoot = mBind.GetCom<RectTransform>("FunctionUnlockRoot");
        }

        protected override void _unbindExUI()
        {
            mSkillTitle = null;
            mFuncTitle = null;
            mSkillTitleBack = null;
            mFuncTitleBack = null;
            mNewSkill = null;
            mSpRoot = null;
            mSpCount = null;
            mTopLevel = null;
            mUpToText = null;
            mBottomBg = null;
            mGXSJ = null;
            mFunctionUnlockRoot = null;
        }
        #endregion
    }
}
