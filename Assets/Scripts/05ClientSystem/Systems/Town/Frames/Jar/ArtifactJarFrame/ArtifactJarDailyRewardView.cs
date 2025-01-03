using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class ArtifactJarDailyRewardView : MonoBehaviour
    {
        [SerializeField]
        private Button closeBtn;
        [SerializeField]
        private Text mDes;
        [SerializeField]
        private ComUIListScript mToggleComUIList;
        [SerializeField]
        private ComUIListScript mRecordComUIList;
        [Space(5)]
        [SerializeField]
        private Button mLeftBtn;
        [SerializeField]
        private Button mRightBtn;
        [SerializeField]
        private Image mJarIcon;
        [SerializeField]
        private Text mJarName;
        [SerializeField]
        private Text mJarTips;
        [SerializeField]
        private Text mJarCount;
        [SerializeField]
        private GameObject mJarFinished;
        [SerializeField]
        private GameObject mFinished;
        [SerializeField]
        private GameObject mFinishedNeedHide;
        [SerializeField]
        private GameObject mShowStateGo;
        [SerializeField]
        private Button mPreviewReward;
        [SerializeField]
        private Button mBoxBtn;
        [Space(5)]
        [SerializeField]
        private Text mRecordTitleText;
        [SerializeField]
        private GameObject mEmptyTips;
        private ArtifactJarToggleView[] jarToggleDataArr; 
        private List<ArtifactJarBuy> allJarData = new List<ArtifactJarBuy>();//所有罐子
        private Dictionary<int, List<ArtifactJarLotteryTable>> jarRewardDic = new Dictionary<int, List<ArtifactJarLotteryTable>>();//罐子id对应的奖励链表
        private Dictionary<int, List<ArtifactJarLotteryRecord>> artifactRecordDic = new Dictionary<int, List<ArtifactJarLotteryRecord>>();//罐子id对应公告链表
        private int mCurJarId;
        private int mCurToggleIndex = 0;
        public void InitView()
        {
            _InitComUIList();
            _InitToggle();
            _InitActivityUI();
            jarToggleDataArr[mCurToggleIndex].SetToggleIsOn(true);
        }
        /// <summary>
        /// 刷新派奖记录
        /// </summary>
        /// <param name="jarId"></param>
        public void UpdateRecord(int jarId)
        {
            if (jarId == mCurJarId)
            {
                artifactRecordDic = ArtifactDataManager.GetInstance().getArtifactRecordDic();
                if (artifactRecordDic.ContainsKey(jarId))
                {
                    if (artifactRecordDic[jarId].Count == 0)
                    {
                        mEmptyTips.CustomActive(true);
                    }
                    else
                    {
                        mEmptyTips.CustomActive(false);
                    }
                    mRecordComUIList.SetElementAmount(artifactRecordDic[jarId].Count);
                }
                else
                {
                    mRecordComUIList.SetElementAmount(0);
                    mEmptyTips.CustomActive(true);
                }
            }
        }
        void _InitToggle()
        {
            allJarData = ArtifactDataManager.GetInstance().getArtiFactJarBuyData();
            jarToggleDataArr = new ArtifactJarToggleView[allJarData.Count];
            mToggleComUIList.SetElementAmount(allJarData.Count);
        }
        void _InitComUIList()
        {
            mToggleComUIList.Initialize();
            mToggleComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < allJarData.Count)
                {
                    var mToggleView = item.GetComponent<ArtifactJarToggleView>();
                    if (mToggleView != null)
                    {
                        mToggleView.SetCallback(toggleCallBack);
                        mToggleView.Init(allJarData[item.m_index]);
                        jarToggleDataArr[item.m_index] = mToggleView;
                    }
                }
            };
            mToggleComUIList.OnItemRecycle = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < allJarData.Count)
                {
                    var mToggleView = item.GetComponent<ArtifactJarToggleView>();
                    if (mToggleView != null)
                    {
                        mToggleView.Dispose();
                    }
                }
            };

            mRecordComUIList.Initialize();
            mRecordComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && artifactRecordDic.ContainsKey(mCurJarId) && item.m_index < artifactRecordDic[mCurJarId].Count)
                {
                    var recordData = artifactRecordDic[mCurJarId][item.m_index];
                    if (recordData != null)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)recordData.itemId);
                        if (itemData != null)
                        {
                            string tempTime = Function.GetDateTimeHaveMonthToSecond((int)recordData.recordTime);
                            string strItem = string.Format(" {{I 0 {0} 0}}", recordData.itemId);
                            item.gameObject.GetComponent<LinkParse>().SetText(TR.Value
                                ("artifact_jar_record", recordData.serverName, recordData.playerName, tempTime, strItem));
                        }
                    }
                }
            };
        }
        void _InitActivityUI()
        {
            var activityData = ArtifactDataManager.GetInstance().getArtifactAwardActData();

            if (activityData != null)
            {
                mDes.text = activityData.desc;
            }
        }

        /// <summary>
        /// 存下罐子可以开出的奖励数据
        /// </summary>
        /// <param name="jarData"></param>
        void SaveJarReward(ArtifactJarBuy jarData)
        {
            if (!jarRewardDic.ContainsKey((int)jarData.jarId))
            {
                List<ArtifactJarLotteryTable> rewardList = new List<ArtifactJarLotteryTable>();
                var tableData = TableManager.GetInstance().GetTable<ArtifactJarLotteryTable>();
                var enumerator = tableData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var tableItem = enumerator.Current.Value as ArtifactJarLotteryTable;
                    if (tableItem.JarId == (int)jarData.jarId)
                    {
                        rewardList.Add(tableItem);
                    }
                }
                jarRewardDic[(int)jarData.jarId] = rewardList;
            }
        }

        /// <summary>
        /// 发送获取记录协议
        /// </summary>
        /// <param name="jarData"></param>
        void SendJarRecord(ArtifactJarBuy jarData)
        {
            ArtifactDataManager.GetInstance().SendArtifactJarRecord((int)jarData.jarId);
            mEmptyTips.CustomActive(true);
        }

        /// <summary>
        /// //刷新界面ui
        /// </summary>
        /// <param name="jarData"></param>
        void UpdateUI(ArtifactJarBuy jarData)
        {
            //UpdateRecord(jarData);

            var jarBonusData = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>((int)jarData.jarId);
            if (jarBonusData != null)
            {
                //名字
                mJarName.text = jarBonusData.Name;
                mRecordTitleText.text = string.Format("{0}派奖记录", jarBonusData.Name);
                //罐子图片
                ETCImageLoader.LoadSprite(ref mJarIcon, jarBonusData.JarImage);
                
                //tips
                mJarTips.text = string.Format(TR.Value("artifact_jar_tips"),jarBonusData.ActifactJarRewardTime);
            }

            //已经完成
            if (jarData.buyCnt >= jarData.totalCnt)
            {
                mFinished.CustomActive(true);
            }
            else
            {
                mFinished.CustomActive(false);
            }

            //显示目前次数
            if (jarData.buyCnt >= jarData.totalCnt)
            {
                mJarCount.text = string.Format(TR.Value("artifact_jar_finish_tips"), jarData.totalCnt, jarData.totalCnt);
                mJarFinished.CustomActive(true);
            }
            else
            {
                mJarFinished.CustomActive(false);
                mJarCount.text = string.Format(TR.Value("artifact_jar_unfinish_tips"), jarData.buyCnt, jarData.totalCnt);
            }

            //打开奖励阅览界面
            mPreviewReward.onClick.RemoveAllListeners();
            mPreviewReward.onClick.AddListener(() =>
            {
                ArtifactJarRewardData artifactjarRewardData = new ArtifactJarRewardData();
                artifactjarRewardData.itemList = ArtifactDataManager.GetInstance().GetArtifactJarLotteryRewards((uint)mCurJarId);
                if(jarBonusData != null)
                {
                    artifactjarRewardData.desc = string.Format(TR.Value("artifact_jar_preview_tips"), jarBonusData.ActifactJarRewardTime);
                }
                ClientSystemManager.GetInstance().OpenFrame<ArtifactJarRewardPreviewFrame>(FrameLayer.Middle, artifactjarRewardData);
            });

            //打开奖励阅览界面
            mBoxBtn.onClick.RemoveAllListeners();
            mBoxBtn.onClick.AddListener(() =>
            {
                ArtifactJarRewardData artifactjarRewardData = new ArtifactJarRewardData();
                artifactjarRewardData.itemList = ArtifactDataManager.GetInstance().GetArtifactJarLotteryRewards((uint)mCurJarId);
                if (jarBonusData != null)
                {
                    artifactjarRewardData.desc = string.Format(TR.Value("artifact_jar_preview_tips"), jarBonusData.ActifactJarRewardTime);
                }
                ClientSystemManager.GetInstance().OpenFrame<ArtifactJarRewardPreviewFrame>(FrameLayer.Middle, artifactjarRewardData);
            });

            //左右按钮的显示隐藏
            if (mCurJarId == allJarData[0].jarId)
            {
                mLeftBtn.CustomActive(false);
            }
            else
            {
                mLeftBtn.CustomActive(true);
            }
            if(mCurJarId == allJarData[allJarData.Count - 1].jarId || !jarToggleDataArr[mCurToggleIndex+1].canPreviewJar())
            {
                mRightBtn.CustomActive(false);
            }
            else
            {
                mRightBtn.CustomActive(true);
            }

            mLeftBtn.onClick.RemoveAllListeners();
            mLeftBtn.onClick.AddListener(() =>
            {
                for(int i = 0;i<allJarData.Count;i++)
                {
                    if(allJarData[i].jarId == mCurJarId)
                    {
                        if (i != 0)
                        {
                            jarToggleDataArr[i - 1].SetToggleIsOn(true);
                            break;
                        }
                    }
                }
            });

            mRightBtn.onClick.RemoveAllListeners();
            mRightBtn.onClick.AddListener(() =>
            {
                for (int i = 0; i < allJarData.Count; i++)
                {
                    if (allJarData[i].jarId == mCurJarId)
                    {
                        if (i != allJarData.Count - 1)
                        {
                            jarToggleDataArr[i + 1].SetToggleIsOn(true);
                            break;
                        }
                    }
                }
            });
            var activityData = ArtifactDataManager.GetInstance().getArtifactAwardActData();
            //当活动为展示状态的时候，添加阴影
            if(ArtifactDataManager.GetInstance().IsArtifactActivityOpen())
            {
                mShowStateGo.CustomActive(true);
                mFinishedNeedHide.CustomActive(false);
            }
            else 
            {
                mShowStateGo.CustomActive(false);
                mFinishedNeedHide.CustomActive(true);
            }
        }

        

        void toggleCallBack(ArtifactJarBuy jarData)
        {
            mCurJarId = (int)jarData.jarId;
            for (int i = 0; i < allJarData.Count; i++)
            {
                if (allJarData[i].jarId == mCurJarId)
                {
                    mCurToggleIndex = i;
                }
            }
            SendJarRecord(jarData);
            SaveJarReward(jarData);
            UpdateUI(jarData);
        }
    }
}