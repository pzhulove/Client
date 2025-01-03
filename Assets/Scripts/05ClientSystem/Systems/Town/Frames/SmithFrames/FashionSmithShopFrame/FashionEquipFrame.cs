using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class FashionEquipFrameData
    {
        public FashionType eFashionType = FashionType.FT_SKY;
        public int SuitID = 1;
        public int Occu = 10;
    }

    class FashionEquipFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionEquipFrame";
        }

        [UIControl("", typeof(ComFashionEquipDataBinder))]
        ComFashionEquipDataBinder comDataBinder;

        FashionEquipFrameData data = null;
        bool mMask = false;

        protected override void _OnOpenFrame()
        {
            data = userData as FashionEquipFrameData;
            if(null != comDataBinder)
            {
                comDataBinder.SetSuit(data.eFashionType, data.Occu, data.SuitID);
            }

            _AddButton("OK", () => { if(!mMask) frameMgr.CloseFrame(this); });
            _AddButton("BtnEquipAll", () => 
            {
                _EquipAllFashions();
                frameMgr.CloseFrame(this);
            });
        }

        protected override void _OnCloseFrame()
        {

        }

        void _EquipAllFashions()
        {
            int iMask = 0;
            var ids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Fashion);
            for(int i = 0; i < ids.Count; ++i)
            {
                var item = ItemDataManager.GetInstance().GetItem(ids[i]);
                if(null == item)
                {
                    continue;
                }

                var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(item.TableID);
                if(null == skyItem)
                {
                    continue;
                }

                if(skyItem.Occu != data.Occu / 10)
                {
                    continue;
                }

                if(skyItem.SuitID != data.SuitID)
                {
                    continue;
                }

                if(skyItem.Type != (int)data.eFashionType)
                {
                    continue;
                }

                int tarMask = (1 << (int)item.SubType);

                if ((iMask & tarMask) == tarMask)
                {
                    continue;
                }

                iMask |= tarMask;

                ItemDataManager.GetInstance().UseItem(item);
            }
        }
    }
}