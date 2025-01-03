using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using System.IO;

namespace GameClient
{
    public class GetItemEffectFrame : ClientFrame
    {
        class ItemDataEx
        {
            public ItemData itemData;
            public bool bHighValue;
        }
        [UIObject("Items/Item")]
        GameObject goItem = null;

        [UIObject("Items/ScrollView/ViewPort/Content")]
        GameObject goContent = null;

        [UIControl("Items/ScrollView")]
        ScrollRect m_scrollRect;

        [UIObject("EffUI_gongxihuode_guangyun")]
        GameObject m_effGuanYun;

        [UIObject("EffUI_gongxihuode_xingguang")]
        GameObject m_effXingGuang;

        [UIControl("btnExit")]
        Button m_btnExit;

        [UIObject("Items")]
        GameObject goItems = null;

        [UIObject("Desc")]
        GameObject goDesc = null;

        [UIObject("BG (3)")]
        GameObject goBg3 = null;
   
        Button btSnap = null;
        Text btSnapText = null;
        List<GetItemEffectItem> m_arrEffecItem = new List<GetItemEffectItem>();    

        float fScrollPos = 0.0f;
        bool bUpdatePos = false;       

        List<ItemDataEx> m_arrGetItemInfo = new List<ItemDataEx>();
        bool bHaveHighValueItem = false;

        float fFrameCreateTime = Time.realtimeSinceStartup;
        const float fInterval1 = 0.2f;
        const float fInterval2 = 0.1f;        
        const float fDelayTime = 0.5f;
        float fTimeElapsed = 0.0f;
        float fTimeElapsedDelay = 0.0f; 
        float fInterval = 0.0f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/GetItemEffect";
        }     

        void SetObjChangeWidthTween(string strPath,float fDelta)
        { 
            GameObject obj = Utility.FindChild(GetFrame(), strPath);
            if (obj != null)
            {
                Tweener tweener = DOTween.To(
                    () =>
                    {
                        if(obj != null)
                        {
                            return obj.GetComponent<RectTransform>().sizeDelta.y;
                        }
                        else
                        {
                            return 0.0f;
                        }
                        
                    },
                    (y) =>
                    {
                        if(obj != null)
                        {
                            Vector2 vec = obj.GetComponent<RectTransform>().sizeDelta;
                            vec.y = y;
                            obj.GetComponent<RectTransform>().sizeDelta = vec;
                        }
                        
                    },
                    obj.GetComponent<RectTransform>().sizeDelta.y + fDelta,
                    0.2f);
                tweener.SetEase(Ease.OutBack);
            }
        }

        public void AddNewItem(ItemData kItem, bool bHighValue = false)
        {
            if(kItem == null)
            {
                return;
            }
            
            if(m_arrGetItemInfo == null)
            {
                return;
            }

            ItemDataEx itemDataEx = new ItemDataEx();
            itemDataEx.itemData = kItem;
            itemDataEx.bHighValue = bHighValue;
            m_arrGetItemInfo.Add(itemDataEx);
            if(bHighValue)
            {
                bHaveHighValueItem = true;
            }
            return; 
        }        

        protected override void _OnOpenFrame()
        {
            BindUIEvent();

            fFrameCreateTime = Time.realtimeSinceStartup;
            fTimeElapsed = 0.0f;
            fTimeElapsedDelay = 0.0f;
            fScrollPos = 0.0f;
            bUpdatePos = false;
            fInterval = fInterval1;
            bHaveHighValueItem = false;

            if(goItem != null)
            {
                goItem.CustomActive(false);
            }

            if (m_effXingGuang != null)
            {
                m_effXingGuang.CustomActive(false);
            }

            if (m_effGuanYun != null)
            {
                m_effGuanYun.CustomActive(false);
            }

            if(m_btnExit != null)
            {
                m_btnExit.onClick.RemoveAllListeners();
                m_btnExit.onClick.AddListener(() => 
                {
                    if(Time.realtimeSinceStartup - fFrameCreateTime < 1.0f)
                    {
                        return;
                    }

                    frameMgr.CloseFrame<GetItemEffectFrame>();     
                });
            }   
            InvokeMethod.Invoke(this, fDelayTime, () => 
            {
                if(m_arrGetItemInfo.Count <= 8)
                {
                    fInterval = fInterval1;
                }
                else
                {
                    fInterval = fInterval2;
                }
            });
            if(btSnap != null)
            {
                btSnap.CustomActive(false);
            }
        }

        void _UpdateEffect()
        {
            if (m_effXingGuang != null)
            {
                m_effXingGuang.CustomActive(true);
            }

            if (m_effGuanYun != null)
            {
                m_effGuanYun.CustomActive(true);
            }

            if(m_arrGetItemInfo == null)
            {
                return;
            }

            if (m_arrGetItemInfo.Count > 0)
            {
                ItemData kItem = m_arrGetItemInfo[0].itemData;
                if (kItem != null && goItem != null && goContent != null)
                {
                    GameObject goCurrent = GameObject.Instantiate(goItem);
                    Utility.AttachTo(goCurrent, goContent);
                    goCurrent.CustomActive(true);

                    GetItemEffectItem item = goCurrent.GetComponent<GetItemEffectItem>();
                    if (item != null)
                    {
                        if (goItems != null)
                        {
                            if (goContent.transform.childCount <= 4)
                            {

                            }
                            else if (goContent.transform.childCount <= 8)
                            {
                                Vector2 vec2 = goItems.gameObject.GetComponent<RectTransform>().sizeDelta;
                                vec2.y = 400;
                                goItems.gameObject.GetComponent<RectTransform>().sizeDelta = vec2;
                            }
                            else
                            {
                                Vector2 vec2 = goItems.gameObject.GetComponent<RectTransform>().sizeDelta;
                                vec2.y = 600;
                                goItems.gameObject.GetComponent<RectTransform>().sizeDelta = vec2;
                            }

                            if (goContent.transform.childCount == 5)
                            {
                                Vector3 vec3 = goItems.transform.localPosition;
                                vec3.y += 50;
                                goItems.transform.localPosition = vec3;
                            }
                        }


                        if (goContent.transform.childCount == 5)
                        {
                            SetObjChangeWidthTween("BG (3)", 100.0f);
                        }
                        else if (goContent.transform.childCount == 9)
                        {
                            SetObjChangeWidthTween("BG (3)", 205.0f);
                        }

                        if ((goContent.transform.childCount - 9) % 4 == 0 && goContent.transform.childCount > 1)
                        {
                            Tweener tweener = DOTween.To(() => 
                            {
                                if(m_scrollRect != null)
                                {
                                    return m_scrollRect.normalizedPosition.y;
                                }
                                else
                                {
                                    return 0.0f;
                                }
                                
                            }, 
                            (float y) => { fScrollPos = y; }, 
                            0.0f, 
                            0.2f);

                            bUpdatePos = true;
                            tweener.OnComplete(() =>
                            {
                                bUpdatePos = false;
                            });
                        }

                        item.SetUp(kItem, 0.0f, this, m_arrGetItemInfo[0].bHighValue);
                        m_arrEffecItem.Add(item);
                    }
                }
                m_arrGetItemInfo.RemoveAt(0);
                
                if(m_arrGetItemInfo.Count == 0)
                {
                    if (bHaveHighValueItem)
                    {
                        //if (btSnap != null)
                        //{
                        //    btSnap.CustomActive(true);
                        //}
                    }
                }
            }

            return;
        }

        protected override void _OnCloseFrame()
        {
            bHaveHighValueItem = false;
            InvokeMethod.RemoveInvokeCall(this);
            m_arrGetItemInfo.Clear();       
            
            for(int i = 0;i < m_arrEffecItem.Count;i++)
            {
                if(m_arrEffecItem[i].gameObject != null)
                {
                    GameObject.Destroy(m_arrEffecItem[i].gameObject);
                    m_arrEffecItem[i] = null;
                }                
            }
            m_arrEffecItem.Clear();

            UnBindUIEvent();
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeElapsedDelay += timeElapsed;
            fTimeElapsed += timeElapsed;

            if(fTimeElapsedDelay >= fDelayTime)
            {
                if (fTimeElapsed >= fInterval)
                {
                    fTimeElapsed = 0.0f;
                    _UpdateEffect();
                }
            }       

            if (goDesc != null && goBg3 != null)
            {
                Vector3 vecPos1 = goDesc.transform.localPosition;
                Vector3 vecPos2 = goBg3.transform.localPosition;

                //vecPos1 = vecPos2;
                vecPos1.y = vecPos2.y - goBg3.transform.GetComponent<RectTransform>().sizeDelta.y - 40;

                goDesc.transform.localPosition = vecPos1;
            }

            if(bUpdatePos)
            {       
                m_scrollRect.normalizedPosition = new Vector2(m_scrollRect.normalizedPosition.x,fScrollPos);                
            }            
        }

        //截屏并保存
        void CaptureSnap(string name)
        {
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            if(tex == null)
            {
                return;
            }
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();            
            File.WriteAllBytes(name, bytes);
            return;  
        }
        protected override void _bindExUI()
        {
            btSnap = mBind.GetCom<Button>("btSnap");
            if(btSnap != null)
            {
                btSnap.onClick.RemoveAllListeners();
                btSnap.onClick.AddListener(() => 
                {
                    UIGray gray = btSnap.gameObject.SafeAddComponent<UIGray>(false);
                    if(gray != null)
                    {
                        gray.enabled = true;
                    }
                    if(btSnapText != null)
                    {
                        btSnapText.text = TR.Value("haveSnapPic");
                    }
                    if(btSnap != null)
                    {
                        btSnap.interactable = false;
                        btSnap.image.raycastTarget = false;
                    }

                    TakePhotoModeFrame.MobileScreenShoot();
                    //string path = Utility.GetWriteablePath() + "ScreenShots";
                    //if(!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}

                    //string filePath = string.Format("{0}/{1}.png",path, string.Format("{0:yyyy-MM-dd-HH_mm_ss_ffff}", System.DateTime.Now));
                    //if(Application.platform == RuntimePlatform.Android)
                    //{
                    //    CaptureSnap(filePath);
                    //}
                    //else
                    //{
                    //    Application.CaptureScreenshot(filePath, 0); // 安卓平台下该接口无效
                    //}

                    //string noticeMsg = TR.Value("SnapPicOK", filePath);                    
                    //ChatManager.GetInstance().AddLocalMsg(noticeMsg, ChanelType.CHAT_CHANNEL_SYSTEM);
                });
            }
            btSnapText = mBind.GetCom<Text>("btSnapText");
        }
        protected override void _unbindExUI()
        {
            btSnap = null;
            btSnapText = null;
        }


        #region set sibling on specialframe

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FirstPayFrameOpen, _OnFirstPayFrameOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SecondPayFrameOpen, _OnSecondPayFrameOpen);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FirstPayFrameOpen, _OnFirstPayFrameOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SecondPayFrameOpen, _OnSecondPayFrameOpen);
        }

        void _OnFirstPayFrameOpen(UIEvent uiEvent)
        {
            var firstPayFrame = ClientSystemManager.GetInstance().GetFrame(typeof(FirstPayFrame)) as FirstPayFrame;
            if (firstPayFrame == null)
            {
                return;
            }
            var firstPayframeObj = firstPayFrame.GetFrame();
            if (firstPayFrame != null && firstPayframeObj != null)
            {
                this.frame.transform.SetSiblingIndex(firstPayframeObj.transform.GetSiblingIndex() + 1);
            }
        }
        void _OnSecondPayFrameOpen(UIEvent uiEvent)
        {
            var secondPayFrame = ClientSystemManager.GetInstance().GetFrame(typeof(SecondPayFrame)) as SecondPayFrame;
            if (secondPayFrame == null)
            {
                return;
            }
            var secondPayframeObj = secondPayFrame.GetFrame();
            if (secondPayFrame != null && secondPayframeObj != null)
            {
                this.frame.transform.SetSiblingIndex(secondPayframeObj.transform.GetSiblingIndex() + 1);
            }
        }

        #endregion
    }
}
