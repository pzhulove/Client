using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceSDK
{
    public interface IYouMiVoiceChatImpl
    {
        //New Add For Chat Voice
        void JoinChatRoom(string roomId, bool beSaveRoomMsg = false);
        void LeaveChatRoom(string roomId);
        void SendVoiceMessage(string receId,GameClient.ChatType chatType,ref ulong iReqId,bool isTranslate);
        void StopAudioMessage(string extra);

        //void DownloadAudioFile(ulong iReqId, string saveFilePath);
    }

}