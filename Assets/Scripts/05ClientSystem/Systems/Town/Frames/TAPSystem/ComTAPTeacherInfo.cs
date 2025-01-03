using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComTAPTeacherInfo : MonoBehaviour
    {
        RelationData data = null;
        public Text Name;
        public Image jobIcon;
        public Text announcement;
        public Button btnAskTeacher;
        public UIGray grayAskTeacher;
        public Text Level;
        public GameObject goAllocated;
        public void OnItemVisible(object value)
        {
            data = value as RelationData;
            if(null != data)
            {
                if(null != Name)
                {
                    Name.text = data.name;
                }
                var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(data.occu);

                if (null != jobIcon)
                {
                    string path = "";
                    if (null != jobItem)
                    {
                        ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                        if (resData != null)
                        {
                            path = resData.IconPath;
                        }
                    }
                    // jobIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref jobIcon, path);
                }

                if(null != announcement)
                {
                    if(!string.IsNullOrEmpty(data.announcement))
                    {
                        announcement.text = data.announcement;
                    }
                    else
                    {
                        announcement.text = TR.Value("tap_default_announcement");
                    }
                }

                _UpdateQueryStatus();

                if (null != Level)
                {
                    Level.text = data.level.ToString();
                }
            }
        }

        void _UpdateQueryStatus()
        {
            bool bCanQuery = TAPNewDataManager.GetInstance().CanQuery(data.uid);
            grayAskTeacher.enabled = !bCanQuery;
            btnAskTeacher.enabled = bCanQuery;
            goAllocated.CustomActive(!bCanQuery);
            btnAskTeacher.CustomActive(bCanQuery);
        }

        public void SendAskForTeacher()
        {
            if(null != data && btnAskTeacher.enabled)
            {
                TAPNewDataManager.GetInstance().SendApplyTeacher(data.uid);
                TAPNewDataManager.GetInstance().AddQueryInfo(data.uid);

                _UpdateQueryStatus();
            }
        }

        public void OnPopupMenu()
        {
            if (null != data)
            {
                RelationMenuData menuData = new RelationMenuData();
                menuData.m_data = this.data;
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_TEACHER;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShowAskTeacherMenu,menuData);
            }
        }

        void Start()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAskTeacherMsgSended, _OnAskTeacherMsgSended);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAskTeacherMsgSended, _OnAskTeacherMsgSended);
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAskTeacherMsgSended, _OnAskTeacherMsgSended);
        }

        void _OnAskTeacherMsgSended(UIEvent uiEvent)
        {
            if (null != data)
            {
                if ((ulong)uiEvent.Param1 == data.uid)
                {
                    if (null != data && btnAskTeacher.enabled)
                    {
                        grayAskTeacher.enabled = true;
                        btnAskTeacher.enabled = false;
                        btnAskTeacher.CustomActive(false);
                        goAllocated.CustomActive(true);
                    }
                }
            }
        }
    }
}