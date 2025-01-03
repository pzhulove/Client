using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //测试二级Toggle的扩展
    public class ComToggleItemWithExample : ComToggleItem
    {
        public Text toggleItemText;

        protected override void InitItemView()
        {
            base.InitItemView();

            InitItemWithExampleName();
        }


        private void InitItemWithExampleName()
        {
            if (toggleItemText == null)
                return;

            var secondLevelToggleDataWithExample = _comToggleData as ComSecondLevelToggleDataWithExample;
            if (secondLevelToggleDataWithExample != null &&
                string.IsNullOrEmpty(secondLevelToggleDataWithExample.SecondLevelExampleName) == false)
                toggleItemText.text = secondLevelToggleDataWithExample.SecondLevelExampleName;
            else
            {
                toggleItemText.text = "";
            }
        }

    }
}
