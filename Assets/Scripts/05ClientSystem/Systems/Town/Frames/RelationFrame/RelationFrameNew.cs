using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;

namespace GameClient
{
    class RelationFrameData
    {
        public RelationOptionType eRelationOptionType = RelationOptionType.ROT_MY_FRIEND;
        public RelationTabType eRelationTabType = RelationTabType.RTT_COUNT;
        public RelationData eCurrentRelationData = null;
    }

    class RelationFrameNew : ClientFrame
    {
        RelationFrameData data = null;

        protected override void _OnOpenFrame()
        {
            data = userData as RelationFrameData;
            if(data == null)
            {
                data = new RelationFrameData();
            }
            _InitFilters();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _CloseChildrenFrames();
            _UnInitFilters();
            _UnRegisterUIEvent();
            data = null;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/RelationFrame";
        }

        public static void CommandOpen(RelationFrameData data = null)
        {
            if(null == data)
            {
                data = new RelationFrameData();
            }
            ClientSystemManager.GetInstance().OpenFrame<RelationFrameNew>(FrameLayer.Middle,data);
        }

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                CommandOpen(new RelationFrameData { eRelationOptionType = (RelationOptionType)int.Parse(strParam) });
            }
            catch(System.Exception e)
            {
                Logger.LogErrorFormat(e.ToString());
            }
        }
        private void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPStartTalk, _OnTAPStartTalk);
        }
        private void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPStartTalk, _OnTAPStartTalk);
        }
        void _OnTAPStartTalk(UIEvent iEvent)
        {
            string mTalk = "";
            data = iEvent.Param1 as RelationFrameData;
            if (iEvent.Param2 != null)
            {
                mTalk = iEvent.Param2 as string;
            }
            
            RelationOptionData optionData = new RelationOptionData();
            optionData.eRelationOptionType = data.eRelationOptionType;
            optionData.eRelationTabType = data.eRelationTabType;
            optionData.eCurrentRelationData = data.eCurrentRelationData;
            optionData.mTalk = mTalk;
            var find = m_akFilters.ActiveObjects.Find(x => { return x.Value.eRelationOptionType == optionData.eRelationOptionType; });
            if (null != find)
            {
                find.OnRefresh(new object[] { optionData } );
                find.OnSelected();
            }
            optionData.eCurrentRelationData = null;
        }
        #region filters
        [UIObject("VerticalFilter")]
        GameObject goFilterParent;
        [UIObject("VerticalFilter/Filter")]
        GameObject goFilter;
        CachedObjectListManager<RelationOption> m_akFilters = new CachedObjectListManager<RelationOption>();
        void _InitFilters()
        {
            goFilter.CustomActive(false);

            for (int i = 0; i < (int)RelationOptionType.ROT_COUNT; ++i)
            {
                m_akFilters.Create(new object[] 
                {
                    goFilterParent,
                    goFilter,
                    new RelationOptionData {
                        eRelationOptionType = (RelationOptionType)i,
                        eRelationTabType = data.eRelationTabType,
                        eCurrentRelationData = data.eCurrentRelationData
                    },
                    System.Delegate.CreateDelegate(typeof(RelationOption.OnSelectedDelegate),this,"OnSelectedDelegate"),
                    false
                });
            }

            var find = m_akFilters.ActiveObjects.Find(x => { return x.Value.eRelationOptionType == data.eRelationOptionType; });
            if(null != find)
            {
                find.OnSelected();
            }
        }

        void OnSelectedDelegate(RelationOptionData data)
        {
            if(data.eRelationOptionType < 0 || data.eRelationOptionType >= RelationOptionType.ROT_COUNT)
            {
                Logger.LogErrorFormat("enum value is out of range ,value = {0}", data.eRelationOptionType);
                return;
            }

            if (data.eRelationOptionType == RelationOptionType.ROT_MY_FRIEND)
            {
                if (friendFrame == null)
                {
                    friendFrame = ClientSystemManager.GetInstance().OpenFrame<RelationFriendFrame>(childrenRoot,
                                new RelationFriendFrameData
                                {
                                    eRelationTabType = data.eRelationTabType,
                                    eCurrentRelationData = data.eCurrentRelationData,
                                    mTalk = data.mTalk
                                }) as RelationFriendFrame;
                }

                //if (TapRelationInfoFrame != null)
                //{
                //    TapRelationInfoFrame.Close(true);
                //    TapRelationInfoFrame = null;
                //}
                if (TapFrame != null)
                {
                    TapFrame.Close(true);
                    TapFrame = null;
                }
                
            }
            else if (data.eRelationOptionType == RelationOptionType.ROT_TEACHERANDPUPIL)
            {
                if (TapFrame == null)
                {
                    TapFrame = ClientSystemManager.GetInstance().OpenFrame<TAPFrame>(childrenRoot) as TAPFrame;
                }

                if (friendFrame != null)
                {
                    friendFrame.Close(true);
                    friendFrame = null;
                }
            }
        }

        void _MainRelationTabChanged(UIEvent uiEvent)
        {
            RelationOptionType eRelationOptionType = (RelationOptionType)uiEvent.Param1;
            var find = m_akFilters.Find(x => { return x.Value.eRelationOptionType == eRelationOptionType; });
            if(null != find)
            {
                find.OnSelected();
            }
        }

        void _UnInitFilters()
        {
            m_akFilters.DestroyAllObjects();
        }
        #endregion

        [UIEventHandle("Close")]
        void _OnClickClose()
        {
            Close();
        }

        #region children frames
        [UIObject("Root")]
        GameObject childrenRoot;
        RelationFriendFrame friendFrame = null;
        TAPFrame TapFrame = null;
        void _CloseChildrenFrames()
        {
            if(null != friendFrame)
            {
                friendFrame.Close(true);
                friendFrame = null;
            }

            //if (TapRelationInfoFrame != null)
            //{
            //    TapRelationInfoFrame.Close(true);
            //    TapRelationInfoFrame = null;
            //}

            if (TapFrame != null)
            {
                TapFrame.Close(true);
                TapFrame = null;
            }
        }
        #endregion
    }
}