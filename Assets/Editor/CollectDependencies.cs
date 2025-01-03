using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using UnityEditor.Experimental.UIElements;

namespace CollectDependencies
{
    public struct CSNode
    {
        public bool isarray;
        public string fieldname;
    }

    public class CodeGenerate
    {
        private static string[] kArray =  new string[]
        {
            "foreach (var <iter> in <prefix>)", 
                "{"                               ,
                "    if (null != <iter>)"         ,
                "    {"                           ,
                "       <body>"                   ,
                "    }"                           ,
                "}"                               ,
        }; 

        private static string[] kSingle = new string[]
        {
            "<prefix>.<body>"
        };

        private static string[] kCheck = new string[]
        {
            "var <iter> = <prefix>",
            "if (null != <iter> && <oldsprite> != <iter>)",
            "{",
            "   <iter> = <newsprite>;",
            "   // mark dirty",
            "   // EditorUtility.SetDirty(<prefab>);",
            "}",
        };

        private static string _array(string prefix, string iter, string body)
        {
            string ans = string.Join("\n", kArray);

            ans = ans.Replace("<prefix>", prefix);
            ans = ans.Replace("<iter>", iter);
            ans = ans.Replace("<body>", body);

            return ans;
        }

        private static string _check(string prefix, string oldsprite, string newsprite, string prefab)
        {
            string ans = string.Join("\n", kCheck);

            ans = ans.Replace("<prefix>", prefix);
            ans = ans.Replace("<oldsprite>", oldsprite);
            ans = ans.Replace("<newsprite>", newsprite);
            ans = ans.Replace("<prefab>", prefab);

            return ans;
        }


        private static string _single(string prefix, string body)
        {
            string ans = string.Join("\n", kSingle);

            ans = ans.Replace("<prefix>", prefix);
            ans = ans.Replace("<body>", body);

            return ans;
        }

        public static string DumpCode(CSNode[] node)
        {
            string ans = "";
            string iter = "i";
            for (int i = node.Length - 1; i >= 0; i--)
            {
                if (node[i].isarray)
                {
                    _check(iter, "old", "new", "prefabroot");
                }
                else 
                {

                }
            }
            return null;
        }
    

        [MenuItem("[TM工具集]/[UI测试工具]/test code format")]
        private static void _test()
        {
            string ans = _array("xx", "iter", _single("iter", "filed"));
            UnityEngine.Debug.LogErrorFormat("ans :\n {0}", ans);
        }
    }

    public class ComUtility
    {
        /*
ComDungeonMap	6	mBackRoundImage	mItemImage	mBossSprite	mStartSprite	mHellSprite[]	mImageList[]
ETCButton	4	normalSprite	pressedSprite	coolDownSprite	fgSprite		
CPKHPBar	4	leftTeamSprite	rightTeamSprite	leftTeamFlagSprite	rightTeamFlagSprite		
ComTags	3	mImageArray[].sprite	mImageArray[].overrideSprite	mDefaulteSprite			
ComChapterDungeonUnit	3	mDungeonTypeSprite[]	mMissionTypeSprite[]	mStateSprite[]			
ImageNumber	3	numberList[]	imageList[].sprite	imageList[].overrideSprite			
UnityEngine.UI.Selectable	3	spriteState.highlightedSprite	spriteState.pressedSprite	spriteState.disabledSprite			
UnityEngine.UI.Image	2	sprite	overrideSprite				
ComDungeonScore	2	infos[].mScore[]	mImages[]				
ComExpBar	2	mImageBar[].sprite	mImageBar[].overrideSprite				
ComDungeonCharactorHeadBar	2	mImageBar[].sprite	mImageBar[].overrideSprite				
ComDungeonFinishScore	2	infos[].mScore[]	mScoreList[]				
SpriteAsset	1	listSpriteAssetInfor.Item.sprite					
ComDungeonScoreInfo	1	mScore[]					
ComCommonBind	1	reses[].sprite					
ComChapterDungeonScore	1	mImageList[]					
ComMouCountTips	1	mNumbers[]					
ComCountScript	1	mNumberSprite[]					
UnityEngine.UI.Dropdown	1	options.Item.image					
CBossHpBar	1	SpriteList[]					
*/
        private static void _replaceSprite(ref Sprite filed)
        {
            if (filed != null && filed.GetInstanceID() == mOld.GetInstanceID())
            {
                UnityEngine.Debug.LogErrorFormat("{0} {1}->{2}", filed.name, mOld.name, mNew.name);
                filed = mNew;
                mRes  = true;
            }
        }

        private static Sprite mOld;
        private static Sprite mNew;
        private static bool   mRes;

        private static Material mOldMat;
        private static Material mNewMat;

        public static bool CustomReplaceSprite(UnityEngine.GameObject obj, Sprite oldsprite, Sprite newsprite, Material newMat = null)
        {
            mNew = newsprite;
            mOld = oldsprite;

        //    mOldMat = oldMat;
            mNewMat = newMat;

            mRes = false;
            {
                var coms = obj.GetComponentsInChildren<ComDungeonMap>(true);
                foreach (var com in coms)
                {
                    _replaceSprite(ref com.mBackRoundImage);
                    _replaceSprite(ref com.mItemImage);
                    _replaceSprite(ref com.mStartSprite);
                    for (int i = 0; i < com.mHellSprite.Length; ++i)
                        _replaceSprite(ref com.mHellSprite[i]);
                    for (int i = 0; i < com.mHellSprite.Length; ++i)
                        _replaceSprite(ref com.mImageList[i]);
                }
            }
            {
                var coms = obj.GetComponentsInChildren<ETCButton>(true);
                foreach (var com in coms)
                {
                    _replaceSprite(ref com.normalSprite);
                    _replaceSprite(ref com.pressedSprite);
                    _replaceSprite(ref com.coolDownSprite);
                    _replaceSprite(ref com.fgSprite);
                }
            }
            {
                var coms = obj.GetComponentsInChildren<CPKHPBar>(true);
                foreach (var com in coms)
                {
                    //_replaceSprite(ref com.leftTeamSprite);
                    //_replaceSprite(ref com.rightTeamSprite);
                    //_replaceSprite(ref com.leftTeamFlagSprite);
                    //_replaceSprite(ref com.rightTeamFlagSprite);
                }
            }
            {
                var coms = obj.GetComponentsInChildren<ComTags>(true);
                foreach (var com in coms)
                {
                    _replaceSprite(ref com.mDefaulteSprite);
                }
            }{
                var coms = obj.GetComponentsInChildren<ComChapterDungeonUnit>(true);
                foreach (var com in coms)
                {
                    //for (int i = 0; i < com.mDungeonTypeSprite.Length; ++i)
                    //    _replaceSprite(ref com.mDungeonTypeSprite[i]);
                    //for (int i = 0; i < com.mMissionTypeSprite.Length; ++i)
                    //    _replaceSprite(ref com.mMissionTypeSprite[i]);
                    //for (int i = 0; i < com.mStateSprite.Length; ++i)
                    //    _replaceSprite(ref com.mStateSprite[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<ImageNumber>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.numberList.Length; ++i)
                        _replaceSprite(ref com.numberList[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<UnityEngine.UI.Selectable>(true);
                foreach (var com in coms)
                {
                    SpriteState state = new SpriteState();
                    state.disabledSprite = com.spriteState.disabledSprite;
                    state.highlightedSprite = com.spriteState.highlightedSprite;
                    state.pressedSprite = com.spriteState.pressedSprite;

                    if (state.highlightedSprite != null && state.highlightedSprite.GetInstanceID() == mOld.GetInstanceID())
                    {
                        state.highlightedSprite = mNew;
                        mRes = true;
                    }
                    if (state.disabledSprite != null && state.disabledSprite.GetInstanceID() == mOld.GetInstanceID())
                    {
                        state.disabledSprite = mNew;
                        mRes = true;
                    }
                    if (state.pressedSprite != null && state.pressedSprite.GetInstanceID() == mOld.GetInstanceID())
                    {
                        state.pressedSprite = mNew;
                        mRes = true;
                    }
                    com.spriteState = state;
                }
            }{
                var coms = obj.GetComponentsInChildren<UnityEngine.UI.Image>(true);
                foreach (var com in coms)
                {
                    if (com.sprite != null && com.sprite.GetInstanceID() == mOld.GetInstanceID())
                    {
                        com.sprite = mNew;
                        com.material = mNewMat;
                        mRes = true;
                    }
                    if (com.overrideSprite != null && com.overrideSprite.GetInstanceID() == mOld.GetInstanceID())
                    {
                        com.overrideSprite = mNew;
                        com.material = mNewMat;
                        mRes = true;
                    }
                    //_replaceSprite(ref com.sprite);
                    //_replaceSprite(ref com.overrideSprite);
                }
            }{
                var coms = obj.GetComponentsInChildren<ComDungeonScore>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.mImages.Length; ++i)
                        _replaceSprite(ref com.mImages[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<ComDungeonFinishScore>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.mScoreList.Length; ++i)
                        _replaceSprite(ref com.mScoreList[i]);
                }
            }{
                //var coms = obj.GetComponentsInChildren<SpriteAsset>();
                //foreach (var com in coms)
                //{
                //    for (int i = 0; i < com.listSpriteAssetInfor.Count; ++i)
                //        _replaceSprite(ref com.listSpriteAssetInfor[i].sprite);
                //}
            }{
                var coms = obj.GetComponentsInChildren<ComDungeonScoreInfo>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.mScore.Length; ++i)
                        _replaceSprite(ref com.mScore[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<ComCommonBind>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.reses.Length; ++i)
                        _replaceSprite(ref com.reses[i].sprite);
                }
            }{
                //var coms = obj.GetComponentsInChildren<ComChapterDungeonScore>(true);
                //foreach (var com in coms)
                //{
                //    for (int i = 0; i < com.mImageList.Length; ++i)
                //        _replaceSprite(ref com.mImageList[i]);
                //}
            }{
                var coms = obj.GetComponentsInChildren<ComMouCountTips>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.mNumbers.Length; ++i)
                        _replaceSprite(ref com.mNumbers[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<ComCountScript>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.mNumberSprite.Length; ++i)
                        _replaceSprite(ref com.mNumberSprite[i]);
                }
            }{
                var coms = obj.GetComponentsInChildren<UnityEngine.UI.Dropdown>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.options.Count; ++i)
                        if (com.options[i].image != null && com.options[i].image.GetInstanceID() == mOld.GetInstanceID())
                        {
                            com.options[i].image = mNew;
                            mRes = true;
                        }
                        //_replaceSprite(ref com.options[i].image);
                }
            }{
                var coms = obj.GetComponentsInChildren<CBossHpBar>(true);
                foreach (var com in coms)
                {
                    for (int i = 0; i < com.SpriteList.Length; ++i)
                        _replaceSprite(ref com.SpriteList[i]);
                }
            }

            mOld = null;
            mNew = null;
            mNewMat = null;

            return mRes;
        }

    }

    public class CollectDependenciesExample : EditorWindow
    {
        static GameObject obj       = null;

        static Sprite    mSprite    = null;
        static Material mMaterial = null;
        static Texture2D mTexture2D = null;


        [MenuItem( "[TM工具集]/[UI测试工具]/UI资源引用 #&Q" )]
        static void Init( )
        {
            // Get existing open window or if none, make a new one:
            CollectDependenciesExample window = (CollectDependenciesExample)EditorWindow.GetWindow( typeof(CollectDependenciesExample) );
            window.titleContent = new GUIContent("找引用");
            window.Show( );
        }

        private static Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();

        [SerializeField]
        private static Dictionary<string, SpriteMetaData[]> mSpriteMetas = new Dictionary<string, SpriteMetaData[]>();

        private static string _getKey(string objpath, string name)
        {
            return string.Format("{0}:{1}", objpath, name);
        }

        static Sprite FindSprite(string imgpath, string name, Vector4 offset)
        {
            string key = _getKey(imgpath, name);
            if (mSprites.ContainsKey(key))
            {
                return mSprites[key];
            }

            var txt = AssetDatabase.FindAssets(string.Format("t:sprite {0}", name), new string[] { "Assets/Resources/UIPacked", "Assets/Resources/Base" });
            foreach (var g in txt)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);

                var ti = TextureImporter.GetAtPath(path) as TextureImporter;

                if (ti.spriteImportMode == SpriteImportMode.Multiple)
                {
                    for (int i = 0; i < ti.spritesheet.Length; ++i)
                    {
                        var sp = ti.spritesheet[i];

                        if (sp.name == name)
                        {
                            //if (offset != Vector4.zero && sp.border == Vector4.zero)
                            //{
                            //    UITexturePacker._modifyMetaInfo(AssetDatabase.GetTextMetaFilePathFromAssetPath(path), name, offset);
                            //}

                            var objs = Resources.LoadAll<Sprite>(CFileManager.EraseExtension(path).Replace("Assets/Resources/", ""));

                            foreach (var ops in objs)
                            {
                                if (ops.name == name)
                                {
                                    mSprites.Add(key, ops);
                                    return ops;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static GameObject[] _getList(GameObject root)
        {
            if (null == root)
            {
                return UnityEngine.Object.FindObjectsOfType<GameObject>();
            }

            List<GameObject> list = new List<GameObject>();

            _getAllGameObjectByRoot(list, root);

            return list.ToArray();
        }


        private static void _getAllGameObjectByRoot(List<GameObject> list, GameObject root)
        {
            list.Add(root);

            for (int i = 0; i < root.transform.childCount; i++)
            {
                _getAllGameObjectByRoot(list, root.transform.GetChild(i).gameObject);
            }
        }

        public static void FindReferencesTo(UnityEngine.Object to, GameObject root = null)
        {
            var referencedBy = new List<UnityEngine.Object>();
            var allObjects = _getList(root);
            for (int j = 0; j < allObjects.Length; j++)
            {
                var go = allObjects[j];

                UnityEngine.Debug.LogFormat("name {0}", go.name);

                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
                    if (PrefabUtility.GetPrefabParent(go) == to)
                    {
                        UnityEngine.Debug.Log(string.Format("referenced by {0}, {1}", go.name, go.GetType()), go);
                        referencedBy.Add(go);
                    }
                }

                var components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    var c = components[i];
                    if (!c) continue;

                    var so = new SerializedObject(c);
                    var sp = so.GetIterator();

                    while (sp.Next(true))
                        if (sp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (sp.objectReferenceValue == to)
                            {
                                UnityEngine.Debug.Log(string.Format("com referenced by {0}, {1}", c.name, c.GetType()), c);
                                referencedBy.Add(c.gameObject);
                            }
                        }
                }
            }


		
            if (referencedBy.Any())
				Selection.objects = referencedBy.ToArray();
            else UnityEngine.Debug.Log("no references in scene");
        }

        static void GetSpriteFiles(Type objType, List<CSNode> prefix, List<CSNode[]> ans, bool isLog = false, int deep = 1)
        {
            if (deep > 3)
            {
                return;
            }

            PropertyInfo[] props = objType.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.DeclaredOnly);

            for (int i = 0; i < props.Length; ++i)
            {
                Type fileType    = props[i].PropertyType;
                string fieldName = props[i].Name;

                if (isLog) UnityEngine.Debug.LogErrorFormat("Filed name {0}, {1}, isArray {2}", fileType.FullName, props[i].Name, fileType.IsArray);

                CSNode node = new CSNode() {isarray=fileType.IsArray, fieldname = fieldName};
                prefix.Add(node);

                if (fileType.IsSubclassOf(typeof(Component)) || fileType.IsSubclassOf(typeof(Transform)) || fileType.IsSubclassOf(typeof(GameObject)))
                {

                }
                else if (fileType.IsArray)
                {
                    Type eleType = fileType.GetElementType();
                    if (eleType != typeof(Sprite))
                    { 
                        GetSpriteFiles(eleType, prefix, ans, isLog, deep+1);
                    }
                    else 
                    {
                        ans.Add(prefix.ToArray());
                    }
                }
                else if (fileType == typeof(Sprite))
                {
                    ans.Add(prefix.ToArray());
                }
                else 
                {
                    GetSpriteFiles(fileType, prefix, ans, isLog, deep+1);
                }

                prefix.Remove(node);
            }

            FieldInfo[] files    = objType.GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public | BindingFlags.DeclaredOnly);

            for (int i = 0; i < files.Length; ++i)
            {
                Type fileType    = files[i].FieldType;
                string fieldName = files[i].Name;

                if (isLog) UnityEngine.Debug.LogErrorFormat("Filed name {0}, {1}, isArray {2}", fileType.FullName, files[i].Name, fileType.IsArray);

                CSNode node = new CSNode() {isarray=fileType.IsArray, fieldname = fieldName};
                prefix.Add(node);

                if (fileType.IsSubclassOf(typeof(Component)))
                {

                }
                else if (fileType.IsArray)
                {
                    Type eleType = fileType.GetElementType();
                    if (eleType != typeof(Sprite))
                    { 
                        GetSpriteFiles(eleType, prefix, ans, isLog, deep+1);
                    }
                    else 
                    {
                        ans.Add(prefix.ToArray());
                    }
                }
                else if (fileType == typeof(Sprite))
                {
                    ans.Add(prefix.ToArray());
                }
                else 
                {
                    GetSpriteFiles(fileType, prefix, ans, isLog, deep+1);
                }

                prefix.Remove(node);
            }
        }

        [MenuItem("[TM工具集]/[UI测试工具]/ui - cscode #&%L")]
        static void GetAllCS()
        {
            var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { kUIPrefab });
            //Dictionary<string, CollectObject> coDict = new Dictionary<string, CollectObject>();

            var coDict = new Dictionary<string, List<CSNode[]>>();

            int cnt = prefabs.Length;
            int ii = 0;

            Assembly ass = Assembly.GetAssembly(typeof(Utility));

            foreach (var preguid in prefabs)
            {
                ii++;

                var path = AssetDatabase.GUIDToAssetPath(preguid);
                UnityEngine.Object root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { root });


                for (int i = 0; i < deps.Length; ++i)
                {
                    //string objpath = AssetDatabase.GetAssetPath(deps[i]);
                    //p
                    //
                    MonoScript script = deps[i] as MonoScript;
                    if (null != script)
                    {
                        Type objType = script.GetClass();

                        string scriptname = objType.FullName;

                        if (!coDict.ContainsKey(scriptname))
                        {
                            coDict.Add(scriptname, new List<CSNode[]>());

                            try 
                            {
                                UnityEngine.Debug.LogFormat("type : {0}", Path.GetFileName(scriptname));

                                EditorUtility.DisplayProgressBar(scriptname, scriptname, ii / cnt);

                                GetSpriteFiles(objType, new List<CSNode>(), coDict[scriptname]);

                            }
                            catch(System.Exception e)
                            {
                                UnityEngine.Debug.LogErrorFormat("{0}, {1}", scriptname, e.ToString());
                                break;
                            }
                        }
                    }
                }
            }

            EditorUtility.DisplayProgressBar("write file", "write file", 1);

            string ans = "cs,prefab\n";
            foreach(var kv in coDict)
            {
                ans += string.Format("{0},{1},", kv.Key, kv.Value.Count);

                foreach (var item in kv.Value)
                {
                    ans += string.Format("{0},", _getString(item));
                }

                ans += string.Format("\n");

            }

            try
            {
                File.WriteAllText("allcsfile.csv", ans);
            }
            catch
            {

            }

            EditorUtility.ClearProgressBar();// ("write file", "write file", 1);
        }

        static string _getString(CSNode[] list)
        {
            string ans = "";
            for (int i = 0; i < list.Length; ++i)
            {
                ans += list[i].fieldname;
                if (list[i].isarray) ans+="[]";

                if (i!=(list.Length-1))
                {
                    ans+=".";
                }
            }
            return ans;
        }


        static Dictionary<UnityEngine.Object, List<string>> GetDicts<T>(string[] prefabpaths) where T : UnityEngine.Object
        {
            var prefabs = AssetDatabase.FindAssets("t:prefab", prefabpaths);

            Dictionary<UnityEngine.Object, List<string>> coDict = new Dictionary<UnityEngine.Object, List<string>>();

            int cnt = prefabs.Length;
            int ii = 0;

            foreach (var preguid in prefabs)
            {
                ii++;

                var path = AssetDatabase.GUIDToAssetPath(preguid);

                UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { root });

                foreach (var com in deps)
                {
                    T sp = com as T;
                    if (null != sp)
                    {
                        string objpath = AssetDatabase.GetAssetPath(sp);

                        EditorUtility.DisplayProgressBar(Path.GetFileName(path), Path.GetFileName(objpath), ii*1.0f/cnt);
                        if (!coDict.ContainsKey(sp))
                        {
                            coDict.Add(sp, new List<string>());
                        }

                        coDict[sp].Add(path);
                    }
                }
            }

            // //按字母排序
            // foreach(var item in coDict)
            // {
            //     List<string> lst = item.Value;
            //     lst.Sort((a, b)=>
            //     {
            //         var foderA = a

            //         return string.Compare(a, b);
            //     });
            // }

            EditorUtility.ClearProgressBar();

            return coDict;

        }

        [MenuItem("[TM工具集]/[UI测试工具]/删除没用的贴图")]
        static void GetNoUsedEffectImage()
        {
            //var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { kEffectPrefab });

            //Dictionary<UnityEngine.Object, List<string>> dict = new Dictionary<UnityEngine.Object, List<string>>();

            //int cnt = prefabs.Length;
            //int ii = 0;

            //foreach (var preguid in prefabs)
            //{
            //    ii++;

            //    var path = AssetDatabase.GUIDToAssetPath(preguid);

            //    UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            //    var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { root });
            //}

            var dict = GetDicts<Texture>(new string[] { kEffectPrefab });
            
            Dictionary<string, int> node = new Dictionary<string,int>();


            string ans = "png, count, list\n";

            foreach (var kv in dict)
            {

                string path = AssetDatabase.GetAssetPath(kv.Key);
                //foreach (var path in kv.Value)
                {
                    if (!node.ContainsKey(path))
                    {
                        node.Add(path, 0);
                    }
                    node[path] += kv.Value.Count;

                    ans += string.Format("{0},{1},", path, node[path]);
                    for (int i = 0; i < kv.Value.Count; ++i)
                    {
                        ans += string.Format(",{0}", kv.Value[i]); 
                    }
                    ans += "\n";
                }
            }

            File.WriteAllText("effectst.cvs", ans);

    
            var texutres = AssetDatabase.FindAssets("t:texture", new string[] { kEffectPrefab });

            List<string> noUsePngList = new List<string>();
            List<string> onceMorePngList = new List<string>();

            foreach (var id in texutres)
            {
                string path = AssetDatabase.GUIDToAssetPath(id);

                EditorUtility.DisplayProgressBar("检查所有贴图", path, 0.3f);

                if (node.ContainsKey(path))
                {
                    if (node[path] > 1)
                    {
                        onceMorePngList.Add(path);
                    }
                }
                else 
                {
                    noUsePngList.Add(path);
                }
            }

            EditorUtility.DisplayProgressBar(string.Format("无用 {0}", noUsePngList.Count), "", 0.3f);

            if (noUsePngList.Count > 0)
                AssetDatabase.ExportPackage(noUsePngList.ToArray(), "没有使用的特效贴图备份.unitypackage");

            foreach (var path in noUsePngList)
            {
                File.Delete(path);
            }

            EditorUtility.DisplayProgressBar(string.Format("1次数目 {0}", onceMorePngList.Count), "", 0.8f);

            if (onceMorePngList.Count > 0)
                AssetDatabase.ExportPackage(onceMorePngList.ToArray(), "使用多次的特效贴图备份.unitypackage");

            EditorUtility.ClearProgressBar();
        }



        //[MenuItem("[UITest]/replace Image's sprite")]
        static void GetReplaceImage()
        {
            mSprites.Clear();
            mSpriteMetas.Clear();

            var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { kUIPrefab });
            //Dictionary<string, CollectObject> coDict = new Dictionary<string, CollectObject>();

            Dictionary<string, List<string>> coDict = new Dictionary<string, List<string>>();

            int cnt = prefabs.Length;
            int ii = 0;

            foreach (var preguid in prefabs)
            {
                ii++;

                var path = AssetDatabase.GUIDToAssetPath(preguid);
                UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var images = root.GetComponentsInChildren<Image>(true);

                foreach (var img in images)
                {
                    if (img.sprite)
                    {
                        string objpath = AssetDatabase.GetAssetPath(img.sprite);

                        if (objpath.StartsWith(kUIDangrous))
                        {
                            EditorUtility.DisplayProgressBar(Path.GetFileName(path), Path.GetFileName(objpath), ii*1.0f/cnt);

                            var getFileName = img.sprite.name;

                            var fs  = FindSprite(objpath, getFileName, img.sprite.border);
                            if (null != fs)
                            {
                                img.sprite = fs;
                                EditorUtility.SetDirty(root);
                            }
                            else
                            {
                                Logger.LogErrorFormat("{0}, {1}", path, objpath);
                            }
                        }
                    }
                }

            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();// ("write file", "write file", 1);
        }

        [MenuItem("[TM工具集]/[UI测试工具]/uitest #&%L")]
        static void GetDeps()
        {
            var prefabs = AssetDatabase.FindAssets("t:prefab", new string[] { kUIPrefab });
            //Dictionary<string, CollectObject> coDict = new Dictionary<string, CollectObject>();

            Dictionary<string, List<string>> coDict = new Dictionary<string, List<string>>();

            int cnt = prefabs.Length;
            int ii = 0;

            foreach (var preguid in prefabs)
            {
                ii++;

                var path = AssetDatabase.GUIDToAssetPath(preguid);
                UnityEngine.Object root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { root });


                for (int i = 0; i < deps.Length; ++i)
                {
                    string objpath = AssetDatabase.GetAssetPath(deps[i]);

                    if (objpath.StartsWith(kUIDangrous))
                    {
                        EditorUtility.DisplayProgressBar(path, objpath, ii / cnt);

                        TextureImporter ti = AssetImporter.GetAtPath(objpath) as TextureImporter;

                        if (null != ti && ti.spriteImportMode == SpriteImportMode.Single)
                        {
                            if (!coDict.ContainsKey(path))
                            {
                                coDict.Add(path, new List<string>());
                            }

                            if (coDict[path].Find(x => { return x == objpath; }) == null)
                            {
                                coDict[path].Add(objpath);
                            }
                        }
                    }
                }
            }

            EditorUtility.DisplayProgressBar("write file", "write file", 1);

            string ans = "prefab,png\n";
            foreach(var kv in coDict)
            {
                ans += string.Format("{0},{1},", kv.Key, kv.Value.Count);

                foreach (var item in kv.Value)
                {
                    ans += string.Format("{0},", item);
                }

                ans += string.Format("\n");

            }

            try
            {
                File.WriteAllText("filter.csv", ans);
            }
            catch
            {

            }

            EditorUtility.ClearProgressBar();// ("write file", "write file", 1);
        }

        private List<UnityEngine.Object> mObjectList = new List<UnityEngine.Object>();

        private List<string> mObjectsKeys = new List<string>();
        private Dictionary<string, CollectObject> mObjects = new Dictionary<string, CollectObject>();

        private class CollectObject
        {
            public CollectObject()
            {
                objects = new List<UnityEngine.Object>();
                objssize = new List<int>();
            }

            public string path   		= "";
            public bool showFlag        = false;
            public List<UnityEngine.Object> objects = null;
            public List<int> objssize   = null;
            public int allsize          = 0;
        }

        private void _collectData(UnityEngine.Object[] alldeps)
        {
            mObjectList.Clear();
            mObjects.Clear();
            for (int i = 0; i < alldeps.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(alldeps[i]);

                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

                if (null != ti)
                {
                    mObjectList.Add(alldeps[i]);

                    CollectObject ob = null;
                    if (!mObjects.ContainsKey(path))
                    {
                        ob = new CollectObject();
                        ob.path = path;
                        mObjects.Add(path, ob);
                    }

                    ob = mObjects[path];
                    ob.objects.Add(alldeps[i]);
                    ob.objssize.Add(_getMemorySize(alldeps[i]));
                    ob.allsize += _getMemorySize(alldeps[i]);
                }
            }


            Selection.objects = mObjectList.ToArray();
        }

        private const string kUIBase     = "Assets/Resources/Base/";
        private const string kUIEffect   = "Assets/Resources/Effects/";
        private const string kUISplite   = "Assets/Resources/UI/";
        private const string kUIPacked   = "Assets/Resources/UIFlatten/Image/Packed";

        private const string kUIPrefab       = "Assets/Resources/UIFlatten/Prefabs";
        private const string kUIOldPrefab    = "Assets/Resources/UI/Prefabs"; 
        private const string kEffectPrefab   = "Assets/Resources/Effects";

        
        private const string kUIDangrous = "Assets/UI/";


        private string[] AllPreFile = new string[] { kUIBase, kUIEffect, kUISplite, kUIPacked, kUIDangrous};

        private void _showDicts()
        {
            EditorGUI.indentLevel = 2;

            mPos = EditorGUILayout.BeginScrollView(mPos);
            {
                EditorGUILayout.BeginVertical();

                mObjectsKeys = mObjects.Keys.ToList();
                mObjectsKeys.Sort();

                int ckUI = 0;

                bool[] isVisited = new bool[AllPreFile.Length];

                int allsize = 0;

                foreach (var key in mObjectsKeys)
                {
                    var value = mObjects[key];

                    allsize += value.allsize;

                    int bkckUI = ckUI;

                    bool hasChange = false;
                    while (ckUI < AllPreFile.Length && !key.StartsWith(AllPreFile[ckUI])) { ckUI++; hasChange = true; } 

                    if (bkckUI < AllPreFile.Length && hasChange)
                    {
                        //EditorGUILayout.EndVertical();
                        //EditorGUILayout.EndToggleGroup();
                        EditorGUI.indentLevel--;
                    }

                    if (ckUI < AllPreFile.Length && !isVisited[ckUI])
                    {
                        isVisited[ckUI] = true;

                        if (!key.StartsWith("Assets/Resource"))
                        {
                            GUI.color = Color.red;
                        }

                        EditorGUILayout.LabelField(AllPreFile[ckUI]);

                        GUI.color = Color.white;

                        EditorGUI.indentLevel++;
                        //EditorGUILayout.BeginToggleGroup("a", true);
                    }

                    EditorGUILayout.BeginVertical();
                    if (obj != null && obj.activeInHierarchy)
                    {
                        if (GUILayout.Button("引用该图集的所有子节点",GUILayout.MaxWidth(200),GUILayout.MaxHeight(20)))
                        {
                            ShowUseImageItem(value);
                        }   
                    }
                    
                    bool flag = EditorGUILayout.BeginToggleGroup(key+EditorUtility.FormatBytes(value.allsize), value.showFlag);
                    {

                        if (value.showFlag != flag)
                        {
                            value.showFlag = flag;
                            if (value.showFlag)
                            {
                                var tmp = new List<UnityEngine.Object>();
                                for (int i = 0; i < value.objects.Count; ++i)
                                {
                                    if (value.objects[i] is Sprite)
                                    {
                                        tmp.Add(value.objects[i]);
                                    }
                                }

                                Selection.objects = tmp.ToArray();
                            }
                        }

                        if (value.showFlag)
                        {
                            for (int i = 0; i < value.objects.Count; ++i)
                            {
                                EditorGUILayout.BeginHorizontal();
                                if (value.objssize[i] > 0)
                                {
                                    EditorGUILayout.LabelField(EditorUtility.FormatBytes(value.objssize[i]));
                                }

                                var spRect = GUILayoutUtility.GetRect(50, 50);
                                EditorGUI.ObjectField(spRect, value.objects[i].name, value.objects[i], typeof(Sprite));
                                //EditorGUILayout.ObjectField(value.objects[i], typeof(Sprite), false);

                                if (value.objssize[i] <= 0)
                                {
                                    var rect = GUILayoutUtility.GetRect(20, 20);
                                    if (GUI.Button(rect, "找"))
                                    {
                                        mSprite    = value.objects[i] as Sprite;
                                        mDirtyFlag = true;
                                    }
                                    
                                    if (obj != null && obj.activeInHierarchy)
                                    {
                                        var rectSmall = GUILayoutUtility.GetRect(20, 20);
                                        if (GUI.Button(rectSmall, "此小图引用节点"))
                                        {
                                            ShowSmallImageItems(value.objects[i] as Sprite);
                                        }    
                                    }
                                }
                                
                                
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndToggleGroup();
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }

                EditorGUILayout.LabelField("总UI占内存大小："+EditorUtility.FormatBytes(allsize));

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// 如果要查看图集对象不在Hierarchy中,则自动打开并替换选中.
        /// </summary>
        void ChangeObjectFiledObj()
        {
            if (!obj.activeInHierarchy)
            {
                Selection.activeGameObject = obj;
                var insObj = HeroGo.UtilityTools.PreviewPrefab();
                if (insObj != null)
                {
                    obj = insObj;
                }
            }
        }


        private void ShowUseImageItem(CollectObject value)
        {
            if (obj!=null && value != null)
            {
                // EditorGUIUtility.PingObject(target);
                if (!obj.activeInHierarchy)
                {
                    Logger.LogError(obj.name+"不在场景内,无法显示使用该图集的子节点");
                    return;
                }
                
                Texture mainTexture = null;
                
                
                for (int i = 0; i < value.objects.Count; i++)
                {
                    if (value.objssize[i] > 0)
                    {
                        mainTexture = value.objects[i] as Texture;
                        break;
                    }
                }
                
                if (mainTexture == null)
                {
                    return;
                }


                List<UnityEngine.Object> selected = new List<UnityEngine.Object>();
                
                
                var imagesArray = obj.GetComponentsInChildren<Image>(true);
                for (int i = 0; i < imagesArray.Length; i++)
                {
                    if (imagesArray[i].sprite != null)
                    {
                        if (imagesArray[i].mainTexture.GetInstanceID() == mainTexture.GetInstanceID())
                        {
                            UnityEngine.Debug.Log(string.Format("使用该图集节点名:{0}",imagesArray[i].name));   
                            var item = imagesArray[i].gameObject as UnityEngine.Object;
                            selected.Add(item);
                        }
                    }
                }
                
                Selection.objects = selected.ToArray();
            }
        }

        private void ShowSmallImageItems(Sprite sprite)
        {
            if (sprite == null || obj == null)
            {
                return;
            }
            
            if (!obj.activeInHierarchy)
            {
                Logger.LogError(obj.name+"不在场景内,无法显示使用该图集的子节点");
                return;
            }
            
            List<UnityEngine.Object> selected = new List<UnityEngine.Object>();
            
            var imagesArray = obj.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < imagesArray.Length; i++)
            {
                if (imagesArray[i].sprite != null)
                {
                    if (imagesArray[i].sprite.GetInstanceID() == sprite.GetInstanceID())
                    {
                        UnityEngine.Debug.Log(string.Format("使用该小图节点名:{0}",imagesArray[i].name));   
                        var item = imagesArray[i].gameObject as UnityEngine.Object;
                        selected.Add(item);
                    }
                }
            }
                
            Selection.objects = selected.ToArray();
        }

        private int _getMemorySize(UnityEngine.Object obj)
        { 
            if (obj is Texture)
            {
                //var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
                //MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

#if UNITY_5_6_3
                return UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(obj);
#else
                return UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(obj);
#endif
            }

            return 0;
        }

        void OnGUI( )
        {
            Rect targetRect = GUILayoutUtility.GetRect(position.width - 6, 20); 
            obj = EditorGUI.ObjectField(targetRect, "目标", obj, typeof(GameObject) ) as GameObject;

            if (obj)
            {
                UnityEngine.Object[] roots = new UnityEngine.Object[] { obj };

                //EditorGUILayout.LabelField(_getMemorySize(obj).ToString());

                Rect fbutton = GUILayoutUtility.GetRect(position.width - 6, 20); 

                if (GUI.Button(fbutton, "Biu~Biu~Biu~"))
                {
                    ChangeObjectFiledObj();
                    UnityEngine.Object[] deps = EditorUtility.CollectDependencies(roots);
                    _collectData(deps);
                }

                {
                    Rect rect = GUILayoutUtility.GetRect(position.width - 6, 20);

                    if (GUI.Button(rect, "打开预制体"))
                    {
                        Selection.activeGameObject = obj;
                        HeroGo.UtilityTools.PreviewPrefab();
                    }
                }

                {
                    Rect rect = GUILayoutUtility.GetRect(position.width - 6, 20);

                    Color origin = GUI.color;

                    GUI.color = Color.green;

                    if (GUI.Button(rect, "保存预制体"))
                    {
                        EditorUtility.SetDirty(obj);
                        AssetDatabase.SaveAssets();
                    }
                    GUI.color = origin;
                }


            }
            else
            {
                Rect rect = GUILayoutUtility.GetRect(position.width - 6, 20);
                EditorGUI.LabelField(rect, "丢了:", "选个预制体");
            }

            _showDicts();

            _showAllPrefabUsed();

            _replaceSprite();
        }


        static Sprite mNewSprite = null;
        static Material mNewMasterial = null;

        private void _replaceSprite()
        {
            EditorGUILayout.BeginHorizontal();

            //{
            //    var rect = GUILayoutUtility.GetRect(1, 100);
            //    EditorGUI.ObjectField(rect, mSprite, typeof(Sprite));
            //}
            //{
            //    var rect = GUILayoutUtility.GetRect(1, 100);
            //    EditorGUI.LabelField(rect, "----替换成--->");
            //}

            {
                var rect = GUILayoutUtility.GetRect(1, 100);
                
                mNewSprite = EditorGUI.ObjectField(rect, "----替换成--->", mNewSprite, typeof(Sprite)) as Sprite;

                mNewMasterial = EditorGUILayout.ObjectField("对应的材质", mNewMasterial, typeof(Material), GUILayout.Width(260), GUILayout.Height(100)) as Material;

                // = EditorGUI.ObjectField(GUILayoutUtility.GetRect(100, 100, 100, 100), "对应的材质", mNewMasterial, typeof(Material)) as Material;
               
            }

            EditorGUILayout.EndHorizontal();

            {
                var rect = GUILayoutUtility.GetRect(1, 30);

                if (mSprite != mNewSprite && null != mNewSprite && null != mSprite)
                {
                    int cnt = 0;

                    if (null != mCacheObject)
                    {
                        cnt = mCacheObject.Count;
                    }

                    if (GUI.Button(rect, string.Format("替换 {0} 个prefab", cnt)))
                    {
                        var flags = new bool[cnt];

                        bool flag = false;
                        for (int i = 0; i < mCacheObject.Count; ++i)
                        {
                            if (ComUtility.CustomReplaceSprite(mCacheObject[i], mSprite, mNewSprite, mNewMasterial))
                            {
                                EditorUtility.SetDirty(mCacheObject[i]);
                                flags[i] = true;
                                flag = true;
                            }
                        }

                        if (flag) 
                        {
                            AssetDatabase.SaveAssets();

                            for(int i=0; i<cnt; ++i)
                            {
                                if (!flags[i])
                                    continue;
                                UpdateDict(mSprite, mNewSprite, mCacheObjectPath[i]);
                            }
                        }
                    }
                }
            }
        }

        static Dictionary<UnityEngine.Object, List<string>> mDicts = null;
        static bool mDirtyFlag = false;
        static string input = "";

        private void _showAllPrefabUsed()
        {
            int cnt = 0;
            if (null != mDicts)
            {
                cnt = mDicts.Count;
            }


            var buttonRect = GUILayoutUtility.GetRect(500, 30);
            if (GUI.Button(buttonRect, string.Format("重新加载缓存(已有{0})", cnt)))
            {
                mDicts = GetDicts<Sprite>(new string[] { kUIPrefab, kUIOldPrefab });
                mDirtyFlag = true;
            }

            GUILayout.BeginHorizontal();

            var rect = GUILayoutUtility.GetRect(500, 100);
            var sp = EditorGUI.ObjectField( rect, "目标", mSprite, typeof(Sprite) ) as Sprite;
         //   mMaterial = EditorGUILayout.ObjectField("对应的材质", mMaterial, typeof(Material), GUILayout.Width(260), GUILayout.Height(100)) as Material;

            GUILayout.EndHorizontal();

            if (sp != mSprite || mDirtyFlag)
            {
                mDirtyFlag = false;
                //for (int i = 0; i < mCacheObject.Count; ++i)
                //{
                //    GameObject.DestroyImmediate(mCacheObject[i]);
                //}
                mCacheObject.Clear();
                mCacheObjectPath.Clear();

                if (sp != null && null != mDicts && mDicts.ContainsKey(sp))
                {
                    foreach (var item in mDicts[sp])
                    {
                        mCacheObject.Add(AssetDatabase.LoadAssetAtPath<GameObject>(item));
                        mCacheObjectPath.Add(item);
                    }
                }

                mSprite = sp;
            }

            input = EditorGUILayout.TextField("筛选", input, GUILayout.Width(300));

            if (mCacheObject != null && mCacheObject.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                mPosPrefab = EditorGUILayout.BeginScrollView(mPosPrefab);
                for (int i = 0; i < mCacheObject.Count; ++i)
                {
                    if (!string.IsNullOrEmpty(input) && !mCacheObjectPath[i].Contains(input, StringComparison.OrdinalIgnoreCase))
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(mCacheObject[i], typeof(GameObject));

                    {

                        var style = new GUIStyle(GUI.skin.button);

                        style.stretchWidth = false;
                        style.stretchHeight = false;

                        {
                            var brect = GUILayoutUtility.GetRect(20, 20);
                            if (GUI.Button(brect, "开", style))
                            {
                                Selection.activeGameObject = mCacheObject[i];
                                GameObject obj = HeroGo.UtilityTools.PreviewPrefab();
								FindReferencesTo(mSprite, obj);
                            }
                        }

                        if (null != mNewSprite && mNewSprite != mSprite)
                        {
                            var brect = GUILayoutUtility.GetRect(20, 20);
                            if (GUI.Button(brect, "换",  style))
                            {
                                if (ComUtility.CustomReplaceSprite(mCacheObject[i], mSprite, mNewSprite, mNewMasterial))
                                {
                                    EditorUtility.SetDirty(mCacheObject[i]);
                                    AssetDatabase.SaveAssets();

                                    UpdateDict(mSprite, mNewSprite, mCacheObjectPath[i]);
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            else 
            {
                EditorGUILayout.HelpBox(string.Format("这个图片么用到!!!"), MessageType.Warning);
            }
        }

        private void UpdateDict(Sprite oldSprite, Sprite newSprite, string objPath)
        {
            if (string.IsNullOrEmpty(objPath))
                return;

            if (oldSprite != null && mDicts.ContainsKey(oldSprite))
            {
                var lst = mDicts[oldSprite];
                if (lst.Contains(objPath))
                    lst.Remove(objPath);
            }

            if (newSprite != null)
            {
                if (!mDicts.ContainsKey(newSprite))
                {
                    mDicts.Add(newSprite, new List<string>());
                }

                if (!mDicts[newSprite].Contains(objPath))
                    mDicts[newSprite].Add(objPath);
            }
        }

        private Vector2 mPosPrefab = Vector2.zero;

        private static List<GameObject> mCacheObject = new List<GameObject>();
        private static List<string> mCacheObjectPath = new List<string>();

        private Vector2 mPos = Vector2.zero;

        void OnInspectorUpdate( )
        {
            Repaint( );
        }
    }
}
