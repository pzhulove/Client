using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum UIState
{
    state1,
    state2,
    state3,
}

namespace GameClient
{
    [System.Serializable]
    public class StateInfo
    {
        public UIState uiState;
        public List<GameObject> showObjs = new List<GameObject>();
    }
    public class LiteStateControl : MonoBehaviour
    {
        [SerializeField]
        List<StateInfo> stateInfos = new List<StateInfo>();

        [SerializeField]
        UIState curState;

        private void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshControlState, OnRefreshControlState);
        }

        private void Start()
        {
            Refresh(curState);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshControlState, OnRefreshControlState);
        }

        void OnRefreshControlState(UIEvent uiEvent)
        {
            if(uiEvent == null)
            {
                return;
            }

            ClientFrame frame = uiEvent.Param1 as ClientFrame;
            if(frame == null || frame.GetFrame() == null)
            {
                return;
            }

            if(!IsChildOf(frame.GetFrame().gameObject) && this.gameObject != frame.GetFrame().gameObject)
            {
                return;
            }   

            if(!(uiEvent.Param2 is UIState))
            {
                return;
            }

            UIState state = (UIState)uiEvent.Param2;
            Refresh(state);
        }

        bool IsChildOf(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            Transform parent = this.transform.parent;
            while (parent != null)
            {
                if (parent == go)
                {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }

        void ShowObjs(List<GameObject> objs,bool show)
        {
            if(objs == null)
            {
                return;
            }

            foreach(var obj in objs)
            {
                obj.CustomActive(show);
            }
        }
        
        public void Refresh(UIState state)
        {
            curState = state;

            if(stateInfos == null)
            {
                return;
            }

            List<GameObject> showObjs = new List<GameObject>();
            for(int i = 0;i < stateInfos.Count;i++)
            {
                if(stateInfos[i].uiState == curState)
                {
                    showObjs.AddRange(stateInfos[i].showObjs);
                }
                else
                {
                    ShowObjs(stateInfos[i].showObjs, false);
                }                
            }

            ShowObjs(showObjs, true);
        }

        public static void RefreshControlsState(ClientFrame frame, UIState state)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshControlState, frame,state);
        }
    }
}


