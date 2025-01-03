using UnityEngine;
using System.Collections;
using System;

namespace GameClient
{
    class LegendFrameData
    {
        public int iLegendID = 0;
    }

    public class LegendFrame : ClientFrame
    {
        public static void CommandOpen(object argv = null)
        {
            if(argv == null)
            {
                argv = new LegendFrameData();
            }

            ClientSystemManager.GetInstance().OpenFrame<LegendFrame>(FrameLayer.Middle, argv);
        }

        public static void OpenLinkFrame(string parms)
        {
            LegendFrameData data = new LegendFrameData();
            int.TryParse(parms, out data.iLegendID);

            ClientSystemManager.GetInstance().OpenFrame<LegendFrame>(FrameLayer.Middle, data);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LegendFrame/LegendFrame";
        }

        LegendFrameData data = null;

        protected override void _OnOpenFrame()
        {
            data = userData as LegendFrameData;
            if(data == null)
            {
                data = new LegendFrameData();
            }

            _InitBinder();
            _InitUIEvent();
            _InitTabs();
        }

        protected override void _OnCloseFrame()
        {
            LegendTab.Clear();
            m_akLegendTabs.DestroyAllObjects();
            data = null;
            _UnInitUIEvent();
            _UnInitBinder();
        }

        GameObject goChildFrameRoot;
        void _InitBinder()
        {
            goChildFrameRoot = Utility.FindChild(frame, "ChildFrame");
        }

        void _UnInitBinder()
        {
            goChildFrameRoot = null;
        }

        CachedObjectListManager<LegendTab> m_akLegendTabs = new CachedObjectListManager<LegendTab>();
        void _InitTabs()
        {
            GameObject goParent = Utility.FindThatChild("MainTab", frame);
            GameObject goPrefab = Utility.FindThatChild("MainTab/Prefab", frame);
            if (goPrefab == null)
            {
                goPrefab = Utility.FindThatChild("Prefab", frame);
            }
            goPrefab.CustomActive(false);

            var legendMainDatas = GamePool.ListPool<object>.Get();

            var legendTable = TableManager.GetInstance().GetTable<ProtoTable.LegendMainTable>();
            var enumerator = legendTable.GetEnumerator();
            while(enumerator.MoveNext())
            {
                ProtoTable.LegendMainTable legendData = enumerator.Current.Value as ProtoTable.LegendMainTable;
                if(null != legendData)
                {
                    legendMainDatas.Add(legendData);
                }
            }

            legendMainDatas.Sort((x, y) =>
            {
                return _SortLegendMain(x as ProtoTable.LegendMainTable, y as ProtoTable.LegendMainTable);
            });

            //if opened by default then selected the first priority
            if(data.iLegendID == 0)
            {
                if(legendMainDatas.Count > 0)
                {
                    data.iLegendID = (legendMainDatas[0] as ProtoTable.LegendMainTable).ID;
                }
            }
            else
            {
                var legendMainItem = legendMainDatas.Find(x =>
                {
                    return (x as ProtoTable.LegendMainTable).ID == data.iLegendID;
                });

                if(null == legendMainItem)
                {
                    if (legendMainDatas.Count > 0)
                    {
                        data.iLegendID = (legendMainDatas[0] as ProtoTable.LegendMainTable).ID;
                    }
                    else
                    {
                        data.iLegendID = 0;
                    }
                }
                else
                {
                    int status = Utility.GetLegendMainStatus(legendMainItem as ProtoTable.LegendMainTable);
                    if(status == 1)
                    {
                        data.iLegendID = (legendMainDatas[0] as ProtoTable.LegendMainTable).ID;
                    }
                }
            }

            if(data.iLegendID == 0)
            {
                Logger.LogErrorFormat("can not find legal selected legendmain item !!!");
                GamePool.ListPool<object>.Release(legendMainDatas);
                return;
            }

            for (int i = 0; i < legendMainDatas.Count; ++i)
            {
                m_akLegendTabs.Create(new object[] {
                        goParent ,
                        goPrefab ,
                        new LegendTabData { mainItem = (legendMainDatas[i] as ProtoTable.LegendMainTable)},
                        System.Delegate.CreateDelegate(typeof(LegendTab.OnSelectedDelegate), this, "_OnTabSelected") ,
                        false });
            }

            GamePool.ListPool<object>.Release(legendMainDatas);

            var find = m_akLegendTabs.Find(x =>
            {
                return x.Value.mainItem.ID == data.iLegendID;
            });

            if(null != find)
            {
                find.OnSelected();
            }
            else
            {
                if(m_akLegendTabs.ActiveObjects.Count > 0)
                {
                    m_akLegendTabs.ActiveObjects[0].OnSelected();
                }

                Logger.LogErrorFormat("this can not be happened! please check code !");
            }
        }

        void _SortTabs()
        {
            m_akLegendTabs.ActiveObjects.Sort((x, y) =>
            {
                return _SortLegendMain(x.Value.mainItem, y.Value.mainItem);
            });

            for (int i = 0; i < m_akLegendTabs.ActiveObjects.Count; ++i)
            {
                m_akLegendTabs.ActiveObjects[i].SetSiblingIndex(i);
            }

            for (int i = 0; i < m_akLegendTabs.ActiveObjects.Count; ++i)
            {
                m_akLegendTabs.ActiveObjects[i].OnUpdate();
            }
        }

        int _SortLegendMain(ProtoTable.LegendMainTable left, ProtoTable.LegendMainTable right)
        {
            int lStatus = Utility.GetLegendMainStatus(left);
            int rStatus = Utility.GetLegendMainStatus(right);
            if(lStatus != rStatus)
            {
                return lStatus - rStatus;
            }

            return left.SortID - right.SortID;
        }

        void _OnTabSelected(LegendTabData data)
        {
            LegendSeriesFrame legendFrame = _GetChildFrame(data.mainItem.ID) as LegendSeriesFrame;
            if(null == legendFrame)
            {
                legendFrame = ClientSystemManager.GetInstance().OpenFrame<LegendSeriesFrame>(goChildFrameRoot,
                    new LegendSeriesFrameData { mainItem = data.mainItem },
                    string.Format("LegendSeriesFrame_{0}", data.mainItem.ID)) as LegendSeriesFrame;

                _AddChildFrame(data.mainItem.ID, legendFrame);
            }

            for (int i = 0; i < childFrames.Count; ++i)
            {
                (childFrames[i].Value as LegendSeriesFrame).SetVisible(data.mainItem.ID == childFrames[i].Key);
            }
        }

        void _InitUIEvent()
        {
            MissionManager.GetInstance().onAddNewMission += DelegateAddNewMission;
            MissionManager.GetInstance().onUpdateMission += DelegateUpdateMission;
            MissionManager.GetInstance().onDeleteMission += DelegateDeleteMission;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;

            //_AddButton("ComWnd/Title/Close", () => { frameMgr.CloseFrame(this); });
        }

        void DelegateAddNewMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID))
            {
                _SortTabs();
            }
        }

        void DelegateUpdateMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID))
            {
                _SortTabs();
            }
        }

        void DelegateDeleteMission(UInt32 taskID)
        {
            if (Utility.IsLegendMission(taskID))
            {
                _SortTabs();
            }
        }

        void OnLevelChanged(int iPre,int iCur)
        {
            _SortTabs();
        }

        void _UnInitUIEvent()
        {
            MissionManager.GetInstance().onAddNewMission -= DelegateAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= DelegateUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= DelegateDeleteMission;
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }
    }
}