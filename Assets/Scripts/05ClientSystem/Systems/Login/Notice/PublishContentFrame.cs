using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;
using Scripts.UI;

namespace GameClient
{
    public class NoticeToggleData
    {
        public int id;
        public int index;
        public string toggleName;
        public int isRedPoint;
    }

    public class PublishContentFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/Publish/Publish";
        }

#region ExtraUIBind
        private Button mClose = null;
        private LinkParse mText = null;
        private ComUIListScript mComUIScirpt = null;
        private CommonBriefTabToggleGroup mCommonBriefTabToggleGroup = null;
        private Text mTestInDec = null;
        private GameObject mTips = null;

        private Text mCalText = null;

        private NoticeData mNoticeData;
        public Vector2 mDefaultSize = new Vector2(1122, 44);

        private List<NoticeToggleData> mNoticeToggleDatas = new List<NoticeToggleData>();
        private List<CommonBriefTabData> mCommonBriefTabDatas = new List<CommonBriefTabData>();
        private Dictionary<int, List<string>> mNoticeContents = new Dictionary<int, List<string>>();
        private List<string> mRichTextStartStrs = new List<string>();       //富文本开始字符串列表
        private Dictionary<int, List<int>> mDicTitleLineIndexs = new Dictionary<int, List<int>>();               //标题行索引
        private Dictionary<int, List<int>> mDicRowSpacingLineIndexs = new Dictionary<int, List<int>>();          //需要特别设置行间距的行索引
        private List<Vector2> mContentLineSizes = new List<Vector2>();

        private int mCurIndex = -1;
        private Dictionary<int, bool> mDicTheIdIsRequestIng = new Dictionary<int, bool>();          //key 表示页签id  value 表示是否在请求该页签对应的内容中
        List<string> listNoticeText;

        // private string mTitleUrlFormat = "http://{0}/title_list?channel={1}";
        private string mTitleUrlFormat = "http://{0}/index/title?channel={1}";
        private string mContentUrlFormat = "http://{0}/index/content?channel={1}&id={2}";

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mText = mBind.GetCom<LinkParse>("text");
            mComUIScirpt = mBind.GetCom<ComUIListScript>("comUIScirpt");
            mCommonBriefTabToggleGroup = mBind.GetCom<CommonBriefTabToggleGroup>("CommonVerticalWindowBriefTab");
            mNoticeData = mBind.GetCom<NoticeData>("noticedata");

            mTestInDec = mBind.GetCom<Text>("testIndec");
            mTips = mBind.GetGameObject("tips");

            mCalText = mBind.GetCom<NewSuperLinkText>("calText");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mText = null;
            mComUIScirpt = null;
            mCommonBriefTabToggleGroup = null;
            mCalText = null;
            mNoticeData = null;

            mTestInDec = null;
            mTips = null;
        }
#endregion

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */

            ClientSystemManager.instance.CloseFrame(this);
        }

#endregion

        private UnityEngine.Coroutine mCo = null;

        private void _stop()
        {
            mTips.CustomActive(false);

            if (null != mCo)
            {
                GameFrameWork.instance.StopCoroutine(mCo);
            }

            mCo = null;
        }

        protected override void _OnOpenFrame()
        {
            listNoticeText = null;

            if (mComUIScirpt != null)
            {
                mComUIScirpt.Initialize();
                mComUIScirpt.onItemVisiable = _OnContentItemVisiableDelegate;
                mComUIScirpt.OnItemUpdate = _OnContentItemVisiableDelegate;
            }

            _stop();
            mCo = GameFrameWork.instance.StartCoroutine(_wt());
#if MG_TESTIN
            if (mTestInDec != null)
            {
                mTestInDec.gameObject.SetActive(true);
                mTestInDec.text = ShowTestInDec();
            }
#endif
        }

        private void _OnContentItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (listNoticeText == null || item.m_index >= listNoticeText.Count)
            {
                return;
            }

            int idCache = _GetToggleIdByIndex(mCurIndex);
            var script = item.GetComponent<NoticeContentItem>();
            if (script != null)
            {
                bool isTitleLine = _IsTitleLine(item.m_index, idCache);
                script.Init(listNoticeText[item.m_index],this,isTitleLine);
            }
        }

        protected override void _OnCloseFrame()
        {
            _stop();
            listNoticeText = null;
            mNoticeToggleDatas.Clear();
            mCommonBriefTabDatas.Clear();
            mNoticeContents.Clear();
            mRichTextStartStrs.Clear();
            mDicTitleLineIndexs.Clear();
            mDicRowSpacingLineIndexs.Clear();
            mDicTheIdIsRequestIng.Clear();
            mCurIndex = -1;
    }

        private IEnumerator _wt()
        {
            mTips.CustomActive(true);

            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            string channelName = SDKInterface.Instance.GetPlatformNameByChannel();
            wt.url = string.Format(mTitleUrlFormat, Global.NOTICE_SERVER_ADDRESS, channelName);

            yield return wt;

            var content = wt.GetResultString();
            mNoticeToggleDatas.Clear();
            object jsonData = MiniJSON.jsonDecode(content);

            if (jsonData == null)
            {
                Logger.LogErrorFormat("公告toggle内容json转化为空");
                yield break;
            }

            if (!(jsonData is ArrayList))
            {
                Logger.LogErrorFormat("jsonData格式错误，不为ArrayList，其格式现在为:{0}", jsonData.GetType());
                yield break;
            }

            ArrayList jsonObjects = jsonData as ArrayList;
            foreach (var v in jsonObjects)
            {
                if (!(v is Hashtable))
                {
                    Logger.LogErrorFormat("v格式错误，不为Hashtable，其格式现在为:{0}", v.GetType());
                    continue;
                }

                Hashtable noticeDataDic = v as Hashtable;
                NoticeToggleData noticeToggleData = new NoticeToggleData();
                int.TryParse(noticeDataDic.GetValue("id"), out noticeToggleData.id);
                noticeToggleData.toggleName = noticeDataDic.GetValue("title").ToString();
                int.TryParse(noticeDataDic.GetValue("red_point"), out noticeToggleData.isRedPoint);
                int.TryParse(noticeDataDic.GetValue("index"), out noticeToggleData.index);

                mNoticeToggleDatas.Add(noticeToggleData);
            }

            mNoticeToggleDatas.Sort(_SortNoticeToggleList);
            mCommonBriefTabDatas.Clear();
            foreach (var data in mNoticeToggleDatas)
            {
                if (data == null)
                {
                    continue;
                }

                CommonBriefTabData tabData = new CommonBriefTabData();
                tabData.id = data.id;
                tabData.tabName = data.toggleName;
                mCommonBriefTabDatas.Add(tabData);
            }

            if (mCommonBriefTabToggleGroup != null)
            {
                mCommonBriefTabToggleGroup.InitComTab(_BriefTabToggleGroupSelect, -1, mCommonBriefTabDatas);
            }
            //if (mToggleComUIList != null)
            //{
            //    mToggleComUIList.UpdateElementAmount(_GetNoticeToggleDatas().Count);
            //}
            //_SelectToggle(0);
        }

        private void _BriefTabToggleGroupSelect(CommonBriefTabData data)
        {
            if (data == null || mCommonBriefTabToggleGroup == null)
            {
                return;
            }

            mCurIndex = mCommonBriefTabToggleGroup.GetIndexById(data.id);
            _InitContent();
        }

        private int _SortNoticeToggleList(NoticeToggleData left, NoticeToggleData right)
        {
            return left.index - right.index;
        }

        private void _InitContent()
        {
            if (mCurIndex == -1)
            {
                return;
            }

            int id = _GetToggleIdByIndex(mCurIndex);
            listNoticeText = _GetContents(id);
            if (listNoticeText == null || listNoticeText.Count <= 0)
            {
                if (!_IsTheIdContentResquesting(id))
                {
                    _RequestContent(id);
                }
            }
            else
            {
                _SetContent();
            }
        }

        private bool _IsTheIdContentResquesting(int id)
        {
            if (mDicTheIdIsRequestIng.ContainsKey(id))
            {
                return mDicTheIdIsRequestIng[id];
            }

            return false;
        }

        private void _SetContent()
        {
            if (listNoticeText != null && mComUIScirpt != null)
            {
                _InitContentLineSizes();

                mComUIScirpt.SetElementAmount(listNoticeText.Count, mContentLineSizes);
                mComUIScirpt.ScrollToItem(0);
            }
        }

        private void _InitContentLineSizes()
        {
            if (listNoticeText == null)
            {
                return;
            }

            if (mContentLineSizes == null)
            {
                mContentLineSizes = new List<Vector2>();
            }

            mContentLineSizes.Clear();

            for (int i = 0; i < listNoticeText.Count; i++)
            {
                int idCache = _GetToggleIdByIndex(mCurIndex);
                if (mNoticeData == null)
                {
                    mContentLineSizes.Add(mDefaultSize);
                }
                else
                {
                    if (_IsRowSpacingLine(i, idCache))
                    {
                        mContentLineSizes.Add(mNoticeData.specialLineSize);
                    }
                    else
                    {
                        mContentLineSizes.Add(mNoticeData.normalLineSize);
                    }
                }
            }
        }

        private void _RequestContent(int id)
        {
            if (HttpClient.Instance == null)
            {
                return;
            }

            string channelName = SDKInterface.Instance.GetPlatformNameByChannel();
            int idCache = id;
            if (mDicTheIdIsRequestIng.ContainsKey(idCache))
            {
                mDicTheIdIsRequestIng[idCache] = true;
            }
            else
            {
                mDicTheIdIsRequestIng.Add(idCache, true);
            }
            mTips.CustomActive(true);
            HttpClient.Instance.GetRequest(string.Format(mContentUrlFormat, Global.NOTICE_SERVER_ADDRESS, channelName, idCache), (WWW wwwReq) =>
            {
                mTips.CustomActive(false);

                if (this == null)
                {
                    return;
                }

                if (mDicTheIdIsRequestIng.ContainsKey(idCache))
                {
                    mDicTheIdIsRequestIng[idCache] = false;
                }
                else
                {
                    mDicTheIdIsRequestIng.Add(idCache, false);
                }

                if (string.IsNullOrEmpty(wwwReq.error))
                {
                    string content = StringHelper.BytesToString(wwwReq.bytes);
                    if (mNoticeContents.ContainsKey(idCache))
                    {
                        mNoticeContents[idCache] = _GetContents(content, mCalText, idCache);
                    }
                    else
                    {
                        mNoticeContents.Add(idCache, _GetContents(content, mCalText, idCache));
                    }

                    _InitContent();
                }
                else
                {
                    Logger.LogProcessFormat("notice http error:{0}", wwwReq.error);
                }
            });
        }

        private List<string> _GetContents(string contents, Text textCal, int idCache)
        {
            List<string> strs = new List<string>();
            strs = textCal.GetTextContents(contents);

            for (int i = 0; i < strs.Count; i++)
            {
                strs[i] = _GetTitleLinesString(strs[i], i, idCache);
                strs[i] = _GetRowSpacintLinesString(strs[i], i, idCache);
                strs[i] = _ParseRichText(strs[i]);
            }

            return strs;
        }

        private string _GetTitleLinesString(string str, int index, int idCache)
        {
            if (str.Contains("@"))
            {
                if (mDicTitleLineIndexs.ContainsKey(idCache))
                {
                    mDicTitleLineIndexs[idCache].Add(index);
                }
                else
                {
                    List<int> list = new List<int>();
                    list.Add(index);
                    mDicTitleLineIndexs.Add(idCache, list);
                }
                return str.Replace("@", "");
            }

            return str;
        }

        private string _GetRowSpacintLinesString(string str, int index, int idCache)
        {
            if (str.Contains("&"))
            {
                if (mDicRowSpacingLineIndexs.ContainsKey(idCache))
                {
                    mDicRowSpacingLineIndexs[idCache].Add(index);
                }
                else
                {
                    List<int> list = new List<int>();
                    list.Add(index);
                    mDicRowSpacingLineIndexs.Add(idCache, list);
                }
                return str.Replace("&", "");
            }

            return str;
        }

        private string _ParseRichText(string str)
        {
            List<int> richTextEndIndex = new List<int>();
            List<string> richTextEndStrs = new List<string>();
            System.Text.RegularExpressions.Match matchEnd = System.Text.RegularExpressions.Regex.Match(str, "\\</[^\\{\\}]*?\\>");
            //提取富文本结束标记字符串列表和其开始字符索引标记
            while (matchEnd.Length > 0)
            {
                richTextEndIndex.Add(matchEnd.Index);
                richTextEndStrs.Add(matchEnd.Value);
                matchEnd = matchEnd.NextMatch();
            }

            int lineStartCount = 0;
            string strNoEndTag = str.Replace("</", "aa");   //将富文本结束标记的开始字符串替换，这样提取富文本开始标记时就不会重复提取富文本结束标记
            List<int> richTextStartIndex = new List<int>();
            List<string> richTextStartStrs = new List<string>();
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(strNoEndTag, "\\<[^\\{\\}]*?\\>");
            //提取富文本开始标记字符串列表和其开始字符索引列表
            while (match.Length > 0)
            {
                richTextStartIndex.Add(match.Index);
                richTextStartStrs.Add(match.Value);
                match = match.NextMatch();
                lineStartCount++;
            }

            //找到在该行开始该行结束的富文本开始索引
            int startIndex = -1;
            int endIndex = -1;
            int matchSucCount = 0;
            for (int i = 0; i < richTextEndIndex.Count; i++)
            {
                if (matchSucCount >= richTextStartIndex.Count)
                {
                    break;
                }

                if (richTextEndIndex[i] > richTextStartIndex[matchSucCount])
                {
                    matchSucCount++;
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    endIndex = i;
                }
            }

            //移除在本行开始本行结束的富文本
            int removeCount = 0;    //移除次数
            while (startIndex >= 0 && richTextEndStrs.Count > startIndex && removeCount <= endIndex - startIndex) //endindex - startindex 表示需要移除几次
            {
                richTextEndStrs.RemoveAt(startIndex);
                if (richTextStartStrs.Count > 0)
                {
                    richTextStartStrs.RemoveAt(0);// richTextStartStrs.Count - 1);
                }
                removeCount++;
            }

            //移除在前面几行开始在本行结束的富文本的结束标记字符串列表
            int endStrsCount = richTextEndStrs.Count;  //用于记录在前几行开始在本行结束的富文本结束标记数量
            for (int i = mRichTextStartStrs.Count - 1; i >= 0; i--)
            {
                if (richTextEndStrs.Count > 0)
                {
                    str = mRichTextStartStrs[i] + str;   //在str头部添加前面几行开始在本行结束的富文本开始标记字符串
                    richTextEndStrs.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            //移除在前面几行开始在本行结束的富文本的开始标记字符串列表
            for (int i = 0; i < endStrsCount; i++)
            {
                if (mRichTextStartStrs.Count > 0)
                {
                    mRichTextStartStrs.RemoveAt(mRichTextStartStrs.Count - 1);
                }
            }

            //添加在本行开始但未在本行结束的富文本的开始标记字符串列表
            mRichTextStartStrs.AddRange(richTextStartStrs);

            //添加前面几行开始本行还未结束的富文本标记字符串的开始或者结束标记
            int index = 0;
            for (int i = mRichTextStartStrs.Count - 1; i >= 0; i--)
            {
                if (richTextStartStrs.Count > index)
                {
                    str += _GetEndStr(mRichTextStartStrs[i]);
                }
                else
                {
                    str = mRichTextStartStrs[i] + str + _GetEndStr(mRichTextStartStrs[i]);
                }

                index++;
            }

            return str;
        }

        private string _GetEndStr(string startStr)
        {
            string str = string.Empty;
            str = startStr.Replace("<", "");
            string[] strs = str.Split('=');
            if (strs.Length > 0)
            {
                str = "</" + strs[0] + ">";
            }

            return str;
        }

        /// <summary>
        /// 获取页签列表
        /// </summary>
        /// <returns></returns>
        private List<NoticeToggleData> _GetNoticeToggleDatas()
        {
            List<NoticeToggleData> list = new List<NoticeToggleData>();
            if (mNoticeToggleDatas == null)
                return list;

            foreach (var v in mNoticeToggleDatas)
            {
                list.Add(v);
            }

            return list;
        }

        /// <summary>
        /// 根据id获取内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<string> _GetContents(int id)
        {
            List<string> contents = new List<string>();
            if (mNoticeContents == null)
            {
                return contents;
            }

            if (mNoticeContents.ContainsKey(id))
            {
                return mNoticeContents[id];
            }

            return contents;
        }

        private int _GetToggleIdByIndex(int index)
        {
            int id = -1;
            if (index < 0 || mNoticeToggleDatas.Count <= index)
                return id;

            return mNoticeToggleDatas[index].id;
        }

        /// <summary>
        /// 判断是否是标题行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool _IsTitleLine(int index, int idCache)
        {
            if (mDicTitleLineIndexs == null)
            {
                return false;
            }

            if (!mDicTitleLineIndexs.ContainsKey(idCache))
            {
                return false;
            }

            if (mDicTitleLineIndexs[idCache] == null)
            {
                return false;
            }

            return mDicTitleLineIndexs[idCache].Contains(index);
        }

        /// <summary>
        /// 判断是否是行间距要特殊处理的行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool _IsRowSpacingLine(int index, int idCache)
        {
            if (mDicRowSpacingLineIndexs == null)
            {
                return false;
            }

            if (!mDicRowSpacingLineIndexs.ContainsKey(idCache))
            {
                return false;
            }

            if (mDicRowSpacingLineIndexs[idCache] == null)
            {
                return false;
            }

            return mDicRowSpacingLineIndexs[idCache].Contains(index);
        }

#if MG_TESTIN
        string ShowTestInDec()
        {
            var TestInTableData = TableManager.GetInstance().GetTable<ProtoTable.TestInDes>();
            if (TestInTableData != null)
            {
                var enumerator = TestInTableData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ProtoTable.TestInDes item = enumerator.Current.Value as ProtoTable.TestInDes;
                    if (Global.Settings.sdkChannel.ToString().Equals(item.channel))
                    {
                        return item.des;
                    }
                }
            }
            return string.Empty;
        }
#endif
    }
}
