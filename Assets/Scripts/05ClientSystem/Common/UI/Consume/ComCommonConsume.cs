using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;
using ProtoTable;
using Protocol;
using Network;
using System.Diagnostics;

namespace GameClient
{
    //[RequireComponent(typeof(LayoutElement))]
    //[RequireComponent(typeof(ContentSizeFitter))]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class ComCommonConsume : MonoBehaviour
    {
        public enum eType
        {
            /// <summary>
            /// 道具
            /// </summary>
            Item,

            /// <summary>
            /// 次数 
            /// </summary>
            Count,
        }

        public enum eCountType
        {
            Fatigue,
            MouCount,
            NorthCount,
            HellTiketCount,
            DeadTower,
            Final_Test, // 终极试炼
        }

        public eType           mType;
        public eCountType      mCountType;

        public int             mItemID;
        public int             mDungeonID;

        public bool            mIsGetParentComFrameBinder = true;

        public ComCommonBind   mBind;

        private Image          mIcon;
        private Image          mBorad;
        private Button         mAdd;
        private Text           mCount;
        private Text           mSumCount;
        private Button         mTipsButton;

        private ICommonConsume mConsume;
        private ClientFrameBinder comFrameBinder;

        private void _preSetting()
        {
//             LayoutElement ele         = this.GetComponent<LayoutElement>();
//             ele.ignoreLayout          = true;
// 
//             ContentSizeFitter filter  = this.GetComponent<ContentSizeFitter>();
//             filter.horizontalFit      = ContentSizeFitter.FitMode.PreferredSize;
//             filter.verticalFit        = ContentSizeFitter.FitMode.PreferredSize;

            HorizontalLayoutGroup hg  = this.GetComponent<HorizontalLayoutGroup>();
            hg.childForceExpandWidth  = false;
            hg.childForceExpandHeight = false;

            if (null != mBind)
            {
                mIcon       = mBind.GetCom<Image>("icon");
                mCount      = mBind.GetCom<Text>("cnt");
                mSumCount   = mBind.GetCom<Text>("sumcnt");
                mAdd        = mBind.GetCom<Button>("addbutton");
                mTipsButton = mBind.GetCom<Button>("tipsButton");
            }

            if (mIsGetParentComFrameBinder)
            {
                comFrameBinder = GetComponentInParent<ClientFrameBinder>();
            }
        }

        bool m_bInitialize = false;
        void Start()
        {
            if(!m_bInitialize)
            {
                _preSetting();

                _init();

                _bindEvent();

                _loadButton();
                m_bInitialize = true;
            }
        }

        private void _init()
        {
            switch (mType)
            {
                case eType.Item:
                    {
                        ItemTable table = TableManager.instance.GetTableItem<ItemTable>(mItemID);
                        if (null != table && null != mIcon)
                        {
                            // mIcon.sprite = AssetLoader.instance.LoadRes(table.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref mIcon, table.Icon);
                        }
                        mConsume = ConsumeFactory.CreateByItemID(mItemID, comFrameBinder);
                        break;
                    }

                case eType.Count:
                    mConsume = ConsumeFactory.CreateByCountType(mCountType, mDungeonID,comFrameBinder);
                    break;
            }
        }

        void OnDestroy()
        {
            _unbindEvent();

            _unloadButton();

            _uninit();
        }

        private void _uninit()
        {
            comFrameBinder = null;
            mConsume    = null;

            mIcon       = null;
            mBorad      = null;
            mAdd        = null;
            mCount      = null;
            mSumCount   = null;
            mTipsButton = null;
        }

        public void SetData(eType type, eCountType countType, int id)
        {
            _unbindEvent();

            mType      = type;
            mCountType = countType;

            switch (mType)
            {
                case eType.Item:
                    mItemID    = id;
                    break;
                case eType.Count:
                    mDungeonID = id;
                    break;
            }

            _init();

            _bindEvent();
        }

        private void _bindEvent()
        {
            if (null != mConsume)
            {
                EUIEventID[] list = mConsume.WatchEvents();

                if (null == list)
                {
                    return;
                }

                for (int i = 0; i < list.Length; ++i)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(list[i], _onUpdate);
                }

                _onUpdate(null);
            }
        }

        private void _unbindEvent()
        {
            if (null != mConsume)
            {
                EUIEventID[] list = mConsume.WatchEvents();

                if (null == list) 
                {
                    return;
                }

                for (int i = 0; i < list.Length; ++i)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(list[i], _onUpdate);
                }
            }
        }

        private void _onUpdate(UIEvent ui)
        {
            if (null != mConsume)
            {
                mConsume.OnChange();

                if (null != mCount) 
                {
                    //mCount.text = mConsume.GetCount().ToString();
                    mCount.text = Utility.ToThousandsSeparator(mConsume.GetCount());
                }

                if (null != mSumCount) 
                {
                    mSumCount.text = mConsume.GetSumCount().ToString();
                }
            }
        }

        private void _onAddClick()
        {
            if (null != mConsume)
            {
                mConsume.OnAdd();
            }
        }

        private void _onPopTips()
        {
            if (mType == eType.Item)
            {
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemID);
                if (itemData != null)
                {
                    Logger.LogWarningFormat("ComCommonConsume ==> _onPopTips");
                    ItemTipManager.GetInstance().CloseAll();
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            }
        }

        private void _loadButton()
        {
            _unloadButton();

            if (null != mAdd)
            {
                mAdd.onClick.AddListener(_onAddClick);                
            }

            if(null != mTipsButton)
            {
                mTipsButton.onClick.AddListener(_onPopTips);
            }
        }

        private void _unloadButton()
        {
            if (null != mAdd)
            {
                mAdd.onClick.RemoveListener(_onAddClick);
            }
            if(null != mTipsButton)
            {
                mTipsButton.onClick.RemoveListener(_onPopTips);
            }
        }

    }
}
