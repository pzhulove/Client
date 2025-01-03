using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{ 
    public class ChapterBattlePotionSetFrameData : IComparable<ChapterBattlePotionSetFrameData>
    {
        public ComCommonBind bind    { get; set;}
        public ulong         id      { 
            get 
            {
                return mID;
            } 
            set
            {
                mID = value;
                _updateTypeAndTableID();
            }
        }

        private int          tableID { get; set;}
        private int          type    { get; set;}

        private ulong        mID = 0;

        public ChapterBattlePotionSetFrameData(ComCommonBind bind, ulong id)
        {
            this.id = id;
            this.bind = bind;

        }

        private void _updateTypeAndTableID()
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(id);
            if (null != itemData)
            {
                tableID = itemData.TableID;
                type    = itemData.SubType;
            }
            else
            {
                type = int.MaxValue;
                tableID = int.MaxValue;
            }
        }

#region IComparable implementation
        public int CompareTo(GameClient.ChapterBattlePotionSetFrameData other)
        {
            if (type != other.type)
            {
                return type - other.type;
            }

            if (tableID != other.tableID)
            {
                return other.tableID - tableID;
            }

            if (id > other.id)
            {
                return -1;
            }
            else if (id == other.id)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
#endregion
    }
}
