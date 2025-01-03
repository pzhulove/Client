using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinPreviewView : MonoBehaviour
    {
        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField]
        private Text moneyNum;
        
        [SerializeField]
        private Button CloseBtn;
        [Space(5)]
        [HeaderAttribute("Middle")]
        [SerializeField]
        private ComUIListScript giftComUIList;
        [Space(5)]
        [HeaderAttribute("Bottom")]
        [SerializeField]
        private Button GoToMallBtn;
        private List<MallItemInfo> tempMallItemInfoList = new List<MallItemInfo>();
        private int specialItemId;
        public void InitPreviewView()
        {
            specialItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_GNOME_COIN_DATA_ID).Value;
            InitComUIList();
            InitBtn();
            InitUI();
            List<MallItemInfo> _limitTimeMallElementDataModelList = new List<MallItemInfo>();
            _limitTimeMallElementDataModelList = MallNewDataManager.GetInstance().GetMallItemInfoList(6);
            tempMallItemInfoList.Clear();
            if(_limitTimeMallElementDataModelList != null)
            {
                for (int i = 0; i < _limitTimeMallElementDataModelList.Count; i++)
                {
                    if (_limitTimeMallElementDataModelList[i].buyGotInfos != null && _limitTimeMallElementDataModelList[i].buyGotInfos.Length != 0)
                    {
                        tempMallItemInfoList.Add(_limitTimeMallElementDataModelList[i]);
                    }
                }
                giftComUIList.SetElementAmount(tempMallItemInfoList.Count);
            }
        }
        private void InitComUIList()
        {
            giftComUIList.Initialize();
            giftComUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    GoblinPreviewItem previewItem = item.GetComponent<GoblinPreviewItem>();
                    if(previewItem != null)
                    {
                        previewItem.InitUI(tempMallItemInfoList[item.m_index]);
                    }
                }
            };
        }
        private void InitBtn()
        {
            CloseBtn.onClick.RemoveAllListeners();
            CloseBtn.onClick.AddListener(() =>
            {
                ClientSystemManager.GetInstance().CloseFrame<GoblinPreviewFrame>();
            });

            GoToMallBtn.onClick.RemoveAllListeners();
            GoToMallBtn.onClick.AddListener(() =>
            {
                ClientSystemManager.GetInstance().CloseFrame<GoblinPreviewFrame>();
                ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall });
                //ClientSystemManager.instance.OpenFrame<MallFrame>(FrameLayer.Middle, new OutComeData() { MainTab = MallType.Gift });
            });
        }

        private void InitUI()
        {
            moneyNum.text = AccountShopDataManager.GetInstance().GetSpecialItemNum(specialItemId).ToString();
        }
    }
}