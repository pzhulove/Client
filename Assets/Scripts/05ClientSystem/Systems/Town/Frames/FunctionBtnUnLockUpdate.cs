using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{ 
    public class FunctionBtnUnLockUpdate : MonoBehaviour
    {
        [SerializeField]
        FunctionUnLock.eLocationType eLocationType = FunctionUnLock.eLocationType.BottomRightExpand;

        [SerializeField]
        GameObject funcBtnRoot = null;

        private void Awake()
        {
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
        }

        // Start is called before the first frame update
        void Start()
        {
            RefreshBtns();
        }

        private void OnDestroy()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            RefreshBtns();
        }

        void RefreshBtns()
        {
            if(funcBtnRoot == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<FunctionUnLock>();
            if (dicts == null)
            {
                return;                
            }

            var iter = dicts.GetEnumerator();
            while (iter.MoveNext())
            {
                FunctionUnLock adt = iter.Current.Value as FunctionUnLock;
                if (adt == null)
                {
                    continue;
                }

                if(adt.LocationType != eLocationType)
                {
                    continue;
                }

                GameObject buttonObj = Utility.FindGameObject(funcBtnRoot, adt.TargetBtnPos);
                if (buttonObj == null)
                {
                    continue;
                }

                buttonObj.CustomActive(PlayerBaseData.GetInstance().Level >= adt.FinishLevel);
            }
        }
    }

}

