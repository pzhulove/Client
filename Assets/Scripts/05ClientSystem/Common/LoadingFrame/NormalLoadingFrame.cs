using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class NormalLoadingFrame : ClientFrame
    {
        public static int iSceneIndex;
        public static int iLoadSpeed = 10;
        [UIControl("loading")]
        Slider loadProcess;
        [UIControl("loading/loadingText")]
        Text loadText;

        StringBuilder strBuilder;
      
        void SetProcess(int iProcess)
        {
            loadProcess.value = iProcess / 100.0f;
            strBuilder.Clear();
            strBuilder.AppendFormat("读取中...{0}%", iProcess);
            loadText.text = strBuilder.ToString();
        }

        protected sealed override void _OnOpenFrame()
        {
            strBuilder = StringBuilderCache.Acquire();
            StartCoroutine(LoadStartLoading());
        }

        protected sealed override void _OnCloseFrame()
        {
            StringBuilderCache.Release(strBuilder);
        }

        delegate bool next();

        public IEnumerator LoadStartLoading()
        {
            AsyncOperation op = ClientApplication.LoadLevelAsync(iSceneIndex);

            int curProcess = 0;
            int displayProcess = 0;

            op.allowSceneActivation = false;

            next next_func = () =>
            {
                while ( displayProcess < curProcess || curProcess == 0 )
                {
                    displayProcess += iLoadSpeed;
                    displayProcess = Mathf.Min(displayProcess, curProcess);
                    SetProcess(displayProcess);
                    return false;
                }

                return true;
            };

            while (op.progress < 0.9f)
            {
                curProcess = (int)(op.progress * 100);
                if (next_func() == false)
                {
                    yield return Yielders.EndOfFrame;
                }
                else
                {
                    break;
                }
            }


            curProcess = 100;
            while (next_func() == false)
            {
                yield return Yielders.EndOfFrame;
            }

            op.allowSceneActivation = true;
            ClientApplication.ops = null;
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Loading/LoadingFrame";
        }
    }
}
