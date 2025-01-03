using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

[RequireComponent(typeof(UniWebView))]
public class UniWebViewUtility : MonoBehaviour 
{
    public const string LoadingNote = "正在努力加载中...请稍候...";
    public Image webViewNormal;
    public Image webViewChanged;
    public GameObject loadingTextGo;

    private UniWebView mUniWebView;
    private Rect contentRect;
    private Rect contentRect2;

    private int bottomOffset;

    private bool needClearCache = false;
    private string currCacheUrl = "";
    private string errorCacheUrl = "";
    private bool beloadErrorUrl = false;

    private List<string> urlSchemeList = new List<string>();

    bool showLog = false;
    bool updateFlag = false;

    void Start()
    {
        //正式  关闭日志
        UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Off;
        showLog = false;
    }

    UniWebView CreateWebView()
    {
        var webView = this.GetComponent<UniWebView>();
        if (webViewNormal)
        {
            contentRect = webViewNormal.rectTransform.rect;
        }
        if (webViewChanged)
        {
            contentRect2 = webViewChanged.rectTransform.rect;
        }
        if (webView == null)
        {
            webView = webViewNormal.gameObject.AddComponent<UniWebView>();
        }
        if (webView != null)
        {
            webView.Frame = contentRect;
            webView.ReferenceRectTransform = webViewNormal.rectTransform;
           // webView.SetToolbarDoneButtonText(LoadingNote);
            webView.SetShowSpinnerWhileLoading(true);
            webView.SetSpinnerText(LoadingNote);
            webView.SetVerticalScrollBarEnabled(true);

            webView.BackgroundColor = new Color(1,1,1,0);
        }
        return webView;
    }

    public void InitWebView(bool needClearCache = false)
    {
        this.needClearCache = needClearCache;
        if (mUniWebView == null)
        {
            mUniWebView = CreateWebView();
            AddWebViewEventHandler();
        }
    }

    public void UnInitWebView()
    {
        if (mUniWebView != null)
        {
            RemoveWebViewEventHandler();
            //RemoveAllUrlSchemes();
            HideWebView();
            TryClearWebCache();

            PageStarted = null;
            PageFinished = null;
            PageErrorReceived = null;
            PageLoadReveiveMsg = null;

            mUniWebView = null;
        }
    }


    public void LoadUrl(string url)
    {
        if (mUniWebView != null)
        {
            mUniWebView.Load(url);
        }
    }

    public void ReLoadUrl()
    {
        if (mUniWebView != null)
        {
            mUniWebView.Reload();
        }
    }

    public void ShowWebView()
    {
        if (mUniWebView != null)
        {
            mUniWebView.Show();
        }
    }

    public void ShowWebViewFade()
    {
        if (mUniWebView != null)
        {
            mUniWebView.Show(true, UniWebViewTransitionEdge.None);
        }

    }

    public void HideWebView()
    {
        if (mUniWebView != null)
        {
            mUniWebView.Stop();
            mUniWebView.Hide();
        }
    }

    public void HideWebViewFade()
    {
        if (mUniWebView != null)
        {
            mUniWebView.Stop();
            mUniWebView.Hide(true, UniWebViewTransitionEdge.None);
        }
    }
    public void GoBackWebView()
    {
        if (mUniWebView != null)
        {
            if (mUniWebView.CanGoBack)
            {
                mUniWebView.GoBack();
            }
        }
    }

    public bool CanWebViewGoBack()
    {
        if (mUniWebView != null)
        {
            return mUniWebView.CanGoBack;
        }
        return false;
    }

    public void TweenMoveTo(float durationTime)
    {
        if (mUniWebView != null)
        {
            mUniWebView.AnimateTo(mUniWebView.Frame, durationTime);
        }
    }

    public void ResetWebViewShow(bool toOriginal, float duration)
    {
        if (mUniWebView != null)
        {
            if (toOriginal)
            {
                 mUniWebView.Frame = contentRect;
                 mUniWebView.ReferenceRectTransform = webViewNormal.rectTransform;
                 //Logger.Log("original ....." + contentRect + "  "+webViewNormal.rectTransform);
            }
            else
            {
                 contentRect2 = new Rect(contentRect2.x, contentRect2.y, contentRect2.width, contentRect.height - bottomOffset);
                 mUniWebView.Frame = contentRect2;
                 mUniWebView.ReferenceRectTransform = webViewChanged.rectTransform;
                 //Logger.Log("key board show .....");
            }

            //Update and set current frame of web view to match the setting.
            //This is useful if the referenceRectTransform is changed and you need to sync the frame change to the web view.
            //This method follows the frame determining rules.
            mUniWebView.UpdateFrame();          
        }
    }

    public void AddJS(string js, System.Action<UniWebViewNativeResultPayload> cb = null)
    {
        if (mUniWebView != null)
        {
            mUniWebView.AddJavaScript(js, cb);
        }
    }

    public void ExcuteJS(string js, System.Action<UniWebViewNativeResultPayload> cb = null)
    {
        if (mUniWebView != null)
        {
            mUniWebView.EvaluateJavaScript(js, cb);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (PageViewUpdate != null)
        {
            if (updateFlag)
            {
                PageViewUpdate();
                updateFlag = false;
            }
        }
    }


    void TryClearWebCache()
    {
        if (needClearCache && mUniWebView != null)
        {
            mUniWebView.CleanCache();
        }
    }

    void AddUrlSchemes(string scheme)
    {
        if (mUniWebView != null && !string.IsNullOrEmpty(scheme))
        {
            mUniWebView.AddUrlScheme(scheme);

            if (urlSchemeList == null)
                return;
            if (!urlSchemeList.Contains(scheme))
            {
                urlSchemeList.Add(scheme);
            }
        }
    }

    void RemoveAllUrlSchemes()
    {
        if (urlSchemeList == null)
            return;
        if (mUniWebView != null)
        {
            for (int i = 0; i < urlSchemeList.Count; i++)
            {
                mUniWebView.RemoveUrlScheme(urlSchemeList[i]);
            }           
        }
        urlSchemeList = null;
    }

    bool HasUrlScheme(string urlScheme)
    {
        if (urlSchemeList == null || urlSchemeList.Count == 0)
            return false;
        if (urlSchemeList.Contains(urlScheme))
        {
            return true;
        }
        return false;
    }

    void OnKeyboardShowOut(string res)
    {
        bool isKeyBoardShow = false;
        if (res.Equals("0"))
        {
            isKeyBoardShow = false;
        }
        else if (res.Equals("1"))
        {
            isKeyBoardShow = true;
        }
        if (isKeyBoardShow && SDKInterface.Instance.GetKeyboardSize() > 0)
        {
            bottomOffset = SDKInterface.Instance.GetKeyboardSize();
            //Logger.Log("offset = "+bottomOffset);
            ResetWebViewShow(false, 0.5f);
        }
        else
        {
            bottomOffset = 0;
            ResetWebViewShow(true, 0.5f);
            //Logger.Log("offset = " + bottomOffset);
        }
    }

    //请在这里处理游戏UI刷新
    public delegate void OnPageViewUpdate();
    [HideInInspector]
    public OnPageViewUpdate PageViewUpdate;

    //***下面的回调只能在内部使用 不要在这些回调里处理游戏UI***
    public delegate void OnPageStarted();
    private OnPageStarted PageStarted;
    public delegate void OnPageFinished();
    private OnPageFinished PageFinished;
    public delegate void OnPageErrorReceived();
    private OnPageErrorReceived PageErrorReceived;
    public delegate void OnPageLoadReceiveMsg(UniWebViewMessage msg);
    [HideInInspector]
    public OnPageLoadReceiveMsg PageLoadReveiveMsg;

    void AddWebViewEventHandler()
    {
        if (mUniWebView != null)
        {
            //当前版本 UniWebView 回调中不处理具体逻辑
            mUniWebView.OnPageStarted += OnPageStart;

            mUniWebView.OnPageFinished += OnPageFinish;

            mUniWebView.OnPageErrorReceived += OnPageError;

            mUniWebView.OnMessageReceived += OnPageMsgReceived;

            //返回键不会关闭网页
            mUniWebView.OnShouldClose += (view) =>
            {
                return false;
            };

            PluginManager.GetInstance().AddKeyboardShowListener(OnKeyboardShowOut);

            //if (PageStarted != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().RegisterEventHandler(GameClient.EUIEventID.OnWebViewLoadStart, OnLoadPageStarted);
            //}

            //if (PageFinished != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().RegisterEventHandler(GameClient.EUIEventID.OnWebVieewLoadFinish, OnLoadPageFinished);
            //}

            //if (PageErrorReceived != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().RegisterEventHandler(GameClient.EUIEventID.OnWebViewLoadError, OnLoadPageError);
            //}
        }
    }

    void RemoveWebViewEventHandler()
    {
        if(mUniWebView != null)
        {
            //当前版本 UniWebView 回调中不处理具体逻辑
            mUniWebView.OnPageStarted -= OnPageStart;

            mUniWebView.OnPageFinished -= OnPageFinish;

            mUniWebView.OnPageErrorReceived -= OnPageError;

            mUniWebView.OnMessageReceived -= OnPageMsgReceived;

            PluginManager.GetInstance().RemoveKeyboardShowListener();

            //if (PageStarted != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().UnRegisterEventHandler(GameClient.EUIEventID.OnWebViewLoadStart, OnLoadPageStarted);
            //}

            //if (PageFinished != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().UnRegisterEventHandler(GameClient.EUIEventID.OnWebVieewLoadFinish, OnLoadPageFinished);
            //}

            //if (PageErrorReceived != null)
            //{
            //    GameClient.UIEventSystem.GetInstance().UnRegisterEventHandler(GameClient.EUIEventID.OnWebViewLoadError, OnLoadPageError);
            //}
        }
    }


    void OnLoadPageStarted(GameClient.UIEvent uiEvent)
    {
        if (PageStarted != null)
        {
            PageStarted();
        }
    }

    void OnLoadPageFinished(GameClient.UIEvent uiEvent)
    {
        if (PageFinished != null)
        {
            PageFinished();
        }
    }

    void OnLoadPageError(GameClient.UIEvent uiEvent)
    {
        if (PageErrorReceived != null)
        {
            PageErrorReceived();
        }
    }


    void OnPageMsgReceived(UniWebView view, UniWebViewMessage message)
    {
        WebViewLog("OnPageMsgReceived : " + message.RawMessage);
        if (message.Equals(null))
            return;

        WebViewLog("OnPageMsgReceived rec scheme = " + message.Scheme);
        WebViewLog("OnPageMsgReceived rec path = " + message.Path);
        WebViewLog("OnPageMsgReceived rec rawMessage = " + message.RawMessage);

        if (PageLoadReveiveMsg != null)
        {
            PageLoadReveiveMsg(message);
        }
    }
    
    void OnPageStart(UniWebView view, string loadingUrl)
    {
        //建议不要尝试在sdk回调里处理主线程UI逻辑 ！！！
        //if (PageStarted != null)
        //{
        //    PageStarted();
        //}

        //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnWebViewLoadStart);

        //SetLoadingTextActive(true);

        //if (string.IsNullOrEmpty(loadingUrl))
        //    return;

        //int schemeIndex = loadingUrl.IndexOf("://");
        //string urlScheme = loadingUrl.Substring(0, schemeIndex);

        //if (!urlScheme.Equals("http") && !urlScheme.Equals("https"))
        //{
        //    if (HasUrlScheme(urlScheme) == false)
        //    {
        //        beloadErrorUrl = true;
        //    }
        //    AddUrlSchemes(urlScheme);
        //    WebViewLog("OnPageStart add url scheme : " + urlScheme);
        //}

        //currCacheUrl = loadingUrl;

        WebViewLog(" OnPageStart Loading start url = " + loadingUrl);
    }
    void OnPageFinish(UniWebView view, int statusCode,string url)
    {
        //建议不要尝试在sdk回调里处理主线程UI逻辑 ！！！
        //if (PageFinished != null)
        //{
        //    PageFinished();
        //}

        //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnWebVieewLoadFinish);

        //SetLoadingTextActive(false);

        if (statusCode == 200)
        {
            updateFlag = true;
        }

        WebViewLog("OnPageFinished error url : " + errorCacheUrl);

        //if (!string.IsNullOrEmpty(errorCacheUrl) && view != null)
        //{
        //    errorCacheUrl = "";
        //    beloadErrorUrl = false;
        //}
        WebViewLog("OnPageFinished success = " + statusCode + " url = " + url);
    }
    void OnPageError(UniWebView view, int errorCode,string errorMsg)
    {
        //建议不要尝试在sdk回调里处理主线程UI逻辑 ！！！
        //if (PageErrorReceived != null)
        //{
        //    PageErrorReceived();
        //}

        //GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnWebViewLoadError);

        //SetLoadingTextActive(true);

        //if (view != null)
        //{
        //    if (beloadErrorUrl)
        //    {
        //        errorCacheUrl = currCacheUrl;
        //        if (view.CanGoBack)
        //        {
        //            view.GoBack();
        //        }
        //    }
        //}

        WebViewLog("OnPageErrorReceived error url : " + errorCacheUrl);
        WebViewLog("OnPageErrorReceived errorCode = " + errorCode + " errorMsg = " + errorMsg);
    }

    void SetLoadingTextActive(bool active)
    {
        //if (loadingTextGo)
        //{
        //    loadingTextGo.CustomActive(active);
        //}
    }

    void WebViewLog(string msg)
    {
        if (showLog)
        {
            Debug.Log(msg);
        }
    }
}
