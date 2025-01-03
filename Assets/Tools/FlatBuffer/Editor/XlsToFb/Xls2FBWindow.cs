using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using xls;
using Debug = behaviac.Debug;
using System.Reflection;
using XUPorterJSON;

[System.Serializable]
public class Xls2FBWindow : EditorWindow {
    public static Xls2FBWindow editorWindow;
    private FileSystemWatcher fileWatcher = new FileSystemWatcher ();

    private readonly string PREFIX = "Xls2FBWindow_";

    private static List<string> showResult = new List<string>();
    private Vector2 m_pResulteVec = new Vector2();

    struct SavedSheet
    {
        public string md5;
        public NPOI.SS.UserModel.ISheet sheet;
        public NPOI.HSSF.UserModel.HSSFWorkbook book;


        public SavedSheet(string _md5, NPOI.SS.UserModel.ISheet _sheet, NPOI.HSSF.UserModel.HSSFWorkbook _book)
        {
            md5 = _md5;
            sheet = _sheet;
            book = _book;
        }
    }

    private static Dictionary<string, SavedSheet> savedExcelSheet = new Dictionary<string, SavedSheet>();

    public static  NPOI.SS.UserModel.ISheet GetSheet(string xlsPath, out NPOI.SS.UserModel.IWorkbook outBook)
    {
        NPOI.SS.UserModel.ISheet sheet = null;
        outBook = null;
        string newMd5 = null;
        using (FileStream xls = new FileStream(xlsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            newMd5 = GetMd5Hash(xls);

            bool needLoad = false;

            SavedSheet item;
            if (savedExcelSheet.TryGetValue(xlsPath, out item))
            {
                if (item.sheet == null)
                {
                    needLoad = true;
                }
                else 
                {
                    if (item.md5.Equals(newMd5))
                    {
                        sheet = item.sheet;
                        outBook = item.book;

                     //   UnityEngine.Debug.Log(string.Format("get sheet from saved:{0}", xlsPath));
                    }
                    else
                    {
                        needLoad = true;
                        savedExcelSheet.Remove(xlsPath);

                    //    UnityEngine.Debug.Log(string.Format("has sheet but is old:{0}", xlsPath));
                    }     
                }
            }
            else 
                needLoad = true;

            if (needLoad)
            {
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook (xls);
                sheet = book.GetSheetAt (0);

                savedExcelSheet.Add(xlsPath, new SavedSheet(newMd5, sheet, book));

                outBook = book;

              //  UnityEngine.Debug.Log(string.Format("saved new sheet :{0}", xlsPath));

                book.Close();
            }

        }

        return sheet;
    }

    Xls2FBWindow()
    {

        fileWatcher.Path = "../Share/table/xls/";
        fileWatcher.Filter = "*.xls";
        fileWatcher.Changed += new FileSystemEventHandler (OnProcess);
        fileWatcher.Created += new FileSystemEventHandler (OnProcess);
        fileWatcher.Deleted += new FileSystemEventHandler (OnProcess);
        fileWatcher.Renamed += new RenamedEventHandler (OnProcess);
        fileWatcher.EnableRaisingEvents = true;
    }

    private void OnProcess (object source, FileSystemEventArgs e) {

        if (e.ChangeType == WatcherChangeTypes.Created) {
            // OnCreated(source, e);
        } else if (e.ChangeType == WatcherChangeTypes.Changed) {
            // 文件修改
            for (int i = 0; i < m_pFileList.Count; i++) {
                var info = m_pFileList[i];
                if (e.FullPath == info.xls) {
                    info.modify = true;
                    return;
                }
            }

        } else if (e.ChangeType == WatcherChangeTypes.Deleted) {
            // OnDeleted(source, e);

        }

    }


#if USE_FB_TABLE
    [MenuItem ("[TM工具集]/辅助FB工具/全部生成/cs和数据")]
#endif	
    public static void GenerateCSAndDataMd5 () {
        ConvertXls (true, true, true);
    }

#if USE_FB_TABLE
    [MenuItem ("[TM工具集]/辅助FB工具/全部生成/cs和数据，无视MD")]
#endif
    public static void GenerateCSAndData () {
        ConvertXls (true, true, true);
    }

#if USE_FB_TABLE
    [MenuItem ("[TM工具集]/辅助FB工具/全部生成/生成数据")]
#endif
    public static void GenerateData () {
        ConvertXls (true, false, true);
    }

    /****
     * ignoremd5 无视md5校验
     * 生成cs
     * 生成数据
     ****/
    private static void ConvertXls (bool ignoremd5, bool cs, bool data) {
        List<string> filelist = FindFile ("../Share/table/xls/");
        int i = 0;
        foreach (var filename in filelist) {
            if (filename.EndsWith (".xlsx")) {
                Debug.Log (string.Format ("不支持 xlsx {0}", filename));
                continue;
            }
            if(!Convert (filename, ignoremd5, cs, data)){
                return;
            }
            i++;

            EditorUtility.DisplayProgressBar("FB转表", "Converting .. " +i+"/"+filelist.Count , (i) / (float)filelist.Count);
        }
        Debug.Log (string.Format ("xls 转换完成"));
        EditorUtility.ClearProgressBar();
    }

#if USE_FB_TABLE
    [MenuItem("[TM工具集]/EXCEL转表工具/xls转txt %#t", false, 1)]
#endif
    public static void OpenWindow()
    {
        Xls2FBWindow.editorWindow = EditorWindow.GetWindow<Xls2FBWindow>(false, "FB转表", true);
        Xls2FBWindow.editorWindow.Show();
        Xls2FBWindow.editorWindow.m_pFileList = ChangeXls();
        Xls2FBWindow.editorWindow.LoadConfig();
    }

    public static List<string> FindFile (string sSourcePath) {

        List<string> list = new List<string> ();

        //遍历文件夹

        DirectoryInfo theFolder = new DirectoryInfo (sSourcePath);

        FileInfo[] thefileInfo = theFolder.GetFiles ("*.xls", SearchOption.TopDirectoryOnly);

        foreach (FileInfo NextFile in thefileInfo) //遍历文件
        {
            var name = NextFile.FullName;
            name = name.Replace('\\', '/');

            if (UseSplitTable() && !m_ShowSplitTable)
            {
                if (XlsxDataUnit.NeedIgnore(name))
                {
                    continue;
                }
            }

            list.Add(name);
        }

        return list;
    }

    public class XlsFileInfo {
        public string xls;
        public string md5;
        public bool toggle;
        public bool modify;
        public string tablename;
        public int id;
        public IProtoTableInspector inspector;

    };

    public List<XlsFileInfo> m_pFileList = ChangeXls();

    public bool m_bIsTextOnly = false;

    public bool m_bIsWaitForCompile = true;

    public bool m_onlyGenData = true;

    public bool m_genServerCode = false;

    public static bool m_ShowSplitTable = false;   //是否显示分表表格

    //    public List<bool> m_pToggle = new List<bool>();

    private Vector2 m_pSelectedVec = new Vector2 ();

    private bool mBuildProto = false;
    private StringBuilder mCountBuilder = StringBuilderCache.Acquire (2000);

    private string mFilter = "";

    private bool mShowDirtyFlag = false;

    public Result m_eResulte = Result.Waitting;

    public enum Result {
        Waitting = 0,
        Running,
        Select,
        Finish,
        };

    //    private readonly string PREFIX = "XlsxDataManger_";

    private static List<XlsFileInfo> ChangeXls()
    {
        List<string> files = FindFile("../Share/table/xls/");
        List<XlsFileInfo> result = new List<XlsFileInfo>();
        for (int index = 0; index < files.Count; ++index)
        {
            string filename = files[index];

            if (filename.EndsWith(".xlsx"))
            {
                continue;
            }

            try
            {
                using (FileStream xls = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    string xlsFileMd5 = filename + ".MD5";
                    string oldmd5 = File.Exists(xlsFileMd5) ? System.IO.File.ReadAllText(xlsFileMd5) : "";
                    string newmd5 = GetMd5Hash(xls);

                    XlsFileInfo info = new XlsFileInfo();
                    info.xls = filename;
                    info.md5 = newmd5;
                    info.toggle = false;
                    info.modify = oldmd5 != newmd5;
                    info.tablename = _GetSheetName(Path.GetFileName(filename));
                    result.Add(info);
                    xls.Close();
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("changexls:{0}", e.ToString());
            }


        }

        return result;
    }

    private static string _GetSheetName(string xls)
    {
        if (m_TableMapData == null)
        {
            InitMapData();
        }

        if (null == m_TableMapData)
        {
            return string.Empty;
        }

        foreach (System.Collections.DictionaryEntry item in m_TableMapData)
        {
            if (item.Key.ToString().Equals(xls))
            {
                return item.Value.ToString();
            }
        }

        return string.Empty;
    }

    public static string GetXlsNameBySheetName(string sheetName)
    {
        if (m_TableMapData == null)
        {
            InitMapData();
        }

        if (null == m_TableMapData)
        {
            return string.Empty;
        }

        foreach (System.Collections.DictionaryEntry item in m_TableMapData)
        {
            if (item.Value.ToString() == sheetName)
            {
                return item.Key.ToString();
            }
        }

        return string.Empty;
    }

    public static string GetXlsPathBySheetName(string sheetName)
    {
        string fileName = GetXlsNameBySheetName(sheetName);

        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        return "../Share/table/xls/" + fileName;
    }

    public static void ConvertXlsBySheetName(string sheetName)
    {
        string xlsPath = GetXlsPathBySheetName(sheetName);
        Convert(xlsPath, true, false, true, false);

        if (EditorApplication.isPlaying)
        {
            var list = new List<string>();
            list.Add(sheetName);
            ReLoadTable(list);
        }
            

    }

    public static string[] GetXlsNamesBySheetName(string sheetName)
    {
        string xlsName = GetXlsNameBySheetName(sheetName);
        var array = new string[1] { xlsName };
        for (int i = 0; i < XlsxDataUnit.splitXls.Length; i++)
        {
            if (XlsxDataUnit.splitXls[i].mainName.Equals(xlsName))
            {
                array = new string[XlsxDataUnit.splitXls[i].splitFileNames.Length + 1];
                array[0] = xlsName;
                for (int j = 0; j < XlsxDataUnit.splitXls[i].splitFileNames.Length; j++)
                {
                    array[1 + j] = XlsxDataUnit.splitXls[i].splitFileNames[j];
                }
                break;
            }
        }
        return array;
    }

    public static void OpenXlsByFileName(string fileName)
    {
        string xlsPath = Path.GetFullPath("../Share/table/xls/" + fileName);
        OpenXlsIWithFilePath(xlsPath);
    }

    public static void OpenXlsIWithFilePath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        ProcessStartInfo processInfo = new ProcessStartInfo();
        processInfo.FileName = filePath;
        processInfo.Arguments = "";

        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();
    }

    public static bool UseSplitTable()
    {
        return true;//Global.Settings.useSplitTable;
    }

    public static bool DoConvertAFile(string filename)
    {
        var list = ChangeXls();
        foreach (var item in list)
        {
            if (item.xls.Contains(filename))
            {
                return Convert(item.xls, true, true, true);
            }
        }

        return false;
    }

     public static List<NPOI.SS.UserModel.ISheet> MergeTableArray(string name, NPOI.SS.UserModel.ISheet targetSheet,Stopwatch watch )
    {
     //   var startTime = watch.ElapsedMilliseconds;
        List<NPOI.SS.UserModel.ISheet> sheetList = new List<NPOI.SS.UserModel.ISheet>();
        sheetList.Add(targetSheet);
       var tokens = name.Split('/');
        var fileName = tokens[tokens.Length - 1];


        var splitXls = XlsxDataUnit.splitXls;


        for (int i = 0; i < splitXls.Length; ++i)
            if (splitXls[i].mainName.Contains(fileName))
            {
                for (int j = 0; j < splitXls[i].splitFileNames.Length; ++j)
                {
                 //   var fileStart = watch.ElapsedMilliseconds;

                    var itemSplit = name.Replace(splitXls[i].mainName, splitXls[i].splitFileNames[j]);

                    NPOI.SS.UserModel.ISheet ipas = null;


                    if (!File.Exists(itemSplit))
                        continue;

                    
                    NPOI.SS.UserModel.IWorkbook iworkbook = null;
                    ipas = GetSheet(itemSplit, out iworkbook);

                    
                    // using (FileStream xls = new FileStream(itemSplit, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    // {
                        
                    //     Logger.LogErrorFormat("step合表 打开文件 读取:{2} time:{0} 总时间:{1}", watch.ElapsedMilliseconds-fileStart, watch.ElapsedMilliseconds, itemSplit);
                    //     if (xls != null)
                    //     {
                    //         NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook(xls);
                    //         ipas = book.GetSheetAt(0);
                    //         book.Close();

                    //         Logger.LogErrorFormat("step合表 GetSheetAt 读取:{2} time:{0} 总时间:{1}", watch.ElapsedMilliseconds-fileStart, watch.ElapsedMilliseconds, itemSplit);
                    //     }
                    //     xls.Close();
                    // }

                    if (ipas != null)
                    {
                          sheetList.Add(ipas);
                         //  Logger.LogErrorFormat("step合表 after add 读取:{2} time:{0} 总时间:{1}", watch.ElapsedMilliseconds-fileStart, watch.ElapsedMilliseconds, itemSplit);
                       // MergeShell(ipas as NPOI.HSSF.UserModel.HSSFSheet, targetSheet as NPOI.HSSF.UserModel.HSSFSheet, originBook, j+1);
                    }
                   // Logger.LogErrorFormat("step合表 读取:{2} time:{0} 总时间:{1}", watch.ElapsedMilliseconds-fileStart, watch.ElapsedMilliseconds, itemSplit);
                 //   startTime = watch.ElapsedMilliseconds;
                }

                break;
            }

        return sheetList;
    }

    public static NPOI.SS.UserModel.ISheet MergeTables(string name, NPOI.SS.UserModel.ISheet targetSheet)
    {
        var tokens = name.Split('/');
        var fileName = tokens[tokens.Length - 1];


        var splitXls = XlsxDataUnit.splitXls;


        for (int i = 0; i < splitXls.Length; ++i)
            if (splitXls[i].mainName.Contains(fileName))
            {
                for (int j = 0; j < splitXls[i].splitFileNames.Length; ++j)
                {
                    var itemSplit = name.Replace(splitXls[i].mainName, splitXls[i].splitFileNames[j]);

                    NPOI.SS.UserModel.ISheet ipas = null;


                    if (!File.Exists(itemSplit))
                        continue;

                    using (FileStream xls = new FileStream(itemSplit, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (xls != null)
                        {
                            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook(xls);
                            ipas = book.GetSheetAt(0);
                        }
                        xls.Close();
                    }

                    if (ipas != null)
                    {
                        targetSheet = MergeShell(targetSheet as NPOI.HSSF.UserModel.HSSFSheet, ipas as NPOI.HSSF.UserModel.HSSFSheet);
                    }
                }

                break;
            }


        return targetSheet;
    }

    public static bool Convert(string filename, bool ignore, bool cs, bool data, bool servercode = false)
    {
        //var startTime = Time.time * 1000;
        //var originTime = startTime;

        Stopwatch watch = new Stopwatch();
        watch.Start();
        var startTime = watch.ElapsedMilliseconds;

        //using (FileStream xls = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            
          //  string xlsFileMd5 = filename + ".MD5";
          //  string oldmd5 = ignore? "": (File.Exists (xlsFileMd5) ? File.ReadAllText (xlsFileMd5) : "");
          //  string newmd5 = GetMd5Hash (xls);

            // md5码一样  就不转
            // if (oldmd5 == newmd5) {
            //     Debug.Log (string.Format ("xls {0} 未发生改变", filename));
            //     return true;
            // }
/*
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook (xls);
            if (book == null) {
                EditorUtility.DisplayDialog ("【表不存在!】", filename, "确定", "");
                return false;
            }

            NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt (0);
            if (sheet == null) {
                return false;
            }
*/
            ////////////////////////////////////////////////////////////

            NPOI.SS.UserModel.IWorkbook iworkbook = null;
            var sheet = GetSheet(filename, out iworkbook);

            //新版转表代码(可以大幅提高转表速度)
           
            List<NPOI.SS.UserModel.ISheet> sheetList = null;
            if (UseSplitTable())
            {
                sheetList = MergeTableArray(filename, sheet, watch );
                //Logger.LogErrorFormat("MergeTableArray  {0}", sheetList.Count);
            }
            else
            {
                sheetList = new List<NPOI.SS.UserModel.ISheet>();
                sheetList.Add(sheet);
            }
                

        //    Logger.LogErrorFormat("step合表 经过time:{0} 总时间:{1}", watch.ElapsedMilliseconds-startTime, watch.ElapsedMilliseconds);
            startTime = watch.ElapsedMilliseconds;

            Table table = new Table();
            table.ParserFrom(sheetList, filename);
            
            //UnityEngine.Debug.Log(string.Format("merge time Elapsed {0}", watch.ElapsedMilliseconds));

            //旧版转表代码(作为备份)
            //if (UseSplitTable())
            //{
            //    sheet = MergeTables(filename, sheet);
            //}
            //Table table = new Table();
            //table.ParserFrom(sheet, filename);

            ////////////////////////////////////////////////////////////

            if (cs && !fb.GenerateDesc(table))
            {
                EditorUtility.DisplayDialog("【生成cs错误!】", filename, "确定", "");
                return false;
            }

            if (!cs)
            {
                if (!fb.CheckCs(table))
                {
                    EditorUtility.DisplayDialog("[表格字段可能和cs不匹配了]", filename, "确定", "");
                    return false;
                }
            }

            if (servercode)
            {
                Cpp cpp = new Cpp();
                cpp.GenerateCode(table);
            }

            if (data && !fb.DumpData (table)) {
                EditorUtility.DisplayDialog ("【生成数据错误!】", filename, "确定", "");
                return false;
            }

            //Logger.LogError(string.Format("xls {0}  转换完成", filename));

            showResult.Add(string.Format("xls {0} 转换完成\n", filename));

           // book.Close();
           // xls.Close();

        //    Logger.LogErrorFormat("step转表完成 经过time:{0} 总时间:{1}", watch.ElapsedMilliseconds-startTime, watch.ElapsedMilliseconds);
            startTime = watch.ElapsedMilliseconds;

//             if (File.Exists (xlsFileMd5)) {
//                 File.Delete (xlsFileMd5);
//             }
//             File.WriteAllText (xlsFileMd5, newmd5);

        }

        XlsxDataUnit unit = new XlsxDataUnit(filename);

      //  Logger.LogErrorFormat("step合表2 加载主表 time:{0} 总时间:{1}", watch.ElapsedMilliseconds-startTime, watch.ElapsedMilliseconds);
        startTime = watch.ElapsedMilliseconds;

        if (unit.NeedConverServerText())
        {
            if (UseSplitTable())
            {
                //XlsxDataUnit.MergeTables(filename, unit);
                XlsxDataUnit.MergeTablesNew(filename, unit, watch);
            }

         //   Logger.LogErrorFormat("step合表2 经过time:{0} 总时间:{1}", watch.ElapsedMilliseconds-startTime, watch.ElapsedMilliseconds);
                startTime = watch.ElapsedMilliseconds;

            if (null != unit)
            {
                unit.GetServerText();
            }
         //   Logger.LogErrorFormat("step服务器转表 经过time:{0} 总时间:{1}", watch.ElapsedMilliseconds-startTime, watch.ElapsedMilliseconds);
                startTime = watch.ElapsedMilliseconds;
        }

        UnityEngine.Debug.LogFormat("总时间time:{0}", watch.ElapsedMilliseconds);
        watch.Stop();

        return true;
    }


    public static NPOI.SS.UserModel.ISheet MergeShell(NPOI.HSSF.UserModel.HSSFSheet origin, NPOI.HSSF.UserModel.HSSFSheet merge)
    {
        NPOI.HSSF.UserModel.HSSFWorkbook product = new NPOI.HSSF.UserModel.HSSFWorkbook();

        if (null == origin || null == merge)
        {
            return null;
        }

        origin.CopyTo(product, origin.SheetName, true, true);
        merge.CopyTo(product, "1", true, true);

        var froms = product.GetSheetAt(1);
        var tos = product.GetSheetAt(0);

        int lastRow = tos.LastRowNum;

        for (int i = 5; i <= froms.LastRowNum; ++i)
        {
            NPOI.SS.Util.SheetUtil.CopyRow(froms, i, tos, lastRow + i - 5+1);
        }

        return tos;
    }

    public static string GetMd5Hash(FileStream fs)
    {
        MD5 md5Hash = MD5.Create();
        byte[] data = null;

        data = md5Hash.ComputeHash (fs);

        StringBuilder sBuilder = new StringBuilder ();

        for (int i = 0; i < data.Length; i++) {
            sBuilder.Append (data[i].ToString ("x2"));
        }

        return sBuilder.ToString ().ToLower ();
    }

    private void SaveConfig()
    {
        EditorPrefs.SetInt(PREFIX + "Count", m_pFileList.Count);

        EditorPrefs.SetBool(PREFIX + "IsTextOnly", m_onlyGenData);

        EditorPrefs.SetBool(PREFIX + "IsWaitForCompile", m_bIsWaitForCompile);

        for (int i = 0; i < m_pFileList.Count; i++)
        {
            string filepath = m_pFileList[i].xls;
            //EditorPrefs.SetString(PREFIX + string.Format("PATH_{0}", i), filepath);

            bool bToggle = m_pFileList[i].toggle;
            EditorPrefs.SetBool(PREFIX + string.Format("TOGGLE_{0}", i), bToggle);

            string sMD5 = m_pFileList[i].md5;
            EditorPrefs.SetString(PREFIX + string.Format("MD5_{0}", i), sMD5);

            //string sProtoMD5 = m_pProtoMD5[i];
            //EditorPrefs.SetString(PREFIX + string.Format("PROTO_MD5_{0}", i), sProtoMD5);                   
        }
    }

    private void LoadConfig()
    {
        if (m_pFileList == null)
            m_pFileList = ChangeXls();

        int iCount = EditorPrefs.GetInt(PREFIX + "Count");
        //m_pFileList = new string[iCount];

        m_bIsTextOnly = EditorPrefs.GetBool(PREFIX + "IsTextOnly");

        m_bIsWaitForCompile = EditorPrefs.GetBool(PREFIX + "IsWaitForCompile");

        for (int i = 0; i < iCount; i++)
        {
            //m_pFileList[i].xls = EditorPrefs.GetString(PREFIX + string.Format("PATH_{0}", i));
            m_pFileList[i].toggle = EditorPrefs.GetBool(PREFIX + string.Format("TOGGLE_{0}", i));
            m_pFileList[i].md5 = EditorPrefs.GetString(PREFIX + string.Format("MD5_{0}", i));
            //m_pProtoMD5[i] = EditorPrefs.GetString(PREFIX + string.Format("PROTO_MD5_{0}", i));
        }
        InitMapData();
    }

    private bool isShowDetail = false;

    public void OnGUI()
    {
        if (m_TableMapData == null || m_TableMapData.Count <= 0) 
            InitMapData();

        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox(string.Format("正在编译中\n"), MessageType.Warning);
            return;
        }

        // if (EditorApplication.isPlaying) {
        //     EditorGUILayout.HelpBox (string.Format ("游戏正在运行\n"), MessageType.Warning);
        //     return;
        // }

        if (m_bIsWaitForCompile && mBuildProto && EditorApplication.isCompiling) {
            mCountBuilder.Append ("..");
            EditorGUILayout.HelpBox (string.Format ("正在编译中{0}\n如果本页面木有刷新，随便点击点击点击", mCountBuilder.ToString ()),
                MessageType.Warning);
            return;
        }

        EditorGUI.BeginChangeCheck();
        {
            EditorGUILayout.BeginVertical();
            {
                bool genData = EditorGUILayout.Toggle("只转数据表", m_onlyGenData);
                if (genData != m_onlyGenData)
                    m_onlyGenData = genData;

                bool genServerCode = EditorGUILayout.Toggle("生成服务器代码", m_genServerCode);
                if (genServerCode != m_genServerCode)
                    m_genServerCode = genServerCode;

                bool vvalue = EditorGUILayout.Toggle("等待编译", m_bIsWaitForCompile);
                if (vvalue != m_bIsWaitForCompile)
                {
                    m_bIsWaitForCompile = vvalue;
                }

                bool value = EditorGUILayout.Toggle("显示分表", m_ShowSplitTable);
                if(m_ShowSplitTable!= value)
                {
                    m_ShowSplitTable = value;
                    m_pFileList = ChangeXls();
                    m_eResulte = Result.Select;
                }

                if (mFilter == null)
                    mFilter = "";
                var str = EditorGUILayout.TextField("筛选", mFilter);
                if (str != mFilter)
                {
                    mFilter = str;
                }

                isShowDetail = EditorGUILayout.Toggle("详情(-.-)", isShowDetail);
            }
            EditorGUILayout.EndVertical ();

            EditorGUILayout.Space (); 

            m_pSelectedVec = EditorGUILayout.BeginScrollView(m_pSelectedVec, GUILayout.Height(500));
            {
                EditorGUILayout.BeginVertical("ObjectFieldThumb");
                {
                    if (m_pFileList == null || m_pFileList.Count <= 0)
                        m_pFileList = ChangeXls();
                    for (int i = 0; i < m_pFileList.Count; i++)
                    {
                        XlsFileInfo info = m_pFileList[i];
                        string filename = System.IO.Path.GetFileName(info.xls);

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUI.indentLevel++;

                            if (i % 2 == 0) {
                                GUI.color = Color.yellow;
                            }
                            //                             if (info.modify) {
                            //                                 GUI.color = Color.red;
                            //                             }
                            
                            if ((mFilter.Length <= 0 || filename.ToLower().StartsWith(mFilter.ToLower()) || filename.Contains(mFilter) || JudgeXlsCs(filename) || info.toggle
                                || info.tablename.ToLower().StartsWith(mFilter.ToLower())))
                            {
                                if (XlsxDataUnit.NeedIgnore(filename))
                                {
                                    EditorGUILayout.LabelField(string.Format("[分表]{0}", filename), GUILayout.Width(200));
                                    EditorGUILayout.LabelField("", GUILayout.Width(50));
                                    if (m_TableMapData != null && m_TableMapData.Count > 0)
                                    {
                                        EditorGUILayout.LabelField("", GUILayout.Width(150));
                                    }
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(filename, GUILayout.Width(200));

                                    bool value = EditorGUILayout.Toggle("", info.toggle, GUILayout.Width(50));
                                    if (value != info.toggle)
                                    {
                                        info.toggle = value;
                                    }

                                    if (m_TableMapData != null && m_TableMapData.Count > 0 && isShowDetail)
                                    {
                                        if (m_TableMapData.ContainsKey(filename))
                                        {
                                            EditorGUILayout.LabelField(m_TableMapData[filename].ToString(), GUILayout.Width(250));
                                        }
                                        else
                                        {
                                            EditorGUILayout.LabelField("", GUILayout.Width(150));
                                        }
                                    }
                                }

                                //#if UNITY_STANDALONE_WIN
                                if (StyledButton("开!"))
                                {

                                    ProcessStartInfo processInfo = new ProcessStartInfo();
                                    processInfo.FileName = info.xls;
                                    processInfo.Arguments = "";

                                    Process process = new Process();
                                    process.StartInfo = processInfo;
                                    process.Start();

                                }
                            }

                            //#endif
                            GUI.color = Color.white;

                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndHorizontal ();

                        if (isShowDetail && info.toggle)
                        {
                            if (info.inspector == null && !string.IsNullOrEmpty(info.tablename))
                            {
                                System.Type type = typeof(ProtoTable.UnitTable).Assembly.GetType("ProtoTable." + info.tablename);

                                info.inspector = new TableInspetor(type);
                            }

                            if (null != info.inspector)
                            {
                                info.id = info.inspector.OnGUIWithID(info.id);
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical ();
            }
            EditorGUILayout.EndScrollView ();

            EditorGUILayout.BeginHorizontal ("ObjectFieldThumb"); {
                if (StyledButton ("刷新")) {
                    m_pFileList = ChangeXls ();
                    m_eResulte = Result.Select;

                    SaveConfig();
                }

                if (StyledButton("生成映射表"))
                {
                    GerenateMapData();
                }

                if (StyledButton ("全选")) {
                    for (int i = 0; i < m_pFileList.Count; i++) {
                        var info = m_pFileList[i];
                        info.toggle = true;
                    }

                    m_eResulte = Result.Waitting;

                    SaveConfig();
                }

                if (StyledButton ("反选")) {
                    for (int i = 0; i < m_pFileList.Count; i++) {
                        var info = m_pFileList[i];
                        info.toggle = !info.toggle;
                    }

                    m_eResulte = Result.Waitting;
                    SaveConfig();
                }

                if (StyledButton ("清空")) {
                    for (int i = 0; i < m_pFileList.Count; i++) {
                        var info = m_pFileList[i];
                        info.toggle = false;
                    }

                    m_eResulte = Result.Waitting;
                    SaveConfig();
                }

                if (StyledButton ("转表")) {
                    mBuildProto = true;
                    showResult.Clear();
                    List<string> convertedTables = new List<string>();

                    for (int i = 0; i < m_pFileList.Count; i++) {
                        var info = m_pFileList[i];
                        if (info.toggle && Convert(info.xls, true, !m_onlyGenData, true, m_genServerCode))
                        {
                            info.modify = false;
                            convertedTables.Add(info.tablename);
                        } 
                    }


                    if (EditorApplication.isPlaying)
                        ReLoadTable(convertedTables);
                    else
                    {
                        TableManager.DestroyInstance();
                        //ResourceManager.instance.ClearCache();
                        //ResourceManager.DestroyInstance();
                        AssetLoader.instance.ClearAll(true);
                        AssetLoader.DestroyInstance();
                    }

                    SaveConfig();

                    m_eResulte = Result.Finish;

                    
                }


            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("ObjectFieldThumb");
            {
                m_pResulteVec = EditorGUILayout.BeginScrollView(m_pResulteVec, GUILayout.Height(180));
                {
                    switch (m_eResulte)
                    {
                        case Result.Waitting:
                            EditorGUILayout.LabelField("");
                            break;
                        case Result.Select:
                            EditorGUILayout.LabelField("修改过的表格:");
                            EditorGUILayout.LabelField(showResult.ToString());
                            break;
                        case Result.Finish:
                            EditorGUILayout.LabelField(string.Format("转表完成 {0}：", System.DateTime.Now.ToLongTimeString().ToString()));

                            for(int i=0; i<showResult.Count; ++i)
                                EditorGUILayout.LabelField(showResult[i]);

                            break;
                        default:
                            break;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();

        }

        // if (EditorGUI.EndChangeCheck ()) {

        // }
    }

    public static void ReLoadTable(List<string> convertedTables)
    {
        if (TableManager.instance != null)
        {
            //TableManager.instance.UnInit();
            //TableManager.instance.Init();
            
            //只重新加载指定的表格
            foreach(var tableName in convertedTables)
            {
                var type = TypeTable.GetType("ProtoTable."+tableName);
                TableManager.instance.ReloadTable(type);
                TableManager.instance.ReloadRelatedLogic(type);

                //Logger.LogErrorFormat("重新加载表格:{0}", tableName);    
            }
            



            Logger.LogErrorFormat("表格重新加载完成!!!");
        }
    }

    public static bool StyledButton (string label) {
        EditorGUILayout.Space ();
        GUILayoutUtility.GetRect (1, 20);
        EditorGUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        bool clickResult = GUILayout.Button (label, "miniButton");
        GUILayout.FlexibleSpace ();
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.Space ();
        return clickResult;
    }

    //[MenuItem("转表工具/数据检查")]
    public static void CheckData()
    {
        //Type[] data = TableManager.instance.GetAllTypeListInEditorMode();
        //for (int i = 0; i < data.Length; i++)
        //{
        //    var curType = data[i];
        //    string filepath = TableManager._getTablePathNew(curType);
        //    Dictionary<int, object> table = ParseTable(curType, filepath);
        //    try
        //    {
        //        foreach (KeyValuePair<int, object> keyValue in table)
        //        {
        //            int id = keyValue.Key;
        //            object obj = keyValue.Value;
        //            PropertyInfo[] propertyInfos = curType.GetProperties();
        //            for (int pi = 0; pi < propertyInfos.Length; ++pi)
        //            {
        //                var method = propertyInfos[pi].GetGetMethod();
        //                object value = method.Invoke(obj, null);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        EditorUtility.ClearProgressBar();
        //        Logger.LogErrorFormat("Type[{0}] {1}", curType.Name, e.ToString());
        //        break;
        //    }
        //    EditorUtility.DisplayProgressBar("共:" + data.Length + "个", "Checking ..当前第 " + i + "个", (i + 1) / (float)data.Length);
        //}
        //EditorUtility.ClearProgressBar();
    }

    private static Dictionary<int, object> ParseTable(Type type, string filename)
    {

        try
        {
            byte[] data = File.ReadAllBytes("Assets/Resources/" + filename);
            Dictionary<int, object> table = new Dictionary<int, object>();
            FlatBuffers.Table ftable = new FlatBuffers.Table();
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(data);

            ftable.bb_pos = 0;
            ftable.bb = buffer;

            int length = ftable.__vector_len(0);

            for (int index = 0; index < length; ++index)
            {
                ;
                int offset = ftable.__vector(index);
                var fobj = Activator.CreateInstance(type);

                MethodInfo __assign = type.GetMethod("__assign");
                var IDMap = type.GetProperty("ID").GetGetMethod();

                BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
                //GetValue方法的参数
                object[] parameters = new object[] { ftable.__indirect(ftable.__vector(0) + index * 4), ftable.bb };
                __assign.Invoke(fobj, flag, Type.DefaultBinder, parameters, null);

                int id = (int)IDMap.Invoke(fobj, null);

                if (!table.ContainsKey(id))
                {
                    table.Add(id, fobj);
                }
                else
                {
                    Logger.LogErrorFormat("id {0} 相同,严重错误 表:{1}", id, type);
                }
            }
            return table;
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("{0}", e.ToString());
            return null;
        }

    }
    
    private static string m_MapPath = "../Share/table/xls/map.json";
    private static Hashtable m_TableMapData = null;

    /// <summary>
    /// 初始化映射表表数据
    /// </summary>
    public static void InitMapData()
    {
        var path = m_MapPath;
        if (!File.Exists(path))
            return;

        StreamReader sr = new StreamReader(path);
        string str = sr.ReadToEnd();
        if (str.Length > 0)
        {
            m_TableMapData = LitJson.JsonMapper.ToObject<Hashtable>(str);
        }
        sr.Close();
        sr = null;
    }

    /// <summary>
    /// 生成映射表数据
    /// </summary>
    protected void GerenateMapData()
    {
        m_pFileList = ChangeXls();
        if (m_pFileList == null || m_pFileList.Count <= 0)
        {
            return;
        }
        m_TableMapData = new Hashtable();//.Clear();
        for (int i = 0; i < m_pFileList.Count; i++)
        {
            var xlsInfo = m_pFileList[i];

            string fileName = Path.GetFileName(xlsInfo.xls);
            if (m_TableMapData != null && !m_TableMapData.ContainsKey(fileName))
            {
                using (FileStream xls = new FileStream(xlsInfo.xls, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook(xls);
                    if (book == null)
                    {
                        Logger.LogErrorFormat("表不存在:{0}", xlsInfo.xls); 
                        break;
                    }

                    NPOI.SS.UserModel.ISheet sheet = book.GetSheetAt(0);
                    if (sheet == null)
                    {
                        Logger.LogErrorFormat("表不存在:{0}", xlsInfo.xls);
                        break;
                    }
                    m_TableMapData.Add(fileName, sheet.SheetName);
                }
            }
        }
        string path = m_MapPath;
        string data = LitJson.JsonMapper.ToJson(m_TableMapData);
        SaveData(data);
        m_pFileList = ChangeXls();
    }


    /// <summary>
    /// 判断表格是否符合筛选要求
    /// </summary>
    protected bool JudgeXlsCs(string fileName)
    {
        if (m_TableMapData == null)
        {
            InitMapData();
        }

        if (m_TableMapData == null)
        {
            return false;
        }
        if (!m_TableMapData.ContainsKey(fileName))
        {
            return false;
        }
        string csTableName = m_TableMapData[fileName].ToString();
        if (string.IsNullOrEmpty(csTableName)) return false;
        if (!csTableName.ToLower().StartsWith(mFilter.ToLower()))
            return false;
        return true;
    }


    private void SaveData(string data)
    {
        var path = m_MapPath;

        StreamWriter sw = null;
        try
        {
            sw = new StreamWriter(path);
            sw.Write(data);
            sw.Flush();
            sw.Close();
        }
        catch (Exception e)
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            Logger.LogErrorFormat("Flush file failed!!!!!!:{0} reason {1} \n", path, e.ToString());
        }
    }
}
