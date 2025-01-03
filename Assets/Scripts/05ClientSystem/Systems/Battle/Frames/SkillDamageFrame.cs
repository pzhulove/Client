using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class SkillDamageFrame : ClientFrame
    {
        #region ExtraUIBind
        private RectTransform mContent = null;
        private GameObject mItem = null;
        private Text mText_CurentPage = null;
        private Button mButton_Left = null;
        private Button mButton_Right = null;
        private Button mButton_Close = null;
        private Button mButton_Reset = null;
        private Text mText_TotalDamage = null;
        private Button mButton_Back = null;
        private ScrollRect mScrollRect = null;
        private RectTransform mScrollRectTrans = null;
        private Button mHelpBtn = null;
        private ComDropDownControl mComDropDownControl = null;

        protected override void _bindExUI()
        {
            mContent = mBind.GetCom<RectTransform>("Content");
            mItem = mBind.GetGameObject("Item");
            mText_CurentPage = mBind.GetCom<Text>("Text_CurentPage");
            mButton_Left = mBind.GetCom<Button>("Button_Left");
            if (null != mButton_Left)
            {
                mButton_Left.onClick.AddListener(_onButton_LeftButtonClick);
            }
            mButton_Right = mBind.GetCom<Button>("Button_Right");
            if (null != mButton_Right)
            {
                mButton_Right.onClick.AddListener(_onButton_RightButtonClick);
            }
            mButton_Close = mBind.GetCom<Button>("Button_Close");
            if (null != mButton_Close)
            {
                mButton_Close.onClick.AddListener(_onButton_CloseButtonClick);
            }
            mButton_Reset = mBind.GetCom<Button>("Button_Reset");
            if (null != mButton_Reset)
            {
                mButton_Reset.onClick.AddListener(_onButton_ResetButtonClick);
            }
            mText_TotalDamage = mBind.GetCom<Text>("Text_TotalDamage");
            mButton_Back = mBind.GetCom<Button>("Button_Back");
            if (null != mButton_Back)
            {
                mButton_Back.onClick.AddListener(_onButton_BackButtonClick);
            }
            mScrollRect = mBind.GetCom<ScrollRect>("ScrollRect");
            mScrollRectTrans = mBind.GetCom<RectTransform>("ScrollRectTrans");
            mHelpBtn = mBind.GetCom<Button>("HelpBtn");
            if (null != mHelpBtn)
            {
                mHelpBtn.onClick.AddListener(_onHelpBtnButtonClick);
            }

            mComDropDownControl = mBind.GetCom<ComDropDownControl>("ComDropDownControl");
        }

        protected override void _unbindExUI()
        {
            if (null != mComDropDownControl)
            {
                mComDropDownControl = null;   
            }
            
            mContent = null;
            mItem = null;
            mText_CurentPage = null;
            if (null != mButton_Left)
            {
                mButton_Left.onClick.RemoveListener(_onButton_LeftButtonClick);
            }
            mButton_Left = null;
            if (null != mButton_Right)
            {
                mButton_Right.onClick.RemoveListener(_onButton_RightButtonClick);
            }
            mButton_Right = null;
            if (null != mButton_Close)
            {
                mButton_Close.onClick.RemoveListener(_onButton_CloseButtonClick);
            }
            mButton_Close = null;
            if (null != mButton_Reset)
            {
                mButton_Reset.onClick.RemoveListener(_onButton_ResetButtonClick);
            }
            mButton_Reset = null;
            mText_TotalDamage = null;
            if (null != mButton_Back)
            {
                mButton_Back.onClick.RemoveListener(_onButton_BackButtonClick);
            }
            mButton_Back = null;
            mScrollRect = null;
            mScrollRectTrans = null;
            if (null != mHelpBtn)
            {
                mHelpBtn.onClick.RemoveListener(_onHelpBtnButtonClick);
            }
            mHelpBtn = null;
        }
        #endregion

        #region Callback
        private void _onDropdown_SelectDropdownValueChange(int index)
        {
            RefershSelect(index);
        }
        private void _onButton_LeftButtonClick()
        {
            LeftPageBtn();
        }
        private void _onButton_RightButtonClick()
        {
            RightPageBtn();
        }
        private void _onButton_CloseButtonClick()
        {
            Close();
        }

        private void _onButton_ResetButtonClick()
        {
            Reset();
        }

        private void _onButton_BackButtonClick()
        {
            Close();
        }

        private void _onHelpBtnButtonClick()
        {
            /* put your code in here */

        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/SkillDamageFrame";
        }

        public struct SkillDamage
        {
            public long Damage;
            public string IconPath;
            public string Name;
            public int UseCount;
            public string Percent;
        }

        private List<SkillDamageData> damageDataList = new List<SkillDamageData>();

        private int pageIndex = -1;
        private int selectMonsterId = 0;
        private bool isTrainingPveFlag = false;
        
        private List<ComControlData> dropDownDataList;


        //显示上一页
        private void LeftPageBtn()
        {
            if (damageDataList == null || damageDataList.Count <= 1)
                return;
            if (pageIndex <= 0)
                return;
            pageIndex -= 1;
            InitDropDownData();
        }

        //显示下一页
        private void RightPageBtn()
        {
            if (damageDataList == null || damageDataList.Count <= 1)
                return;
            if (pageIndex >= damageDataList.Count - 1)
                return;
            pageIndex += 1;
            InitDropDownData();
        }

        //选择其他怪物
        private void RefershSelect(int selectIndex)
        {
            if (damageDataList == null || damageDataList.Count <= 0)
                return;
            SkillDamageData damageData = damageDataList[pageIndex];
            selectMonsterId = damageData.monsterIdList[selectIndex];
            RefreshAllData();
        }

        //初始化数据
        public void InitData(bool isTrainingPve = false)
        {
            BeActor actor = GetMainPlayer();
            damageDataList.Clear();
            isTrainingPveFlag = isTrainingPve;
            if (isTrainingPveFlag)
            {
                if (actor != null
                    && actor.skillDamageManager != null
                    && actor.skillDamageManager.skillDamageData.skillDamageDic != null
                    && actor.skillDamageManager.skillDamageData.skillDamageDic.Count > 0)
                {
                    SkillDamageData damageData = actor.skillDamageManager.skillDamageData;
                    damageData.dungeonName = "练习场";
                    damageDataList.Add(damageData);

                    actor.skillDamageManager.SetTimingFlag(false);
                }
            }

            if (actor != null && actor.skillDamageManager != null)
            {
                List<SkillDamageData> oldData = actor.skillDamageManager.GetSkillDamageData();
                if (oldData != null)
                {
                    for (int i = 0; i < oldData.Count; i++)
                    {
                        if (damageDataList.Count >= 3)
                            continue;
                        damageDataList.Add(oldData[i]);
                    }
                }
            }

            if (damageDataList == null || damageDataList.Count == 0)
                return;
            pageIndex = 0;

            InitDropDownData();
        }

        void InitDropDownData()
        {
            mScrollRectTrans.anchoredPosition = Vector2.zero;
            mScrollRect.StopMovement();
            if (mComDropDownControl != null)
            {
                mComDropDownControl.ClearAndRefresh();
            }
            if (damageDataList == null || damageDataList.Count <= 0)
                return;
            if (damageDataList[pageIndex].monsterNameList == null)
                return;
            List<string> monsterNameList = damageDataList[pageIndex].monsterNameList;
            if (monsterNameList == null || monsterNameList.Count <= 0)
                return;

            dropDownDataList = new List<ComControlData>();
            for (int i = 0; i < monsterNameList.Count; i++)
            {
                ComControlData data = new ComControlData();
                data.Id = i;
                data.Index = i;
                data.Name = monsterNameList[i];
                data.IsSelected = false;
                
                dropDownDataList.Add(data);
            }
            
            selectMonsterId = damageDataList[pageIndex].monsterIdList[0];
            RefershSelect(0);
            if (mComDropDownControl != null)
            {
                mComDropDownControl.InitComDropDownControl(dropDownDataList[0],dropDownDataList,OnTypeDropDownItemClick);
            }
        }
        
        private void OnTypeDropDownItemClick(ComControlData data)
        {
            if (null == data || data.Index < 0)
            {
                return;
            }
            
            RefershSelect(data.Index);
        }
        

        //刷新列表数据
        private void RefreshAllData()
        {
            HideAll();
            RefreshPageBtn();
            RefreshDungeonName();
            RefreshUIList();
        }

        //隐藏列表所有的子项
        private void HideAll()
        {
            if (mContent == null)
                return;
            int childCount = mContent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                mContent.GetChild(i).gameObject.CustomActive(false);
            }
        }

        //刷新UIlist数据
        private void RefreshUIList()
        {
            if (mContent == null)
                return;
            List<SkillDamage> skillDamageData = GetSkillDamageData();
            if (skillDamageData == null)
                return;
            int childCount = mContent.childCount;
            int num = 0;
            for (int i = 0; i < skillDamageData.Count; i++)
            {
                ComCommonBind bind = null;
                if (num + 1 < childCount)
                    bind = mContent.GetChild(num + 1).GetComponent<ComCommonBind>();
                else
                    bind = GameObject.Instantiate(mItem).GetComponent<ComCommonBind>();
                Utility.AttachTo(bind.gameObject, mContent.gameObject);

                string iconPath = skillDamageData[i].IconPath;
                string name = skillDamageData[i].Name;
                string useCount = skillDamageData[i].UseCount.ToString();
                string damage = skillDamageData[i].Damage.ToString();
                string percent = skillDamageData[i].Percent;

                SetSkillIconImage(bind.GetCom<Image>("Image"), iconPath);
                SetTextData(bind.GetCom<Text>("skillName"), name);
                SetTextData(bind.GetCom<Text>("useCount"), useCount);
                SetTextData(bind.GetCom<Text>("damage"), damage);
                SetTextData(bind.GetCom<Text>("damagePercent"), percent);
                bind.GetCom<Image>("Bg1").CustomActive(false);
                bind.GetCom<Image>("Bg2").CustomActive(false);
                if ((i + 1) % 2 != 0)
                    bind.GetCom<Image>("Bg1").CustomActive(true);
                else
                    bind.GetCom<Image>("Bg2").CustomActive(true);
                Transform shareBtn = bind.GetCom<Button>("Button_Option").transform;
                bind.GetCom<Button>("Button_Option").onClick.AddListener(() =>
                {
                    OnShareBtnClick(name, useCount, damage, percent, shareBtn.transform.position);
                });
                bind.CustomActive(true);
                num++;
            }
        }

        //刷型页码
        private void RefreshDungeonName()
        {
            if (damageDataList == null || damageDataList.Count <= 0)
                return;
            string dungeonName = damageDataList[pageIndex].dungeonName;
            if (dungeonName == null)
                mText_CurentPage.text = null;
            else
                mText_CurentPage.text = "副本:" + dungeonName;
            SkillDamageData data = damageDataList[pageIndex];
			BeActor actor = GetMainPlayer();
            if (data.skillDamageDic == null || data.skillDamageDic.Count <= 0)
                mText_TotalDamage = null;
            else
            {
                long totalDamage = actor.skillDamageManager.GetTotalDamage(selectMonsterId, data);
                double totalTime = actor.skillDamageManager.GetMonsterTime(selectMonsterId, data);
                if (totalTime == 0)
                {
                    mText_TotalDamage.text = "   总伤害数值:" + totalDamage;
                }
                else
                {
                    totalTime = (int)(totalDamage / (totalTime / 1000));
                    mText_TotalDamage.text = "每秒伤害数值：" + totalTime + "   总伤害数值:" + totalDamage;
                }
            }
        }

        //刷新翻页按钮
        private void RefreshPageBtn()
        {
            mButton_Left.CustomActive(false);
            mButton_Right.CustomActive(false);

            if (damageDataList == null || damageDataList.Count == 0)
                return;

            if (pageIndex != 0)
                mButton_Left.CustomActive(true);

            if (pageIndex != damageDataList.Count - 1)
                mButton_Right.CustomActive(true);
        }

        //重置所有
        private void Reset()
        {
            //重置修炼场当前数据
            BeActor actor = GetMainPlayer();
            if (isTrainingPveFlag
                && actor != null
                && actor.skillDamageManager != null
                && actor.skillDamageManager.skillDamageData.skillDamageDic != null
                )
            {
                actor.skillDamageManager.skillDamageData = new SkillDamageData();
            }
            damageDataList.Clear();

            if (actor != null && actor.skillDamageManager != null)
            {
                actor.skillDamageManager.ClearSkillDamageData();
            }

            RefreshAllData();
            InitDropDownData();
            mText_CurentPage.text = "";
            mText_TotalDamage.text = "";
        }

        //设置技能图标
        private void SetSkillIconImage(Image image, string path)
        {
            if (image == null)
                return;
            ETCImageLoader.LoadSprite(ref image, path);
        }

        //设置文本数据
        private void SetTextData(Text text, string str)
        {
            if (text == null)
                return;
            text.text = str;
        }

        //点击分享按钮
        private void OnShareBtnClick(string skillName, string useCount, string totalDamage, string damagePercent, Vector3 pos)
        {
            string msg = string.Format("技能名称:{0},技能使用次数:{1},造成总伤害:{2},伤害占比:{3}", skillName, useCount, totalDamage, damagePercent);
            ShareToChatFrame sharFame = ClientSystemManager.instance.OpenFrame<ShareToChatFrame>(FrameLayer.Middle) as ShareToChatFrame;
            if (sharFame != null)
            {
                sharFame.InitData(pos, msg);
            }
        }

        //获取技能伤害数据并且排序
        private List<SkillDamage> GetSkillDamageData()
        {
            if (damageDataList == null || damageDataList.Count <= 0)
                return null;
            SkillDamageData data = damageDataList[pageIndex];
            if (data.origionSkillIdList == null || data.origionSkillIdList.Count <= 0)
                return null;
            BeActor actor = GetMainPlayer();
            List<SkillDamage> newData = new List<SkillDamage>();
            for (int i = 0; i < data.origionSkillIdList.Count; i++)
            {
                SkillDamage damage = new SkillDamage();
                int skillId = data.origionSkillIdList[i];
                SkillTable skillData = TableManager.instance.GetTableItem<SkillTable>(skillId);
                damage.IconPath = skillData.Icon;
                if (skillData.IsAttackCombo == 1)
                    damage.Name = "普通攻击";
                else
                    damage.Name = skillData.Name;
                damage.UseCount = GetSkillUseCount(data, skillId);
                if(actor!=null && actor.skillDamageManager != null)
                {
                    damage.Damage = actor.skillDamageManager.GetSkilDamage(skillId, selectMonsterId, data);
                    damage.Percent = actor.skillDamageManager.GetSkillDamagePercent(skillId, selectMonsterId, data);
                }
                if (damage.Damage == 0)
                    continue;
                newData.Add(damage);
            }
            newData.Sort(delegate (SkillDamage damage1, SkillDamage damage2)
            {
                return damage2.Damage.CompareTo(damage1.Damage);
            });
            return newData;
        }

        //获取技能使用次数
        private int GetSkillUseCount(SkillDamageData data, int skillId)
        {
            int index = 0;
            for (int i = 0; i < data.origionSkillIdList.Count; i++)
            {
                if (skillId == data.origionSkillIdList[i])
                {
                    index = i;
                    break;
                }
            }
            return data.origionSkillIdUseCount[index];
        }

        private BeActor GetMainPlayer()
        {
            if (BattleMain.instance == null)
            {
                return BeUtility.GetMainPlayerActor();
            }
            if (BattleMain.instance.GetPlayerManager() == null)
                return null;
            if (BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
                return null;
            return BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
        }
    }
}