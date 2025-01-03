using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class CommonGetItemFrame : ClientFrame
    {
        ComItem m_comItem;

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/GetItem/CommonGetItem";
        }

        protected override void _OnOpenFrame()
        {
            CommonGetItemData getItemData = userData as CommonGetItemData;
            if (getItemData == null)
            {
                Logger.LogError("open CommonGetItemFrame, userdata is invalid");
                return;
            }

            GameObject titleObj = Utility.FindGameObject(frame, "Title/EffUI_renwuwancheng_gai/EffUI_renwuwancheng_gaiDZ/zi");
            if (titleObj != null)
            {
                MeshRenderer meshRender = titleObj.GetComponent<MeshRenderer>();
                if (meshRender != null)
                {
                    Texture tex = AssetLoader.instance.LoadRes(getItemData.title,typeof(Texture)).obj as Texture;
                    if (tex != null)
                    {
                        meshRender.material.mainTexture = tex;
                    }
                }
            }

            GameObject itemRoot = Utility.FindGameObject(frame, "Item");
            if (itemRoot != null)
            {
                m_comItem = CreateComItem(itemRoot);
               // m_comItem.Setup(getItemData.itemData, getItemData.itemClickCallback);
            }

            Text labDesc = Utility.GetComponetInChild<Text>(frame, "Desc");
            if (labDesc != null)
            {
                //labDesc.text = getItemData.desc;
            }
        }

        protected override void _OnCloseFrame()
        {
            m_comItem = null;
        }

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
