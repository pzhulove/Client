using System;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    //通用控件的数据模型，如：下拉单，Toggle，以及二级Toggle
    public class ComControlData
    {
        public int Index;
        public int Id;
        public string Name;
        public bool IsSelected;

        public ComControlData()
        {

        }

        public ComControlData(int index, int id, string name, bool isSelected)
        {
            Index = index;
            Id = id;
            Name = name;
            IsSelected = isSelected;
        }
    }
}