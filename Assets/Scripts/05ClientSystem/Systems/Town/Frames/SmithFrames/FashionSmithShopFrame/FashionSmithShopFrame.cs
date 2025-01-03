using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class FashionSmithShopFrameData
    {
        public FashionSmithShopFrame.FunctionType eFunction = FashionSmithShopFrame.FunctionType.FT_MODIFY;
        public ulong linkGUID;
    }

    class FashionSmithShopFrame : ClientFrame
    {
        public static void CommandOpen(FashionSmithShopFrameData data = null)
        {
            if (null == data)
            {
                data = new FashionSmithShopFrameData();
            }
            ClientSystemManager.GetInstance().OpenFrame<FashionSmithShopFrame>(FrameLayer.Middle, data);
        }

        public static void OpenLinkFrame(string strParam)
        {
            var tokens = strParam.Split(new char[] { '_' });
            if(tokens.Length != 2)
            {
                return;
            }

            int iTab = 0;
            if(!int.TryParse(tokens[0], out iTab))
            {
                return;
            }

            ulong guid = 0;
            if(!ulong.TryParse(tokens[1],out guid))
            {
                return;
            }

            FashionSmithShopFrameData data = new FashionSmithShopFrameData
            {
                eFunction = (FunctionType)iTab,
                linkGUID = guid,
            };

            ClientSystemManager.GetInstance().OpenFrame<FashionSmithShopFrame>(FrameLayer.Middle,data);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionSmithshop";
        }

        FashionSmithShopFrameData data = null;
        protected override void _OnOpenFrame()
        {
            data = (userData as FashionSmithShopFrameData);
            if(data == null)
            {
                data = new FashionSmithShopFrameData { eFunction = FunctionType.FT_MODIFY };
            }
            else
            {
                data.eFunction = FunctionType.FT_MODIFY;
            }

            _InitFunctionTabs();
        }

        protected override void _OnCloseFrame()
        {
            FunctionTab.Clear();
            m_akFunctionTabs.DestroyAllObjects();
        }

        [UIObject("VerticalFilter")]
        GameObject goTabParent;
        [UIObject("VerticalFilter/Filter")]
        GameObject goTabPrefab;
        CachedObjectListManager<FunctionTab> m_akFunctionTabs = new CachedObjectListManager<FunctionTab>();

        [UIObject("ChildFrameParent")]
        GameObject goChildParent;

        [UIEventHandle("ComWnd/Title/Close")]
        void _OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        void _InitFunctionTabs()
        {
            goTabPrefab.CustomActive(false);

            for(int i = 0; i < (int)FunctionType.FT_COUNT; ++i)
            {
                if (_IsFunctionOpen((FunctionType)i))
                {
                    m_akFunctionTabs.Create(new object[] {
                    goTabParent,
                    goTabPrefab,
                    new FunctionTabData { eFunctionType = (FunctionType)i },
                    System.Delegate.CreateDelegate(typeof(FunctionTab.OnSelectedDelegate), this, "_OnFunctionChanged")});
                }
            }

            var find = m_akFunctionTabs.Find(x =>
            {
                return x.Value.eFunctionType == data.eFunction;
            });

            if(find == null && m_akFunctionTabs.ActiveObjects.Count > 0)
            {
                find = m_akFunctionTabs.ActiveObjects[0];
                data.eFunction = find.Value.eFunctionType;
            }

            if(find != null)
            {
                find.OnSelected();
            }
        }

        bool _IsFunctionOpen(FunctionType eFunction)
        {
            int[] aiOpenID = new int[(int)FunctionType.FT_COUNT] { (int)ProtoTable.FunctionUnLock.eFuncType.FashionAttrSel };

            if (eFunction >= 0 && eFunction < FunctionType.FT_COUNT)
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(aiOpenID[(int)eFunction]);
                if (FuncUnlockdata != null)
                {
                    return FuncUnlockdata.FinishLevel <= PlayerBaseData.GetInstance().Level;
                }
            }

            return false;
        }

        void _OnFunctionChanged(FunctionTabData value)
        {
            if(value.frame == null)
            {
                if(value.eFunctionType == FunctionType.FT_MODIFY)
                {
                    value.frame = frameMgr.OpenFrame<FashionAttributesModifyFrame>(goChildParent,data.linkGUID);
                }
            }

            m_akFunctionTabs.ActiveObjects.ForEach(x =>
            {
                if(null != x && null != x.Value && null != x.Value.frame)
                {
                    x.Value.frame.Show(value.eFunctionType == x.Value.eFunctionType);
                }
            });
        }

        public enum FunctionType
        {
            [System.ComponentModel.Description("fashion_smith_modify")]
            FT_MODIFY = 0,
            FT_COUNT,
        }

        class FunctionTabData
        {
            public FunctionType eFunctionType = FunctionType.FT_COUNT;
            public IClientFrame frame = null;
        }

        class FunctionTab : CachedSelectedObject<FunctionTabData,FunctionTab>
        {
            GameObject goCheckMark;
            Text labelName;
            Text checkLabelName;

            public override void Initialize()
            {
                goCheckMark = Utility.FindChild(goLocal, "CheckMark");
                labelName = Utility.FindComponent<Text>(goLocal, "Text");
                checkLabelName = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
            }

            public override void OnUpdate()
            {
                labelName.text = checkLabelName.text = TR.Value(Utility.GetEnumDescription(Value.eFunctionType));
            }

            public override void UnInitialize()
            {
                if(Value.frame != null)
                {
                    Value.frame.Close(true);
                    Value.frame = null;
                }
            }

            public override void OnDisplayChanged(bool bShow)
            {
                goCheckMark.CustomActive(bShow);
            }
        }
    }
}