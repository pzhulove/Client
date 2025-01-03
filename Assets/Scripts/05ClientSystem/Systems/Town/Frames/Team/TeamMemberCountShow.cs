using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamMemberCountShow : MonoBehaviour
    {
        [SerializeField]
        Text memberCnt = null;

        private void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, OnTeamCreateSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnTeamAddMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnTeamRemoveMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, OnTeamJoinSuccess);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, OnTeamCreateSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnTeamAddMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnTeamRemoveMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, OnTeamJoinSuccess);
        }

        void OnTeamCreateSuccess(UIEvent uiEvent)
        {
            UpdateUI();
        }

        void OnTeamAddMemberSuccess(UIEvent uiEvent)
        {
            UpdateUI();
        }

        void OnTeamRemoveMemberSuccess(UIEvent uiEvent)
        {
            UpdateUI();
        }

        void OnTeamJoinSuccess(UIEvent uiEvent)
        {
            UpdateUI();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            int cnt = TeamDataManager.GetInstance().GetMemberNum();
            memberCnt.SafeSetText(cnt.ToString());

            gameObject.CustomActive(cnt > 0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


