using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using VoiceSDK;

public class AudioVoiceElement : MonoBehaviour , ISDKVoiceCallback
{
	private enum AudioVoicePlayStatus
	{
        Recycle,               //被回收
        Use,                  //可使用 
	}

    private UGUISpriteAnimation thisSpriteAnim;
    private ImageFillAnimation fillAnimation = null;

	//private Sprite voice_level_1;
	//private Sprite voice_level_2;
	private Sprite voice_level_3;
	//private List<Sprite> voice_level_imgs;

	private Image voiceImg;

	private AudioVoicePlayStatus currPlayStatus;

    void Awake()
    {
        voiceImg = this.gameObject.GetComponent<Image>();
        thisSpriteAnim = this.gameObject.GetComponent<UGUISpriteAnimation>();
        voice_level_3 = AssetLoader.instance.LoadRes ("UI/Image/NewPacked/MainUI_Liaotian.png:Liaotian_Icon_Yuyinxianshi", typeof(Sprite), false).obj as Sprite;

        currPlayStatus = AudioVoicePlayStatus.Recycle;

        fillAnimation = this.gameObject.GetComponent<ImageFillAnimation>();

        SDKVoiceManager.GetInstance().AddVoiceEventListener(this);
    }

    void OnDestroy()
    {
        SDKVoiceManager.GetInstance().RemoveVoiceEventListener(this);

        thisSpriteAnim = null;
        voiceImg = null;
        voice_level_3 = null;
        fillAnimation = null;
    }

    void OnVoicePlayStart()
    {
        if (currPlayStatus == AudioVoicePlayStatus.Recycle)
            return;
        StartPlayVoiceTex();
    }

    void OnVoicePlayEnd()
    {
        if (currPlayStatus == AudioVoicePlayStatus.Recycle)
            return;
        StopPlayVoiceTex();
    }


    void StartPlayVoiceTex()
    {
        if (thisSpriteAnim)
            thisSpriteAnim.IsPlaying = true;

        if(fillAnimation != null)
        {
            fillAnimation.StartAnimation();
        }
    }

    void StopPlayVoiceTex()
    {
        if (thisSpriteAnim)
            thisSpriteAnim.IsPlaying = false;

        if(fillAnimation != null)
        {
            fillAnimation.StopAnimation();
        }

        RecyclePlayVoiceTex();                
    }

    public void RecyclePlayVoiceTex()
    {
        ResetVoiceImg();
        currPlayStatus = AudioVoicePlayStatus.Recycle;
    }

    public void ResetPlayVoiceTex()
    {
        ResetVoiceImg();
        OnVoicePlayEnd();

        currPlayStatus = AudioVoicePlayStatus.Use;
    }

    private void ResetVoiceImg()
    {
        if (voiceImg != null)
            voiceImg.sprite = voice_level_3;
    }

    public void OnVoiceEventCallback(object sender, SDKVoiceEventArgs e)
    {
        if (null == e)
        {
            return;
        }
        switch (e.eventType)
        {
            case SDKVoiceEventType.ChatPlayStart:
                OnVoicePlayStart();
                break;

            case SDKVoiceEventType.ChatPlayEnd:
                OnVoicePlayEnd();
                break;
        }
    }
}
