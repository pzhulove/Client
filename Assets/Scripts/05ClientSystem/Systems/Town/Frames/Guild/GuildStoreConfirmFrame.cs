using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Network;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    class GuildStoreConfirmFrameData
    {
        public string title;
        public int iMax;
        public int iCurCount;
        public UnityAction<int> onOk;
    }

    class GuildStoreConfirmFrame : ClientFrame
    {
        GuildStoreConfirmFrameData data = new GuildStoreConfirmFrameData();

        [UIControl("", typeof(StateController))]
        StateController comState;
        [UIControl("count", typeof(InputField))]
        InputField inputField;
        [UIControl("Name", typeof(Text))]
        Text title;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildStoreConfirmFrame";
        }

        protected override void _OnOpenFrame()
        {
            data = userData as GuildStoreConfirmFrameData;

            _AddButton("Close", () =>
            {
                frameMgr.CloseFrame(this);
            });

            _AddButton("minus", () =>
            {
                --data.iCurCount;
                _Refresh();
            });

            _AddButton("min", () =>
            {
                data.iCurCount = 1;
                _Refresh();
            });

            _AddButton("add", () =>
            {
                data.iCurCount += 1;
                _Refresh();
            });

            _AddButton("max", () =>
            {
                data.iCurCount = data.iMax;
                _Refresh();
            });

            _AddButton("ok", () =>
            {
                if (null != data.onOk)
                {
                    data.onOk.Invoke(data.iCurCount);
                    data.onOk = null;
                    frameMgr.CloseFrame(this);
                }
            });

            if (null != inputField)
                inputField.onValueChanged.AddListener(_OnValueChanged);

            if(null != title)
            {
                title.text = data.title;
            }

            _Refresh();
        }

        protected override void _OnCloseFrame()
        {
            if (null != inputField)
            {
                inputField.onValueChanged.RemoveListener(_OnValueChanged);
                inputField = null;
            }
            data = null;
        }

        void _OnValueChanged(string value)
        {
            int iOutValue = 0;
            if (int.TryParse(value, out iOutValue))
            {
                data.iCurCount = iOutValue;
            }
            _Refresh();
        }

        void _Refresh()
        {
            data.iCurCount = (int)IntMath.Clamp(data.iCurCount, 1, data.iMax);
            if (data.iCurCount == data.iMax && data.iCurCount == 1)
            {
                comState.Key = "balance";
            }
            else if (data.iCurCount == 1)
            {
                comState.Key = "min";
            }
            else if (data.iCurCount == data.iMax)
            {
                comState.Key = "max";
            }
            else
            {
                comState.Key = "normal";
            }
            if (null != inputField)
            {
                inputField.onValueChanged.RemoveListener(_OnValueChanged);
                inputField.text = data.iCurCount.ToString();
                inputField.onValueChanged.AddListener(_OnValueChanged);
            }
        }
    }
}