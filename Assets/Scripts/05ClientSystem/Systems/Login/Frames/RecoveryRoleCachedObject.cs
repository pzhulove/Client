using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    class RecoveryRoleCachedObject : CachedNormalObject<RoleData>
    {
        ComRecoveryRole comRoleConfig;
        public static int ms_max_roles_count = 8;
        public static string formatTimeString = "{0:D2}天{1:D2}时{2:D2}分{3:D2}秒";
        public static uint ms_max_life_time
        {
            get
            {
                return 7 * 24 * 3600;
            }
        }

        public static bool OnFilterAlive(RoleInfo roleInfo)
        {
            if(roleInfo == null)
            {
                return false;
            }

            if (roleInfo.deleteTime != 0)
            {
                return false;
            }

            return true;
        }

        public static int HasOwnedRoles
        {
            get
            {
                int iHasCount = 0;
                if (ClientApplication.playerinfo != null)
                {
                    var roles = ClientApplication.playerinfo.roleinfo;
                    for (int i = 0; i < roles.Length; ++i)
                    {
                        if (OnFilterAlive(roles[i]))
                        {
                            ++iHasCount;
                        }
                    }
                }
                return iHasCount;
            }
        }

        /// <summary>
        /// 可用角色栏位
        /// </summary>
        public static int EnabledRoleField
        {
            get 
            {
                int fieldCount = 0;
                if (ClientApplication.playerinfo != null)
                {
                    if (ms_max_roles_count != (int)ClientApplication.playerinfo.baseRoleFieldNum)
                    {
                        //Logger.LogErrorFormat("[RecoveryRoleCachedObject] StandardRoleField netData {0} is not equal to localData {1}", (int)ClientApplication.playerinfo.baseRoleFieldNum, ms_max_roles_count);
                        fieldCount = ms_max_roles_count + (int)ClientApplication.playerinfo.unLockedExtendRoleFieldNum;
                    }
                    else
                    {
                        fieldCount = (int)(ClientApplication.playerinfo.baseRoleFieldNum + ClientApplication.playerinfo.unLockedExtendRoleFieldNum);
                    }
                }
                return fieldCount;
            }
        }

        public static bool OnFilter(RoleInfo roleInfo)
        {
            if (roleInfo != null)
            {
                var serverTime = TimeManager.GetInstance().GetServerTime();
                if (roleInfo.deleteTime == 0 ||
                    roleInfo.deleteTime > serverTime ||
                    serverTime - roleInfo.deleteTime > RecoveryRoleCachedObject.ms_max_life_time)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public override void Initialize()
        {
            comRoleConfig = goLocal.GetComponent<ComRecoveryRole>();
            comRoleConfig.button.onClick.AddListener(OnClickRecoveryRole);
            comRoleConfig.onTick += OnTick;
        }

        public override void UnInitialize()
        {
            comRoleConfig.button.onClick.RemoveListener(OnClickRecoveryRole);
            comRoleConfig.onTick -= OnTick;
            comRoleConfig = null;
        }

        //public Sprite _LoadJobHeadIcon()
        //{
        //    var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(Value.roleInfo.occupation);
        //    if(jobItem == null)
        //    {
        //        return null;
        //    }

        //    ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
        //    if (resData == null)
        //    {
        //        return null;
        //    }

        //    return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
        //}
        public void _LoadJobHeadIcon(ref Image image)
        {
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(Value.roleInfo.occupation);
            if (jobItem == null)
            {
                return;
            }

            ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
            if (resData == null)
            {
                return;
            }

            // return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref image, resData.IconPath);
        }

        public override void OnUpdate()
        {
            // comRoleConfig.headIcon.sprite = _LoadJobHeadIcon();
            _LoadJobHeadIcon(ref comRoleConfig.headIcon);
            comRoleConfig.LvInfo.text = "Lv." + Value.roleInfo.level;
            comRoleConfig.RoleName.text = Value.roleInfo.name;
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(Value.roleInfo.occupation);
            if(jobItem != null)
            {
                comRoleConfig.JobName.text = jobItem.Name;
            }
        }

        void OnClickRecoveryRole()
        {
            int iHasRoles = HasOwnedRoles;
            //if (iHasRoles >= ms_max_roles_count)
            if (iHasRoles >= EnabledRoleField)
            {
                SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("recovery_failed_roles_full"),iHasRoles));
                return;
            }

            GateRecoverRoleReq kSend = new GateRecoverRoleReq();
            kSend.roleId = Value.roleInfo.roleId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);

            WaitNetMessageManager.GetInstance().Wait<GateRecoverRoleRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    if(msgRet.result == (int)Protocol.ProtoErrorCode.ENTERGAME_RECOVER_ROLE_LIMIT)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(msgRet.roleUpdateLimit);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                }
                else
                {
                    var roles = ClientApplication.playerinfo.roleinfo;
                    for (int i = 0; i < roles.Length; ++i)
                    {
                        if (roles[i].roleId == msgRet.roleId)
                        {
                            roles[i].deleteTime = 0;
                            break;
                        }
                    }
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleRecoveryUpdate, msgRet.roleId);

                   // Logger.LogErrorFormat("roleid = {0} recovery succeed!", msgRet.roleId);
                }
            }, true, 15.0f);
        }

        void OnTick()
        {
            if(goLocal.activeSelf)
            {
                var servertime = TimeManager.GetInstance().GetServerTime();
                uint time = servertime >= Value.roleInfo.deleteTime ? (servertime - Value.roleInfo.deleteTime) : 0;
                if(time < ms_max_life_time)
                {
                    time = ms_max_life_time - time;
                }
                else
                {
                    time = 0;
                }
                uint iDays = time / 86400;
                uint iHours = (time / 3600) % 24;
                uint iMinutes = (time / 60) % 60;
                uint iSeconds = time % 60;
                comRoleConfig.LifeTime.text = string.Format(formatTimeString, iDays, iHours, iMinutes, iSeconds);
                comRoleConfig.grayRecovery.enabled = time == 0;
            }
        }
    }
}