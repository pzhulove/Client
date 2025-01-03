using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using DG.Tweening;
namespace GameClient
{
    public class  StrengthenResult
    {
        public bool StrengthenSuccess;
        public ItemData EquipData;
        public List<ItemData> BrokenItems;
        public uint code;
        public int iStrengthenLevel;
        public int iTargetStrengthenLevel;
        public int iTableID;
        public int[] orgAttr = new int[7]
        {
            0,0,0,0,0,0,0
        };
        public int[] curAttr = new int[7]
        {
            0,0,0,0,0,0,0
        };

        public int[] growthOrgAttr = new int[8]
        {
            0,0,0,0,0,0,0,0
        };

        public int[] growthCurAttr = new int[8]
        {
            0,0,0,0,0,0,0,0
        };

        //辅助装备强化属性
        public int[] assistEquipStrengthenOrgAttr = new int[4]
        {
            0,0,0,0
        };

        //辅助装备强化属性
       public int[] assistEquipStrengthenCurAttr = new int[4]
       {
            0,0,0,0
       };

        //辅助装备激化属性
        public int[] assistEquipGrowthOrgAttr = new int[5]
        {
            0,0,0,0,0
        };

        //辅助装备激化属性
        public int[] assistEquipGrowthCurAttr = new int[5]
        {
            0,0,0,0,0
        };

        public static float ToValue(int iIndex,int iValue)
        {
            if(iIndex >= 0 && iIndex < convertRate.Length)
            {
                return convertRate[iIndex] * iValue;
            }
            return iValue;
        }

        public static string ToDesc(int iIndex, int iValue)
        {
            float value = ToValue(iIndex, iValue);
            return string.Format(formatString[iIndex], iValue);
        }

        public static float GrowthToValue(int iIndex,int iValue)
        {
            if (iIndex >= 0 && iIndex < growthConverRate.Length)
            {
                return growthConverRate[iIndex] * iValue;
            }

            return iValue;
        }

        public static string GrowthToDesc(int iIndex, int iValue)
        {
            float value = GrowthToValue(iIndex, iValue);
            return string.Format(growthFormatString[iIndex], iValue);
        }

        public static float[] convertRate = new float[7]
        {
            1.0f,1.0f,1.0f,1.0f,1.0f,0.01f,0.01f,
        };
        public static string[] formatString = new string[7]
        {
            "{0}",
            "{0}",
            "{0}",
            "{0}",
            "{0}",
            "{0:F2}",
            "{0:F2}",
        };

        public static float[] growthConverRate = new float[8]
        {
            1.0f,1.0f,1.0f,1.0f,1.0f,1.0f,0.01f,0.01f,
        };

        public static string[] growthFormatString = new string[8]
       {
            "{0}",
            "{0}",
            "{0}",
            "{0}",
            "{0}",
            "{0}",
            "{0:F2}",
            "{0:F2}",
       };
    }

    class StrengthenResultUserData
    {
        public uint uiCode;
        public ItemData EquipData;
        public List<ItemData> BrokenItems;
        public bool bContinue;
        public bool NeedBeforeAnimation = false;
    }

    class StrengthenResultFrame : ClientFrame
    {
        List<ComItemNew> m_arrComItems = new List<ComItemNew>();

        //[UIControl("ok/Image/title")]
        //Image m_title;

        Text m_description;

        Text m_descIgnore;

        Image m_textSuccessTitle;

        [UIControl("failed_broken/Title")]
        Text m_textBroken;

        [UIControl("failed_zero/Title")]
        Text m_textZero;

        [UIControl("failed_broken/TitleBg/TitleLeft")]
        Image m_textBrokenTitle;

        [UIControl("failed_zero/TitleBg/TitleLeft")]
        Image m_textZeroTitle;

        ComItem comItemZero;

        [UIObject("failed_zero/common_black/ItemParent")]
        GameObject m_goZeroItem;

        Button m_kBtnStop0;//ok
        Button m_kBtnStop1;//failed
        Button m_kBtnContinue0;//ok
        Button m_kBtnContinue1;//failed
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenResult";
        }

        void _OnClickStopStrengthen()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.IntterruptContineStrengthen);
            frameMgr.CloseFrame(this);
        }

        void _OnClickContinueStrengthen()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.StrengthenContinue);
            frameMgr.CloseFrame(this);
        }

        void _InitAnimations()
        {
            StrengthenResultUserData resultData = userData as StrengthenResultUserData;
            
            //if(resultData.NeedBeforeAnimation == false)
            {
                var dotweens = GamePool.ListPool<DOTweenAnimation>.Get();
                frame.GetComponentsInChildren<DOTweenAnimation>(true, dotweens);
                for (int i = 0; i < dotweens.Count; ++i)
                {
                    dotweens[i].delay -= 2.5f;
                }
                GamePool.ListPool<DOTweenAnimation>.Release(dotweens);

                var uiparticles = GamePool.ListPool<GeUIEffectParticle>.Get();
                frame.GetComponentsInChildren<GeUIEffectParticle>(true, uiparticles);
                for (int i = 0; i < uiparticles.Count; ++i)
                {
                    var emitter = uiparticles[i].SafeGetEmitter();
                    emitter.delayEmit = emitter.delayEmit - 2.5f;
                }
                GamePool.ListPool<GeUIEffectParticle>.Release(uiparticles);
            }
        }

        protected override void _OnOpenFrame()
        {
            StrengthenResultUserData resultData = userData as StrengthenResultUserData;
            if (resultData == null)
            {
                Logger.LogError("StrengthenResultFrame resultData");
                return;
            }

            //新特效带了开头动画，根据需要剔除动画
            _InitAnimations();
 
            string strengthenOk = "ok_10down";
            if(resultData.EquipData != null && resultData.EquipData.StrengthenLevel > 10)
            {
                strengthenOk = "ok_10up";
            }

           m_description = Utility.FindComponent<Text>(frame, strengthenOk + "/common_black/itemname");
            m_descIgnore = Utility.FindComponent<Text>(frame, strengthenOk + "/common_black/desc");
            m_textSuccessTitle = Utility.FindComponent<Image>(frame, strengthenOk + "/common_black/TitleBg/TitleLeft");

            m_kBtnStop0 = Utility.FindComponent<Button>(frame, strengthenOk+"/BtnStop");
            m_kBtnStop0.CustomActive(false);
            m_kBtnStop1 = Utility.FindComponent<Button>(frame, "failed/BtnStop");
            m_kBtnStop1.CustomActive(false);
            m_kBtnContinue0 = Utility.FindComponent<Button>(frame, strengthenOk+"/BtnContinue");
            m_kBtnContinue0.CustomActive(false);
            m_kBtnContinue1 = Utility.FindComponent<Button>(frame, "failed/BtnContinue");
            m_kBtnContinue1.CustomActive(false);

            m_kBtnStop0.onClick.RemoveAllListeners();
            m_kBtnStop0.onClick.AddListener(_OnClickStopStrengthen);
            m_kBtnStop1.onClick.RemoveAllListeners();
            m_kBtnStop1.onClick.AddListener(_OnClickStopStrengthen);
            m_kBtnContinue0.onClick.RemoveAllListeners();
            m_kBtnContinue0.onClick.AddListener(_OnClickContinueStrengthen);
            m_kBtnContinue1.onClick.RemoveAllListeners();
            m_kBtnContinue1.onClick.AddListener(_OnClickContinueStrengthen);

            GameObject currentActice = null;

            GameObject goOk = Utility.FindChild(frame, strengthenOk);
            goOk.CustomActive(resultData.uiCode == (uint)ProtoErrorCode.SUCCESS);
            if(goOk.activeSelf == true) currentActice = goOk;

            GameObject goFailedBroken = Utility.FindChild(frame, "failed_broken");
            goFailedBroken.CustomActive(resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_BROKE);
            if (goFailedBroken.activeSelf == true) currentActice = goFailedBroken;

            GameObject goFailedToZero = Utility.FindChild(frame, "failed_zero");
            goFailedToZero.CustomActive(resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_ZERO);
            if (goFailedToZero.activeSelf == true) currentActice = goFailedToZero;

            GameObject goFailed = Utility.FindChild(frame, "failed");
            goFailed.CustomActive(resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL ||
                resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_MINUS ||
                resultData.uiCode == (uint)ProtoErrorCode.ITEM_SPECIAL_STRENTH_FAIL);
            if (goFailed.activeSelf == true) currentActice = goFailed;

            if(currentActice == goOk)
            {
                Text text = Utility.FindComponent<Text>(goOk,"common_black/hint");
                if(text != null && resultData.EquipData != null)
                {
                    if (resultData.EquipData.EquipType == EEquipType.ET_COMMON)
                    {
                        text.text = "强化至+" + resultData.EquipData.StrengthenLevel;
                    }
                    else if (resultData.EquipData.EquipType == EEquipType.ET_REDMARK)
                    {
                        text.text = "激化至+" + resultData.EquipData.StrengthenLevel;
                    }
                }
                
            }else if(currentActice == goFailed)
            {
               Text text = Utility.FindComponent<Text>(goFailed, "common_black/hint");
                Image title = Utility.FindComponent<Image>(goFailed, "TitleBg/TitleLeft");
                if (text != null && resultData.EquipData != null)
                {
                    text.text = "降级至+" + resultData.EquipData.StrengthenLevel;
                    text.enabled = resultData.uiCode != (uint)ProtoErrorCode.ITEM_SPECIAL_STRENTH_FAIL;
                }
                   
                if (title != null && resultData.EquipData != null)
                {
                    if (resultData.EquipData.EquipType == EEquipType.ET_REDMARK)
                    {
                        if (title != null)
                            ETCImageLoader.LoadSprite(ref title, TR.Value("growth_faild_jihua"));
                    }
                }
            }
            
			if (resultData.uiCode == (uint)ProtoErrorCode.SUCCESS)
			{
				AudioManager.instance.PlaySound(12);
			}
			else {
				AudioManager.instance.PlaySound(11);
			}

            if (resultData.uiCode == (uint)ProtoErrorCode.SUCCESS)
            {
                ComItemNew comItem = CreateComItemNew(Utility.FindChild(frame, strengthenOk+"/common_black/ItemParent"));
                m_arrComItems.Add(comItem);
                comItem.SetActive(true);
                comItem.Setup(resultData.EquipData, null);

                if (resultData.EquipData.EquipType == EEquipType.ET_REDMARK)
                {
                    if (m_textSuccessTitle != null)
                    {
                        ETCImageLoader.LoadSprite(ref m_textSuccessTitle, TR.Value("growth_success_jihua"));
                    }
                }

                if(resultData.bContinue)
                {
                    m_kBtnStop0.CustomActive(true);
                    m_kBtnContinue0.CustomActive(false);
                }
                else
                {
                    m_kBtnStop0.CustomActive(false);
                    m_kBtnContinue0.CustomActive(true);
                }
                m_kBtnStop0.CustomActive(false);
                m_kBtnContinue0.CustomActive(false);

                string desc = string.Format("+{0} {1}", resultData.EquipData.StrengthenLevel, resultData.EquipData.GetColorName());
                m_description.text = desc;

                var strengthenDesc = resultData.EquipData.GetStrengthenDescs();
                var result = "";
                for(int i = 0; i < strengthenDesc.Count; ++i)
                {
                    if(!string.IsNullOrEmpty(result))
                    {
                        result += "\n";
                    }
                    result += strengthenDesc[i];
                }
                m_descIgnore.text = result;
            }
            else if(resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_ZERO)
            {
                GameObject goParent = Utility.FindChild(goFailedToZero, "common_black/Items");
                GameObject goPrefab = Utility.FindChild(goParent, "Item");
                goPrefab.CustomActive(false);

                if (resultData.EquipData.EquipType == EEquipType.ET_COMMON)
                {
                    if (m_textZero != null)
                        m_textZero.text = TR.Value("strenghten_result_failed_zero");
                }
                else if (resultData.EquipData.EquipType == EEquipType.ET_REDMARK)
                {
                    if (m_textZero != null)
                        m_textZero.text = TR.Value("growth_result_failed_zero");
                    if (m_textZeroTitle != null)
                        ETCImageLoader.LoadSprite(ref m_textZeroTitle, TR.Value("growth_faild_jihua"));
                }

                if (comItemZero == null)
                {
                    comItemZero = CreateComItem(m_goZeroItem);
                }

                comItemZero.Setup(resultData.EquipData, null);

                for (int i = 0; resultData.BrokenItems != null && i < resultData.BrokenItems.Count; ++i)
                {
                    GameObject goCurrent = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goCurrent, goParent);

                    goCurrent.CustomActive(true);

                    ComItemNew comItem = CreateComItemNew(Utility.FindChild(goCurrent, "ItemParent"));
                    m_arrComItems.Add(comItem);
                    comItem.Setup(resultData.BrokenItems[i], null);

                    Text text = Utility.FindComponent<Text>(goCurrent, "itemname");
                    var itemData = resultData.BrokenItems[i];
                    string desc = "";
                    if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                    {
                        desc = string.Format("{0}", itemData.GetColorName());
                    }
                    else
                    {
                        desc = string.Format("{0}", itemData.GetColorName());
                    }
                    text.text = desc;
                }
            }
            else if(resultData.uiCode == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_BROKE)
            {
                GameObject goParent = Utility.FindChild(goFailedBroken, "Items");
                GameObject goPrefab = Utility.FindChild(goParent, "Item");
                if(goPrefab != null)
                goPrefab.CustomActive(false);

                if (resultData.EquipData.EquipType == EEquipType.ET_REDMARK)
                {
                    if (m_textBrokenTitle != null)
                        ETCImageLoader.LoadSprite(ref m_textBrokenTitle, TR.Value("growth_faild_jihua"));
                }

                m_textBroken.text = TR.Value("strenghten_result_broken");

                for (int i = 0; resultData.BrokenItems != null && i < resultData.BrokenItems.Count; ++i)
                {
                    GameObject goCurrent = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goCurrent, goParent);

                    goCurrent.CustomActive(true);

                    ComItemNew comItem = CreateComItemNew(Utility.FindChild(goCurrent, "ItemParent"));
                    m_arrComItems.Add(comItem);
                    comItem.Setup(resultData.BrokenItems[i], null);

                    Text text = Utility.FindComponent<Text>(goCurrent, "itemname");
                    var itemData = resultData.BrokenItems[i];
                    string desc = "";
                    if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                    {
                        desc = string.Format("{0}", itemData.GetColorName());
                    }
                    else
                    {
                        desc = string.Format("{0}", itemData.GetColorName());
                    }
                    text.text = desc;
                }
            }
            else
            {
                if (resultData.bContinue)
                {
                    m_kBtnStop1.CustomActive(true);
                    m_kBtnContinue1.CustomActive(false);
                }
                else
                {
                    m_kBtnStop1.CustomActive(false);
                    m_kBtnContinue1.CustomActive(true);
                }
                m_kBtnStop1.CustomActive(false);
                m_kBtnContinue1.CustomActive(false);

                ComItemNew comItem = CreateComItemNew(Utility.FindChild(goFailed, "common_black/ItemParent"));
                m_arrComItems.Add(comItem);
                comItem.Setup(resultData.EquipData, null);
                Text name = Utility.FindComponent<Text>(goFailed, "common_black/itemname");

                string desc = string.Format("+{0} {1}", resultData.EquipData.StrengthenLevel, resultData.EquipData.GetColorName());
                name.text = desc;
            }
        }

        protected override void _OnCloseFrame()
        {
            //add by mjx on 170823 for limitTime gift to show
            LimitTimeGift.LimitTimeGiftFrameManager.GetInstance().WaitToShowLimitTimeGiftFrame();

            if(comItemZero != null)
            {
                comItemZero.Setup(null, null);
                comItemZero = null;
            }
            m_arrComItems.Clear();
           
            m_kBtnStop0.onClick.RemoveAllListeners();
            m_kBtnStop0 = null;
            m_kBtnStop1.onClick.RemoveAllListeners();
            m_kBtnStop1 = null;
            m_kBtnContinue0.onClick.RemoveAllListeners();
            m_kBtnContinue0 = null;
            m_kBtnContinue1.onClick.RemoveAllListeners();
            m_kBtnContinue1 = null;
        }

        [UIEventHandle("ok_10down/close")]
        void _OnClose()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("ok_10down/close_1")]
        void _OnClose_1()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("ok_10down/close_2")]
        void _OnClose_2()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("ok_10up/close")]
        void _OnUpClose()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("ok_10up/close_1")]
        void _OnUpClose_1()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("ok_10up/close_2")]
        void _OnUpClose_2()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("failed_broken/close")]
        void _OnClose2()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("failed_zero/close")]
        void _OnClose4()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("failed/close")]
        void _OnClose3()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("failed/close_1")]
        void _OnClose3_1()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("failed/close_2")]
        void _OnClose3_2()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
