using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{

    public class ArborDayActivityView : MonoBehaviour, IActivityView
    {

        private ILimitTimeActivityModel mModel;


        [Space(10)]
        [HeaderAttribute("ArborDayView")]
        [Space(10)]
        [SerializeField] private ArborDayTreeInformationController treeInformationController;
        [SerializeField] private ArborDayRewardController rewardController;

        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {

            if (model != null)
            {
                mModel = model;
            }
            
            InitViewContent();
        }


        public void Show()
        {
            UpdateViewContent();
        }

        public void UpdateData(ILimitTimeActivityModel data)
        {
            //需要特别注意
            mModel = data;

            UpdateViewContent();
        }

        private void InitViewContent()
        {
            if (treeInformationController != null)
                treeInformationController.InitTreeInformationController(mModel);

            if (rewardController != null)
                rewardController.InitRewardController(mModel);
        }

        private void UpdateViewContent()
        {
            if (treeInformationController != null)
                treeInformationController.OnUpdateTreeInformationController(mModel);

            if (rewardController != null)
                rewardController.OnUpdateRewardController(mModel);
        }

        #region BaseClass

        public void Close()
        {
            mModel = null;
            Destroy(gameObject);
        }

        public void Dispose()
        {
        }

        public void Hide()
        {
        }



        #endregion

    }
}
