using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRelationInfoList
    {
        bool bInitialize = false;
        public bool Initilized
        {
            get
            {
                return bInitialize;
            }
        }
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        public delegate void OnItemSelected(RelationData relationData);
        public OnItemSelected onItemSelected;
        List<RelationData> relationDatas = new List<RelationData>();
        public RelationTabType eRelationTabType = RelationTabType.RTT_RECENTLY;
        RelationData currentRelationData = null;
        ScrollRect mScrollRect;
        public RelationData Selected
        {
            get
            {
                return ComRelationInfo.ms_selected;
            }
        }

        public List<RelationData> GetRelationDatas()
        {
            return relationDatas;
        }

        public void Initialize(ClientFrame clientFrame,
            GameObject gameObject,
            OnItemSelected onItemSelected,
            RelationTabType eRaltionTabType,RelationData currentRelationData)
        {
            if (bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.onItemSelected += onItemSelected;
            this.eRelationTabType = eRaltionTabType;
            this.currentRelationData = currentRelationData;
            GameObject scrollList = gameObject.GetComponent<ComCommonBind>().GetGameObject("go"); 
            comUIListScript = scrollList.GetComponent<ComUIListScript>();
            if (comUIListScript != null)
            {
                comUIListScript.Initialize();
                comUIListScript.onBindItem += _OnBindItemDelegate;
                comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
                comUIListScript.onItemSelected += _OnItemSelectedDelegate;
                comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;
                mScrollRect = comUIListScript.GetComponent<ScrollRect>();    
            }
            
            _LoadAllRelations(ref relationDatas);
            if (relationDatas.Count > 0)
            {
                _TrySetDefaultEquipment();
            }
        }

        ComRelationInfo _OnBindItemDelegate(GameObject itemObiect)
        {
            ComRelationInfo comRelationInfo = itemObiect.GetComponent<ComRelationInfo>();
            if (comRelationInfo != null)
            {
                return comRelationInfo;
            }

            return null;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item != null)
            {
                var current = item.gameObjectBindScript as ComRelationInfo;
                if (current != null)
                {
                    if (this.eRelationTabType == RelationTabType.RTT_FRIEND)
                    {
                        if (item.m_index >= 0 && item.m_index < relationDatas.Count)
                        {
                            current.OnItemVisible(relationDatas[item.m_index], this.eRelationTabType);
                        }
                    }
                    else if (this.eRelationTabType == RelationTabType.RTT_RECENTLY)
                    {
                        if (item.m_index >= 0 && item.m_index < relationDatas.Count)
                        {
                            current.OnItemVisible(relationDatas[item.m_index], this.eRelationTabType);
                        }
                    }
                    else if (this.eRelationTabType == RelationTabType.RTT_BLACK)
                    {
                        if (item.m_index >= 0 && item.m_index < relationDatas.Count)
                        {
                            current.OnItemVisible(relationDatas[item.m_index], this.eRelationTabType);
                        }
                    }
                }

                if (ComRelationInfo.ms_selected == current.RelationData)
                {
                    current.OnItemChangeDisplay(true);
                }
                else
                {
                    current.OnItemChangeDisplay(false);
                }
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComRelationInfo;
            ComRelationInfo.ms_selected = (current == null) ? null : current.RelationData;

            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComRelationInfo.ms_selected);
            }

        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComRelationInfo;
            
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        public void RefreshAllRelations(RelationTabType eRelationTabType)
        {
            //if (this.eRelationTabType == eRelationTabType)
            //{
            //    return;
            //}
            this.eRelationTabType = eRelationTabType;
            _LoadAllRelations(ref relationDatas);

            if (relationDatas.Count > 0)
            {
                _TrySetDefaultEquipment();
            }

            if (mScrollRect != null)
            {
                if (mScrollRect.verticalNormalizedPosition <= 0.04f)
                {
                    mScrollRect.verticalNormalizedPosition = 0;
                }
            }
        }
        
        void _LoadAllRelations(ref List<RelationData> kRelationDtatas)
        {
            kRelationDtatas.Clear();
            List<Vector2> size = new List<Vector2>();

            if (this.eRelationTabType == RelationTabType.RTT_RECENTLY)
            {
                List<PrivateChatPlayerData> list = new List<PrivateChatPlayerData>();
                var srclist = RelationDataManager.GetInstance().GetPriChatList();
                list.AddRange(srclist);
                list.Sort((x, y) =>
                {
                    if (x.relationData.isOnline != y.relationData.isOnline)
                    {
                        return x.relationData.isOnline == 1 ? -1 : 1;
                    }

                    if (x.relationData.status != y.relationData.status)
                    {
                        return x.relationData.status < y.relationData.status ? -1 : 1;
                    }

                    if (x.chatNum != y.chatNum)
                    {
                        return y.chatNum < x.chatNum ? -1 : 1;
                    }

                    if (x.iOrder != y.iOrder)
                    {
                        return y.iOrder - x.iOrder;
                    }

                    return x.relationData.uid < y.relationData.uid ? -1 : (x.relationData.uid == y.relationData.uid ? 0 : 1);
                });

                kRelationDtatas = new List<RelationData>();

                for (int i = 0; i < list.Count; ++i)
                {
                    kRelationDtatas.Add(list[i].relationData);
                }
            }
            else if (this.eRelationTabType == RelationTabType.RTT_FRIEND)
            {
                var datas_friends = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_FRIEND);
                var datas_teachers = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                var datas_pupils = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                kRelationDtatas = new List<RelationData>();
                kRelationDtatas.AddRange(datas_teachers);
                kRelationDtatas.AddRange(datas_pupils);
                kRelationDtatas.AddRange(datas_friends);
                kRelationDtatas.Sort(_SortMyFriend);
            }
            else if (this.eRelationTabType == RelationTabType.RTT_BLACK)
            {
                var datas_blacks = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_BLACKLIST);
                kRelationDtatas = new List<RelationData>();
                kRelationDtatas.AddRange(datas_blacks);
            }

            for (int i = 0; i < kRelationDtatas.Count; i++)
            {
                size.Add(new Vector2(806, 186));
            }

            comUIListScript.SetElementAmount(kRelationDtatas.Count, size);
        }

        int _SortMyFriend(RelationData x, RelationData y)
        {
            if (x.isOnline != y.isOnline)
            {
                return x.isOnline == 1 ? -1 : 1;
            }

            if (x.status != y.status)
            {
                return x.status < y.status ? -1 : 1;
            }

            if (x.intimacy != y.intimacy)
            {
                return y.intimacy < x.intimacy ? -1 : 1;
            }

            if (x.level != y.level)
            {
                return y.level - x.level;
            }

            if (x.seasonLv != y.seasonLv)
            {
                return y.seasonLv < x.seasonLv ? -1 : 1;
            }

            return x.uid < y.uid ? -1 : 1;
        }

        void _TrySetDefaultEquipment()
        {
            if (ComRelationInfo.ms_selected != null)
            {
                var find = relationDatas.Find(x =>
                {
                    return x.uid == ComRelationInfo.ms_selected.uid;
                });

                if (find != null)
                {
                    ComRelationInfo.ms_selected = find;
                }
                else
                {
                    ComRelationInfo.ms_selected = null;
                }
            }

            int iBindIndex = -1;
            if (ComRelationInfo.ms_selected != null)
            {
                for (int i = 0; i < relationDatas.Count; ++i)
                {
                    if (relationDatas[i].uid == ComRelationInfo.ms_selected.uid)
                    {
                        iBindIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if (relationDatas.Count > 0)
                {
                    if (this.currentRelationData != null)
                    {
                        for (int i = 0; i < relationDatas.Count; i++)
                        {
                            if (relationDatas[i].uid == this.currentRelationData.uid)
                            {
                                iBindIndex = i;
                                break;
                            }
                        }
                    }
                }
            }

            //if (eRelationTabType == RelationTabType.RTT_BLACK)
            //{
            //    if (relationDatas.Count > 0)
            //    {
            //        iBindIndex = 0;
            //    }
            //}

            _SetSelectedRelationItem(iBindIndex);
        }

        void _SetSelectedRelationItem(int iBindIndex)
        {
            if (iBindIndex >= 0 && iBindIndex < relationDatas.Count)
            {
                if (ComRelationInfo.ms_selected == relationDatas[iBindIndex])
                {
                    return;
                }

                ComRelationInfo.ms_selected = relationDatas[iBindIndex];
                if (!comUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    comUIListScript.EnsureElementVisable(iBindIndex);
                }
                comUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                comUIListScript.SelectElement(-1);
                ComRelationInfo.ms_selected = null;
            }

            if (onItemSelected != null)
            {
                onItemSelected.Invoke(ComRelationInfo.ms_selected);
            }
        }

        public void UnInitialize()
        {
            if (comUIListScript != null)
            {
                comUIListScript.onBindItem -= _OnBindItemDelegate;
                comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                comUIListScript = null;
            }
            this.eRelationTabType = RelationTabType.RTT_RECENTLY;
            this.onItemSelected -= onItemSelected;
            this.gameObject = null;
            this.clientFrame = null;
            this.relationDatas.Clear();
            ComRelationInfo.ms_selected = null;
            bInitialize = false;
        }
    }
}

