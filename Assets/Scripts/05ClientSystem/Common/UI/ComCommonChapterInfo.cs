using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using GameClient;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public enum eChapterNodeState
    {
        None,
        /// <summary>
        /// 没有这个难度
        /// </summary>
        Miss,
        /// <summary>
        /// 未解锁, 前置关卡评价不足
        /// </summary>
        Lock,
        /// <summary>
        /// 未解锁, 等级不足
        /// </summary>
        LockLevel,
        /// <summary>
        /// 解锁,未通关
        /// </summary>
        Unlock,
        /// <summary>
        /// 通关
        /// </summary>
        Passed,
    }

    public interface IChapterProcess
    {
        GameObject GetProcessRoot();

        void SetProcess(int id, int[] presList);
    }

    public interface IChapterInfoCommon
    {
        void SetName(string name);

        void SetDescription(string desc);

        void SetRecommnedLevel(string level);

        void SetRecommnedLevel(string[] level);

        void SetRecommnedWeapon(string weapon);

        void SetOpenTime(string opentime);
    }

    public interface IChapterInfoDrops
    {
        void SetDropList(IList<int> drops,int dungonID);

        void UpdateDropCount(List<ComItemList.Items> drops);
    }

    public interface IChapterInfoDrugs
    {
        void SetBuffDrugs(IList<int> drugs);
    }

    public interface IChapterPassReward
    {
        void SetExp(int num);

        void SetGold(int num);
    }

    public interface IChapterScore
    {
        GameObject GetScoreRoot();
            
        void SetHitCount(int cnt);

        void SetRebornCount(int cnt);

        void SetFightTime(int time);

        void SetPassed(bool isPass);

        void SetBestScore(Protocol.DungeonScore score);
    }

    public delegate void ChapterDiffCallback(int idx);

    public interface IChapterInfoDiffculte
    {
        GameObject GetDiffculteRoot();

        void SetActiveDiffculteByIdx(int idx, bool enable);

        void SetLevelLimite(int[] limit);

        void SetLevelDescription(string[] descs);
        
        void SetTopDiffculte(int top);

        int GetDiffculte();

        //添加一地下城ID参数，用来判断活动堕落深渊的特殊处理
        void SetDiffculte(int diff,int dungeonId);

        void SetDiffculteCallback(ChapterDiffCallback cb);

        void SetLock(bool isLock);
    }

    public interface IChapterNodeState
    {
        void SetChapterState(eChapterNodeState[] state, int[] limitLevel);

        void SetChapterScore(Protocol.DungeonScore[] score);
    }

    public interface IChapterMonsterInfo
    {
        void SetMonsterList(List<int> monsters);
    }

    public interface IChapterDungeonMap
    {
        void SetDungeonMap(IDungeonData data);
    }

    public interface IChapterConsume
    {
        void SetFatigueConsume(int value, bool isLimit, int dungonID);

        void SetHellConsume(string name, int value, string spritePath, bool ishell);
    }

    public interface IChapterActivityTimes
    {
        void SetActivityTimes(int id);
    }

    public interface IChapterMask
    {
        void SetChapterMask(int dungeonID);

        void SetBarState(int dungeonID);
    }

    public class ComCommonChapterInfo : MonoBehaviour, 
        IChapterInfoCommon, 
        IChapterInfoDiffculte,
        IChapterInfoDrops, 
        IChapterPassReward,
        IChapterScore,
        IChapterMonsterInfo,
        IChapterProcess,
        IChapterInfoDrugs,
        IChapterDungeonMap,
        IChapterNodeState,
        IChapterConsume,
        IChapterActivityTimes,
        IChapterMask
    {

#region IChapterActivityTimes implementation
        public ComCommonConsume mCommonConsumeResumeLeftTimes;
        public void SetActivityTimes(int id)
        {
            if (null != mCommonConsumeResumeLeftTimes)
            {
                mCommonConsumeResumeLeftTimes.SetData(ComCommonConsume.eType.Count, ComCommonConsume.eCountType.MouCount, id);
            }
        }
#endregion

#region IChapterConsume implementation
        public ComCommonBind mResBind;
        public ComCommonBind mConsume;

        [SerializeField] private Color mUnSelectColor;
        [SerializeField] private Color mSelectColor;

        private float mUnlockNormalAlpha = 0.6f;

        public void SetFatigueConsume(int value, bool isLimit, int dungonID)
        {
            GameObject root = mConsume.GetGameObject("fatigueroot");
            if (value > 0)
            {
                var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungonID);
                if (mDungeonTable == null)
                {
                    return;
                }

                GameObject mFatigueCombustionIcon = mConsume.GetGameObject("Icon1");

                bool mBisFlag = false;
                ActivityLimitTime.ActivityLimitTimeData data = null;
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.FindFatigueCombustionActivityIsOpen(ref mBisFlag, ref data);
                
                Text text = mConsume.GetCom<Text>("fatigue");
                if (isLimit)
                {
                    text.text = string.Format(">{0}", value);
                }
                else
                {
                    if (mDungeonTable.CostFatiguePerArea == value)
                    {
                        text.text = string.Format("{0}", value);
                    }
                    else
                    {
                        text.text = string.Format("{0}~{1}", mDungeonTable.CostFatiguePerArea, value);
                    }
                    mFatigueCombustionIcon.CustomActive(false);
                    if (mBisFlag && data != null && !TeamUtility.IsEliteDungeonID(dungonID)) // 精英地下城不参与精力燃烧，也是是说 精力消耗显示不会加倍
                    {
                        bool isFind = false;

                        for (int i = 0; i < data.activityDetailDataList.Count; i++)
                        {
                            if (data.activityDetailDataList[i].ActivityDetailState != ActivityLimitTime.ActivityTaskState.Finished)
                            {
                                continue;
                            }

                            isFind = true;
                        }

                        bool mBIsmainGate = false;

                        if (mDungeonTable.SubType != DungeonTable.eSubType.S_NORMAL)
                        {
                            mBIsmainGate = false;
                        }
                        else
                        {
                            mBIsmainGate = true;
                        }

                        if (isFind && mBIsmainGate)
                        {
                            mFatigueCombustionIcon.CustomActive(true);
                            if (mDungeonTable.CostFatiguePerArea * 2 == value * 2)
                            {
                                text.text = TR.Value("fatigue_combustion_fatiguedouble2", value * 2);
                            }
                            else
                            {
                                text.text = TR.Value("fatigue_combustion_fatiguedouble", mDungeonTable.CostFatiguePerArea * 2, value * 2);
                            }
                        }
                        
                    }
                }
                if(TeamUtility.IsEliteDungeonID(dungonID) && TeamDataManager.GetInstance().IsNotCostFatigueInEliteDungeon)
                {
                    text.text = "0";
                }

                root.SetActive(true);
            }
            else 
            {
                root.SetActive(false);
            }
        }
        //public void SetHellConsume(string stext, int value, Sprite sp, bool ishell)
        //{
        //    GameObject root = mConsume.GetGameObject("hellroot");
        //    Text       name = mConsume.GetCom<Text>("name");
        //    Image      img  = mConsume.GetCom<Image>("icon");

        //    name.text = stext;
        //    img.sprite = sp;

        //    if (value > 0)
        //    {
        //        Text text = mConsume.GetCom<Text>("hell");
        //        text.text = string.Format("{0}", value);
        //        root.SetActive(true);
        //    }
        //    else 
        //    {
        //        root.SetActive(false);
        //    }
        //}
        public void SetHellConsume(string stext, int value, string spritePath, bool ishell)
        {
            GameObject root = mConsume.GetGameObject("hellroot");
            Text name = mConsume.GetCom<Text>("name");
            Image img = mConsume.GetCom<Image>("icon");

            name.text = stext;
            // img.sprite = sp;
            if(null == spritePath || spritePath.Length <= 0)
            {
                img.sprite = null;
            }
            else
            {
                ETCImageLoader.LoadSprite(ref img, spritePath);
            }

            if (value > 0)
            {
                Text text = mConsume.GetCom<Text>("hell");
                text.text = string.Format("{0}", value);
                root.SetActive(true);
            }
            else
            {
                root.SetActive(false);
            }
        }
        #endregion

        #region IChapterNodeState implementation
        public ComCommonBind[] mDiffCommonBind = new ComCommonBind[0];

        Dictionary<int, bool> difficultyUnlockDic = new Dictionary<int, bool>(); //记录当前选中的难度是否解锁

        public void SetChapterState(GameClient.eChapterNodeState[] state, int[] limitLevel)
        {
            difficultyUnlockDic.Clear();
            bool unLock = false;

            for (int i = 0; i < mDiffCommonBind.Length; ++i)
            {
                var bind = mDiffCommonBind[i];

                GameObject scoreroot = bind.GetGameObject("scoreroot");
                GameObject failroot  = bind.GetGameObject("failroot");
                GameObject passroot  = bind.GetGameObject("passroot");
                GameObject diffimageroot = bind.GetGameObject("diffimageroot");
                GameObject reclevelroot = bind.GetGameObject("reclevelroot");
                GameObject levelRoot = bind.GetGameObject("levelRoot");

                CanvasGroup mNormalCanvas = bind.GetCom<CanvasGroup>("Normal");

                Image bg = bind.GetCom<Image>("bg");
                GameObject root = bind.GetGameObject("root");
                Text levelRootText = bind.GetCom<Text>("levelRootLevel");

                if (root != null)
                {
                    root.CustomActive(true);
                }
                if (scoreroot != null)
                {
                    scoreroot.CustomActive(false);
                }
                if (failroot != null)
                {
                    failroot.CustomActive(false);
                }
                if (passroot != null)
                {
                    passroot.CustomActive(false);
                }
                if (reclevelroot != null)
                {
                    reclevelroot.CustomActive(true);
                }
                if (levelRoot != null)
                {
                    levelRoot.CustomActive(false);
                }
                

                if (i < limitLevel.Length)
                {
                    if (levelRootText != null)
                    {
                        levelRootText.text = limitLevel[i].ToString();
                    }
                }

                if (mNormalCanvas != null)
                {
                    mNormalCanvas.alpha = 1f;
                }

                if (i < state.Length)
                {
                    switch (state[i])
                    {
                        case eChapterNodeState.Miss:
                            if (root != null)
                            {
                                root.CustomActive(false);
                            }
                            break;
                        case eChapterNodeState.Unlock:
                            if (failroot != null)
                            {
                                failroot.CustomActive(true);
                            }
                            unLock = true;
                            break;
                        case eChapterNodeState.Passed:
                            if (scoreroot != null)
                            {
                                scoreroot.CustomActive(true);
                            }
                            unLock = true;
                            break;
                        case eChapterNodeState.Lock:
                            //bg.sprite = mResBind.GetSprite("disablebg");
                            if (passroot != null)
                            {
                                passroot.CustomActive(true);
                            }

                            if (mNormalCanvas != null)
                            {
                                mNormalCanvas.alpha = mUnlockNormalAlpha;
                            }

                            unLock = false;
                            break;
                        case eChapterNodeState.LockLevel:
                            if (levelRoot != null)
                            {
                                levelRoot.CustomActive(true);
                            }

                            if (mNormalCanvas != null)
                            {
                                mNormalCanvas.alpha = mUnlockNormalAlpha;
                            }

                            unLock = false;
                            break;
                    }

                    difficultyUnlockDic.Add(i, unLock);
                }
            }
        }

        public void SetChapterScore(Protocol.DungeonScore[] score)
        {
            if (null == mResBind)
            {
                return ;
            }

            for (int i = 0; i < mDiffCommonBind.Length; ++i)
            {
                var bind = mDiffCommonBind[i];

                GameObject scoreImage0 = bind.GetGameObject("scoreImage0");
                GameObject scoreImage1 = bind.GetGameObject("scoreImage1");
                GameObject scoreImage2 = bind.GetGameObject("scoreImage2");

                scoreImage0.SetActive(false);
                scoreImage1.SetActive(false);
                scoreImage2.SetActive(false);

                //scoreImage0.GetComponent<Image>().sprite = bind.GetSprite("s");
                //scoreImage1.GetComponent<Image>().sprite = bind.GetSprite("s");
                //scoreImage2.GetComponent<Image>().sprite = bind.GetSprite("s");
                Image img0 = scoreImage0.GetComponent<Image>();
                Image img1 = scoreImage1.GetComponent<Image>();
                Image img2 = scoreImage2.GetComponent<Image>();
                bind.GetSprite("s", ref img0);
                bind.GetSprite("s", ref img1);
                bind.GetSprite("s", ref img2);

                if (i < score.Length)
                {
                    switch (score[i])
                    {
                        case Protocol.DungeonScore.SSS:
                            scoreImage0.SetActive(true);
                            scoreImage1.SetActive(true);
                            scoreImage2.SetActive(true);
                            break;
                        case Protocol.DungeonScore.SS:
                            scoreImage0.SetActive(true);
                            scoreImage1.SetActive(true);
                            break;
                        case Protocol.DungeonScore.S:
                            scoreImage0.SetActive(true);
                            break;
                        case Protocol.DungeonScore.A:
                        case Protocol.DungeonScore.B:
                        case Protocol.DungeonScore.C:
                            scoreImage0.SetActive(true);
                            // scoreImage0.GetComponent<Image>().sprite = bind.GetSprite("a");
                            Image image = scoreImage0.GetComponent<Image>();
                            bind.GetSprite("a", ref image);
                            break;
                    }
                }
            }
        }
#endregion

#region IChapterDungeonMap implementation
        public ComChapterDungeonMap mDungeonMap;
        public void SetDungeonMap(IDungeonData data)
        {
            if (null != mDungeonMap)
            {
                mDungeonMap.SetDungeonData(data);
            }
        }
#endregion

#region IChapterInfoDrugs implementation
        public ComChapterInfoDrug mChapterInfoDrug;
        public void SetBuffDrugs(IList<int> drugs)
        {
            if (null != mChapterInfoDrug)
            {
                mChapterInfoDrug.SetBuffDrugs(drugs);
            }
        }

        public GameObject mCostInfoRoot;
        public Text mCostInfoText;

        public void UpDateCost(bool isOn,DungeonID dungeonID)
        {
            mCostInfoRoot.CustomActive(isOn);
            if( null != mCostInfoText)
            {
                List<CostItemManager.CostInfo> costs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsCost(dungeonID.dungeonID);
                int totalCount = 0;
                for(int i = 0; i < costs.Count; i++)
                {
                    totalCount += costs[i].nCount;
                }
                if(totalCount == 0)
                {
                    mCostInfoRoot.CustomActive(false);
                }
                mCostInfoText.text = totalCount.ToString();
            }
        }

        public Text[] infoText;
        public void SetBuffDrugsInfo(IList<int> buffDrugList)
        {
            var attribute = BeUtility.GetMainPlayerActorAttribute();
            var beEntityData = BeUtility.GetMainPlayerActor().GetEntityData();
            var bData = beEntityData.battleData;
            if(buffDrugList.Count < 4)
            {
                return;
            }
            if (infoText.Length < 4)
            {
                return;
            }
            _setAttackInfo(infoText[0], attribute.attack, ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0]));
            _setAttackInfo(infoText[1], attribute.magicAttack, ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0]));
            _setHpInfo(infoText[2], bData, ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[1]));
            _setPercentInfo(infoText[3], ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[2]));
            _setPercentInfo(infoText[4], ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[3]));
        }

        void _setAttackInfo(Text infoText,float attack,bool isOn)
        {
            int itemID;
            int.TryParse(infoText.gameObject.name, out itemID);
            var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
            if (null == buffDrug) return;
            int buffID = buffDrug.OnUseBuffId;
            var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffID);
            if (null == buff) return;
            var baseAdd = buff.attack.fixValue;
            var percent = buff.attackAddRate.fixValue;
            if (isOn)
            {
                infoText.text = "+" + (_floatToInt(((attack + baseAdd) * (1 + (float)percent / GlobalLogic.VALUE_1000)) - attack)).ToString();
                infoText.color = mSelectColor;
            }
            else
            {
                infoText.text = TR.Value("chapter_value_string", 0);
                infoText.color = mUnSelectColor;
            }
        }

        void _setHpInfo(Text infoText,BattleData bData,bool isOn)
        {
            if (isOn)
            {
                int itemID;
                int.TryParse(infoText.gameObject.name, out itemID);
                var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
                if (null == buffDrug) return;
                int buffID = buffDrug.OnUseBuffId;
                var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffID);
                if (null == buff) return;
                var baseAdd = buff.maxHp.fixValue;
                var percent = buff.maxHpAddRate.fixValue;
                int baseHpBuff = _floatToInt(((bData.fMaxHp + baseAdd) * (1 + percent / (float)(GlobalLogic.VALUE_1000)) - bData.fMaxHp));
                int baseMaxHp1 = bData.fMaxHp;
                bData._maxHp += baseAdd;
                bData._maxHp += IntMath.Float2Int(bData._maxHp * (percent / (float)(GlobalLogic.VALUE_1000)));
                int actualHpBuff = bData.fMaxHp - baseMaxHp1;
                infoText.text = TR.Value("chapter_buffdrug_hpdisplay", baseHpBuff, actualHpBuff);
            }
            else
            {
                infoText.text = TR.Value("chapter_value_string", 0);
                infoText.color = mUnSelectColor;
            }
        }

        void _setPercentInfo(Text infoText,bool isOn)
        {
            int itemID;
            int.TryParse(infoText.gameObject.name, out itemID);
            var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
            if (null == buffDrug) return;
            int buffID = buffDrug.OnUseBuffId;
            var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffID);
            if (null == buff) return;
            var percent = buff.ciriticalAttack.fixValue == 0 ? buff.dodge.fixValue : buff.ciriticalAttack.fixValue;
            if (isOn)
            {
                infoText.text = TR.Value("chapter_percent_string", percent / 10);
                infoText.color = mSelectColor;
            }
            else
            {
                infoText.text = TR.Value("chapter_percent_string", 0);
                infoText.color = mUnSelectColor;
            }
        }

        int _floatToInt(float f)
        {
            int i = 0;
            if (f > 0) //正数
                i = (int)(f * 10 + 5) / 10;
            else if (f < 0) //负数
                i = (int)(f * 10 - 5) / 10;
            else i = 0;

            return i;

        }
        #endregion

        void Awake()
        {
            _initDiffculte();
        }

        #region Common
        public Text mName;
        public Text mDescription;
        public Text mRecommondLevel;
        public Text mRecommondWeapon;
        public Text mOpenTime;

        public void SetName(string name)
        {
            if (null != mName)
            {
                mName.text = name;
            }

            if (null != mResBind)
            {
                Text title = mResBind.GetCom<Text>("title");

                if (null != title)
                {
                    title.text = name;
                }
            }
        }

        public void SetDescription(string desc)
        {
            if (null != mDescription)
            {
                mDescription.text = desc;
            }
        }

        public void SetRecommnedLevel(string level)
        {
            if (null != mRecommondLevel)
            {
                mRecommondLevel.text = level;
            }
        }

        public void SetRecommnedLevel(string[] level)
        {
            for (int i = 0; i < mDiffCommonBind.Length; ++i)
            {
                var text = mDiffCommonBind[i].GetCom<Text>("reclevel");
                var textNormal = mDiffCommonBind[i].GetCom<Text>("reclevelNormal");

                if (i < level.Length)
                {
                    text.SafeSetText(level[i].ToString());
                    textNormal.SafeSetText(level[i].ToString());
                }
                else 
                {
                    text.SafeSetText("");
                    textNormal.SafeSetText("");
                }
            }
        }

        public void SetRecommnedWeapon(string weapon)
        {
            if (null != mRecommondWeapon)
            {
                mRecommondWeapon.text = weapon;
            }
        }

        public void SetOpenTime(string opentime)
        {
            if (null != mOpenTime)
            {
                //if (Utility.IsDateFullDay((uint)end, (uint)start))
                {
                    mOpenTime.text = opentime;
                }
                //else 
                //{
                //    mOpenTime.text = string.Format("{0}-{1}",
                //            Utility.ToUtcTime2Local(start).ToString("HH:mm"),
                //            Utility.ToUtcTime2Local(end).ToString("HH:mm"));
                //}
            }
        }
        #endregion

        #region Drops
        public ComItemList mComItemList;
        public ComChapterInfoDrop mComChapterInfoDrop;

        public void SetDropList(IList<int> drops,int dungonID)
        {
            if (null != mComItemList) mComItemList.SetItems(drops);
            if (null != mComChapterInfoDrop) mComChapterInfoDrop.SetDropList(drops, dungonID);
        }

        public void UpdateDropCount(List<ComItemList.Items> drops)
        {
            if (null != mComItemList) mComItemList.SetItems(drops.ToArray());
            //if (null != mComChapterInfoDrop) mComChapterInfoDrop.SetDropList(drops);

        }
        #endregion


        #region Diff
        public ChapterDiffCallback mCallback;
        public Toggle[] mDiffList = new Toggle[0];

        private void _initDiffculte()
        {
            for (int i = 0; i < mDiffList.Length; ++i)
            {
                var idx = i;

                //mDiffNameList[i].text = GameClient.ChapterUtility.GetHardString(i);

                mDiffList[i].onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        if (null != mCallback)
                        {
                            try
                            {
                                mCallback.Invoke(idx);
                            }
                            catch(Exception e)
                            {
                                Logger.LogErrorFormat("SetDiffculteCallback error e : {0}", e.ToString());
                            }
                        }
                    }
                });
            }
        }

        public GameObject mDiffRoot;
        public GameObject GetDiffculteRoot()
        {
            return mDiffRoot;
        }

        public void SetActiveDiffculteByIdx(int idx, bool enabled)
        {
            int count = mDiffList.Length;

            if (idx >= 0 && idx < count)
            {
                mDiffList[idx].gameObject.SetActive(enabled);
            }
        }


        public void SetDiffculteCallback(ChapterDiffCallback cb)
        {
            mCallback = cb;
        }


        public Text[] mAllDiffText = new Text[0];

        public void SetLevelLimite(int[] limit)
        {
            int i;

            for (i = 0; i < mAllDiffText.Length && i < limit.Length; ++i)
            {
                if (mAllDiffText[i] != null)
                {
                    mAllDiffText[i].text = string.Format("{0}", limit[i]);
                }
            }

            for (i = limit.Length; i < mAllDiffText.Length ; ++i)
            {
                if (mAllDiffText[i] != null)
                {
                    mAllDiffText[i].text = "";
                }
            }
        }

        public Text[] mAllDiffDesc = new Text[0];

        public void SetLevelDescription(string[] descs)
        {
            int i;

            for (i = 0; i < mAllDiffDesc.Length && i < descs.Length; ++i)
            {
                if (mAllDiffDesc[i] != null)
                {
                    mAllDiffDesc[i].text = descs[i];
                }
            }

            for (i = descs.Length; i < mAllDiffDesc.Length ; ++i)
            {
                if (mAllDiffDesc[i] != null)
                {
                    mAllDiffDesc[i].text = "";
                }
            }
        }

        public void SetTopDiffculte(int top)
        {
            int count = mDiffList.Length;
            if (count > 0)
            {
                top = Mathf.Clamp(top, 0, count - 1);

                for (int i = 0; i <= top; ++i)
                {
                    mDiffList[i].interactable = true;
                }

                for (int i = top + 1; i < count; ++i)
                {
                    mDiffList[i].interactable = false;
                }
            }
        }

        public int GetDiffculte()
        {
            for (int i = 0; i < mDiffList.Length; ++i)
            {
                if (mDiffList[i].isOn)
                {
                    return i;
                }
            }
            return 0;
        }

        public Text mDiffName;
        public Text mDiffExpRate;
        public Text mDiffGlodRate;

        /// <summary>
        /// 之前需要解锁的评价
        /// </summary>
        public Text mDiffPassUnlock;
        public Text mDiffLastDiff;

        public Text[] mDiffNameList;


        public string[] mDiffUnlockScore;


        


        private int _getExpRate(int diff)
        {
            int[] arr = { 0, 10, 20, 50 };
            return arr[diff % 4];
        }

        private int _getGlodRate(int diff)
        {
            int[] arr = { 0, 5, 10, 20 };
            return arr[diff % 4];
        }

        public void SetDiffculte(int diff,int dungeonId)
        {
            int count = mDiffList.Length;
            if (diff < count && diff >= 0)
            {
                mDiffList[diff].isOn = true;
            }

            UpdateDiffInfo(dungeonId);
        }

        public GameObject mDiffUnlock;
        public GameObject mDiffLock;

        public void SetLock(bool isLock)
        {
            if (mDiffUnlock)
                mDiffUnlock.SetActive(!isLock);
            if (mDiffLock)
                mDiffLock.SetActive(isLock);
        }

        public void UpdateDiffInfo(int dungeonId)
        {
            for (int i = 0; i < mDiffList.Length; ++i)
            {
                if (mDiffList[i].isOn)
                {
                    if (null != mDiffName)
                        mDiffName.text = mDiffNameList[i].text;
                    //如果是活动堕落深渊特殊处理，比较特殊它只有王者关卡，之前流程掉落加成和经验加成是按照普通到王者顺序显示的，这里处理成显示王者加成信息
                    if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(dungeonId))
                    {
                        if (null != mDiffExpRate)
                            mDiffExpRate.text = _getExpRate(i).ToString();
                        if (null != mDiffGlodRate)
                            mDiffGlodRate.text = 200.ToString();
                    }
                    else
                    {
                        if (null != mDiffExpRate)
                            mDiffExpRate.text = _getExpRate(i).ToString();
                        if (null != mDiffGlodRate)
                            mDiffGlodRate.text = _getGlodRate(i).ToString();
                    }

                    if (null != mDiffLastDiff && i > 0)
                        mDiffLastDiff.text = mDiffNameList[i - 1].text;
                    if (null != mDiffPassUnlock)
                        mDiffPassUnlock.text = mDiffUnlockScore[i];

                    if (groupStartEffectGameObject != null)
                    {
                        bool unLock = false;
                        difficultyUnlockDic.TryGetValue(i, out unLock);
                        groupStartEffectGameObject.CustomActive(unLock);
                    }
                }
            }
        }
        #endregion

        #region Item
        private void _setItem(Battle.DungeonItem.eType type, ComItem item, int num)
        {
            var itemconfig = TableManager.instance.GetTableItem<ItemConfigTable>((int)type);
            var dropId = itemconfig.ItemID;

            ItemData dropData = ItemDataManager.CreateItemDataFromTable(dropId);
            if (dropData != null)
            {
                dropData.CanSell = false;
                dropData.UseType = ProtoTable.ItemTable.eCanUse.CanNot;
                dropData.Count = num;

                item.SetActive(true);
                item.Setup(dropData, (go, data) =>
                {
                    ItemTipManager.GetInstance().ShowTip(data);
                });
            }
            else
            {
                Logger.LogError("DropItem not found with id : " + dropId);
            }
        }

        public ComItem mExp;
        public void SetExp(int num)
        {
            if (null != mExp)
            {
                _setItem(Battle.DungeonItem.eType.Exp, mExp, num);
            }
        }

        public ComItem mGlod;
        public void SetGold(int num)
        {
            if (null != mGlod)
            {
                _setItem(Battle.DungeonItem.eType.Glod, mGlod, num);
            }
        }

        public Text mHitCount;
        public void SetHitCount(int cnt)
        {
            if (null != mHitCount)
            {
                mHitCount.text = cnt.ToString();
            }
        }

        public Text mRebornCount;
        public void SetRebornCount(int cnt)
        {
            if (null != mRebornCount)
            {
                mRebornCount.text = cnt.ToString();
            }
        }

        public ComTime mFightTime;
        public void SetFightTime(int time)
        {
            if (null != mFightTime)
            {
                mFightTime.SetTime(time);
            }
        }

        public void SetPassed(bool isPassed)
        {
            return ;
        }

        public ComChapterDungeonScore mScore;
        public void SetBestScore(Protocol.DungeonScore score)
        {
            if (null != mScore)
            {
                mScore.SetScore(score);
            }
        }

        public GameObject mScoreRoot;
        public GameObject GetScoreRoot()
        {
            return mScoreRoot;
        }
        #endregion

        #region MonsterInfo
        public ComMonsterItem[] mMonsterList;
        public void SetMonsterList(List<int> monsters)
        {
			List<int> uniqueMonsters = new List<int>();
			for(int i=0; i<monsters.Count; ++i)
			{
				if (!uniqueMonsters.Contains(monsters[i]))
				{
					var item = TableManager.instance.GetTableItem<UnitTable>(monsters[i]);
					if (item != null && item.IsShowPortrait > 0)
					{
						if (item.Type == UnitTable.eType.BOSS)
							uniqueMonsters.Insert(0, monsters[i]);
						else
							uniqueMonsters.Add(monsters[i]);
					}
				}
			}

			var len = uniqueMonsters.Count - 1;

            if (len < 0)
            {
                return;
            }

			for (int i = 0, j=0; i < mMonsterList.Length; ++i)
            {
				if (j>=uniqueMonsters.Count)
				{
					mMonsterList[i].SetVisible(false);
					continue;
				}
				mMonsterList[i].SetMonster(uniqueMonsters[j++]);

            }
        }
        #endregion

        #region IChapterProcess implementation
        public ComCommonBind mProcessBind;

        public GameObject GetProcessRoot()
        {
            if (null != mProcessBind && null != mProcessBind.transform.parent)
            {
                return mProcessBind.transform.parent.gameObject;
            }

            return null;
        }

        private const string kProcessNodePath = "UI/Prefabs/Chapter/ChapterNormalProcessUnit";


        //private void _loadProcessUnit(GameObject root, Sprite sprite, string name, bool showFlag = false)
        //{
        //    GameObject go = AssetLoader.instance.LoadResAsGameObject(kProcessNodePath);
        //    Utility.AttachTo(go, root);

        //    ComCommonBind bd = go.GetComponent<ComCommonBind>();

        //    if (null != bd)
        //    {
        //        Image image  = bd.GetCom<Image>("image");
        //        image.sprite = sprite;
        //        image.SetNativeSize();

        //        Text text    = bd.GetCom<Text>("num");
        //        text.text    = name;

        //        GameObject current = bd.GetGameObject("current");
        //        current.SetActive(showFlag);
        //    }
        //}
        private void _loadProcessUnit(GameObject root, string spriteName, string name, bool showFlag = false)
        {
            GameObject go = AssetLoader.instance.LoadResAsGameObject(kProcessNodePath);
            Utility.AttachTo(go, root);

            ComCommonBind bd = go.GetComponent<ComCommonBind>();

            if (null != bd)
            {
                Image image = bd.GetCom<Image>("image");
                // image.sprite = sprite;
                mProcessBind.GetSprite(spriteName, ref image);
                image.SetNativeSize();

                Text text = bd.GetCom<Text>("num");
                text.text = name;

                GameObject current = bd.GetGameObject("current");
                current.SetActive(showFlag);
            }
        }

        private void _loadProcessGap(GameObject root)
        {
            GameObject em     = new GameObject();
            LayoutElement le  = em.AddComponent<LayoutElement>();
            le.flexibleWidth  = 1.0f;
            le.flexibleHeight = 1.0f;

            Utility.AttachTo(em, root);
        }

        private string _getShowSprite(ComChapterDungeonUnit.eState state, bool flag)
        {
            switch (state)
            {
                case ComChapterDungeonUnit.eState.Passed:
                case ComChapterDungeonUnit.eState.LockPassed:
                    return flag ? "normalpass" : "prepass";
                default:
                    return flag ? "normallock" : "prelock";
            }
        }

        public void SetProcess(int id, int[] presList)
        {
            if (presList == null)
            {
                presList = new int[0];
            }

            DungeonID did = new DungeonID(id);
            did.diffID = 0;

            int selectedIndex = presList.Length;

            for (int i = 0; i < presList.Length; ++i)
            {
                if (presList[i] == did.dungeonID)
                {
                    selectedIndex = i;
                }
            }
                
            if (null != mProcessBind)
            {
                GameObject root = mProcessBind.GetGameObject("root");

                for (int i = 0; i < presList.Length; ++i)
                {
                    ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(presList[i]);

                    string presprite = _getShowSprite(state, selectedIndex == i);

                    // _loadProcessUnit(root, mProcessBind.GetSprite(presprite), string.Format("{0}", i+1), selectedIndex == i);
                    _loadProcessUnit(root, presprite, string.Format("{0}", i + 1), selectedIndex == i);
                    _loadProcessGap(root);
                }

                did.prestoryID = 0;
                ComChapterDungeonUnit.eState fstate = ChapterUtility.GetDungeonState(did.dungeonID);
                string normalsprite = _getShowSprite(fstate, selectedIndex == presList.Length);

                // _loadProcessUnit(root, mProcessBind.GetSprite(normalsprite), string.Format("{0}", presList.Length+1), selectedIndex == presList.Length);
                _loadProcessUnit(root, normalsprite, string.Format("{0}", presList.Length + 1), selectedIndex == presList.Length);

                Slider slider = mProcessBind.GetCom<Slider>("slider");
                slider.value  = selectedIndex * 1.0f / presList.Length;
            }
        }
        #endregion

        #region IChapterMask implementation
        public void SetChapterMask(int dungeonID)
        {
            DungeonID id = new DungeonID(dungeonID);
            if (_isAllDiffNoScores(dungeonID) && ChapterUtility.PreconditionIDList(id.dungeonIDWithOutPrestory).Count != 0) 
            {
                if (!PlayerPrefs.HasKey(_getChapterOnceFinishBossString(id.dungeonIDWithOutDiff)))
                {
                    PlayerPrefs.SetInt(_getChapterOnceFinishBossString(id.dungeonIDWithOutDiff), 1);
                }
                _showChapterMask(dungeonID, true);
            }
            else
            {
                if (PlayerPrefs.HasKey(_getChapterOnceFinishBossString(id.dungeonIDWithOutDiff)))
                {
                    //Logger.LogError("播放合并拼图特效！美术资源呢！");
                    _showChapterMask(dungeonID, false);
                    StartCoroutine(_playFinishEffect());
                    PlayerPrefs.DeleteKey(_getChapterOnceFinishBossString(id.dungeonIDWithOutDiff));
                }
            }
        }

        void _showChapterMask(int dungeonID,bool isShowNewEffect)
        {
            DungeonID id = new DungeonID(dungeonID);
            IList<int> preDungeonID = ChapterUtility.PreconditionIDList(id.dungeonIDWithOutPrestory);
            int chapterMaskCount = preDungeonID.Count + 1;
            int div = preDungeonID.Count / 4;
            int dungeonIndex = preDungeonID.IndexOf(dungeonID) + 1;
            if (dungeonIndex >= 1 && dungeonIndex <= div * 4)
            {
                int startIndexDiv = (dungeonIndex - 1) / 4;
                int startIndex = startIndexDiv * 4 + 1;
                int displayIndex = dungeonIndex - 4 * startIndexDiv;
                _showMaskItem(4, startIndex, displayIndex, false, isShowNewEffect);
            }
            else
            {
                int type = chapterMaskCount - 4 * div;
                int startIndex = 4 * div + 1;
                int displayIndex;
                if(dungeonIndex == 0)
                {
                    displayIndex = type;
                }
                else
                {
                    displayIndex = dungeonIndex % 4;
                }
                _showMaskItem(type, startIndex, displayIndex, true, isShowNewEffect);
            }
        }

        void _hideChapterMask()
        {
            //Logger.LogError("隐藏Mask");
            Image mBackgroundGray = mResBind.GetCom<Image>("BackgroundGray");
            mBackgroundGray.gameObject.CustomActive(false);
            UIGray mBackgroundUIGray = mResBind.GetCom<UIGray>("BackgroundUIGray");
            mBackgroundUIGray.gameObject.CustomActive(false);
            RectTransform mChapterMask = mResBind.GetCom<RectTransform>("ChapterMask");
            mChapterMask.gameObject.CustomActive(false);
            for (int i = 0; i < mBarList.Length; i++)
            {
                mBarList[i].bar.CustomActive(false);
            }
        }
        [SerializeField] private float barDieTime = 0;
        [SerializeField] private float pauseTime = 0;
        IEnumerator _playFinishEffect()
        {
            LoadEffect("Effects/UI/Prefab/EffUI_juqingguanka/Prefab/EffUI_juqingguanka_hetu", mFinishPuzzleEffect);
            mFinishPuzzleEffect.CustomActive(true);
            mRefuse.CustomActive(true);
            yield return new WaitForSeconds(barDieTime > 0 ? barDieTime : 0);
            _hideChapterMask();
            yield return new WaitForSeconds(pauseTime - barDieTime > 0 ? pauseTime - barDieTime : 0);
            mFinishPuzzleEffect.CustomActive(false);
            mRefuse.CustomActive(false);
        }

        enum PuzzleType
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
        }

        [Serializable]
        public struct ChapterPuzzle
        {
            public GameObject bar;
            public Text[] numList;
            //public Text[] nowList;
            public GameObject[] posList;
        }

        [SerializeField] private ChapterPuzzle[] mBarList;

        [SerializeField] private GameObject mFinishPuzzleEffect;
        [SerializeField] private GameObject mRefuse;
        
        private const string nowEffectPath = "UI/UIEffects/Skill_UI_ChapterSelect/Prefab/Skill_UI_ChapterSelect_kuang";
        /// <summary>
        /// 关卡遮罩显示函数
        /// </summary>
        /// <param name="type">遮罩类型</param>
        /// <param name="startIndex">遮罩第一张图片索引</param>
        /// <param name="dungeonIndex">当前显示第几张拼图</param>
        /// <param name="showBoss">是否显示boss图片并将最后一张索引改为boss</param>
        void _showMaskItem(int type, int startIndex, int dungeonIndex, bool showBoss, bool isShowNewEffect)
        {
            Image mBackground = mResBind.GetCom<Image>("Background");
            Image mCharactorIcon = mResBind.GetCom<Image>("CharactorIcon");
            Image mBackgroundGray = mResBind.GetCom<Image>("BackgroundGray");
            Image mCharactorGray = mResBind.GetCom<Image>("CharactorGray");
            UIGray mBackgroundUIGray = mResBind.GetCom<UIGray>("BackgroundUIGray");
            RectTransform mChapterMask = mResBind.GetCom<RectTransform>("ChapterMask");

            mBackgroundUIGray.enabled = false;
            mBackgroundGray.sprite = mBackground.sprite;
            mBackgroundGray.material = mBackground.material;
            mCharactorGray.sprite = mCharactorIcon.sprite;
            mCharactorGray.material = mCharactorIcon.material;
            mCharactorGray.rectTransform.sizeDelta = mCharactorIcon.rectTransform.sizeDelta;
            mBackgroundUIGray.enabled = true;

            //更改Mask形状
            PuzzleType mPuzzleType = (PuzzleType)type;
            mChapterMask.gameObject.CustomActive(true);
            Utility.AttachTo(mBackground.gameObject, mBackgroundGray.transform.parent.gameObject);
            float maxWidth = mBackground.rectTransform.sizeDelta.x;
            float maxHeight = mBackground.rectTransform.sizeDelta.y;
            string effectPath = nowEffectPath;

            switch (mPuzzleType)
            {
                case PuzzleType.None:
                    mChapterMask.gameObject.CustomActive(false);
                    break;
                case PuzzleType.One:
                    mChapterMask.gameObject.CustomActive(false);
                    break;
                case PuzzleType.Two:
                    if (1 == dungeonIndex) 
                    {
                        mChapterMask.sizeDelta = new Vector2(maxWidth / 2, maxHeight);
                        mChapterMask.anchoredPosition = new Vector3(maxWidth / 4, -maxHeight / 2, 0);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (2 == dungeonIndex) 
                    {
                        mChapterMask.gameObject.CustomActive(false);
                    }
                    break;
                case PuzzleType.Three:
                    if (1 == dungeonIndex) 
                    {
                        mChapterMask.sizeDelta = new Vector2(maxWidth / 2, maxHeight / 2);
                        mChapterMask.anchoredPosition = new Vector3(maxWidth / 4, -maxHeight / 4);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (2 == dungeonIndex) 
                    {
                        mChapterMask.sizeDelta = new Vector2(maxWidth / 2, maxHeight);
                        mChapterMask.anchoredPosition = new Vector2(maxWidth / 4, -maxHeight / 2);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (3 == dungeonIndex) 
                    {
                        mChapterMask.gameObject.CustomActive(false);
                    }
                    if (type != dungeonIndex) 
                    {
                        effectPath += "2";
                    }
                    break;
                case PuzzleType.Four:
                    if (1 == dungeonIndex) 
                    {
                        mChapterMask.sizeDelta = new Vector2((maxWidth * 89) / 456, maxHeight);
                        mChapterMask.anchoredPosition = new Vector3((maxWidth * 44) / 456, -maxHeight / 2);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (2 == dungeonIndex)
                    {
                        mChapterMask.sizeDelta = new Vector2((maxWidth * 185) / 456, maxHeight);
                        mChapterMask.anchoredPosition = new Vector3((maxWidth * 93) / 456, -maxHeight / 2);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (3 == dungeonIndex) 
                    {
                        mChapterMask.sizeDelta = new Vector2((maxWidth * 278) / 456, maxHeight);
                        mChapterMask.anchoredPosition = new Vector3((maxWidth * 139) / 456, -maxHeight / 2);
                        Utility.AttachTo(mBackground.gameObject, mChapterMask.gameObject);
                    }
                    if (4 == dungeonIndex) 
                    {
                        mChapterMask.gameObject.CustomActive(false);
                    }
                    if (type != dungeonIndex)
                    {
                        effectPath += "3";
                    }
                    else
                    {
                        effectPath += "4";
                    }
                    break;
                default:
                    mChapterMask.gameObject.CustomActive(false);
                    break;
            }
            //当前进度加流光特效
            if (PuzzleType.None != mPuzzleType && PuzzleType.One != mPuzzleType && null != mBarList[type - 1].posList[dungeonIndex - 1] && isShowNewEffect) 
            {
                LoadEffect(effectPath, mBarList[type - 1].posList[dungeonIndex - 1]);
            }
            //设置边框 、索引 、now
            for(int i = 0; i < mBarList.Length; ++i)
            {
                if (type != i + 1) 
                {
                    mBarList[i].bar.CustomActive(false);
                }
                else
                {
                    mBarList[i].bar.CustomActive(true);
                    for(int j = 0; j < type; ++j)
                    {
                        mBarList[i].numList[j].text = (startIndex + j).ToString();
                        if (type == j + 1 && showBoss && null != mBarList[i].posList[j] && isShowNewEffect)  
                        {
                            //mBarList[i].numList[j].text = TR.Value("chapter_Boss");
                            LoadEffect("Effects/UI/Prefab/EffUI_juqingguanka/Prefab/map_boss", mBarList[i].posList[j]);
                        }
                        if (dungeonIndex == j + 1 && null != mBarList[i].posList[j] && isShowNewEffect)  
                        {
                            LoadEffect("Effects/UI/Prefab/EffUI_juqingguanka/Prefab/player_main", mBarList[i].posList[j]);
                        }
                        //else
                        //{
                        //    mBarList[i].nowList[j].text = "";
                        //}
                    }
                }
            }

            if (!showBoss)
            {
                mCharactorIcon.gameObject.CustomActive(false);
                mCharactorGray.gameObject.CustomActive(false);
            }
            //Logger.LogError(string.Format("显示第{0}类遮罩，拼图开始索引{1}，显示拼图索引{2}，显示boss",type,startIndex,dungeonIndex,showBoss));
        }
        
        private void LoadEffect(string effectPath, GameObject parent)
        {
            if(string.IsNullOrEmpty(effectPath) || null == parent)
            {
                return;
            }

            GameObject effect = AssetLoader.instance.LoadResAsGameObject(effectPath);

            if(effect != null)
            {
                Utility.AttachTo(effect, parent);

            }
        }

        public void SetBarState(int diffID)
        {
            GameObject mBarListRoot = mResBind.GetGameObject("BarListRoot");
            if(null != mBarListRoot)
            {
                if (diffID == 0)
                {
                    mBarListRoot.CustomActive(true);
                }
                else
                {
                    mBarListRoot.CustomActive(false);
                }
            }
        }

      public static bool _isAllDiffNoScores(int dungeonID)
        {
            DungeonID id = new DungeonID(dungeonID);

            int tophard = ChapterUtility.GetDungeonTopHard(dungeonID);
            for(int i = 0; i <= tophard; i++)
            {
                id.diffID = i;
                if (ChapterUtility.GetDungeonBestScore(id.dungeonID) != DungeonScore.C)
                {
                    return false;
                }
            }
            return true;
        }

        string _getChapterOnceFinishBossString(int dungeonID)
        {
            string serverID = "";
            string roleID = "";
            if (null != ClientApplication.playerinfo)
            {
                serverID = ClientApplication.playerinfo.serverID.ToString();
            }
            if (null != PlayerBaseData.GetInstance())
            {
                roleID = PlayerBaseData.GetInstance().RoleID.ToString();
            }
            return TR.Value("chapter_has_prestory_and_once_finish_boss", serverID, roleID, dungeonID);
        }
        #endregion

        #region  开始挑战按钮下特效
        public GameObject groupStartEffectGameObject;
#endregion
    }
   
}

