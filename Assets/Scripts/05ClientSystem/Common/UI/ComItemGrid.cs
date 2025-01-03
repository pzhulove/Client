using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// ComItem 格子类 用于创建存储 ComItem
    /// 没有数据时 存在一个空格子（改空格子为一个父节点）
    /// </summary>
    public class ComItemGrid : MonoBehaviour
    {
        #region VIEW PARAMS

        public GameObject gridParentGo;
        public GameObject highestGradeTag;

        private ComItem comItemGrid;

        #endregion

        //Unity life cycle
        private void Awake()
        {
            InitGrid();
        }

        //Unity life cycle
        private void OnDestroy()
        {
            UnInitGrid();
        }

        public void InitGrid()
        {
            if (null == gridParentGo)
            {
                return;
            }
            comItemGrid = ComItemManager.Create(gridParentGo);
            SetGridActive(false);

            highestGradeTag.CustomActive(false);
        }

        public void UnInitGrid()
        {
            if (null != comItemGrid)
            {
                ComItemManager.Destroy(comItemGrid);
                comItemGrid = null;
            }
        }

        /// <summary>
        /// 显示隐藏父节点Grid
        /// </summary>
        /// <param name="bActive"></param>
        public void SetGridActive(bool bActive)
        {
            if (null != gridParentGo)
            {
                gridParentGo.CustomActive(bActive);
            }

            if (!bActive)
            {
                //隐藏极品标签
                highestGradeTag.CustomActive(false);
            }
        }

        public void SetGridItemData(ItemData itemData, bool needClickEnabled = true, bool isHighestGradeItem = false)
        {
            if (null != comItemGrid && null != itemData)
            {
                if (needClickEnabled)
                {
                    comItemGrid.Setup(itemData, Utility.OnItemClicked);
                }
                else
                {
                    comItemGrid.Setup(itemData, null);
                }

                //显示极品标签
                highestGradeTag.CustomActive(isHighestGradeItem);
            }
        }
    }
}
