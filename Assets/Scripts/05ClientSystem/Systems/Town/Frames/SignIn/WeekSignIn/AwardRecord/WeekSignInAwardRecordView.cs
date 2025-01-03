using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class WeekSignInAwardRecordView : MonoBehaviour
    {
        private WeekSignInType _weekSingInType = WeekSignInType.None;
        private List<WeekSignRecord> _weekSingInRecordList;

        private string _awardRecordDescription;

        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField]
        private Text title;

        [SerializeField] private Button closeButton;

        [SerializeField] private Text awardZeroTips;

        [Space(10)]
        [HeaderAttribute("Title")]
        [Space(10)]
        [SerializeField]
        private ComUIListScript awardItemList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();

            WeekSignInDataManager.GetInstance().ResetWeekSignInRecord();
        }

        private void BindEvents()
        {
            if (awardItemList != null)
            {
                awardItemList.Initialize();
                awardItemList.onItemVisiable += OnItemVisible;
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveGasWeekSignInRecordRes, OnReceiveGASWeekSingInRecordRes);
        }

        private void UnBindEvents()
        {
            if (awardItemList != null)
            {
                awardItemList.onItemVisiable -= OnItemVisible;
            }

            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveGasWeekSignInRecordRes,
                OnReceiveGASWeekSingInRecordRes);
        }

        private void ClearData()
        {
            _weekSingInType = WeekSignInType.None;

            //将缓存的当前数据清除掉
            _weekSingInRecordList = null;
            _awardRecordDescription = null;

        }

        public void InitView(int awardRecordType)
        {
            _weekSingInType = (WeekSignInType)awardRecordType;

            //默认是活动周签到
            if (_weekSingInType != WeekSignInType.NewPlayerWeekSignIn
                && _weekSingInType != WeekSignInType.ActivityWeekSignIn)
            {
                _weekSingInType = WeekSignInType.ActivityWeekSignIn;
            }


            var commonTipsDesc = WeekSignInDataManager.ActivityWeekSignInTipDesId;
            if (_weekSingInType == WeekSignInType.NewPlayerWeekSignIn)
                commonTipsDesc = WeekSignInDataManager.NewPlayerWeekSignInTipDesId;
            //奖励记录的基础描述
            var commonTipDesTable = TableManager.GetInstance().GetTableItem<CommonTipsDesc>(commonTipsDesc);
            if (commonTipDesTable != null)
                _awardRecordDescription = commonTipDesTable.Descs;

            InitContent();


            WeekSignInDataManager.GetInstance().SendGASWeekSignRecordReq(_weekSingInType);
        }

        private void InitContent()
        {
            if (title != null)
                title.text = TR.Value("week_sing_in_award_record_title");

            if (awardZeroTips != null)
                awardZeroTips.text = TR.Value("week_sing_in_award_record_zero");
        }

        private void OnReceiveGASWeekSingInRecordRes(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var weekSignInType = (WeekSignInType)uiEvent.Param1;
            if (weekSignInType != _weekSingInType)
                return;

            UpdateAwardRecordItemList();

        }

        private void UpdateAwardRecordItemList()
        {
            var awardRecordItemCount = 0;

            _weekSingInRecordList = WeekSignInDataManager.GetInstance()
                .GetWeekSignInRecordListByWeekSignInType(_weekSingInType);

            if (_weekSingInRecordList != null && _weekSingInRecordList.Count > 0)
                awardRecordItemCount = _weekSingInRecordList.Count;

            if (awardItemList != null)
                awardItemList.SetElementAmount(awardRecordItemCount);
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_weekSingInRecordList == null
                || _weekSingInRecordList.Count <= 0)
                return;

            if (awardItemList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _weekSingInRecordList.Count)
                return;

            var awardRecordDataModel = _weekSingInRecordList[item.m_index];
            var awardRecordItem = item.GetComponent<WeekSignInAwardRecordItem>();
            if (awardRecordDataModel != null && awardRecordItem != null)
            {

                var linkParseString = CommonUtility.GetItemNameLinkParseString((int)awardRecordDataModel.itemId);
                //服务器名字，角色名字，道具名字，道具数量
                var awardRecordItemStr = string.Format(_awardRecordDescription,
                    awardRecordDataModel.serverName,
                    awardRecordDataModel.roleName,
                    linkParseString,
                    awardRecordDataModel.itemNum);
                awardRecordItemStr = awardRecordItemStr.Replace(System.Environment.NewLine, "")
                    .Replace("\t", "");
                awardRecordItem.InitItem(awardRecordItemStr);
                
            }
        }

        private void OnCloseButtonClick()
        {
            WeekSignInUtility.OnCloseWeekSignInAwardRecordFrame();
        }


    }
}
