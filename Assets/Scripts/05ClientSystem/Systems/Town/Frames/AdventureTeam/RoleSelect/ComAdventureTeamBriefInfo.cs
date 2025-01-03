using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComAdventureTeamBriefInfo : MonoBehaviour
    {
        #region Model Params        
        
        private string tr_adventure_team_nameinfo = "";

        #endregion
        
        #region View Params

        [SerializeField]
        private ComArtLettering mLevelText;
        [SerializeField]
        private Text mNameText;

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake () 
        {
            _InitTR();
        }

        //Unity life cycle
        void OnDestroy () 
        {
            _ClearView();
        }

        void _InitTR()
        {
            tr_adventure_team_nameinfo = TR.Value("adventure_team_role_select_nameinfo");
        }

        void _SetName(string name)
        {
            if (mNameText)
            {
                mNameText.text = string.Format(tr_adventure_team_nameinfo, name);
            }
        }

        void _SetLevel(int level)
        {
            if (mLevelText)
            {
                mLevelText.SetNum(level);
            }
        }

        void _ClearView()
        {
            tr_adventure_team_nameinfo = "";
        }
        
        void _SetName(string name,int level)
        {
            if (mNameText)
            {
                mNameText.text = string.Format("{0}级{1}", level, name);
            }
        }
        #endregion
        
        #region  PUBLIC METHODS

        public void RefreshView()
        {
            _SetName(AdventureTeamDataManager.GetInstance().GetColorAdventureTeamName(), AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel());
            //_SetLevel(AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel());
        }

        #endregion
    }
}