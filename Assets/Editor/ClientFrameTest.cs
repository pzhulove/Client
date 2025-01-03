using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using GameClient;

using System.Security.Cryptography;


public class ClientFrameTest
{
    private List<string> mClientFrames = new List<string>();
    private List<string> mClientSystems = new List<string>();

    [SetUp]
    public void Init()
    {
        mClientFrames.Clear();
        mClientSystems.Clear();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                var isClientFrame = type.IsSubclassOf(typeof(GameClient.ClientFrame));
                if (isClientFrame)
                {
                    mClientFrames.Add(type.ToString());
                }
                else
                {
                    var isys = type.GetInterface("GameClient.IClientSystem");
                    if (null != isys)
                    {
                        if (type.IsClass && null != type.BaseType.GetInterface("GameClient.IClientSystem"))
                        {
                            //Logger.LogError(type.ToString());
                            mClientSystems.Add(type.ToString());
                        }
                    }
                }
            }
        }

    }


    [Test]
    public void TestMd5()
    {
        {
            byte[] datatest = new byte[] {22, 24, 244, 242, 34, 234, 234, 23,4,234,2,34,234,23 ,42,34 ,234, 34,234};
            _testWithBytes(datatest);
        }

        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("test11111");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("asef23498u234u20347u902187348901705897208937508927508927348957");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("123901820318029371723897zxuvzxjcvlasdjfq9182u48912498017u4812");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("nearxcv2io3re");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("taisfdjisjdf");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("uasidfuiasljdfzxjvk");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("vjizxjvizxjv");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("asdfxcbbg9t48-210846=278=324872-49");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("324872-49");
            _testWithBytes(datatest);
        }
        {
            byte[] datatest = System.Text.Encoding.UTF8.GetBytes("324872-4:");
            _testWithBytes(datatest);
        }
    }

    private bool _testWithBytes(byte[] bytes)
    {
        MD5 originMD5 = MD5.Create();
        byte[] originData = originMD5.ComputeHash(bytes);
        byte[] targetData = DungeonUtility.GetMD5(bytes);

        if (!_isSame(originData, targetData))
        {
            UnityEngine.Debug.LogError("failed");
            return false;
        }

        return true;
    }

    private bool _isSame(byte[] src, byte[] target)
    {
        if (null == src || null == target)
        {
            return false;
        }

        if (src.Length != target.Length)
        {
            return false;
        }

        for (int i = 0; i < src.Length; ++i)
        {
            if (src[i] != target[i])
            {
                return false;
            }

        }

        return true;
    }


    private UnityEngine.Object _checkFileExist(string path)
    {
        var obj = Resources.Load(path);

        if (obj == null)
        {
            Logger.LogErrorFormat("{0} file not exits {1}", mCurrentFrame, path);
        }
        //Assert.AreNotEqual(null, obj, "file not exits " + path);
        return obj;
    }

    private GameObject _checkPrefabPath(string prefab)
    {
        var res = _checkFileExist(prefab);
        if (res == null)
        {
            return null;
        }

        var go = GameObject.Instantiate(res) as GameObject;

        if (go == null)
        {
            Logger.LogErrorFormat("{0} intance fail {1}", mCurrentFrame, prefab);
        }
        //Assert.AreNotEqual(null, go, "instance fail " + prefab);
        
        return go;
    }

    private void _checkGameObject(GameObject frame, string path, Type type = null, bool isArray = false, int basenum = 0, int arrayLen = 0)
    {
        if (type == null)
        {

            GameObject obj = Utility.FindGameObject(frame, path);

            if (null == obj)
            {
                Logger.LogErrorFormat("{0} can't find obj with path {1}", mCurrentFrame, path);
            }
            //Assert.AreNotEqual(null, obj, );
        }
        else
        {
            if (isArray)
            {
                for (int j = 0; j < arrayLen; ++j)
                {
                    var comPath = string.Format(path, j + basenum);

                    Component com = Utility.FindComponent(frame, type, comPath);

                    if (com == null)
                    {
                        Logger.LogErrorFormat("{0} can't find com with path {1} {2}", mCurrentFrame, comPath, type);
                    }
                    //Assert.AreNotEqual(null, com, string.Format("can't find com with path {0} {1}", comPath, type));
                }
            }
            else
            {
                Component com = Utility.FindComponent(frame, type, path);
                if (com == null)
                {
                    Logger.LogErrorFormat("{0} can't find com with path {1} {2}", mCurrentFrame, path, type);
                }
                //Assert.AreNotEqual(null, com, string.Format("can't find com with path {0} {1}", path, type));
            }
        }
    }

    private void _checkClienFrameAttributes(GameObject frame, Type frameType, object instance)
    {
        FieldInfo[] fieldInfo = frameType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        for (int i = 0; i < fieldInfo.Length; ++i)
        {
            var current = fieldInfo[i];
            object[] oats = current.GetCustomAttributes(typeof(UIObjectAttribute), false);

            if (oats.Length > 0)
            {
                _checkGameObject(frame, (oats[0] as UIObjectAttribute).objectName);

                continue;
            }

            oats = current.GetCustomAttributes(typeof(UIControlAttribute), false);
            if (oats.Length > 0)
            {
                var attr = oats[0] as UIControlAttribute;
                var arrayLen = 0;
                if (current.FieldType.IsArray)
                {
                    var array = current.GetValue(instance) as Array;
                    arrayLen = array.Length;
                }

                _checkGameObject(frame, attr.controlName, attr.componentType, current.FieldType.IsArray, attr.baseNum, arrayLen);
            }
        }

        MethodInfo[] methods = frameType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        for (int i = 0; i < methods.Length; ++i)
        {
            var current = methods[i];

            object[] oats = current.GetCustomAttributes(typeof(UIFrameSound), false);
            if (oats.Length > 0)
            {
                var frameSound = oats[0] as UIFrameSound;
                if (frameSound != null)
                {
                    _checkFileExist(frameSound.sound);
                }
            }

            oats = current.GetCustomAttributes(typeof(UIEventHandleAttribute), false);

            if (oats.Length > 0)
            {
                var eventAttr = oats[0] as UIEventHandleAttribute;
                //单个
                if (eventAttr.start == 0 && eventAttr.end == 0)
                {
                    _checkGameObject(frame, eventAttr.controlName, eventAttr.controlType, false, 0, 0);
                }
                else
                {
                    _checkGameObject(frame, eventAttr.controlName, eventAttr.controlType, true, eventAttr.start, eventAttr.end);
                    /*
                    StringBuilder stringFormat = StringBuilderCache.Acquire();
                    for (int k = eventAttr.start; k <= eventAttr.end; ++k)
                    {
                        stringFormat.Clear();
                        stringFormat.AppendFormat(eventAttr.controlName, k);
                        eb.strList.Add(stringFormat.ToString());
                    }
                    StringBuilderCache.Release(stringFormat);

                    eb.method = Delegate.CreateDelegate(eventAttr.eventType,
                                    this, current, true);
                                    */
                }
            }
        }
    }

    private string mCurrentFrame = "";

    [Test]
    public void AllFrames()
    {
        Assembly ass = Assembly.GetAssembly(typeof(Utility));
        foreach (var clientFrame in mClientFrames)
        {
            object frame = ass.CreateInstance(clientFrame);
            Type frameType = frame.GetType();

            MethodInfo info = frameType.GetMethod("GetPrefabPath",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

            mCurrentFrame = clientFrame;

            if (info != null)
            {
                var prefabPath = info.Invoke(frame, null) as string;
                var res = _checkFileExist(prefabPath);
                //var go = _checkPrefabPath(prefabPath.ToString());
                if (res != null)
                {
                    var assetpath = AssetDatabase.GetAssetPath(res);
                    var path = assetpath.Replace("Assets/Resources/", "");
                    path = path.Replace(".prefab", "");

                    if (path != prefabPath)
                    {
                        UnityEngine.Debug.LogErrorFormat("{0} 原来的 assetPath:{3}, \n{1}*\n{2}*", frameType.Name, prefabPath, path, assetpath);
                    }

                    var go = GameObject.Instantiate(res) as GameObject;
                    if (null != go)
                    {
                        _checkClienFrameAttributes(go, frameType, frame);
                        GameObject.DestroyImmediate(go);
                    }
                    go = null;
                }
            }

            mCurrentFrame = "";
        }
    }

    [Test]
    public void AllSystems()
    {
        Assembly ass = Assembly.GetAssembly(typeof(Utility));
        foreach (var clientFrame in mClientSystems)
        {
            object frame = ass.CreateInstance(clientFrame);
            Type frameType = frame.GetType();

            MethodInfo info = frameType.GetMethod("GetMainUIPrefabName",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);

            mCurrentFrame = clientFrame;

            if (info != null)
            {
                var prefabPath = info.Invoke(frame, null);
                var go = _checkPrefabPath(prefabPath.ToString());
                if (go != null)
                {
                    _checkClienFrameAttributes(go, frameType, frame);
                    GameObject.DestroyImmediate(go);
                    go = null;
                }
            }

            mCurrentFrame = "";
        }

    }
}
