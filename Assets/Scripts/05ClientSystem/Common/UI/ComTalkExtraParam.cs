using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComTalkExtraParam : MonoBehaviour {

    public float upOffsetHeight = 300f;
    public float normalHeight = 178f;

    // 显示和隐藏话筒相关
    public float anchorPos0 = 0.0f;
    public float anchorPos1 = 0.0f;

    public Button mFriend = null;
    public Image mFriendRedPoint = null;
    public GameObject mPrivateChatBubble = null;

    public RectTransform talkContent = null;

    void Start()
    {
 
    }
}
