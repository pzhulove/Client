using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class ComHornMessage : MonoBehaviour
    {
        List<Protocol.HornInfo> horns = new List<Protocol.HornInfo>();
        float start,end,endprotected;
        Protocol.HornInfo current;
        public LinkParse linkParse;
        public UINumber combo;
        public ScalerJump comJump;
        public ScalerJump comJumpX;
        public StateController stateController;

        public void LinkToHornFrame()
        {
            HornFrame.Open();
        }

        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WordChatHorn,OnHornInfo);
            start = 0.0f;
            end = 0.0f;
            current = null;
            comJump.onJumpEnd += _OnJumpEnd;
            stateController.Key = "Off";
        }

        void _OnJumpEnd(int iIndex)
        {
            combo.Value = iIndex;
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WordChatHorn,OnHornInfo);
            current = null;
            horns.Clear();
            comJump.onJumpEnd -= _OnJumpEnd;
        }

        void OnHornInfo(UIEvent uiEvent)
        {
            Protocol.HornInfo hornInfo = uiEvent.Param1 as Protocol.HornInfo;
            if(null == hornInfo)
            {
                Logger.LogErrorFormat("horn convered failed !!!");
                return;
            }

            horns.Add(hornInfo);
        }

        void _OnHornInfoChanged(Protocol.HornInfo hornInfo)
        {
            if(null != linkParse && hornInfo != null)
            {
                bool bComb = hornInfo.num > 1 || hornInfo.combo - hornInfo.num > 0;

                var kBuilder = StringBuilderCache.Acquire();
                kBuilder.AppendFormat("{{P {0} {1} {2} {3}}}:{4}", hornInfo.roldId, hornInfo.name, hornInfo.occu, hornInfo.level,hornInfo.content);
                linkParse.SetText(kBuilder.ToString());
                StringBuilderCache.Release(kBuilder);
                if(null != combo)
                {
                    stateController.Key = "On";

                    if (!bComb)
                    {
                        combo.Value = hornInfo.combo;
                        //Logger.LogErrorFormat("none ----!");
                    }
                    else if(hornInfo.num == 1)
                    {
                        combo.Value = hornInfo.combo - hornInfo.num;
                        comJumpX.StartJumpNormal(0);
                        comJump.StartJumpNormal(hornInfo.combo);
                        //Logger.LogErrorFormat("StartJumpNormal {0}", hornInfo.combo);
                    }
                    else
                    {
                        combo.Value = hornInfo.combo - hornInfo.num;
                        comJumpX.StartJumpNormal(0);
                        comJump.StartJumpContinue(hornInfo.combo - hornInfo.num + 1,hornInfo.num - 1);
                        //Logger.LogErrorFormat("StartJumpContinue {0} --> {1}", hornInfo.combo - hornInfo.num + 1, hornInfo.combo);
                    }
                }
            }
            else
            {
                if (null == linkParse)
                {
                    Logger.LogErrorFormat("missing script LinkParse !!");
                }
            }
        }

        void Update()
        {
            if(horns.Count > 0)
            {
                if(current == null)
                {
                    current = horns[0];
                    start = Time.time;
                    end = Time.time + current.maxTime;
                    endprotected = Time.time + current.minTime;
                    _OnHornInfoChanged(current);
                    return;
                }

                bool bCanBeReplaced = false;
                if(horns.Count > 1)
                {
                    bCanBeReplaced = true;
                    /*
                    if (current.num < 10)
                    {
                        bCanBeReplaced = true;
                    }
                    else
                    {
                        bCanBeReplaced = horns[1].num >= 10;
                    }*/
                }

                float endTime = bCanBeReplaced ? endprotected : end;

                if(Time.time > endTime)
                {
                    horns.Remove(current);
                    current = null;
                    if (horns.Count > 0)
                    {
                        current = horns[0];
                        start = Time.time;
                        end = Time.time + current.maxTime;
                        endprotected = Time.time + current.minTime;
                        _OnHornInfoChanged(current);
                    }
                }
            }

            stateController.Key = current != null ? "On" : "Off";
        }
    }
}