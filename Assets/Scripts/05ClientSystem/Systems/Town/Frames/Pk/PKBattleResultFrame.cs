using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using DG.Tweening;
using UnityEngine.Assertions;

namespace GameClient
{
    public class PKBattleResultData
    {
        public PKBattleResultData(SceneMatchPkRaceEnd res)
        {
            pkType                  = res.pkType;
            result                  = res.result;
            oldPkValue              = res.oldPkValue;
            newPkValue              = res.newPkValue;
            oldMatchScore           = res.oldMatchScore;
            newMatchScore           = res.newMatchScore;
            oldPkCoin               = res.oldPkCoin;
            addPkCoinFromRace       = res.addPkCoinFromRace;
            totalPkCoinFromRace     = res.totalPkCoinFromRace;
            isInPvPActivity         = res.isInPvPActivity;
            addPkCoinFromActivity   = res.addPkCoinFromActivity;
            totalPkCoinFromActivity = res.totalPkCoinFromActivity;
            oldSeasonLevel          = res.oldSeasonLevel;
            newSeasonLevel          = res.newSeasonLevel;
            oldSeasonStar           = res.oldSeasonStar;
            newSeasonStar           = res.newSeasonStar;
            oldSeasonExp            = res.oldSeasonExp;
            newSeasonExp            = res.newSeasonExp;
            changeSeasonExp         = res.changeSeasonExp;
            changeGlory             = res.getHonor;
        }

        public PKBattleResultData(SceneRoomMatchPkRaceEnd res)
        {
            pkType                  = res.pkType;
            result                  = res.result;
            oldPkValue              = res.oldPkValue;
            newPkValue              = res.newPkValue;
            oldMatchScore           = res.oldMatchScore;
            newMatchScore           = res.newMatchScore;
            oldPkCoin               = res.oldPkCoin;
            addPkCoinFromRace       = res.addPkCoinFromRace;
            totalPkCoinFromRace     = res.totalPkCoinFromRace;
            isInPvPActivity         = res.isInPvPActivity;
            addPkCoinFromActivity   = res.addPkCoinFromActivity;
            totalPkCoinFromActivity = res.totalPkCoinFromActivity;
            oldSeasonLevel          = res.oldSeasonLevel;
            newSeasonLevel          = res.newSeasonLevel;
            oldSeasonStar           = res.oldSeasonStar;
            newSeasonStar           = res.newSeasonStar;
            oldSeasonExp            = res.oldSeasonExp;
            newSeasonExp            = res.newSeasonExp;
            changeSeasonExp         = res.changeSeasonExp;
            changeGlory             = res.getHonor;
        }

        public byte pkType
        {
            get {
                return mPkType;
            }
            set {
                Logger.LogProcessFormat("[] mPkType, {0} -> {1}", mPkType, value);
                mPkType = value;
            }
        }
        private byte mPkType;

		public byte result
        {
            get {
                return mResult;
            }
            set {
                Logger.LogProcessFormat("[] mResult, {0} -> {1}", mResult, value);
                mResult = value;
            }
        }
        private byte mResult;

		public UInt32 oldPkValue
        {
            get {
                return mOldPkValue;
            }
            set {
                Logger.LogProcessFormat("[] mOldPkValue, {0} -> {1}", mOldPkValue, value);
                mOldPkValue = value;
            }
        }
        private UInt32 mOldPkValue;

		public UInt32 newPkValue
        {
            get {
                return mNewPkValue;
            }
            set {
                Logger.LogProcessFormat("[] mNewPkValue, {0} -> {1}", mNewPkValue, value);
                mNewPkValue = value;
            }
        }
        private UInt32 mNewPkValue;

		public UInt32 oldMatchScore
        {
            get {
                return mOldMatchScore;
            }
            set {
                Logger.LogProcessFormat("[] mOldMatchScore, {0} -> {1}", mOldMatchScore, value);
                mOldMatchScore = value;
            }
        }
        private UInt32 mOldMatchScore;

		public UInt32 newMatchScore
        {
            get {
                return mNewMatchScore;
            }
            set {
                Logger.LogProcessFormat("[] mNewMatchScore, {0} -> {1}", mNewMatchScore, value);
                mNewMatchScore = value;
            }
        }
        private UInt32 mNewMatchScore;

		/// <summary>
		///  初始决斗币数量
		/// </summary>
		public UInt32 oldPkCoin
        {
            get {
                return mOldPkCoin;
            }
            set {
                Logger.LogProcessFormat("[] mOldPkCoin, {0} -> {1}", mOldPkCoin, value);
                mOldPkCoin = value;
            }
        }
        private UInt32 mOldPkCoin;

		/// <summary>
		///  战斗获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromRace
        {
            get {
                return mAddPkCoinFromRace;
            }
            set {
                Logger.LogProcessFormat("[] mAddPkCoinFromRace, {0} -> {1}", mAddPkCoinFromRace, value);
                mAddPkCoinFromRace = value;
            }
        }
        private UInt32 mAddPkCoinFromRace;

		/// <summary>
		///  今日战斗获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromRace
        {
            get {
                return mTotalPkCoinFromRace;
            }
            set {
                Logger.LogProcessFormat("[] mTotalPkCoinFromRace, {0} -> {1}", mTotalPkCoinFromRace, value);
                mTotalPkCoinFromRace = value;
            }
        }
        private UInt32 mTotalPkCoinFromRace;

		/// <summary>
		///  是否在PVP活动期间
		/// </summary>
		public byte isInPvPActivity
        {
            get {
                return mIsInPvPActivity;
            }
            set {
                Logger.LogProcessFormat("[] mIsInPvPActivity, {0} -> {1}", mIsInPvPActivity, value);
                mIsInPvPActivity = value;
            }
        }
        private byte mIsInPvPActivity;

		/// <summary>
		///  活动额外获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromActivity
        {
            get {
                return mAddPkCoinFromActivity;
            }
            set {
                Logger.LogProcessFormat("[] mAddPkCoinFromActivity, {0} -> {1}", mAddPkCoinFromActivity, value);
                mAddPkCoinFromActivity = value;
            }
        }
        private UInt32 mAddPkCoinFromActivity;

		/// <summary>
		///  今日活动获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromActivity
        {
            get {
                return mTotalPkCoinFromActivity;
            }
            set {
                Logger.LogProcessFormat("[] mTotalPkCoinFromActivity, {0} -> {1}", mTotalPkCoinFromActivity, value);
                mTotalPkCoinFromActivity = value;
            }
        }
        private UInt32 mTotalPkCoinFromActivity;

		/// <summary>
		///  原段位
		/// </summary>
		public UInt32 oldSeasonLevel
        {
            get {
                return mOldSeasonLevel;
            }
            set {
                Logger.LogProcessFormat("[] mOldSeasonLevel, {0} -> {1}", mOldSeasonLevel, value);
                mOldSeasonLevel = value;
            }
        }
        private UInt32 mOldSeasonLevel;

		/// <summary>
		///  现段位
		/// </summary>
		public UInt32 newSeasonLevel
        {
            get {
                return mNewSeasonLevel;
            }
            set {
                Logger.LogProcessFormat("[] mNewSeasonLevel, {0} -> {1}", mNewSeasonLevel, value);
                mNewSeasonLevel = value;
            }
        }
        private UInt32 mNewSeasonLevel;

		/// <summary>
		///  原星
		/// </summary>
		public UInt32 oldSeasonStar
        {
            get {
                return mOldSeasonStar;
            }
            set {
                Logger.LogProcessFormat("[] mOldSeasonStar, {0} -> {1}", mOldSeasonStar, value);
                mOldSeasonStar = value;
            }
        }
        private UInt32 mOldSeasonStar;

		/// <summary>
		///  现星
		/// </summary>
		public UInt32 newSeasonStar
        {
            get {
                return mNewSeasonStar;
            }
            set {
                Logger.LogProcessFormat("[] mNewSeasonStar, {0} -> {1}", mNewSeasonStar, value);
                mNewSeasonStar = value;
            }
        }
        private UInt32 mNewSeasonStar;

		/// <summary>
		///  原经验
		/// </summary>
		public UInt32 oldSeasonExp
        {
            get {
                return mOldSeasonExp;
            }
            set {
                Logger.LogProcessFormat("[] mOldSeasonExp, {0} -> {1}", mOldSeasonExp, value);
                mOldSeasonExp = value;
            }
        }
        private UInt32 mOldSeasonExp;

		/// <summary>
		///  现经验
		/// </summary>
		public UInt32 newSeasonExp
        {
            get {
                return mNewSeasonExp;
            }
            set {
                Logger.LogProcessFormat("[] mNewSeasonExp, {0} -> {1}", mNewSeasonExp, value);
                mNewSeasonExp = value;
            }
        }
        private UInt32 mNewSeasonExp;

		/// <summary>
		///  改变的经验
		/// </summary>
		public Int32 changeSeasonExp
        {
            get {
                return mChangeSeasonExp;
            }
            set {
                Logger.LogProcessFormat("[] mChangeSeasonExp, {0} -> {1}", mChangeSeasonExp, value);
                mChangeSeasonExp = value;
            }
        }
        private Int32 mChangeSeasonExp;

        public UInt32 changeGlory
        {
            get
            {
                return mChangeGlory;
            }
            set
            {
                Logger.LogProcessFormat("[] mChangeGlory, {0} -> {1}", mChangeGlory, value);
                mChangeGlory = value;
            }
        }
        private UInt32 mChangeGlory;

    }

    // PK结算代码，改不动了，只能按照原来的结构写
    class PKBattleResultFrame : ClientFrame
    {
        [UIObject("Content/TitleNode/Title/Win")]
        GameObject m_objTitleWin;

        [UIObject("Content/TitleNode/Title/Fail")]
        GameObject m_objTitleFail;

        [UIObject("Content/TitleNode/Title/Draw")]
        GameObject m_objTitleDraw;

        [UIObject("Content/TitleNode/Title/Error")]
        GameObject m_objTitleError;

        [UIObject("Content/TitleNode/Title/PromotionSuccess")]
        GameObject m_objPromotionSuccess;

        [UIObject("Content/TitleNode/Title/PromotionFail")]
        GameObject m_objPromotionFail;

        [UIObject("Content/RankNode/RankRoot")]
        GameObject m_objRankRoot;

        [UIObject("Content/PromotionNode")]
        GameObject m_objPromotionRoot;

        [UIObject("Content/PromotionNode/Records")]
        GameObject m_objRecordRoot;

        [UIObject("Content/PromotionNode/Records")]
        GameObject m_objRecordConetnt;

        [UIObject("Content/PromotionNode/Records/Template")]
        GameObject m_objRecordTemplate;

        [UIControl("Content/PromotionNode/Desc")]
        Text m_labPromotionDesc;

        [UIObject("Content/DescNode/DescGroup")]
        GameObject m_objDescRoot;

        [UIObject("Content/DescNode/DescGroup/Text")]
        GameObject m_objDescTemplate;

        [UIObject("Content/DescNode/DescGroup/UpTxt")]
        GameObject m_objDescUpTxt;
        [UIObject("Content/DescNode/DescGroup/MiddleTxt")]
        GameObject m_objDescMiddleTxt;

        [UIControl("Content/RankNode/Score")]
        Text m_labScore;

        [UIObject("Reporter")]
        GameObject m_reportBtn;

        ComPKRank m_comPKRank = null;
        PKBattleResultData m_msgRet = null;
        PromotionInfo m_promotionInfo = null;
        PKResult m_ePKResult = PKResult.INVALID;
        UnityEngine.Coroutine m_corShowDesc = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PKBattleResult";
        }

        protected override void _OnOpenFrame()
        {
            if (userData is SceneMatchPkRaceEnd)
            {
                m_msgRet = new PKBattleResultData(userData as SceneMatchPkRaceEnd);
            }
            else if (userData is SceneRoomMatchPkRaceEnd)
            {
                m_msgRet = new PKBattleResultData(userData as SceneRoomMatchPkRaceEnd);
            }
            else
            {
                Logger.LogErrorFormat("[战斗] 战斗结算 错误传入类型");
                return ;
            }

            m_ePKResult = (PKResult)m_msgRet.result;
            m_promotionInfo = SeasonDataManager.GetInstance().GetPromotionInfo((int)m_msgRet.oldSeasonLevel, m_ePKResult);
            if (m_reportBtn != null)
            {
                m_reportBtn.CustomActive(true);
            }
            //Logger.LogErrorFormat("result:{0} oldSeasonLevel:{1} newSeasonLevel:{2} oldSeasonStar:{3} newSeasonStar:{4}",
            //   m_msgRet.result, m_msgRet.oldSeasonLevel, m_msgRet.newSeasonLevel, m_msgRet.oldSeasonStar, m_msgRet.newSeasonStar);

            m_objRankRoot.CustomActive(m_msgRet.pkType != (byte)PkType.Pk_Friends && m_msgRet.pkType != (byte)PkType.Pk_1V1_CHIJI);
            m_objPromotionRoot.CustomActive(m_msgRet.pkType != (byte)PkType.Pk_Friends && m_msgRet.pkType != (byte)PkType.Pk_1V1_CHIJI);
            mDescNode.CustomActive(m_msgRet.pkType != (byte)PkType.Pk_Friends && m_msgRet.pkType != (byte)PkType.Pk_1V1_CHIJI);

            _InitTitle();
            _InitPKRank();
            _InitPromotion();
            _InitDesc();
            _StatisticResult();
            _bindUIEvent();
            if (m_msgRet.pkType == (byte)PkType.PK_EQUAL_1V1||m_msgRet.pkType==(byte)PkType.PK_EQUAL_PRACTICE)
            {
                m_objDescRoot.CustomActive(false);
                m_objRankRoot.CustomActive(false);
            }
        }

        protected override void _OnCloseFrame()
        {
            m_msgRet = null;
            m_promotionInfo = null;
            m_ePKResult = PKResult.INVALID;

            _ClearTitle();
            _ClearPKRank();
            _ClearPromotion();
            _ClearDesc();
            _unBindUIEvent();
        }

        private void _bindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUploadFileSucc, _OnUpLoadFileSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, _OnCounterChanged);
        }

        private void _unBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUploadFileSucc, _OnUpLoadFileSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, _OnCounterChanged);
        }

        private void _OnCounterChanged(UIEvent ui)
        {
            _UpdateGloryTxt();
        }

        void _InitTitle()
        {
            m_objTitleWin.SetActive(false);
            m_objTitleFail.SetActive(false);
            m_objTitleDraw.SetActive(false);
            m_objTitleError.SetActive(false);
            m_objPromotionSuccess.SetActive(false);
            m_objPromotionFail.SetActive(false);

            EPromotionState ePromotionState = m_promotionInfo.eState;
            if (ePromotionState == EPromotionState.Invalid || ePromotionState == EPromotionState.Promoting)
            {
                if (m_ePKResult == PKResult.WIN) // 胜利
                {
                    m_objTitleWin.SetActive(true);
                }
                else if (m_ePKResult == PKResult.LOSE) // 失败
                {
                    m_objTitleFail.SetActive(true);
                }
                else if (m_ePKResult == PKResult.DRAW) // 平局
                {
                    m_objTitleDraw.SetActive(true);
                }
                else if (m_ePKResult == PKResult.INVALID) // 异常
                {
                    m_objTitleError.SetActive(true);
                }
            }
            else if (ePromotionState == EPromotionState.Successed)
            {
                m_objPromotionSuccess.SetActive(true);
                AudioManager.instance.PlaySound(15);
            }
            else if (ePromotionState == EPromotionState.Failed)
            {
                m_objPromotionFail.SetActive(true);
                AudioManager.instance.PlaySound(16);
            }
        }

        void _ClearTitle()
        {

        }

        void _InitPKRank()
        {
            m_comPKRank = ComPKRank.Create(m_objRankRoot);
            if (m_comPKRank != null)
            {
                int nStartID, nStartStar, nStartExp, nEndID, nEndStar, nEndExp;
                if (m_ePKResult == PKResult.INVALID)
                {
                    nStartID = SeasonDataManager.GetInstance().seasonLevel;
                    nStartStar = SeasonDataManager.GetInstance().seasonStar;
                    nStartExp = SeasonDataManager.GetInstance().seasonExp;
                    nEndID = SeasonDataManager.GetInstance().seasonLevel;
                    nEndStar = SeasonDataManager.GetInstance().seasonStar;
                    nEndExp = SeasonDataManager.GetInstance().seasonExp;
                }
                else
                {
                    nStartID = (int)m_msgRet.oldSeasonLevel;
                    nStartStar = (int)m_msgRet.oldSeasonStar;
                    nStartExp = (int)m_msgRet.oldSeasonExp;

                    int nCurrentWinStreak, nMaxWinStreak;
                    _GetWinStreakInfo(out nCurrentWinStreak, out nMaxWinStreak);
                    if (nCurrentWinStreak >= nMaxWinStreak)
                    {
                        SeasonDataManager.GetInstance().GetPreLevel(
                            (int)m_msgRet.newSeasonLevel, (int)m_msgRet.newSeasonStar, (int)m_msgRet.newSeasonExp,
                            out nEndID, out nEndStar, out nEndExp
                            );
                    }
                    else
                    {
                        nEndID = (int)m_msgRet.newSeasonLevel;
                        nEndStar = (int)m_msgRet.newSeasonStar;
                        nEndExp = (int)m_msgRet.newSeasonExp;
                    }
                    //Logger.LogErrorFormat("连胜次数：{0}/{1}", nCurrentWinStreak, nMaxWinStreak);
                    //Logger.LogErrorFormat("段位：{0}_{1} => {2}_{3}", nStartID, nStartStar, nEndID, nEndStar);
                }

                m_comPKRank.Initialize(nStartID, nStartExp);

                if (m_msgRet.changeSeasonExp > 0)
                {
                    m_labScore.gameObject.SetActive(true);
                    m_labScore.text = TR.Value("pk_rank_battle_increase_score", m_msgRet.changeSeasonExp);
                }
                else if (m_msgRet.changeSeasonExp < 0)
                {
                    m_labScore.gameObject.SetActive(true);
                    m_labScore.text = TR.Value("pk_rank_battle_decrease_score", m_msgRet.changeSeasonExp);
                }
                else
                {
                    m_labScore.gameObject.SetActive(false);
                }

                if (m_promotionInfo.eState != EPromotionState.Promoting)
                {
                    DOTweenAnimation doTween = m_objRankRoot.GetComponent<DOTweenAnimation>();
                    if (doTween != null)
                    {
                        if (doTween.onStepComplete != null && m_objRankRoot.activeSelf)
                        {
                            doTween.onStepComplete.AddListener(() =>
                            {
                                m_comPKRank.StartRankChange(nStartID, nStartStar, nStartExp, nEndID, nEndStar, nEndExp);
                            });
                        }
                    }
                }
            }
        }
        private void _OnUpLoadFileSucc(UIEvent a_event)
        {
            if (m_reportBtn != null)
            {
                m_reportBtn.CustomActive(false);
            }
        }
        void _ClearPKRank()
        {
            if (m_comPKRank != null)
            {
                m_comPKRank.Clear();
                m_comPKRank = null;
            }
        }

        void _InitPromotion()
        {
            if (m_promotionInfo.eState != EPromotionState.Promoting && m_promotionInfo.eState != EPromotionState.Failed)
            {
                m_objPromotionRoot.SetActive(false);
                return;
            }

            PromotionInfo info = SeasonDataManager.GetInstance().GetPromotionInfo((int)m_msgRet.oldSeasonLevel);
            if (info.eState == EPromotionState.Promoting)
            {
                m_objPromotionRoot.SetActive(true);

                _SetChildrenEnable(m_objRecordConetnt, false);
                for (int i = 0; i < info.nTotalCount; ++i)
                {
                    GameObject objRecord = null;
                    if (i < m_objRecordConetnt.transform.childCount)
                    {
                        objRecord = m_objRecordConetnt.transform.GetChild(i).gameObject;
                    }
                    else
                    {
                        objRecord = GameObject.Instantiate(m_objRecordTemplate);
                        objRecord.transform.SetParent(m_objRecordConetnt.transform, false);
                    }
                    objRecord.SetActive(true);
                    _SetChildrenEnable(objRecord, false);
                }

                Assert.IsTrue(info.arrRecords.Count < m_objRecordConetnt.transform.childCount);
                for (int i = 0; i < info.arrRecords.Count; ++i)
                {
                    GameObject objRecord = m_objRecordConetnt.transform.GetChild(i).gameObject;
                    if (info.arrRecords[i] == (byte)PKResult.WIN)
                    {
                        Utility.FindGameObject(objRecord, "Win").SetActive(true);
                    }
                    else if (info.arrRecords[i] == (byte)PKResult.LOSE || info.arrRecords[i] == (byte)PKResult.DRAW)
                    {
                        Utility.FindGameObject(objRecord, "Lose").SetActive(true);
                    }
                }

                if (m_ePKResult != PKResult.INVALID)
                {
                    DOTweenAnimation doTween = m_objRecordRoot.GetComponent<DOTweenAnimation>();
                    if (doTween != null)
                    {
                        if (doTween.onStepComplete != null)
                        {
                            doTween.onStepComplete.AddListener(() =>
                            {
                                GameObject objRecord = m_objRecordConetnt.transform.GetChild(info.arrRecords.Count).gameObject;
                                if (m_ePKResult == PKResult.WIN)
                                {
                                    GameObject objWin = Utility.FindGameObject(objRecord, "Win");
                                    objWin.SetActive(true);
                                    _PlayAnims(objWin.GetComponents<DOTweenAnimation>());
                                    Utility.FindGameObject(objRecord, "EffUI_ui_sheng").SetActive(true);
                                    AudioManager.instance.PlaySound(17);
                                }
                                else if (m_ePKResult == PKResult.LOSE || m_ePKResult == PKResult.DRAW)
                                {
                                    GameObject objWin = Utility.FindGameObject(objRecord, "Lose");
                                    objWin.SetActive(true);
                                    _PlayAnims(objWin.GetComponents<DOTweenAnimation>());
                                    Utility.FindGameObject(objRecord, "EffUI_ui_bai").SetActive(true);
                                    AudioManager.instance.PlaySound(18);
                                }
                            });
                        }
                    }

                }

                m_labPromotionDesc.text = TR.Value("pk_rank_detail_promotion_rule",
                    info.nTotalCount, info.nTargetWinCount, SeasonDataManager.GetInstance().GetRankName(info.nNextSeasonLevel));
            }
            else
            {
                m_objPromotionRoot.SetActive(false);
            }
        }

        void _ClearPromotion()
        {

        }

        void _UpdateGloryTxt()
        {
            m_objDescTemplate.SetActive(false);

            List<string> arrDescs = new List<string>();
            int nIdx = -1;

            EPromotionState ePromotionState = m_promotionInfo.eState;
            if (ePromotionState == EPromotionState.Invalid)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_coin_get", m_msgRet.addPkCoinFromRace));
                arrDescs.Add(TR.Value("pk_rank_battle_coin_info", m_msgRet.totalPkCoinFromRace, _GetDailyMaxPKCoin(), PlayerBaseData.GetInstance().VipLevel));

                arrDescs.Add(TR.Value("pk_rank_battle_glory_get", m_msgRet.changeGlory));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_info", _GetWeeklyTotalGlory(), _GetWeeklyMaxPVPGlory()));
                int nCurrentWinStreak, nMaxWinStreak;
                _GetWinStreakInfo(out nCurrentWinStreak, out nMaxWinStreak);
                arrDescs.Add(TR.Value("pk_rank_battle_winning_streak", nMaxWinStreak, nCurrentWinStreak));

                nIdx = nCurrentWinStreak >= nMaxWinStreak ? 2 : -1;
            }
            else if (ePromotionState == EPromotionState.Promoting || ePromotionState == EPromotionState.Successed)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_coin_get", m_msgRet.addPkCoinFromRace));
                arrDescs.Add(TR.Value("pk_rank_battle_coin_info", m_msgRet.totalPkCoinFromRace, _GetDailyMaxPKCoin(), PlayerBaseData.GetInstance().VipLevel));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_get", m_msgRet.changeGlory));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_info", _GetWeeklyTotalGlory(), _GetWeeklyMaxPVPGlory()));
            }
            else if (ePromotionState == EPromotionState.Failed)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_promotion_failed"));
            }

            for (int i = 0; i < arrDescs.Count; ++i)
            {
                var txt = arrObjDescs[i].GetComponent<Text>();
                if(txt != null)
                {
                    txt.text = arrDescs[i];
                }
            }
        }

        List<GameObject> arrObjDescs;

        void _InitDesc()
        {
            m_objDescTemplate.SetActive(false);

            List<string> arrDescs = new List<string>();
            int nIdx = -1;

            EPromotionState ePromotionState = m_promotionInfo.eState;
            if (ePromotionState == EPromotionState.Invalid)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_coin_get", m_msgRet.addPkCoinFromRace));
                arrDescs.Add(TR.Value("pk_rank_battle_coin_info", m_msgRet.totalPkCoinFromRace, _GetDailyMaxPKCoin(), PlayerBaseData.GetInstance().VipLevel));

                arrDescs.Add(TR.Value("pk_rank_battle_glory_get", m_msgRet.changeGlory));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_info", _GetWeeklyTotalGlory(), _GetWeeklyMaxPVPGlory()));
                int nCurrentWinStreak, nMaxWinStreak;
                _GetWinStreakInfo(out nCurrentWinStreak, out nMaxWinStreak);
                arrDescs.Add(TR.Value("pk_rank_battle_winning_streak", nMaxWinStreak, nCurrentWinStreak));

                nIdx = nCurrentWinStreak >= nMaxWinStreak ? 2 : -1;
            }
            else if (ePromotionState == EPromotionState.Promoting || ePromotionState == EPromotionState.Successed)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_coin_get", m_msgRet.addPkCoinFromRace));
                arrDescs.Add(TR.Value("pk_rank_battle_coin_info", m_msgRet.totalPkCoinFromRace, _GetDailyMaxPKCoin(), PlayerBaseData.GetInstance().VipLevel));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_get", m_msgRet.changeGlory));
                arrDescs.Add(TR.Value("pk_rank_battle_glory_info", _GetWeeklyTotalGlory(), _GetWeeklyMaxPVPGlory()));
            }
            else if (ePromotionState == EPromotionState.Failed)
            {
                arrDescs.Add(TR.Value("pk_rank_battle_promotion_failed"));
            }

            arrObjDescs = _CreateDescs(arrDescs);
            DOTweenAnimation doTween = m_objDescRoot.GetComponent<DOTweenAnimation>();
            if (doTween != null)
            {
                if (doTween.onStepComplete != null)
                {
                    doTween.onStepComplete.AddListener(() =>
                    {
                        m_corShowDesc = GameFrameWork.instance.StartCoroutine(_ShowDescs(arrObjDescs, nIdx));
                    });
                }
            }
        }

        List<GameObject> _CreateDescs(List<string> a_arrDescs)
        {
            List<GameObject> arrObjDescs = new List<GameObject>();
            for (int i = 0; i < a_arrDescs.Count; ++i)
            {
                GameObject objDesc = GameObject.Instantiate(m_objDescTemplate);
                if (i < 2)
                {
                    objDesc.transform.SetParent(m_objDescUpTxt.transform, false);
                }
                else if (i < 4) 
                {
                    objDesc.transform.SetParent(m_objDescMiddleTxt.transform, false);
                }
                else
                {
                    objDesc.transform.SetParent(m_objDescRoot.transform, false);
                }
                objDesc.GetComponent<Text>().text = a_arrDescs[i];
                objDesc.SetActive(true);
                arrObjDescs.Add(objDesc);
            }
            return arrObjDescs;
        }

        IEnumerator _ShowDescs(List<GameObject> a_arrObjDescs, int a_nWinStreakSuccessIdx = -1)
        {
            string errormsg = "";
            for (int i = 0; i < a_arrObjDescs.Count; ++i)
            {
                try {
                    if(a_arrObjDescs[i] == null)
                    {
                        //Logger.LogErrorFormat("_ShowDescs a_arrObjDescs is Null pointer {0}", i);
                        continue;
                    }
                    DOTweenAnimation[] doTweens = a_arrObjDescs[i].GetComponents<DOTweenAnimation>();
                    if (doTweens != null)
                    {
                        if (i != a_nWinStreakSuccessIdx)
                        {
                            for (int j = 0; j < doTweens.Length; ++j)
                            {
                                DOTweenAnimation doTween = doTweens[j];
                                if (doTween == null)
                                {
                                    errormsg += string.Format("[ _ShowDescs {0} WinStreakSuccess {1} is null]", a_nWinStreakSuccessIdx, j);
                                    continue;
                                }
                                if (doTween.id != "special")
                                {
                                    doTween.isActive = true;
                                    if (doTween.tween == null)
                                    {
                                        doTween.CreateTween();
                                    }
                                    if(doTween.tween != null)
                                        doTween.tween.Restart();
                                    else
                                    {
                                        errormsg += string.Format("[ _ShowDescs {0} WinStreakSuccess {1} is createTween failure]", a_nWinStreakSuccessIdx, j);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < doTweens.Length; ++j)
                            {
                                DOTweenAnimation doTween = doTweens[j];
                                if (doTween == null)
                                {
                                    errormsg += string.Format("[ _ShowDescs {0} not WinStreakSuccess {1} is null]", a_nWinStreakSuccessIdx, j);
                                    continue;
                                }
                                if (doTween.id != "common")
                                {
                                    doTween.isActive = true;
                                    if (doTween.onStepComplete != null && m_objRankRoot.activeSelf)
                                    {
                                        doTween.onStepComplete.AddListener(() =>
                                                {
                                                int nStartLevel, nStartStar, nStartExp;
                                                SeasonDataManager.GetInstance().GetPreLevel(
                                                        (int)m_msgRet.newSeasonLevel, (int)m_msgRet.newSeasonStar, (int)m_msgRet.newSeasonExp,
                                                        out nStartLevel, out nStartStar, out nStartExp
                                                        );
                                                m_comPKRank.StartRankChange(
                                                        nStartLevel, nStartStar, nStartExp, 
                                                        (int)m_msgRet.newSeasonLevel, (int)m_msgRet.newSeasonStar, (int)m_msgRet.newSeasonExp
                                                        );
                                                });
                                    }

                                    if (doTween.tween == null)
                                    {
                                        doTween.CreateTween();
                                    }
                                   
                                    if(doTween.tween != null)
                                    {
                                        doTween.tween.Restart();
                                    }
                                    else
                                    {
                                        errormsg += string.Format("[ _ShowDescs {0} not WinStreakSuccess {1} is createTween failure]", a_nWinStreakSuccessIdx, j);
                                    }
                                }
                            }
                        }
                        //Logger.LogErrorFormat("_ShowDescs Null pointer find reason {0}", errormsg);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("[nulljudge] {0}", e.ToString());
                }

                yield return Yielders.GetWaitForSeconds(0.2f);
            }
        }

        void _ClearDesc()
        {
            if (m_corShowDesc != null)
            {
                GameFrameWork.instance.StopCoroutine(m_corShowDesc);
                m_corShowDesc = null;
            }
        }

        int _GetDailyMaxPKCoin()
        {
            int nValue = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PKCOIN_BASENUM);
            if (SystemValueTableData != null)
            {
                nValue = SystemValueTableData.Value;
            }

            float fMaxCoinData = Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.PK_MONEY_LIMIT);
            nValue += (int)fMaxCoinData;

            return nValue;
        }

        int _GetWeeklyTotalGlory()
        {
            int tempValue = 0;
            if(CountDataManager.GetInstance() != null)
            {
                tempValue = CountDataManager.GetInstance().GetCount("pk_season_1v1_honor");
            }
            return tempValue;
        }
        int _GetWeeklyMaxPVPGlory()
        {
            int tempValue = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_PK_SEASON_1V1_HONOR_MAX);
            if (SystemValueTableData != null)
            {
                tempValue = SystemValueTableData.Value;
            }
            return tempValue;
        }
        void _GetWinStreakInfo(out int a_nCurrentWinStreak, out int a_nMaxWinStreak)
        {
            EPromotionState ePromotionState = m_promotionInfo.eState;
            if (ePromotionState == EPromotionState.Invalid)
            {
                a_nCurrentWinStreak = CountDataManager.GetInstance().GetCount("season_win_streak");
                if (m_ePKResult == PKResult.WIN)
                {
                    //a_nCurrentWinStreak++;
                }
                else if (m_ePKResult == PKResult.LOSE)
                {
                    a_nCurrentWinStreak = 0;
                }
            }
            else
            {
                a_nCurrentWinStreak = 0;
            }

            a_nMaxWinStreak = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_SEASON_WIN_STREAK_COUNT).Value;
        }

        PKResult _GetBattleResult()
        {
            if (m_msgRet != null)
            {
                return (PKResult)m_msgRet.result;
            }
            else
            {
                return PKResult.INVALID;
            }
        }

        void _StatisticResult()
        {
            string sResult = "";
            if (m_ePKResult == PKResult.WIN) // 胜利
            {
                sResult = "胜利";
            }
            else if (m_ePKResult == PKResult.LOSE) // 失败
            {
                sResult = "失败";
            }
            else if (m_ePKResult == PKResult.DRAW) // 平局
            {
                sResult = "平局";
            }
            else if (m_ePKResult == PKResult.INVALID) // 异常
            {
                sResult = "异常";
            }
            GameStatisticManager.GetInstance().DoStatPk(StatPKType.RESULT, sResult);
        }

        void _SetChildrenEnable(GameObject a_obj, bool a_bEnable)
        {
            for (int i = 0; i < a_obj.transform.childCount; ++i)
            {
                a_obj.transform.GetChild(i).gameObject.SetActive(a_bEnable);
            }
        }

        float _PlayAnims(DOTweenAnimation[] a_arrAnims)
        {
            float fTime = 0.0f;
            for (int i = 0; i < a_arrAnims.Length; ++i)
            {
                a_arrAnims[i].isActive = true;
                if (a_arrAnims[i].tween == null)
                {
                    a_arrAnims[i].CreateTween();
                }
                a_arrAnims[i].tween.Restart();

                float fTempTime = a_arrAnims[i].delay + a_arrAnims[i].duration;
                if (fTime < fTempTime)
                {
                    fTime = fTempTime;
                }
            }

            return fTime;
        }

        [UIEventHandle("Content/OkNode/Ok")]
        void _OnConfirmClicked()
        {
            if(m_msgRet!=null)
            {
                if (m_msgRet.pkType == (byte)PkType.Pk_Wudao)
                {
                    BudoManager.GetInstance().pkResult = m_ePKResult;
                    BudoManager.GetInstance().ReturnFromPk = true;
                }
            }

            ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = false; 
            
            if (m_ePKResult == PKResult.WIN && BattleDataManager.GetInstance().PkRaceType == RaceType.ChiJi) // 胜利
            {
                frameMgr.CloseFrame(this);
                ClientSystemManager.instance.SwitchSystem<ClientSystemGameBattle>();
            }
            else if (BattleDataManager.GetInstance().PkRaceType == RaceType.ChiJi)
            {
                ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = true;
                frameMgr.CloseFrame(this);
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
            else
            {
                frameMgr.CloseFrame(this);
                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>(null, null, false/*, targetSceneid*/);
            }
        }

        [UIEventHandle("Reporter")]
        void _OnReporterClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<PKReporterFrame>(FrameLayer.Middle);
        }

        #region ExtraUIBind
        private GameObject mDescNode = null;

        protected override void _bindExUI()
        {
            mDescNode = mBind.GetGameObject("DescNode");
        }

        protected override void _unbindExUI()
        {
            mDescNode = null;
        }
        #endregion

    }
}
