/*
 * 
 * 语音 - 发送状态提示框
 * 
 * 
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;

public class VoiceStatusPopup : MonoBehaviour
{
	public const string RES_PATH = "UIFlatten/Prefabs/Chat/VoiceChat/VoiceStatusPopup";

	/*录音状态提示*/
	private GameObject text_info_go;

	[SerializeField]
	private Image text_info_bg;
	[SerializeField]
	private Text text_info;
	[SerializeField]
	private Image mic_start_show;	//开始录音提示
	[SerializeField]
	private GameObject mic_start_decibel;//分贝
	[SerializeField]
	private Image mic_stop_show;	//结束语音提示
	[SerializeField]
    private GameObject[] decibelObjs = new GameObject[8]; 

    [SerializeField]
    private float voiceRecordTexDuration = 0.25f;

    private Coroutine autoPlayTexCor;

	private void Awake()
	{
		_AddUIEvent();
		_InitComponent();
	}

    private void OnDisable()
    {
        _UnInitComponent();
    }

    private void OnDestroy()
    {
		_RemoveUIEvent();
        _UnInitComponent();
    }

	private void _AddUIEvent()
	{
		UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceChatRecordVolumeChanged, _OnRecordVolumeChanged);
		UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceChatSendStart, _OnVoiceSendStart);
		UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceChatReset, _OnVoiceChatReset);
		UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceChatReadySendCancel, _OnVoiceChatReadySendCancel);
		UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VoiceChatShowSendCancel, _OnVoiceChatShowSendCancel);
	}

	private void _RemoveUIEvent()
	{
		UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceChatRecordVolumeChanged, _OnRecordVolumeChanged);
		UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceChatSendStart, _OnVoiceSendStart);
		UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceChatReset, _OnVoiceChatReset);
		UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceChatReadySendCancel, _OnVoiceChatReadySendCancel);
		UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VoiceChatShowSendCancel, _OnVoiceChatShowSendCancel);
	}

	private void _OnRecordVolumeChanged(UIEvent uiEvent)
	{
		if(null == uiEvent || null == uiEvent.Param1)
		{
			return;
		}
		float[] volumes = uiEvent.Param1 as float[];
		if(null == volumes || volumes.Length <= 0)
		{
			return;
		}
		_PlayMicDecibelImage(volumes[0]);
	}

	private void _OnVoiceSendStart(UIEvent uiEvent)
	{
		_SetInfoTextActive(true);
	}

	private void _OnVoiceChatReset(UIEvent uiEvent)
	{
		_SetInfoTextWithUIEventParam1(uiEvent);
		_ShowVoiceInfoBg(false);
		_SetInfoTextActive(false);
	}

	private void _SetInfoTextWithUIEventParam1(UIEvent uiEvent)
	{
		if(uiEvent != null && uiEvent.Param1 != null)
		{
			string desc = uiEvent.Param1 as string;
			if(!string.IsNullOrEmpty(desc))
			{
				_SetInfoText(desc);
			}
		}
	}

	private void _OnVoiceChatReadySendCancel(UIEvent uiEvent)
	{
		_SetInfoTextWithUIEventParam1(uiEvent);
		_ShowVoiceInfoBg(true);
	}

	private void _OnVoiceChatShowSendCancel(UIEvent uiEvent)
	{
		_SetInfoTextWithUIEventParam1(uiEvent);
		_ShowVoiceInfoBg(false);
	}

	private void _InitComponent()
	{
		text_info_go = this.gameObject;
	}

    private void _UnInitComponent()
    {
		if (autoPlayTexCor != null)
        {
            StopCoroutine(autoPlayTexCor);
            autoPlayTexCor = null;
        }
        StopAllCoroutines();
        _PlayMicDecibelImage(0.0f);
    }

    private void _AutoPlayMicDecibelImage(bool isPlay)
	{
		if (this.gameObject.activeSelf)
		{
            if (autoPlayTexCor != null)
            {
                StopCoroutine(autoPlayTexCor);
            }
            autoPlayTexCor = StartCoroutine (PlayDecibelImageLoop (isPlay));
		} 
	}

	IEnumerator PlayDecibelImageLoop(bool isLoop)
	{
		float value = 0.0f;
        float duration = voiceRecordTexDuration;
        while (isLoop)
		{
			if (value > 1.0f)
				value = 0.0f;
            _PlayMicDecibelImage(value);
            yield return Yielders.GetWaitForSeconds(duration);
			value += 0.2f;
			yield return null;
		}
		_PlayMicDecibelImage (0.0f);
	}

	private void _PlayMicDecibelImage(float voiceDecibel)
	{
		if (voiceDecibel < 0f || voiceDecibel > 1f)
			return;
		voiceDecibel = voiceDecibel < 0.2f ? 0.2f :
					   voiceDecibel < 0.4f ? 0.4f :
					   voiceDecibel < 0.6f ? 0.6f :
					   voiceDecibel < 0.8f ? 0.8f :
				       voiceDecibel < 1.0f ? 1.0f : 0.0f;
		

        if(decibelObjs != null && decibelObjs.Length > 0)
        {
            float value = 1.0f / decibelObjs.Length;
            int num = Mathf.CeilToInt(voiceDecibel / value);

            for (int i = 0; i < decibelObjs.Length; i++)
            {
                decibelObjs[i].CustomActive(false);
            }

            for (int i = 0; i < num && i < decibelObjs.Length; i++)
            {
                decibelObjs[i].CustomActive(true);
            }
        }
	}

	private void _SetVoiceInfoPos()
	{
		if (text_info_go)
		{
			RectTransform textGo = text_info_go.GetComponent<RectTransform> ();
			textGo.anchoredPosition = new Vector2 (Screen.width/2.0f ,//- this.transform.position.x,
				Screen.height/2.0f //- this.transform.position.y
			);
		}
	}

	private void _ShowVoiceInfoBg(bool isShow)
	{
        text_info_bg.CustomActive(isShow);
        mic_start_show.CustomActive(!isShow);
        mic_start_decibel.CustomActive(!isShow);
        mic_stop_show.CustomActive(isShow);
	}

	private void _SetInfoText(string text)
	{
		if (text_info) 
		{
			text_info.text = text;
		}
	}

	private void _SetInfoTextActive(bool isShow)
	{
		text_info_go.CustomActive(isShow);     
		_PlayMicDecibelImage(0.0f);    
	}
}
