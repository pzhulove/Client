using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    class ActiveAwardBinder : MonoBehaviour
    {
        public int iBindActiveID;
        public int iActiveConfigID;
        public UnityEngine.UI.Image bindImg;
        public ComItem comItem = null;

        void Start()
        {

            _CheckStatus();
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
            
			_destoryComItem();
        }

		private void _destoryComItem()
		{
			if (comItem)
			{
				ComItemManager.Destroy (comItem);
				comItem = null;
			}
		}

		private void _createComItem(int id, GameObject root)
		{
			var itemData = ItemDataManager.GetInstance ().GetCommonItemTableDataByID (id);
			if (itemData != null) 
			{
				comItem = ComItemManager.Create (root);
				comItem.Setup (itemData, null);
			}
		}

        void _BindImage()
        {
            bindImg.enabled = false;
		
            if(bindImg == null)
            {
                return;
            }

            var awards = ActiveManager.GetInstance().GetActiveAwards(iBindActiveID);
            if(awards == null || awards.Count <= 0)
            {
                return;
            }

            // first 
            if (null == comItem)
            {
				_createComItem(awards[0].ID, bindImg.gameObject);
            }
            else if (comItem.ItemData.TableID != awards [0].ID)
            {
				_destoryComItem();
				_createComItem(awards[0].ID, bindImg.gameObject);
            }

            bindImg.enabled = false;
        }

        void _CheckStatus()
        {
            bool bShow = true;
            var activeData = ActiveManager.GetInstance().GetActiveData(iActiveConfigID);
            if (activeData != null)
            {
                bShow = activeData.mainInfo.state == 0;
            }
            gameObject.CustomActive(bShow);

            _BindImage();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            _CheckStatus();
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _CheckStatus();
            }
        }

        public void _OnClickPreView()
        {
            var activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(iBindActiveID);
            if(activeItem == null)
            {
                return;
            }

            ActiveAwardFrameData data = new ActiveAwardFrameData();
            data.title = string.Format(TR.Value("activity_award_title"), (iActiveConfigID - 7000) / 100);
            data.datas = ActiveManager.GetInstance().GetActiveAwards(iBindActiveID);

            ClientSystemManager.GetInstance().OpenFrame<ActiveAwardFrame>(FrameLayer.Middle, data);
        }
    }
}
