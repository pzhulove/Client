using System;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    //个性化Item的数据
    public class TestLevelData : ComControlData
    {

        //个性化的数据
        public string LevelName;
        public TestLevelData()
        {

        }

        public TestLevelData(int index, int id, string name, bool isSelected)
            : base(index, id, name, isSelected)
        {
        }
    }
}