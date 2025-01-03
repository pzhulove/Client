using DataModel;
using ProtoTable;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 开奖界面
        /// </summary>
        public class ActivityTreasureLotteryDrawView : MonoBehaviour, IDisposable
        {
            GameObject mEffectChiXu;
            GameObject mEffectZhongJiang;
            GameObject mEffectWeiZhongJiang;
            ComItem mComItem = null;

            #region serialize field
            [SerializeField]
            Text mTextName;
            [SerializeField]
            CanvasGroup mCanvasName;

            [SerializeField]
            CanvasGroup mCanvasResultBingo;

            [SerializeField]
            CanvasGroup mCanvasResultNoBingo;

            [SerializeField]
            ComUIListScript mComUIList;

            //[SerializeField]
            //Text mTextRates;

            [SerializeField]
            private Transform mComItemRoot;

            [SerializeField]
            [Header("控制显示前几名的玩家数据,默认前5")]
            int mShowPlayerRateCount = 5;

            [SerializeField]
            [Header("前几名中奖玩家名字和中奖概率间的总字符数")]
            int mShowPlayerRateSpacesCount = 14;

            [Header("名字随机的变量")]
            [SerializeField]
            [Tooltip("名字随机的时间")]
            float mAnimationTime = 3;

            [SerializeField]
            [Tooltip("名字随机的频率")]
            float mAnimtaionRate = 0.02f;

            [SerializeField]
            [Tooltip("特效播放延迟的时间")]
            float mEffectDelay = 0.1f;

            [SerializeField]
            string mEffectChiXuPrefabPath;

            [SerializeField]
            string mEffectZhongJiangPrefabPath;

            [SerializeField]
            string mEffectWeiZhongJiangPrefabPath;
            #endregion

            IActivityTreasureLotteryDrawModel mModel;
            float mDelta = 0;
            int mShowId = 0;

            private bool mIsInited = false;

            public void Init(IActivityTreasureLotteryDrawModel model, bool isRandomName = true)
            {
                if (mIsInited == false)
                {
                    mComUIList.Initialize();
                    mComUIList.onItemVisiable = _OnItemVisable;
                    mComUIList.OnItemUpdate = _OnItemVisable;

                    mIsInited = true;
                }

                mDelta = 0;
                mShowId = 0;
                mModel = model;
                InitEffects();
                if (isRandomName)
                {
                    SetName(0);
                    InvokeRepeating("UpdateName", mAnimtaionRate, mAnimtaionRate);
                }
                else
                {
                    ShowResult();
                }

                if (mModel == null)
                {
                    Logger.LogError("model is null!");
                    return;
                }

                int maxNum = Mathf.Min(mShowPlayerRateCount, mModel.TopFiveInvestPlayers.Length);
                string rateText = TR.Value("activity_treasure_lottery_draw_winner_info").Replace("\\n", "\n");
                string winnerInfos = "";

                mComUIList.SetElementAmount(maxNum);
                //for (int i = 0; i < maxNum; ++i)
                //{
                //    winnerInfos += string.Format(rateText, i + 1, mModel.TopFiveInvestPlayers[i].Name, mModel.TopFiveInvestPlayers[i].Rate, mModel.TopFiveInvestPlayers[i].ServerName);
                //}
                //mTextRates.SafeSetText(winnerInfos);
                if (mComItem == null && mComItemRoot != null)
                {
                    mComItem = ComItemManager.Create(mComItemRoot.gameObject);
                }
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mModel.ItemId);
                mComItem.Setup(itemData, ShowItemTip);
            }

            private void _OnItemVisable(ComUIListElementScript item)
            {
                if (item == null)
                {
                    return;
                }

                if (mModel == null || mModel.TopFiveInvestPlayers == null)
                {
                    return;
                }

                if (item.m_index >= mModel.TopFiveInvestPlayers.Length)
                {
                    return;
                }

                var script = item.GetComponent<ActivityTreasureLotteryRankItem>();
                if (script != null)
                {
                    script.Init(mModel, item.m_index);
                }
            }

            public void Dispose()
            {
                CancelInvoke();
                mCanvasResultBingo.CustomActive(false);
                mCanvasResultNoBingo.CustomActive(false);
                if (mEffectWeiZhongJiang != null)
                {
                    mEffectWeiZhongJiang.CustomActive(false);
                }
                if (mEffectZhongJiang != null)
                {
                    mEffectZhongJiang.CustomActive(false);
                }
                ComItemManager.Destroy(this.mComItem);
                mComItem = null;
                //mTextWinner.SafeSetText(TR.Value("activity_treasure_winner"));
            }


            void ShowItemTip(GameObject go, ItemData itemData)
            {
                if (null != itemData)
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            }

            void InitEffects()
            {
                if (mEffectChiXu == null)
                {
                    mEffectChiXu = AssetLoader.instance.LoadResAsGameObject(mEffectChiXuPrefabPath);
                    if (mEffectChiXu != null)
                    {
                        Utility.AttachTo(mEffectChiXu, gameObject);
                    }
                }
            }

            void UpdateName()
            {
                mDelta += mAnimtaionRate;
                if (mDelta >= mAnimationTime)
                {
                    //结束 显示中奖者
                    CancelInvoke("UpdateName");
                    ShowResult();
                }
                else
                {
                    if (mModel != null && mModel.TopFiveInvestPlayers != null)
                    {
                        mShowId = mShowId + 1 >= mModel.TopFiveInvestPlayers.Length ? 0 : mShowId + 1;
                    }
                    SetName(mShowId);
                }
            }

            void ShowResult()
            {
                mCanvasName.CustomActive(mModel != null);
                if (mModel != null)
                {
                    string rateText = TR.Value("activity_treasure_lottery_draw_rand_text").Replace("\\n", "\n");
                    mTextName.SafeSetText(string.Format(rateText, mModel.WinnerName, mModel.WinnerRate, this.mModel.PlatformName, this.mModel.ServerName));
                    //mTextWinner.SafeSetText(string.Format(TR.Value("activity_treasure_winner"), mModel.WinnerName + "  " + mModel.WinnerRate + "%").Replace("\\n", "\n"));

                    mCanvasResultNoBingo.CustomActive(!mModel.IsPlayerWin);
                    mCanvasResultBingo.CustomActive(mModel.IsPlayerWin);

                    if (mModel.IsPlayerWin)
                    {
                        if (mEffectZhongJiang == null)
                        {
                            mEffectZhongJiang = AssetLoader.instance.LoadResAsGameObject(mEffectZhongJiangPrefabPath);
                            if (mEffectZhongJiang != null)
                            {
                                Utility.AttachTo(mEffectZhongJiang, gameObject);
                            }
                        }
                        mEffectZhongJiang.CustomActive(false);
                    }
                    else
                    {
                        if (mEffectWeiZhongJiang == null)
                        {
                            mEffectWeiZhongJiang = AssetLoader.instance.LoadResAsGameObject(mEffectWeiZhongJiangPrefabPath);
                            if (mEffectWeiZhongJiang != null)
                            {
                                Utility.AttachTo(mEffectWeiZhongJiang, gameObject);
                            }
                        }
                        mEffectWeiZhongJiang.CustomActive(false);
                    }
                }
                //InvokeMethod.Invoke(mEffectDelay, ShowResultEffect);
            }

            void ShowResultEffect()
            {

                if (mEffectZhongJiang != null && mModel != null)
                {
                    if (mModel.IsPlayerWin)
                    {
                        mEffectZhongJiang.CustomActive(true);
                    }
                }
                else if(mEffectWeiZhongJiang != null && mModel != null)
                {
                    if (!mModel.IsPlayerWin)
                    {
                        mEffectWeiZhongJiang.CustomActive(true);
                    }
                }
            }

            void OnDestroy()
            {
                InvokeMethod.RemoveInvokeCall(ShowResultEffect);
            }

            void SetName(int id)
            {
                mCanvasName.CustomActive(mModel != null && mModel.TopFiveInvestPlayers != null);
                if (mModel != null && mModel.TopFiveInvestPlayers != null && id >= 0 && id < mModel.TopFiveInvestPlayers.Length)
                {
                    mCanvasName.CustomActive(true);
                    string rateText = TR.Value("activity_treasure_lottery_draw_rand_text").Replace("\\n", "\n");
                    mTextName.SafeSetText(string.Format(rateText, mModel.TopFiveInvestPlayers[id].Name, mModel.TopFiveInvestPlayers[id].Rate, this.mModel.TopFiveInvestPlayers[id].PlatformName, this.mModel.TopFiveInvestPlayers[id].ServerName));
                }
                else
                {
                    mCanvasName.CustomActive(false);
                }
            }
        }
    }
}