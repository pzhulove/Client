using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComApplyPupilInfo : MonoBehaviour
    {
        public Text Name;
        public Image jobIcon;
        public Button btnRefuse;
        public UIGray grayRefuse;
        public Button btnAccept;
        public UIGray grayAccept;
        public Text Level;
        public Text jobText;
        public Text region;
        public Text information;
        public ComPlayerVipLevelShow vipLv;

        RelationData data = null;

        void _OnRefusePupilApply(UIEvent uiEvent)
        {
            if(null != data && data.uid == (ulong)uiEvent.Param1)
            {
                OnClickRefuse();
            }
        }

        public void OnClickRefuse()
        {
            if(null != data)
            {
                if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
                {
                    RelationDataManager.GetInstance().RefuseApplyPupils(new ulong[]{data.uid});
                    RelationDataManager.GetInstance().RemoveApplyPupil(data.uid);    
                }
                else
                {
                    RelationDataManager.GetInstance().RefuseApplyTeachers(new ulong[]{data.uid});
                    RelationDataManager.GetInstance().RemoveApplyTeacher(data.uid);
                }
                
            }
        }

        public void OnClickAccept()
        {
            if(null != data)
            {
                if(TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
                {
                    RelationDataManager.GetInstance().AcceptApplyPupils(data.uid);
                    RelationDataManager.GetInstance().RemoveApplyPupil(data.uid);
                }
                else
                {
                    RelationDataManager.GetInstance()._AcceptApplyTeachers(data.uid);
                    RelationDataManager.GetInstance().RemoveApplyTeacher(data.uid);
                }
            }
        }

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

                        if (null != jobText)
                        {
                            jobText.text = jobItem.Name;
                        }
                    }
                    // jobIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref jobIcon, path);
                }
                
                if(null != Level)
                {
                    Level.text = data.level.ToString();
                }

                if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
                {
                    information.text = string.Format("宣言:{0}", TR.Value("tap_teacher_region")); ;
                }
                else
                {
                    information.text = string.Format("宣言:{0}", TR.Value("tap_pupil_region"));
                }
                if (null != region)
                {
                    var regionData = TableManager.GetInstance().GetTableItem<AreaProvinceTable>(data.regionId);
                    if(regionData != null)
                    {
                        region.text = regionData.Name;
                    }
                }

                if(null != information)
                {
                    if(data.declaration != null && data.declaration != "")
                    {
                        information.text = string.Format("宣言:{0}", data.declaration);
                    }
                }

                if (null != vipLv)
                {
                    vipLv.SetVipLevel(data.vipLv);
                }
            }
        }

        public void OnPopupMenu()
        {
            if (null != data)
            {
                RelationMenuData menuData = new RelationMenuData();
                menuData.m_data = data;
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_PUPIL_APPLY;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShowPupilApplyMenu, menuData);
            }
        }
    }
}