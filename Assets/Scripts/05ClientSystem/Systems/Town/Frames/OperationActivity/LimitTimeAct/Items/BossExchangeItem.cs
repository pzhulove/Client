using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BossExchangeItem : MonoBehaviour, IDisposable
    {
        [SerializeField] private Text mTextExchangeCount; //兑换次数
        [SerializeField] private RectTransform mItemRoot; //所需物品和兑换物品根节点
        [SerializeField] Button mButtonExchange; //兑换
        [SerializeField] Button mButtonGoLink; //获取物品
        [SerializeField] private string ItemPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/BossExchangeThing";
        [SerializeField] private UIGray mExchangeGray;
        private readonly List<ComItem> mComItems = new List<ComItem>();
        private BossExchangeTaskModel mData;
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private int mId;
        private readonly Dictionary<int, Text> mItemTextCountsDictionary = new Dictionary<int, Text>();

        private bool mIsChallengScoreActivity = false;

        private int mAccountNumLeft = 0;
        [SerializeField]
        private int mChallengeScoreItemId = 800001235;//挑战者积分id 
        private bool mIsInit = false;

        public void Init(int id, BossExchangeTaskModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick,bool isChallengScoreActivity=false)
        {
            mId = id;
            mOnItemClick = onItemClick;
            mButtonExchange.SafeAddOnClickListener(_OnButtonGoChallengeClick);
            ActivityDataManager.GetInstance().RegisterBossAccountData(_OnUpdateAccounterNum);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateIntegrationChallengeScore, _OnUpdateChallengeNum);
            ActivityDataManager.GetInstance().OnRequsetBossAccountData(data);
            mData = data;
            mIsChallengScoreActivity = isChallengScoreActivity;
            if(mIsChallengScoreActivity==false)
            {
                UpdateData(data);
                _InitItems();
            }
           
        }

       
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public void UpdateData(BossExchangeTaskModel data)
        {
            mData = data;
            if(mData.AccountTotalSubmitLimit==0)
            {
                if (mData.TaskCycleCount == -1)
                {
                    mTextExchangeCount.CustomActive(false);
                }
                else
                {
                   mTextExchangeCount.CustomActive(true);
                   mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_boss_exchange_item_count_role"), mData.RemainCount));
                }
            }
            if(mData.AccountTotalSubmitLimit==0)
            {
                _UpdateButtonState(mData.RemainCount);
            }
           
            _UpdateItems();
        }

        void _UpdateItems()
        {
            if (mData.NeedItems == null)
            {
                return;
            }

            foreach (var itemText in mItemTextCountsDictionary)
            {
                //显示数量
                Text textCount = itemText.Value;
                int id = itemText.Key;
                if (!mData.NeedItems.ContainsKey(id))
                {
                    continue;
                }
                int count = mData.NeedItems[id];
                textCount.CustomActive(true);
                int curCount = 0;
                if(!mIsChallengScoreActivity)
                {
                    curCount = ItemDataManager.GetInstance().GetItemCountInPackage(id);
                }
                else
                {
                    curCount = PlayerBaseData.GetInstance().ChanllengeScore;
                }
                string color = curCount.ToString();
                if (curCount < count)
                {
                    color = _TrySetColor(color, Color.red);
                }
                else
                {
                    color = _TrySetColor(color, Color.green);
                }
                textCount.SafeSetText(color + "/" + count);
            }
        }

        public void Destroy()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }

                mComItems.Clear();
            }
            mIsInit = false;
            mAccountNumLeft = 0;
            mItemTextCountsDictionary.Clear();
            mOnItemClick = null;
            mButtonExchange.SafeRemoveOnClickListener(_OnButtonGoChallengeClick);
            mButtonGoLink.SafeRemoveAllListener();
            ActivityDataManager.GetInstance().UnRegisterBossAccountData(_OnUpdateAccounterNum);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateIntegrationChallengeScore, _OnUpdateChallengeNum);
        }

        void _InitItems()
        {
            if (mData.NeedItems == null)
            {
                return;
            }

            GameObject templateItem = AssetLoader.instance.LoadResAsGameObject(ItemPrefabPath);
            if (templateItem == null)
            {
                Logger.LogError("load prefab failed at path: " + ItemPrefabPath);
                return;
            }
            var bind = templateItem.GetComponent<ComCommonBind>();
            if (bind == null)
            {
                Logger.LogErrorFormat("can not find ComcomonBind in mBindThing at Path:" + ItemPrefabPath);
                return;
            }

            GameObject itemRoot = bind.GetGameObject("ItemRoot");
            if (itemRoot == null)
            {
                Logger.LogErrorFormat("can not find ItemRoot in mBind at Path:" + ItemPrefabPath);
                return;
            }

            //创建左侧需要的物品
            _InitConsueItems(templateItem);
            //创建右侧可兑换的物品
            _InitAwardItems(templateItem);
            //销毁模板
            Destroy(templateItem);
        }

        void _InitConsueItems(GameObject template)
        {
            bool isFirst = true;
            foreach (var task in mData.NeedItems)
            {
                _InitItem(template, task.Key, task.Value, !isFirst, false);
                isFirst = false;
            }
        }

        void _UpdateButtonState(int remainCount)
        {
            bool isItemEnough = true;
            int notEnoughItemId = 0;
            if (mData.NeedItems != null)
            {
                foreach (var task in mData.NeedItems)
                {
                    int curNum = 0;
                    if(!mIsChallengScoreActivity)
                    {
                        curNum = ItemDataManager.GetInstance().GetItemCountInPackage(task.Key);
                    }
                    else
                    {
                        curNum = PlayerBaseData.GetInstance().ChanllengeScore;

                    }
                    if (isItemEnough && curNum < task.Value)
                    {
                        isItemEnough = false;
                        notEnoughItemId = task.Key;
                        break;
                    }
                }
            }

            //如果兑换次数耗尽并且不是无限兑换类型
            if (remainCount == 0 && mData.TaskCycleCount != -1)
            {
                mButtonExchange.CustomActive(true);
                mButtonGoLink.CustomActive(false);
                if (mExchangeGray != null)
                    mExchangeGray.enabled = true;
            }
            else
            {
                //如果需要物品的数量不足
                if (!isItemEnough)
                {
                    mButtonExchange.CustomActive(false);
                    mButtonGoLink.CustomActive(true);
                    mButtonGoLink.SafeRemoveAllListener();
                    mButtonGoLink.SafeAddOnClickListener(() =>
                    {
                        ItemComeLink.OnLink(notEnoughItemId, 0);
                    });
                }
                else
                {
                    mButtonExchange.CustomActive(true);
                    mButtonGoLink.CustomActive(false);
                }

                if (mExchangeGray != null)
                    mExchangeGray.enabled = false;
            }

        }

        void _InitAwardItems(GameObject template)
        {
            //创建右侧兑换的物品
            bool isFirst = true;
            foreach (var task in mData.ExchangeItems)
            {
                _InitItem(template, task.Key, task.Value, !isFirst, isFirst, false);
                isFirst = false;
            }
        }

        void _InitItem(GameObject template, int id, int count, bool isShowAdd = false, bool isShowEqual = false, bool isShowOwnCount = true)
        {
            if (template == null)
                return;

            GameObject go = GameObject.Instantiate(template);
            go.transform.SetParent(mItemRoot, false);

            var bind = go.GetComponent<ComCommonBind>();
            GameObject itemRoot = bind.GetGameObject("ItemRoot");

            //创建comitem
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);

	        ComItem comitem = itemRoot.gameObject.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                comitem = ComItemManager.Create(itemRoot);
            }
            comitem.Setup(itemData, Utility.OnItemClicked);

            //显示加号 等于号
            GameObject equal = bind.GetGameObject("dengyu");
            GameObject add = bind.GetGameObject("jia");
            add.CustomActive(isShowAdd);
            equal.CustomActive(isShowEqual);

            if (isShowOwnCount)
            {
                //显示数量
                Text textCount = bind.GetCom<Text>("Count");
                textCount.CustomActive(true);
                int curNum = 0;
                if(!mIsChallengScoreActivity)
                {
                    curNum = ItemDataManager.GetInstance().GetItemCountInPackage(id);
                }
                else
                {
                    curNum = PlayerBaseData.GetInstance().ChanllengeScore;
                }
                string color = curNum.ToString();
                if (curNum < count)
                {
                    color = _TrySetColor(color, Color.red);
                }
                else
                {
                    color = _TrySetColor(color, Color.green);
                }
                textCount.SafeSetText(color + "/" + count);
                if(!mItemTextCountsDictionary.ContainsKey(id))
                {
                    mItemTextCountsDictionary.Add(id, textCount);
                }
              
            }
            else
            {
	            if (count > 1)
	            {
		            Text textCount = bind.GetCom<Text>("Count");
		            textCount.CustomActive(true);
		            textCount.SafeSetText(count.ToString());
	            }
			}
		}

        void _OnButtonGoChallengeClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick(mId, 0);
            }
            ActivityDataManager.GetInstance().OnRequsetBossAccountData(mData);
            if(mIsChallengScoreActivity)
            {
                ActivityDataManager.GetInstance().RequestChallengeScore();
            }
        }

        string _TrySetColor(string str, Color color)
        {
            if (color == Color.red)
            {
                return "<color=#FF0101FF>" + str + "</color>";
            }
            else if (color == Color.green)
            {
                return "<color=#01FF01FF>" + str + "</color>";
            }
            else
            {
                return str;
            }
        }

        private void _OnUpdateAccounterNum(UIEvent uiEvent)
        {
           if ((uint)uiEvent.Param1 == mData.Id&&(uint)uiEvent.Param2==(uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK)
           {
               if(mData.AccountTotalSubmitLimit>0)
                {
                    int haveNum = (int)uiEvent.Param3;
                    int leftNum = mData.AccountTotalSubmitLimit - haveNum;
                    if (leftNum < 0) leftNum = 0;
                    mAccountNumLeft = leftNum;
                    mTextExchangeCount.CustomActive(true);
                    mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_boss_exchange_item_count_account"), leftNum));
                    _UpdateButtonState(leftNum);
                }
           }
        }

        private void _OnUpdateChallengeNum(UIEvent uiEvent)
        {
            if(mIsChallengScoreActivity)
            {
                UpdateData(mData);
                if(!mIsInit)
                {
                    _InitItems();
                    mIsInit = true;
                }
                
                if (mData.AccountTotalSubmitLimit > 0)
                {
                    _UpdateButtonState(mAccountNumLeft);
                }
              
            }
            
        }

    }
}

