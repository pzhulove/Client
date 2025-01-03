using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;

namespace GameClient
{
    class ClassmateItem
    {
        const string ClassmateItemPath = "UIFlatten/Prefabs/TAP/ClassmateItem";
        private Image mIcon;
        private Text mName;
        private Text mOnline;
        private Text mLevel;
        private Button mMenu;
        private GameObject thisGameObject;
        private UIGray mGray;

        public GameObject ThisGameObject
        {
            get
            {
                return thisGameObject;
            }
            set
            {
                thisGameObject = value;
            }
        }
        public ClassmateItem(ClassmateRelationData relationData)
        {
            CreateGo(relationData);
        }
        private void CreateGo(ClassmateRelationData relationData)
        {
            GameObject ClassmateItem = AssetLoader.instance.LoadResAsGameObject(ClassmateItemPath);
            if (ClassmateItem == null)
            {
                return;
            }
            var mBind = ClassmateItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            mName = mBind.GetCom<Text>("Name");
            mIcon = mBind.GetCom<Image>("Icon");
            mOnline = mBind.GetCom<Text>("Online");
            mLevel = mBind.GetCom<Text>("Level");
            mMenu = mBind.GetCom<Button>("Menu");
            mGray = mBind.GetCom<UIGray>("Gray");
            //Name
            mName.text = relationData.name;

            //online
            

            mLevel.text = relationData.level.ToString();

            mMenu.onClick.RemoveAllListeners();
            mMenu.onClick.AddListener(() =>
            {
                RelationMenuData menuData = new RelationMenuData();
                RelationData tempRelationData = new RelationData();
                tempRelationData.uid = relationData.uid;
                tempRelationData.name = relationData.name;
                tempRelationData.level = relationData.level;
                tempRelationData.occu = relationData.occu;
                tempRelationData.type = 3;//默认为陌生人
                tempRelationData.vipLv = relationData.vipLv;
                tempRelationData.status = relationData.status;

                menuData.m_data = tempRelationData;
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_CLASSMATE;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShowPupilRealMenu, menuData);
            });
            //People Icon
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(relationData.occu);

            if (null != mIcon)
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
                ETCImageLoader.LoadSprite(ref mIcon, path);
            }
            ThisGameObject = ClassmateItem;

            mOnline.CustomActive(true);
            if (relationData.status == (byte)FriendMatchStatus.Idle)
            {
                mOnline.text = "<color=#11EE11FF>在线</color>";
                mGray.SetEnable(false);
            }
            else if (relationData.status == (byte)FriendMatchStatus.Busy)
            {
                mOnline.text = "<color=#E95137FF>战斗中</color>";
                mGray.SetEnable(false);
            }
            else if (relationData.status == (byte)FriendMatchStatus.Offlie)
            {
                mOnline.text = "<color=#99AABBFF>离线</color>";
                mGray.SetEnable(true);
            }
            else
            {
                mOnline.CustomActive(false);
                mGray.SetEnable(true);
            }
        }

        public void DestoryGo()
        {
            GameObject.Destroy(ThisGameObject);
        }
    }
}