using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tenmove.Runtime;
using Tenmove.Runtime.Unity;
using UnityEngine;

public static class ScriptHelper
{
    private static Dictionary<string, byte[]> m_ScriptData = new Dictionary<string, byte[]>();

    public static void Clear()
    {
        m_ScriptData.Clear();
    }

    public static byte[] GetData(string scriptName)
    {
#if UNITY_EDITOR
        if(!Application.isPlaying)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Path.ChangeExtension(scriptName, null));
            if(textAsset != null)
            {
                return textAsset.bytes;
            }

            return null;
        }
        else
#endif
        {
            byte[] scriptData = null;
            if(!m_ScriptData.TryGetValue(scriptName, out scriptData))
            {
                var ins = AssetLoader.instance.LoadRes(scriptName, typeof(TextAsset));
                var textAsset = ins.obj as TextAsset;

                if (textAsset != null)
                {
                    scriptData = textAsset.bytes;
                }
                m_ScriptData.Add(scriptName, scriptData);
            }

            return scriptData;
        }
    }

    private static Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        TextAsset textAsset = asset as TextAsset;

        byte[] scriptData = null;

        if (textAsset != null)
        {
            scriptData = textAsset.bytes;
        }

        m_ScriptData.Add(assetPath, scriptData);

        var OnLoad = userData as Action<byte[]>;
        if (null != OnLoad)
        {
            OnLoad(scriptData);
        }
    }

    static void _OnLoadAssetFailure(string assetPath, int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
    }


    public static void GetDataAsync(string scriptName, Action<byte[]> OnLoad)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Path.ChangeExtension(scriptName, null));
            OnLoad(textAsset != null ? textAsset.bytes : null);
        }
        else
#endif
        {
            //byte[] scriptData = GetData(scriptName);
            //OnLoad(scriptData);

            byte[] scriptData = null;
            if (!m_ScriptData.TryGetValue(scriptName, out scriptData))
            {
                AssetLoader.LoadResAsync(scriptName, typeof(TextAsset), m_AssetLoadCallbacks, OnLoad);
            }
            else
            {
                OnLoad(scriptData);
            }
        }
    }

    public static void GetHeader(string path, Action<byte[]> OnLoad)
    {
        var data = ScriptHelper.GetData(path);
        if (null != OnLoad)
        {
            OnLoad(data);
        }
    }

    /// <summary>
    /// 如果是全部加载模式，返回大文件的byte数据和请求数据的偏移和长度。
    /// 如果是stream模式，返回请求数据byte。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static byte[] GetScript(string path, int offset, int len, ref int startIndex, ref int length)
    {
        startIndex = offset;
        length = len;

        return ScriptHelper.GetData(path);
    }

    public static byte[] DecodeLZ4(byte[] inbuffer, int inputOffset, int inputLength, int outLength)
    {
        return LZ4.LZ4Codec.Decode(inbuffer, inputOffset, inputLength, outLength);
    }

    public static byte[] EncodeLZ4(byte[] inbuffer, int inputOffset, int inputLength)
    {
        return LZ4.LZ4Codec.Encode(inbuffer, inputOffset, inputLength);
    }
}
