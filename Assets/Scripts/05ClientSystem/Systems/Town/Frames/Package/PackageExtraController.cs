
using UnityEngine;

namespace GameClient
{
    //背包中额外添加的控制器
    public class PackageExtraController : MonoBehaviour
    {

        [SerializeField] private GameObject packageEquipPlanRoot;
        private PackageEquipPlanSwitchView _packageEquipPlanSwitchView;

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            _packageEquipPlanSwitchView = null;
        }

        private void OnEnable()
        {
            OnEnableEquipPlanController();

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
            UpdateEquipPlanController();
        }

        private void OnEnableEquipPlanController()
        {
            UpdateEquipPlanController();
        }

        //更新装备方案的控制器
        private void UpdateEquipPlanController()
        {
            //没有开着
            if (EquipPlanUtility.IsEquipPlanOpenedByServer() == false)
            {
                CommonUtility.UpdateGameObjectVisible(packageEquipPlanRoot, false);
                return;
            }

            var currentClientSystem = ClientSystemManager.GetInstance().CurrentSystem 
                as ClientSystemGameBattle;
            //吃鸡系统
            if (currentClientSystem != null)
            {
                CommonUtility.UpdateGameObjectVisible(packageEquipPlanRoot, false);
                return;
            }

            //不展示
            if (EquipPlanUtility.IsShowEquipPlanFunction() == false)
            {
                CommonUtility.UpdateGameObjectVisible(packageEquipPlanRoot, false);
            }
            else
            {
                //展示
                CommonUtility.UpdateGameObjectVisible(packageEquipPlanRoot, true);

                if (_packageEquipPlanSwitchView == null)
                {
                    if (packageEquipPlanRoot != null)
                    {
                        var packageEquipPlanPrefab = CommonUtility.LoadGameObject(packageEquipPlanRoot);
                        if (packageEquipPlanPrefab != null)
                        {
                            _packageEquipPlanSwitchView =
                                packageEquipPlanPrefab.GetComponent<PackageEquipPlanSwitchView>();
                        }
                    }
                }
            }
        }
        #endregion

    }
}
