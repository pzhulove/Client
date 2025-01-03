using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnChallengeMapToggleClick(DungeonModelTable.eType modelType);

    public class ChallengeMapModelDataModel
    {
        public int ModelId;
        public DungeonModelTable.eType ModelType;
        public string ToggleName;
    }


    public class ChallengeMapModelControl : MonoBehaviour
    {


        private DungeonModelTable.eType _defaultModelType;

        private List<ChallengeMapModelDataModel> _toggleDataModelList = new List<ChallengeMapModelDataModel>();

        private OnChallengeMapToggleClick _onChallengeMapToggleClick;

        [SerializeField] private ComUIListScript modelItemList;


        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (modelItemList != null)
            {
                modelItemList.Initialize();
                modelItemList.onItemVisiable += OnModelItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (modelItemList != null)
                modelItemList.onItemVisiable -= OnModelItemVisible;
        }

        private void ClearData()
        {
            _defaultModelType = DungeonModelTable.eType.Type_None;
            _toggleDataModelList = null;
            _onChallengeMapToggleClick = null;
        }

        //需要传递默认的参数
        public void InitMapModelControl(DungeonModelTable.eType defaultModelType,
            OnChallengeMapToggleClick onChallengeMapToggleClick)
        {

            _defaultModelType = defaultModelType;
            _onChallengeMapToggleClick = onChallengeMapToggleClick;

            InitMapModelContent();
        }

        private void InitToggleDataModelList()
        {
            var dungeonModelTables = TableManager.GetInstance().GetTable<DungeonModelTable>();  //地下城模式表中的模式（深渊 远古之类）

            if (dungeonModelTables != null)
            {
                var iter = dungeonModelTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dungeonModelTable = iter.Current.Value as DungeonModelTable;

                    if (dungeonModelTable != null)
                    {
                        if (dungeonModelTable.Type == DungeonModelTable.eType.WeekHellModel
                            || dungeonModelTable.Type == DungeonModelTable.eType.TeamDuplicationModel)      //混沌地下城和团本暂时先屏蔽
                        {
                            // 开放团本 ckm
                            // continue;
                        }

                        if (dungeonModelTable.Type == DungeonModelTable.eType.VoidCrackModel)       //虚空裂缝模式
                        {
                            FunctionUnLock unLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.YijieAreaOpen);
                            if (unLockData == null)
                            {
                                continue;
                            }

                            if (!Utility.IsUnLockArea(unLockData.AreaID))
                            {
                                continue;
                            }
                        }

                        
                        if (dungeonModelTable.Type == DungeonModelTable.eType.TeamDuplicationModel)
                        {
                            //团本，服务器开关没有打开，不添加团本的页签
                            if (TeamDuplicationUtility.IsTeamDuplicationServerSwitchOpen() == false)
                            {
                                continue;
                            }
                        }

                        var toggleDataModel = new ChallengeMapModelDataModel();
                        toggleDataModel.ModelType = dungeonModelTable.Type;
                        toggleDataModel.ToggleName = dungeonModelTable.Name;
                        toggleDataModel.ModelId = dungeonModelTable.ID;

                        if (_toggleDataModelList == null)
                            _toggleDataModelList = new List<ChallengeMapModelDataModel>();
                        _toggleDataModelList.Add(toggleDataModel);
                    }
                }
            }
        }

        private void InitMapModelContent()
        {

            if (_defaultModelType == DungeonModelTable.eType.Type_None)
                _defaultModelType = DungeonModelTable.eType.YunShangChangAnModel;
                // _defaultModelType = DungeonModelTable.eType.DeepModel;


            InitToggleDataModelList();  //对_toggleDataModelList进行初始化

            if (modelItemList != null)
            {
                if (_toggleDataModelList == null)
                    modelItemList.SetElementAmount(0);
                else
                    modelItemList.SetElementAmount(_toggleDataModelList.Count);
            }
        }
        
        private void OnModelItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var modelItem = item.GetComponent<ChallengeMapModelItem>();
            if (modelItem == null)
                return;

            if (_toggleDataModelList != null
                && item.m_index >= 0 && item.m_index < _toggleDataModelList.Count)
            {
                var dataModel = _toggleDataModelList[item.m_index];
                if (dataModel != null)
                {
                    if (_defaultModelType == dataModel.ModelType)
                    {
                        modelItem.Init(dataModel,
                            OnTabClicked,
                            true);
                    }
                    else
                    {
                        modelItem.Init(dataModel,
                            OnTabClicked,
                            false);
                    }
                }
            }
        }

        private void OnTabClicked(DungeonModelTable.eType modelType)
        {
            if (_onChallengeMapToggleClick != null)
                _onChallengeMapToggleClick(modelType);
        }
        
    }
}