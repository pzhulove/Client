using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
namespace GameClient
{
    public class SwitchWeaponDataManager : DataManager<SwitchWeaponDataManager>
    {
        private Dictionary<byte, ulong> sideWeaponDic = new Dictionary<byte, ulong>();
        public override void Initialize()
        {
            for (byte i = 0; i < 1; i++)
            {
                sideWeaponDic[i] = 0;

            }
        }

        public override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }

        public override void Clear()
        {
            sideWeaponDic.Clear();
        }

        public void TakeOnSideWeapon(uint index, ulong id)
        {
            SceneSetWeaponBarReq msg = new SceneSetWeaponBarReq();
            msg.index = (byte)(index-1);
            msg.weaponId = id;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            WaitNetMessageManager.GetInstance().Wait<SceneSetWeaponBarRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
            });
        }


        public void UpdateSideWeapon(List<ulong> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                sideWeaponDic[(byte)i] = list[i];
            }

            
        }

        public bool IsInSidePack(ItemData data)
        {
            if (data.GUID == 0) return false;

                foreach (var item in sideWeaponDic)
                {
                    if (data.GUID == item.Value)
                        return true;
                }
            
            return false;
        }

        public ulong GetPosWeapon(uint index)
        {
            ulong id = 0;
            sideWeaponDic.TryGetValue((byte)index, out id);
            return id;
        }

        public bool MainAndSideBothHaveWeapon()
        {
            bool flag = false;
            foreach (var item in sideWeaponDic)
            {
                if (item.Value != 0)
                {
                    flag = true;
                    break;
                }
            }
            ulong id = ItemDataManager.GetInstance().GetMainWeapon();
            return flag && id > 0;
        }

        public Dictionary<byte, ulong> GetSideWeaponDic()
        {
            return sideWeaponDic;
        }

        public List<ulong> GetSideWeaponIDList()
        {
            List<ulong> idList = sideWeaponDic.Values.ToList();
            return idList;
        }


    }
}
