using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public interface ITwoLevelToggleData
    {
        string Name { get; }
        string SelectName { get; }
        bool IsShowRedPoint { get; }
    }

    public enum ETwoLevelToggleGroupLayout
    {
        Horizontal,
        Vertical
    }

    public class ComTwoLevelToggleGroup : MonoBehaviour, IDisposable
    {
        public delegate void OnValueChanged(int id, bool isOn);
        [SerializeField] private Transform mParentToggleGroupRoot;//一级页签预制体的父节点
        [SerializeField] private Transform mSubToggleGroupRoot;//二级页签预制体的父节点
        [Header("一级页签布局")] [SerializeField] private ETwoLevelToggleGroupLayout mParentGroupLayout;
        [Header("二级页签布局")] [SerializeField] private ETwoLevelToggleGroupLayout mSubGroupLayout;
        [Header("一级页签预制体路径")] [SerializeField] private string mParentTogglePrefabPath;
        [Header("二级页签预制体路径")] [SerializeField] private string mSubTogglePrefabPath;

        private ComTabGroup mParentGroup;
        private ComTabGroup mSubGroup;
        private OnValueChanged mOnParentValueChanged;
        private OnValueChanged mOnSubValueChanged;

        private readonly List<List<string>> mSubToggleNames = new List<List<string>>();//二级页签的名字
        private readonly List<List<string>> mSubToggleSelectNames = new List<List<string>>();//二级页签选中时的名字
        private readonly List<List<bool>> mSubToggleRedPoint = new List<List<bool>>();//二级页签红点
        private readonly List<uint> mSubToggleSelectId = new List<uint>();//每个大页签选中的二级页签的id

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parentToggleDatas">一级页签数据数组</param>
        /// <param name="subToggleDatas">二级页签数据数组</param>
        /// <param name="parentTogglePrefabPath">一级页签预制体路径</param>
        /// <param name="subTogglePrefabPath">二级页签预制体路径</param>
        /// <param name="parentLayout">一级页签布局方式(水平和垂直)</param>
        /// <param name="subLayout">二级页签布局方式(水平和垂直)</param>
        /// <param name="onParentValueChanged">一级页签切换事件</param>
        /// <param name="onSubValueChanged">二级页签切换事件</param>
        /// <param name="defaultSelectParent">默认选中的一级页签id</param>
        /// <param name="defaultSelectSub">默认选中的二级页签id</param>
        public void Init(List<ITwoLevelToggleData> parentToggleDatas, List<List<ITwoLevelToggleData>> subToggleDatas, 
            OnValueChanged onParentValueChanged, OnValueChanged onSubValueChanged, 
            uint defaultSelectParent = 0, uint defaultSelectSub = 0)
        {
            if (parentToggleDatas.Count != subToggleDatas.Count)
            {
                return;
            }
            Dispose();
            List<string> parentNameList = new List<string>(parentToggleDatas.Count);
            List<bool> parentRedPointList = new List<bool>(parentToggleDatas.Count);
            for (int i = 0; i < parentToggleDatas.Count; ++i)
            {
                parentNameList.Add(parentToggleDatas[i].Name);
                parentRedPointList.Add(parentToggleDatas[i].IsShowRedPoint);
            }

            for (int i = 0; i < subToggleDatas.Count; ++i)
            {
                mSubToggleNames.Add(new List<string>(subToggleDatas[i].Count));
                mSubToggleSelectNames.Add(new List<string>(subToggleDatas[i].Count));
                mSubToggleRedPoint.Add(new List<bool>(subToggleDatas[i].Count));
                for (int j = 0; j < subToggleDatas[i].Count; ++j)
                {
                    mSubToggleNames[i].Add(subToggleDatas[i][j].Name);
                    mSubToggleSelectNames[i].Add(subToggleDatas[i][j].SelectName);
                    mSubToggleRedPoint[i].Add(subToggleDatas[i][j].IsShowRedPoint);
                }
                mSubToggleSelectId.Add(0);
            }

            mOnParentValueChanged = onParentValueChanged;
            mOnSubValueChanged = onSubValueChanged;

            if (defaultSelectParent < mSubToggleSelectId.Count)
            {
                mSubToggleSelectId[(int)defaultSelectParent] = defaultSelectSub;
            }

            if (mSubGroupLayout == ETwoLevelToggleGroupLayout.Horizontal)
            {
                mSubGroup = ComTabGroup.CreateHorizontal(mSubToggleGroupRoot, null, mSubTogglePrefabPath, _OnParentToggleValueChanged);
            }
            else
            {
                mSubGroup = ComTabGroup.CreateVertical(mSubToggleGroupRoot, null, mSubTogglePrefabPath, _OnParentToggleValueChanged);
            }

            if (mParentGroupLayout == ETwoLevelToggleGroupLayout.Horizontal)
            {
                mParentGroup = ComTabGroup.CreateHorizontal(mParentToggleGroupRoot, parentNameList.ToArray(), mParentTogglePrefabPath, _OnParentToggleValueChanged, (int)defaultSelectParent, parentRedPointList.ToArray());
            }
            else
            {
                mParentGroup = ComTabGroup.CreateVertical(mParentToggleGroupRoot, parentNameList.ToArray(), mParentTogglePrefabPath, _OnParentToggleValueChanged, (int)defaultSelectParent, parentRedPointList.ToArray());
            }

        }

		/// <summary>
		/// 选中Toggle
		/// </summary>
		/// <param name="parentId"></param>
		/// <param name="subId"></param>
	    public void SelectToggle(int parentId, int subId = -1)
	    {
		    if (mParentGroup != null)
		    {
				mParentGroup.Select(parentId);
		    }

		    if (mSubGroup != null && subId != -1)
		    {
			    mSubGroup.Select(subId);
		    }
	    }

        /// <summary>
        /// 设置一级页签的红点
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">是否显示红点</param>
        public void SetParentRedPoint(int id, bool value)
        {
            if (mParentGroup != null)
            {
                mParentGroup.SetRedPoint(id, value);
            }
        }

        /// <summary>
        /// 设置二级页签的红点
        /// </summary>
        /// <param name="parentId">一级页签id</param>
        /// <param name="id">要设置的二级页签的id</param>
        /// <param name="value">是否显示红点</param>
        public void SetSubRedPoint(int parentId, int id, bool value)
        {
            //如果parentId是当前选中的大页签，设置其红点
            if (mParentGroup != null && parentId == mParentGroup.SelectId)
            {
                if (mSubGroup != null)
                {
                    mSubGroup.SetRedPoint(id, value);
                }
            }
            //保存数据
            if (mSubToggleRedPoint != null && mParentGroup != null && parentId >= 0 && parentId < mSubToggleRedPoint.Count)
            {
                if (id >= 0 && id < mSubToggleRedPoint[parentId].Count)
                {
                    mSubToggleRedPoint[parentId][id] = value;
                }
            }
        }

        /// <summary>
        /// 获取一级页签的个数
        /// </summary>
        /// <returns></returns>
        public int GetParentToggleCount()
        {
            if (mParentGroup != null)
                return mParentGroup.GetToggleCount();

            return 0;
        }

        /// <summary>
        /// 添加一个二级页签
        /// </summary>
        /// <param name="parentId">一级页签id</param>
        /// <param name="data">页签数据</param>
        /// <param name="insertPosition">插入的位置, 默认值-1为添加到最后,0为最前面</param>
        public void AddSubToggle(uint parentId, ITwoLevelToggleData data, int insertPosition = -1)
        {
            if (parentId < mSubToggleNames.Count)
            {
                mSubToggleNames[(int)parentId].Insert(insertPosition >=0 ? insertPosition : mSubToggleNames[(int)parentId].Count -1, data.Name);
            }

            if (parentId < mSubToggleSelectNames.Count)
            {
                mSubToggleSelectNames[(int)parentId].Insert(insertPosition >= 0 ? insertPosition : mSubToggleSelectNames[(int)parentId].Count - 1, data.Name);
            }

            if (parentId < mSubToggleRedPoint.Count)
            {
                mSubToggleRedPoint[(int)parentId].Insert(insertPosition >= 0 ? insertPosition : mSubToggleRedPoint[(int)parentId].Count - 1, data.IsShowRedPoint);
            }

            if (mParentGroup != null && parentId == mParentGroup.SelectId && mSubGroup != null)
            {
                mSubGroup.AddTap(data.Name, data.SelectName, mSubTogglePrefabPath, _OnSubToggleValueChanged, data.IsShowRedPoint, insertPosition);
            }
        }

        /// <summary>
        /// 移除一个二级页签
        /// </summary>
        /// <param name="parentId">一级页签id</param>
        /// <param name="id">要移除的二级页签id</param>
        public void RemoveSubToggle(uint parentId, uint id)
        {
            if (parentId < mSubToggleNames.Count && id < mSubToggleNames[(int)parentId].Count)
            {
                mSubToggleNames[(int)parentId].RemoveAt((int)id);
            }

            if (parentId < mSubToggleRedPoint.Count && id < mSubToggleRedPoint[(int)parentId].Count)
            {
                mSubToggleRedPoint[(int)parentId].RemoveAt((int)id);
            }

            if (parentId < mSubToggleSelectNames.Count && id < mSubToggleSelectNames[(int)parentId].Count)
            {
                mSubToggleSelectNames[(int)parentId].RemoveAt((int)id);
            }

            if (mParentGroup != null && parentId == mParentGroup.SelectId && mSubGroup != null)
            {
                mSubGroup.RemoveTap((int)id);
            }
        }

        /// <summary>
        /// 添加一个一级页签
        /// </summary>
        /// <param name="subToggleDatas">二级页签数据</param>
        /// <param name="data">一级页签数据</param>
        /// <param name="insertPosition">插入的位置, 默认值-1为添加到最后,0为最前面</param>
        public void AddParentToggle(List<ITwoLevelToggleData> subToggleDatas, ITwoLevelToggleData data, int insertPosition = -1)
        {
            if (insertPosition < 0 || insertPosition >= mSubToggleNames.Count)
            {
                mSubToggleNames.Add(new List<string>(subToggleDatas.Count));
                mSubToggleSelectNames.Add(new List<string>(subToggleDatas.Count));
                mSubToggleRedPoint.Add(new List<bool>(subToggleDatas.Count));
                mSubToggleSelectId.Add( 0);
            }
            else
            {
                mSubToggleNames.Insert(insertPosition, new List<string>(subToggleDatas.Count));
                mSubToggleSelectNames.Insert(insertPosition, new List<string>(subToggleDatas.Count));
                mSubToggleRedPoint.Insert(insertPosition, new List<bool>(subToggleDatas.Count));
                mSubToggleSelectId.Insert(insertPosition, 0);
            }

            if (mSubToggleNames.Count == mSubToggleSelectNames.Count && mSubToggleNames.Count == mSubToggleRedPoint.Count)
            {
                for (int i = 0; i < subToggleDatas.Count; ++i)
                {
                    mSubToggleNames[i].Add(subToggleDatas[i].Name);
                    mSubToggleSelectNames[i].Add(subToggleDatas[i].Name);
                    mSubToggleRedPoint[i].Add(subToggleDatas[i].IsShowRedPoint);
                }
            }


            if (mParentGroup != null)
            {
                mParentGroup.AddTap(data.Name, data.SelectName, mParentTogglePrefabPath, _OnParentToggleValueChanged, data.IsShowRedPoint, insertPosition);
            }
        }

        /// <summary>
        /// 移除一个一级页签
        /// </summary>
        /// <param name="id">页签id</param>
        public void RemoveParentToggle(int id)
        {
            if (mParentGroup != null)
            {
                if (mSubToggleNames != null && id >= 0 && id < mSubToggleNames.Count)
                    mSubToggleNames.RemoveAt(id);

                if (mSubToggleSelectNames != null && id >= 0 && id < mSubToggleSelectNames.Count)
                    mSubToggleSelectNames.RemoveAt(id);

                if (mSubToggleRedPoint != null && id >= 0 && id < mSubToggleRedPoint.Count)
                    mSubToggleRedPoint.RemoveAt(id);

                mParentGroup.RemoveTap((int)id);
            }
        }

        /// <summary>
        /// 为一级页签组添加ScrollRect,滑动方式和布局方式一致
        /// </summary>
        /// <param name="movementType">移动类型</param>
        public void SetParentGroupCanScroll(ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (mParentGroup == null)
                return;

            _AddScrollRect(mParentGroup.gameObject, mParentToggleGroupRoot as RectTransform, mParentGroup.Content, mParentGroupLayout == ETwoLevelToggleGroupLayout.Horizontal, mParentGroupLayout == ETwoLevelToggleGroupLayout.Vertical, movementType);
        }

        /// <summary>
        /// 为一级页签组添加ScrollRect
        /// </summary>
        /// <param name="isHorizontal">是否水平滑动</param>
        /// <param name="isVertical">是否垂直华东</param>
        /// <param name="movementType">移动类型</param>
        public void SetParentGroupCanScroll(bool isHorizontal, bool isVertical, ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (mParentGroup == null)
                return;

            _AddScrollRect(mParentGroup.gameObject, mParentToggleGroupRoot as RectTransform, mParentGroup.Content, isHorizontal, isVertical, movementType);
        }

        /// <summary>
        /// 为二级页签组添加ScrollRect,滑动方式和布局方式一致
        /// </summary>
        /// <param name="movementType">移动类型</param>
        public void SetSubGroupCanScroll(ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (mSubGroup == null)
                return;

            _AddScrollRect(mSubGroup.gameObject, mSubToggleGroupRoot as RectTransform, mSubGroup.Content, mSubGroupLayout == ETwoLevelToggleGroupLayout.Horizontal, mSubGroupLayout == ETwoLevelToggleGroupLayout.Vertical, movementType);
        }

        /// <summary>
        /// 为二级页签组添加ScrollRect
        /// </summary>
        /// <param name="isHorizontal">是否水平滑动</param>
        /// <param name="isVertical">是否垂直华东</param>
        /// <param name="movementType">移动类型</param>
        public void SetSubGroupCanScroll(bool isHorizontal, bool isVertical, ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (mSubGroup == null)
                return;

            _AddScrollRect(mSubGroup.gameObject, mSubToggleGroupRoot as RectTransform, mSubGroup.Content, isHorizontal, isVertical, movementType);
        }

        void _AddScrollRect(GameObject groupObject, RectTransform viewPort, RectTransform content, bool isHorizontal, bool isVertical, ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic)
        {
            if (groupObject == null)
                return;

            var scrollRect = groupObject.gameObject.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                scrollRect = groupObject.gameObject.AddComponent<ScrollRect>();
            }

            if (scrollRect != null)
            {
                scrollRect.viewport = viewPort;
                scrollRect.horizontal = isHorizontal;
                scrollRect.vertical = isVertical;
                scrollRect.movementType = movementType;
                scrollRect.content = content;
            }
        }

        void _OnParentToggleValueChanged(bool value, int selectId)
        {
            if (mOnParentValueChanged != null)
            {
                mOnParentValueChanged(selectId, value);
            }

            //二级页签需要重新创建
            if (value && mSubGroup != null)
            {
                if (mSubToggleNames != null && mSubToggleSelectNames != null && mSubToggleRedPoint != null && mSubToggleRedPoint.Count == mSubToggleNames.Count && mSubToggleNames.Count == mSubToggleSelectNames.Count && selectId >= 0 && selectId < mSubToggleNames.Count)
                {
                    mSubGroup.Init(mSubToggleNames[selectId].ToArray(), mSubToggleRedPoint[selectId].ToArray(), mSubTogglePrefabPath, _OnSubToggleValueChanged, mSubToggleSelectNames[selectId].ToArray(), (int)mSubToggleSelectId[selectId]);
                }
            }
        }

        void _OnSubToggleValueChanged(bool value, int selectId)
        {
            if (mOnSubValueChanged != null)
            {
                mOnSubValueChanged(selectId, value);
            }
            //保存大页签下选中的小页签id 用于下一次切换回来时恢复显示该小页签
            if (value && mParentGroup != null && mSubToggleSelectId != null && mParentGroup.SelectId >= 0 && mParentGroup.SelectId < mSubToggleSelectId.Count)
            {
                mSubToggleSelectId[mParentGroup.SelectId] = (uint)selectId;
            }
        }

        public void Dispose()
        {
            if (mParentGroup != null) mParentGroup.Dispose();
            if (mSubGroup != null) mSubGroup.Dispose();
            mSubToggleNames.Clear();
            mSubToggleSelectNames.Clear();
            mSubToggleRedPoint.Clear();
            mSubToggleSelectId.Clear();
            mOnSubValueChanged = null;
            mOnParentValueChanged = null;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}