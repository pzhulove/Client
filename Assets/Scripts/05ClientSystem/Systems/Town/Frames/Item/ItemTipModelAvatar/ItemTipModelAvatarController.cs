using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ItemTipModelAvatarController : MonoBehaviour
    {
        //点击的道具
        private ItemTable _clickedItemTable;
        //模型的层级
        private int _itemTipModelAvatarLayerIndex;

        //礼包相关
        private GiftPackTable.eShowAvatarModelType _giftPackShowModelAvatarType;
        private List<ItemTable> _giftPackShowItemTableList;
        //礼包下面包含整套预览的礼包
        private bool _isOwnerCompleteShowType = false;

        //其他职业的角色(全是非本职业)
        private int _otherPlayerProfessionId = 0;
        private List<int> _otherPlayerProfessionIdList = null;
        private int _otherPlayerProfessionIdIndex = 1;

        //混合职业的角色(当前职业和其他的基础职业)
        private int _mixPlayerProfessionId = 0;
        private List<int> _mixPlayerProfessionIdList = null;
        private int _mixPlayerProfessionIdIndex = 1;

        //礼包Match类型的宠物的Avatar
        private GeAvatarRendererEx _followPetAvatarEx;

        //礼包：分开预览：第几个道具: 1->MaxNumber
        private int _giftPackSelectItemTableIndex = 1;
        private ItemTipModelAvatarEnumerationController _itemTipModelAvatarEnumerationController;

        [Space(10)] [HeaderAttribute("ItemName")] 
        [SerializeField] private Text itemNameLabel;

        [Space(10)] [HeaderAttribute("ItemTitle")] 
        [SerializeField] private GameObject itemTitleRoot;
        [SerializeField] private SpriteAniRenderChenghao spriteAniRenderChangeHao;
        [SerializeField] private Image spriteAniImage;

        [Space(10)]
        [HeaderAttribute("GeAvatar")]
        [SerializeField] private GeAvatarRendererEx geAvatarRenderEx;

        [Space(10)]
        [HeaderAttribute("followPet")]
        [SerializeField] private GameObject followPetAvatarRoot;

        [Space(10)] [HeaderAttribute("ChangeAvatarButton")] [Space(10)]
        [SerializeField] private GameObject avatarChangeRoot;

        #region Init
        private void OnDestroy()
        {
            ClearData();
        }
        
        private void ClearData()
        {
            _clickedItemTable = null;
            _itemTipModelAvatarLayerIndex = 0;
            
            ResetGiftPackDataModel();

            _followPetAvatarEx = null;

            _giftPackSelectItemTableIndex = 1;
            _itemTipModelAvatarEnumerationController = null;

            _otherPlayerProfessionId = 0;
            _otherPlayerProfessionIdList = null;
            _otherPlayerProfessionIdIndex = 1;
        }

        private void ResetGiftPackDataModel()
        {
            _giftPackShowModelAvatarType = GiftPackTable.eShowAvatarModelType.None;
            _giftPackShowItemTableList = null;
            _isOwnerCompleteShowType = false;
        }
        #endregion

        
        public void UpdateModelAvatarController(ItemTable itemTable,
            int itemTipModelAvatarLayerIndex,
            GiftPackModelAvatarDataModel giftPackModelAvatarDataModel = null,
            int otherProfessionId = 0)
        {
            _clickedItemTable = itemTable;
            _itemTipModelAvatarLayerIndex = itemTipModelAvatarLayerIndex;

            ResetGiftPackDataModel();
            if (giftPackModelAvatarDataModel != null)
            {
                _giftPackShowItemTableList = giftPackModelAvatarDataModel.GiftPackShowItemTableList;
                _giftPackShowModelAvatarType = giftPackModelAvatarDataModel.GiftPackShowModelAvatarType;
                _isOwnerCompleteShowType = giftPackModelAvatarDataModel.IsOwnerCompleteShowType;
            }

            _otherPlayerProfessionId = otherProfessionId;

            if (_clickedItemTable == null)
                return;

            if (geAvatarRenderEx != null)
            {
                //geAvatarRenderEx.m_Layer = _itemTipModelAvatarLayerIndex;
            }

            UpdateModelAvatar();
        }

        private void UpdateModelAvatar()
        {
            CommonUtility.UpdateGameObjectVisible(itemTitleRoot, false);
            CommonUtility.UpdateTextVisible(itemNameLabel, false);
            CommonUtility.UpdateGameObjectVisible(avatarChangeRoot, false);

            if (_clickedItemTable.SubType == ItemTable.eSubType.PetEgg)
            {
                //宠物蛋
                InitPetAvatar(_clickedItemTable);
            }
            else if (_clickedItemTable.Type == ItemTable.eType.VirtualPack || 
                _clickedItemTable.SubType == ItemTable.eSubType.GiftPackage)
            {
                //礼包
                InitGiftPackModelAvatar();
            }
            else
            {
                //非本职业（武器和时装武器）
                if (_otherPlayerProfessionId > 0)
                {
                    //不是本职业的道具，显示其他职业的模型
                    ShowOtherPlayerAvatar();
                }
                else
                {
                    //if (_clickedItemTable.SubType == ItemTable.eSubType.WEAPON
                    //    || _clickedItemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
                    //{
                    //    //（武器，时装武器类型)
                    //    //包含本职业，（可能是多种职业分开预览)
                    //    ShowPlayerAvatarByWeaponType();
                    //}
                    //else
                    //{
                    //    UpdateSelfPlayerByItemTable(_clickedItemTable);
                    //}

                    //更新自己的模型
                    UpdateSelfPlayerByItemTable(_clickedItemTable);
                }
            }
        }

        #region selfPlayerAvatar
        private void InitPlayerAvatar()
        {
            PlayerUtility.LoadPlayerAvatarBySelfPlayer(geAvatarRenderEx);
        }

        //更新自己身上的一个道具
        private void UpdateSelfPlayerByItemTable(ItemTable itemTable)
        {
            if (itemTable == null)
                return;

            InitPlayerName(itemTable);
            InitPlayerAvatar();
            UpdateModelAvatarByItemTable(itemTable);
        }

        #endregion

        #region ItemNameAndTitle

        //角色的称号
        private void InitPlayerTitle(ItemTable itemTable)
        {
            if (itemTable == null)
                return;

            if (itemTable.Path2.Count < 4)
                return;

            var titlePathList = itemTable.Path2.ToList();

            CommonUtility.UpdateGameObjectVisible(itemTitleRoot, true);
            if (spriteAniRenderChangeHao != null)
            {
                var titlePath = titlePathList[0];
                var titleName = titlePathList[1];

                var titleCount = 0;
                var titleScale = 0.0f;
                //解析成功
                if (int.TryParse(titlePathList[2], out titleCount)
                    && float.TryParse(titlePathList[3], out titleScale))
                {
                    spriteAniRenderChangeHao.Reset(titlePath,
                        titleName,
                        titleCount,
                        titleScale,
                        itemTable.ModelPath);
                }
            }

            if (spriteAniImage != null)
                spriteAniImage.enabled = true;
        }

        private void InitPlayerName(ItemTable itemTable)
        {
            if (itemNameLabel == null)
                return;

            if (itemTable == null)
                return;

            var itemNameStr = CommonUtility.GetItemColorName(itemTable);
            itemNameLabel.text = itemNameStr;

            CommonUtility.UpdateTextVisible(itemNameLabel, true);
        }

        #endregion

        #region MixPlayer

        //武器类型的预览（武器和时装武器)
        private void ShowPlayerAvatarByWeaponType()
        {
            if (_mixPlayerProfessionIdList == null)
                _mixPlayerProfessionIdList = PlayerUtility.GetItemTableSuitMixProfessionIdList(_clickedItemTable);

            if (_mixPlayerProfessionIdList == null || _mixPlayerProfessionIdList.Count <= 1)
            {
                //只有本角色的职业
                UpdateSelfPlayerByItemTable(_clickedItemTable);
            }
            else
            {
                //多个职业,分开预览
                ShowMixPlayerAvatarByEnumerationType();
            }
        }

        //显示多个职业,需要分开预览
        private void ShowMixPlayerAvatarByEnumerationType()
        {
            //根节点不存在
            if (avatarChangeRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(avatarChangeRoot, true);

            if (_itemTipModelAvatarEnumerationController == null)
            {
                var enumerationControllerPrefab = CommonUtility.LoadGameObject(avatarChangeRoot);
                if (enumerationControllerPrefab != null)
                {
                    _itemTipModelAvatarEnumerationController = enumerationControllerPrefab
                        .GetComponent<ItemTipModelAvatarEnumerationController>();
                }

                if (_itemTipModelAvatarEnumerationController != null)
                {
                    _itemTipModelAvatarEnumerationController.InitController(
                        _mixPlayerProfessionIdList,
                        _mixPlayerProfessionIdIndex,
                        OnMixPlayerAvatarChangeActionInEnumerationType);
                }
            }
            else
            {
                _itemTipModelAvatarEnumerationController.OnEnableController(_mixPlayerProfessionIdIndex);
            }

            ShowMixPlayerAvatarInEnumerationType();
        }

        //其他角色的回掉
        private void OnMixPlayerAvatarChangeActionInEnumerationType(int selectedIndex)
        {
            if (_mixPlayerProfessionIdIndex == selectedIndex)
                return;

            _mixPlayerProfessionIdIndex = selectedIndex;

            ShowMixPlayerAvatarInEnumerationType();
        }

        //显示一个其他角色
        private void ShowMixPlayerAvatarInEnumerationType()
        {
            var showIndex = _mixPlayerProfessionIdIndex - 1;
            if (showIndex < 0)
                showIndex = 0;
            else if (showIndex >= _mixPlayerProfessionIdList.Count)
            {
                showIndex = _mixPlayerProfessionIdList.Count - 1;
            }

            var mixPlayerProfessionId = _mixPlayerProfessionIdList[showIndex];
            //显示本职业
            if (mixPlayerProfessionId == PlayerBaseData.GetInstance().JobTableID)
            {
                UpdateSelfPlayerByItemTable(_clickedItemTable);
            }
            else
            {
                //显示其他的基础职业
                ShowOneOtherPlayerAvatarByProfessionId(mixPlayerProfessionId);
            }
        }

        #endregion

        #region OtherPlayer
        //显示其他职业的道具模型
        private void ShowOtherPlayerAvatar()
        {
            //第一次,获得
            if(_otherPlayerProfessionIdList == null)
                _otherPlayerProfessionIdList = PlayerUtility.GetItemTableSuitBaseProfessionIdList(_clickedItemTable);

            //只有一个职业
            if (_otherPlayerProfessionIdList == null || _otherPlayerProfessionIdList.Count <= 1)
            {
                ShowOneOtherPlayerAvatarByProfessionId(_otherPlayerProfessionId);
            }
            else
            {
                ShowOtherPlayerAvatarByEnumerationType();
            }
        }

        //显示一个职业的道具模型
        private void ShowOneOtherPlayerAvatarByProfessionId(int professionId)
        {
            InitPlayerName(_clickedItemTable);
            PlayerUtility.LoadPlayerAvatarByProfessionId(geAvatarRenderEx,
                professionId);

            var itemTableSubType = _clickedItemTable.SubType;
            if (itemTableSubType == ItemTable.eSubType.WEAPON
                || itemTableSubType == ItemTable.eSubType.FASHION_WEAPON)
            {
                PlayerBaseData.GetInstance().AvatarEquipWeapon(geAvatarRenderEx,
                    professionId,
                    _clickedItemTable.ID);
            }
        }

        //显示多个职业,需要分开预览
        private void ShowOtherPlayerAvatarByEnumerationType()
        {
            //根节点不存在
            if (avatarChangeRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(avatarChangeRoot, true);

            if (_itemTipModelAvatarEnumerationController == null)
            {
                var enumerationControllerPrefab = CommonUtility.LoadGameObject(avatarChangeRoot);
                if (enumerationControllerPrefab != null)
                {
                    _itemTipModelAvatarEnumerationController = enumerationControllerPrefab
                        .GetComponent<ItemTipModelAvatarEnumerationController>();
                }

                if (_itemTipModelAvatarEnumerationController != null)
                {
                    _itemTipModelAvatarEnumerationController.InitController(
                        _otherPlayerProfessionIdList,
                        _otherPlayerProfessionIdIndex,
                        OnOtherPlayerAvatarChangeActionInEnumerationType);
                }
            }
            else
            {
                _itemTipModelAvatarEnumerationController.OnEnableController(_otherPlayerProfessionIdIndex);
            }

            ShowOneOtherPlayerAvatarInEnumerationType();
        }

        //其他角色的回掉
        private void OnOtherPlayerAvatarChangeActionInEnumerationType(int selectedIndex)
        {
            if (_otherPlayerProfessionIdIndex == selectedIndex)
                return;

            _otherPlayerProfessionIdIndex = selectedIndex;

            ShowOneOtherPlayerAvatarInEnumerationType();
        }

        //显示一个其他角色
        private void ShowOneOtherPlayerAvatarInEnumerationType()
        {
            var showIndex = _otherPlayerProfessionIdIndex - 1;
            if (showIndex < 0)
                showIndex = 0;
            else if (showIndex >= _otherPlayerProfessionIdList.Count)
            {
                showIndex = _otherPlayerProfessionIdList.Count - 1;
            }

            var showProfessionId = _otherPlayerProfessionIdList[showIndex];
            ShowOneOtherPlayerAvatarByProfessionId(showProfessionId);
        }
        #endregion

        #region PetEggAvatar
        private void InitPetAvatar(ItemTable itemTable)
        {
            var petId = ItemDataUtility.GetPetIdByItemTable(itemTable);

            var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petId);
            if (petTable == null)
                return;

            InitPetNameLabel(petTable);

            PlayerUtility.LoadPetAvatarRenderEx(petId, geAvatarRenderEx);
        }

        private void InitPetNameLabel(PetTable petTable)
        {
            if (itemNameLabel == null)
                return;

            if (petTable == null)
                return;

            CommonUtility.UpdateTextVisible(itemNameLabel, true);

            var itemNameStr = CommonUtility.GetPetItemName(petTable);
            
            itemNameLabel.text = itemNameStr;
        }


        #endregion

        #region GiftPackModelAvatar

        private void InitGiftPackModelAvatar()
        {
            if (_giftPackShowModelAvatarType == GiftPackTable.eShowAvatarModelType.None)
                return;

            if (_giftPackShowItemTableList == null || _giftPackShowItemTableList.Count <= 0)
                return;

            //分类展示
            ShowGiftPackModelAvatar();
        }

        private void ShowGiftPackModelAvatar()
        {
            switch (_giftPackShowModelAvatarType)
            {
                case GiftPackTable.eShowAvatarModelType.Single:
                    //单一预览
                    ShowGiftPackModelAvatarBySingleShow();
                    break;
                case GiftPackTable.eShowAvatarModelType.Complete:
                    //整套预览
                    ShowGiftPackModelAvatarByCombinationShow();
                    break;
                case GiftPackTable.eShowAvatarModelType.Enumeration:
                    //分开预览
                    ShowGiftPackModelAvatarByEnumerationShow();
                    break;
                case GiftPackTable.eShowAvatarModelType.Combination:
                    //组合预览
                    ShowGiftPackModelAvatarByCombinationShow();
                    break;
                case GiftPackTable.eShowAvatarModelType.Matching:
                    //道具和宠物预览
                    ShowGiftPackModelAvatarByMatchingShow();
                    break;
                case GiftPackTable.eShowAvatarModelType.CompleteEnumeration:
                    //礼包分开预览
                    ShowGiftPackModelAvatarByCompleteEnumerationShow();
                    break;
                default:
                    break;
            }
        }

        //单独一个物体预览
        private void ShowGiftPackModelAvatarBySingleShow()
        {
            var itemTable = _giftPackShowItemTableList[0];

            UpdateOneItemTableInPackGift(itemTable);
        }

        //组合预览
        private void ShowGiftPackModelAvatarByCombinationShow()
        {
            InitPlayerAvatar();
            //脱掉时装
            TakeOffFashionItemInPlayerModel(_isOwnerCompleteShowType);

            for (var i = 0; i < _giftPackShowItemTableList.Count; i++)
            {
                var showItemTable = _giftPackShowItemTableList[i];
                if(showItemTable == null)
                    continue;

                UpdateModelAvatarByItemTable(showItemTable);
            }
        }

        //分开预览
        private void ShowGiftPackModelAvatarByEnumerationShow()
        {
            //根节点不存在
            if (avatarChangeRoot == null)
                return;

            CommonUtility.UpdateGameObjectVisible(avatarChangeRoot, true);

            if (_itemTipModelAvatarEnumerationController == null)
            {
                var enumerationControllerPrefab = CommonUtility.LoadGameObject(avatarChangeRoot);
                if (enumerationControllerPrefab != null)
                {
                    _itemTipModelAvatarEnumerationController = enumerationControllerPrefab
                        .GetComponent<ItemTipModelAvatarEnumerationController>();
                }

                if (_itemTipModelAvatarEnumerationController != null)
                {
                    _itemTipModelAvatarEnumerationController.InitController(
                        _giftPackShowItemTableList,
                        _giftPackSelectItemTableIndex,
                        OnItemPageChangeActionInItemEnumerationType);
                }
            }
            else
            {
                _itemTipModelAvatarEnumerationController.OnEnableController(_giftPackSelectItemTableIndex);
            }

            UpdateModelAvatarInItemEnumerationType();
        }

        //组合+宠物预览
        private void ShowGiftPackModelAvatarByMatchingShow()
        {            
            var itemTableCount = _giftPackShowItemTableList.Count;
            
            //初始化人物
            InitPlayerAvatar();
            TakeOffFashionItemInPlayerModel(_isOwnerCompleteShowType);

            for (var i = 0; i < itemTableCount - 1; i++)
            {
                var showItemTable = _giftPackShowItemTableList[i];
                if (showItemTable == null)
                    continue;

                UpdateModelAvatarByItemTable(showItemTable);
            }

            //最后一个是宠物
            var matchPetItemTable = _giftPackShowItemTableList[itemTableCount - 1];
            //加载
            ShowMatchingTypePetAvatar(matchPetItemTable);
        }

        //分开预览礼包
        private void ShowGiftPackModelAvatarByCompleteEnumerationShow()
        {
            if (avatarChangeRoot == null)
                return;
            CommonUtility.UpdateGameObjectVisible(avatarChangeRoot, true);

            if (_itemTipModelAvatarEnumerationController == null)
            {
                var enumerationControllerPrefab = CommonUtility.LoadGameObject(avatarChangeRoot);
                if (enumerationControllerPrefab != null)
                {
                    _itemTipModelAvatarEnumerationController = enumerationControllerPrefab
                        .GetComponent<ItemTipModelAvatarEnumerationController>();
                }

                if (_itemTipModelAvatarEnumerationController != null)
                {
                    _itemTipModelAvatarEnumerationController.InitController(
                        _giftPackShowItemTableList,
                        _giftPackSelectItemTableIndex,
                        OnGiftPackPageChangeActionInGiftPackEnumerationType);
                }
            }
            else
            {
                _itemTipModelAvatarEnumerationController.OnEnableController(_giftPackSelectItemTableIndex);
            }

            UpdateModelAvatarInGiftPackEnumerationType();
        }

        #endregion

        #region UpdatePlayerAvatar

        //刷新角色身上的东西
        private void UpdateModelAvatarByItemTable(ItemTable itemTable)
        {
            if (itemTable == null)
                return;

            //头像框类型，不刷新
            if (itemTable.Type == ItemTable.eType.HEAD_FRAME)
                return;

            //虚拟礼包类型，不刷新
            if (itemTable.Type == ItemTable.eType.VirtualPack)
                return;

            //宠物蛋或者礼包不刷新
            if (itemTable.SubType == ItemTable.eSubType.PetEgg
                || itemTable.SubType == ItemTable.eSubType.GiftPackage)
                return;
            
            if (itemTable.SubType == ItemTable.eSubType.TITLE)
            {
                InitPlayerTitle(itemTable);
            }
            else
            {
                RefreshPlayerAvatar(itemTable);
            }
        }

        private void RefreshPlayerAvatar(ItemTable itemTable)
        {

            if (itemTable == null)
                return;

            if (itemTable.SubType == ItemTable.eSubType.FASHION_HAIR)
            {
                PlayerBaseData.GetInstance().AvatarEquipWing(geAvatarRenderEx, itemTable.ID);
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_AURAS)
            {
                PlayerBaseData.GetInstance().AvatarEquipHalo(geAvatarRenderEx, itemTable.ID);
            }
            else if (itemTable.SubType == ItemTable.eSubType.WEAPON)
            {
                //武器可能存在强化
                //将可能存在的武器强化设置默认值(0);之后更新武器部位的模型
                PlayerBaseData.GetInstance().AvatarShowWeaponStrengthen(geAvatarRenderEx);

                PlayerBaseData.GetInstance().AvatarEquipWeapon(geAvatarRenderEx,
                    PlayerBaseData.GetInstance().JobTableID,
                    itemTable.ID);
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
            {
                PlayerBaseData.GetInstance().AvatarEquipWeapon(geAvatarRenderEx,
                    PlayerBaseData.GetInstance().JobTableID,
                    itemTable.ID);
            }
            else
            {
                EFashionWearSlotType slotType = FashionMallUtility.GetEquipSlotType(itemTable);
                //首先清空对应的位置，之后再次刷新
                PlayerBaseData.GetInstance().AvatarEquipPart(geAvatarRenderEx,
                    slotType, 0);
                PlayerBaseData.GetInstance().AvatarEquipPart(geAvatarRenderEx,
                    slotType,
                    itemTable.ID);
            }
            geAvatarRenderEx.ChangeAction("Anim_Show_Idle", 1f, true);
        }
        #endregion

        #region Helper

        //预览礼包翻页
        private void OnGiftPackPageChangeActionInGiftPackEnumerationType(int selectedIndex)
        {
            if (_giftPackSelectItemTableIndex == selectedIndex)
                return;

            _giftPackSelectItemTableIndex = selectedIndex;

            UpdateModelAvatarInGiftPackEnumerationType();
        }

        //分开预览礼包
        private void UpdateModelAvatarInGiftPackEnumerationType()
        {
            CommonUtility.UpdateGameObjectVisible(itemTitleRoot, false);
            CommonUtility.UpdateTextVisible(itemNameLabel, false);

            var showIndex = _giftPackSelectItemTableIndex - 1;
            if (showIndex < 0)
            {
                showIndex = 0;
            }
            else if (showIndex >= _giftPackShowItemTableList.Count)
            {
                showIndex = _giftPackShowItemTableList.Count - 1;
            }

            //当前要展示的礼包
            var itemTable = _giftPackShowItemTableList[showIndex];
            if (itemTable == null)
                return;

            bool isOwnerCompleteShowType = false;
            var giftPackShowItemTableList = ItemDataUtility.GetFinalGiftPackShowItemTableList(itemTable.PackageID,
                GiftPackTable.eShowAvatarModelType.CompleteEnumeration,
                out isOwnerCompleteShowType);

            //更新礼包名字
            InitPlayerName(itemTable);

            //初始化模型
            InitPlayerAvatar();
            TakeOffFashionItemInPlayerModel(isOwnerCompleteShowType);

            //更新模型
            for (var i = 0; i < giftPackShowItemTableList.Count; i++)
            {
                var showItemTable = giftPackShowItemTableList[i];
                if(showItemTable == null)
                    continue;

                UpdateModelAvatarByItemTable(showItemTable);
            }
        }

        //分页预览道具
        private void OnItemPageChangeActionInItemEnumerationType(int selectedIndex)
        {
            if (_giftPackSelectItemTableIndex == selectedIndex)
                return;

            _giftPackSelectItemTableIndex = selectedIndex;

            UpdateModelAvatarInItemEnumerationType();
        }

        //预览道具
        private void UpdateModelAvatarInItemEnumerationType()
        {
            //分类预览的时候，首先隐藏称号
            CommonUtility.UpdateGameObjectVisible(itemTitleRoot, false);

            var showIndex = _giftPackSelectItemTableIndex - 1;
            if (showIndex < 0)
                showIndex = 0;
            else if (showIndex >= _giftPackShowItemTableList.Count)
            {
                showIndex = _giftPackShowItemTableList.Count - 1;
            }

            var itemTable = _giftPackShowItemTableList[showIndex];
            UpdateOneItemTableInPackGift(itemTable);
        }

        //更新礼包中的一个道具
        private void UpdateOneItemTableInPackGift(ItemTable itemTable)
        {
            if (itemTable == null)
                return;

            if (itemTable.SubType == ItemTable.eSubType.PetEgg)
            {
                InitPetAvatar(itemTable);
            }
            else
            {
                UpdateSelfPlayerByItemTable(itemTable);
            }
        }

        //加载Match模式下的宠物
        private void ShowMatchingTypePetAvatar(ItemTable itemTable)
        {
            //根节点不存在
            if (followPetAvatarRoot == null)
                return;

            if (itemTable == null)
                return;

            if (itemTable.SubType != ItemTable.eSubType.PetEgg)
                return;

            //宠物Id
            var petId = ItemDataUtility.GetPetIdByItemTable(itemTable);

            //加载宠物模型的预制体
            CommonUtility.UpdateGameObjectVisible(followPetAvatarRoot, true);
            //加载预制体
            if (_followPetAvatarEx == null)
            {
                var followPetAvatarPrefab = CommonUtility.LoadGameObject(followPetAvatarRoot);
                if (followPetAvatarPrefab != null)
                {
                    _followPetAvatarEx = followPetAvatarPrefab.GetComponent<GeAvatarRendererEx>();
                }
            }

            //宠物AvatarEx不存在
            if (_followPetAvatarEx == null)
                return;

            //避免同时展示人物和宠物的时候，Avatar重叠，设置Match类型下宠物Avatar的Layer
            if (_itemTipModelAvatarLayerIndex <
                ItemTipManager.GetInstance().ItemTipModelAvatarMaxLayerIndex)
            {
                //_followPetAvatarEx.m_Layer = _itemTipModelAvatarLayerIndex + 1;
            }
            else
            {
                //_followPetAvatarEx.m_Layer = _itemTipModelAvatarLayerIndex - 1;
            }

            //加载宠物Avatar的内容
            PlayerUtility.LoadPetAvatarRenderEx(petId, _followPetAvatarEx, false);
        }

        //隐藏的时候，将Layer重置，重置为基础Layer
        public void ResetModelAvatarEx()
        {
            if (geAvatarRenderEx != null)
            {
                //geAvatarRenderEx.m_Layer = ItemTipManager.GetInstance().ItemTipModelAvatarBaseLayerIndex;
                geAvatarRenderEx.ClearAvatar();
            }

            if (_followPetAvatarEx != null)
            {
                //_followPetAvatarEx.m_Layer = ItemTipManager.GetInstance().ItemTipModelAvatarBaseLayerIndex;
                _followPetAvatarEx.ClearAvatar();
            }
        }

        //脱掉身上的时装（如果礼包包含整套预览)
        private void TakeOffFashionItemInPlayerModel(bool isOwnerCompleteShowType)
        {
            //不包含整套预览，直接返回
            if (isOwnerCompleteShowType == false)
                return;

            //自己的Avatar
            var selfPlayerAvatar = PlayerBaseData.GetInstance().avatar;
            if (selfPlayerAvatar == null)
                return;

            var equipmentIds = selfPlayerAvatar.equipItemIds;
            if (equipmentIds == null || equipmentIds.Length <= 0)
                return;

            for (var i = 0; i < equipmentIds.Length; i++)
            {
                var currentEquipId = (int)equipmentIds[i];
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(currentEquipId);
                if(itemTable == null)
                    continue;

                var fashionWearSlotType = FashionMallUtility.GetEquipSlotType(itemTable);
                if(fashionWearSlotType == EFashionWearSlotType.Weapon
                   || fashionWearSlotType == EFashionWearSlotType.Invalid)
                    continue;

                PlayerBaseData.GetInstance().AvatarEquipPart(geAvatarRenderEx, fashionWearSlotType, 0);

            }

        }
        #endregion

    }
}
