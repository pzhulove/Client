using System;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    //个性化Item的数据
    public class TestDropDownData : ComControlData
    {

        //个性化的数据
        public string DropDownName;
        public TestDropDownData()
        {

        }

        public TestDropDownData(int index, int id, string name, bool isSelected)
            : base(index, id, name, isSelected)
        {
        }
    }
}