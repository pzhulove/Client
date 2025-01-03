using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    //class ChatNotifyListener : MonoBehaviour
    //{
    //    public GameObject goTarget;
    //    List<GameObject> NeedReshowEffectFuncs = new List<GameObject>();
    //    // Use this for initialization
    //    void Start()
    //    {
    //        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnChatNotified);
    //        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnChatFrameStatusChanged, _OnChatFrameStatusChanged);
    //        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MiddleFrameClose, _OnMiddleFrameClose);
    //        _Check();
    //    }

    //    public void OnClickChatNotify()
    //    {
    //        ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle,new ChatFrameData { curPrivate = null, eChatType = ChatType.CT_PRIVATE_LIST });
    //    }

    //    void OnDestroy()
    //    {
    //        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnChatNotified);
    //        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnChatFrameStatusChanged, _OnChatFrameStatusChanged);
    //        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MiddleFrameClose, _OnMiddleFrameClose);
    //        NeedReshowEffectFuncs.Clear();
    //    }

    //    void _OnChatNotified(UIEvent uiEvent)
    //    {
    //        ulong uid = (ulong)uiEvent.Param1;
    //        bool bDirty = (bool)uiEvent.Param2;
    //        if(bDirty)
    //        {
    //            if (!ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>())
    //            {
    //                if (_IsMiddleFrameOpen()&& goTarget.gameObject.activeSelf == false)
    //                {
    //                    _AddReshowEffectFuncs(goTarget);
    //                    return;
    //                }
    //            }

    //            goTarget.CustomActive(!ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>());
    //        }
    //        else
    //        {
    //            if (!ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>() &&
    //                RelationDataManager.GetInstance().GetPriDirty())
    //            {
    //                if (_IsMiddleFrameOpen() && goTarget.gameObject.activeSelf == false)
    //                {
    //                    _AddReshowEffectFuncs(goTarget);
    //                    return;
    //                }
    //            }
    //            goTarget.CustomActive(!ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>() &&
    //                RelationDataManager.GetInstance().GetPriDirty());
    //        }
    //    }

    //    void _OnChatFrameStatusChanged(UIEvent uiEvent)
    //    {
    //        bool bOpened = (bool)uiEvent.Param1;
    //        if(bOpened)
    //        {
    //            goTarget.CustomActive(false);
    //        }
    //        //else
    //        //{
    //        //    gameObject.CustomActive(RelationDataManager.GetInstance().GetPriDirty());
    //        //}
    //    }
    //    void _OnMiddleFrameClose(UIEvent iEvent)
    //    {
    //        for (int i = 0; i < NeedReshowEffectFuncs.Count; i++)
    //        {
    //            GameObject bind = NeedReshowEffectFuncs[i];

    //            if (bind != null)
    //            {
    //                bind.CustomActive(true);
    //            }
    //        }

    //        NeedReshowEffectFuncs.Clear();
    //    }
    //    void _Check()
    //    {
    //        goTarget.CustomActive(!ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>() &&
    //RelationDataManager.GetInstance().GetPriDirty());
    //    }

    //    bool _IsMiddleFrameOpen()
    //    {
    //        DictionaryView<string, IClientFrame> allFrames = ClientSystemManager.GetInstance().GetAllFrames();

    //        var allframe = allFrames.GetEnumerator();

    //        while (allframe.MoveNext())
    //        {
    //            var frame = allframe.Current.Value as IClientFrame;

    //            if (frame == null)
    //            {
    //                continue;
    //            }

    //            if (frame.GetLayer() == FrameLayer.Middle && frame.GetFrameType() == eFrameType.FullScreen)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    void _AddReshowEffectFuncs(GameObject go)
    //    {
    //        GameObject bind = NeedReshowEffectFuncs.Find(value => { return value == go; });

    //        if (bind == null && go != null)
    //        {
    //            NeedReshowEffectFuncs.Add(go);
    //        }
    //    }
    //}
}