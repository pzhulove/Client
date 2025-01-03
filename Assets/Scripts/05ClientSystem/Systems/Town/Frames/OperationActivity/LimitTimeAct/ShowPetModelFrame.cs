using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System.Collections;

namespace GameClient
{
    
    class ShowPetModelFrame : ClientFrame
    {
        int iPetId = 0;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/ShowPetModelFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            iPetId = (int)userData;
            UpdateActor(iPetId);
        }

        protected sealed override void _OnCloseFrame()
        {
            iPetId = 0;
            mAvatarRenderer.ClearAvatar();
        }

        void UpdateActor(int iPetID)
        {
            PetTable Pet = TableManager.instance.GetTableItem<PetTable>(iPetID);
            if (Pet == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", iPetID);
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(Pet.ModeID);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", Pet.ModeID);
                }
                else
                {
                    PlayerUtility.LoadPetAvatarRenderEx(iPetID, mAvatarRenderer);
                    
                    Vector3 avatarPos = mAvatarRenderer.avatarPos;
                    avatarPos.y = Pet.ChangedHeight / 1000.0f;
                    mAvatarRenderer.avatarPos = avatarPos;

                    Quaternion qua = mAvatarRenderer.avatarRoation;
                    mAvatarRenderer.avatarRoation = Quaternion.Euler(qua.x, Pet.ModelOrientation / 1000.0f, qua.z);

                    var vscale = mAvatarRenderer.avatarScale;

                    Vector3 avatarScale = mAvatarRenderer.avatarScale;
                    avatarScale.y = avatarScale.x = avatarScale.z = Pet.PetModelSize / 1000.0f;
                    mAvatarRenderer.avatarScale = avatarScale;
                }
            }
        }
        #region ExtraUIBind
        private GeAvatarRendererEx mAvatarRenderer = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mAvatarRenderer = mBind.GetCom<GeAvatarRendererEx>("AvatarRenderer");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected sealed override void _unbindExUI()
        {
            mAvatarRenderer = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}