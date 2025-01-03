using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //师徒获取表格问题数据的类型
    public enum TAPQuestionType
    {
        None = 0,
        Teacher = 1,
        Pupil = 2,
    }
    public class TAPQuestionnaireFrame : ClientFrame
    {
        const string regionFirstSelectGOPath = "UIFlatten/Prefabs/TAP/RegionSelectBtn";
        const string regionGOPath = "UIFlatten/Prefabs/TAP/RegionNameSelectBtn";
        //传给服务器的
        int activeTimeIndex = 0;     //活跃时间的下标
        int abilityIndex = 0;        //能力下标
        int regionIndex = 0;         //地区下标
        string declaration;          //宣言

        //客户端
        List<GameObject> questionGOList = new List<GameObject>();
        List<string> activeAnswer = new List<string>();
        List<string> abilityAnswer = new List<string>();
        int curIndex = -1;//当前到达的第几个问题（包括答题板）
        TAPQuestionType tapType = TAPQuestionType.None;
        bool haveInformation = false;   //这个玩家是不是已经填写过信息了
        string activeTimeStr;           //活动时间的描述
        string abilityStr;              //能力的描述
        string regionStr;               //地区描述
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPQuestionnaireFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            _InitData();
            _InitUI();
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();

        }

        void _RegisterUIEvent()
        {
            
            
        }

        void _UnRegisterUIEvent()
        {
            
        }

        void _InitData()
        {
            activeTimeIndex = 0;     //活跃时间的下标
            abilityIndex = 0;        //能力下标
            regionIndex = 0;         //地区下标
            declaration = null;
            questionGOList.Clear();
            activeAnswer.Clear();
            abilityAnswer.Clear();
            GameObject[] tempList = {mQuestionOneRoot,mQuestionTwoRoot,mQuestionThreeRoot,mQuesionFourRoot,mSubmitAnswer };
            questionGOList.AddRange(tempList);

            TAPType myType = TAPNewDataManager.GetInstance().IsTeacher();
            if ((int)myType > 1)
            {
                tapType = TAPQuestionType.Teacher;
            }
            else
            {
                tapType = TAPQuestionType.Pupil;
            }
        }

        void _StartQuestion()
        {

        }

        void _ClearData()
        {
            activeTimeIndex = 0;     //活跃时间的下标
            abilityIndex = 0;        //能力下标
            regionIndex = 0;         //地区下标
            declaration = null;
            questionGOList.Clear();
            activeAnswer.Clear();
            abilityAnswer.Clear();
            haveInformation = false;   //这个玩家是不是已经填写过信息了
            activeTimeStr = null;           //活动时间的描述
            abilityStr = null;              //能力的描述
            regionStr = null;               //地区描述
        }

        void _InitUI()
        {
            _InitNormalQuestion();
            _InitRegionQuestion();
            _InitAnswerInformation();
            if(haveInformation)
            {
                _OpenSubmitAnswer();
            }
            else
            {
                _StartAnswerQuestion();
            }
            if(tapType == TAPQuestionType.Teacher)
            {
                mSubmitText.text = "查找徒弟";
            }
            else
            {
                mSubmitText.text = "查找师父";
            }
        }

        void _ClearUI()
        {

        }
        
        /// <summary>
        /// 读表里数据初始化
        /// </summary>
        void _InitNormalQuestion()
        {
            var TAPQuestionaireTableDatas = TableManager.GetInstance().GetTable<TAPQuestionnaireTable>();
            var enumerator = TAPQuestionaireTableDatas.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var TAPQuestionnaireTableData = enumerator.Current.Value as TAPQuestionnaireTable;
                if(null == TAPQuestionnaireTableData)
                {
                    continue;
                }
                if(TAPQuestionnaireTableData.Type != (int)tapType)
                {
                    continue;
                }
                if(TAPQuestionnaireTableData.QuestionIndex == 1)
                {
                    mActiveTimeQuestionText.text = TAPQuestionnaireTableData.QuestionDes;
                    mSubmitQuestionText1.text = TAPQuestionnaireTableData.QuestionDes;
                    switch (TAPQuestionnaireTableData.AnswerIndex)
                    {
                        case 1:
                            {
                                mActiveTimeAnswer1.text = TAPQuestionnaireTableData.AnswerDes;
                                activeAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                        case 2:
                            {
                                mActiveTimeAnswer2.text = TAPQuestionnaireTableData.AnswerDes;
                                activeAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                        case 3:
                            {
                                mActiveTimeAnswer3.text = TAPQuestionnaireTableData.AnswerDes;
                                activeAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                    }
                }
                if(TAPQuestionnaireTableData.QuestionIndex == 2)
                {
                    mAbilityQuestionText.text = TAPQuestionnaireTableData.QuestionDes;
                    mSubmitQuestionText2.text = TAPQuestionnaireTableData.QuestionDes;
                    switch (TAPQuestionnaireTableData.AnswerIndex)
                    {
                        case 1:
                            {
                                mAbilityAnswer1.text = TAPQuestionnaireTableData.AnswerDes;
                                abilityAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                        case 2:
                            {
                                mAbilityAnswer2.text = TAPQuestionnaireTableData.AnswerDes;
                                abilityAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                        case 3:
                            {
                                mAbilityAnswer3.text = TAPQuestionnaireTableData.AnswerDes;
                                abilityAnswer.Add(TAPQuestionnaireTableData.AnswerDes);
                                break;
                            }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化地区相关界面的ui
        /// </summary>
        void _InitRegionQuestion()
        {
            List<string> firstLetterList = new List<string>();
            firstLetterList = TAPNewDataManager.GetInstance().GetFirstLetterList();
            for(int i = 0;i < firstLetterList.Count; i++)
            {
                GameObject regionFirstSelectGO = AssetLoader.instance.LoadResAsGameObject(regionFirstSelectGOPath);
                if (regionFirstSelectGO == null)
                {
                    return;
                }
                var mBind = regionFirstSelectGO.GetComponent<ComCommonBind>();
                if (mBind == null)
                {
                    return;
                }
                Text btnText = mBind.GetCom<Text>("Text");
                btnText.text = firstLetterList[i];

                Button btn = mBind.GetCom<Button>("RegionSelectBtn");
                string region = firstLetterList[i];
                if (firstLetterList[i][0] >= 'A' && firstLetterList[i][0] <= 'Z' || firstLetterList[i][0] >= 'a' && firstLetterList[i][0] <= 'z')
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(delegate
                    {
                        _SelectFirstLetter(region);
                    });
                }
                else
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(delegate
                    {
                        _SelectRegion(region);
                    });
                }
                Utility.AttachTo(regionFirstSelectGO, mFirstLetterRoot);
            }
        }

        /// <summary>
        /// 玩家如果已经有个人资料，则会用这些数据初始化结算面板界面，并且，默认直接打开结算界面，而不是，默认开始答题
        /// </summary>
        void _InitAnswerInformation()
        {
            var questionData = TAPNewDataManager.GetInstance().tapQuestionnaireInformation;
            if(questionData == null)
            {
                return;
            }
            if (questionData.activeTimeType == 0) //0为无效数据，0的时候视为玩家没有填写过个人信息
            {
                haveInformation = false;
            }
            else
            {
                haveInformation = true;
                activeTimeIndex = questionData.activeTimeType;     //活跃时间的下标
                abilityIndex = questionData.masterType;        //能力下标
                regionIndex = questionData.regionId;
                activeTimeStr = activeAnswer[questionData.activeTimeType - 1];           //活动时间的描述
                abilityStr = abilityAnswer[questionData.masterType - 1];              //能力的描述
                var regionTableData = TableManager.GetInstance().GetTableItem<AreaProvinceTable>(questionData.regionId);
                if(regionTableData != null)
                {
                    regionStr = regionTableData.Name;               //地区描述
                }
                mActiveTimeAnswer.text = activeTimeStr;     //活动时间的描述
                mAbilityAnswer.text = abilityStr;           //能力的描述
                mRegionAnswer.text = regionStr;             //地区的描述
                declaration = questionData.declaration;
                if(declaration == null || declaration.Length == 0)
                {
                    if(tapType == TAPQuestionType.Teacher)
                    {
                        declaration = TR.Value("tap_teacher_region");
                    }
                    else
                    {
                        declaration = TR.Value("tap_pupil_region");
                    }
                }
                mInputField.text = declaration;
            }
        }
        

        /// <summary>
        /// 选择地区首选字母之后
        /// </summary>
        /// <param name="firstLetter"></param>
        void _SelectFirstLetter(string firstLetter)
        {
            _AnswerQuestion(3);
            List<string> regionList = new List<string>();
            regionList = TAPNewDataManager.GetInstance().GetRegionList(firstLetter);
            if(regionList == null)
            {
                return;
            }
            for(int i = 0;i<regionList.Count; i++)
            {
                GameObject regionGO = AssetLoader.instance.LoadResAsGameObject(regionGOPath);
                if(regionGO == null)
                {
                    return;
                }
                var mBind = regionGO.GetComponent<ComCommonBind>();
                if (mBind == null)
                {
                    return;
                }
                Text btnText = mBind.GetCom<Text>("Text");
                string tempStr = regionList[i];
                btnText.text = tempStr;

                Button btn = mBind.GetCom<Button>("RegionSelectBtn");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(delegate
                {
                    _SelectRegion(tempStr);
                });
                Utility.AttachTo(regionGO, mRegionRoot);
            }
        }

        /// <summary>
        /// 选择地区
        /// </summary>
        /// <param name="region"></param>
        void _SelectRegion(string region)
        {
            _AnswerQuestion(4);
            regionIndex = TAPNewDataManager.GetInstance().GetRegionID(region);
            regionStr = region;
            _SetSubmitAnswer();
        }

        /// <summary>
        /// 将之前玩家填写的数据展现在ui面板上
        /// </summary>
        void _SetSubmitAnswer()
        {
            mActiveTimeAnswer.text = activeTimeStr;     //活动时间的描述
            mAbilityAnswer.text = abilityStr;           //能力的描述
            mRegionAnswer.text = regionStr;             //地区的描述
        }

        void _StartAnswerQuestion()
        {
            curIndex = 0;
            _AnswerQuestion();
        }

        void _OpenSubmitAnswer()
        {
            curIndex = questionGOList.Count - 1;
            _AnswerQuestion(curIndex);
        }

        void _AnswerQuestion(int index = -1)
        {
            if(index != -1)
            {
                curIndex = index;
            }
            if(curIndex > questionGOList.Count)
            {
                frameMgr.CloseFrame(this);
            }
            if(curIndex == questionGOList.Count - 1)
            {
                mTitleName.text = TR.Value("tap_question_last_name");
                mTitleQuestionText.text = TR.Value("tap_question_submit_title");
            }
            else
            {
                mTitleName.text = TR.Value("tap_question_normal_name");
                if (tapType == TAPQuestionType.Teacher)
                {
                    mTitleQuestionText.text = TR.Value("tap_question_teacher_title");
                }
                else
                {
                    mTitleQuestionText.text = TR.Value("tap_question_pupil_title");
                }
            }
            for (int i = 0; i < questionGOList.Count; i++)
            {
                questionGOList[i].CustomActive(false);
                if(i == curIndex)
                {
                    questionGOList[i].CustomActive(true);
                }
            }
            curIndex++;

            int questionIndex = -1;
            if(curIndex > 2)
            {
                questionIndex = 3;
            }
            else
            {
                questionIndex = curIndex;
            }
            mSchedule.text = string.Format("第（{0} / 3）题", questionIndex);
        }

        

        #region ExtraUIBind
        private Button mClose = null;
		private Text mSchedule = null;
		private GameObject mQuestionOneRoot = null;
		private GameObject mQuestionTwoRoot = null;
		private GameObject mQuestionThreeRoot = null;
		private GameObject mQuesionFourRoot = null;
		private GameObject mSubmitAnswer = null;
		private Button mActiveTimeSubmit1 = null;
		private Button mActiveTimeSubmit2 = null;
		private Button mActiveTimeSubmit3 = null;
		private Button mAbilitySubmit1 = null;
		private Button mAbilitySubmit2 = null;
		private Button mAbilitySubmit3 = null;
		private GameObject mFirstLetterRoot = null;
		private GameObject mRegionRoot = null;
		private Text mActiveTimeAnswer = null;
		private Text mAbilityAnswer = null;
		private Text mRegionAnswer = null;
		private Text mDeclarationAnswer = null;
		private Button mReWrite = null;
		private Button mWorldSay = null;
		private Button mSubmit = null;
		private InputField mInputField = null;
		private Text mActiveTimeQuestionText = null;
		private Text mActiveTimeAnswer1 = null;
		private Text mActiveTimeAnswer2 = null;
		private Text mActiveTimeAnswer3 = null;
		private Text mAbilityQuestionText = null;
		private Text mAbilityAnswer1 = null;
		private Text mAbilityAnswer2 = null;
		private Text mAbilityAnswer3 = null;
		private Text mSubmitText = null;
        private Text mTitleName = null;
        private Text mTitleQuestionText = null;
        private Text mSubmitQuestionText1 = null;
        private Text mSubmitQuestionText2 = null;
        private Text mSubmitQuestionText3 = null;

        protected override void _bindExUI()
		{
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mSchedule = mBind.GetCom<Text>("Schedule");
			mQuestionOneRoot = mBind.GetGameObject("QuestionOneRoot");
			mQuestionTwoRoot = mBind.GetGameObject("QuestionTwoRoot");
			mQuestionThreeRoot = mBind.GetGameObject("QuestionThreeRoot");
			mQuesionFourRoot = mBind.GetGameObject("QuesionFourRoot");
			mSubmitAnswer = mBind.GetGameObject("SubmitAnswer");
			mActiveTimeSubmit1 = mBind.GetCom<Button>("ActiveTimeSubmit1");
			if (null != mActiveTimeSubmit1)
			{
				mActiveTimeSubmit1.onClick.AddListener(_onActiveTimeSubmit1ButtonClick);
			}
			mActiveTimeSubmit2 = mBind.GetCom<Button>("ActiveTimeSubmit2");
			if (null != mActiveTimeSubmit2)
			{
				mActiveTimeSubmit2.onClick.AddListener(_onActiveTimeSubmit2ButtonClick);
			}
			mActiveTimeSubmit3 = mBind.GetCom<Button>("ActiveTimeSubmit3");
			if (null != mActiveTimeSubmit3)
			{
				mActiveTimeSubmit3.onClick.AddListener(_onActiveTimeSubmit3ButtonClick);
			}
			mAbilitySubmit1 = mBind.GetCom<Button>("AbilitySubmit1");
			if (null != mAbilitySubmit1)
			{
				mAbilitySubmit1.onClick.AddListener(_onAbilitySubmit1ButtonClick);
			}
			mAbilitySubmit2 = mBind.GetCom<Button>("AbilitySubmit2");
			if (null != mAbilitySubmit2)
			{
				mAbilitySubmit2.onClick.AddListener(_onAbilitySubmit2ButtonClick);
			}
			mAbilitySubmit3 = mBind.GetCom<Button>("AbilitySubmit3");
			if (null != mAbilitySubmit3)
			{
				mAbilitySubmit3.onClick.AddListener(_onAbilitySubmit3ButtonClick);
			}
			mFirstLetterRoot = mBind.GetGameObject("FirstLetterRoot");
			mRegionRoot = mBind.GetGameObject("RegionRoot");
			mActiveTimeAnswer = mBind.GetCom<Text>("ActiveTimeAnswer");
			mAbilityAnswer = mBind.GetCom<Text>("AbilityAnswer");
			mRegionAnswer = mBind.GetCom<Text>("RegionAnswer");
			mDeclarationAnswer = mBind.GetCom<Text>("DeclarationAnswer");
			mReWrite = mBind.GetCom<Button>("ReWrite");
			if (null != mReWrite)
			{
				mReWrite.onClick.AddListener(_onReWriteButtonClick);
			}
			mWorldSay = mBind.GetCom<Button>("WorldSay");
			if (null != mWorldSay)
			{
				mWorldSay.onClick.AddListener(_onWorldSayButtonClick);
			}
			mSubmit = mBind.GetCom<Button>("Submit");
			if (null != mSubmit)
			{
				mSubmit.onClick.AddListener(_onSubmitButtonClick);
			}
			mInputField = mBind.GetCom<InputField>("InputField");
if (null != mInputField)
            {
                mInputField.onValueChanged.AddListener(_OnValueChanged);
            }			mActiveTimeQuestionText = mBind.GetCom<Text>("ActiveTimeQuestionText");
			mActiveTimeAnswer1 = mBind.GetCom<Text>("ActiveTimeAnswer1");
			mActiveTimeAnswer2 = mBind.GetCom<Text>("ActiveTimeAnswer2");
			mActiveTimeAnswer3 = mBind.GetCom<Text>("ActiveTimeAnswer3");
			mAbilityQuestionText = mBind.GetCom<Text>("AbilityQuestionText");
			mAbilityAnswer1 = mBind.GetCom<Text>("AbilityAnswer1");
			mAbilityAnswer2 = mBind.GetCom<Text>("AbilityAnswer2");
			mAbilityAnswer3 = mBind.GetCom<Text>("AbilityAnswer3");
			mSubmitText = mBind.GetCom<Text>("SubmitText");
            mTitleName = mBind.GetCom<Text>("TitleName");
            mTitleQuestionText = mBind.GetCom<Text>("TitleQuestionText");
            mSubmitQuestionText1 = mBind.GetCom<Text>("SubmitQuestionText1");
            mSubmitQuestionText2 = mBind.GetCom<Text>("SubmitQuestionText2");
            mSubmitQuestionText3 = mBind.GetCom<Text>("SubmitQuestionText3");
        }
		
		protected override void _unbindExUI()
		{
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			mSchedule = null;
			mQuestionOneRoot = null;
			mQuestionTwoRoot = null;
			mQuestionThreeRoot = null;
			mQuesionFourRoot = null;
			mSubmitAnswer = null;
			if (null != mActiveTimeSubmit1)
			{
				mActiveTimeSubmit1.onClick.RemoveListener(_onActiveTimeSubmit1ButtonClick);
			}
			mActiveTimeSubmit1 = null;
			if (null != mActiveTimeSubmit2)
			{
				mActiveTimeSubmit2.onClick.RemoveListener(_onActiveTimeSubmit2ButtonClick);
			}
			mActiveTimeSubmit2 = null;
			if (null != mActiveTimeSubmit3)
			{
				mActiveTimeSubmit3.onClick.RemoveListener(_onActiveTimeSubmit3ButtonClick);
			}
			mActiveTimeSubmit3 = null;
			if (null != mAbilitySubmit1)
			{
				mAbilitySubmit1.onClick.RemoveListener(_onAbilitySubmit1ButtonClick);
			}
			mAbilitySubmit1 = null;
			if (null != mAbilitySubmit2)
			{
				mAbilitySubmit2.onClick.RemoveListener(_onAbilitySubmit2ButtonClick);
			}
			mAbilitySubmit2 = null;
			if (null != mAbilitySubmit3)
			{
				mAbilitySubmit3.onClick.RemoveListener(_onAbilitySubmit3ButtonClick);
			}
			mAbilitySubmit3 = null;
			mFirstLetterRoot = null;
			mRegionRoot = null;
			mActiveTimeAnswer = null;
			mAbilityAnswer = null;
			mRegionAnswer = null;
			mDeclarationAnswer = null;
			if (null != mReWrite)
			{
				mReWrite.onClick.RemoveListener(_onReWriteButtonClick);
			}
			mReWrite = null;
			if (null != mWorldSay)
			{
				mWorldSay.onClick.RemoveListener(_onWorldSayButtonClick);
			}
			mWorldSay = null;
			if (null != mSubmit)
			{
				mSubmit.onClick.RemoveListener(_onSubmitButtonClick);
			}
			mSubmit = null;
if(null != mInputField)
            {
                mInputField.onValueChanged.RemoveListener(_OnValueChanged);
            }			mInputField = null;
			mActiveTimeQuestionText = null;
			mActiveTimeAnswer1 = null;
			mActiveTimeAnswer2 = null;
			mActiveTimeAnswer3 = null;
			mAbilityQuestionText = null;
			mAbilityAnswer1 = null;
			mAbilityAnswer2 = null;
			mAbilityAnswer3 = null;
			mSubmitText = null;
            mTitleName = null;
            mTitleQuestionText = null;
            mSubmitQuestionText1 = null;
            mSubmitQuestionText2 = null;
            mSubmitQuestionText3 = null;
        }
		#endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onActiveTimeSubmit1ButtonClick()
        {
            /* put your code in here */
            activeTimeIndex = 1;
            Text des = mActiveTimeSubmit1.GetComponentInChildren<Text>();
            if(des != null)
            {
                activeTimeStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onActiveTimeSubmit2ButtonClick()
        {
            /* put your code in here */
            activeTimeIndex = 2;
            Text des = mActiveTimeSubmit2.GetComponentInChildren<Text>();
            if (des != null)
            {
                activeTimeStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onActiveTimeSubmit3ButtonClick()
        {
            /* put your code in here */
            activeTimeIndex = 3;
            Text des = mActiveTimeSubmit3.GetComponentInChildren<Text>();
            if (des != null)
            {
                activeTimeStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onAbilitySubmit1ButtonClick()
        {
            /* put your code in here */
            abilityIndex = 1;
            Text des = mAbilitySubmit1.GetComponentInChildren<Text>();
            if (des != null)
            {
                abilityStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onAbilitySubmit2ButtonClick()
        {
            /* put your code in here */
            abilityIndex = 2;
            Text des = mAbilitySubmit2.GetComponentInChildren<Text>();
            if (des != null)
            {
                abilityStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onAbilitySubmit3ButtonClick()
        {
            /* put your code in here */
            abilityIndex = 3;
            Text des = mAbilitySubmit3.GetComponentInChildren<Text>();
            if (des != null)
            {
                abilityStr = des.text;
            }
            _AnswerQuestion();
        }
        private void _onReWriteButtonClick()
        {
            /* put your code in here */
            _StartAnswerQuestion();
        }
        private void _onWorldSayButtonClick()
        {
            /* put your code in here */
            if (declaration == null || declaration.Length == 0)
            {
                if (tapType == TAPQuestionType.Teacher)
                {
                    declaration = TR.Value("tap_teacher_region");
                }
                else
                {
                    declaration = TR.Value("tap_pupil_region");
                }
            }
            TAPNewDataManager.GetInstance().SendTAPInformation(activeTimeIndex, abilityIndex, regionIndex, declaration);

            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                //当前等级不能收徒也不能拜师
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacherOrPupil"));
            }
            else if(tapType == TAPQuestionType.Teacher)
            {
                //走收徒逻辑
                if (!TAPNewDataManager.GetInstance().canSearchTeacher())
                {
                    //有师傅的时候没法收徒
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacher2"));
                }
                else if (TAPNewDataManager.GetInstance().canSearchPupil())
                {
                    //正常收徒
                    TAPNewDataManager.GetInstance().AnnounceWorld(Protocol.RelationAnnounceType.Master);
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_world_announce"));
                }
                else
                {
                    //徒弟满了不能在收徒
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchPupil"));
                }
            }
            else
            {
                //走拜师逻辑
                if (TAPNewDataManager.GetInstance().canSearchTeacher())
                {
                    TAPNewDataManager.GetInstance().AnnounceWorld(Protocol.RelationAnnounceType.Disciple);
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_world_announce"));
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacher"));
                }
            }

            
            frameMgr.CloseFrame(this);
            //SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_people_information"));
        }
        private void _onSubmitButtonClick()
        {
            if (declaration == null || declaration.Length == 0)
            {
                if (tapType == TAPQuestionType.Teacher)
                {
                    declaration = TR.Value("tap_teacher_region");
                }
                else
                {
                    declaration = TR.Value("tap_pupil_region");
                }
            }
            TAPNewDataManager.GetInstance().SendTAPInformation(activeTimeIndex, abilityIndex, regionIndex, declaration);

            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.TeacherSoon)
            {
                //当前等级不能收徒也不能拜师
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacherOrPupil"));
            }
            else if (tapType == TAPQuestionType.Teacher)
            {
                //走收徒逻辑
                if (!TAPNewDataManager.GetInstance().canSearchTeacher())
                {
                    //有师傅的时候没法收徒
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacher2"));
                }
                else if (TAPNewDataManager.GetInstance().canSearchPupil())
                {
                    //正常收徒
                    ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
                }
                else
                {
                    //徒弟满了不能在收徒
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchPupil"));
                }
            }
            else
            {
                //走拜师逻辑
                if (TAPNewDataManager.GetInstance().canSearchTeacher())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TAPSearchFrame>(FrameLayer.Middle, 0);
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_cannot_searchTeacher"));
                }
            }
            
            frameMgr.CloseFrame(this);
            //SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("tap_people_information"));
        }

        private void _OnValueChanged(string value)
        {
            declaration = value;
        }
        #endregion
    }
}
