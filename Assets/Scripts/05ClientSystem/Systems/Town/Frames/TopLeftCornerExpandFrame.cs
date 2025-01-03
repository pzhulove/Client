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
    // 左上角按钮扩展
    public class TopLeftCornerExpandFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        List<object> testComUIListDatas = new List<object>();
        #endregion

        #region ui bind
        ComUIListScript testComUIList = null;
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
            return "UIFlatten/Prefabs/MainFrameTown/TopLeftCornerExpand";
        }

        protected override void OnOpenFrame()
        {
            InitTestComUIList();
            UpdateTestComUIList();                
        }

        protected override void OnCloseFrame()
        { 
            
        }

        protected override void _bindExUI()
        {
            testComUIList = mBind.GetCom<ComUIListScript>("testComUIList");
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
            testComUIList = null;

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
        }

        protected override void OnUnBindUIEvent()
        {
            UnBindUIEvent(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
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
