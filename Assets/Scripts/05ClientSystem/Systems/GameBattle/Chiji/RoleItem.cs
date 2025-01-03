using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ProtoTable;

namespace GameClient
{
    public delegate void OnSelectJobClick(int jobID);
    public class RoleItem : MonoBehaviour,IDisposable
    {
        [SerializeField]private Image mJobName;
        [SerializeField]private GeObjectRenderer mGeObjectRenderer;
        [SerializeField]private Image mJobImage;
        [SerializeField]private Button mSelectRoleBtn;

        private int mJobID;
        private OnSelectJobClick mOnSelectJobClick;
        int[] geObjectRenderLayers = new int[6] { 25, 26, 27, 28, 29, 30 };

        private void Awake()
        {
            if (mSelectRoleBtn != null)
            {
                mSelectRoleBtn.onClick.RemoveAllListeners();
                mSelectRoleBtn.onClick.AddListener(() => 
                {
                    if (mOnSelectJobClick != null)
                    {
                        mOnSelectJobClick.Invoke(mJobID);
                    }
                });
            }
        }

        public void OnItemVisiable(int jobID, OnSelectJobClick onSelectJobClick,int index)
        {
            mJobID = jobID;
            mOnSelectJobClick = onSelectJobClick;

            UpdateRoleItemInfo(mJobID,index);
        }

        private void UpdateRoleItemInfo(int jobID,int idx)
        {
            var mJobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobID);
            if (mJobTable == null)
            {
                return;
            }

            if (mJobName != null)
            {
                ETCImageLoader.LoadSprite(ref mJobName, mJobTable.CharacterCollectionArtLetting);
            }

            if (mGeObjectRenderer != null)
            {
                mGeObjectRenderer.gameObject.CustomActive(false);
            }

            //mJobImage 节点显示
            if (mJobImage != null)
            {
                ETCImageLoader.LoadSprite(ref mJobImage, mJobTable.CharacterCollectionPhoto);
                mJobImage.gameObject.CustomActive(true);
            }
        }

        public void Dispose()
        {
            mJobID = 0;
            mOnSelectJobClick = null;

            if (mSelectRoleBtn != null)
            {
                mSelectRoleBtn.onClick.RemoveAllListeners();
            }

            mSelectRoleBtn = null;
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

