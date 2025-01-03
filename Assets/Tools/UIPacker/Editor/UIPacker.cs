// using UnityEngine;
// using UnityEditor;
// using System.IO;
// using System.Text;
// using System.Collections.Generic;
// using YamlDotNet.RepresentationModel;
// 
// using System.Collections;
// 
// using XUPorterJSON;
// 
// using NPOI.HSSF.UserModel;
// using NPOI.SS.UserModel;
// 
// public class UITexturePacker : Editor
// {
//     static readonly string m_SrcAssetPath = "Assets\\UI";
//     static readonly string m_DstAssetPath = "Assets\\Resources\\UIPacked";
//     static readonly string m_UIPrefabPath = "Assets\\Resources\\UI\\Prefabs";
//     static readonly string m_UIPrefabPath2 = "Assets\\Resources\\UIFrame";
// 
//     static protected List<Sprite> m_SrcSpriteList = new List<Sprite>();
// 
//     public class SpriteDesc
//     {
//         public string m_Path ="";
//         public string m_FileID="";
//         public string m_GUID="";
//         public string m_Name="";
//         public SpriteMetaData m_MetaData;
//     }
// 
//     public class SpriteMetaDesc
//     {
//         public SpriteDesc m_NewSpriteDesc = new SpriteDesc();
//         public List<SpriteDesc> m_OriginSpriteList = new List<SpriteDesc>();
//     }
//     public class SpriteRepMetaDesc
//     {
//         public SpriteDesc oldSp = new SpriteDesc();
//         public SpriteDesc newSp = new SpriteDesc();
//     }
// 
//     static protected List<SpriteMetaDesc> m_DstSpriteList = new List<SpriteMetaDesc>();
//     static protected List<string> m_UIPrefabList = new List<string>();
// 
//     public static void UIFrameStatusAtText()
//     {
//         /// 生成PackedUI的guid，fileid
//         Dictionary<string, List<SpriteDesc>> dict = new Dictionary<string, List<SpriteDesc>>();
// 
//         string fromPacked = "Assets/Resources/UIPacked";
//         string toPacked   = "Assets/Resources/UIPacked1";
// 
//         EditorUtility.DisplayProgressBar("Begin fromPacked", fromPacked, 0);
// 
//         var guids = AssetDatabase.FindAssets("t:texture", new string[] { fromPacked });
//         int cnt = 0;
//         foreach (var guid in guids)
//         {
// 
//             var path = AssetDatabase.GUIDToAssetPath(guid);
//             dict.Add(path.ToLower(), new List<SpriteDesc>());
//             var list = dict[path.ToLower()];
// 
//             EditorUtility.DisplayProgressBar("Begin fromPacked", path, cnt++ * 1.0f / guids.Length);
// 
//             TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
//             if (ti.spriteImportMode == SpriteImportMode.Multiple)
//             {
//                 string[] lines = File.ReadAllLines(AssetDatabase.GetTextMetaFilePathFromAssetPath(path));
//                 var flag = false;
//                 foreach (var line in lines)
//                 {
//                     if (!flag)
//                     {
//                         if (line.Contains("fileIDToRecycleName"))
//                         {
//                             flag = true;
//                         }
//                     }
//                     else
//                     {
//                         if (!line.Contains("213"))
//                         {
//                             break;
//                         }
// 
//                         var desc = new SpriteDesc();
//                         desc.m_FileID = line.Split(':')[0].Trim();
//                         desc.m_GUID = guid;
//                         desc.m_Path = path;
//                         desc.m_Name = line.Split(':')[1].Trim();
// 
//                         list.Add(desc);
//                     }
//                 }
// 
//                 // 找到SpriteMeta
//                 foreach (var meta in ti.spritesheet)
//                 {
//                     var fItem = list.Find(x=>{ return x.m_Name == meta.name;} );
//                     if (null != fItem) { fItem.m_MetaData = meta; }
//                     else { UnityEngine.Debug.LogErrorFormat("can't find the {0}", meta.name); }
//                 }
// 
//             }
//             else if (ti.spriteImportMode == SpriteImportMode.Single)
//             {
//                 UnityEngine.Debug.LogErrorFormat("single mode do some thing {0}", path);
//                 var desc = new SpriteDesc();
//                 desc.m_FileID = "21300000";
//                 desc.m_GUID = guid;
//                 desc.m_Path = path;
//                 desc.m_Name = Path.GetFileNameWithoutExtension(path);
// 
//                 list.Add(desc);
//             }
//         }
// 
//         //EditorUtility.DisplayProgressBar("Replace Prefab", toPacked, 0);
//         cnt = 0;
// 
//         string uiFramePath = "Assets/Resources/UI/Prefabs";
//         string uiFramePath2 = "Assets/Resources/UIFrame";
// 
//         string[] prefabguids = AssetDatabase.FindAssets("t:prefab", new string[] {uiFramePath, uiFramePath2});
// 
//         string finalans = "AssetCount, PrefabPath, AssetPackedCount, AssetPackedPaths, AssetUnpackedPath\n";
// 
//         Dictionary<string, int> assetDict = new Dictionary<string, int>();
// 
//         for (int i = 0; i < prefabguids.Length; ++i)
//         {
//             assetDict.Clear();
// 
//             string path = AssetDatabase.GUIDToAssetPath(prefabguids[i]);
//             EditorUtility.DisplayProgressBar("Replace Prefab", path, cnt++*1.0f/prefabguids.Length);
// 
//             var lines = File.ReadAllLines(path);
//             for (int j = 0; j < lines.Length; ++j)
//             {
//                 if (lines[j].Contains("fileID: 213"))
//                 {
//                     //string fileId = _getID(lines[j], "fileID: ", 8);
//                     string guid = _getID(lines[j], "guid: ", 32);
// 
//                     if (!assetDict.ContainsKey(guid))
//                     {
//                         assetDict.Add(guid, 0);
//                     }
//                     assetDict[guid]++;
//                 }
//             }
// 
// 
//             finalans += assetDict.Count + ","+ path + ",";
// 
//             string part1 = "";
//             int part1cnt = 0;
//             string part2 = "";
//             int part2cnt = 0;
//             foreach (var item in assetDict)
//             {
//                 string assetPath = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(item.Key));
// 
//                 if (assetPath.StartsWith("up-"))
//                 {
//                     part1 += string.Format("{0}({1})|", assetPath, item.Value);
//                     part1cnt++;
//                 }
//                 if (assetPath.StartsWith("p-"))
//                 {
//                     part2 += string.Format("{0}({1})|", assetPath, item.Value);
//                     part2cnt++;
//                 }
//                 //finalans += string.Format(, , item.Value);
//             }
// 
//             finalans += part2cnt.ToString() +","+ part2 + "," +part1;
//             finalans += "\n";
// 
//         }
// 
//         File.WriteAllText("state.csv", finalans);
// 
//         EditorUtility.ClearProgressBar();
//     }
// 
// 
//     [MenuItem("[TM工具集]/ArtTools/UIFramesStatus #%&z")]
//     public static void UIFrameStatusAtBinary()
//     {
//         string uiFramePath = "Assets/Resources/UI/Prefabs";
//         string[] prefabguids = AssetDatabase.FindAssets("t:prefab", new string[] {uiFramePath});
//         int cnt = 0;
// 
//         string finalans = "AssetCount, PrefabPath, AssetPackedCount, AssetPackedPaths, AssetUnpackedPath\n";
//         Dictionary<string, int> assetDict = new Dictionary<string, int>();
// 
//         for (int i = 0; i < prefabguids.Length; ++i)
//         {
//             assetDict.Clear();
// 
//             string path = AssetDatabase.GUIDToAssetPath(prefabguids[i]);
// 
//             EditorUtility.DisplayProgressBar("Replace Prefab", path, cnt++*1.0f/prefabguids.Length);
// 
//             string[] deps = AssetDatabase.GetDependencies(path);
// 
//             int packedcnt = 0;
//             int unpackedcnt = 0;
//             string packedstr = "";
//             string unpackedstr = "";
// 
//             for (int j = 0; j < deps.Length; ++j)
//             {
//                 string depitem = deps[j];
// 
//                 TextureImporter ti = AssetImporter.GetAtPath(depitem) as TextureImporter;
//                 if (null != ti)
//                 {
//                     if (ti.spriteImportMode == SpriteImportMode.Multiple)
//                     {
//                         packedcnt++;
//                         packedstr += Path.GetFileName(depitem) + "|";
//                     }
//                     else if (ti.spriteImportMode == SpriteImportMode.Single)
//                     {
//                         unpackedcnt++;
//                         unpackedstr += Path.GetFileName(depitem) + "|";
//                     }
//                 }
//             }
//             finalans += string.Format("{0},{1},{2},{3},{4},{5}\n", packedcnt + unpackedcnt, path, packedcnt, packedstr, unpackedcnt, unpackedstr);
//         }
// 
//         File.WriteAllText("state.csv", finalans);
// 
//         EditorUtility.ClearProgressBar();
//     }
// 
//     private static string _getID(string str, string cmp, int len)
//     {
//         int idx = str.IndexOf(cmp);
//         return str.Substring(idx + cmp.Length, len);
//     }
// 
//     public static Dictionary<string, List<SpriteDesc>> _getDictFromPackedPath(string uiresourcePath)
//     {
//         /// 生成PackedUI的guid，fileid
//         Dictionary<string, List<SpriteDesc>> dict = new Dictionary<string, List<SpriteDesc>>();
// 
//         EditorUtility.DisplayProgressBar("Begin fromPacked", uiresourcePath, 0);
// 
//         var guids = AssetDatabase.FindAssets("t:texture", new string[] { uiresourcePath });
//         int cnt = 0;
//         foreach (var guid in guids)
//         {
//             var path = AssetDatabase.GUIDToAssetPath(guid);
//             if (!dict.ContainsKey(guid))
//             {
//                 dict.Add(guid, new List<SpriteDesc>());
//             }
//             var list = dict[guid];
// 
//             EditorUtility.DisplayProgressBar("Begin fromPacked", path, cnt++ * 1.0f / guids.Length);
// 
//             TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
//             if (ti.spriteImportMode == SpriteImportMode.Multiple)
//             {
//                 string[] lines = File.ReadAllLines(AssetDatabase.GetTextMetaFilePathFromAssetPath(path));
//                 var flag = false;
//                 foreach (var line in lines)
//                 {
//                     if (!flag)
//                     {
//                         if (line.Contains("fileIDToRecycleName"))
//                         {
//                             flag = true;
//                         }
//                     }
//                     else
//                     {
//                         if (!line.Contains("213"))
//                         {
//                             break;
//                         }
// 
//                         var desc = new SpriteDesc();
//                         desc.m_FileID = line.Split(':')[0].Trim();
//                         desc.m_GUID = guid;
//                         desc.m_Path = path;
//                         desc.m_Name = line.Split(':')[1].Trim();
// 
//                         list.Add(desc);
//                     }
//                 }
// 
//                 // 找到SpriteMeta
//                 foreach (var meta in ti.spritesheet)
//                 {
//                     var fItem = list.Find(x=>{ return x.m_Name == meta.name;} );
//                     if (null != fItem) { fItem.m_MetaData = meta; }
//                     else { UnityEngine.Debug.LogErrorFormat("can't find the {0}", meta.name); }
//                 }
// 
//             }
//             else if (ti.spriteImportMode == SpriteImportMode.Single)
//             {
//                 //UnityEngine.Debug.LogErrorFormat("single mode do some thing {0}", path);
//                 var desc = new SpriteDesc();
//                 desc.m_FileID = "21300000";
//                 desc.m_GUID = guid;
//                 desc.m_Path = path;
//                 desc.m_Name = Path.GetFileNameWithoutExtension(path);
//                 desc.m_MetaData = new SpriteMetaData();
//                 desc.m_MetaData.border = ti.spriteBorder;
// 
//                 list.Add(desc);
//             }
//         }
// 
//         EditorUtility.ClearProgressBar();
// 
//         return dict;
//     }
// 
//     public static Dictionary<string, string> _getJsonFile()
//     {
//         /// json file deal
//         byte[] bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/ui.json");
//         Hashtable packageInfo = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(bytes)) as Hashtable;
//         Dictionary<string, string> jsonDict = new Dictionary<string, string>();
// 
//         foreach (DictionaryEntry kvp in packageInfo)
//         {
//             Hashtable info = kvp.Value as Hashtable;
// 
//             var key = (string)info["path"]+":"+(string)info["name"];
//             if (!jsonDict.ContainsKey(key)) jsonDict.Add(key, (string)kvp.Key);
//             else 
//             {
//                 Debug.LogErrorFormat("d key {0}, {1}, {2}", key, jsonDict[key], (string)kvp.Key);
//             }
//         }
// 
//         return jsonDict;
//     }
// 
// 
// 
//     private static void _check(Dictionary<string, List<SpriteDesc>> dic)
//     {
//         foreach (var item in dic)
//         {
//             var path = AssetDatabase.GUIDToAssetPath(item.Key);
//             var ti = AssetImporter.GetAtPath(path) as TextureImporter;
//             
//             var list = item.Value;
//             for (int i = 0; i < ti.spritesheet.Length; ++i)
//             {
//                 var sprite = ti.spritesheet[i];
//                 var listitem = list[i];
// 
//                 if (sprite.name != listitem.m_Name)
//                 {
//                     Debug.LogErrorFormat("sprite. don't match {0}, {1}", sprite.name, listitem.m_Name);
//                 }
//             }
//         }
//     }
// 
//     [MenuItem("[TM工具集]/ArtTools/PackUIFromPacked")]
//     public static void PackUITexutureFromPacked()
//     {
//         string fromPacked = "Assets/Resources/UIPacked";
//         string toPacked = "Assets/Resources/UIPacked1";
// 
//         /// 生成PackedUI的guid，fileid
//         Dictionary<string, List<SpriteDesc>> dict = _getDictFromPackedPath(fromPacked);
//         Dictionary<string, List<SpriteDesc>> todict = _getDictFromPackedPath(toPacked);
// 
//         _check(dict);
//         _check(todict);
// 
//         Dictionary<string, string> jsonDict = _getJsonFile();
// 
//         int cnt = 0;
// 
//         /// 替换相关Prefab
//         string uiFramePath = "Assets/Resources/UI/Prefabs";
//         string uiFramePath2 = "Assets/Resources/UIFrame";
// 
//         string[] prefabguids = AssetDatabase.FindAssets("t:prefab", new string[] { uiFramePath2, uiFramePath});
// 
// 
//         EditorUtility.DisplayProgressBar("Replace Prefab", toPacked, 0);
//         cnt = 0;
// 
//         //prefabguids = new string[] { AssetDatabase.AssetPathToGUID("Assets/Resources/UIFrame/LoginFrame.prefab") };
// 
//         for (int i = 0; i < prefabguids.Length; ++i)
//         {
//             string metaPath = AssetDatabase.GUIDToAssetPath(prefabguids[i]);
// 
//             cnt++;
//             EditorUtility.DisplayProgressBar("Replace Prefab", metaPath, cnt*1.0f/prefabguids[i].Length);
// 
//             var lines = File.ReadAllLines(metaPath);
// 
//             List<SpriteRepMetaDesc> spriteDescList = new List<SpriteRepMetaDesc>();
// 
//             for (int j = 0; j < lines.Length; ++j)
//             {
//                 if (lines[j].Contains("fileID: 213"))
//                 {
//                     string fileId = _getID(lines[j], "fileID: ", 8);
//                     string guid = _getID(lines[j], "guid: ", 32);
//                     
//                     if (dict.ContainsKey(guid))
//                     {
//                         var list = dict[guid];
// 
//                         var fItem = list.Find(x=>{return x.m_FileID.ToLower() == fileId && x.m_GUID == guid; });
//                         if (null != fItem)
//                         {
//                             //Debug.LogErrorFormat("Name {0}", fItem.m_Name);
// 
//                             var key = CFileManager.EraseExtension(fItem.m_Path.Replace("Assets/Resources/", "")).ToLower()+":"+fItem.m_Name;
// 
//                             if (jsonDict.ContainsKey(key))
//                             {
//                                 //Debug.LogError("get the path " + jsonDict[key]);
// 
//                                 var finalFileName = Path.GetFileName(jsonDict[key]);
// 
//                                 var ttpath = "Assets/"+jsonDict[key]+".png";
//                                 TextureImporter spIm = AssetImporter.GetAtPath(ttpath) as TextureImporter;
// 
//                                 var desc = new SpriteRepMetaDesc();
//                                 spriteDescList.Add(desc);
//                                 desc.oldSp = fItem;
// 
//                                 if (jsonDict[key].ToLower().StartsWith("ui/image/icon"))
//                                 {
//                                     var obj = Resources.Load(jsonDict[key]);
//                                     var path = AssetDatabase.GetAssetPath(obj);
//                                     var singleGUID = AssetDatabase.AssetPathToGUID(path);
// 
//                                     TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
// 
//                                     desc.newSp = new SpriteDesc();
//                                     desc.newSp.m_FileID = "21300000";
//                                     desc.newSp.m_GUID = singleGUID;
//                                     var metaData = new SpriteMetaData();
//                                     desc.newSp.m_MetaData = metaData;
//                                     metaData.border = ti.spriteBorder;
//                                     metaData.pivot = ti.spritePivot;
// 
//                                     obj = null;
//                                 }
//                                 else
//                                 {
//                                     var finalSearch = AssetDatabase.FindAssets("t:sprite " + finalFileName, new string[] { toPacked });
// 
//                                     foreach (var fguid in finalSearch)
//                                     {
//                                         if (todict.ContainsKey(fguid))
//                                         {
//                                             var flist = todict[fguid];
// 
//                                             var fitem = flist.Find(x => { return x.m_Name.ToLower() == finalFileName.ToLower(); });
//                                             if (null != fitem)
//                                             {
//                                                 desc.newSp = fitem;
//                                                 desc.newSp.m_MetaData = new SpriteMetaData();
//                                                 desc.newSp.m_MetaData.border = spIm.spriteBorder;
//                                                 break;
//                                             }
//                                         }
//                                         else
//                                         {
//                                             Debug.LogWarningFormat("todict don't contain {0}, {1}", fguid, AssetDatabase.GUIDToAssetPath(fguid));
//                                         }
//                                     }
//                                 }
//                             }
//                             else
//                             {
//                                 Debug.LogWarningFormat("can't find the {0}", key);
//                             }
// 
//                         }
//                         else 
//                         {
//                             Debug.LogWarningFormat("can't find {0}", AssetDatabase.GUIDToAssetPath(guid));
//                         }
//                     }
//                     else 
//                     {
//                         Debug.LogWarningFormat("can't find guid {0}, {1}", guid, AssetDatabase.GUIDToAssetPath(guid));
// 
//                     }
//                 }
//             }
// 
//         _check(dict);
//         _check(todict);
//                 //var fkcnt = 0;
//                 //var fkl = File.ReadAllLines(metaPath);
// 
//                 //for (int j = 0; j < fkl.Length; ++j)
//                 //{
//                 //    if (fkl[j].Contains("fileID: 213"))
//                 //    {
//                 //        for (int k = 0; k < spriteDescList.Count; ++k)
//                 //        {
//                 //            var knode = spriteDescList[k];
//                 //            if (fkl[j].Contains(knode.oldSp.m_GUID) && fkl[j].Contains(knode.oldSp.m_FileID))
//                 //            {
//                 //                fkl[j] = fkl[j].Replace(knode.oldSp.m_FileID, knode.newSp.m_FileID);
//                 //                fkl[j] = fkl[j].Replace(knode.oldSp.m_GUID, knode.newSp.m_GUID);
//                 //                fkcnt++;
//                 //                break;
//                 //            }
// 
//                 //        }
//                 //    }
//                 //}
// 
//                 //if (fkcnt != spriteDescList.Count)
//                 //{
//                 //    Debug.LogErrorFormat("fkC {0}, {1}", fkcnt, spriteDescList.Count);
//                 //}
// 
//             var flines = File.ReadAllLines(metaPath);
//             for (int j = 0; j < flines.Length; ++j)
//             {
//                 for (int k = 0; k < spriteDescList.Count; ++k)
//                 {
//                     var knode = spriteDescList[k];
// 
//                     if (flines[j].Contains(knode.oldSp.m_GUID) && flines[j].Contains(knode.oldSp.m_FileID))
//                     {
//                         if (knode.newSp == null)
//                         {
//                             UnityEngine.Debug.LogErrorFormat("newsp is nil {0}", spriteDescList[k].oldSp.m_Path);
//                         }
//                         else
//                         {
//                             if (knode.newSp.m_FileID.Length <= 0)
//                             {
//                                 Debug.LogErrorFormat("m_FileID !!!!!!!!!!!!!!!!!!! {0}, {1}", knode.oldSp.m_Name, knode.oldSp.m_Path);
//                             }
// 
//                             if (knode.newSp.m_GUID.Length <= 0)
//                             {
//                                 Debug.LogErrorFormat("m_GUID !!!!!!!!!!!!!!!!!!! {0} {1}", knode.oldSp.m_Name, metaPath);
//                                 break;
//                             }
// 
//                             flines[j] = flines[j].Replace(knode.oldSp.m_FileID, knode.newSp.m_FileID);
//                             flines[j] = flines[j].Replace(knode.oldSp.m_GUID, knode.newSp.m_GUID);
// 
//                             _modifyMetaInfo(AssetDatabase.GetTextMetaFilePathFromAssetPath(knode.newSp.m_Path), knode.newSp.m_Name, knode.oldSp.m_MetaData.border);
//                         }
//                         break;
//                     }
//                 }
//             }
// 
//         _check(dict);
//         _check(todict);
// 
//             //var writePath = metaPath.Replace("Assets/Resources", "Assets/NewUI");
//             var writePath = metaPath;
//             var direxist = Path.GetDirectoryName(writePath);
//             if (!Directory.Exists(direxist))
//             {
//                 Directory.CreateDirectory(direxist);
//             }
// 
//             File.WriteAllLines(writePath, flines);
//         }
// 
//         EditorUtility.ClearProgressBar();
//     }
// 
//     public static void _modifyMetaInfo(string path, string keyname, Vector4 border)
//     {
//         if (!File.Exists(path))
//         {
//             Debug.LogError("not exist " + path);
//             return;
//         }
// 
//         FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
//         StreamReader sr = new StreamReader(stream, Encoding.UTF8);
// 
//         YamlStream yamlStream = new YamlStream();
//         yamlStream.Load(sr);
// 
//         YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
// 
//         var spritesNode = UITexturePacker._FindYAMLHierarchyNode("sprites", rootNode);
// 
//         YamlSequenceNode sprites = spritesNode as YamlSequenceNode;
// 
//         if (null != sprites)
//         {
//             foreach (var sprite in sprites.Children)
//             {
//                 YamlMappingNode spriteAttris = (YamlMappingNode)sprite;
//                 foreach (var spriteAttri in spriteAttris.Children)
//                 {
//                     if (spriteAttri.Key.ToString() == "name")
//                     {
//                         YamlScalarNode name = (YamlScalarNode)spriteAttri.Value;
//                         if (name.ToString() == keyname)
//                         {
//                             YamlMappingNode boarderNode = UITexturePacker._FindYAMLHierarchyNode("border", sprite) as YamlMappingNode;
//                             if (null != boarderNode)
//                             {
//                                 if (null != border)
//                                 {
//                                     foreach (var boarder in boarderNode.Children)
//                                     {
//                                         YamlScalarNode key = boarder.Key as YamlScalarNode;
//                                         YamlScalarNode value = boarder.Value as YamlScalarNode;
//                                         if (key.Value == "x")
//                                             value.Value = border.x.ToString();
//                                         else if (key.Value == "y")
//                                             value.Value = border.y.ToString();
//                                         else if (key.Value == "z")
//                                             value.Value = border.z.ToString();
//                                         else if (key.Value == "w")
//                                             value.Value = border.w.ToString();
//                                         else
//                                             ;
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
//         }
// 
//         stream.Close();
// 
//         FileStream streamW = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
//         StreamWriter sw = new StreamWriter(streamW, Encoding.UTF8);
//         yamlStream.Save(sw, false);
// 
//         streamW.Flush();
// 
//         sw.Close();
//         streamW.Close();
//     }
//     
//     [MenuItem("[TM工具集]/ArtTools/Pack UI Textures 2")]
// 
//     public static void PackUITextures2()
//     {
//         /// 生成PackedUI的guid，fileid
//         Dictionary<string, List<SpriteDesc>> dict = new Dictionary<string, List<SpriteDesc>>();
// 
//         var guids = AssetDatabase.FindAssets("t:texture", new string[] { m_DstAssetPath });
//         foreach (var guid in guids)
//         {
//             var path = AssetDatabase.GUIDToAssetPath(guid);
//             dict.Add(path.ToLower(), new List<SpriteDesc>());
//             var list = dict[path.ToLower()];
// 
//             TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
//             if (ti.spriteImportMode == SpriteImportMode.Multiple)
//             {
//                 string[] lines = File.ReadAllLines(AssetDatabase.GetTextMetaFilePathFromAssetPath(path));
//                 var flag = false;
//                 foreach (var line in lines)
//                 {
//                     if (!flag)
//                     {
//                         if (line.Contains("fileIDToRecycleName"))
//                         {
//                             flag = true;
//                         }
//                     }
//                     else
//                     {
//                         if (!line.Contains("213"))
//                         {
//                             break;
//                         }
// 
//                         var desc = new SpriteDesc();
//                         desc.m_FileID = line.Split(':')[0].Trim();
//                         desc.m_GUID = guid;
//                         desc.m_Path = path;
//                         desc.m_Name = line.Split(':')[1].Trim();
// 
//                         list.Add(desc);
//                     }
//                 }
//             }
//             else if (ti.spriteImportMode == SpriteImportMode.Single)
//             {
//                 var desc = new SpriteDesc();
//                 desc.m_FileID = "21300000";
//                 desc.m_GUID = guid;
//                 desc.m_Path = path;
//                 desc.m_Name = Path.GetFileNameWithoutExtension(path);
// 
//                 list.Add(desc);
//             }
//         }
// 
//         /// 替换相关Prefab
//         List<string> fileList = new List<string>();
//         string[] pathList = Directory.GetFiles(m_UIPrefabPath, "*.prefab", SearchOption.AllDirectories);
//         fileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_UIPrefabPath2, "*.prefab", SearchOption.AllDirectories);
//         fileList.AddRange(pathList);
// 
//         for (int i = 0; i < fileList.Count; ++i)
//         {
//             string[] deps = AssetDatabase.GetDependencies(fileList[i]);
//             List<SpriteRepMetaDesc> spriteDescList = new List<SpriteRepMetaDesc>();
// 
//             foreach (string depspath in deps)
//             {
//                 if (depspath.Contains("Assets/UI/Image")
//                     && (depspath.Contains("jpg") || depspath.Contains("png"))
//                     )
//                 {
//                     SpriteRepMetaDesc desc = new SpriteRepMetaDesc();
// 
//                     desc.oldSp.m_Path = depspath;
//                     desc.oldSp.m_GUID = AssetDatabase.AssetPathToGUID(depspath);
//                     desc.oldSp.m_FileID = "21300000";
// 
//                     UIPackageInfo oppath = AssetUIPackage.instance.FindUIPackage(CFileManager.EraseExtension(depspath.Replace("Assets/", "").ToLower()));
//                     var pngPath = "Assets/Resources/" + oppath.m_ImageRes + ".png";
//                     var jpgPath = "Assets/Resources/" + oppath.m_ImageRes + ".jpg";
// 
//                     string finalPath = pngPath;
// 
//                     if (File.Exists(jpgPath)) finalPath = jpgPath;
//                     if (File.Exists(pngPath)) finalPath = pngPath;
// 
//                     //TextureImporter ti = AssetImporter.GetAtPath(finalPath) as TextureImporter;
//                     if (dict.ContainsKey(finalPath.ToLower()))
//                     {
//                         var desclist = dict[finalPath.ToLower()];
//                         desc.newSp = desclist.Find(x =>
//                         {
//                             return x.m_Name == oppath.m_SpriteName;
//                         });
//                     }
//                     else
//                     {
//                         UnityEngine.Debug.LogErrorFormat("don't contain the {0}", finalPath);
//                     }
// 
//                     spriteDescList.Add(desc);
//                 }
//             }
// 
//             var lines = File.ReadAllLines(fileList[i]);
//             for (int j = 0; j < lines.Length; ++j)
//             {
//                 for (int k = 0; k < spriteDescList.Count; ++k)
//                 {
//                     if (lines[j].Contains(spriteDescList[k].oldSp.m_GUID) && lines[j].Contains("fileID: 213"))
//                     {
//                         if (spriteDescList[k].newSp == null)
//                         {
//                             UnityEngine.Debug.LogErrorFormat("newsp is nil {0}", spriteDescList[k].oldSp.m_Path);
//                         }
//                         else
//                         {
//                             lines[j] = lines[j].Replace("fileID: 21300000", string.Format("fileID: {0}", spriteDescList[k].newSp.m_FileID));
//                             lines[j] = lines[j].Replace(spriteDescList[k].oldSp.m_GUID, spriteDescList[k].newSp.m_GUID);
//                         }
//                         break;
//                     }
//                 }
//             }
// 
//             var writePath = fileList[i].Replace("Assets\\", "");
//             var direxist = Path.GetDirectoryName(writePath);
//             if (!Directory.Exists(direxist))
//             {
//                 Directory.CreateDirectory(direxist);
//             }
// 
//             File.WriteAllLines(writePath, lines);
//         }
//     }
// 
//     [MenuItem("[TM工具集]/ArtTools/Pack UI Textures")]
//     public static void PackUITextures()
//     {
//         m_SrcSpriteList.Clear();
//         m_DstSpriteList.Clear();
// 
//         string[] pathList = null;
//         List <string> srcFileList = new List<string>();
//         pathList = Directory.GetFiles(m_SrcAssetPath, "*.png", SearchOption.AllDirectories);
//         srcFileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_SrcAssetPath, "*.tga", SearchOption.AllDirectories);
//         srcFileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_SrcAssetPath, "*.jpg", SearchOption.AllDirectories);
//         srcFileList.AddRange(pathList);
// 
//         for (int i = 0; i < srcFileList.Count; ++i)
//         {
//             if (srcFileList[i].Contains(m_DstAssetPath))
//                 continue;
// 
//             Sprite curSprite = AssetDatabase.LoadAssetAtPath<Sprite>(srcFileList[i]);
//             if (null != curSprite)
//                 m_SrcSpriteList.Add(curSprite);
//         }
// 
// 
//         List<string> dstFileList = new List<string>();
//         pathList = Directory.GetFiles(m_DstAssetPath, "*.png", SearchOption.AllDirectories);
//         dstFileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_DstAssetPath, "*.tga", SearchOption.AllDirectories);
//         dstFileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_DstAssetPath, "*.jpg", SearchOption.AllDirectories);
//         dstFileList.AddRange(pathList);
// 
//         for (int i = 0; i < dstFileList.Count; ++i)
//         {
//             Sprite curSprite = AssetDatabase.LoadAssetAtPath<Sprite>(dstFileList[i]);
// 
//             if (null != curSprite)
//             {
//                 SpriteMetaDesc newMetaDsc = new SpriteMetaDesc();
//                 newMetaDsc.m_NewSpriteDesc.m_Path = dstFileList[i];
//                 newMetaDsc.m_NewSpriteDesc.m_GUID = AssetDatabase.AssetPathToGUID(dstFileList[i]);
//                 m_DstSpriteList.Add(newMetaDsc);
//             }
//         }
//         
//         for (int i = 0; i < m_DstSpriteList.Count; ++i)
//         {
//             string dstMeta = m_DstSpriteList[i].m_NewSpriteDesc.m_Path + ".meta";
//             FileStream stream = new FileStream(dstMeta, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
//             StreamReader sr = new StreamReader(stream, Encoding.Default);
// 
//             YamlStream yamlStream = new YamlStream();
//             yamlStream.Load(sr);
// 
//             YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
// 
//             Dictionary<string, string> nameToFIDMap = new Dictionary<string, string>();
//             YamlNode spriteToNameNode = _FindYAMLHierarchyNode("fileIDToRecycleName", rootNode);
//             YamlMappingNode spriteToNameNodeMap = spriteToNameNode as YamlMappingNode;
//             if (null != spriteToNameNodeMap)
//             {
//                 foreach (var s2n in spriteToNameNodeMap.Children)
//                     nameToFIDMap[s2n.Value.ToString()] = s2n.Key.ToString();
//             }
// 
//             YamlNode spritesNode = _FindYAMLHierarchyNode("sprites", rootNode);
//             YamlSequenceNode sprites = spritesNode as YamlSequenceNode;
//             if (null != sprites)
//             {
//                 foreach (var sprite in sprites.Children)
//                 {
//                     YamlMappingNode spriteAttris = (YamlMappingNode)sprite;
//                     foreach (var spriteAttri in spriteAttris.Children)
//                     {
//                         if (spriteAttri.Key.ToString() == "name")
//                         {
//                             YamlScalarNode name = (YamlScalarNode)spriteAttri.Value;
//                             YamlMappingNode boarderNode = _FindYAMLHierarchyNode("border", sprite) as YamlMappingNode;
//                             if (null != boarderNode)
//                             {
//                                 Sprite srcSprite = _FindSpriteByName(name.ToString(), m_SrcSpriteList);
//                                 if (null != srcSprite)
//                                 {
//                                     foreach (var boarder in boarderNode.Children)
//                                     {
//                                         YamlScalarNode key = boarder.Key as YamlScalarNode;
//                                         YamlScalarNode value = boarder.Value as YamlScalarNode;
//                                         if (key.Value == "x")
//                                             value.Value = srcSprite.border.x.ToString();
//                                         else if (key.Value == "y")
//                                             value.Value = srcSprite.border.y.ToString();
//                                         else if (key.Value == "z")
//                                             value.Value = srcSprite.border.z.ToString();
//                                         else if (key.Value == "w")
//                                             value.Value = srcSprite.border.w.ToString();
//                                         else
//                                             ;
//                                     }
// 
//                                     SpriteDesc newOrginSprite = new SpriteDesc();
//                                     newOrginSprite.m_Path = name.ToString();
//                                     newOrginSprite.m_FileID = nameToFIDMap[newOrginSprite.m_Path];
//                                     newOrginSprite.m_GUID =AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath(srcSprite) );
//                                     m_DstSpriteList[i].m_OriginSpriteList.Add(newOrginSprite);
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
// 
//             stream.Close();
// 
//             ////FileStream streamW = new FileStream(dstMeta, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
//             //StreamWriter sw = new StreamWriter(streamW);
//             //yamlStream.Save(sw);
// 
//             //streamW.Flush();
// 
//             //sw.Close();
//             //streamW.Close();
//         }
// 
//         /// 替换相关Prefab
//         List<string> fileList = new List<string>();
//         pathList = Directory.GetFiles(m_UIPrefabPath, "*.prefab", SearchOption.AllDirectories);
//         fileList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_UIPrefabPath2, "*.prefab", SearchOption.AllDirectories);
//         fileList.AddRange(pathList);
// 
//         for (int i = 0; i < fileList.Count; ++i)
//         {
//             FileStream stream = new FileStream(fileList[i], FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
//             StreamReader sr = new StreamReader(stream, Encoding.GetEncoding(936));
// 
//             List<string> yamlDataList = new List<string>();
//             string line = null;
//             string pattern = "guid: ";
//             string patternFID = "fileID: ";
//             while ((line = sr.ReadLine()) != null)
//             {
//                 if(line.Contains("fileID: 213"))
//                 {
//                     int guidIdx = line.IndexOf(pattern);
//                     if(guidIdx < line.Length)
//                     {
//                         string newFID = "";
// 
//                         string oldGuid = line.Substring(guidIdx + pattern.Length, 32);
//                         string newGuid = _FindDstSpriteGUID(oldGuid, m_DstSpriteList,ref newFID);
//                         line = line.Replace(oldGuid, newGuid);
// 
//                         /// 替换FileID
//                         if(!string.IsNullOrEmpty(newFID))
//                         {
//                             int fileIDIdx = line.IndexOf(patternFID);
//                             if (fileIDIdx < line.Length)
//                             {
//                                 string oldFileID = line.Substring(fileIDIdx + patternFID.Length, 8);
//                                 line = line.Replace(oldFileID, newFID);
//                             }
//                         }
//                     }
//                 }
//                 yamlDataList.Add(line);
//             }
// 
//             stream.Close();
// 
//             FileStream streamW = new FileStream(fileList[i], FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
//             StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
//             for(int j = 0; j < yamlDataList.Count; ++ j )
//             {
//                 sw.WriteLine(yamlDataList[j]);
//             }
//             streamW.Flush();
//             
//             sw.Close();
//             streamW.Close();
//         }
//     }
// 
//     public static YamlNode _FindYAMLHierarchyNode(string key, YamlNode rootNode)
//     {
//         if (null == rootNode)
//             return null;
//         
//         if(typeof(YamlMappingNode) == rootNode.GetType())
//         {
//             YamlMappingNode root = rootNode as YamlMappingNode;
//         
//             foreach (var entry in root.Children)
//             {
//                 if (entry.Key.ToString() == key)
//                 {
//                     return entry.Value;
//                 }
//                 else
//                 {
//                     YamlNode res = _FindYAMLHierarchyNode(key, entry.Value);
//                     if (null != res)
//                         return res;
//                 }
//             }
//         }
//         else if (typeof(YamlSequenceNode) == rootNode.GetType())
//         {
//             YamlSequenceNode root = rootNode as YamlSequenceNode;
// 
//             foreach (var entry in root.Children)
//             {
//                 if (entry.ToString() == key)
//                 {
//                     return entry;
//                 }
//                 else
//                 {
//                     YamlNode res = _FindYAMLHierarchyNode(key, entry);
//                     if (null != res)
//                         return res;
//                 }
//             }
//         }
//         else
//         {
//             if (rootNode.ToString() == key)
//                 return rootNode;
//         }
// 
//         return null;
//     }
// 
// 
//     protected static Sprite _FindSpriteByName(string name,List<Sprite> spriteList)
//     {
//         for(int i = 0; i < spriteList.Count; ++ i)
//         {
//             if (spriteList[i].name == name)
//                 return spriteList[i];
//         }
// 
//         return null;
//     }
// 
//     protected static string _FindDstSpriteGUID(string guid, List<SpriteMetaDesc> spriteMetaList,ref string fileID)
//     {
//         for (int i = 0; i < spriteMetaList.Count; ++i)
//         {
//             List<SpriteDesc> spriteDescList = spriteMetaList[i].m_OriginSpriteList;
//             for (int j = 0; j < spriteDescList.Count; ++ j)
//             {
//                 if(guid == spriteDescList[j].m_GUID)
//                 {
//                     fileID = spriteDescList[j].m_FileID;
//                     return spriteMetaList[i].m_NewSpriteDesc.m_GUID;
//                 }
//             }
//         }
// 
//         return guid;
//     }
// }
// 
// 
// public class UITextureRenamer : Editor
// {
//     static readonly string m_AssetPath = "Assets\\Resources\\UI\\SkillIcon";
//     static protected List<string> m_SrcSpriteList = new List<string>();
// 
//     protected class FileDesc
//     {
//         public string m_Name;
//         public string m_Path;
//         public int m_SameCnt = 0;
//     }
// 
//     static protected List<FileDesc> m_RenamedSpriteList = new List<FileDesc>();
// 
//     [MenuItem("[TM工具集]/ArtTools/Check UI Textures Name")]
//     static public void CheckAndRenameUITexture()
//     {
//         m_SrcSpriteList.Clear();
//         m_RenamedSpriteList.Clear();
// 
//         //EditorUtility.DisplayProgressBar(string title, string info, float progress);
// 
//         string[] pathList = null;
//         pathList = Directory.GetFiles(m_AssetPath, "*.png", SearchOption.AllDirectories);
//         m_SrcSpriteList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_AssetPath, "*.tga", SearchOption.AllDirectories);
//         m_SrcSpriteList.AddRange(pathList);
//         pathList = Directory.GetFiles(m_AssetPath, "*.jpg", SearchOption.AllDirectories);
//         m_SrcSpriteList.AddRange(pathList);
// 
//         for(int i = 0; i < m_SrcSpriteList.Count; ++ i)
//         {
//             _RenameAndAddToList(m_SrcSpriteList[i]);
//         }
// 
//         /// BuffTable
//         string[] feildList_BuffTable = new string[] { "Icon" };
//         string[] expathList_BuffTable = new string[] { "UI\\Buff\\" };
//         _RenameResInXlsxTable("BuffTable", feildList_BuffTable, expathList_BuffTable);
// 
//         string[] feildList_DungeonTable = new string[] { "ThumbIconPath" };
//         string[] expathList_DungeonTable = new string[] { "" };
//         _RenameResInXlsxTable("DungeonTable", feildList_DungeonTable, expathList_DungeonTable);
// 
//         string[] feildList_ResTable = new string[] { "IconPath" };
//         string[] expathList_ResTable = new string[] { "" };
//         _RenameResInXlsxTable("ResTable", feildList_ResTable, expathList_ResTable);
// 
//         string[] feildList_NpcTable = new string[] { "NpcIcon" , "NpcBody" , "NpcTitleIcon" , "FunctionIcon" };
//         string[] expathList_NpcTable = new string[] { "", "" , "" , "" };
//         _RenameResInXlsxTable("NpcTable", feildList_NpcTable, expathList_NpcTable);
// 
//         string[] feildList_ItemTable = new string[] { "Icon" };
//         string[] expathList_ItemTable = new string[] { "UI\\ICON\\Item" };
//         _RenameResInXlsxTable("ItemTable", feildList_ItemTable, expathList_ItemTable);
// 
//         string[] feildList_SkillTable = new string[] { "Icon" };
//         string[] expathList_SkillTable = new string[] { "" };
//         _RenameResInXlsxTable("SkillTable", feildList_SkillTable, expathList_SkillTable);
// 
//         string[] feildList_PkLevelTable = new string[] { "Path" };
//         string[] expathList_PkLevelTable = new string[] { "" };
//         _RenameResInXlsxTable("PkLevelTable", feildList_PkLevelTable, expathList_PkLevelTable);
// 
//         string[] feildList_MissionTable = new string[] { "MissionNpcIcon" };
//         string[] expathList_MissionTable = new string[] { "" };
//         _RenameResInXlsxTable("MissionTable", feildList_MissionTable, expathList_MissionTable);
// 
//         string[] feildList_CommonTipsDesc = new string[] { "TitleIcon" };
//         string[] expathList_CommonTipsDesc = new string[] { "" };
//         _RenameResInXlsxTable("CommonTipsDesc", feildList_CommonTipsDesc, expathList_CommonTipsDesc);
// 
//         string[] feildList_JobTable = new string[] { "JobPortrayal" , "JobHead" , "JobIcon" , "SkillIcon" };
//         string[] expathList_JobTable = new string[] {            "" ,        "" ,        "" ,          "" };
//         _RenameResInXlsxTable("JobTable", feildList_JobTable, expathList_JobTable);
//     }
// 
//     static protected void _RenameAndAddToList(string path)
//     {
//         string filename = Path.GetFileNameWithoutExtension(path);
//         string pattern = "_rename";
//         int subIdx = filename.LastIndexOf(pattern);
//         string name = filename;
//         if (0 <= subIdx && subIdx < filename.Length)
//             name = filename.Substring(0, subIdx);
// 
//         string ext = Path.GetExtension(path);
//         FileDesc newFileDesc = null;
//         for (int i = 0; i < m_RenamedSpriteList.Count; ++i)
//         {
//             FileDesc curSpriteDesc = m_RenamedSpriteList[i];
//             if (curSpriteDesc.m_Name == name)
//             {
//                 string newName = curSpriteDesc.m_Name + "_rename" + (curSpriteDesc.m_SameCnt++).ToString("00");
//                 AssetDatabase.RenameAsset(path, newName);
// 
//                 return;
//             }
//         }
// 
//         newFileDesc = new FileDesc();
//         newFileDesc.m_Name = name;
//         newFileDesc.m_Path = path;
//         m_RenamedSpriteList.Add(newFileDesc);
//     }
// 
//     static protected void _RenameResInXlsxTable(string tableName, string[] feildList, string[] expathList)
//     {
//         XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(tableName);
//         if (null != bufftbl)
//         {
//             for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < bufftbl.RowCount; ++i)
//             {
//                 Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(i);
// 
//                 for (int feild = 0; feild < feildList.Length; ++feild)
//                 {
//                     ICell curCell = tbl[feildList[feild]];
//                     if(null == curCell)
//                         continue;
// 
//                     string file = curCell.CustomToString();
//                     if(string.IsNullOrEmpty(file))
//                         continue;
// 
//                     string[] fileList = file.Split('|');
//                     for(int split = 0; split < fileList.Length; ++ split)
//                     {
//                         if (!string.IsNullOrEmpty(file) && "-" != file)
//                         {
//                             Object res = Resources.Load(expathList[feild] + file);
//                             if (null == res)
//                             {
//                                 for (int j = 0; j < 100; ++j)
//                                 {
//                                     string fileName = file + "_rename" + j.ToString("00");
//                                     res = Resources.Load(expathList[feild] + fileName);
//                                     if (null != res)
//                                     {
//                                         curCell.SetCellValue(fileName);
//                                         Resources.UnloadAsset(res);
//                                         break;
//                                     }
//                                 }
//                             }
//                             else
//                                 Resources.UnloadAsset(res);
//                         }
//                     }
//                 }
//             }
// 
//             /// 保存
//             bufftbl.Write();
//             /// 转表
//             bufftbl.GetText();
//         }
//         else
//         {
//             Logger.LogWarningFormat("Can not find table {0}!", tableName);
//         }
//     }
// 
// 
//     [MenuItem("[TM工具集]/ArtTools/Rename Table Sprite")]
//     static public void RenameUISprite()
//     {
//         ///// BuffTable
//         //string[] feildList_BuffTable = new string[] { "Icon" };
//         //string[] expathList_BuffTable = new string[] { "UI\\Buff\\" };
//         //_RenameSpriteInXlsxTable("BuffTable", feildList_BuffTable, expathList_BuffTable);
//         //
//         //string[] feildList_DungeonTable = new string[] { "ThumbIconPath" };
//         //string[] expathList_DungeonTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("DungeonTable", feildList_DungeonTable, expathList_DungeonTable);
//         //
//         //string[] feildList_ResTable = new string[] { "IconPath" };
//         //string[] expathList_ResTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("ResTable", feildList_ResTable, expathList_ResTable);
//         //
//         //string[] feildList_NpcTable = new string[] { "NpcIcon" , "NpcBody" , "NpcTitleIcon" , "FunctionIcon" };
//         //string[] expathList_NpcTable = new string[] { "", "" , "" , "" };
//         //_RenameSpriteInXlsxTable("NpcTable", feildList_NpcTable, expathList_NpcTable);
//         //
//         //string[] feildList_ItemTable = new string[] { "Icon" };
//         //string[] expathList_ItemTable = new string[] { "UI\\ICON\\Item" };
//         //_RenameSpriteInXlsxTable("ItemTable", feildList_ItemTable, expathList_ItemTable);
//         //
//         //string[] feildList_SkillTable = new string[] { "Icon" };
//         //string[] expathList_SkillTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("SkillTable", feildList_SkillTable, expathList_SkillTable);
//         //
//         //string[] feildList_PkLevelTable = new string[] { "Path" };
//         //string[] expathList_PkLevelTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("PkLevelTable", feildList_PkLevelTable, expathList_PkLevelTable);
//         //
//         //string[] feildList_MissionTable = new string[] { "MissionNpcIcon" };
//         //string[] expathList_MissionTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("MissionTable", feildList_MissionTable, expathList_MissionTable);
//         //
//         //string[] feildList_CommonTipsDesc = new string[] { "TitleIcon" };
//         //string[] expathList_CommonTipsDesc = new string[] { "" };
//         //_RenameSpriteInXlsxTable("CommonTipsDesc", feildList_CommonTipsDesc, expathList_CommonTipsDesc);
//         //
//         //string[] feildList_JobTable = new string[] { "JobPortrayal" , "JobHead" , "JobIcon" , "SkillIcon" };
//         //string[] expathList_JobTable = new string[] {            "" ,        "" ,        "" ,          "" };
//         //_RenameSpriteInXlsxTable("JobTable", feildList_JobTable, expathList_JobTable);
// 
// 
//         //string[] feildList_CitySceneTable = new string[] { "NamePath"};
//         //string[] expathList_CitySceneTable = new string[] {        ""};
//         //_RenameSpriteInXlsxTable("CitySceneTable", feildList_CitySceneTable, expathList_CitySceneTable);
//         //
//         //string[] feildList_AreaTable = new string[] { "LoadBG" };
//         //string[] expathList_AreaTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("AreaTable", feildList_AreaTable, expathList_AreaTable);
//         //
//         //string[] feildList_DungeonTable = new string[] { "LoadingBgPath", "ThumbIconPath" };
//         //string[] expathList_DungeonTable = new string[] { "" , "" };
//         //_RenameSpriteInXlsxTable("DungeonTable", feildList_DungeonTable, expathList_DungeonTable);
// 
//         //string[] feildList_ItemTable = new string[] { "Icon" };
//         //string[] expathList_ItemTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("ItemTable", feildList_ItemTable, expathList_ItemTable);
// 
//         //string[] feildList_PkLevelTable = new string[] { "Path"};
//         //string[] expathList_PkLevelTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("PkLevelTable", feildList_PkLevelTable, expathList_PkLevelTable);
//         //
//         //string[] feildList_NpcTable = new string[] { "FunctionIcon" };
//         //string[] expathList_NpcTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("NpcTable", feildList_NpcTable, expathList_NpcTable);
//         //
//         //string[] feildList_ShopTable = new string[] { "ShopNamePath" };
//         //string[] expathList_ShopTable = new string[] { "" };
//         //_RenameSpriteInXlsxTable("ShopTable", feildList_ShopTable, expathList_ShopTable);
//         //
//         //string[] feildList_JobTable = new string[] { "JobPortrayal", "JobHead" , "JobIcon", "AwakeJobIcon" , "AwakeJobName", "AwakeSkillName" };
//         //string[] expathList_JobTable = new string[] { "" ,"", "", "", "", "" };
//         //_RenameSpriteInXlsxTable("JobTable", feildList_JobTable, expathList_JobTable);
//     }
// 
//     static protected void _RenameSpriteInXlsxTable(string tableName, string[] feildList, string[] expathList)
//     {
//         XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(tableName);
//         if (null != bufftbl)
//         {
//             for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < bufftbl.RowCount; ++i)
//             {
//                 Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(i);
// 
//                 for (int feild = 0; feild < feildList.Length; ++feild)
//                 {
//                     ICell curCell = tbl[feildList[feild]];
//                     if (null == curCell)
//                         continue;
// 
//                     string file = curCell.CustomToString();
//                     if (string.IsNullOrEmpty(file))
//                         continue;
// 
//                     string[] fileList = file.Split('|');
//                     for (int split = 0; split < fileList.Length; ++split)
//                     {
//                         if (!string.IsNullOrEmpty(file) && "-" != file)
//                         {
//                             Object res = Resources.Load(expathList[feild] + file);
//                             if (null == res)
//                             {
//                                 string resKey = PathUtil.EraseExtension(expathList[feild] + file).ToLower();
//                                 UIPackageInfo uiPackage = AssetUIPackage.instance.FindUIPackage(resKey);
// 
//                                 if (null != uiPackage)
//                                 {
//                                     string fileName = "";
// 
//                                     if(string.IsNullOrEmpty(expathList[feild]))
//                                         fileName = uiPackage.m_ImageRes + ":" + uiPackage.m_SpriteName;
//                                     else
//                                         fileName = Path.GetFileName(uiPackage.m_ImageRes) + ":" + uiPackage.m_SpriteName;
// 
//                                     Sprite dst = AssetLoader.instance.LoadRes(expathList[feild] + fileName, typeof(Sprite)).obj as Sprite;
//                                     if (null != dst)
//                                     {
//                                         curCell.SetCellValue(fileName);
//                                         Resources.UnloadAsset(dst);
//                                     }
//                                 }
//                             }
//                             else
//                                 Resources.UnloadAsset(res);
//                         }
//                     }
//                 }
//             }
// 
//             /// 保存
//             bufftbl.Write();
//             /// 转表
//             bufftbl.GetText();
//         }
//         else
//         {
//             Logger.LogWarningFormat("Can not find table {0}!", tableName);
//         }
//     }
// 
//     ///////////////////////////////////////////////////////////////////////////////////////////
//     /// <summary>
//     /// 
//     /// </summary>
// 
//     [MenuItem("[TM工具集]/ArtTools/Rename Skill Icon")]
//     static void _RenameSkillIconName()
//     {
// 
//         List<string> srcSpriteList = new List<string>();
//         string assetPath = "Assets/Resources/UI/Image/Icon/Icon_skillIcon";
//         
//         string[] pathList = null;
//         pathList = Directory.GetFiles(assetPath, "*.png", SearchOption.AllDirectories);
//         srcSpriteList.AddRange(pathList);
//         pathList = Directory.GetFiles(assetPath, "*.tga", SearchOption.AllDirectories);
//         srcSpriteList.AddRange(pathList);
//         pathList = Directory.GetFiles(assetPath, "*.jpg", SearchOption.AllDirectories);
//         srcSpriteList.AddRange(pathList);
//         
//         for (int i = 0; i < srcSpriteList.Count; ++i)
//         {
//             srcSpriteList[i] = srcSpriteList[i].Replace('\\', '/');
//             string pathListTemp = srcSpriteList[i].Replace("Assets/Resources/UI/Image/Icon/Icon_skillIcon/", null);
//             string[] strTbl = pathListTemp.Split('/');
//         
//             string final = srcSpriteList[i];
//             string oldName = Path.GetFileNameWithoutExtension(final);
//         
//         
//             string prefix = "";
//             string dir = Path.GetDirectoryName(final);
//             if(strTbl.Length > 0)
//             {
//                 prefix = strTbl[0];
//                 if (!oldName.Contains(prefix))
//                 {
//                     final = prefix + "_" + oldName;
//                     string res = AssetDatabase.RenameAsset(srcSpriteList[i], final);
//                 }
//             }
//         }
// 
//         string[] feildList_SkillTable = new string[] { "Icon" };
//         string[] expathList_SkillTable = new string[] { "" };
//         _RenameSkillIconInXlsxTable("SkillTable", feildList_SkillTable, expathList_SkillTable);
//     }
// 
// 
//     static protected void _RenameSkillIconInXlsxTable(string tableName, string[] feildList, string[] expathList)
//     {
//         XlsxDataUnit bufftbl = XlsxDataManager.Instance().GetXlsxByName(tableName);
//         if (null != bufftbl)
//         {
//             for (int i = XlsxDataUnit.XLS_DATA_INDEX; i < bufftbl.RowCount; ++i)
//             {
//                 Dictionary<string, ICell> tbl = bufftbl.GetRowDataByLine(i);
// 
//                 for (int feild = 0; feild < feildList.Length; ++feild)
//                 {
//                     ICell curCell = tbl[feildList[feild]];
//                     if (null == curCell)
//                         continue;
// 
//                     string file = curCell.CustomToString();
//                     if (string.IsNullOrEmpty(file))
//                         continue;
// 
//                     string[] fileList = file.Split('|');
//                     for (int split = 0; split < fileList.Length; ++split)
//                     {
//                         if (!string.IsNullOrEmpty(file) && "-" != file)
//                         {
//                             string resKey = expathList[feild] + file;
// 
// 
//                             string pathListTemp = resKey.Replace("UI/Image/Icon/Icon_skillIcon/", null);
//                             string[] strTbl = pathListTemp.Split('/');
//                             
//                             string oldName = Path.GetFileName(resKey);
//                             string prefix = "";
//                             string dir = Path.GetDirectoryName(resKey);
//                             if (strTbl.Length > 0)
//                             {
//                                 prefix = strTbl[0];
//                                 if (!oldName.Contains(prefix))
//                                 {
//                                     string final = dir + "/" + prefix + "_" + oldName;
// 
//                                     Sprite dst = AssetLoader.instance.LoadRes(final, typeof(Sprite)).obj as Sprite;
//                                     if (null != dst)
//                                     {
//                                         curCell.SetCellValue(final);
//                                         Resources.UnloadAsset(dst);
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
// 
//             /// 保存
//             bufftbl.Write();
//             /// 转表
//             bufftbl.GetText();
//         }
//         else
//         {
//             Logger.LogWarningFormat("Can not find table {0}!", tableName);
//         }
//     }
// }
// 
