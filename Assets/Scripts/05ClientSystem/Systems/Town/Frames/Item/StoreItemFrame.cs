using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine.UI;

namespace GameClient
{
    class StoreItemFrame : ClientFrame
    {
        protected ItemData m_itemData;
        protected int m_count;

        protected ComItem m_comItem;

        //[UIControl("NumberArea/InputField", typeof(InputField))]
        protected InputField m_editCount;

        //[UIControl("ItemInfo/ItemName", typeof(Text))]
        protected Text m_itemName;

        private Button m_btnClose;
        private Button m_btnAdd;
        private Button m_btnMinus;
        private Button m_btnMax;
        private Button m_btnStore;
        private Button m_btnTake;
        private UIGray m_grayAdd;
        private UIGray m_grayMinus;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/StoreItem";
        }

        protected override void _OnOpenFrame()
        {
            //m_editCount.onValueChanged.RemoveAllListeners();
            //m_editCount.onValueChanged.AddListener((string value) => { _OnValueChanged(value); });
            _Update();
        }

        protected override void _bindExUI()
        {
            m_editCount = mBind.GetCom<InputField>("InputField");
            m_editCount.onValueChanged.RemoveAllListeners();
            m_editCount.onValueChanged.AddListener((string value) => { _OnValueChanged(value); });

            m_itemName = mBind.GetCom<Text>("ItemName");

            m_btnClose = mBind.GetCom<Button>("closeicon");
            if (m_btnClose != null)
            {
                m_btnClose.SafeAddOnClickListener(_OnClose);
            }
            m_btnAdd = mBind.GetCom<Button>("Increase");
            if (m_btnAdd != null)
            {
                m_btnAdd.SafeAddOnClickListener(_OnIncreaseNumber);
            }
            m_btnMinus = mBind.GetCom<Button>("Decrease");
            if (m_btnMinus != null)
            {
                m_btnMinus.SafeAddOnClickListener(_OnDecreaseNumber);
            }
            m_btnMax = mBind.GetCom<Button>("MaxNum");
            if (m_btnMax != null)
            {
                m_btnMax.SafeAddOnClickListener(_OnMaxNumber);
            }
            m_btnStore = mBind.GetCom<Button>("store");
            if (m_btnStore != null)
            {
                m_btnStore.SafeAddOnClickListener(_OnStore);
            }
            m_btnTake = mBind.GetCom<Button>("take");
            if (m_btnTake != null)
            {
                m_btnTake.SafeAddOnClickListener(_OnTake);
            }

            m_grayAdd = mBind.GetCom<UIGray>("IncreaseGray");
            m_grayMinus = mBind.GetCom<UIGray>("DecreaseGray");
        }

        protected override void _unbindExUI()
        {
            if (m_editCount != null)
            {
                m_editCount.onValueChanged.RemoveAllListeners();
            }
            m_editCount = null;

            m_itemName = null;

            if (m_btnClose != null)
            {
                m_btnClose.SafeRemoveOnClickListener(_OnClose);
            }
            m_btnClose = null;
            
            if (m_btnAdd != null)
            {
                m_btnAdd.SafeRemoveOnClickListener(_OnIncreaseNumber);
            }
            m_btnAdd = null;

            if (m_btnMinus != null)
            {
                m_btnMinus.SafeRemoveOnClickListener(_OnDecreaseNumber);
            }
            m_btnMinus = null;

            if (m_btnMax != null)
            {
                m_btnMax.SafeRemoveOnClickListener(_OnMaxNumber);
            }
            m_btnMax = null;

            if (m_btnStore != null)
            {
                m_btnStore.SafeRemoveOnClickListener(_OnStore);
            }
            m_btnStore = null;

            if (m_btnTake != null)
            {
                m_btnTake.SafeRemoveOnClickListener(_OnTake);
            }
            m_btnTake = null;

            m_grayAdd = null;
            m_grayMinus = null;
        }

        protected override void _OnCloseFrame()
        {
            m_comItem = null;
            m_itemData = null;
            m_count = 0;
        }

        public void StoreItem(ItemData data)
        {
            if (data != null)
            {
                m_itemData = data;
                m_count = m_itemData.Count;
            }
            else
            {
                Logger.LogError("item data is null!");
            }

            m_btnStore.CustomActive(true);
            m_btnTake.CustomActive(false);
            _Update();
        }

        public void TakeItem(ItemData data)
        {
            if (data != null)
            {
                m_itemData = data;
                m_count = m_itemData.Count;
            }
            else
            {
                Logger.LogError("item data is null!");
            }

            m_btnStore.CustomActive(false);
            m_btnTake.CustomActive(true);
            _Update();
        }

        protected void _Update()
        {
            if (m_comItem == null)
            {
                m_comItem = CreateComItem(Utility.FindGameObject(frame, "ItemInfo/PackageItem"));
            }
            if (m_itemData == null)
            {
                m_editCount.text = "0";
                m_comItem.Setup(null, null);
                m_itemName.text = "";
            }
            else
            {
                m_editCount.text = m_count.ToString();
                m_comItem.Setup(m_itemData, null);
                m_itemName.text = m_itemData.GetColorName();
            }

            if (m_grayAdd != null)
            {
                m_grayAdd.SetEnable(m_itemData != null && m_count >= m_itemData.Count);
            }
            if (m_grayMinus != null)
            {
                m_grayMinus.SetEnable(m_count <= 1);
            }
        }

        protected void _OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        //[UIEventHandle("NumberArea/Increase")]
        protected void _OnIncreaseNumber()
        {
            int count = int.Parse(m_editCount.text);
            count++;
            if (count >= 1 && count <= m_itemData.Count)
            {
                m_editCount.text = count.ToString();
            }
        }

        //[UIEventHandle("NumberArea/Decrease")]
        protected void _OnDecreaseNumber()
        {
            int count = int.Parse(m_editCount.text);
            count--;
            if (count >= 1 && count <= m_itemData.Count)
            {
                m_editCount.text = count.ToString();
            }
        }

        //[UIEventHandle("NumberArea/MaxNum")]
        protected void _OnMaxNumber()
        {
            m_editCount.text = m_itemData.Count.ToString();
        }

        //[UIEventHandle("store")]
        protected void _OnStore()
        {
            if (m_itemData != null)
            {
                if (m_count >= 1 && m_count <= m_itemData.Count)
                {
                    StorageDataManager.GetInstance().OnSendStoreItemReq(m_itemData, m_count);
                    frameMgr.CloseFrame(this);
                    ItemTipManager.GetInstance().CloseAll();
                }
            }
        }

        //[UIEventHandle("take")]
        protected void _OnTake()
        {
            if (m_itemData != null)
            {
                if (m_count >= 1 && m_count <= m_itemData.Count)
                {
                    ItemDataManager.GetInstance().TakeItem(m_itemData, m_count);
                    frameMgr.CloseFrame(this);
                    ItemTipManager.GetInstance().CloseAll();
                }
            }
        }

        protected void _OnValueChanged(string value)
        {
            if (m_itemData == null)
            {
                return;
            }

            m_count = 0;
            try
            {
                m_count = int.Parse(value);
            }
            catch (Exception e)
            {

            }
            
            if (m_count < 1)
            {
                m_count = 1;
            }
            if (m_count > m_itemData.Count)
            {
                m_count = m_itemData.Count;
            }
            _Update();
        }
    }
}
