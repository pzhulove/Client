
using UnityEngine;

namespace GameClient
{
    //荣誉入口控制器
    public class PackageHonorSystemEntryController : MonoBehaviour
    {

        //荣誉入口
        [SerializeField] private GameObject packageHonorSystemEntryRoot;
        private PackageHonorSystemEntryView _packageHonorSystemEntryView;

        private void OnDestroy()
        {
            _packageHonorSystemEntryView = null;
        }

        private void OnEnable()
        {
            OnEnableController();

            //等级改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged,
                OnPlayerLevelChangeMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged,
                OnPlayerLevelChangeMessage);
        }

        #region EquipPlanController       

        private void OnPlayerLevelChangeMessage(UIEvent uiEvent)
        {
            UpdateController();
        }

        private void OnEnableController()
        {
            UpdateController();
        }

        //加载荣誉系统的入口
        private void UpdateController()
        {
            //已经加载完成
            if (_packageHonorSystemEntryView != null)
                return;

            var isShowHonorSystemEntry = HonorSystemUtility.IsShowHonorSystem();
            //不展示
            if (isShowHonorSystemEntry == false)
            {
                return;
            }

            var entryPrefab = CommonUtility.LoadGameObject(packageHonorSystemEntryRoot);
            if (entryPrefab != null)
                _packageHonorSystemEntryView = entryPrefab.GetComponent<PackageHonorSystemEntryView>();
        }
        #endregion

    }
}
