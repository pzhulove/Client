using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //二级菜单的数据
    public class ComTwoLevelToggleData
    {
        //第一级toggle的数据
        public ComControlData FirstLevelToggleData;

        //第二级Toggle的数据
        public List<ComControlData> SecondLevelToggleDataList;       
    }
}