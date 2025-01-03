using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using ProtoTable;

namespace GameClient
{
    class LegendSeriesFrameData
    {
        public ProtoTable.LegendMainTable mainItem;
    }

    public class LegendSeriesFrame : ClientFrame
    {
        LegendSeriesFrameData data = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LegendFrame/LegendSeriesFrame";
        }

        void _InitMain()
        {
            if (data != null && data.mainItem != null)
            {
                Image imgBG = Utility.FindComponent<Image>(frame, "BG");
                if (null != imgBG && data.mainItem.Icons.Count >= 3)
                {
                    // imgBG.sprite = AssetLoader.instance.LoadRes(data.mainItem.Icons[2], typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref imgBG, data.mainItem.Icons[2]);
                }

                Text desc = Utility.FindComponent<Text>(frame, "Desc");
                if (null != desc)
                {
                    desc.text = data.mainItem.Desc;
                }

                Text openCondition = Utility.FindComponent<Text>(frame, "OpenCondition");
                bool bOpen = PlayerBaseData.GetInstance().Level >= data.mainItem.UnLockLevel;
                openCondition.gameObject.CustomActive(!bOpen);
                if(!bOpen)
                {
                    openCondition.text = TR.Value("legend_series_open_desc", data.mainItem.UnLockLevel);
                }
            }
        }

        void _InitGuidance()
        {
            if (data != null && data.mainItem != null)
            {
                GameObject goParent = Utility.FindChild(frame, "LinkItemParent/ScrollView/ViewPort/Content");
                GameObject goPrefab = Utility.FindChild(frame, "LinkItemParent/ScrollView/ViewPort/Content/Prefab");
                GameObject goNewPrefab = Utility.FindChild(frame, "LinkItemParent/ScrollView/ViewPort/Content/PrefabNew");
                goPrefab.CustomActive(false);
                if (goNewPrefab != null)
                {
                    goNewPrefab.CustomActive(false);
                }
                m_akLegendItems.RecycleAllObject();

                for (int i = 0; i < data.mainItem.LinkItems.Count; ++i)
                {
                    var legendItem = TableManager.GetInstance().GetTableItem<ProtoTable.LegendTraceTable>(data.mainItem.LinkItems[i]);
                    if (null != legendItem)
                    {
                        //创建任务
                        if (legendItem.LinkMissionID > 0)
                        {
                            var legendMissionItem = TableManager.GetInstance()
                                .GetTableItem<MissionTable>(legendItem.LinkMissionID);
                            if (legendMissionItem != null)
                            {
                                var legendMissionItemPrefab = GameObject.Instantiate(goNewPrefab) as GameObject;
                                Utility.AttachTo(legendMissionItemPrefab, goParent);
                                LegendMissionItemNew legendMissionItemNew =
                                    legendMissionItemPrefab.transform.GetComponent<LegendMissionItemNew>();
                                if (legendMissionItemNew != null)
                                {
                                    var singleMissionInfo = GetLegendMissionItemNewData(legendItem.LinkMissionID, data.mainItem.ID);
                                    legendMissionItemNew.Init(singleMissionInfo.missionValue);
                                }
                                legendMissionItemPrefab.CustomActive(true);
                            }
                        }
                        else
                        {
                            m_akLegendItems.Create(new object[]
                            {
                                goParent,
                                goPrefab,
                                new LegendItemData
                                {
                                    methodItem = legendItem,
                                    bFirst = i == 0,
                                },
                                false
                            });
                        }
                    }
                }
            }
        }

        GameObject goMissionParent = null;
        GameObject goMissionPrefab = null;
        void _InitLegendMissions()
        {
            goMissionParent = Utility.FindChild(frame, "LinkMissionParent/ScrollView/ViewPort/Content");
            goMissionPrefab = Utility.FindChild(frame, "LinkMissionParent/ScrollView/ViewPort/Content/Prefab");
            goMissionPrefab.CustomActive(false);

            _RefreshLegendMissions();
        }

        void _UnInitLegendMissions()
        {
            goMissionParent = null;
            goMissionPrefab = null;

            m_akLegendMissionItems.DestroyAllObjects();
        }

        void _InitUIEvent()
        {
            MissionManager.GetInstance().onAddNewMission += DelegateAddNewMission;
            MissionManager.GetInstance().onDeleteMission += DelegateDeleteMission;
            MissionManager.GetInstance().onUpdateMission += DelegateUpdateMission;
            MissionManager.GetInstance().onSyncMission += DelegateSyncMission;
        }

        void DelegateAddNewMission(UInt32 taskID)
        {
            if(Utility.IsLegendMission(taskID) && data.mainItem.missionIds.Contains((int)taskID))
            {
                var find = m_akLegendMissionItems.Find(x =>
                {
                    return x.Value.missionValue.taskID == taskID;
                });

                if(null != find)
                {
                    var missionValue = MissionManager.GetInstance().GetMission(taskID);
                    if(null != missionValue)
                    {
                        var legendMissionItemData = CreateLegendMissionData(missionValue, data.mainItem.ID);
                        find.OnRefresh(new object[] { legendMissionItemData });
                    }
                }
                else
                {
                    var legendMIData = _CreateLegendMissionData((int)taskID, data.mainItem.ID);
                    if (null != legendMIData)
                    {
                        _CreateLegendMission(legendMIData);
                    }
                }

                _SortLegendMissions();
            }
        }

        private void DelegateSyncMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID) && data.mainItem.missionIds.Contains((int)taskID))
            {
                var find = m_akLegendMissionItems.Find(x =>
                {
                    return x.Value.missionValue.taskID == taskID;
                });

                var missionValue = MissionManager.GetInstance().GetMission(taskID);
                if (null == missionValue)
                {
                    return;
                }

                if (null == find)
                {
                    return;
                }

                var legendMissionItemData = CreateLegendMissionData(missionValue, data.mainItem.ID);
                find.OnRefresh(new object[] { legendMissionItemData });
            }
        }

        void DelegateUpdateMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID) && data.mainItem.missionIds.Contains((int)taskID))
            {
                var find = m_akLegendMissionItems.Find(x =>
                {
                    return x.Value.missionValue.taskID == taskID;
                });

                var missionValue = MissionManager.GetInstance().GetMission(taskID);
                if(null == missionValue)
                {
                    return;
                }

                if(null == find)
                {
                    return;
                }

                var legendMissionItemData = CreateLegendMissionData(missionValue, data.mainItem.ID);
                find.OnRefresh(new object[] { legendMissionItemData});

                _SortLegendMissions();
            }
        }

        void DelegateDeleteMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID) && data.mainItem.missionIds.Contains((int)taskID))
            {
                var find = m_akLegendMissionItems.Find(x =>
                {
                    return x.Value.missionValue.taskID == taskID;
                });

                var missionValue = MissionManager.GetInstance().GetMission(taskID);
                if (null == missionValue)
                {
                    return;
                }

                if (null == find)
                {
                    return;
                }

                var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)taskID);
                if(null == missionItem)
                {
                    return;
                }

                missionValue = _CreateMissionFromTable(taskID);

                var legendMissionItemData = CreateLegendMissionData(missionValue, data.mainItem.ID);
                find.OnRefresh(new object[] { legendMissionItemData });

                _SortLegendMissions();
            }
        }

        MissionManager.SingleMissionInfo _CreateMissionFromTable(uint taskID)
        {
            MissionManager.SingleMissionInfo ret = null;
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)taskID);
            if(null != missionItem)
            {
                ret = new MissionManager.SingleMissionInfo
                {
                    taskID = taskID,
                    missionItem = missionItem,
                    status = 0,
                };
            }
            return ret;
        }

        void _SortLegendMissions()
        {
            m_akLegendMissionItems.ActiveObjects.Sort((x, y) =>
            {
                return x.Value.missionValue.LegendCompareTo(y.Value.missionValue);
            });

            for(int i = 0; i < m_akLegendMissionItems.ActiveObjects.Count;++i)
            {
                m_akLegendMissionItems.ActiveObjects[i].SetSiblingIndex(i);
            }
        }

        void _RefreshLegendMissions()
        {
            m_akLegendMissionItems.RecycleAllObject();
            var pools = GamePool.ListPool<object>.Get();
            for (int i = 0; i < data.mainItem.missionIds.Count; ++i)
            {
                var legendMIData = _CreateLegendMissionData(data.mainItem.missionIds[i],data.mainItem.ID);
                if (null != legendMIData)
                {
                    pools.Add(legendMIData);
                }
            }

            pools.Sort((x, y) =>
            {
                return (x as LegendMissionItemData).missionValue.LegendCompareTo((y as LegendMissionItemData).missionValue);
            });

            for(int i = 0; i < pools.Count; ++i)
            {
                _CreateLegendMission(pools[i] as LegendMissionItemData);
            }

            GamePool.ListPool<object>.Release(pools);
        }

        private LegendMissionItemData GetLegendMissionItemNewData(int iTaskID,int legendId = -1)
        {
            return _CreateLegendMissionData(iTaskID, legendId);
        }
        
        private LegendMissionItemData CreateLegendMissionData(MissionManager.SingleMissionInfo curmissionValue,
            int curLegendId)
        {
            return new LegendMissionItemData
            {
                missionValue = curmissionValue,
                LegendId = curLegendId,
            };
        }

        LegendMissionItemData _CreateLegendMissionData(int iTaskID,int legendId = -1)
        {
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
            if (null != missionItem && missionItem.MissionOnOff == 1)
            {
                var missionAwards = MissionManager.GetInstance().GetMissionAwards(iTaskID);
                if(null == missionAwards || missionAwards.Count <= 0)
                {
                    return null;
                }

                var missionValue = MissionManager.GetInstance().GetMission((uint)missionItem.ID);
                if (null == missionValue)
                {
                    missionValue = _CreateMissionFromTable((uint)missionItem.ID);
                }
                return new LegendMissionItemData
                {
                    LegendId = legendId,
                    missionValue = missionValue
                };
            }
            return null;
        }

        void _CreateLegendMission(LegendMissionItemData lmiD)
        {
            m_akLegendMissionItems.Create(new object[] {
                            goMissionParent,
                            goMissionPrefab,
                            lmiD,
                            false
                        });
        }

        void _UnInitUIEvent()
        {
            MissionManager.GetInstance().onAddNewMission -= DelegateAddNewMission;
            MissionManager.GetInstance().onDeleteMission -= DelegateDeleteMission;
            MissionManager.GetInstance().onUpdateMission -= DelegateUpdateMission;
            MissionManager.GetInstance().onSyncMission -= DelegateSyncMission;
        }

        protected override void _OnOpenFrame()
        {
            data = userData as LegendSeriesFrameData;
            _InitMain();
            _InitGuidance();
            _InitLegendMissions();
            _InitUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _UnInitUIEvent();
            _UnInitLegendMissions();
            m_akLegendItems.DestroyAllObjects();
            data = null;
        }

        CachedObjectListManager<LegendItem> m_akLegendItems = new CachedObjectListManager<LegendItem>();
        CachedObjectListManager<LegendMissionItem> m_akLegendMissionItems = new CachedObjectListManager<LegendMissionItem>();
    }
}