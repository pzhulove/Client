using Protocol;
using System.Runtime.InteropServices;

namespace GameClient
{
    class UIClientFrameEvent : UIEvent
    {
        public UIClientFrameEvent(EUIEventID eventID,OnCallBack callback)
        {
            this.callback = callback;
            EventID = eventID;
            IsUsing = true;
        }
        public delegate void OnCallBack(IClientFrame frame);
        public OnCallBack callback;
    }
}