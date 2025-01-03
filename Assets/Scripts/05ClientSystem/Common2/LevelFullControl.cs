using UnityEngine;
using System.Collections;

namespace GameClient
{
    public class LevelFullControl : MonoBehaviour
    {
        public GameObject goTarget;
        public int iBindActiveID;
        bool bMainActive = false;
        public int BindActiveID
        {
            get
            {
                return iBindActiveID;
            }
            set
            {
                iBindActiveID = value;
                _Update();
            }
        }
        // Use this for initialization
        void Start()
        {
            _Register();
            _Update();
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _Update();
        }

        public void _Update()
        {
            int iFullLevel = 50;
            var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
            if(systemValueItem != null)
            {
                iFullLevel = systemValueItem.Value;
            }
            bool bShow = PlayerBaseData.GetInstance().Level >= iFullLevel;
            if(bShow)
            {
                var data = ActiveManager.GetInstance().GetChildActiveData(iBindActiveID);
                if(data == null || data.activeItem.IsWorkWithFullLevel != 0 ||
                    data.status > (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    bShow = false;
                }
            }
            goTarget.CustomActive(bShow);
        }

        void OnDestroy()
        {
            _UnRegister();
        }

        void _Register()
        {
            _UnRegister();
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
        }

        void _UnRegister()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }
    }
}