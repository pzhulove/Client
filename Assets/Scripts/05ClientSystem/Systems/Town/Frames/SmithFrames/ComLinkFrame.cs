using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class ComLinkFrame : ClientFrame
    {
        public delegate void OnClick();
        public class ComLinkFrameData
        {
            public ProtoTable.ItemTable item;
            public bool bOrgLink;
            public ProtoTable.AcquiredMethodTable linkItem;
            public string title;
            public OnClick onClick;
            public bool bNotEnough;
            public string titleContent;
        }
        ComLinkFrameData data;
        Text m_kDesc;
        ComItem comItem;
        GameObject goParent;
        GameObject goPrefab;
        Text desc;
        [UIControl("back/BlankTitle/Name", typeof(Text))]
        Text comItemName;
        [UIControl("back/Scrollview/ViewPort", typeof(LayoutElement))]
        LayoutElement comViewPortLayoutElement;
        [UIControl("back/FixTitle", typeof(Text))]
        Text mTitleTxt;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/ComLinkFrame";
        }

        protected override void _OnOpenFrame()
        {
            data = userData as ComLinkFrameData;
            _Initialize();
        }

        void _Initialize()
        {
            m_kDesc = Utility.FindComponent<Text>(frame, "back/BlankDesc/Desc");
            Utility.FindComponent<Button>(frame, "backclose").onClick.AddListener(() => { frameMgr.CloseFrame(this); });
            comItem = CreateComItem(Utility.FindChild(frame, "back/BlankTitle/ItemParent"));

            if (data != null)
            {
                m_kDesc.CustomActive(data.bNotEnough);
                goParent = Utility.FindChild(frame, "back/Scrollview/ViewPort/Content");
                goPrefab = Utility.FindChild(goParent, "Item");
                GameObject goEquipPrefab = Utility.FindChild(goParent, "EquipItem");
                goPrefab.CustomActive(false);
                goEquipPrefab.CustomActive(false);
                m_akLinkItemObjects.RecycleAllObject();
                m_akEquipLinkItemObjects.RecycleAllObject();
                desc = Utility.FindComponent<Text>(frame, "back/Name");
                //set item icon and name
                if (data.item != null)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.item.ID);
                    comItem.Setup(itemData, null);
                    if (null != itemData)
                    {
                        comItemName.text = itemData.GetColorName();
                    }
                }
                else
                {
                    comItem.Setup(null, null);
                    comItemName.text = string.Empty;
                    if (null != data.linkItem)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.linkItem.ItemId);
                        if(null != itemData)
                        {
                            comItem.Setup(itemData, null);
                            comItemName.text = itemData.GetColorName();
                        }
                    }
                }

                if (data.item != null)
                {
                    bool isFind = EquipHandbookDataManager.GetInstance().EquipHandbookAttachedTableIsFindItemID(data.item.ID);
                    if (isFind)
                    {
                        int iSourceCount = 0;
                        var iter = ItemSourceInfoTableManager.GetInstance().GetSourceInfos(data.item.ID);
                        while (iter.MoveNext())
                        {
                            ISourceInfo info = iter.Current as ISourceInfo;

                            if (null == info)
                            {
                                continue;
                            }

                            m_akEquipLinkItemObjects.Create(iSourceCount, new object[] { goParent, goEquipPrefab, info, this });

                            iSourceCount++;
                        }
                    }
                    else
                    {
                        CreatLinkItemObj();
                    }
                }
                else
                {
                    CreatLinkItemObj();
                }

                if(!string.IsNullOrEmpty(data.titleContent))
                {
                    mTitleTxt.SafeSetText(data.titleContent);
                }
            }
            else
            {
                int iCount = 1;
                float iDeltaYMax = (iCount - 1) * 14.0f + iCount * 150.0f;
                //comViewPortLayoutElement.preferredHeight = iDeltaYMax;
            }
        }

        void CreatLinkItemObj()
        {
            // create item links
            List<object> linkItems = GamePool.ListPool<object>.Get();
            try
            {
                if (data.bOrgLink && data.item != null)
                {
                    desc.text = data.title;
                    for (int i = 0; i < data.item.ComeLink.Count; ++i)
                    {
                        var linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.AcquiredMethodTable>(data.item.ComeLink[i]);
                        if (linkItem != null)
                        {
                            linkItems.Add(linkItem);
                            //m_akLinkItemObjects.Create(linkItem.ID, new object[] { goParent, goPrefab, linkItem, this ,data.onClick});
                        }
                        else
                        {
                            Logger.LogErrorFormat("can not find linkItem with id = {0} in table AcquiredMethodTable , to HXL !!!", data.item.ComeLink[i]);
                        }
                    }
                }
                else if (!data.bOrgLink && data.linkItem != null)
                {
                    desc.text = data.title;
                    for (int i = 0; i < data.linkItem.ReLinks.Count; ++i)
                    {
                        var linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.AcquiredMethodTable>(data.linkItem.ReLinks[i]);
                        if (linkItem != null)
                        {
                            linkItems.Add(linkItem);
                            //m_akLinkItemObjects.Create(linkItem.ID, new object[] { goParent, goPrefab, linkItem, this , data.onClick });
                        }
                        else
                        {
                            Logger.LogErrorFormat("relinkerror id = {0} belong to id is {1}", data.linkItem.ReLinks[i], data.linkItem.ID);
                        }
                    }
                }

                int iCount = IntMath.Min(linkItems.Count, 4);
                if (iCount == 0)
                {
                    iCount = 1;
                }
                float iDeltaYMax = 580.0f;
                if (iCount != 4)
                {
                    iDeltaYMax = (iCount - 1) * 14.0f + iCount * 150.0f;
                }
                //comViewPortLayoutElement.preferredHeight = iDeltaYMax;

                for (int i = 0; i < linkItems.Count; ++i)
                {
                    ProtoTable.AcquiredMethodTable linkItem = linkItems[i] as ProtoTable.AcquiredMethodTable;
                    m_akLinkItemObjects.Create(linkItem.ID, new object[] { goParent, goPrefab, linkItem, this, data.onClick });
                }
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat(e.ToString());
            }
            GamePool.ListPool<object>.Release(linkItems);
        }

        protected sealed override void _OnCloseFrame()
        {
            if(data.onClick != null)
            {
                data.onClick = null;
            }
            data = null;
            m_akLinkItemObjects.DestroyAllObjects();
            m_akEquipLinkItemObjects.DestroyAllObjects();
        }

        class EquipLinkItemObject : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            ISourceInfo info;
            ComLinkFrame THIS;
            Text Name;
            Button Button;
            string linkInfo;
            string linkString;
            public sealed override void OnDestroy()
            {
                Button.onClick.RemoveAllListeners();
                Button = null;
                THIS = null;
                info = null;
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                info = param[2] as ISourceInfo;
                THIS = param[3] as ComLinkFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    ComCommonBind mBind = goLocal.GetComponent<ComCommonBind>();
                    if (mBind != null)
                    {
                        Name = mBind.GetCom<Text>("Name");
                        Button = mBind.GetCom<Button>("Button");
                        Button.onClick.RemoveAllListeners();
                        Button.onClick.AddListener(() => 
                        {
                            if (ItemSourceInfoUtility.IsLinkFunctionOpen(info))
                            {
                                ActiveManager.GetInstance().OnClickLinkInfo(linkString);
                                THIS.frameMgr.CloseFrame(THIS);
                            }
                            else
                            {
                                SystemNotifyManager.SystemNotify(1013);
                            }
                        });
                    }

                    linkInfo = ItemSourceInfoTableManager.GetInstance().GetSourceInfoName(info);
                    linkString = ItemSourceInfoTableManager.GetInstance().GetSourceInfoLink(info);
                }
                Enable();
                SetAsLastSibling();
                _Update();
            }

            public sealed override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            public sealed override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public sealed override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public sealed override void OnRefresh(object[] param)
            {
                _Update();
            }
            public sealed override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public sealed override bool NeedFilter(object[] param)
            {
                return false;
            }

            void _Update()
            {
                Name.text = linkInfo;
            }
        }

        class LinkItemObject : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            ProtoTable.AcquiredMethodTable linkItem;
            ComLinkFrame THIS;
            OnClick onClick;

            Text Name;
            Text LinkZone;
            Button Button;
            UIGray gray;
            Text Desc;
            Text Probility;
            Text UnLockHint;
            GameObject goContent;

            public sealed override void OnDestroy()
            {
                Button.onClick.RemoveAllListeners();
                Button = null;
                onClick = null;
                linkItem = null;
                THIS = null;
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                linkItem = param[2] as ProtoTable.AcquiredMethodTable;
                THIS = param[3] as ComLinkFrame;
                onClick = param[4] as OnClick;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    Name = Utility.FindComponent<Text>(goLocal, "Name");
                    LinkZone = Utility.FindComponent<Text>(goLocal, "LinkZone");
                    Button = Utility.FindComponent<Button>(goLocal, "Button");
                    Desc = Utility.FindComponent<Text>(goLocal, "Button/Text");
                    //Probility = Utility.FindComponent<Text>(goLocal, "AcquireProbility");
                    //UnLockHint = Utility.FindComponent<Text>(goLocal, "Button/UnlockHint");

                    Button.onClick.RemoveAllListeners();
                    Button.CustomActive(true);
                    Button.onClick.RemoveAllListeners();
                    Button.onClick.AddListener(OnClick);

                    gray = Utility.FindComponent<UIGray>(goLocal, "Button");
                }
                Enable();
                SetAsLastSibling();
                _Update();
            }
            public sealed override void SetAsLastSibling()
            {
                if(goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }
            public sealed override void OnRecycle()
            {
                if(onClick != null)
                {
                    onClick = null;
                }
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public sealed override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public sealed override void OnRefresh(object[] param)
            {
                _Update();
            }
            public sealed override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public sealed override bool NeedFilter(object[] param)
            {
                return false;
            }

            public bool IsOpened()
            {
                if(linkItem == null)
                {
                    return false;
                }

                if (!Utility.IsFunctionCanUnlock((ProtoTable.FunctionUnLock.eFuncType)linkItem.FuncitonID))
                {
                    return false;
                }

                return true;
            }

            void _Update()
            {
                if (linkItem != null)
                {
                    bool bIsOpened = IsOpened();
                    Name.text = linkItem.Name;
                    Desc.text = linkItem.ActionDesc;
                    LinkZone.text = linkItem.LinkZone;
                    Button.enabled = linkItem.IsLink != 0 && bIsOpened;
                    Button.CustomActive(linkItem.IsLink != 0 && bIsOpened);
                    gray.enabled = !bIsOpened;
                    //Probility.text = linkItem.ProbilityDesc;
                    //UnLockHint.text = TR.Value("ComLinkUnlockHint", Utility.GetFuncUnlockLevel((ProtoTable.FunctionUnLock.eFuncType)linkItem.FuncitonID));
                    //goContent.CustomActive(bIsOpened && linkItem.IsLink != 0);
                    //UnLockHint.CustomActive(!bIsOpened);
                }
            }

            void OnClick()
            {
                if (linkItem != null && linkItem.IsLink != 0)
                {
                    // 这里做个特殊处理
                    // 如果是通行证相关的链接，如果通信证活动没有开启或者玩家没有解锁，则提示玩家
                    if(linkItem.FuncitonID == (int)ProtoTable.FunctionUnLock.eFuncType.AdventurePassSeason)
                    {
                        if(AdventurerPassCardDataManager.GetInstance().CardLv == 0)
                        {
                            bool locked = false;
                            bool seasonNotOpen = false;
                            if(AdventurerPassCardDataManager.GetInstance().SeasonID > 0) // 如果赛季id有效，说明赛季已经开启
                            {
                                locked = true;
                            }
                            else
                            {
                                seasonNotOpen = true;
                            }
                            if (locked)
                            {
                                ProtoTable.FunctionUnLock functionUnLock = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.AdventurePassSeason);
                                if (functionUnLock != null)
                                {
                                    SystemNotifyManager.SystemNotify(functionUnLock.CommDescID);
                                }
                            }
                            else if (seasonNotOpen)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("adventurer_pass_card_not_open_tip"));
                            }
                            return;
                        }
                    }
                    if(linkItem.ID== 283)
                    {
                        ActiveManager.GetInstance().OnClickLinkInfo(linkItem.LinkInfo,null,true);
                    }
                    else
                    {
                        ActiveManager.GetInstance().OnClickLinkInfo(linkItem.LinkInfo);
                    }
                    
                    THIS.frameMgr.CloseFrame(THIS);
                    if(onClick != null)
                    {
                        onClick.Invoke();
                    }

                    if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                    }
                }
            }
        }

        CachedObjectDicManager<int, LinkItemObject> m_akLinkItemObjects = new CachedObjectDicManager<int, LinkItemObject>();
        CachedObjectDicManager<int, EquipLinkItemObject> m_akEquipLinkItemObjects = new CachedObjectDicManager<int, EquipLinkItemObject>();
    }
}