using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 通用的一个toggleGroup管理控件，可以设置红点，可以动态的增加和删除toggle，可配置toggle的选中和非选中状态的图片
    /// </summary>
    [RequireComponent(typeof(ToggleGroup))]
    public sealed class ComTabGroup : MonoBehaviour, IDisposable
    {
        public delegate void OnToggleValueChanged(bool value, int selectId);

        //当前选中的toggle的id
        public int SelectId
        {
            get { return mSelectedId; }
        }

        public RectTransform Content
        {
            get { return mToggleRoot; }
        }

        [SerializeField] private RectTransform mToggleRoot;
        [SerializeField] private ToggleGroup mToggleGroup;

        private List<Toggle> mToggles = new List<Toggle>();

        private OnToggleValueChanged mOnToggleValueChanged;
        private int mSelectedId;              //当前选中的toggleId

        public static ComTabGroup CreateHorizontal(Transform parent, string[] toggleNames, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, int defaultSelectId = 0, bool[] isShowRedPoints = null, string[] selectedNames = null)
        {
            return Create("UIFlatten/Prefabs/Common/ComHorizontalTabGroup", parent, toggleNames, togglePrefabPath, onToggleValueChanged, defaultSelectId, isShowRedPoints, selectedNames);
        }

        public static ComTabGroup CreateVertical(Transform parent, string[] toggleNames, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, int defaultSelectId = 0, bool[] isShowRedPoints = null, string[] selectedNames = null)
        {
            return Create("UIFlatten/Prefabs/Common/ComVerticalTabGroup", parent, toggleNames, togglePrefabPath, onToggleValueChanged, defaultSelectId, isShowRedPoints, selectedNames);
        }

        static ComTabGroup Create(string prefabPath, Transform parent, string[] toggleNames, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, int defaultSelectId = 0, bool[] isShowRedPoints = null, string[] selectedNames = null)
        {
            if (parent == null)
            {
                Logger.LogError("Create ComTapGroup failed, parent is null");
                return null;
            }

            GameObject tapGroup = AssetLoader.instance.LoadResAsGameObject(prefabPath);

            if (tapGroup != null)
            {
                var script = tapGroup.GetComponent<ComTabGroup>();
                if (script == null)
                {
                    Debug.LogError("Can't get scripte <ComTapGroup> from prefab!");
                    return null;
                }
                tapGroup.transform.SetParent(parent, false);
                if (isShowRedPoints != null)
                {
                    script.Init(toggleNames, isShowRedPoints, togglePrefabPath, onToggleValueChanged, selectedNames, defaultSelectId);
                }
                else
                {
                    script.Init(toggleNames, togglePrefabPath, onToggleValueChanged, selectedNames, defaultSelectId);
                }

                return script;
            }
            else
            {
                Logger.LogError("LoadResAsGameObject ComTapGroup failed");
            }
            return null;
        }

        /// <summary>
        /// 初始化togglegroup
        /// </summary>
        /// <param name="toggleNames">toggle上文字的数组</param>
        /// <param name="togglePrefabPath">toggle预制体路径</param>
        /// <param name="onToggleValueChanged">toggle事件处理回调</param>
        /// <param name="defaultSelectId">默认选中的toggle</param>
        /// <param name="selectedImagePath">选中时的图片</param>
        /// <param name="unSelectedImagePath">未选中时图片</param>
        public void Init(string[] toggleNames, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, string[] selectedNames = null, int defaultSelectId = 0, string selectedImagePath = null, string unSelectedImagePath = null)
        {
            if (mToggleGroup == null)
            {
                mToggleGroup = GetComponent<ToggleGroup>();
            }

            Dispose();
            //加载预制体
            var toggleGameObject = AssetLoader.instance.LoadResAsGameObject(togglePrefabPath);
            if (toggleGameObject == null)
            {
                Logger.LogError("加载预制体失败,路径:" + togglePrefabPath);
            }
            var toggle = toggleGameObject.GetComponent<Toggle>();
            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
                if (toggle == null)
                {
                    Logger.LogError("Can't find Toggle in togglePrefab ");
                    return;
                }
            }

            //设置图片
            if (!string.IsNullOrEmpty(selectedImagePath))
            {
                var selectedImage = (toggle.graphic as Image);
                ETCImageLoader.LoadSprite(ref selectedImage, selectedImagePath);
            }

            if (!string.IsNullOrEmpty(unSelectedImagePath))
            {
                var unSelectedImage = (toggle.targetGraphic as Image);
                ETCImageLoader.LoadSprite(ref unSelectedImage, unSelectedImagePath);
            }

            //添加toggles
            mOnToggleValueChanged = onToggleValueChanged;
            if (toggleNames != null)
            {
                for (var i = 0; i < toggleNames.Length; i++)
                {
                    var go = GameObject.Instantiate(toggleGameObject);
                    string selectName = null;
                    if (selectedNames != null && i >= 0 && i < selectedNames.Length)
                    {
                        selectName = selectedNames[i];
                    }
                    _AddToggle(toggleNames[i], selectName, go, i);
                }

                //默认选中
                if (defaultSelectId >= 0 && mToggles.Count > defaultSelectId)
                {
                    mSelectedId = (int)defaultSelectId;
                    mToggles[(int) defaultSelectId].isOn = true;
                }
            }

            Destroy(toggleGameObject);
        }

        public void Init(string[] toggleNames, bool[] isShowRedPoints, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, string[] selectedNames = null, int defaultSelectId = 0, string selectedImagePath = null, string unSelectedImagePath = null)
        {
            _ClearToggles();
            Init(toggleNames, togglePrefabPath, onToggleValueChanged, selectedNames, -1, selectedImagePath, unSelectedImagePath);
            if (isShowRedPoints != null)
            {
                if (toggleNames.Length != isShowRedPoints.Length)
                {
                    Logger.LogError("toggle数量和红点数组数量不一致");
                }

                for (int i = 0; i < isShowRedPoints.Length; i++)
                {
                    SetRedPoint(i, isShowRedPoints[i]);
                }
            }

			//默认选中
	        Select(defaultSelectId);
        }

	    public void Select(int id)
	    {
		    if (id >= 0 && mToggles.Count > id)
		    {
			    mSelectedId = (int)id;
			    mToggles[(int)id].isOn = true;
		    }
		}

        public void SetRedPoint(int id, bool value)
        {
            if (id >= 0 && id < mToggles.Count)
            {
                IRedPointToggle toggle = mToggles[id].GetComponent<IRedPointToggle>();
                if (toggle != null)
                {
                    toggle.SetRedPointActive(value);
                }
            }
        }

        public void SetSelectOutLineColor(Color color)
        {
            for (int i = 0; i < mToggles.Count; ++i)
            {
                IOutLineToggle toggle = mToggles[i].GetComponent<IOutLineToggle>();
                if (toggle != null)
                {
                    toggle.SetSelectOutLineColor(color);
                }
            }
        }

        /// <summary>
        /// 获取有多少个页签
        /// </summary>
        /// <returns>页签数</returns>
        public int GetToggleCount()
        {
            if (mToggles != null)
                return mToggles.Count;

            return 0;
        }

        /// <summary>
        /// 增加一个tap
        /// </summary>
        /// <param name="name">显示的文字</param>
        /// <param name="selectedName">选中时显示的文字</param>
        /// <param name="togglePrefabPath">预制体路径</param>
        /// <param name="onToggleValueChanged">valueChange回调</param>
        /// <param name="siblingIndex">插入到哪个位置, -1为最后, 0为第一个</param>
        public void AddTap(string name, string selectedName, string togglePrefabPath, OnToggleValueChanged onToggleValueChanged, bool isShowRedPoint = false,int siblingIndex = -1)
        {
            GameObject toggleGameObject = AssetLoader.instance.LoadResAsGameObject(togglePrefabPath);
            mOnToggleValueChanged = onToggleValueChanged;
            _AddToggle(name, selectedName, toggleGameObject, siblingIndex);
            SetRedPoint(siblingIndex, isShowRedPoint);
        }

        public void RemoveTap(int siblingIndex)
        {
            if (siblingIndex < 0 || siblingIndex >= mToggles.Count)
            {
                Logger.LogError("错误的id");
            }

            var toggle = mToggles[siblingIndex];


            //所有在id之后的toggle 事件需要重新监听
            for (var i = siblingIndex + 1; i < mToggles.Count; ++i)
            {
                mToggles[i].onValueChanged.RemoveAllListeners();
                var tabToggle = mToggles[i].GetComponent<IComTabToggle>();
                if (tabToggle != null)
                {
                    tabToggle.BindEvent();
                }
                mToggles[i].SafeAddOnValueChangedListener(_ToggleListener(i - 1));
            }
            //移除事件监听并从list中移除
            mToggles[siblingIndex].onValueChanged.RemoveAllListeners();

            //如果销毁的toggle isOn是true 
            if (mSelectedId == siblingIndex)
            {
                toggle.group = null;
                toggle.isOn = false;
                //如果它后面还有toggle，则选中后面的toggle
                if (siblingIndex + 1 < mToggles.Count)
                {
                    mToggles[siblingIndex + 1].isOn = true;
                }
                //如果后面没有，选中前面一个,如果都没有。。则啥都不选中
                else if (siblingIndex - 1 >= 0)
                {
                    mToggles[siblingIndex - 1].isOn = true;
                    mSelectedId -= 1;
                }
                else
                {
                    mSelectedId = 0;
                }
            }

            else if (mSelectedId > siblingIndex)
            {
                mSelectedId -= 1;
            }

            mToggles.RemoveAt(siblingIndex);
            //销毁
            Destroy(toggle.gameObject);
        }

        void _AddToggle(string toggleName, string selectedName, GameObject toggleGameObject, int index)
        {
            if (toggleGameObject == null)
            {
                Logger.LogError("toggle prefab load failed");
                return;
            }
            //设置toggle的一些属性
            var toggle = toggleGameObject.GetComponent<Toggle>();
            toggle.group = mToggleGroup;
            toggle.transform.SetParent(mToggleRoot, false);
            toggle.transform.SetSiblingIndex(index);
            toggle.onValueChanged.RemoveAllListeners();

            if (mSelectedId >= index)
            {
                mSelectedId++;
            }

            //绑定事件,如果index超出范围 则add到list的末尾
            if (index < 0 || index >= mToggles.Count)
            {
                mToggles.Add(toggle);
                toggle.SafeAddOnValueChangedListener(_ToggleListener(mToggles.Count - 1));
            }
            //否则插入到index的位置,并且需要把从index+1开始到最后的toggle的事件重新绑定一次，因为后面的toggle的index都加1了。
            else
            {
                mToggles.Insert(index, toggle);
                toggle.SafeAddOnValueChangedListener(_ToggleListener(index));
                for (var i = index + 1; i < mToggles.Count; ++i)
                {
                    mToggles[i].onValueChanged.RemoveAllListeners();
                    var tabToggle = mToggles[i].GetComponent<IComTabToggle>();
                    if (tabToggle != null)
                    {
                        tabToggle.BindEvent();
                    }

                    mToggles[i].SafeAddOnValueChangedListener(_ToggleListener(i));
                }
            }

            var setNameToggle = toggleGameObject.GetComponent<ISetNameToggle>();
            if (setNameToggle != null)
            {
                setNameToggle.SetName(toggleName, selectedName);
            }
            else
            {
                var toggleText = toggle.GetComponentInChildren<Text>();
                if (toggleText != null)
                {
                    toggleText.text = toggleName;
                }
            }
            var comTabToggle = toggle.GetComponent<IComTabToggle>();
            if (comTabToggle != null)
            {
                comTabToggle.Init();
            }
        }

        void _ClearToggles()
        {
            for (int i = mToggles.Count - 1; i >= 0; i--)
            {
                mToggles[i].onValueChanged.RemoveAllListeners();
                Destroy(mToggles[i].gameObject);
            }
            mToggles.Clear();
            mSelectedId = 0;
        }

        private UnityEngine.Events.UnityAction<bool> _ToggleListener(int id)
        {
            return (val) => _OnTapToggleValueChanged(val, id);
        }

        private void _OnTapToggleValueChanged(bool value, int id)
        {
            if (value)
            {
                mSelectedId = id;
            }

            if (mOnToggleValueChanged != null)
            {
                mOnToggleValueChanged(value, id);
            }
        }

        public void Dispose()
        {
            if (mToggles != null)
            {
                for (var i = 0; i < mToggles.Count; i++)
                {
                    if (mToggles[i] != null)
                    {
                        mToggles[i].onValueChanged.RemoveAllListeners();
                    }
                }
                mToggles.Clear();
            }

            mSelectedId = 0;
            mOnToggleValueChanged = null;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}