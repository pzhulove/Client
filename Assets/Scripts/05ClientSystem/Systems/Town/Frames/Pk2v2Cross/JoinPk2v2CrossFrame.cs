using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // JoinPk2v2Cross
    public class JoinPk2v2CrossFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        List<object> testComUIListDatas = new List<object>();
        const int ITEM_NUM = 4;
        #endregion

        #region ui bind
        ComUIListScript testComUIList = null;
        Text testTxt = null;
        Button Join = null;
        Image testImg = null;
        Slider testSlider = null;
        Toggle testToggle = null;
        GameObject testGo = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk2v2Cross/JoinPk2v2Cross";
        }

        protected override void OnOpenFrame()
        {
            InitTestComUIList();
            UpdateTestComUIList();

            BindUIEvent();

            int iTableID = 0;
            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWar2v2RewardTable>();
                if (dicts == null)
                {
                    Logger.LogErrorFormat("TableManager.instance.GetTable<ScoreWarRewardTable>() error!!!");
                    return;
                }
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    ScoreWar2v2RewardTable adt = iter.Current.Value as ScoreWar2v2RewardTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.RewardPreview.Count > 1)
                    {
                        iTableID = adt.ID;
                        break;
                    }
                }
            }

            ScoreWar2v2RewardTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWar2v2RewardTable>(iTableID);
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

        protected override void OnCloseFrame()
        { 
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            testComUIList = mBind.GetCom<ComUIListScript>("testComUIList");
            testTxt = mBind.GetCom<Text>("testTxt");

            Join = mBind.GetCom<Button>("Join");
            Join.SafeSetOnClickListener(() => 
            {
                if(TeamDataManager.GetInstance().HasTeam())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("2v2melee_score_war_can_not_enter_with_team"));
                    return;
                }

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

                GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                           new SceneParams
                           {
                               currSceneID = systemTown.CurrentSceneID,
                               currDoorID = 0,
                               targetSceneID = 5008,
                               targetDoorID = 0,
                           }));

                frameMgr.CloseFrame(this);
            });

            testImg = mBind.GetCom<Image>("testImg");

            testSlider = mBind.GetCom<Slider>("testSlider");
            testSlider.SafeSetValueChangeListener((value) => 
            {

            });

            testToggle = mBind.GetCom<Toggle>("testToggle");
            testToggle.SafeSetOnValueChangedListener((value) => 
            {

            });

            testGo = mBind.GetGameObject("testGo");
        }

        protected override void _unbindExUI()
        {
            testComUIList = null;

            testTxt = null;
            Join = null;
            testImg = null;
            testSlider = null;
            testToggle = null;
            testGo = null;
        }

        public override bool IsNeedUpdate()
        {
            return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {

        }

        #endregion

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);   
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);       
        }

        void InitTestComUIList()
        {
            if(testComUIList == null)
            {
                return;
            }

            testComUIList.Initialize();
            testComUIList.onBindItem = (go) => 
            {
                return go;
            };

            testComUIList.onItemVisiable = (go) => 
            {
                if(go == null)
                {
                    return;
                }

                if(testComUIListDatas == null)
                {
                    return;
                }

                ComUIListTemplateItem comUIListItem = go.GetComponent<ComUIListTemplateItem>();
                if(comUIListItem == null)
                {
                    return;
                }

                if(go.m_index >= 0 && go.m_index < testComUIListDatas.Count)
                {
                    comUIListItem.SetUp(testComUIListDatas[go.m_index]);
                }                
            };          
        }

        void CalcTestComUIListDatas()
        {
            testComUIListDatas = new List<object>();
            if(testComUIListDatas == null)
            {
                return;
            }
        }

        void UpdateTestComUIList()
        {
            if(testComUIList == null)
            {
                return;
            }

            CalcTestComUIListDatas();
            if(testComUIListDatas == null)
            {
                return;
            }

            testComUIList.SetElementAmount(testComUIListDatas.Count);            
        }
                       
        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {           
            return;
        }

        #endregion
    }
}
