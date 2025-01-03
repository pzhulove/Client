using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    public class OpenAwakeFrame : ClientFrame
    {
        protected sealed override void _OnOpenFrame()
        {
        }

        protected sealed override void _OnCloseFrame()
        {
        }

        public sealed override string GetPrefabPath()
        {
			return "UI/Prefabs/OpenAwakeFrame";
        }

        [UIEventHandle("btOK")]
        void OnClickOK()
        {
            frameMgr.CloseFrame(this);
        }

        public void SetNotifyDataByTable(ProtoTable.CommonTipsDesc NotifyData, string content)
        {
            if(NotifyData != null)
            {
                SetContent(content);
                SetDes(NotifyData.TitleText);
                //SetIcon(NotifyData.TitleIcon);
            }            
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack)
        {
            if(OnOKCallBack != null)
            {
                ButtonOK.onClick.RemoveListener(OnOKCallBack);
                ButtonOK.onClick.AddListener(OnOKCallBack);
            }
        }

        public void SetContent(string str)
        {
            content.gameObject.SetActive(false);
            content.text = str;
        }

        public void SetDes(string str)
        {
            description.text = str;
        }

        public void SetIcon(string path)
        {
            if(path == "" || path == "-")
            {
                icon.gameObject.SetActive(false);
            }

            Sprite Icon = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;

            if(Icon == null)
            {
                icon.gameObject.SetActive(false);
            }

            // icon.sprite = Icon;
            ETCImageLoader.LoadSprite(ref icon, path);
        }

        [UIControl("content")]
        protected Text content;

        [UIControl("icon")]
        protected Image icon;

        [UIControl("description")]
        protected Text description;

        [UIControl("btOK")]
        protected Button ButtonOK;
    }
}
