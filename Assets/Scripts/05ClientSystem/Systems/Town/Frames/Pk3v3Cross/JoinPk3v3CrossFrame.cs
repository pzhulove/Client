using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class JoinPK3v3CrossFrame : ClientFrame
    {     

        [UIControl("Join")]
        Button m_btnJoin;

        [UIControl("Close")]
        Button m_btnClose;
      
        const int ITEM_NUM = 4;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/JoinPk3v3Cross";
        }
        
        void ExitTeamAndEnterPk3v3Cross()
        {
            TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(true);
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                new SceneParams
                {
                    currSceneID = systemTown.CurrentSceneID,
                    currDoorID = 0,
                    targetSceneID = 5007,
                    targetDoorID = 0,
                }));
        }

        protected override void _OnOpenFrame()
        {
           if(m_btnJoin != null)
            {
                m_btnJoin.onClick.RemoveAllListeners();            

                m_btnJoin.onClick.AddListener(() => 
                {
                    if (frameMgr.IsFrameOpen<PkSeekWaiting>())
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                        return;
                    }

                    //ScoreWarStatus status = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                    //if (status != ScoreWarStatus.SWS_PREPARE && status != ScoreWarStatus.SWS_BATTLE)
                    //{
                    //    SystemNotifyManager.SysNotifyFloatingEffect("活动未开始或者已结束，无法进入活动场景");
                    //    return;
                    //}
                    //
                    if (TeamDataManager.GetInstance().HasTeam())
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel("进入积分赛场景会退出当前所在队伍，是否确认进入？", () => 
                        {
                            ExitTeamAndEnterPk3v3Cross();
                            frameMgr.CloseFrame(this);
                        });
                    }
                    else
                    {
                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                        if (systemTown == null)
                        {
                            return;
                        }

                        CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                        if (TownTableData == null)
                        {
                            return;
                        }

                        ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();

                        if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
                        {
                            ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                            Townframe.SetForbidFadeIn(true);
                        }

                        GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                            new SceneParams
                            {
                                currSceneID = systemTown.CurrentSceneID,
                                currDoorID = 0,
                                targetSceneID = 5007,
                                targetDoorID = 0,
                            }));

                        frameMgr.CloseFrame(this);
                    }                   
                });
            }

            if (m_btnClose != null)
            {
                m_btnClose.onClick.RemoveAllListeners();
                m_btnClose.onClick.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                });              
            }


            int iTableID = 0;
            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWarRewardTable>();
                if (dicts == null)
                {
                    Logger.LogErrorFormat("TableManager.instance.GetTable<ScoreWarRewardTable>() error!!!");
                    return;
                }
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    ScoreWarRewardTable adt = iter.Current.Value as ScoreWarRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    
                    if(adt.RewardPreview.Count > 1)
                    {
                        iTableID = adt.ID;
                        break;
                    }
                }
            }

            ScoreWarRewardTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWarRewardTable>(iTableID);
            if (tableItem != null)
            {
                for (int i = 0; i < tableItem.RewardPreview.Count; i++)
                {
                    string strReward = tableItem.RewardPreviewArray(i);
                    string[] reward = strReward.Split('_');
                    if (reward.Length >= 2)
                    {
                        int id = int.Parse(reward[0]);
                        int iCount = int.Parse(reward[1]);
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
                        if (itemData != null)
                        {
                            itemData.Count = iCount;
                            if (i < ITEM_NUM)
                            {
                                ComItem item = mBind.GetCom<ComItem>(string.Format("Item{0}", i));
                                if (item != null)
                                {
                                    item.Setup(itemData, (var1, var2) =>
                                    {
                                        ItemTipManager.GetInstance().CloseAll();
                                        ItemTipManager.GetInstance().ShowTip(var2);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
          
        }

        public override bool IsNeedUpdate()
        {
            return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
           
        }
    }
}
