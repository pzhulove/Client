using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;
using System.Reflection;

namespace GameClient
{
    class FashionAttributeSelectManager : DataManager<FashionAttributeSelectManager>
    {
        #region process
        public override void Initialize()
        {
        }

        public override void Clear()
        {

        }
        #endregion

        public int CostItemID
        {
            get
            {
                var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_FASHION_ATTRIBUTE_RESET_COST_ITEM_ID);
                if(systemValueItem != null)
                {
                    return systemValueItem.Value;
                }
                return 0;
            }
        }

        public ProtoTable.ItemTable CostItem
        {
            get
            {
                return TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(CostItemID);
            }
        }

        public class FashionAttrItemContent
        {
            public enum FashionAttrItemContentType
            {
                FICT_INVALID = -1,
                FICT_BASE_ATTR = 0,
                FICT_BUFFER_ID,
            }
            public FashionAttrItemContentType eType;
            public EEquipProp eProp;
            public int iValue;
        }

        public FashionAttrItemContent GetAttributeItemContent(int iID)
        {
            var equipAttriItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(iID);
            if (equipAttriItem == null)
            {
                return null;
            }

            if(equipAttriItem.AttachBuffInfoIDs.Count > 0 && equipAttriItem.AttachBuffInfoIDs[0] != 0)
            {
                var bufferInfoItem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(equipAttriItem.AttachBuffInfoIDs[0]);
                if(null == bufferInfoItem)
                {
                    Logger.LogErrorFormat("attach buffer info is error with bufferinfo id {0}", equipAttriItem.AttachBuffInfoIDs[0]);
                    return null;
                }

                return new FashionAttrItemContent
                {
                     eType = FashionAttrItemContent.FashionAttrItemContentType.FICT_BUFFER_ID,
                    iValue = equipAttriItem.AttachBuffInfoIDs[0],
                };
            }

            var type = typeof(ProtoTable.EquipAttrTable);
            BindingFlags flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
            var fields = type.GetProperties(flag);
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];
                if (field == null || string.Equals(field.Name, "ID") /*|| !field.Name.StartsWith("_")*/)
                {
                    continue;
                }

				if (typeof(int) != field.PropertyType && typeof(CrypticInt32) != field.PropertyType)
                {
                    continue;
                }

                int iCurValue = 0;

                if (field.PropertyType == typeof(int))
                    iCurValue = (int)field.GetValue(equipAttriItem, null);
                if (typeof(CrypticInt32) == field.PropertyType)
                    iCurValue = (int)(CrypticInt32)field.GetValue(equipAttriItem, null);


                if (iCurValue == 0)
                {
                    continue;
                }

                for (int j = 0; j < (int)EEquipProp.Count; ++j)
                {
                    EEquipProp eCur = (EEquipProp)j;
                    var enumValue = Utility.GetEnumAttribute<EEquipProp, PropAttribute>(eCur);
                    if (enumValue != null && string.Equals(enumValue.fieldName, field.Name))
                    {
                        return new FashionAttrItemContent
                        {
                            eType = FashionAttrItemContent.FashionAttrItemContentType.FICT_BASE_ATTR,
                            eProp = eCur,
                            iValue = iCurValue
                        };
                    }
                }
            }

            return null;
        }

        public string GetAttributesDesc(int iID,string colorKey = "fashion_attribute_color",string inner="")
        {
            var attrItemContent = GetAttributeItemContent(iID);
            if(attrItemContent != null)
            {
                if(attrItemContent.eType == FashionAttrItemContent.FashionAttrItemContentType.FICT_BASE_ATTR)
                {
                    var kStringBuilder = StringBuilderCache.Acquire();
                    kStringBuilder.Clear();
                    kStringBuilder.AppendFormat("<color={0}>", TR.Value(colorKey));
                    kStringBuilder.Append(Utility.GetEEquipProDesc(attrItemContent.eProp, attrItemContent.iValue,inner));
                    kStringBuilder.Append("</color>");
                    string ret = kStringBuilder.ToString();
                    StringBuilderCache.Release(kStringBuilder);
                    return ret;
                }
                else if(attrItemContent.eType == FashionAttrItemContent.FashionAttrItemContentType.FICT_BUFFER_ID)
                {
                    var buffInfoItem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(attrItemContent.iValue);
                    if(null == buffInfoItem || buffInfoItem.Description.Count <= 0)
                    {
                        return string.Empty;
                    }
                    var kStringBuilder = StringBuilderCache.Acquire();
                    kStringBuilder.Clear();
                    kStringBuilder.AppendFormat("<color={0}>", TR.Value(colorKey));
                    kStringBuilder.Append(buffInfoItem.Description[0]);
                    kStringBuilder.Append("</color>");
                    string ret = kStringBuilder.ToString();
                    StringBuilderCache.Release(kStringBuilder);
                    return ret;
                }
            }
            return string.Empty;
        }

        public void SendFashionAttributeSelect(ulong guid, int selectedId)
        {
            SceneFashionAttributeSelectReq kSend = new SceneFashionAttributeSelectReq();
            kSend.guid = guid;
            kSend.attributeId = selectedId;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }
    }
}