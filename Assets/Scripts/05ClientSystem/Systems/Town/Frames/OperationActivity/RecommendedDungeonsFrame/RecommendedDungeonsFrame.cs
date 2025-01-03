using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RecommendedDungeonsFrame : ClientFrame
    {
        #region ExtraUIBind
        private GameObject mItem = null;
        private GameObject mContent = null;

        protected sealed override void _bindExUI()
        {
            mItem = mBind.GetGameObject("Item");
            mContent = mBind.GetGameObject("content");
        }

        protected sealed override void _unbindExUI()
        {
            mItem = null;
            mContent = null;
        }
        #endregion

        WeekSignSpringTable weekSignSpringTable;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/RecommendedDungeonsFrame/RecommendedDungeonsFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            weekSignSpringTable = userData as WeekSignSpringTable;

            Initialize();
        }

        protected sealed override void _OnCloseFrame()
        {
            weekSignSpringTable = null;
        }

        private void Initialize()
        {
            mItem.CustomActive(false);
            if (weekSignSpringTable != null)
            {
                List<int> ids = new List<int>();

                string[] strs = weekSignSpringTable.AcquiredMethod.Split('|');

                for (int i = 0; i < strs.Length; i++)
                {
                    int id = 0;
                    if(int.TryParse(strs[i], out id))
                    {
                        ids.Add(id);
                    }
                }

                for (int i = 0; i < ids.Count; i++)
                {
                    var linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.AcquiredMethodTable>(ids[i]);
                    if (linkItem == null)
                    {
                        continue;
                    }

                    GameObject go = GameObject.Instantiate(mItem);
                    Utility.AttachTo(go, mContent);
                    go.CustomActive(true);

                    ComCommonBind bind = go.GetComponent<ComCommonBind>();
                    Text name = bind.GetCom<Text>("Name");
                    Text linkZone = bind.GetCom<Text>("LinkZone");
                    Button goBtn = bind.GetCom<Button>("Go");

                    if (name != null)
                        name.text = linkItem.Name;
                    if (linkZone != null)
                        linkZone.text = linkItem.LinkZone;

                    if(goBtn != null)
                    {
                        goBtn.onClick.RemoveAllListeners();
                        goBtn.onClick.AddListener(() => 
                        {
                            ActiveManager.GetInstance().OnClickLinkInfo(linkItem.LinkInfo);

                            frameMgr.CloseFrame(this);
                          
                            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                            {
                                ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                            }
                        });
                    }
                }
            }
        }
    }
}