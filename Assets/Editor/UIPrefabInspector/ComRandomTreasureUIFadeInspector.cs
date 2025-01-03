using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [UnityEditor.CustomEditor(typeof(ComRandomTreasureUIFade))]
    public class ComRandomTreasureUIFadeInspector : UnityEditor.Editor
    {
        public enum eAtlasStatus
        {
            eNone,
            eOpen,
            eClose
        }

        private bool mDirty = false;
        private eAtlasStatus mStatusOption = eAtlasStatus.eNone;
        private ComRandomTreasureUIFade mCurrentAtlas = null;
        public override void OnInspectorGUI()
        {
            eAtlasStatus option = (eAtlasStatus)UnityEditor.EditorGUILayout.EnumPopup("状态改变", mStatusOption);

            if (option != mStatusOption)
            {
                mDirty = true;
                mStatusOption = option;
                mCurrentAtlas = this.target as ComRandomTreasureUIFade;
            }

            if (mDirty && null != mCurrentAtlas)
            {
                mDirty = false;
                switch (mStatusOption)
                {
                    case eAtlasStatus.eOpen:
                        mCurrentAtlas.OpenAtlas();
                        break;
                    case eAtlasStatus.eClose:
                        mCurrentAtlas.CloseAtlas();
                        break;
                    case eAtlasStatus.eNone:

                        break;
                }
            }

            base.OnInspectorGUI();
        }
    }
}