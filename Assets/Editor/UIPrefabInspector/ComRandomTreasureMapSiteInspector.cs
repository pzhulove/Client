using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GameClient
{
    [CustomEditor(typeof(ComRandomTreasureMapSite))]
    public class ComRandomTreasureMapSiteInspector : Editor
    {
        public enum eMapSiteDebug
        {
            eDebugOff,
            eDebugOn,
        }

        private bool mDirty = false;
        private eMapSiteDebug debug = eMapSiteDebug.eDebugOff;
        private ComRandomTreasureMapSite mCurrMapSite = null;

        private int bgTexAssetNameIndex;
        private const string YUEKA_MAP_TEXTURE_NAME_HEAD = "UI/Image/Background/UI_Beijing_Yuekabaoxiang_Ditu_Di_01.jpg:UI_Beijing_Yuekabaoxiang_Ditu_Di_0{0}";

        public override void OnInspectorGUI()
        {
            mCurrMapSite = this.target as ComRandomTreasureMapSite;

            if (GUILayout.Button("打开"))
            {
                if (debug == eMapSiteDebug.eDebugOff)
                {
                    mCurrMapSite.CustomActive(true);
                }
                else
                {
                    EditorGUILayout.HelpBox("调试模式下，不用再点这个按钮了！", MessageType.Warning);
                }
            }

            bgTexAssetNameIndex = EditorGUILayout.IntField("地图背景图名尾序号:", bgTexAssetNameIndex);
            eMapSiteDebug option = (eMapSiteDebug)EditorGUILayout.EnumPopup("开始编辑调试", debug);

            if (option != debug)
            {
                mDirty = true;
                debug = option;      
            }

            if (mDirty && null != mCurrMapSite)
            {               
                GameObject parent = mCurrMapSite.transform.parent.gameObject;
                if (parent == null)
                {
                    return;
                }
                Image mapBgImg = parent.GetComponent<Image>();
                switch (debug)
                {
                    case eMapSiteDebug.eDebugOn:

                            //mCurrMapSite.CustomActive(true);
                            if (bgTexAssetNameIndex <= 0)
                            {
                                EditorGUILayout.HelpBox("序号大于0！", MessageType.Warning);
                                return;
                            }
                            string path = string.Format(YUEKA_MAP_TEXTURE_NAME_HEAD, bgTexAssetNameIndex);
                            bool bSuccess = ETCImageLoader.LoadSprite(ref mapBgImg, path);
                            if (!bSuccess)
                            {
                                EditorGUILayout.HelpBox(string.Format("序号为:{0}, 找不到图片路径！！！", bgTexAssetNameIndex), MessageType.Error);
                                return;
                            }
                            mCurrMapSite.DebugInitView();
                            ActiveEditorTracker.sharedTracker.isLocked = true;

                        break;
                    case eMapSiteDebug.eDebugOff:


                            mCurrMapSite.CustomActive(false);
                            mapBgImg.sprite = null;
                            mCurrMapSite.DestroyDebugGO();
                            ActiveEditorTracker.sharedTracker.isLocked = false;
                        break;
                }

            }

            base.OnInspectorGUI();
        }

    }
}