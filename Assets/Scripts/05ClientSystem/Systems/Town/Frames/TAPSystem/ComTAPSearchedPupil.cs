using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComTAPSearchedPupil : MonoBehaviour
    {
        public Text Name;
        public Image jobIcon;
        public Text Level;
        public StateController comState = null;
        RelationData data = null;

        public void AskForPupil()
        {
            if(null != data)
            {
                if(TAPNewDataManager.GetInstance().CheckApplyPupil(true))
                {
                    TAPNewDataManager.GetInstance().SendApplyPupil(data.uid);
                    TAPNewDataManager.GetInstance().AddApplyedPupil(data.uid);
                    _UpdateStatus();
                }
            }
        }

        void _UpdateStatus()
        {
            if (null != comState)
            {
                if (TAPNewDataManager.GetInstance().HasPupil(data.uid))
                {
                    comState.Key = "hasBeenTap";
                }
                else
                {
                    if (!TAPNewDataManager.GetInstance().CanApplyedPupil(data.uid))
                    {
                        var find = RelationDataManager.GetInstance().ApplyPupils.Find(x => { return x.uid == data.uid; });
                        if (null == find)
                        {
                            comState.Key = "applyed";
                        }
                        else
                        {
                            comState.Key = "applyed_allocated";
                        }
                    }
                    else
                    {
                        var find = RelationDataManager.GetInstance().ApplyPupils.Find(x => { return x.uid == data.uid; });
                        if (null != find)
                        {
                            comState.Key = "allocated";
                        }
                        else
                        {
                            comState.Key = "normal";
                        }
                    }
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

        public void OnItemVisible(object value)
        {
            data = value as RelationData;
            if (null != data)
            {
                if (null != Name)
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

                if(null != Level)
                {
                    Level.text = data.level.ToString();
                }

                _UpdateStatus();
            }
        }
    }
}