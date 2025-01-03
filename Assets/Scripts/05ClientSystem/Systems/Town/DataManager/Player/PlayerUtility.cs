using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using Object = UnityEngine.Object;

namespace GameClient
{
    public static class PlayerUtility
    {

        //加载宠物的模型
        public static void LoadPetAvatarRenderEx(int petId,
            GeAvatarRendererEx geAvatarRenderEx,
            bool isShowFootSite = true)
        {
            if (geAvatarRenderEx == null)
            {
                Logger.LogError("geAvatarRenderEx is null");
                return;
            }

            var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petId);
            if (petTable == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", petId);
                return;
            }

            var resTable = TableManager.GetInstance().GetTableItem<ResTable>(petTable.ModeID);
            if (resTable == null)
            {
                Logger.LogErrorFormat("can not find ResTable with id:{0}", petTable.ModeID);
                return;
            }

            geAvatarRenderEx.ClearAvatar();
            geAvatarRenderEx.LoadAvatar(resTable.ModelPath);

            var backUp = geAvatarRenderEx.avatarPos;
            geAvatarRenderEx.ChangeAction("Anim_Idle01", 1f, true);

            //是否显示宠物下面的地盘，默认显示
            if (isShowFootSite == true)
            {
                geAvatarRenderEx.CreateEffect("Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                    999999, backUp);
            }

            if (petTable.UseNewFunction)
            {
                GeEffectEx effect = geAvatarRenderEx.CreateEffect(petTable.PackageEffectPath, 999999, Vector3.zero,
                    1.0f, 1.0f, false, false, true);
                if (effect != null)
                {
                    var pos = backUp;
                    pos.x += petTable.PackageEffectOffset[0] / 1000.0f;
                    pos.y += petTable.PackageEffectOffset[1] / 1000.0f;
                    pos.z += petTable.PackageEffectOffset[2] / 1000.0f;
                    effect.SetLocalPosition(pos);
                }
            }
        }

        //加载人物模型：职业Id，PlayerAvatar
        public static void LoadPlayerAvatarByPlayerAvatar(
            GeAvatarRendererEx geAvatarRenderEx,
            int professionId,
            PlayerAvatar playerAvatar)
        {
            //geAvatarRenderEx
            if (geAvatarRenderEx == null)
            {
                Logger.LogErrorFormat("HeroAvatarRenderer is null");
                return;
            }

            //职业
            var jobItem = TableManager.GetInstance().GetTableItem<JobTable>(professionId);
            if (jobItem == null)
            {
                Logger.LogErrorFormat("Cannot find JobItem and JobId is {0}", professionId);
                return;
            }

            //资源
            var resItem = TableManager.GetInstance().GetTableItem<ResTable>(jobItem.Mode);
            if (resItem == null)
            {
                Logger.LogErrorFormat("Cannot find ResItem with id : {0} ", jobItem.Mode);
                return;
            }

            geAvatarRenderEx.ClearAvatar();
            //穿戴模型
            geAvatarRenderEx.LoadAvatar(resItem.ModelPath);

            //取默认值
            uint[] playerEquipIds = new uint[0];
            int weaponStrengthen = 0;
            byte isShowWeapon = 0;
            if (playerAvatar != null)
            {
                playerEquipIds = playerAvatar.equipItemIds;
                weaponStrengthen = (int) playerAvatar.weaponStrengthen;
                isShowWeapon = playerAvatar.isShoWeapon;
            }

            //穿戴装备
            PlayerBaseData.GetInstance().AvatarEquipFromItems(geAvatarRenderEx,
                playerEquipIds,
                professionId,
                weaponStrengthen,
                null,
                false,
                isShowWeapon);

            geAvatarRenderEx.AttachAvatar("Aureole",
                "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                "[actor]Orign",
                false);

            geAvatarRenderEx.SuitAvatar();
            geAvatarRenderEx.ChangeAction("Anim_Show_Idle",
                1f,
                true);
        }

        //只加载人物模型
        public static void LoadPlayerAvatarByProfessionId(GeAvatarRendererEx geAvatarRenderEx,
            int professionId)
        {
            //geAvatarRenderEx
            if (geAvatarRenderEx == null)
            {
                Logger.LogErrorFormat("HeroAvatarRenderer is null");
                return;
            }

            //职业
            var jobItem = TableManager.GetInstance().GetTableItem<JobTable>(professionId);
            if (jobItem == null)
            {
                Logger.LogErrorFormat("Cannot find JobItem and JobId is {0}", professionId);
                return;
            }

            //资源
            var resItem = TableManager.GetInstance().GetTableItem<ResTable>(jobItem.Mode);
            if (resItem == null)
            {
                Logger.LogErrorFormat("Cannot find ResItem with id : {0} ", jobItem.Mode);
                return;
            }

            geAvatarRenderEx.ClearAvatar();
            //穿戴模型
            geAvatarRenderEx.LoadAvatar(resItem.ModelPath);

            geAvatarRenderEx.AttachAvatar("Aureole",
                "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                "[actor]Orign",
                false);

            geAvatarRenderEx.SuitAvatar();
            geAvatarRenderEx.ChangeAction("Anim_Show_Idle",
                1f,
                true);
        }

        //加载自身的模型
        public static void LoadPlayerAvatarBySelfPlayer(GeAvatarRendererEx geAvatarRenderEx)
        {
            if (geAvatarRenderEx == null)
                return;

            //自己的职业
            var jobId = PlayerBaseData.GetInstance().JobTableID;

            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobId);
            if (jobTable == null)
                return;

            var resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
            if (resTable == null)
                return;

            geAvatarRenderEx.ClearAvatar();
            geAvatarRenderEx.LoadAvatar(resTable.ModelPath);
            //自己的模型数据
            PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(geAvatarRenderEx);
            geAvatarRenderEx.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
            geAvatarRenderEx.SuitAvatar();
        }

        //得到职业头像的路径
        public static string GetPlayerProfessionHeadIconPath(int professionId)
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(professionId);
            if (jobTable == null)
                return null;

            var resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
            if (resTable == null)
                return null;

            return resTable.IconPath;
        }

        //得到职业名字
        public static string GetPlayerProfessionName(int professionId)
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(professionId);
            if (jobTable == null)
                return null;

            return jobTable.Name;
        }

        //得到道具的适配职业，如果适配本职业，则应该保存当前角色的小职业;非本职业则保留其他职业的基础职业
        public static List<int> GetItemTableSuitMixProfessionIdList(ItemTable itemTable)
        {
            if (itemTable == null)
                return null;

            if (itemTable.Occu == null)
                return null;

            var suitProfessionIdList = itemTable.Occu.ToList();
            if (suitProfessionIdList == null || suitProfessionIdList.Count <= 0)
                return null;

            //当前角色的职业Id和基础的职业Id
            var currentPlayerProfessionId = PlayerBaseData.GetInstance().JobTableID;
            var basePlayerProfessionId = CommonUtility.GetSelfBaseJobId();

            List<int> finalMixProfessionIdList = new List<int>();
            finalMixProfessionIdList.Add(currentPlayerProfessionId);

            for (var i = 0; i < suitProfessionIdList.Count; i++)
            {
                var suitProfessionId = suitProfessionIdList[i];
                var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(suitProfessionId);
                if (jobTable == null)
                    continue;

                var baseProfessionId = suitProfessionId;
                //小职业，获得基础职业
                if (jobTable.JobType == 1)
                    baseProfessionId = jobTable.prejob;

                if (baseProfessionId <= 0)
                    continue;

                //已经添加，不再添加
                if(baseProfessionId == currentPlayerProfessionId 
                   || baseProfessionId == basePlayerProfessionId)
                    continue;

                //添加
                if (finalMixProfessionIdList.Contains(baseProfessionId) == false)
                    finalMixProfessionIdList.Add(baseProfessionId);
            }

            return finalMixProfessionIdList;
        }


        //得到道具的适配的基础职业
        public static List<int> GetItemTableSuitBaseProfessionIdList(ItemTable itemTable)
        {
            if (itemTable == null)
                return null;

            if (itemTable.Occu == null)
                return null;

            var suitProfessionIdList = itemTable.Occu.ToList();
            if (suitProfessionIdList == null || suitProfessionIdList.Count <= 0)
                return null;

            List<int> finalBaseProfessionIdList = new List<int>();
            for (var i = 0; i < suitProfessionIdList.Count; i++)
            {
                var suitProfessionId = suitProfessionIdList[i];
                var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(suitProfessionId);
                if(jobTable == null)
                    continue;

                var baseProfessionId = suitProfessionId;
                //小职业，获得基础职业
                if (jobTable.JobType == 1)
                    baseProfessionId = jobTable.prejob;

                if(baseProfessionId <= 0)
                    continue;

                //添加
                if (finalBaseProfessionIdList.Contains(baseProfessionId) == false)
                    finalBaseProfessionIdList.Add(baseProfessionId);
            }

            return finalBaseProfessionIdList;
        }

        //得到账号下，各个角色的最大等级
        public static int GetMaxLevelInAccount()
        {
            var maxLevel = PlayerBaseData.GetInstance().Level;

            var roleInfo = ClientApplication.playerinfo.roleinfo;
            if (roleInfo != null)
            {
                for (var i = 0; i < roleInfo.Length; i++)
                {
                    var currentRoleInfo = roleInfo[i];
                    if(currentRoleInfo == null)
                        continue;

                    if (currentRoleInfo.level > maxLevel)
                        maxLevel = currentRoleInfo.level;
                }
            }
             return maxLevel;
        }

        //账号下角色最高等级对应的经验数值表
        public static ExpTable GetExpTableByMaxLevelInAccount()
        {
            var maxLevel = GetMaxLevelInAccount();
            var expTable = TableManager.GetInstance().GetTableItem<ExpTable>(maxLevel);
            return expTable;
        }

        //得到Vip等级对应的数值
        public static int GetVipPrivilegeValue(VipPrivilegeTable.eType type)
        {
            var vipPrivilegeValueTable = TableManager.GetInstance().GetTableItem<VipPrivilegeTable>((int) type);
            if (vipPrivilegeValueTable == null)
                return 0;

            var vipLevel = PlayerBaseData.GetInstance().VipLevel;
            if (vipLevel == 0)
            {
                return vipPrivilegeValueTable.VIP0;
            }
            else if(vipLevel == 1)
            {
                return vipPrivilegeValueTable.VIP1;
            }
            else if (vipLevel == 2)
            {
                return vipPrivilegeValueTable.VIP2;
            }
            else if (vipLevel == 3)
            {
                return vipPrivilegeValueTable.VIP3;
            }
            else if (vipLevel == 4)
            {
                return vipPrivilegeValueTable.VIP4;
            }
            else if (vipLevel == 5)
            {
                return vipPrivilegeValueTable.VIP5;
            }
            else if (vipLevel == 6)
            {
                return vipPrivilegeValueTable.VIP6;
            }
            else if (vipLevel == 7)
            {
                return vipPrivilegeValueTable.VIP7;
            }
            else if (vipLevel == 8)
            {
                return vipPrivilegeValueTable.VIP8;
            }
            else if (vipLevel == 9)
            {
                return vipPrivilegeValueTable.VIP9;
            }
            else if (vipLevel == 10)
            {
                return vipPrivilegeValueTable.VIP10;
            }
            else if (vipLevel == 11)
            {
                return vipPrivilegeValueTable.VIP11;
            }

            return vipPrivilegeValueTable.VIP0;
        }

    }
}
