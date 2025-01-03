using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComPlayerPKLevel : MonoBehaviour
    {
        [SerializeField]
        Image pkMainLv = null;

        [SerializeField]
        Image pkSubLv = null;

        public ShowMode showMode = ShowMode.MySelf;

        private void Start()
        {
            if (showMode == ShowMode.MySelf)
            {
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnPkRankChanged);
                SetPkLevel(SeasonDataManager.GetInstance().seasonLevel);
            }
        }   

        private void OnDestroy()
        {
            if (showMode == ShowMode.MySelf)
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnPkRankChanged);
            }
        }

        void _OnPkRankChanged(UIEvent a_event)
        {
            SetPkLevel(SeasonDataManager.GetInstance().seasonLevel);
        }

        public void SetPkLevel(int seasonLevel)
        {
            int nSeasonLevel = seasonLevel;

            if ((SeasonDataManager.GetInstance().IsLevelValid(nSeasonLevel)))
            {
                pkMainLv.enabled = true;
                pkSubLv.enabled = true;
                pkMainLv.SafeSetImage(SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(nSeasonLevel));
                pkSubLv.SafeSetImage(SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(nSeasonLevel));
            }
            else
            {
                pkMainLv.enabled = false;
                pkSubLv.enabled = false;
            }
        }        
    }
}