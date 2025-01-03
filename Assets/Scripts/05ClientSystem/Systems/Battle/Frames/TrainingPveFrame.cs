using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using ProtoTable;
using UnityEngine;
using System;

namespace GameClient
{
    public struct TrainingPveMonsterData
    {
        public int monsterId;          //怪物ID
        public int level;              //怪物等级
        public int abnormalId;     //异常BuffId
        public bool isBati;            //是否霸体
        public bool isNormalMode;      //是否正常模式
        public bool isSecondMode;      //是否秒表模式
    }

    public class TrainingPveFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mButton_Open = null;
        private Button mButton_Close = null;
        private GameObject mContent = null;
        private Dropdown mDropdown_Name = null;
        private Dropdown mDropdown_Level = null;
        private Dropdown mDropdown_Abnormal = null;
        private Toggle mToggle_Bati = null;
        private Toggle mToggle_Normal = null;
        private Toggle mToggle_Second = null;
        private Dropdown mDropdown_Baoji = null;
        private Dropdown mDropdown_Siwei = null;
        private Dropdown mDropdown_ShuxingQianghu = null;
        private Dropdown mDropdown_Renwusudu = null;
        private Toggle mToggle_Pozhao = null;
        private Toggle mToggle_Mingzhong = null;
        private Button mButton_ResetCD = null;
        private Button mButton_Damage = null;
        private Button mButton_Summon = null;
        private Button mButton_DeleteAll = null;
        private Button mButton_Exit = null;
        private GameObject mSecondContent = null;
        private Text mSecondTime = null;
        private Text mTime = null;

        protected override void _bindExUI()
        {
            mButton_Open = mBind.GetCom<Button>("Button_Open");
            if (null != mButton_Open)
            {
                mButton_Open.onClick.AddListener(_onButton_OpenButtonClick);
            }
            mButton_Close = mBind.GetCom<Button>("Button_Close");
            if (null != mButton_Close)
            {
                mButton_Close.onClick.AddListener(_onButton_CloseButtonClick);
            }
            mContent = mBind.GetGameObject("Content");
            mDropdown_Name = mBind.GetCom<Dropdown>("Dropdown_Name");
            if (null != mDropdown_Name)
            {
                mDropdown_Name.onValueChanged.AddListener(_onDropdown_NameDropdownValueChange);
            }
            mDropdown_Level = mBind.GetCom<Dropdown>("Dropdown_Level");
            if (null != mDropdown_Level)
            {
                mDropdown_Level.onValueChanged.AddListener(_onDropdown_LevelDropdownValueChange);
            }
            mDropdown_Abnormal = mBind.GetCom<Dropdown>("Dropdown_Abnormal");
            if (null != mDropdown_Abnormal)
            {
                mDropdown_Abnormal.onValueChanged.AddListener(_onDropdown_AbnormalDropdownValueChange);
            }
            mToggle_Bati = mBind.GetCom<Toggle>("Toggle_Bati");
            if (null != mToggle_Bati)
            {
                mToggle_Bati.onValueChanged.AddListener(_onToggle_BatiToggleValueChange);
            }
            mToggle_Normal = mBind.GetCom<Toggle>("Toggle_Normal");
            if (null != mToggle_Normal)
            {
                mToggle_Normal.onValueChanged.AddListener(_onToggle_NormalToggleValueChange);
            }
            mToggle_Second = mBind.GetCom<Toggle>("Toggle_Second");
            if (null != mToggle_Second)
            {
                mToggle_Second.onValueChanged.AddListener(_onToggle_SecondToggleValueChange);
            }
            mDropdown_Baoji = mBind.GetCom<Dropdown>("Dropdown_Baoji");
            if (null != mDropdown_Baoji)
            {
                mDropdown_Baoji.onValueChanged.AddListener(_onDropdown_BaojiDropdownValueChange);
            }
            mDropdown_Siwei = mBind.GetCom<Dropdown>("Dropdown_Siwei");
            if (null != mDropdown_Siwei)
            {
                mDropdown_Siwei.onValueChanged.AddListener(_onDropdown_SiweiDropdownValueChange);
            }
            mDropdown_ShuxingQianghu = mBind.GetCom<Dropdown>("Dropdown_ShuxingQianghu");
            if (null != mDropdown_ShuxingQianghu)
            {
                mDropdown_ShuxingQianghu.onValueChanged.AddListener(_onDropdown_ShuxingQianghuDropdownValueChange);
            }
            mDropdown_Renwusudu = mBind.GetCom<Dropdown>("Dropdown_Renwusudu");
            if (null != mDropdown_Renwusudu)
            {
                mDropdown_Renwusudu.onValueChanged.AddListener(_onDropdown_RenwusuduDropdownValueChange);
            }
            mToggle_Pozhao = mBind.GetCom<Toggle>("Toggle_Pozhao");
            if (null != mToggle_Pozhao)
            {
                mToggle_Pozhao.onValueChanged.AddListener(_onToggle_PozhaoToggleValueChange);
            }
            mToggle_Mingzhong = mBind.GetCom<Toggle>("Toggle_Mingzhong");
            if (null != mToggle_Mingzhong)
            {
                mToggle_Mingzhong.onValueChanged.AddListener(_onToggle_MingzhongToggleValueChange);
            }
            mButton_ResetCD = mBind.GetCom<Button>("Button_ResetCD");
            if (null != mButton_ResetCD)
            {
                mButton_ResetCD.onClick.AddListener(_onButton_ResetCDButtonClick);
            }
            mButton_Damage = mBind.GetCom<Button>("Button_Damage");
            if (null != mButton_Damage)
            {
                mButton_Damage.onClick.AddListener(_onButton_DamageButtonClick);
            }
            mButton_Summon = mBind.GetCom<Button>("Button_Summon");
            if (null != mButton_Summon)
            {
                mButton_Summon.onClick.AddListener(_onButton_SummonButtonClick);
            }
            mButton_DeleteAll = mBind.GetCom<Button>("Button_DeleteAll");
            if (null != mButton_DeleteAll)
            {
                mButton_DeleteAll.onClick.AddListener(_onButton_DeleteAllButtonClick);
            }
            mButton_Exit = mBind.GetCom<Button>("Button_Exit");
            if (null != mButton_Exit)
            {
                mButton_Exit.onClick.AddListener(_onButton_ExitButtonClick);
            }
            mSecondContent = mBind.GetGameObject("SecondContent");
            mSecondTime = mBind.GetCom<Text>("SecondTime");
            mTime = mBind.GetCom<Text>("Time");
        }

        protected override void _unbindExUI()
        {
            if (null != mButton_Open)
            {
                mButton_Open.onClick.RemoveListener(_onButton_OpenButtonClick);
            }
            mButton_Open = null;
            if (null != mButton_Close)
            {
                mButton_Close.onClick.RemoveListener(_onButton_CloseButtonClick);
            }
            mButton_Close = null;
            mContent = null;
            if (null != mDropdown_Name)
            {
                mDropdown_Name.onValueChanged.RemoveListener(_onDropdown_NameDropdownValueChange);
            }
            mDropdown_Name = null;
            if (null != mDropdown_Level)
            {
                mDropdown_Level.onValueChanged.RemoveListener(_onDropdown_LevelDropdownValueChange);
            }
            mDropdown_Level = null;
            if (null != mDropdown_Abnormal)
            {
                mDropdown_Abnormal.onValueChanged.RemoveListener(_onDropdown_AbnormalDropdownValueChange);
            }
            mDropdown_Abnormal = null;
            if (null != mToggle_Bati)
            {
                mToggle_Bati.onValueChanged.RemoveListener(_onToggle_BatiToggleValueChange);
            }
            mToggle_Bati = null;
            if (null != mToggle_Normal)
            {
                mToggle_Normal.onValueChanged.RemoveListener(_onToggle_NormalToggleValueChange);
            }
            mToggle_Normal = null;
            if (null != mToggle_Second)
            {
                mToggle_Second.onValueChanged.RemoveListener(_onToggle_SecondToggleValueChange);
            }
            mToggle_Second = null;
            if (null != mDropdown_Baoji)
            {
                mDropdown_Baoji.onValueChanged.RemoveListener(_onDropdown_BaojiDropdownValueChange);
            }
            mDropdown_Baoji = null;
            if (null != mDropdown_Siwei)
            {
                mDropdown_Siwei.onValueChanged.RemoveListener(_onDropdown_SiweiDropdownValueChange);
            }
            mDropdown_Siwei = null;
            if (null != mDropdown_ShuxingQianghu)
            {
                mDropdown_ShuxingQianghu.onValueChanged.RemoveListener(_onDropdown_ShuxingQianghuDropdownValueChange);
            }
            mDropdown_ShuxingQianghu = null;
            if (null != mDropdown_Renwusudu)
            {
                mDropdown_Renwusudu.onValueChanged.RemoveListener(_onDropdown_RenwusuduDropdownValueChange);
            }
            mDropdown_Renwusudu = null;
            if (null != mToggle_Pozhao)
            {
                mToggle_Pozhao.onValueChanged.RemoveListener(_onToggle_PozhaoToggleValueChange);
            }
            mToggle_Pozhao = null;
            if (null != mToggle_Mingzhong)
            {
                mToggle_Mingzhong.onValueChanged.RemoveListener(_onToggle_MingzhongToggleValueChange);
            }
            mToggle_Mingzhong = null;
            if (null != mButton_ResetCD)
            {
                mButton_ResetCD.onClick.RemoveListener(_onButton_ResetCDButtonClick);
            }
            mButton_ResetCD = null;
            if (null != mButton_Damage)
            {
                mButton_Damage.onClick.RemoveListener(_onButton_DamageButtonClick);
            }
            mButton_Damage = null;
            if (null != mButton_Summon)
            {
                mButton_Summon.onClick.RemoveListener(_onButton_SummonButtonClick);
            }
            mButton_Summon = null;
            if (null != mButton_DeleteAll)
            {
                mButton_DeleteAll.onClick.RemoveListener(_onButton_DeleteAllButtonClick);
            }
            mButton_DeleteAll = null;
            if (null != mButton_Exit)
            {
                mButton_Exit.onClick.RemoveListener(_onButton_ExitButtonClick);
            }
            mButton_Exit = null;
            mSecondContent = null;
            mSecondTime = null;
            mTime = null;
        }
        #endregion

        #region Callback
        private void _onButton_OpenButtonClick()
        {
            mContent.CustomActive(true);
            mButton_Open.CustomActive(false);
        }
        private void _onButton_CloseButtonClick()
        {
            mButton_Open.CustomActive(true);
            mContent.CustomActive(false);
        }
        private void _onDropdown_NameDropdownValueChange(int index)
        {
            /* put your code in here */

        }
        private void _onDropdown_LevelDropdownValueChange(int index)
        {
            /* put your code in here */

        }
        private void _onDropdown_AbnormalDropdownValueChange(int index)
        {
            /* put your code in here */

        }
        private void _onToggle_BatiToggleValueChange(bool changed)
        {
            /* put your code in here */

        }
        private void _onToggle_NormalToggleValueChange(bool changed)
        {
            mSecondContent.CustomActive(false);
        }
        private void _onToggle_SecondToggleValueChange(bool changed)
        {
            mSecondContent.CustomActive(true);
        }
        private void _onDropdown_BaojiDropdownValueChange(int index)
        {
            if (battle != null)
                battle.ChangeMainActorBuff(index, 0);
        }
        private void _onDropdown_SiweiDropdownValueChange(int index)
        {
            if (battle != null)
                battle.ChangeMainActorBuff(index, 1);
        }
        private void _onDropdown_ShuxingQianghuDropdownValueChange(int index)
        {
            if (battle != null)
                battle.ChangeMainActorBuff(index, 2);
        }
        private void _onDropdown_RenwusuduDropdownValueChange(int index)
        {
            if (battle != null)
                battle.ChangeMainActorBuff(index, 3);
        }
        private void _onToggle_PozhaoToggleValueChange(bool changed)
        {
            if (battle != null)
                battle.ChangeActorBroken(changed);
        }
        private void _onToggle_MingzhongToggleValueChange(bool changed)
        {
            if (battle != null)
                battle.ChangeActorHitRate(changed);
        }
        private void _onButton_ResetCDButtonClick()
        {
            if (battle != null)
                battle.ResetAllCD();
        }
        private void _onButton_DamageButtonClick()
        {
            SkillDamageFrame frame = ClientSystemManager.instance.OpenFrame<SkillDamageFrame>(FrameLayer.Middle) as SkillDamageFrame;
            if (frame != null)
            {
                frame.InitData(true);
            }
        }
        private void _onButton_SummonButtonClick()
        {
            if (!mToggle_Second.isOn)
            {
                //普通模式
                if (battle != null)
                    battle.SummonMonster();
                mContent.CustomActive(false);
                mButton_Open.CustomActive(true);
            }       
            else
            {
                //秒表模式
                if(battle != null)
                    battle.DeleteAllMonster();
                mContent.CustomActive(false);
                mButton_Open.CustomActive(true);
                if (battle != null)
                    battle.ResetAllCD();
                SwitchSecondMode(true);
                needDelaySummon = true;
            }
        }
        private void _onButton_DeleteAllButtonClick()
        {
            if (battle != null)
                battle.DeleteAllMonster();
        }
        private void _onButton_ExitButtonClick()
        {
            OnPauseButtonClick();
        }
        #endregion

        TrainingPVEBattle battle = null;

        //怪物相关参数
        public int[] monsterIdArr = new int[] {  };
        public string[] monsterNameDesArr = new string[] { };
        
        private int[] levelArr = new int[] { 30, 40, 50, 55, 60};
        private string[] levelDesArr = new string[] {"30","40","50","55","60"};

        private int[] abnormaleArr = new int[] { 15, 17, 5, 7, 8, 3, 4, 811043, 6, 9, 10, 18};
        private string[] abnormalDesArr = new string[] { "中毒", "灼烧", "出血","睡眠","冰冻","眩晕","石化","诅咒","感电","束缚","混乱","失明"};

        //角色相关参数
        public int[] critBuffIdArr = new int[] { 811001, 811002, 811003, 811004, 811005, 811006, 811007, 811008, 811009, 811010 };
        private string[] critRateDesArr = new string[] { "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };

        public int[] staAddBuffIdArr = new int[] { 811011, 811012, 811013, 811014, 811015, 811016, 811017, 811018, 811019, 811020 };
        private string[] staAddDesArr = new string[] { "50", "100", "150", "200", "250", "300", "350", "400", "450", "500" };

        public int[] attAddBuffIdArr = new int[] { 811021, 811022, 811023, 811024, 811025, 811026, 811027, 811028, 811029, 811030 };
        private string[] attAddDesArr = new string[] { "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };

        public int[] speedAddBuffIdArr = new int[] { 811031, 811032, 811033, 811034, 811035, 811036, 811037, 811038, 811039, 811040 };
        private string[] speedAddDesArr = new string[] { "10", "20", "30", "40", "50", "60", "70", "80", "90", "100"};

        private bool needDelaySummon = false;
        private bool isInSecond= false;
        private BeActor secondMonster = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/TrainingPveMain";
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            base._OnUpdate(timeElapsed);
            UpdateCountDown(timeElapsed);
            UpdateSecondTime(timeElapsed);
        }

        public void InitUserData(TrainingPVEBattle trainingBattle)
        {
            battle = trainingBattle;
            InitTableData();
            InitDropDownData();
        }

        //初始化PVE修炼场怪物索引表数据
        private void InitTableData()
        {
            var tableData = TableManager.instance.GetTable<PveTrainingMonsterTable>();
            Dictionary<int, object>.Enumerator it = tableData.GetEnumerator();

            monsterIdArr = new int[tableData.Count];
            monsterNameDesArr = new string[tableData.Count];

            int index = 0;
            while (it.MoveNext())
            {
                PveTrainingMonsterTable item = it.Current.Value as PveTrainingMonsterTable;
                monsterIdArr[index] = item.ID;
                monsterNameDesArr[index] = item.Name;
                //预加载怪物数据
                PreloadManager.PreloadMonsterID(item.ID, null, null);
                index++;
            }
        }

        //初始化所有下拉列表的数据
        private void InitDropDownData()
        {
            InitSingleDropDownData(null, mDropdown_Name, monsterNameDesArr);
            InitSingleDropDownData("基本", mDropdown_Level, levelDesArr);
            InitAbnormalList();
            //InitSingleDropDownData("限制时间", mDropdown_Limit, timeLimitDesArr);
            InitSingleDropDownData("暴击几率", mDropdown_Baoji, critRateDesArr);
            InitSingleDropDownData("人物四维", mDropdown_Siwei, staAddDesArr);
            InitSingleDropDownData("属性强化", mDropdown_ShuxingQianghu, attAddDesArr);
            InitSingleDropDownData("人物速度", mDropdown_Renwusudu, speedAddDesArr);
        }

        //初始化单个下拉列表数据
        private void InitSingleDropDownData(string defaultName,Dropdown dropDown,string[] strArr)
        {
            List<Dropdown.OptionData> list = GamePool.ListPool<Dropdown.OptionData>.Get();
            if (defaultName != null)
            {
                Dropdown.OptionData operaData = new Dropdown.OptionData();
                operaData.text = defaultName;
                list.Add(operaData);
            }

            if (strArr != null)
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    Dropdown.OptionData operaData = new Dropdown.OptionData();
                    operaData.text = strArr[i];
                    list.Add(operaData);
                }
            }
            dropDown.AddOptions(list);
            GamePool.ListPool<Dropdown.OptionData>.Release(list);
        }

        private void InitAbnormalList()
        {
            List<string> list = GamePool.ListPool<string>.Get();
            list.Add("异常状态设置");
            
            for (int i = 0; i < abnormalDesArr.Length; i++)
            {
                list.Add(abnormalDesArr[i]);
            }

            mDropdown_Abnormal.AddOptions(list);
            GamePool.ListPool<string>.Release(list);
        }
        
        //获取设置界面设置的参数
        public TrainingPveMonsterData GetSummonMonsterData()
        {
            TrainingPveMonsterData monsterData = new TrainingPveMonsterData();

            int index = mDropdown_Name.value;
            monsterData.monsterId = monsterIdArr[index];

            index = mDropdown_Level.value;
            if (index == 0)
                monsterData.level = 30;
            else
                monsterData.level = levelArr[index - 1];

            index = mDropdown_Abnormal.value;
            if (index != 0)
                monsterData.abnormalId = abnormaleArr[index - 1];

            monsterData.isBati = mToggle_Bati.isOn;
            monsterData.isNormalMode = mToggle_Normal.isOn;
            monsterData.isSecondMode = mToggle_Second.isOn;

            return monsterData;
        }

        private void OnPauseButtonClick()
        {
            if (battle == null)
                return;
            battle.ReturnToTown();   
        }

        //切换到秒表模式
        private void SwitchSecondMode(bool isSecond)
        {
            bool flag = !isSecond;
            mDropdown_Baoji.enabled = flag;
            mDropdown_Baoji.GetComponent<UIGray>().enabled = isSecond;
            mDropdown_Siwei.enabled = flag;
            mDropdown_Siwei.GetComponent<UIGray>().enabled = isSecond;
            mDropdown_Renwusudu.enabled = flag;
            mDropdown_Renwusudu.GetComponent<UIGray>().enabled = isSecond;
            mDropdown_ShuxingQianghu.enabled = flag;
            mDropdown_ShuxingQianghu.GetComponent<UIGray>().enabled = isSecond;
            mToggle_Pozhao.enabled = flag;
            mToggle_Pozhao.GetComponent<UIGray>().enabled = isSecond;
            mToggle_Mingzhong.enabled = flag;
            mToggle_Mingzhong.GetComponent<UIGray>().enabled = isSecond;

            mDropdown_Name.enabled = flag;
            mDropdown_Name.GetComponent<UIGray>().enabled = isSecond;
            mDropdown_Level.enabled = flag;
            mDropdown_Level.GetComponent<UIGray>().enabled = isSecond;
            mDropdown_Abnormal.enabled = flag;
            mDropdown_Abnormal.GetComponent<UIGray>().enabled = isSecond;
            mToggle_Bati.enabled = flag;
            mToggle_Bati.GetComponent<UIGray>().enabled = isSecond;

            mButton_Summon.enabled = flag;
            mButton_Summon.GetComponent<UIGray>().enabled = isSecond;
            mButton_ResetCD.enabled = flag;
            mButton_ResetCD.GetComponent<UIGray>().enabled = isSecond;

            mToggle_Normal.enabled = flag;
            mToggle_Normal.GetComponent<UIGray>().enabled = isSecond;

            mToggle_Second.enabled = flag;
            mToggle_Second.GetComponent<UIGray>().enabled = isSecond;
        }


        private float currentTime = 3.0f;
        private float secondTime = 0;
        //刷新倒计时
        private void UpdateCountDown(float deltaTime)
        {
            if (!needDelaySummon)
                return;
            if(currentTime <= 0)
            {
                mTime.CustomActive(false);
                currentTime = 3.0f;
                needDelaySummon = false;
                secondTime = 0;
                secondMonster = battle.SummonMonster();
                isInSecond = true;
                mContent.CustomActive(false);
                mButton_Open.CustomActive(true);
                mSecondTime.text = "00:00:00";
            }
            else
            {
                mTime.CustomActive(true);
                mTime.text = Mathf.CeilToInt(currentTime).ToString();
                currentTime -= deltaTime;
            }
        }

        private void UpdateSecondTime(float deltaTime)
        {
            if (!isInSecond)
                return;
            secondTime += deltaTime;
            //怪物死亡
            if (secondMonster == null || secondMonster.IsDead())
            {
                SecondModeEnd();
                return;
            }
            float timeSpend = secondTime;
            int hour = (int)timeSpend / 3600;
            int minute = ((int)timeSpend - hour * 3600) / 60;
            int second = (int)timeSpend - hour * 3600 - minute * 60;
            int millisecond = (int)((secondTime - (int)timeSpend) * 1000);
            string millisecondStr = millisecond.ToString();
            if(millisecondStr.Length>=2)
                millisecondStr = millisecond.ToString().Substring(0, 2);

            mSecondTime.text = string.Format("{0:D2}:{1:D2}:{2:D3}", minute, second, millisecondStr);
        }

        private void SecondModeEnd()
        {
            isInSecond = false;
            SwitchSecondMode(false);
        }

    }
}

