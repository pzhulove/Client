using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;
using Network;
using System.Collections;

namespace GameClient
{
    public enum RewardJarStatic
    {
        None = 0,
        UnOpen = 1,
        CanOpen = 2,
        HaveOpen = 3,
    }
    class EquipRecoveryFrame : ClientFrame
    {
        public delegate void SetSliderValue(float fatigueCoinText);
        public SetSliderValue setSliderValueHandler;

        int maxjarScore = 0;//最大积分
        int maxjarCount = 0;//最大次数
        int rewardScore = 0;
        int minScore = 0;
        int maxScore = 100;
        List<RewardJarItem> rewardJarList = new List<RewardJarItem>(); //奖励罐子的链表
        WorldEquipRecoOpenJarsRecordRes m_recordData = null;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipRecoveryFrame";
        }

        public static void OpenLinkFrame(string strParam)
        {
            GameClient.ClientSystemManager.GetInstance().CloseFrame<EquipRecoveryFrame>();
            GameClient.ClientSystemManager.GetInstance().OpenFrame<EquipRecoveryFrame>();
        }
        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            _InitData();
            _InitUI();
        }

        void _InitData()
        {
            EquipRecoveryDataManager.GetInstance().HaveWeekRedPoint = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
            rewardJarList.Clear();
            setSliderValueHandler = null;
            SystemValueTable jarPointNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_REQUIP_RECOVERY_JAR_MAX);
            if(jarPointNumData != null)
            {
                maxjarScore = jarPointNumData.Value;
            }
            SystemValueTable jarWeekNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_TIMES);
            if(jarWeekNumData != null)
            {
                maxjarCount = jarWeekNumData.Value;
            }
            _UpdateWeekCount();
            rewardScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
        }
        void _InitUI()
        {
            _InitRecordList();
            
            _ClearRecord();
            _InitReward();
            _UpdateJarRecord();

        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _ClearData()
        {
            maxjarScore = 0;
            maxjarCount = 0;
            rewardScore = 0;
            minScore = 0;
            maxScore = 100;
            setSliderValueHandler = null;
            rewardJarList.Clear();
        }

        void _ClearUI()
        {
            _ClearRecord();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRecoveryPriceReqSuccess, OnEquipRecoveryPriceReqSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipSubmitSuccess, OnEquipSubmitSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRedeemSuccess, OnUpdateJarStatic);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipUpgradeSuccess, OnUpdateJarStatic);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipJarListUpdate, _OnEquipJarListUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnGetJarRecord);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipSubmitScore, _EquipSubmitScore);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRecoveryPriceReqSuccess, OnEquipRecoveryPriceReqSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipSubmitSuccess, OnEquipSubmitSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRedeemSuccess, OnUpdateJarStatic);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipUpgradeSuccess, OnUpdateJarStatic);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipJarListUpdate, _OnEquipJarListUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnGetJarRecord);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipSubmitScore, _EquipSubmitScore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iEvent"></param>
        void OnEquipRecoveryPriceReqSuccess(UIEvent iEvent)
        {
            int iItemID = (int)iEvent.Param1;
            int iPrice = (int)iEvent.Param2;
        }

        /// <summary>
        /// 装备回收提交成功
        /// </summary>
        /// <param name="iEvent"></param>
        void OnEquipSubmitSuccess(UIEvent iEvent)
        {
            uint score = (uint)iEvent.Param1;
            //_UpdateJarRecord();
            _TryPlayEffect((int)score);
            
            //尝试播放某个特效
            if (mEffect.activeSelf)
            {
                mEffect.CustomActive(false);
            }
            mEffect.CustomActive(true);
            StartCoroutine(DonateResult((int)score));
            
        }

        /// <summary>
        /// ToDo 优化成动画可配置曲线
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        IEnumerator DonateResult(int score)
        {
            yield return new WaitForSeconds(1f);
            mTextRoot.CustomActive(true);
            int count = 0;
            List<EqRecScoreItem> rewardList = EquipRecoveryDataManager.GetInstance().submitResult;
            for (int i = 0; i < rewardList.Count; i++)
            {
                count += (int)rewardList[i].score;
            }
            float time = 0.01f;
            for (int i = 0;i < 30; i++)
            {
                
                mDonateResult.text = UnityEngine.Random.Range(minScore, maxScore+1).ToString(); ;
                yield return new WaitForSeconds(time);
                time += 0.005f;
            }
            mDonateResult.text = count.ToString() ;
            yield return null;
            yield return new WaitForSeconds(1.45f);
            mTextRoot.CustomActive(false);

        }
        /// <summary>
        /// 刷新罐子的状态
        /// </summary>
        /// <param name="iEvent"></param>
        void OnUpdateJarStatic(UIEvent iEvent)
        {
            _UpdateReward();
        }

        /// <summary>
        /// 刷新打开罐子的记录
        /// </summary>
        /// <param name="a_event"></param>
        void _OnUpdateOpenRecord(UIEvent a_event)
        {
            _ClearRecord();
            m_recordData = a_event.Param1 as WorldEquipRecoOpenJarsRecordRes;
            _UpdateRecord();
        }

        /// <summary>
        /// 刷新打开罐子的记录
        /// </summary>
        /// <param name="a_event"></param>
        void _OnEquipJarListUpdate(UIEvent iEvent)
        {
            _ClearRecord();
            m_recordData = iEvent.Param1 as WorldEquipRecoOpenJarsRecordRes;
            StartCoroutine(_UpdateEquipRecord());
        }

        IEnumerator _UpdateEquipRecord()
        {
            while(true)
            {
                if(!frameMgr.IsFrameOpen<ShowItemsFrame>())
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            //yield return new WaitForSeconds(3.0f);
            //_ClearRecord();
            _UpdateRecord();
        }

        /// <summary>
        /// 刷新罐子记录
        /// </summary>
        /// <param name="iEvent"></param>
        void _OnGetJarRecord(UIEvent iEvent)
        {
            _UpdateJarRecord();
        }

        /// <summary>
        /// 得到这次获得的积分范围
        /// </summary>
        /// <param name="iEvent"></param>
        void _EquipSubmitScore(UIEvent iEvent)
        {
            minScore = (int)iEvent.Param1;
            maxScore = (int)iEvent.Param2;
        }

        void _InitReward()
        {
            EquipRecoveryDataManager.GetInstance(). _ClearJarKeyList();
            var requipRecoveryRewardTableData = TableManager.GetInstance().GetTable<EquipRecoveryRewardTable>();
            var enumerator = requipRecoveryRewardTableData.GetEnumerator();
            int index = 0;
            while(enumerator.MoveNext())
            {
                if(index >= mRootList.Count)
                {
                    break;
                }
                var equipRecoverRewardTableItem = enumerator.Current.Value as EquipRecoveryRewardTable;
                RewardJarItem jarItem = new RewardJarItem();
                jarItem.CreateGo(mRootList[index], equipRecoverRewardTableItem);
                rewardJarList.Add(jarItem);
                EquipRecoveryDataManager.GetInstance()._AddJarKeyList(equipRecoverRewardTableItem.Integral);
                index++;
            }

            _UpdateReward();
        }

        void _UpdateJarRecord()
        {
            List<int> tempJarIDList = new List<int>();
            for(int i=0;i<rewardJarList.Count;i++)
            {
                tempJarIDList.Add(rewardJarList[i].GetJarID());
            }
            EquipRecoveryDataManager.GetInstance().RequestJarRecord(tempJarIDList);
        }

        /// <summary>
        /// 提交成功时刷新罐子状态和走特效
        /// </summary>
        void _TryPlayEffect(int score)
        {
            //rewardScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
            rewardScore = score;
            float sliderValue = mGetAniValue.GetValue(rewardScore);
            float jarRectWidth = mBoxRoot.GetComponent<RectTransform>().sizeDelta.x;//slider的宽度

            Vector2 start = new Vector2(jarRectWidth * mBoxSlider.value, 0);
            Vector2 end = new Vector2(jarRectWidth * sliderValue, 0);
            mSliderChange.CustomActive(true);
            mSliderChange.StartRemove(start,end, _UpdateRewardCallBack);
        }

        void _UpdateReward()
        {
            rewardScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
            mHaveScore.text = rewardScore.ToString();
            mBoxSlider.value = mGetAniValue.GetValue(rewardScore);
            for (int i = 0; i < rewardJarList.Count; i++)
            {
                rewardJarList[i].UpdateRewardJar(i + 1);
            }
        }

        void _UpdateRewardCallBack()
        {
            if(mBoxSlider == null || mGetAniValue == null || mSliderChange == null)
            {
                return;
            }
            mBoxSlider.value = mGetAniValue.GetValue(rewardScore);
            for (int i = 0; i < rewardJarList.Count; i++)
            {
                if(rewardJarList[i] == null)
                {
                    continue;
                }

                rewardJarList[i].UpdateRewardJar(i + 1);
            }
            mSliderChange.CustomActive(false);
        }

        void _UpdateWeekCount()
        {
            mContributeContent.text = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_WEEK_COUNT).ToString() + "/" + maxjarCount + "次";
        }

        void _UpdateRecoveryCount()
        {

        }

        void _OnPackageItemClicked(GameObject obj, ItemData item)
        {
            obj.GetComponent<ComItem>().SetShowSelectState(true);
        }

        private void OnCountValueChanged(UIEvent uiEvent)
        {
            _UpdateReward();
            _UpdateWeekCount();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.EquipRecovery);
        }

        void _InitRecordList()
        {
            mRecordUIList.Initialize();

            mRecordUIList.onItemVisiable = var =>
            {
                if (m_recordData != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_recordData.records.Length)
                    {
                        int nIdx = m_recordData.records.Length - var.m_index - 1;
                        OpenJarRecord record = m_recordData.records[nIdx];
                        ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)record.itemId);
                        if (item != null)
                        {
                            string strItem = string.Format(" {{I 0 {0} 0}}", record.itemId);
                            var.GetComponent<LinkParse>().SetText(TR.Value("jar_record", record.name, strItem, record.num));
                            return;
                        }
                    }
                }
            };
        }

        void _UpdateRecord()
        {

            if (m_recordData != null)
            {
                //if (
                //    ((m_togMagicJar.isOn || m_togMagic_Jar_Lv55.isOn) && m_recordData.jarId == m_magicJarData.nID) ||
                //    (m_togGoldJar.isOn && m_recordData.jarId == m_currGoldJarData.nID)
                //    )
                //{
                    mRecordUIList.SetElementAmount(m_recordData.records.Length);
                    mRecordUIList.EnsureElementVisable(0);
                    return;
                //}
            }

            mRecordUIList.SetElementAmount(0);
        }

        void _ClearRecord()
        {
            mRecordUIList.SetElementAmount(0);
            m_recordData = null;
        }

        


        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mRecordUIList = null;
        private ComUIListElementScript mRecordUIListElement = null;
        private GameObject mScoreBar = null;
        private Slider mBoxSlider = null;
        private GameObject mBoxRoot = null;
        private Button mContribute = null;
        private Button mContributeRecord = null;
        private Button mEquipPromote = null;
        private Text mContributeContent = null;
        private List<GameObject> mRootList = new List<GameObject>();
        private GameObject mEffect = null;
        private SliderChange mSliderChange = null;
        private GameObject mRemoveEffect = null;
        private Text mDonateResult = null;
        private GameObject mTextRoot = null;
        private GetAniValue mGetAniValue = null;
        private Text mHaveScore = null;
        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mRecordUIList = mBind.GetCom<ComUIListScript>("recordUIList");
            mRecordUIListElement = mBind.GetCom<ComUIListElementScript>("recordUIListElement");
            mScoreBar = mBind.GetGameObject("scoreBar");
            mBoxSlider = mBind.GetCom<Slider>("boxSlider");
            mBoxRoot = mBind.GetGameObject("boxRoot");
            mContribute = mBind.GetCom<Button>("contribute");
            mContribute.onClick.AddListener(_onContributeButtonClick);
            mContributeRecord = mBind.GetCom<Button>("contributeRecord");
            mContributeRecord.onClick.AddListener(_onContributeRecordButtonClick);
            mEquipPromote = mBind.GetCom<Button>("equipPromote");
            mEquipPromote.onClick.AddListener(_onEquipPromoteButtonClick);
            mContributeContent = mBind.GetCom<Text>("contributeContent");
            mRootList.Add(mBind.GetGameObject("root1"));
            mRootList.Add(mBind.GetGameObject("root2"));
            mRootList.Add(mBind.GetGameObject("root3"));
            mRootList.Add(mBind.GetGameObject("root4"));
            mRootList.Add(mBind.GetGameObject("root5"));
            mRootList.Add(mBind.GetGameObject("root6"));
            mRootList.Add(mBind.GetGameObject("root7"));
            mRootList.Add(mBind.GetGameObject("root8"));
            mRootList.Add(mBind.GetGameObject("root9"));
            mRootList.Add(mBind.GetGameObject("root10"));
            mEffect = mBind.GetGameObject("Effect");
            mSliderChange = mBind.GetCom<SliderChange>("SliderChange");
            mRemoveEffect = mBind.GetGameObject("removeEffect");
            mDonateResult = mBind.GetCom<Text>("donateResult");
            mTextRoot = mBind.GetGameObject("textRoot");
            mGetAniValue = mBind.GetCom<GetAniValue>("GetAniValue");
            mHaveScore = mBind.GetCom<Text>("haveScore");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mRecordUIList = null;
            mRecordUIListElement = null;
            mScoreBar = null;
            mBoxSlider = null;
            mBoxRoot = null;
            mContribute.onClick.RemoveListener(_onContributeButtonClick);
            mContribute = null;
            mContributeRecord.onClick.RemoveListener(_onContributeRecordButtonClick);
            mContributeRecord = null;
            mEquipPromote.onClick.RemoveListener(_onEquipPromoteButtonClick);
            mEquipPromote = null;
            mContributeContent = null;
            mRootList.Clear();
            mEffect = null;
            mSliderChange = null;
            mRemoveEffect = null;
            mDonateResult = null;
            mTextRoot = null;
            mGetAniValue = null;
            mHaveScore = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        private void _onContributeButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<EquipDonateFrame>();
        }
        private void _onContributeRecordButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<EquipReturnFrame>();
        }
        private void _onEquipPromoteButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeFrame>();
        }
        #endregion
    }
}
