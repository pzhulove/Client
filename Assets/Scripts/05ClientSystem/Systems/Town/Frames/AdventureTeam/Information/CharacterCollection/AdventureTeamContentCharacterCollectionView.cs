using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamContentCharacterCollectionView : AdventureTeamContentBaseView
    {
        [HeaderAttribute("职业选择页签")]
        [SerializeField] private ComTabGroup jobTabControl;
        [HeaderAttribute("页签预制体")]
        [SerializeField] private string jobTabResPath = "UIFlatten/Prefabs/AdventureTeam/ComHorizonalTabItem";

        [HeaderAttribute("职业主界面")]
        [SerializeField] private ComUIListScriptExtension jobMainList;

        private int currJobTabId = 0;//默认为全部页签

        void Awake()
        {
            _Init();
            _BindUIEvent();
        }

        void OnDestroy()
        {
            _Clear();
            _UnBindUIEvent();
        }

        private void _Init()
        {
            if (jobMainList != null)
            {
                if (jobMainList.IsInitialised() == false)
                {
                    jobMainList.Initialize();
                    jobMainList.onItemVisiable += _OnJobMainListItemVisible;
                    jobMainList.OnItemRecycle += _OnJobMainListItemRecycle;
                    jobMainList.OnItemLocalPosAllInRectView += _OnJobMainListItemAllInRectView;
                }
            }
        }

        private void _Clear()
        {
            if (jobMainList != null)
            {
                jobMainList.SetElementAmount(0);
                jobMainList.onItemVisiable -= _OnJobMainListItemVisible;
                jobMainList.OnItemRecycle -= _OnJobMainListItemRecycle;
                jobMainList.OnItemLocalPosAllInRectView -= _OnJobMainListItemAllInRectView;
                jobMainList.UnInitialize();
            }

            currJobTabId = 0;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamCollectionInfoRes, _OnAdventureTeamCollectionInfoRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _OnLevelChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamCollectionInfoRes, _OnAdventureTeamCollectionInfoRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _OnLevelChanged);
        }

        private void _OnJobTabChanged(bool isOn, int index)
        {
            if (isOn)
            {
                var baseJobIds = AdventureTeamDataManager.GetInstance().GetTotalBaseJobTabIds();
                if (baseJobIds == null)
                    return;
                if (index < 0 && index >= baseJobIds.Length)
                    return;
                currJobTabId = baseJobIds[index];
                _RefreshJobMainList();
            }
        }

        private void _OnJobMainListItemVisible(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            var collectionDataList = AdventureTeamDataManager.GetInstance().GetCharacterCollectionModelsByBaseJobId(currJobTabId);
            if (collectionDataList == null || item.m_index >= collectionDataList.Count)
            {
                return;
            }

            //Logger.LogError("collectionItem visible , index : " + item.m_index);

            var collectionData = collectionDataList[item.m_index];
            var collectionItem = item.GetComponent<AdventureTeamCharacterCollectionItem>();
            if (collectionItem != null && collectionData != null)
            {                
                //collectionItem.IsInViewRect = jobMainList.IsElementTotalInScrollArea(item.m_index);
                //if (collectionData.isNewActivated && collectionItem.IsInViewRect)
                //{
                //    //collectionItem.onWaitActiveEffectPlayEnd = _OnWaitActiveEffectPlayEnd;
                //    collectionItem.PlayNewJobActivate(collectionData);                         
                //}

                //if (collectionItem.IsInViewRect)
                //{
                //    Logger.LogError("collectionItem in view , index : "+ item.m_index);
                //}


                collectionItem.onWaitActiveEffectPlayEnd += _OnWaitActiveEffectPlayEnd;
                //Logger.LogError("_OnJobMainListItemVisible : " + item.m_index);
                collectionItem.InitCollectionItem(collectionData);

                bool isAllIn = jobMainList.IsElementTotalInScrollArea(item.m_index);
                //如果不显示在界面上 则不执行播放操作 ！！！
                if (isAllIn && collectionData.needPlay && collectionItem.gameObject.activeInHierarchy)
                {
                    collectionItem.PlayNewJobActivate();
                }
            }
        }

        private void _OnJobMainListItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var collectionItem = item.GetComponent<AdventureTeamCharacterCollectionItem>();
            if (collectionItem != null)
            {
                collectionItem.onWaitActiveEffectPlayEnd -= _OnWaitActiveEffectPlayEnd;
                //Logger.LogError("_OnJobMainListItemRecycle : "+item.m_index);
                collectionItem.Clear();
            }
        }

        private void _OnJobMainListItemAllInRectView(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            var collectionDataList = AdventureTeamDataManager.GetInstance().GetCharacterCollectionModelsByBaseJobId(currJobTabId);
            if (collectionDataList == null || item.m_index >= collectionDataList.Count)
            {
                return;
            }
            var collectionData = collectionDataList[item.m_index];

            //Logger.LogError("collectionItem in view , index : " + item.m_index);

            var collectionItem = item.GetComponent<AdventureTeamCharacterCollectionItem>();
            if (collectionItem != null && collectionData != null)
            {
                //如果不显示在界面上 则不执行播放操作 ！！！
                if (collectionData.needPlay && collectionItem.gameObject.activeInHierarchy)
                {
                    collectionItem.PlayNewJobActivate();                    
                }
            }
        }

        private void _OnWaitActiveEffectPlayEnd(CharacterCollectionModel collectionData)
        {
            if (collectionData == null)
            {
                return;
            }
            AdventureTeamDataManager.GetInstance().ReqClearActivatedJob(new int[] { collectionData.jobTableId });
            //修改新激活状态 变为已激活
            AdventureTeamDataManager.GetInstance().ChangeSelectJobPlayStatus(collectionData, false); 
        }

        private void _OnAdventureTeamCollectionInfoRes(UIEvent uiEvent)
        {
            _RefreshJobMainList();
        }

        private void _OnLevelChanged(UIEvent uiEvent)
        {
            _TryRefreshView();
        }

        private void _RefreshJobMainList()
        {
            var collectionDataList = AdventureTeamDataManager.GetInstance().GetCharacterCollectionModelsByBaseJobId(currJobTabId);
            if (jobMainList != null && collectionDataList != null)
            {
                jobMainList.SetElementAmount(collectionDataList.Count);
                //页签不为0时刷新
                //由于该页签内的功能 涉及是存在缺陷的  所以全部页签后的其他页签 并无法触发 特效播放
                if (currJobTabId != 0)
                {
                    jobMainList.ResetContentPosition();
                }
            }
        }

        private void _TryRefreshView()
        {
			//默认刷新一次
            _RefreshJobMainList();

            int[] baseJobIds = new int[] { currJobTabId };
            if (currJobTabId == 0)
            {
                baseJobIds = AdventureTeamDataManager.GetInstance().GetTotalBaseJobTabIds();
            }
            AdventureTeamDataManager.GetInstance().ReqOwnJobInfo(baseJobIds);
        }

        public override void InitData()
        {
            var totalJobNames = AdventureTeamDataManager.GetInstance().GetTotalBaseJobNames();
            if (totalJobNames != null && totalJobNames.Length > 0)
            {
                if (jobTabControl != null)
                {
                    jobTabControl.Init(totalJobNames, jobTabResPath, _OnJobTabChanged, null, currJobTabId);
                }
            }

            _TryRefreshView();
        }

        public override void OnEnableView()
        {
            _TryRefreshView();
        }
    }
}