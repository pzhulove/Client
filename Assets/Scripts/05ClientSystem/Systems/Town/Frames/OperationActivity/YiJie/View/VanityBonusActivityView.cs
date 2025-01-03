using Protocol;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
   
    public class VanityBonusActivityView : MonoBehaviour, IDungeonBuffView
    {
        [SerializeField]
        private RectTransform[] mItemRoots = null;
        [SerializeField]
        private ActivityNote mNote;
        [SerializeField]
        private Button goBtn;
        [SerializeField]
        private string mVanityBonusItemRightPath = "UIFlatten/Prefabs/OperateActivity/YiJie/Items/VanityBonusItemRight";

        UnityAction goOnClick;
        public void Init(ILimitTimeActivityModel model,UnityAction callBack)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            goOnClick = callBack;
            InitItems(model);
            mNote.Init(model, false,this.GetComponent<ComCommonBind>());
            goBtn.SafeAddOnClickListener(goOnClick);
        }
        
        void InitItems(ILimitTimeActivityModel model)
        {
            if (model.ParamArray == null || model.ParamArray.Length <= 0)
            {
                return;
            }

            GameObject mItemLeftGo = AssetLoader.GetInstance().LoadResAsGameObject(model.ItemPath);
            if (mItemLeftGo == null)
            {
                Logger.LogError("加载预制体失败，路径:" + model.ItemPath);
                return;
            }

            if (mItemLeftGo.GetComponent<VanityBonusItem>() == null)
            {
                GameObject.Destroy(mItemLeftGo);
                Logger.LogError("预制体上找不到VanityBonusItem的脚本，预制体路径是:" + model.ItemPath);
                return;
            }

            GameObject mItemRightGo = AssetLoader.GetInstance().LoadResAsGameObject(mVanityBonusItemRightPath);
            if (mItemRightGo == null)
            {
                Logger.LogError("加载预制体失败，路径:" + model.ItemPath);
                return;
            }

            if (mItemRightGo.GetComponent<VanityBonusItem>() == null)
            {
                GameObject.Destroy(mItemRightGo);
                Logger.LogError("预制体上找不到VanityBonusItem的脚本，预制体路径是:" + model.ItemPath);
                return;
            }

            for (int i = 0; i < model.ParamArray.Length; i++)
            {
                if (i < 3)
                {
                    AddItem(mItemLeftGo, (int)model.ParamArray[i],i);
                }
                else
                {
                    AddItem(mItemRightGo, (int)model.ParamArray[i], i);
                }
            }

            Destroy(mItemLeftGo);
            Destroy(mItemRightGo);
        }

        void AddItem(GameObject go,int id,int index)
        {
           if (index >= mItemRoots.Length)
           {
                return;
           }

            if (go == null)
            {
                return;
            }

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoots[index], false);
            var script = item.GetComponent<VanityBonusItem>();
            if (script != null)
            {
                script.Init(id);
            }
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            if (mNote != null)
            {
                mNote.Dispose();
            }

            goBtn.SafeRemoveOnClickListener(goOnClick);
            goOnClick = null;
        }
        
    }
}
