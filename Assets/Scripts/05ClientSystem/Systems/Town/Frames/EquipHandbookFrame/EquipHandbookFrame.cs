using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using Spine.Unity;
using Scripts.UI;

namespace GameClient
{
    /// <summary>
    /// 装备图鉴
    /// </summary>
    public class EquipHandbookFrame : ClientFrame
    {
        int itemid = 0;//记录选中道具ID
        GeAvatarRendererEx m_AvatarRenderer = null;
        bool bIsPlayAnimation = false;

        private int mLastSelectedIndex = -1;
        float fViewPortPos = 726.0f;
        int mSelectItemID = 0; //当前选择的装备ID
        EquipHandbookTabData mcurrentSelectTab = null;
        EquipHandbookEquipItemData mcurrentSelectItem = null;
        /// <summary>
        /// 超链接
        /// </summary>
        /// <param name="value"></param>
        public static void OpenLinkFrame(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            try
            {
                EquipHandbookDataManager.GetInstance().TabSelectedIndex = int.Parse(value);
                ClientSystemManager.GetInstance().CloseFrame<EquipHandbookFrame>();
                ClientSystemManager.GetInstance().OpenFrame<EquipHandbookFrame>(FrameLayer.Middle);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }

            // TODO 根据参数传入打开界面

            /// ClientSystemManager.instance.OpenFrame<EquipHandbookFrame>(xxxx, xxxx);
        }

        #region ExtraUIBind
        private GameObject mTabRoot = null;
        private GameObject mLeftRoot = null;
        private GameObject mRightSourceRoot = null;
        private GameObject mRightCommentRoot = null;
        private GameObject mRightPreviewRoot = null;
        private Text mTabName = null;
        private Button mClosePreviewButton = null;
        private Button mTryEquipButton = null;
        private Button mDetailButton = null;
        private ToggleGroup mLeftGirdToggleGroup = null;
        private CanvasGroup mRightRoot = null;
        private ToggleGroup mTabToggleGroup = null;
        private ComEquipHandbookItem mDetailEquip = null;
        private GameObject mSourceEmptyRoot = null;
        private GameObject mCommentEmptyRoot = null;
        private Button mCloseButton = null;
        private GameObject mModelRT = null;
        private Text mEquipName = null;
        private Image mArrowState = null;
        private Text mDifferenceScore = null;
        private Text mTatleScore = null;
        private GameObject mSocreRoot = null;
        // private SkeletonAnimation mSkeletonAnimation = null;
        private ComUIListScript mLeftRootComUIList = null;
        private GameObject mSecendsocreRoot = null;
        private GameObject mPerfectRoot = null;
        private Image mLeftImg = null;
        private Image mRightImg = null;
        private ScrollRect mLeftRootScrollRect = null;
        private RectTransform mContent = null;
        private ScrollRect mSRScrollRect = null;
        private ScrollRect mCRScrollRect = null;

        protected override void _bindExUI()
        {
            mTabRoot = mBind.GetGameObject("tabRoot");
            mLeftRoot = mBind.GetGameObject("leftRoot");
            mRightSourceRoot = mBind.GetGameObject("rightSourceRoot");
            mRightCommentRoot = mBind.GetGameObject("rightCommentRoot");
            mRightPreviewRoot = mBind.GetGameObject("rightPreviewRoot");
            mTabName = mBind.GetCom<Text>("tabName");
            mClosePreviewButton = mBind.GetCom<Button>("closePreviewButton");
            mClosePreviewButton.onClick.AddListener(_onClosePreviewButtonButtonClick);
            mTryEquipButton = mBind.GetCom<Button>("tryEquipButton");
            mTryEquipButton.onClick.AddListener(_onTryEquipButtonButtonClick);
            mDetailButton = mBind.GetCom<Button>("detailButton");
            mDetailButton.onClick.AddListener(_onDetailButtonButtonClick);
            mLeftGirdToggleGroup = mBind.GetCom<ToggleGroup>("leftGirdToggleGroup");
            mRightRoot = mBind.GetCom<CanvasGroup>("rightRoot");
            mTabToggleGroup = mBind.GetCom<ToggleGroup>("tabToggleGroup");
            mDetailEquip = mBind.GetCom<ComEquipHandbookItem>("detailEquip");
            mSourceEmptyRoot = mBind.GetGameObject("sourceEmptyRoot");
            mCommentEmptyRoot = mBind.GetGameObject("commentEmptyRoot");
            mCloseButton = mBind.GetCom<Button>("closeButton");
            mCloseButton.onClick.AddListener(_onCloseButtonButtonClick);
            mModelRT = mBind.GetGameObject("modelRT");
            mEquipName = mBind.GetCom<Text>("equipName");
            mArrowState = mBind.GetCom<Image>("ArrowState");
            mDifferenceScore = mBind.GetCom<Text>("differenceScore");
            mTatleScore = mBind.GetCom<Text>("tatleScore");
            mSocreRoot = mBind.GetGameObject("SocreRoot");
            // mSkeletonAnimation = mBind.GetCom<SkeletonAnimation>("SkeletonAnimation");
            mLeftRootComUIList = mBind.GetCom<ComUIListScript>("LeftRootComUIList");
            mSecendsocreRoot = mBind.GetGameObject("secendsocreRoot");
            mPerfectRoot = mBind.GetGameObject("perfectRoot");
            mLeftImg = mBind.GetCom<Image>("leftImg");
            mRightImg = mBind.GetCom<Image>("rightImg");
            mLeftRootScrollRect = mBind.GetCom<ScrollRect>("LeftRootScrollRect");
            mContent = mBind.GetCom<RectTransform>("Content");
            mSRScrollRect = mBind.GetCom<ScrollRect>("SRScrollRect");
            mCRScrollRect = mBind.GetCom<ScrollRect>("CRScrollRect");
        }

        protected override void _unbindExUI()
        {
            mTabRoot = null;
            mLeftRoot = null;
            mRightSourceRoot = null;
            mRightCommentRoot = null;
            mRightPreviewRoot = null;
            mTabName = null;
            mClosePreviewButton.onClick.RemoveListener(_onClosePreviewButtonButtonClick);
            mClosePreviewButton = null;
            mTryEquipButton.onClick.RemoveListener(_onTryEquipButtonButtonClick);
            mTryEquipButton = null;
            mDetailButton.onClick.RemoveListener(_onDetailButtonButtonClick);
            mDetailButton = null;
            mLeftGirdToggleGroup = null;
            mRightRoot = null;
            mTabToggleGroup = null;
            mDetailEquip = null;
            mSourceEmptyRoot = null;
            mCommentEmptyRoot = null;
            mCloseButton.onClick.RemoveListener(_onCloseButtonButtonClick);
            mCloseButton = null;
            mModelRT = null;
            mEquipName = null;
            mArrowState = null;
            mDifferenceScore = null;
            mTatleScore = null;
            mSocreRoot = null;
            // mSkeletonAnimation = null;
            mLeftRootComUIList = null;
            mSecendsocreRoot = null;
            mPerfectRoot = null;
            mLeftImg = null;
            mRightImg = null;
            mLeftRootScrollRect = null;
            mContent = null;
            mSRScrollRect = null;
            mCRScrollRect = null;
        }
        #endregion

        #region Callback
        private void _onClosePreviewButtonButtonClick()
        {
            /* put your code in here */
            mRightPreviewRoot.CustomActive(false);
            mRightRoot.CustomActive(true);

            _onGirdItemClick(itemid, mcurrentSelectTab, mcurrentSelectItem,false);
        }

        private void _onTryEquipButtonButtonClick()
        {
            /* put your code in here */
            mRightPreviewRoot.CustomActive(true);
            mRightRoot.CustomActive(false);

            _setEquipName(itemid);
            
            _initModel(itemid);
            
        }

        private void _onDetailButtonButtonClick()
        {
            /* put your code in here */
            _showTips(itemid);

        }
        private void _onCloseButtonButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.instance.CloseFrame(this);
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipHandbookFrame/EquipHandbookFrame";
        }

        protected override void _OnOpenFrame()
        {
            bIsPlayAnimation = false;
            mLastSelectedIndex = -1;

            mLeftRootComUIList.Initialize();
            
            _createTabs();

            _selectDefalutTab();
        }

        private void _createTabs()
        {
            string tabUnitPath = mBind.GetPrefabPath("tabUnit");

            mBind.ClearCacheBinds(tabUnitPath);

            List<EquipHandbookTabData> tabs = EquipHandbookDataManager.GetInstance().equipTabs;

            for (int i = 0; i < tabs.Count; i++)
            {
                ComCommonBind bind = mBind.LoadExtraBind(tabUnitPath);

                Utility.AttachTo(bind.gameObject, mTabRoot);

                EquipHandbookTabData data = tabs[i];

                if (null == data)
                {
                    continue ;
                }

                data.bind = bind;

                Toggle toggle = bind.GetCom<Toggle>("toggle");
                Text diablename = bind.GetCom<Text>("diablename");
                Text name = bind.GetCom<Text>("name");

                name.text = data.name;
                diablename.text = data.name;

                //_onTabclick()

                toggle.group = mTabToggleGroup;

                int index = i;

                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(isOn => 
                {
                    _changeTabStatus(isOn, data);

                    if (mRightPreviewRoot.gameObject.activeSelf)
                    {
                        mRightPreviewRoot.CustomActive(false);
                    }

                    if (isOn)
                    {
                        if (mLastSelectedIndex != index)
                        {
                            mLastSelectedIndex = index;
                            EquipHandbookDataManager.GetInstance().TabSelectedIndex = index;


                            _stopCoroutineCurrentTabClick();
                            mCurrentTabClick =mBind.StartCoroutine(_onTabClickWithAnimate(isOn, data));
                        }
                    }
                });
            }
        }

        private void _stopCoroutineCurrentTabClick()
        {
            if (null != mCurrentTabClick)
            {
                mBind.StopCoroutine(mCurrentTabClick);
                mCurrentTabClick = null;
            }
        }

        private Coroutine mCurrentTabClick = null;
        private Coroutine mSelectCurrentLevelGridItem = null;
        private void _changeTabStatus(bool isOn, EquipHandbookTabData data)
        {
            ComCommonBind bind = data.bind;
            if (null == bind)
            {
                return;
            }

            GameObject disableRoot = bind.GetGameObject("disableRoot");
            GameObject enableRoot = bind.GetGameObject("enableRoot");

            disableRoot.CustomActive(!isOn);
            enableRoot.CustomActive(isOn);

            mTabName.text = data.name;
        }

        /// <summary>
        /// 选择默认的叶签
        /// 引导相关的
        /// </summary>
        private void _selectDefalutTab()
        {
            List<EquipHandbookTabData> tabs = EquipHandbookDataManager.GetInstance().equipTabs;

            int selectedIdx = EquipHandbookDataManager.GetInstance().TabSelectedIndex;
            if (selectedIdx >= 0 && selectedIdx < tabs.Count)
            {
                ComCommonBind bind = tabs[selectedIdx].bind;
                if (null != bind)
                {
                    Toggle toggle = bind.GetCom<Toggle>("toggle");
                    toggle.isOn = true;
                }
            }
       }

        private void _playAnimation()
        {
            // mSkeletonAnimation.CustomActive(true);

            // Spine.AnimationState spineAnimationState = mSkeletonAnimation.state;

            // mSkeletonAnimation.skeleton.SetToSetupPose();
            // spineAnimationState.ClearTracks();

            // spineAnimationState.SetAnimation(0, "tujian", false);
            // spineAnimationState.Complete += SpineAnimationState_Complete;
        }

        private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
        {
            // mSkeletonAnimation.CustomActive(false);
        }

        private void _initModel(int itemid)
        {
            if (mModelRT != null)
            {
                if (m_AvatarRenderer == null)
                {
                    m_AvatarRenderer = mModelRT.GetComponent<GeAvatarRendererEx>();
                }
                
            }

            JobTable job = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (job == null)
            {
                Logger.LogError("职业ID找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                if (res == null)
                {
                    Logger.LogError("职业ID Mode表 找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
                }
                else
                {
                    m_AvatarRenderer.ClearAvatar();
                    m_AvatarRenderer.LoadAvatar(res.ModelPath);

                    PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(m_AvatarRenderer);
                    PlayerBaseData.GetInstance().AvatarEquipWeapon(m_AvatarRenderer, PlayerBaseData.GetInstance().JobTableID, itemid);
                    m_AvatarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                    m_AvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
                }
            }
        }

        void _clearModel()
        {
            if (m_AvatarRenderer != null)
            {
                m_AvatarRenderer.ClearAvatar();
                m_AvatarRenderer = null;
            }
        }

        void _setEquipName(int itemid)
        {
            ItemTable table = TableManager.GetInstance().GetTableItem<ItemTable>(itemid);
            if (table != null)
            {
                ItemData data = ItemDataManager.GetInstance().GetCommonItemTableDataByID(table.ID);

                ItemData.QualityInfo qualityInfo = data.GetQualityInfo();

                mEquipName.text = TR.Value("equiphandbook_equip_name", data.GetColorName(), qualityInfo.ColStr);
            }
        }

        bool _setTryOnBtnIsShow(int itemid)
        {
            ItemTable table = TableManager.GetInstance().GetTableItem<ItemTable>(itemid);
            if (table != null)
            {
                if (table.Type!=ItemTable.eType.EQUIP)
                {
                    return false;
                }

                if (table.SubType!=ItemTable.eSubType.WEAPON)
                {
                    return false;
                }
            }
            return true;
        }

        IEnumerator _onTabClickWithAnimate(bool isOn, EquipHandbookTabData data)
        {
            // mSkeletonAnimation.CustomActive(false);

            if (bIsPlayAnimation)
            {
                _clearGirdItems(data);

                //  翻页动画
                _playAnimation();
                yield return Yielders.GetWaitForSeconds(0.55f);
            }

            _onTabclick(data);

            yield break;
        }

        private void _clearGirdItems(EquipHandbookTabData data)
        {
            string titleRoot = mBind.GetPrefabPath("titleRoot");
            string girdRoot = mBind.GetPrefabPath("girdRoot");
            string girdItemPath = mBind.GetPrefabPath("girdItemPath");

            mBind.ClearCacheBinds(girdItemPath);
            mBind.ClearCacheBinds(girdRoot);
            mBind.ClearCacheBinds(titleRoot);

            mLeftRoot.CustomActive(false);
            mRightRoot.CustomActive(false);
            mSocreRoot.CustomActive(false);
        }

        List<EquipHandbookEquipItemData> mRecommendEquips = null;

        private void _onTabclick(EquipHandbookTabData data)
        {
            bIsPlayAnimation = true;
            mLeftRoot.CustomActive(true);
            mRightRoot.CustomActive(true);


            int itemCount = 0;
            List<Vector2> size = new List<Vector2>();

            if (data.isShowEquipScore)
            {
                mSocreRoot.CustomActive(true);
                mSecendsocreRoot.CustomActive(true);
                mPerfectRoot.CustomActive(false);

                itemCount = 1;

                int mPlayerWeaponScore = EquipHandbookDataManager.GetInstance().GetPlayerWeaponScore();
                int mPlayerArmorScore = EquipHandbookDataManager.GetInstance().GetPlayerArmorScore();
                int mPlayerJewelryScore = EquipHandbookDataManager.GetInstance().GetPlayerJewelryScore();

                bool mPlayerEquipIsAchieveBest = false;

                mRecommendEquips = data.GetRecommendedCollect(mPlayerWeaponScore, mPlayerArmorScore, mPlayerJewelryScore,ref mPlayerEquipIsAchieveBest);

               
                if (mPlayerEquipIsAchieveBest)
                {
                    mSecendsocreRoot.CustomActive(false);
                    mPerfectRoot.CustomActive(true);
                }
                else
                {
                    int mTotalScore = 0;

                    for (int i = 0; i < mRecommendEquips.Count; i++)
                    {
                        mTotalScore += mRecommendEquips[i].baseScore;
                    }

                    mTatleScore.text = mTotalScore.ToString();

                    int score = mTotalScore - EquipHandbookDataManager.GetInstance().sumPlayerEquipCollectScore;

                    if (score > 0)
                    {
                        mDifferenceScore.text = (score).ToString();
                    }
                    else
                    {
                        mSecendsocreRoot.CustomActive(false);
                    mPerfectRoot.CustomActive(true);
                    }
                    
                }
                size.Add(_getCellVector(mRecommendEquips.Count));
            }
            else
            {
                mSocreRoot.CustomActive(false);
                itemCount = data.collectIDs.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    size.Add(_getCellVector(data.collectIDs[i].itemIDs.Count));
                }
            }
            
            mLeftRootComUIList.ResetContentPosition();

            mLeftRootComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && itemCount != 0 && item.m_index < itemCount)
                {
                    string girdItemPath = mBind.GetPrefabPath("girdItemPath");
                    ComCommonBind mComBind = item.GetComponent<ComCommonBind>();
                    GameObject mEquipHandbookLeftTitleRoot = mComBind.GetGameObject("EquipHandbookLeftTitleRoot");
                    GameObject mEquipHandbookLeftGirdRoot = mComBind.GetGameObject("EquipHandbookLeftGirdRoot");
                   
                    if (data.isShowEquipScore)
                    {
                        mEquipHandbookLeftTitleRoot.CustomActive(false);
                        _createGirdItemUnit(girdItemPath, mRecommendEquips, mEquipHandbookLeftGirdRoot, data);

                    }
                    else
                    {
                        EquipHandbookTabCollectionData itemData = null;
                        itemData = data.collectIDs[item.m_index];

                        if (data.type == ProtoTable.EquipHandbookContentTable.eType.Collect)
                        {
                            mEquipHandbookLeftTitleRoot.CustomActive(true);

                            ComCommonBind mTitleRootBind = mEquipHandbookLeftTitleRoot.GetComponent<ComCommonBind>();

                            Text textLevel = mTitleRootBind.GetCom<Text>("textLevel");
                            Text textName = mTitleRootBind.GetCom<Text>("textName");

                            textLevel.text = itemData.level.ToString();
                            textName.text = itemData.name;

                        }
                        else
                        {
                            mEquipHandbookLeftTitleRoot.CustomActive(false);
                        }
                        _createGirdItemUnit(girdItemPath, itemData.itemIDs, mEquipHandbookLeftGirdRoot, data);

                    }
                }
            };

            mLeftRootComUIList.SetElementAmount(itemCount, size);

            _stopCoroutineSelectCurrentLevelGridItem();
            mSelectCurrentLevelGridItem = mBind.StartCoroutine(_selectDefauteGridItem(data));
        }

        private void _stopCoroutineSelectCurrentLevelGridItem()
        {
            if (null != mSelectCurrentLevelGridItem)
            {
                mBind.StopCoroutine(mSelectCurrentLevelGridItem);
                mSelectCurrentLevelGridItem = null;
            }
        }

        private Vector2 _getCellVector(int cellCount)
        {
            float contentHeight = 0;

            if (cellCount % 4 != 0)
            {
                contentHeight = (float)Math.Ceiling((decimal)(cellCount/ 4));
            }
            else
            {
                contentHeight = (float)Math.Ceiling((decimal)(cellCount / 4)) - 1;
            }

            return new Vector2 { x = 662.0f, y = 223.0f + contentHeight * 213.0f };
        }

        private void _createGirdItemUnit(string path, List<EquipHandbookEquipItemData> itemIDs, GameObject root, EquipHandbookTabData tabdata)
        {
            if ( null == root)
            {
                return;
            }
            ComCommonBind rootBind = root.GetComponent<ComCommonBind>();

            rootBind.ClearCacheBinds(path);

            for (int i = 0; i < itemIDs.Count; ++i)
            {
                EquipHandbookEquipItemData equipItemData = itemIDs[i];

                ComCommonBind girdItem = rootBind.LoadExtraBind(path);
                Utility.AttachTo(girdItem.gameObject, root);

                ComEquipHandbookItem equipHandbookItem = girdItem.GetCom<ComEquipHandbookItem>("equipHandbookItem");
                Toggle toggle = girdItem.GetCom<Toggle>("toggle");
                GameObject xuanzhongEffects = girdItem.GetGameObject("xuanzhong");

                int itemId = equipItemData.id;

                equipItemData.bind = girdItem;

                equipHandbookItem.SetItemId(itemId);

                if (tabdata.isShowEquipScore)
                {
                    equipHandbookItem.gostate.CustomActive(true);
                    equipHandbookItem.SetItemState(equipItemData);
                }
                else
                {
                    equipHandbookItem.gostate.CustomActive(false);
                }

                xuanzhongEffects.CustomActive(false);

                toggle.group = mLeftGirdToggleGroup;

                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener( isOn=>
                {
                    xuanzhongEffects.CustomActive(isOn);
                    if (isOn)
                    {
                        mSelectItemID = itemId;

                        _onGirdItemClick(itemId, tabdata, equipItemData,true);
                    }
                });

                if (mSelectItemID == itemId)
                {
                    toggle.isOn = true;
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        IEnumerator _selectDefauteGridItem(EquipHandbookTabData data)
        {
            if (null == data)
            {
                yield break;
            }

            if (data.isShowEquipScore)
            {
                if (mRecommendEquips.Count <= 0)
                {
                    mRightRoot.CustomActive(false);
                    yield break;
                }

                ComCommonBind bind = mRecommendEquips[0].bind;
                if (null == bind)
                {
                    yield break;
                }

                Toggle toggle = bind.GetCom<Toggle>("toggle");

                toggle.isOn = true;
            }
            else
            {
                mRightRoot.CustomActive(false);

                float PosYy = 0;
                float fMinY = mContent.GetComponent<RectTransform>().offsetMax.y;
                float fMaxY = mContent.GetComponent<RectTransform>().offsetMin.y;

                float fHeight = Math.Abs(fMaxY - fMinY);

                int index = 0;
                int splitLevel = EquipHandbookDataManager.GetInstance().GetSplitLevel(data.collectIDs);

                if (splitLevel > 0)
                {
                    for (int i = 0; i < data.collectIDs.Count; i++)
                    {
                        float contentHeight = 0;

                        if (data.collectIDs[i].itemIDs.Count % 4 != 0)
                        {
                            contentHeight = (float)Math.Ceiling((decimal)(data.collectIDs[i].itemIDs.Count / 4));
                        }
                        else
                        {
                            contentHeight = (float)Math.Ceiling((decimal)(data.collectIDs[i].itemIDs.Count / 4)) - 1;
                        }
                        
                        if (splitLevel != data.collectIDs[i].level)
                        {
                            PosYy += 223.0f + contentHeight * 213.0f + 40;
                        }
                        else
                        {
                            index = i;
                            
                            float fPercent = 1f - ((PosYy) / (fHeight - fViewPortPos));

                            mLeftRootScrollRect.verticalNormalizedPosition = fPercent;
                            break;
                        }
                    }
                }
                
                yield return Yielders.GetWaitForSeconds(0.1f);
                
                mRightRoot.CustomActive(true);

                if (data.collectIDs.Count <= 0)
                {
                    mRightRoot.CustomActive(false);
                    yield break;
                }

                EquipHandbookTabCollectionData collectData = data.collectIDs[index];
                if (collectData.itemIDs.Count <= 0)
                {
                    yield break;
                }

                ComCommonBind bind = collectData.itemIDs[0].bind;
                if (null == bind)
                {
                    yield break;
                }

                Toggle toggle = bind.GetCom<Toggle>("toggle");

                toggle.isOn = true;
                yield break;
                
            }
        }

        class CommentNode:System.IComparable<CommentNode>
        {
            public CommentNode(int id, ComCommonBind bind,int sortOrder)
            {
                this.id = id;
                this.bind = bind;
                this.sortOrder = sortOrder;
            }

            public int id { private set; get; }
            public ComCommonBind bind { private set; get; }
            public int count { set; get; }
            public int sortOrder { set; get; }

            public int CompareTo(CommentNode other)
            {
                if (count == other.count)
                {
                    return sortOrder - other.sortOrder;
                }

                return other.count - count;
            }
        }

        List<CommentNode> mCommentNodes = new List<CommentNode>();

        private void _onGirdItemClick(int itemId, EquipHandbookTabData tabdata, EquipHandbookEquipItemData data,bool isFlag)
        {
            if (isFlag)
            {
                if (itemid == itemId)
                {
                    return;
                }
            }
            
            itemid = itemId;
            mcurrentSelectTab = tabdata;
            mcurrentSelectItem = data;

            if (mRightPreviewRoot.gameObject.activeSelf)
            {
                if (_setTryOnBtnIsShow(itemid))
                {
                    _setEquipName(itemid);

                    _initModel(itemid);

                    return;
                }
                else
                {
                    mRightPreviewRoot.CustomActive(false);
                    mRightRoot.CustomActive(true);
                }
            }

            mDetailEquip.SetItemId(itemId);

            if (tabdata.isShowEquipScore)
            {
                mDetailEquip.gostate.CustomActive(true);
                mDetailEquip.SetItemState(data);
            }
            else
            {
                mDetailEquip.gostate.CustomActive(false);
            }

            if (_setTryOnBtnIsShow(itemId))
            {
                mBind.GetSprite("anniu", ref mRightImg);
                mBind.GetSprite("anniu", ref mLeftImg);
                mTryEquipButton.image.raycastTarget = true;
            }
            else
            {
                mBind.GetSprite("grey", ref mRightImg);
                mBind.GetSprite("grey", ref mLeftImg);
                mTryEquipButton.image.raycastTarget = false;
            }


            string commentUnitPath = mBind.GetPrefabPath("commentUnit");
            string sourceUnitPath = mBind.GetPrefabPath("sourceUnit");

            mBind.ClearCacheBinds(commentUnitPath);
            mBind.ClearCacheBinds(sourceUnitPath);

            mSourceEmptyRoot.CustomActive(true);
            mCommentEmptyRoot.CustomActive(true);

            _stopCoroutineCommentNodesAndClear();

            ProtoTable.EquipHandbookAttachedTable table = TableManager.instance.GetTableItem<ProtoTable.EquipHandbookAttachedTable>(itemId);
            if (null == table)
            {
                return;
            }

            List<string> commentIds = new List<string>();

            for (int i = 0; i < table.EquipHandbookCommentIDs.Count; ++i)
            {
                mCommentEmptyRoot.CustomActive(false);

                int commentID = table.EquipHandbookCommentIDs[i];

                ProtoTable.EquipHandbookCommentTable commentTable = TableManager.instance.GetTableItem<ProtoTable.EquipHandbookCommentTable>(commentID);

                if (null == commentTable)
                {
                    continue;
                }


                ComCommonBind bind = mBind.LoadExtraBind(commentUnitPath);
                Utility.AttachTo(bind.gameObject, mRightCommentRoot);

                commentIds.Add(commentID.ToString());
                mCommentNodes.Add(new CommentNode(commentID, bind, commentTable.SortOrder));

                Text likeWord = bind.GetCom<Text>("likeWord");
                GameObject bgBoard = bind.GetGameObject("bgBoard");
                Image likeImage = bind.GetCom<Image>("likeImage");
                Text likeNum = bind.GetCom<Text>("likeNum");
                Button likeButton = bind.GetCom<Button>("likeButton");

                bgBoard.CustomActive(i % 2 == 1);
                likeWord.text = commentTable.Comment;
                likeNum.text = "?";

                int idx = i;

                likeButton.onClick.RemoveAllListeners();
                likeButton.onClick.AddListener(() =>
                {
                    if (mCommentNodes.Count > idx)
                    {
                        mCommentNodes[idx].bind.StartCoroutine(_equipLike(itemId, commentID));
                    }
                });
            }

            if (mCommentNodes.Count > 0)
            {
                mCommentNodes[0].bind.StartCoroutine(_queryLike(itemId, commentIds.ToArray()));
            }

            mCRScrollRect.verticalNormalizedPosition = 1;

            var iter = ItemSourceInfoTableManager.GetInstance().GetSourceInfos(itemId);

            int iSourceCount = 0;

            while (iter.MoveNext())
            {
                mSourceEmptyRoot.CustomActive(false);

                ISourceInfo info = iter.Current as ISourceInfo;

                if (null == info)
                {
                    continue;
                }

                ComCommonBind bind = mBind.LoadExtraBind(sourceUnitPath);
                Utility.AttachTo(bind.gameObject, mRightSourceRoot);

                iSourceCount++;

                Text desc = bind.GetCom<Text>("desc");
                Button onClickLink = bind.GetCom<Button>("onClickLink");

                string linkInfo = ItemSourceInfoTableManager.GetInstance().GetSourceInfoName(info);
                string linkString = ItemSourceInfoTableManager.GetInstance().GetSourceInfoLink(info);

                desc.text = linkInfo;


                onClickLink.onClick.RemoveAllListeners();
                onClickLink.onClick.AddListener(() =>
               {
                   if (ItemSourceInfoUtility.IsLinkFunctionOpen(info))
                   {
                       ActiveManager.GetInstance().OnClickLinkInfo(linkString);
                       ClientSystemManager.instance.CloseFrame(this);
                   }
                   else
                   {
                       SystemNotifyManager.SystemNotify(1013);
                   }
               });
            }

            mSRScrollRect.verticalNormalizedPosition = 1;

            if (iSourceCount > 6)
            {
                mSRScrollRect.enabled = true;
            }
            else
            {
                mSRScrollRect.enabled = false;
            }
        }

        private void _stopCoroutineCommentNodesAndClear()
        {
            for (int i = 0; i < mCommentNodes.Count; ++i)
            {
                if (mCommentNodes[i] != null && mCommentNodes[i].bind != null)
                {
                    mCommentNodes[i].bind.StopAllCoroutines();
                }
            }

            mCommentNodes.Clear();
        }

        protected override void _OnCloseFrame()
        {
            _clear();
            _stopCoroutineCommentNodesAndClear();
            _stopCoroutineSelectCurrentLevelGridItem();
            _stopCoroutineCurrentTabClick();
        }

        private readonly string kCommentAddress = ClientApplication.commentServerAddr;

        private IEnumerator _queryLike(int itemId, string[] commentids)
        {
            //192.168.2.26:19359/query_likes?accId=1&roleId=2&itemId=3&comments=4

            BaseWaitHttpRequest req = new BaseWaitHttpRequest();

            req.url = string.Format("http://{0}/query_likes?accId={1}&roleId={2}&itemId={3}&comments={4}",
                kCommentAddress,
                ClientApplication.playerinfo.accid,
                ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().roleId,
                itemId, string.Join(",", commentids));

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                EquipHandbookEquipQueryData data = req.GetResultJson<EquipHandbookEquipQueryData>();
                _refreshCommentNode(data,true);
            }
            else
            {

            }
        }

        private void _refreshCommentNode(EquipHandbookEquipQueryData data,bool isRefresh)
        {
            if (null == mBind)
            {

                return;
            }

            if (null == data || 0 != data.code)
            {
                return;
            }

            for (int i = 0; i < data.itemcomments.Count; ++i)
            {
                EquipHandbookCommentData commentData = data.itemcomments[i];

                CommentNode node = _findCommentNode(commentData.id);

                if (null != node)
                {
                    ComCommonBind bind = node.bind;
                    node.count = commentData.count;

                    Image likeImage = bind.GetCom<Image>("likeImage");
                    Text likeNum = bind.GetCom<Text>("likeNum");

                    if (commentData.selflike == 1)
                    {
                        mBind.GetSprite("containselflike", ref likeImage);
                    }
                    else
                    {
                        mBind.GetSprite("otherlike", ref likeImage);
                    }

                    likeNum.text = commentData.count.ToString();
                }
            }

            if (isRefresh)
            {
                _refreshEquipHandbookCommentSort();
            }
        }

        private void _refreshEquipHandbookCommentSort()
        {
            mCommentNodes.Sort();

            for (int i = 0; i < mCommentNodes.Count; i++)
            {
                ProtoTable.EquipHandbookCommentTable commentTable = TableManager.instance.GetTableItem<ProtoTable.EquipHandbookCommentTable>(mCommentNodes[i].id);

                if (null == commentTable)
                {
                    continue;
                }
                ComCommonBind bind = mCommentNodes[i].bind;
                Text likeWord = bind.GetCom<Text>("likeWord");
                GameObject bgBoard = bind.GetGameObject("bgBoard");
                Image likeImage = bind.GetCom<Image>("likeImage");
                Text likeNum = bind.GetCom<Text>("likeNum");
                bgBoard.CustomActive(i % 2 == 1);
                likeWord.text = commentTable.Comment;
                likeNum.text = mCommentNodes[i].count.ToString();
                bind.transform.SetAsLastSibling();
            }
        }
        private CommentNode _findCommentNode(int commentId)
        {
            for (int i = 0; i < mCommentNodes.Count; ++i)
            {
                if (mCommentNodes[i].id == commentId)
                {
                    return mCommentNodes[i];
                }
            }

            return null;
        }

        private IEnumerator _equipLike(int itemId, int commentId)
        {
            BaseWaitHttpRequest req = new BaseWaitHttpRequest();

            req.url = string.Format("http://{0}/like_item?accId={1}&roleId={2}&itemId={3}&commentId={4}",
                kCommentAddress,
                ClientApplication.playerinfo.accid,
                ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().roleId,
                itemId, commentId);

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                EquipHandbookEquipQueryData data = req.GetResultJson<EquipHandbookEquipQueryData>();
                _refreshCommentNode(data,false);
            }
            else
            {

            }
        }

        private void _showTips(int itemId)
        {
            ItemData data = ItemDataManager.GetInstance().GetCommonItemTableDataByID(itemId);
            data.RefreshRateScore();
            if (data != null)
            {
                ItemTipManager.GetInstance().ShowTip(data);
            }
        }

        private void _clear()
        {
            _clearModel();
            itemid = 0;
            mSelectItemID = 0;
            bIsPlayAnimation = false;

            mLastSelectedIndex = -1;
            mLeftRootComUIList.onItemVisiable = null;

            mcurrentSelectTab = null;
            mcurrentSelectItem = null;
        }

    }
}
