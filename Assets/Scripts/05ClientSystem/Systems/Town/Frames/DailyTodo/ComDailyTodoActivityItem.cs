using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComDailyTodoActivityItem : MonoBehaviour
    {
        #region MODEL PARAMS

        private List<ComDailyTodoActivityRewardItem> tempRewardItems;

        private DailyTodoActivity tempModel;

        #endregion

        #region VIEW PARAMS

        [SerializeField]
        private Image backgroundImg;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text timeText;
        [SerializeField]
        private GameObject endTagGo;
        [SerializeField]
        private Button gotoBtn;
        [SerializeField]
        private GameObject notStartRagGo;
        [SerializeField]
        private ComUIListScript rewardScroll;

        #endregion

        #region PRIVATE METHODS

        private void Awake()
        {
            if (gotoBtn)
            {
                gotoBtn.onClick.AddListener(_OnGotoBtnClick);
            }

            _InitView();
        }

        private void OnDestroy()
        {
            if (gotoBtn)
            {
                gotoBtn.onClick.RemoveListener(_OnGotoBtnClick);
            }

            _ClearView();
        }

        private void _InitView()
        {
            if (tempRewardItems == null)
            {
                tempRewardItems = new List<ComDailyTodoActivityRewardItem>();
            }

            if (rewardScroll != null)
            {
                if (rewardScroll.IsInitialised() == false)
                {
                    rewardScroll.Initialize();
                }
                rewardScroll.onBindItem += _OnActivityDropItemBind;
                rewardScroll.onItemVisiable += _OnActivityDropItemVisable;
                rewardScroll.OnItemRecycle += _OnActivityDropItemRecycle;
            }
        }

        private void _ClearView()
        {
            if (tempRewardItems != null)
            {
                for (int i = 0; i < tempRewardItems.Count; i++)
                {
                    var item = tempRewardItems[i];
                    if (item != null)
                        item.UnInit();
                }
            }

            if (rewardScroll != null)
            {
                rewardScroll.onBindItem -= _OnActivityDropItemBind;
                rewardScroll.onItemVisiable -= _OnActivityDropItemVisable;
                rewardScroll.OnItemRecycle -= _OnActivityDropItemRecycle;
                rewardScroll.UnInitialize();
            }

            tempModel = null;
        }

        private void _OnGotoBtnClick()
        {
            if (tempModel != null && tempModel.gotoHandler != null)
            {
                tempModel.gotoHandler(tempModel);
            }
        }

        private void _SetName(string name)
        {
            if (nameText)
            {
                nameText.text = name;
            }
        }

        private void _SetBackground(string imgPath)
        {
            if (!string.IsNullOrEmpty(imgPath) && backgroundImg)
            {
                //重置色调
                backgroundImg.color = Color.white;

                bool bEnable = ETCImageLoader.LoadSprite(ref backgroundImg, imgPath);
                if (!bEnable)
                {
                    Logger.LogErrorFormat("[ComDailyTodoActivityItem] - SetBackground can not load img : {0}", imgPath);
                }
            }
        }

        private void _SetTime(string time)
        {
            if (timeText)
            {
                timeText.text = time;
            }
        }

        private void _SetActivityState(eActivityDungeonState state)
        {
            if (state == eActivityDungeonState.Start)
            {
                gotoBtn.CustomActive(true);
                endTagGo.CustomActive(false);
                notStartRagGo.CustomActive(false);
            }
            else if (state == eActivityDungeonState.End)
            {
                gotoBtn.CustomActive(false);
                endTagGo.CustomActive(true);
                notStartRagGo.CustomActive(false);
            }
            else
            {
                gotoBtn.CustomActive(false);
                endTagGo.CustomActive(false);
                notStartRagGo.CustomActive(true);
            }
        }

        private void _SetActivityRewardItems()
        {
            if (rewardScroll != null && this.tempModel.rewardItemIds != null)
            {
                rewardScroll.SetElementAmount(this.tempModel.rewardItemIds.Count);
                //rewardScroll.ResetContentPosition();
            }
        }

        private ComDailyTodoActivityRewardItem _OnActivityDropItemBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<ComDailyTodoActivityRewardItem>();
        }

        private void _OnActivityDropItemVisable(ComUIListElementScript item)
        {
            if(item == null || tempModel == null)
            {
                return;
            }
            if (tempModel.rewardItemIds == null || tempModel.rewardItemIds.Count <= 0)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= tempModel.rewardItemIds.Count)
            {
                return;
            }
            var rewardItem = item.gameObjectBindScript as ComDailyTodoActivityRewardItem;
            if (rewardItem == null)
            {
                return;
            }
            rewardItem.Init(tempModel.rewardItemIds[i_Index]);

            if (tempRewardItems != null && !tempRewardItems.Contains(rewardItem))
            {
                tempRewardItems.Add(rewardItem);
            }
        }

        private void _OnActivityDropItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var rewardItem = item.gameObjectBindScript as ComDailyTodoActivityRewardItem;
            if (rewardItem != null)
            {
                rewardItem.UnInit();
            }
        }

        #endregion

        #region  PUBLIC METHODS

        public void RefreshView(DailyTodoActivity model)
        {
            if (null == model)
            {
                return;
            }

            this.tempModel = model;

            _SetActivityState(model.activityDungeonState);

            _SetTime(model.timeDesc);

            _SetBackground(model.backgroundImgPath);

            _SetName(model.name);

            _SetActivityRewardItems();
        }

        public void Recycle()
        {
            _ClearView();
        } 

        #endregion
    }
}
