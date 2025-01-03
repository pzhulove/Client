using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;

namespace GameClient
{
    class AchievementLevelAwardFrame : ClientFrame
    {
        [UIControl("AwardItems", typeof(ComUIListScript))]
        ComUIListScript comAwardList;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActiveGroup/AchievementLevelAwardFrame";
        }

        public static void CommandOpen(object argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<AchievementLevelAwardFrame>(FrameLayer.Middle,argv);
        }

        List<ProtoTable.AchievementLevelInfoTable> mItems = new List<ProtoTable.AchievementLevelInfoTable>(16);
        void _LoadAllAwardItems()
        {
            if (0 == mItems.Count)
            {
                var enumerator = TableManager.GetInstance().GetTable<ProtoTable.AchievementLevelInfoTable>().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    mItems.Add(enumerator.Current.Value as ProtoTable.AchievementLevelInfoTable);
                }
            }
        }

        void _InitializeAwardList()
        {
            if (null != comAwardList)
            {
                comAwardList.Initialize();
                comAwardList.onBindItem = (GameObject go) =>
                {
                    if (null != go)
                    {
                        return go.GetComponent<ComAchievementAwardItem>();
                    }
                    return null;
                };
                comAwardList.onItemVisiable = (ComUIListElementScript element) =>
                {
                    if (null != element)
                    {
                        var script = element.gameObjectBindScript as ComAchievementAwardItem;
                        if (null != script && element.m_index >= 0 && element.m_index < mItems.Count)
                        {
                            script.OnItemVisible(mItems[element.m_index]);
                        }
                    }
                };
            }
        }

        void _UpdateAwardList()
        {
            if(null != comAwardList)
            {
                comAwardList.SetElementAmount(mItems.Count);
            }
        }

        void _UnInitializeAwardList()
        {
            if (null != comAwardList)
            {
                comAwardList.onBindItem = null;
                comAwardList.onItemVisiable = null;
                comAwardList = null;
            }
        }

        protected override void _OnOpenFrame()
        {           
            _LoadAllAwardItems();
            _InitializeAwardList();
            _UpdateAwardList();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementMaskPropertyChanged, _OnOnAchievementMaskPropertyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementMaskPropertyChanged, _OnOnAchievementMaskPropertyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
            _UnInitializeAwardList();
        }

        void _OnOnAchievementMaskPropertyChanged(UIEvent uiEvent)
        {
            _UpdateAwardList();
        }

        void _OnAchievementScoreChanged(UIEvent uiEvent)
        {
            _UpdateAwardList();
        }
    }
}