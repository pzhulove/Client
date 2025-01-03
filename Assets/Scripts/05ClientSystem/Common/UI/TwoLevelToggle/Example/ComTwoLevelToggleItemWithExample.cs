using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //测试二级Toggle的扩展
    public class ComTwoLevelToggleItemWithExample : ComTwoLevelToggleItem
    {
        public Text twoLevelExampleNameText;

        protected override void InitItemView()
        {
            base.InitItemView();


            InitItemWithExample();
        }

        private void InitItemWithExample()
        {
            if (twoLevelExampleNameText == null)
                return;

            var firstLevelToggleDataWithExample = _comFirstLevelToggleData as ComFirstLevelToggleDataWithExample;
            if (firstLevelToggleDataWithExample != null &&
                string.IsNullOrEmpty(firstLevelToggleDataWithExample.FirstLevelExampleName) == false)
                twoLevelExampleNameText.text = firstLevelToggleDataWithExample.FirstLevelExampleName;
            else
            {
                twoLevelExampleNameText.text = "";
            }
        }

    }
}