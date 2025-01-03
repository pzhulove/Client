using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
using Network;
using Protocol;
using ProtoTable;
namespace GameClient
{
    public class TitleDataManager : DataManager<TitleDataManager>
    {
        /// <summary>
        /// 头衔的类型 文字或者的图片
        /// </summary>
        public enum eTitleStyle
        {
            Txt=1,//文字
            Img=2,//图片
            Group=3,//文字图片组合
        }
        List<PlayerTitleInfo> allTitle = new List<PlayerTitleInfo>();//所有头衔
        public override void Initialize()
        {
            allTitle.Clear();
            _RegisterNetHandler();
        }

        public override void Clear()
        {
            allTitle.Clear();
            _UnRegisterNetHandler();
            PlayerBaseData.GetInstance().TitleGuid = 0;
            PlayerBaseData.GetInstance().WearedTitleInfo = null;
        }

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldGetPlayerTitleSyncList.MsgID, _OnWorldGetPlayerTitleSyncList);//同步头衔
            NetProcess.AddMsgHandler(WorldNewTitleSyncUpdate.MsgID, _OnWorldNewTitleSyncUpdate);//同步新增或者删除头衔
            NetProcess.AddMsgHandler(WorldNewTitleTakeUpRes.MsgID, _OnWorldNewTitleTakeUpRes);//头衔穿戴返回
            NetProcess.AddMsgHandler(WorldNewTitleTakeOffRes.MsgID, _OnWorldNewTitleTakeOffRes);//头衔脱掉返回
            NetProcess.AddMsgHandler(WorldNewTitleUpdateData.MsgID, _OnWorldNewTitleUpdateData);//头衔更新数据
        }

        void _UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldGetPlayerTitleSyncList.MsgID, _OnWorldGetPlayerTitleSyncList);//同步头衔
            NetProcess.RemoveMsgHandler(WorldNewTitleSyncUpdate.MsgID, _OnWorldNewTitleSyncUpdate);//同步新增或者删除头衔
            NetProcess.RemoveMsgHandler(WorldNewTitleTakeUpRes.MsgID, _OnWorldNewTitleTakeUpRes);//头衔穿戴返回
            NetProcess.RemoveMsgHandler(WorldNewTitleTakeOffRes.MsgID, _OnWorldNewTitleTakeOffRes);//头衔脱掉返回
            NetProcess.RemoveMsgHandler(WorldNewTitleUpdateData.MsgID, _OnWorldNewTitleUpdateData);//头衔更新数据
        }

        /// <summary>
        /// 通过subtype拿到所有这个类型的数据，传入0的时候拿所有的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<PlayerTitleInfo> getTitleListForSubType(int type)
        {
            List<PlayerTitleInfo> tempTitleList = new List<PlayerTitleInfo>();
            if(type == 0)
            {
                return allTitle;
            }
            else
            {
                for(int i = 0;i<allTitle.Count;i++)
                {
                    var newTitleTableItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)allTitle[i].titleId);
                    if(newTitleTableItem != null)
                    {
                        if((int)newTitleTableItem.SubType == type)
                        {
                            tempTitleList.Add(allTitle[i]);
                        }
                    }
                }
                return tempTitleList;
            }
        }

        ///// <summary>
        /////拉取称谓数据请求
        ///// </summary>
        //public void SendGetPlayerTitleListReq()
        //{
        //    GetPlayerTitleListReq req = new GetPlayerTitleListReq();
        //    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        //}

        /// <summary>
        /// 发送头衔穿戴请求
        /// </summary>
        /// <param name="guid"></param>头衔id
        public void SendNewTitleTakeUpReq(ulong guid,uint titleId)
        {
            WorldNewTitleTakeUpReq req = new WorldNewTitleTakeUpReq();
            req.titleGuid = (ulong)guid;
            req.titleId = titleId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 脱掉头衔的请求
        /// </summary>
        /// <param name="guid"></param>
        public void SendNewTitleTakeOffReq(ulong guid,uint titleId)
        {
            WorldNewTitleTakeOffReq req = new WorldNewTitleTakeOffReq();
            req.titleGuid = (ulong)guid;
            req.titleId = titleId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 是否有头衔
        /// </summary>
        /// <returns></returns>
        public bool IsHaveTitle()
        {
            
            Protocol.PlayerWearedTitleInfo playerWearedTitleInfo= PlayerBaseData.GetInstance().WearedTitleInfo;
            if(playerWearedTitleInfo==null)
            {
                return false;
            }
            if(playerWearedTitleInfo.titleId==0)
            {
                return false;
            }
            return true;
        }
        #region 接受协议
        /// <summary>
        /// 拉取头衔数据返回
        /// </summary>
        /// <param name="msgData"></param>
        void _OnWorldGetPlayerTitleSyncList(MsgDATA msgData)
        {
            WorldGetPlayerTitleSyncList res = new WorldGetPlayerTitleSyncList();
            if (res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            //if (res.errorCode != (uint)ProtoErrorCode.SUCCESS)
            //{
            //    SystemNotifyManager.SystemNotify((int)res.errorCode);
            //    return;
            //}
            allTitle.Clear();
            for(int i = 0;i<res.titles.Length;i++)
            {
                allTitle.Add(res.titles[i]);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleDataUpdate);
        }

        void _OnWorldNewTitleSyncUpdate(MsgDATA msgData)
        {
            WorldNewTitleSyncUpdate res = new WorldNewTitleSyncUpdate();
            res.decode(msgData.bytes);
            for (int i = 0;i<res.adds.Length; i++)
            {
                bool haveTitle = false;
                for(int j = 0;j<allTitle.Count;j++)
                {
                    if(allTitle[j].guid == res.adds[i].guid)
                    {
                        haveTitle = true;
                        break;
                    }
                }
                if(!haveTitle)
                {
                    allTitle.Add(res.adds[i]);
                }
            }
            for(int i = 0;i<res.dels.Length;i++)
            {
                for(int j = 0;j<allTitle.Count;j++)
                {
                    if(allTitle[j].guid == res.dels[i])
                    {
                        allTitle.RemoveAt(j);
                        break;
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleDataUpdate);
        }

        void _OnWorldNewTitleTakeUpRes(MsgDATA msgData)
        {
            WorldNewTitleTakeUpRes res = new WorldNewTitleTakeUpRes();
            if (res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            if (res.res != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.res);
                return;
            }
        }

        void _OnWorldNewTitleTakeOffRes(MsgDATA msgData)
        {
            WorldNewTitleTakeOffRes res = new WorldNewTitleTakeOffRes();
            if (res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            if (res.res != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.res);
                return;
            }
        }

        void _OnWorldNewTitleUpdateData(MsgDATA msgData)
        {
            WorldNewTitleUpdateData res = new WorldNewTitleUpdateData();
            if(res == null)
            {
                return;
            }
            res.decode(msgData.bytes);
            for(int i = 0;i<res.updates.Length;i++)
            {
                for(int j = 0;j<allTitle.Count;j++)
                {
                    if(res.updates[i].guid == allTitle[j].guid)
                    {
                        allTitle[j] = res.updates[i];
                        break;
                    }
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleDataUpdate);
            }
        }
        #endregion
    }
}