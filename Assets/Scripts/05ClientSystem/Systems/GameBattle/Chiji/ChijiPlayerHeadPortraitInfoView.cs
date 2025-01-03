using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ProtoTable;
using Scripts.UI;
using Protocol;

namespace GameClient
{
    public class ChijiPlayerHeadPortraitInfoView : MonoBehaviour,IDisposable
    {
        [SerializeField]private Image mHeadIcon;
        [SerializeField]private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;
        [SerializeField]private Text mLevel;
        [SerializeField]private Text mName;
        [SerializeField]private Slider mHpSlider;
        [SerializeField]private Slider mMpSlider;
        [SerializeField]private Text mHp;
        [SerializeField]private Text mMp;
        [SerializeField]private ComUIListScript mChijiBuffComUIList;
        [SerializeField]private string sHPMPDesc = "{0} / {1}";

        private int iMaxHp = 0;
        private int iMaxMp = 0;
        private List<BeFightBuff> mChijiBuffList = new List<BeFightBuff>();
        private void Awake()
        {
            InitChijiBuffComUIList();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDReset, OnJobIDChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiHpChanged, OnChijiHPChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiMpChanged, OnChijiMPChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HeadPortraitFrameChange, OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged, OnBuffListChanged);

            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void InitView(DisplayAttribute attribute)
        {
            if (attribute != null)
            {
                var hpFieldInfo = attribute.GetType().GetField("maxHp");
                if (hpFieldInfo != null)
                {
                    float maxHp = (float)hpFieldInfo.GetValue(attribute);
                    iMaxHp = (int)maxHp;
                }

                var mpFieldInfo = attribute.GetType().GetField("maxMp");
                if (mpFieldInfo != null)
                {
                    float maxMp = (float)mpFieldInfo.GetValue(attribute);
                    iMaxMp = (int)maxMp;
                }
            }

            if(mChijiBuffList == null)
            {
                mChijiBuffList = new List<BeFightBuff>();
            }
            else
            {
                mChijiBuffList.Clear();
            }           
           
            ReplaceHeadPortraitFrame();
            SetName();
            SetLevel();
            UpdateHeadIcon();
            UpdateHP();
            UpdateMP();
            UpdateChijiBuffElementAmount();
        }

        /// <summary>
        /// 更新头像
        /// </summary>
        private void UpdateHeadIcon()
        {
            string sPath = "";

            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobTable != null)
            {
                var resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
                if (resTable != null)
                {
                    sPath = resTable.IconPath;
                }
            }

            if (mHeadIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mHeadIcon, sPath);
            }
        }

        private void UpdateHP()
        {
            if (mHpSlider != null)
            {
                mHpSlider.value = PlayerBaseData.GetInstance().Chiji_HP_Percent;
                //Logger.LogErrorFormat("吃鸡HP测试----界面刷新，Chiji_HP_Percent = {0}, mHpSlider.value = {1}", PlayerBaseData.GetInstance().Chiji_HP_Percent, mHpSlider.value);
            }

            if (mHp != null)
            {
                mHp.text = string.Format(sHPMPDesc, ((int)(iMaxHp * PlayerBaseData.GetInstance().Chiji_HP_Percent)).ToString(), iMaxHp.ToString()); 
            }
        }
        
        private void UpdateMP()
        {
            if (mMpSlider != null)
            {
                mMpSlider.value = PlayerBaseData.GetInstance().Chiji_MP_Percent;
                //Logger.LogErrorFormat("吃鸡MP测试----界面刷新，Chiji_MP_Percent = {0}, mMpSlider.value = {1}", PlayerBaseData.GetInstance().Chiji_MP_Percent, mMpSlider.value);
            }

            if (mMp != null)
            {
                mMp.text = string.Format(sHPMPDesc, ((int)(iMaxMp * PlayerBaseData.GetInstance().Chiji_MP_Percent)).ToString(), iMaxMp.ToString());
            }
        }

        private void SetName()
        {
            if (mName != null)
            {
                mName.text = PlayerBaseData.GetInstance().Name;
            }
        }

        private void SetLevel()
        {
            if (mLevel != null)
            {
                mLevel.text = PlayerBaseData.GetInstance().Level.ToString();
            }
        }
        private void ReplaceHeadPortraitFrame()
        {
            if (mReplaceHeadPortraitFrame != null)
            {
                if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID != 0)
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
                }
                else
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        private void OnJobIDChanged(UIEvent uiEvent)
        {
            UpdateHeadIcon();
        }

        private void OnChijiHPChanged(UIEvent uiEvent)
        {
            UpdateHP();
        }

        private void OnChijiMPChanged(UIEvent uiEvent)
        {
            UpdateMP();
        }

        private void OnHeadPortraitFrameChanged(UIEvent uiEvent)
        {
            ReplaceHeadPortraitFrame();
        }

        private void OnLevelChanged(UIEvent uiEvent)
        {
            SetLevel();
        }

        private void OnBuffListChanged(UIEvent uiEvent)
        {
            UpdateChijiBuffElementAmount();
        }

        private void OnAddNewItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type != ItemTable.eType.EQUIP)
                {
                    continue;
                }

                RefreshHpValue();
                break;
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            if (itemData.Type != ItemTable.eType.EQUIP)
            {
                return;
            }

            RefreshHpValue();
        }

        private void OnUpdateItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type != ItemTable.eType.EQUIP)
                {
                    continue;
                }

                RefreshHpValue();
                break;
            }
        }

        private void RefreshHpValue()
        {
            DisplayAttribute attribute = BeUtility.GetMainPlayerActorAttribute(true, true);
            if (attribute != null)
            {
                var hpFieldInfo = attribute.GetType().GetField("maxHp");
                if (hpFieldInfo != null)
                {
                    float maxHp = (float)hpFieldInfo.GetValue(attribute);
                    iMaxHp = (int)maxHp;
                }
            }

            UpdateHP();
        }

        private void InitChijiBuffComUIList()
        {
            if (mChijiBuffComUIList != null)
            {
                mChijiBuffComUIList.Initialize();
                mChijiBuffComUIList.onBindItem += OnBindItemDelegate;
                mChijiBuffComUIList.onItemVisiable += OnItemVisiableDelegate;
                mChijiBuffComUIList.OnItemUpdate += OnItemVisiableDelegate;
            }
        }

        private ChijiBuffItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ChijiBuffItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            ChijiBuffItem chijiBuffItem = item.gameObjectBindScript as ChijiBuffItem;
            if (chijiBuffItem != null && item.m_index >= 0 && item.m_index < mChijiBuffList.Count)
            {
                chijiBuffItem.OnItemVisiable(mChijiBuffList[item.m_index]);
            }
        }

        private void UpdateChijiBuffElementAmount()
        {
            if (mChijiBuffList != null)
            {
                mChijiBuffList.Clear();
            }
            
            if (PlayerBaseData.GetInstance().BuffMgr != null)
            {
                var buffList = PlayerBaseData.GetInstance().BuffMgr.GetBuffList();
                if (buffList != null)
                {
                    for (int i = 0; i < buffList.Count; i++)
                    {
                        //吃鸡buff表没有字段来区分哪个buff不显示，所以用id来区分，暂时写死
                        if (buffList[i].BuffID == 402000003 || buffList[i].BuffID == 400000001)
                        {
                            continue;
                        }

                        mChijiBuffList.Add(buffList[i]);
                    }
                }
            }

            if (mChijiBuffList.Count <= 0)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiBuffTipsFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<ChijiBuffTipsFrame>();
                }
            }

            if (mChijiBuffComUIList != null)
            {
                mChijiBuffComUIList.UpdateElementAmount(mChijiBuffList.Count);
            }
        }

        private void UnInitChijiBuffComUIList()
        {
            if (mChijiBuffComUIList != null)
            {
                mChijiBuffComUIList.onBindItem -= OnBindItemDelegate;
                mChijiBuffComUIList.onItemVisiable -= OnItemVisiableDelegate;
                mChijiBuffComUIList.OnItemUpdate -= OnItemVisiableDelegate;
            }
        }

        public void Dispose()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDReset, OnJobIDChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiHpChanged, OnChijiHPChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiMpChanged, OnChijiMPChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HeadPortraitFrameChange, OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged, OnBuffListChanged);

            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            iMaxHp = 0;
            iMaxMp = 0;
            mChijiBuffList.Clear();
            UnInitChijiBuffComUIList();
        }
    }
}