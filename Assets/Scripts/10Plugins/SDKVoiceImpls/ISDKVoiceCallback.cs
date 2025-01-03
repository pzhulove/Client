using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceSDK
{
    public interface ISDKVoiceCallback
    {
        void OnVoiceEventCallback(object sender, SDKVoiceEventArgs e);
    }
}
