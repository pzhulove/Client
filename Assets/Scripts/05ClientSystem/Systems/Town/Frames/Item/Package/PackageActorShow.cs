using DG.Tweening;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PackageActorShow : MonoBehaviour
    {
        [SerializeField] private Image mSeasonSubLv;
        [SerializeField] private Image mSeasonMainLv;
        [SerializeField] private Text mTextLv;
        [SerializeField] private Text mTextJob;
        [SerializeField] private Text mTextName;

        [SerializeField] private Text mTextZhandouliScore;

        [SerializeField] private CanvasGroup mFashionWeaponGroup;
        [SerializeField] private Button mButtonFashionWeapon;
        [SerializeField] private Image mFashionWeaponCheck;
        [SerializeField] private GeAvatarRendererEx mAvatar;
        [SerializeField] private Button mButtonSwitchWeapon;
        [SerializeField] private Image mImageHonor;
        [SerializeField] private Image mImageTitle;
        [SerializeField] private Text mTextTitle;
        [SerializeField] private RectTransform mTitleRectTransform;
        [SerializeField] private string mEquipsPrefabPath;
        [SerializeField] private string mFashionEquipsPrefabPath;
        [SerializeField] private Toggle mToggleAttrDetail;
        [SerializeField] private Transform mEquipsRoot;
        [SerializeField] private CanvasGroup mAttribute2Group;
        [SerializeField] private Image mImageAttribute1;
        [SerializeField] private Text mTextAttribute1;
        [SerializeField] private Image mImageAttribute2;
        [SerializeField] private Text mTextAttribute2;
        [SerializeField] private SpriteAniRenderChenghao mTitleAnim;

        [SerializeField] private GameObject mObjPvpIcon;

        private DOTweenAnimation mWeaponAnim;
        private DOTweenAnimation mBackUpWeaponAnim;
        private GeAvatarRendererExIdleController mAvatarController;

        private CanvasGroup mEquipsGroup;
        private CanvasGroup mFashionEquipsGroup;
        private ClientFrame mFrame;
        private Action<GameObject, IItemDataModel> mOnWearItemClick;
        private Action<bool> mOnDetailValueChanged;

        public void Init(ClientFrame frame, Action<GameObject, IItemDataModel> onWearClick, Action<bool> onDetailValueChanged)
        {
            mFrame = frame;
            mOnWearItemClick = onWearClick;
            mOnDetailValueChanged = onDetailValueChanged;
            RefreshBaseInfo();
            RefreshMainAttribute();
            ShowAvatar();
            UpdatePvpIconShow();
        }

        public void OnUpdate(float timeElapsed)
        {
            if (null != mAvatarController)
            {
                mAvatarController.OnUpdate(timeElapsed);
            }
        }

        public void Clear()
        {
            if (mAvatar != null)
            {
                mAvatar.ClearAvatar();
            }
        }

        public void RefreshAvatar()
        {
            if (mAvatar != null)
            {
                //Logger.LogErrorFormat("_RefreshModel in PackageNewFrame !!!!!!!!!!");

                PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mAvatar, null, true);
                mAvatar.AttachAvatar("Aureole", "Effects/Scene_effects/EffectUI/Eff_chuangjue/Prefab/EffUI_chuangjue_beibao", "[actor]Orign", false);
            }
        }

        public void RefreshMode(int mode)
        {
            PackageNewFrame.TabMode tabMode = (PackageNewFrame.TabMode)mode;
            mFashionWeaponGroup.CustomActive(tabMode == PackageNewFrame.TabMode.TM_FASHION);
            switch (tabMode)
            {
                case PackageNewFrame.TabMode.TM_EQUIP:
                    mFashionWeaponGroup.CustomActive(false);
                    mButtonSwitchWeapon.CustomActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.SideWeapon));
                    _ShowEquips();
                    break;
                case PackageNewFrame.TabMode.TM_FASHION:
                    mButtonSwitchWeapon.CustomActive(false);
                    mFashionWeaponGroup.CustomActive(true);
                    mFashionWeaponCheck.enabled = PlayerBaseData.GetInstance().isShowFashionWeapon;
                    RefreshFashionWeapon();
                    _ShowFashionEquips();
                    break;
                case PackageNewFrame.TabMode.TM_TITLE:
                    mButtonSwitchWeapon.CustomActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.SideWeapon));
                    mFashionWeaponGroup.CustomActive(false);
                    _ShowEquips();
                    break;
                case PackageNewFrame.TabMode.TM_PET:
                    mButtonSwitchWeapon.CustomActive(false);
                    mFashionWeaponGroup.CustomActive(false);
                    _ClearModel();
                    break;
            }
        }

        public void RefreshFashionWeapon()
        {
            mFashionWeaponCheck.enabled = PlayerBaseData.GetInstance().isShowFashionWeapon;
        }

        public void OnFashionWeaponClick()
        {
            mFashionWeaponCheck.enabled = !mFashionWeaponCheck.enabled;
            PackageDataManager.GetInstance().SendShowFashionWeaponReq(mFashionWeaponCheck.enabled);
        }

        public void OnAttrDetailValueChanged(bool value)
        {
            mOnDetailValueChanged?.Invoke(value);
        }

        public void SetAttrDetailToggleActive(bool value)
        {
            mToggleAttrDetail.CustomActive(value);
        }

        public void OnSwitchWeaponFrame()
        {
            ulong mainWeaponId = ItemDataManager.GetInstance().GetMainWeapon();
            var backWeaponId = ItemDataManager.GetInstance().GetBackWeapon();
            mWeaponAnim.DORestart();
            mBackUpWeaponAnim.DORestart();
            StartCoroutine(_AniBack());

            if (backWeaponId > 0)
            {
                SwitchWeaponDataManager.GetInstance().TakeOnSideWeapon(1, 0);
            }

            if (mainWeaponId > 0)
            {
                ItemDataManager.GetInstance().SwitchWeapon(ItemDataManager.GetInstance().GetItem(mainWeaponId));
            }

            else if (backWeaponId > 0)
            {
                ItemDataManager.GetInstance().SwitchWeapon(ItemDataManager.GetInstance().GetItem(backWeaponId));
            }

            if (mainWeaponId > 0)
            {
                SwitchWeaponDataManager.GetInstance().TakeOnSideWeapon(1, mainWeaponId);
            }

            //ClientSystemManager.GetInstance().OpenFrame<SwitchWeaponFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("SwitchWeapon");
        }

        IEnumerator _AniBack()
        {
            yield return new WaitForSeconds(mWeaponAnim.duration / 2);
            mWeaponAnim.DOPlayBackwards();
            mBackUpWeaponAnim.DOPlayBackwards();
        }

        public void RefreshMainAttribute()
        {
            BeEntityData data = BeUtility.GetMainPlayerActor().GetEntityData();
            var attribute = BeEntityData.GetActorAttributeForDisplay(data);
            var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            var attrIndexList = jobTableItem.RecommendedAttributeIndex;
            if (attrIndexList.Count > 0)
            {
                var tableData = TableManager.GetInstance().GetTableItem<AttrDescTable>(attrIndexList[0]);
                ETCImageLoader.LoadSprite(ref mImageAttribute1, tableData.IconPath);
                mTextAttribute1.SafeSetText(_GetAttrDesc(tableData, attribute, data));
                if (attrIndexList.Count > 1)
                {
                    tableData = TableManager.GetInstance().GetTableItem<AttrDescTable>(attrIndexList[1]);
                    if (tableData != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageAttribute2, tableData.IconPath);
                        mAttribute2Group.CustomActive(true);
                        mTextAttribute2.SafeSetText(_GetAttrDesc(tableData, attribute, data));
                    }
                    else
                    {
                        Logger.LogWarningFormat("背包界面 主属性错误！职业表中职业id为{0}的那一行中配置的推荐属性：id {1} 在属性说明表中找不到!!!!!!!", PlayerBaseData.GetInstance().JobTableID, attrIndexList[1]);
                    }
                }
                else
                {
                    mAttribute2Group.CustomActive(false);
                }

            }
        }

        private string _GetAttrDesc(AttrDescTable tableData, DisplayAttribute attribute, BeEntityData entityData)
        {
            var bData = entityData.battleData;
            float param = 0f;
            switch ((EEquipProp)tableData.ID)
            {
                case EEquipProp.Strenth:
                    param = attribute.baseSta;
                    break;
                case EEquipProp.Stamina:
                    param = attribute.baseSta;
                    break;
                case EEquipProp.PhysicsAttack:
                    param = bData.displayAttack;
                    break;
                case EEquipProp.MagicAttack:
                    param = bData.displayMagicAttack;
                    break;
                case EEquipProp.PhysicsDefense:
                    param = GameUtility.Item.GetReduceRate(entityData.level, bData.fDefence);
                    break;
                case EEquipProp.MagicDefense:
                    param = GameUtility.Item.GetReduceRate(entityData.level, bData.fMagicDefence);
                    break;
                case EEquipProp.PhysicCritRate:
                    param = attribute.ciriticalAttack;
                    break;
                case EEquipProp.MagicCritRate:
                    param = attribute.ciriticalMagicAttack;
                    break;
                case EEquipProp.AttackSpeedRate:
                    param = attribute.attackSpeed;
                    break;
                case EEquipProp.FireSpeedRate:
                    param = attribute.spellSpeed;
                    break;
                case EEquipProp.MoveSpeedRate:
                    param = attribute.moveSpeed;
                    break;
                case EEquipProp.HitRate:
                    param = attribute.dex;
                    break;
                case EEquipProp.AvoidRate:
                    param = attribute.dodge;
                    break;
                case EEquipProp.HPRecover:
                    param = (attribute.hpRecover);
                    break;
                case EEquipProp.MPRecover:
                    param = (attribute.mpRecover);
                    break;
                case EEquipProp.Intellect:
                    param = attribute.baseInt;
                    break;
                case EEquipProp.Spirit:
                    param = attribute.baseSpr;
                    break;
                case EEquipProp.HPMax:
                    param = attribute.hp;
                    break;
                case EEquipProp.MPMax:
                    param = attribute.mp;
                    break;
            }

            return TR.Value("package_main_attribute", tableData.Name, param);
        }

        public void RefreshBaseInfo()
        {
            string jobName = "";
            JobTable job = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (job != null)
            {
                jobName = job.Name;
            }
            mTextJob.SafeSetText(jobName);
            mTextName.SafeSetText(PlayerBaseData.GetInstance().Name);
            mTextLv.SafeSetText(TR.Value("player_level", PlayerBaseData.GetInstance().Level));

            // marked by ckm
            _ShowPlayerScore();

            RefreshHead();
            RefreshTitle();
        }

        private void _ShowPlayerScore()
        {
            int score = 0;
            var tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        score += item.finalRateScore;
                    }
                }
            }

            //装备的时装
            tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        score += item.finalRateScore;
                    }
                }
            }

            // m_labPlayerScore.text = string.Format("冒险家名望：{0}", score);
            mTextZhandouliScore.SafeSetText(score + "");
        }

        private void OnLoadEquipsSuccess(string path, object obj, object param)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                mEquipsGroup = go.GetOrAddComponent<CanvasGroup>();
                go.transform.SetParent(mEquipsRoot, false);
                var bind = go.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    mWeaponAnim = bind.GetCom<DOTweenAnimation>("MainWeaponAnim");
                    mBackUpWeaponAnim = bind.GetCom<DOTweenAnimation>("BackWeaponAnim");
                }
                RefreshEquips();
            }
        }

        private void OnLoadFashionEquipsSuccess(string path, object obj, object param)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                mFashionEquipsGroup = go.GetOrAddComponent<CanvasGroup>();
                go.transform.SetParent(mEquipsRoot, false);
                RefreshFashionEquips();
            }
        }

        public void RefreshEquips()
        {
            
            if (mEquipsGroup == null)
            {
                return;
            }

            var equipItems = mEquipsGroup.GetComponentsInChildren<PackageActorEquipItem>();
            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipItems.Length; ++i)
            {
                var comItem = equipItems[i].GetComItem();
                bool isFind = false;
                if (comItem != null)
                {
                    for (int j = 0; j < equipIDs.Count; ++j)
                    {
                        ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[j]);
                        if (item != null && item.EquipWearSlotType == equipItems[i].SlotType)
                        {
                            comItem.Setup(item, mOnWearItemClick);
                            isFind = true;
                            break;
                        }
                    }
                }

                if (!isFind)
                {
                    comItem.ShowEmpty();
                }
            }

             
        }

        private void _ShowEquips()
        {
            mFashionEquipsGroup.CustomActive(false);
            if (mEquipsGroup == null)
            {
                UIManager.GetInstance().LoadObject(mFrame, mEquipsPrefabPath, null, OnLoadEquipsSuccess, typeof(GameObject));
            }
            else
            {
                mEquipsGroup.CustomActive(true);
                RefreshEquips();
            }
        }

        private void _ShowFashionEquips()
        {
            mEquipsGroup.CustomActive(false);
            if (mFashionEquipsGroup == null)
            {
                UIManager.GetInstance().LoadObject(mFrame, mFashionEquipsPrefabPath, null, OnLoadFashionEquipsSuccess, typeof(GameObject));
            }
            else
            {
                mFashionEquipsGroup.CustomActive(true);
                RefreshFashionEquips();
            }

        }

        public void RefreshFashionEquips()
        {
            if (mFashionEquipsGroup == null)
            {
                return;
            }

            var equipItems = mFashionEquipsGroup.GetComponentsInChildren<PackageActorEquipItem>();
            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            for (int i = 0; i < equipItems.Length; ++i)
            {
                var comItem = equipItems[i].GetComItem();
                bool isFind = false;
                if (comItem != null)
                {
                    for (int j = 0; j < equipIDs.Count; ++j)
                    {
                        ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[j]);
                        if (item != null && GameUtility.Item.GetFashionNewSlotTypeByOld(item.FashionWearSlotType) == equipItems[i].FashionSlotType)
                        {
                            comItem.Setup(item, mOnWearItemClick);
                            isFind = true;
                            break;
                        }
                    }
                }

                if (!isFind)
                {
                    comItem.ShowEmpty();
                }
            }
        }

        public void ShowAvatar()
        {
            ProtoTable.JobTable job = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);

            if (job != null)
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                if (res == null)
                {
                    Logger.LogError("职业ID Mode表 找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
                }
                else
                {
                    mAvatar.LoadAvatar(res.ModelPath);

                    PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mAvatar);

                    mAvatar.AttachAvatar("Aureole", "Effects/Scene_effects/EffectUI/Eff_chuangjue/Prefab/EffUI_chuangjue_beibao", "[actor]Orign", false);
                    //mAvatar.ChangeAction(mActionTable[0], 1.0f, true);
                }
            }
        }

        //称号
        public void RefreshTitle()
        {
            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            ItemData title = null;
            for (int j = 0; j < equipIDs.Count; ++j)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[j]);
                if (item != null && item.EquipWearSlotType == EEquipWearSlotType.Equiptitle)
                {
                    title = item;
                    break;
                }
            }

            if (null != mTitleAnim)
            {
                if (title != null)
                {
                    mTitleAnim.SetEnable(false);
                    if (null != title.TableData)
                    {
                        if (title.TableData.Path2.Count == 4)
                        {
                            int id = 0;
                            float fValue = 0.0f;
                            if (int.TryParse(title.TableData.Path2[2], out id) && float.TryParse(title.TableData.Path2[3], out fValue))
                            {
                                mTitleAnim.Reset(title.TableData.Path2[0], title.TableData.Path2[1], id, fValue, title.TableData.ModelPath);
                                mTitleAnim.SetEnable(true);
                            }
                        }
                    }
                }
                else
                {
                    mTitleAnim.SetEnable(false);
                }
            }
        }

        //头衔
        public void RefreshHead()
        {
            var weardTitleInfo = PlayerBaseData.GetInstance().WearedTitleInfo;
            bool isHaveTitle = TitleDataManager.GetInstance().IsHaveTitle();

            if (!isHaveTitle)
            {
                mTextTitle.enabled = false;
                mImageHonor.enabled = false;
                mImageTitle.enabled = false;
                return;
            }
            if (weardTitleInfo != null)
            {
                if (weardTitleInfo.style == (int)TitleDataManager.eTitleStyle.Txt)
                {
                    mTextTitle.enabled = true;
                    mImageHonor.enabled = false;
                    mImageTitle.enabled = false;

                    mTextTitle.SafeSetText(weardTitleInfo.name);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(mTitleRectTransform);
                }
                else if (weardTitleInfo.style == (int)TitleDataManager.eTitleStyle.Img)
                {
                    mTextTitle.enabled = false;
                    mImageHonor.enabled = false;
                    mImageTitle.enabled = true;
                    var titleItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)weardTitleInfo.titleId);
                    if (titleItem != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageTitle, titleItem.path);
                    }
                }
                else if (weardTitleInfo.style == (int)TitleDataManager.eTitleStyle.Group)
                {
                    mTextTitle.enabled = true;
                    mImageHonor.enabled = true;
                    mImageTitle.enabled = false;
                    mTextTitle.SafeSetText(weardTitleInfo.name);
                    var titleItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)weardTitleInfo.titleId);
                    if (titleItem != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageHonor, titleItem.path);
                    }
                    LayoutRebuilder.ForceRebuildLayoutImmediate(mTitleRectTransform);
                }
            }
        }

        private void _ClearModel()
        {
            if (null != mAvatarController)
            {
                mAvatarController.Clear();
            }
        }

        private void Start()
        {
            mAvatarController = new GeAvatarRendererExIdleController();
            mAvatarController.Init(mAvatar);
        }

        private void OnDestroy()
        {
            if (null != mAvatarController)
            {
                mAvatarController.UnInit();
            }
            mAvatarController = null;
        }

        //设置pvpIcon显示
        public void UpdatePvpIconShow()
        {
            mObjPvpIcon.CustomActive(Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Duel));
        }
    }
}