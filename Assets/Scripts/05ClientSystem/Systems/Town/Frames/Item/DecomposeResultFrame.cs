using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class DecomposeResultData
    {
        public bool bSingle = false;
        public List<ItemData> arrItems = null;
        public bool bIsDecomposeFashion = false;
    }

    class DecomposeResultFrame : ClientFrame
    {
        [UIObject("Progress")]
        GameObject m_objProgressRoot;

        //[UIObject("Progress/EffUI_fenjiedh02")]
        //GameObject m_objEffectSingle;

        //[UIObject("Progress/EffUI_fenjiedh01")]
        //GameObject m_objEffectMulti;

        [UIObject("Result")]
        GameObject m_objResultRoot;

        [UIControl("Result/ItemListView")]
        ComUIListScript m_comItemList;

        [UIControl("Result/Title/Text")]
        Text m_titleTxt;

        private string sEffectPath = "Effects/UI/Prefab/EffUI_Common/Prefab/EffUI_common_huode";
        private List<int> indexs = new List<int>();

        DecomposeResultData m_resultData = null;
		UnityEngine.Coroutine m_coroutineDecompose = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/DecomposeResult";
        }

        protected override void _OnOpenFrame()
        {
            m_resultData = userData as DecomposeResultData;

            if (m_resultData != null)
            {
                if (m_resultData.bIsDecomposeFashion)
                {
                    if (m_titleTxt != null)
                    {
                        m_titleTxt.text = "时装分解成功";
                    }
                }

                m_coroutineDecompose = GameFrameWork.instance.StartCoroutine(_ShowDecompose());
            }
            else
            {
                Logger.LogError("分解界面数据错误！");
            }
        }

        protected override void _OnCloseFrame()
        {
            if (m_coroutineDecompose != null)
            {
                GameFrameWork.instance.StopCoroutine(m_coroutineDecompose);
                m_coroutineDecompose = null;
            }

            m_resultData = null;
            indexs.Clear();
        }

        private ButtonEx mBtnConfirm;
        protected override void _bindExUI()
        {
            base._bindExUI();
            mBtnConfirm = mBind.GetCom<ButtonEx>("ButtonConfirm");
            mBtnConfirm.SafeRemoveAllListener();
            mBtnConfirm.SafeAddOnClickListener(_OnConfirmClicked);
        }

        protected override void _unbindExUI()
        {
            base._unbindExUI();
            mBtnConfirm.SafeRemoveAllListener();
            mBtnConfirm = null;
        }

        IEnumerator _ShowDecompose()
        {
            m_objProgressRoot.SetActive(true);
            m_objResultRoot.SetActive(false);

			//m_objEffectSingle.SetActive(m_resultData.bSingle);
			//m_objEffectMulti.SetActive(!m_resultData.bSingle);
			var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_DECOMPOSE_ANIM_TIME_DELAY);
	        float delayTime = systemValue == null ? 2f : (float)systemValue.Value / 1000;

			yield return Yielders.GetWaitForSeconds(delayTime);

            _ShowResult();

            m_coroutineDecompose = null;
        }

        void _ShowResult()
        {
            m_objProgressRoot.SetActive(false);
            m_objResultRoot.SetActive(true);

            m_comItemList.Initialize();

            m_comItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };
            
            m_comItemList.onItemVisiable = var =>
            {
                if (m_resultData != null && m_resultData.arrItems != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_resultData.arrItems.Count)
                    {
                        ItemData item = m_resultData.arrItems[var.m_index];
                        ComItem comItem = var.gameObjectBindScript as ComItem;
                        comItem.Setup(item, (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });

                        Utility.GetComponetInChild<Text>(var.gameObject, "Desc").text =
                        string.Format("{0}x{1}", item.GetColorName(), item.Count);

                        GameObject mContent = Utility.FindChild(var.gameObject,"EffectRoot");

                        if(!indexs.Contains(var.m_index))
                        {
                            if (m_resultData.bIsDecomposeFashion)
                            {
                                GameObject mEffectGo = Utility.FindChild(mContent, "EffUI_common_huode");

                                if (mEffectGo != null)
                                {
                                    GameObject.Destroy(mEffectGo);
                                }

                                GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(sEffectPath);
                                go.name = "EffUI_common_huode";
                                Utility.AttachTo(go, mContent);

                                indexs.Add(var.m_index);
                            }
                        }
                    }
                }
            };

            if (m_resultData != null && m_resultData.arrItems != null)
            {
                m_comItemList.SetElementAmount(m_resultData.arrItems.Count);
            }
            else
            {
                m_comItemList.SetElementAmount(0);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DecomposeFinished);
        }

        [UIEventHandle("Result/Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        void _OnConfirmClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
