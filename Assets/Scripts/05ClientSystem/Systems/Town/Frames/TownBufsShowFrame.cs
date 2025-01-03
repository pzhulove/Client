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
    // TownBufsShow
    public class TownBufsShowFrame : GameFrame
    {
        #region inner define

        public class TownBufInfo
        {
            public int bufID = 0;
            public int endTime = 0;
        }

        #endregion

        #region val
        List<object> townBufComUIListDatas = new List<object>();
        #endregion

        #region ui bind
        ComUIListScript townBufComUIList = null;
        Text testTxt = null;
        Button testBtn = null;
        Image testImg = null;
        Slider testSlider = null;
        Toggle testToggle = null;
        GameObject testGo = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/TownBufsShow";
        }

        protected override void OnOpenFrame()
        {
            InitTownBufComUIList();
            UpdateTownBufComUIList();                
        }

        protected override void OnCloseFrame()
        { 
            
        }

        protected override void _bindExUI()
        {
            townBufComUIList = mBind.GetCom<ComUIListScript>("townBufComUIList");
            testTxt = mBind.GetCom<Text>("testTxt");

            testBtn = mBind.GetCom<Button>("testBtn");
            testBtn.SafeSetOnClickListener(() => 
            {

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
            townBufComUIList = null;

            testTxt = null;
            testBtn = null;
            testImg = null;
            testSlider = null;
            testToggle = null;
            testGo = null;
        }

        protected override void OnBindUIEvent()
        {
            BindUIEvent(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(_OnTaskChange);
        }

        protected override void OnUnBindUIEvent()
        {
            UnBindUIEvent(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveSyncTaskDataChangeListener(_OnTaskChange);
        }

        public override bool IsNeedUpdate()
        {
            return false;//false means _OnUpdate is invalid
        }

        protected override void _OnUpdate(float timeElapsed)
        {

        }

        #endregion

        #region method

        void InitTownBufComUIList()
        {
            if(townBufComUIList == null)
            {
                return;
            }

            townBufComUIList.Initialize();
            townBufComUIList.onBindItem = (go) => 
            {
                return go;
            };

            townBufComUIList.onItemVisiable = (go) => 
            {
                if(go == null)
                {
                    return;
                }

                if(townBufComUIListDatas == null)
                {
                    return;
                }

                ComUIListTemplateItem comUIListItem = go.GetComponent<ComUIListTemplateItem>();
                if(comUIListItem == null)
                {
                    return;
                }

                if(go.m_index >= 0 && go.m_index < townBufComUIListDatas.Count)
                {
                    comUIListItem.SetUp(townBufComUIListDatas[go.m_index]);
                }                
            };          
        }

        void CalcTownBufComUIListDatas()
        {
            if(townBufComUIListDatas == null)
            {
                townBufComUIListDatas = new List<object>();
            }
            else
            {
                townBufComUIListDatas.Clear();
            }
            GetFatigueBuff();
        }
        //获取燃烧buff信息
        private void GetFatigueBuff()
        {
            var curData = ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.GetCurUseFatigueData();
            if (null == curData)
                return;
            TownBufsShowFrame.TownBufInfo info = new TownBufsShowFrame.TownBufInfo();
            info.endTime = curData.DoneNum;
            string strID = curData.DataId.ToString();
            string str = strID.Substring(strID.Length - 1);
            int index = 0;
            if (int.TryParse(str, out index))
            {
                //普通
                if (1 == index)
                    info.bufID = Utility.GetClientIntValue(ClientConstValueTable.eKey.NORMAL_FATIGUE_BUFF_INFO_ID, 1001);
                //高级
                else
                    info.bufID = Utility.GetClientIntValue(ClientConstValueTable.eKey.HIGH_FATIGUE_BUFF_INFO_ID, 1002);
            }
            townBufComUIListDatas.Add(info);
        }

        void UpdateTownBufComUIList()
        {
            if(townBufComUIList == null)
            {
                return;
            }

            CalcTownBufComUIListDatas();
            if(townBufComUIListDatas == null)
            {
                return;
            }

            townBufComUIList.SetElementAmount(townBufComUIListDatas.Count);            
        }       
        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {           
            return;
        }

        /// <summary>
        /// 疲劳燃烧活动任务变化
        /// </summary>
        private void _OnTaskChange()
        {
            UpdateTownBufComUIList();
        }

        #endregion
    }
}
