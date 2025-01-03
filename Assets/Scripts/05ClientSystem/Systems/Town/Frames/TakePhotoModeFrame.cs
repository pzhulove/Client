using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace GameClient
{
    public class TakePhotoModeFrame : ClientFrame
    {
        private static bool isInitAlart = false;
        #region ExtraUIBind
        private Button mClose = null;
        private Button mScreenShoot = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            //mScreenShoot = mBind.GetCom<Button>("ScreenShoot");
            //mScreenShoot.onClick.AddListener(_onScreenShootButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            //mScreenShoot.onClick.RemoveListener(_onScreenShootButtonClick);
            //mScreenShoot = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            Close();

        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TownUI/TakePhotoModeFrame";
        }

        protected static Coroutine waitToScreenShoot;

        public static void MobileScreenShoot(Camera camera = null, float x = 0, float y = 0, float width = 1, float height = 1)
        {
            string name = string.Format("{0:yyyy-MM-dd-HH_mm_ss_ffff}", System.DateTime.Now);
#if UNITY_IPHONE || UNITY_IOS
            if (!isInitAlart)
            {
                isInitAlart = true;
                SDKInterface.instance.InitAlartText(TR.Value("savePhotoIntoAlarm_title"), TR.Value("savePhotoIntoAlarm_message"), TR.Value("savePhotoIntoAlarm_btnText")); 
            }
#endif
            //#if UNITY_IOS || UNITY_ANDROID
            if (waitToScreenShoot != null)
        {
            GameFrameWork.instance.StopCoroutine(waitToScreenShoot);
        }
        waitToScreenShoot = GameFrameWork.instance.StartCoroutine(WaitToScreenShoot(name, camera, new Rect(0 * x, 0 * y, (int)Screen.width * width, (int)Screen.height * height)));
//#endif
        }

        protected static IEnumerator WaitToScreenShoot(string name, Camera camera, Rect rect)
        {
            yield return new WaitForEndOfFrame();
            name = name + ".jpg";
            RenderTexture currentRT = RenderTexture.active;
            if (camera)
            {
                RenderTexture.active = camera.targetTexture;
                camera.Render();
            }
            Texture2D image = new Texture2D((int)rect.width, (int)rect.height);
            image.ReadPixels(rect, 0, 0);
            image.Apply();
            if (camera)
            {
                RenderTexture.active = currentRT;
            }
            byte[] bytes = image.EncodeToPNG();
            string path = Application.persistentDataPath;
            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android")) + TR.Value("zymg_game_name");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                System.IO.File.WriteAllBytes(path + "/" + name, bytes);
                PluginManager.GetInstance().ScanFile(path + "/" + name);
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("haveSucceedSnapPic"));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}
