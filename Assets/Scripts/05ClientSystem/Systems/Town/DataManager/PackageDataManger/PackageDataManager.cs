using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;


namespace GameClient
{
    public class PackageDataManager : DataManager<PackageDataManager>
    {

        private readonly string _levelStr = "当前等级";
        private readonly string _percentStr = "%";

        public bool AlreadyUseTotalMagicBox { get; set; }
        public bool AlreadyUseTotalMagicHammer { get; set; }

        private bool _isSendingShowFashionWeapon = false;    //时装武器


        #region Initialize
        public override void Initialize()
        {
            BindUiEvents();
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindUiEvents();
            UnBindNetEvents();
        }

        private void BindUiEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, OnItemUsedSucceed);
        }

        private void UnBindUiEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, OnItemUsedSucceed);
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneSetFashionWeaponShowRes.MsgID, OnShowFashionWeaponRes);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneSetFashionWeaponShowRes.MsgID, OnShowFashionWeaponRes);
        }
        #endregion

        #region SpeicalItem
        //主要针对消耗品中的女神之泪
        public bool IsPackageTabShowRedPoint(EPackageType ePackageType)
        {
            var itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(ePackageType);

            for (var i = 0; i < itemGuids.Count; i++)
            {
                var itemGuid = itemGuids[i];
                var curItemData = ItemDataManager.GetInstance().GetItem(itemGuid);
                if (null != curItemData)
                {
                    if (IsItemShowRedPoint(curItemData) == true)
                        return true;
                }
            }
            return false;
        }
        
        public bool IsItemShowRedPoint(ItemData itemData)
        {
            if (itemData == null)
                return false;

            var isTearsOfGodItem = IsTearOfGodItem(itemData);
            //非女神之泪
            if (isTearsOfGodItem == false)
            {
                return false;
            }
            else
            {
                //女神之泪Item，判断等级
                var playerLevel = PlayerBaseData.GetInstance().Level;
                if (playerLevel >= itemData.LevelLimit && playerLevel <= itemData.MaxLevelLimit)
                {
                    return true;
                }

                return false;
            }
        }

        //成功使用女神之泪类型的item
        public void OnItemUsedSucceed(UIEvent ui)
        {
            if (ui == null)
                return;

            var itemData = ui.Param1 as ItemData;
            if(itemData == null)
                return;

            var isTearOfGodItem = IsTearOfGodItem(itemData);
            if (isTearOfGodItem == false)
                return;


            var descriptionStr = itemData.Description;
            var curLevel = PlayerBaseData.GetInstance().Level;
            var totalExpByLevel = TableManager.GetInstance().GetExpByLevel(curLevel);
            var curExp = PlayerBaseData.GetInstance().CurExp;

            //索引
            var indexStart = descriptionStr.IndexOf(_levelStr);
            var indexEnd = descriptionStr.IndexOf(_percentStr);

            if (indexStart == -1 || indexEnd == -1)
            {
                Logger.LogErrorFormat("ItemData tableID is {0} and description is not valid {1}", itemData.TableID, itemData.Description);
                return;
            }

            //截取30%格式的字符串
            var subLength = indexEnd - (indexStart + 3);
            var addPercentValue = descriptionStr.Substring(indexStart + 4, subLength);

            //字符串是否为30%的格式
            var isPercentStrFlag = IsPercentStr(addPercentValue);
            if (isPercentStrFlag == false)
                return;

            var percentValue = Convert.ToDouble(addPercentValue.TrimEnd('%')) / 100;
            var upgradeExpValue = (float)percentValue * totalExpByLevel;
            //角色升级， 则不显示经验提升界面
            if (upgradeExpValue + curExp >= totalExpByLevel)
                return;

            var expUpgradeData = new ExpUpgradeData()
            {
                CurExpValue = curExp,
                MaxExpValue = totalExpByLevel,
                AddExpValue = upgradeExpValue,
            };

            if (ClientSystemManager.GetInstance().IsFrameOpen<ExpUpgradeFrame>() == true)
            {
                ClientSystemManager.GetInstance().CloseFrame<ExpUpgradeFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<ExpUpgradeFrame>(FrameLayer.Middle, expUpgradeData);

        }

        /// <summary>
        /// 是否为女神之泪类型的Item
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        private bool IsTearOfGodItem(ItemData itemData)
        {
            //边界
            if (itemData == null)
                return false;

            //非经验丹
            if (itemData.SubType != (int) ItemTable.eSubType.ExperiencePill)
                return false;

            //女神之泪的类型：由SubType 和 ThirdType共同决定
            if (itemData.ThirdType == ItemTable.eThirdType.GoddessTear)
                return true;
            //if (itemData.TableID == 800000206 || itemData.TableID == 800000207
            //                                  || itemData.TableID == 800000208 || itemData.TableID == 800000209
            //                                  || itemData.TableID == 800000210 || itemData.TableID == 800000211
            //                                  || itemData.TableID == 800000212 || itemData.TableID == 800000213
            //                                  || itemData.TableID == 800000214
            //                                  || itemData.TableID == 800000615 || itemData.TableID == 800000616
            //                                  || itemData.TableID == 800000617 || itemData.TableID == 800000618
            //                                  || itemData.TableID == 800000619)
            //{
            //    return true;
            //}

            return false;
        }
        #endregion

        #region MagicBox
        public void UsingMagicBoxAndMagicHammer()
        {
            AlreadyUseTotalMagicBox = true;
            AlreadyUseTotalMagicHammer = true;
        }

        public void ResetMagicBoxAndMagicHammer()
        {
            if (AlreadyUseTotalMagicBox == true)
                AlreadyUseTotalMagicBox = false;
            if (AlreadyUseTotalMagicHammer == true)
                AlreadyUseTotalMagicHammer = false;
        }
        #endregion

        #region Helper
        /// <summary>
        /// 判断字符串是否为30%的类型
        /// </summary>
        /// <param name="percentStr"></param>
        /// <returns></returns>
        private bool IsPercentStr(string percentStr)
        {
            if (percentStr == null)
                return false;

            var strLength = percentStr.Length;
            if (strLength <= 1)
                return false;

            if (percentStr[strLength - 1] != '%')
                return false;

            for (var i = 0; i < strLength - 1; i++)
            {
                if (percentStr[i] < '0' || percentStr[i] > '9')
                    return false;
            }

            return true;
        }

        #endregion


        #region FashionWeapon
        public void ResetSendFashionWeaponReqFlag()
        {
            _isSendingShowFashionWeapon = false;
        }

        public void SendShowFashionWeaponReq(bool setFashionWeaponFlag)
        {
            if(_isSendingShowFashionWeapon == true)
                return;

            _isSendingShowFashionWeapon = true;

            SceneSetFashionWeaponShowReq req = new SceneSetFashionWeaponShowReq();
            if (setFashionWeaponFlag == true)
                req.isShow = 1;
            else
                req.isShow = 0;

            if (NetManager.Instance() != null)
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

        }

        private void OnShowFashionWeaponRes(MsgDATA data)
        {
            SceneSetFashionWeaponShowRes sceneSetFashionWeaopnShowRes = new SceneSetFashionWeaponShowRes();
            sceneSetFashionWeaopnShowRes.decode(data.bytes);

            _isSendingShowFashionWeapon = false;

            if (sceneSetFashionWeaopnShowRes.ret != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneSetFashionWeaopnShowRes.ret);
                return;
            }
            return;
        }

        //是否包含武器时装
        public bool IsEquipedFashionWeapon(UInt32[] equipedItemIds)
        {

            if (equipedItemIds == null || equipedItemIds.Length <= 0)
                return false;

            for (var i = 0; i < equipedItemIds.Length; i++)
            {
                var equipedItemId = (int) equipedItemIds[i];
                if (equipedItemId <= 0)
                {
                    continue;
                }

                var equipedItemData = TableManager.GetInstance().GetTableItem<ItemTable>(equipedItemId);
                if (equipedItemData != null && equipedItemData.SubType == ItemTable.eSubType.FASHION_WEAPON)
                {
                    return true;
                }
            }

            return false;
        }

        //是否需要刷新角色的Avatar
        public bool IsPlayerAvatarNeedChanged(PlayerAvatar curAvatar, PlayerAvatar changeAvatar)
        {
            //当前Avatar为null
            if (curAvatar == null)
            {
                if (changeAvatar == null)
                    return false;
                else
                    return true;
            }

            if (changeAvatar == null)
                return true;

            //武器力量
            if (curAvatar.weaponStrengthen != changeAvatar.weaponStrengthen)
                return true;

            ////称号
            //if (curAvatar.titleId != changeAvatar.titleId)
            //    return true;

            //当前装备为null
            if (curAvatar.equipItemIds == null)
            {
                if (changeAvatar.equipItemIds == null)
                    return false;
                else
                    return true;
            }

            if (changeAvatar.equipItemIds == null)
                return true;

            var curAvatarItemIdNumber = curAvatar.equipItemIds.Length;
            var changeAvatarItemIdNumber = changeAvatar.equipItemIds.Length;
            //装备数量不同
            if (curAvatarItemIdNumber != changeAvatarItemIdNumber)
                return true;

            //在装备数量相同的情况下进行比较装备ID
            var curAvatarItemIdList = new List<UInt32>();
            for (var i = 0; i < curAvatarItemIdNumber; i++)
            {
                curAvatarItemIdList.Add(curAvatar.equipItemIds[i]);
            }
            curAvatarItemIdList.Sort((x, y) => x.CompareTo(y));

            var changeAvatarItemIdList = new List<UInt32>();
            for (var i = 0; i < changeAvatarItemIdNumber; i++)
            {
                changeAvatarItemIdList.Add(changeAvatar.equipItemIds[i]);
            }
            changeAvatarItemIdList.Sort((x, y) => x.CompareTo(y));

            for (var i = 0; i < curAvatarItemIdNumber && i < changeAvatarItemIdNumber; i++)
            {
                if (curAvatarItemIdList[i] != changeAvatarItemIdList[i])
                    return true;
            }

            if (curAvatar.isShoWeapon != changeAvatar.isShoWeapon)
            {
                //1 显示时装
                if (changeAvatar.isShoWeapon == 1)
                {
                    //存在时装ID， 并且需要显示时装
                    if (PackageDataManager.GetInstance().IsEquipedFashionWeapon(changeAvatar.equipItemIds) == true)
                        return true;
                }
                else if (changeAvatar.isShoWeapon == 0)
                {
                    //0 存在时装ID， 并且不显示时装
                    if (PackageDataManager.GetInstance().IsEquipedFashionWeapon(changeAvatar.equipItemIds) == true)
                        return true;
                }
            }


            return false;
        }

        public EFashionWearSlotType GetFashionWearSlotTypeByItemFashionWearNewSlotType(
            EFashionWearNewSlotType fashionWearNewSlotType)
        {
            switch (fashionWearNewSlotType)
            {
                case EFashionWearNewSlotType.Chest:
                    return EFashionWearSlotType.Chest;
                case EFashionWearNewSlotType.Head:
                    return EFashionWearSlotType.Head;
                case EFashionWearNewSlotType.UpperBody:
                    return EFashionWearSlotType.UpperBody;
                case EFashionWearNewSlotType.LowerBody:
                    return EFashionWearSlotType.LowerBody;
                case EFashionWearNewSlotType.Waist:
                    return EFashionWearSlotType.Waist;
                case EFashionWearNewSlotType.Weapon:
                    return EFashionWearSlotType.Weapon;
                case EFashionWearNewSlotType.Halo:
                    return EFashionWearSlotType.Halo;
                case EFashionWearNewSlotType.Auras:
                    return EFashionWearSlotType.Auras;
            }

            return EFashionWearSlotType.Invalid;
        }
        public EFashionWearNewSlotType GetFashionWearNewSlotTypeByItemFashionWearSlotType(
            EFashionWearSlotType fashionWearSlotType)
        {
            switch (fashionWearSlotType)
            {
                case EFashionWearSlotType.Chest:
                    return EFashionWearNewSlotType.Chest;
                case EFashionWearSlotType.Head:
                    return EFashionWearNewSlotType.Head;
                case EFashionWearSlotType.UpperBody:
                    return EFashionWearNewSlotType.UpperBody;
                case EFashionWearSlotType.LowerBody:
                    return EFashionWearNewSlotType.LowerBody;
                case EFashionWearSlotType.Waist:
                    return EFashionWearNewSlotType.Waist;
                case EFashionWearSlotType.Weapon:
                    return EFashionWearNewSlotType.Weapon;
                case EFashionWearSlotType.Halo:
                    return EFashionWearNewSlotType.Halo;
                case EFashionWearSlotType.Auras:
                    return EFashionWearNewSlotType.Auras;
            }

            return EFashionWearNewSlotType.Invalid;
        }



        #endregion
    }
}
