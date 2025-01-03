using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CleanAssetsTool
{
    public static class XlsxCellUtility
    {
        public static int CustomToInt(this ICell cell)
        {
            return Convert.ToInt32(cell.ToString());
        }

        public static int CustomIDToInt(this ICell cell, string type = "sint32")
        {
            string cellStr = cell.ToString();
            if (cellStr.Length <= 0)
            {
                UnityEngine.Debug.LogErrorFormat("CustomIDToInt : the string len is 0");
                return 0;
            }

            if ("sint32" == type || "int32" == type)
            {
                int value = -1;

                if (int.TryParse(cellStr, out value))
                {
                    return value;
                }
                else
                {
                    float fv = 0.0f;
                    if (float.TryParse(cellStr, out fv))
                    {
                        return (int)fv;
                    }
                    UnityEngine.Debug.LogErrorFormat("convert to number error : {0}", cellStr);
                }

                return value;
            }
            else if ("string" == type)
            {
                return cellStr.GetHashCode();
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Error Type With : {0}, {1}", type, cellStr);
                return -1;
            }
        }

        public static string CustomToString(this ICell cell)
        {
            return cell.ToString();
        }

        public static T CustomToEnum<T>(this ICell cell) where T : class
        {
            return cell.CustomToInt() as T;
        }


        public static bool CustomToBool(this ICell cell)
        {
            return cell.CustomToInt() != 0;
        }
    }

    public class XlsUtil
    {
        public static string XLS_PATH = "../Share/table/xls";

        public static string[] GetXLSFileList()
        {
            List<string> finalList = new List<string>();
            string[] strList = Directory.GetFiles(XLS_PATH, "*.xls", SearchOption.AllDirectories);

            for (int i = 0; i < strList.Length; i++)
            {
                strList[i] = strList[i].Replace('\\', '/');
                if (strList[i].EndsWith("xls"))
                {
                    finalList.Add(strList[i]);
                }
            }

            strList = finalList.ToArray();

            return strList;
        }
    }

    public class XlsxDataUnit : ICloneable
    {
        class ProtoXlsxEnumItem
        {
            private string mKey = "";
            private int mValue = 0;
            private string mComment = "";
            private string mType = "";

            private bool mValid = false;

            public string key
            {
                get
                {
                    return mKey;
                }
            }

            public int value
            {
                get
                {
                    return mValue;
                }
            }

            public string comment
            {
                get
                {
                    return mComment;
                }
            }

            public bool valid
            {
                get
                {
                    return mValid;
                }
            }

            public string type
            {
                get
                {
                    return mType;
                }
            }


            public ProtoXlsxEnumItem(string item)
            {
                string[] enumKVList = item.Split(SPLITE_KEY_VALUE);
                if (enumKVList.Length > 2)
                {
                    mValid = true;
                    try
                    {
                        mKey = enumKVList[0];
                        mValue = int.Parse(enumKVList[1]);
                        mComment = enumKVList[2];
                    }
                    catch (Exception e)
                    {
                        mValid = false;
                        //XlsxDebug.LogErrorFormat("[ProtoXlsxEnumItem] Prase Error : {0}, {1}", item, e.ToString());
                    }
                }
                else
                {
                    //XlsxDebug.LogWarningFormat("[ProtoXlsxEnumItem] Format Error : {0}", item);
                }
            }

            public bool IsValidEnumItem(string str)
            {
                int intvalue = int.MaxValue;

                try
                {
                    intvalue = int.Parse(str);
                }
                catch
                {

                }

                return intvalue == mValue || str == mComment || str == mKey;
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}", mKey, mValue, mComment);
            }
        };

        public const string STRING_CELL_DEFAULTE = "-";

        public const char SPLITE_LINE = '\n';
        public const char SPLITE_KEY_VALUE = ':';
        public const char SPLITE_REPEATED = '|';

        private const int PROTO_MOD_ROW_INDEX = 0;
        private const int PROTO_TYPE_ROW_INDEX = 1;
        private const int PROTO_KEY_ROW_INDEX = 2;
        private const int PROTO_SERVER_FLAG = 3;

        public const int XLS_HEAD_INDEX = 4;
        public const int XLS_DATA_INDEX = 5;

        private IRow mProtoModRow;
        private IRow mProtoTypeRow;
        private IRow mProtoKeyRow;
        private IRow mHeadRow;

        // KV.key -> ProtoTypeString, KV.Value -> None
        private Dictionary<int, KeyValuePair<string, string>> mDictProtoModKV;

        // KV.key -> ProtoType, KV.Value -> proto dependence
        // KV.key === 'enum' -> check the HeadRow
        private Dictionary<int, KeyValuePair<string, string>> mDictProtoTypeKV; // key是列号，后面的string是类型，比如sint32, string.

        // KV.key -> ProtoKeyString, KV.Value -> default value or value range
        private Dictionary<int, KeyValuePair<string, string>> mDictProtoKeyKV;

        // map.key -> ID
        // KV.key -> ItemName
        private Dictionary<int, KeyValuePair<string, List<ProtoXlsxEnumItem>>> mDictHeadKV;

        // 下面三个数据是针对每行数据的，有多少行数据，Dic中就有多少个元素
        // key: ID(sint32的值或者string的hashcode)，Value：每个key和列Cell的映射
        public Dictionary<int, Dictionary<string, ICell>> mDictData;
        // key: Line number(没去掉头几行的偏移)，Value：每个key和列Cell的映射
        public Dictionary<int, Dictionary<string, ICell>> mDictDataByLine;
        // key: ID, Line Number(去掉了头几行的偏移)
        public Dictionary<int, int> mDictID2Line;

        private Dictionary<int, string> mDicIsServer;

        // 是string类型的key的集合，用于提取资源路径
        private HashSet<string> mStringTypeKey;

        // ID type string or sint32. 第一列都是ID，mCurIDType表示ID的类型，可能是sint32，或者string。
        private string mCurIDType = "";

        private string mCurPath = "";   // 该表的路径名。
        private string mXlsName = "";   // 该表的路径名。

        private bool mInitFail = false;

        private IWorkbook mWorkbook;
        private ISheet mCurSheet;

        private int mRowCount = 0;    // 行的数量
        private int mCellCount = 0;   // 列的数量

        private string mProtoName = "";
        private string mErrorMsg = "";

        public string XlsName
        {
            get { return mXlsName; }
        }

        public string ProtoName
        {
            get { return mProtoName; }
        }

        /// <summary>
        /// 获取该表中所有表示资源的路径
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAssetNames()
        {
            List<string> assetNames = new List<string>();

            // 每列
            foreach (var itrKey in mStringTypeKey)
            {
                // 每行
                bool columnHasAsset = false;
                foreach (var itrline in mDictDataByLine)
                {
                    string[] cellValue = GetCellValue(itrline.Value[itrKey], itrline.Key, itrKey);
                    if(cellValue != null)
                    {
                        for(int i = 0; i < cellValue.Length; ++i)
                        {
                            if(AssetUtility.IsAssetPath(ref cellValue[i]))
                            {
                                assetNames.Add(cellValue[i]);

                                columnHasAsset = true;
                            }
                            else if(columnHasAsset && (cellValue[i].Contains("/") || cellValue[i].Contains("\\")))
                            {
                                WarningWindow.PushWarning("疑似缺失资源路径：{0}, (表 {1}, 第 {2} 行， key: {3})", cellValue[i], XlsName, itrline.Key, itrKey);
                            }
                        }
                    }
                }
            }

            return assetNames;
        }

        public string[] GetCellValue(ICell cell, int line, string key)
        {
            if (cell == null || cell.CellType != CellType.String)
                return null;

            /*
                        if((cell.CellType != CellType.String))
                        {
                            WarningWindow.PushWarning("表 {0}, 第 {1} 行， key: {2} 不是字符串类型", XlsName, line, key);
                        }
            */

            if (string.IsNullOrEmpty(cell.StringCellValue) || cell.StringCellValue == STRING_CELL_DEFAULTE)
                return null;

            return cell.StringCellValue.Split(SPLITE_REPEATED);
        }

        public XlsxDataUnit(string filepath)
        {
            mCurPath = filepath;
            mXlsName = Path.GetFileName(mCurPath);

            try
            {
                using (Stream fs = File.Open(mCurPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    _init(fs);
                }
            }
            catch (Exception e)
            {
                //XlsxDebug.LogErrorFormat("[XlsxDataUnit] Open File({0}) Error {1}", m_pCurPath, e.ToString());
                mInitFail = true;
            }
        }

        private void _init(Stream fs)
        {
            mWorkbook = new HSSFWorkbook(fs);
            mCurSheet = mWorkbook.GetSheetAt(0);

            mDictData = new Dictionary<int, Dictionary<string, ICell>>();
            mDictDataByLine = new Dictionary<int, Dictionary<string, ICell>>();
            mDictID2Line = new Dictionary<int, int>();
            mStringTypeKey = new HashSet<string>();

            //mDicToggleDic = new Dictionary<string, bool>();
            //mDicToggleCount = new Dictionary<string, int>();
            //mDicMask = new Dictionary<string, int>();

            mDicIsServer = new Dictionary<int, string>();

            //mDicSelectDic = new Dictionary<string, int>();

            mProtoName = mCurSheet.SheetName;

            mErrorMsg = "";

            _generateProtoRow();
            _generateHeadRow();
            _generateDataRow();
            _generateServerRow();

            if (!_isValidXls())
            {
                throw new Exception("XlsxDataUnit Not a Valid Xls");
            }
            mWorkbook.Close();
            fs.Close();
        }

        // 分析第1，2，3行的数据。其中第二行是类型。
        private void _generateProtoRow()
        {
            mProtoModRow = mCurSheet.GetRow(PROTO_MOD_ROW_INDEX);
            mDictProtoModKV = _generateKeyValueDic(mProtoModRow);

            mProtoTypeRow = mCurSheet.GetRow(PROTO_TYPE_ROW_INDEX);
            mDictProtoTypeKV = _generateKeyValueDic(mProtoTypeRow);

            mCurIDType = mDictProtoTypeKV[0].Key.ToString();

            mProtoKeyRow = mCurSheet.GetRow(PROTO_KEY_ROW_INDEX);
            mDictProtoKeyKV = _generateKeyValueDic(mProtoKeyRow);

            mCellCount = mDictProtoModKV.Count;

            for (int j = 0; j < mCellCount; j++)
            {
                string typeName = mDictProtoTypeKV[j].Key;
                string keyName = mDictProtoKeyKV[j].Key;

                if(typeName == "string")
                {
                    mStringTypeKey.Add(keyName);
                }
            }
        }

        // 分析第5行head的数据
        private void _generateHeadRow()
        {
            mHeadRow = mCurSheet.GetRow(XLS_HEAD_INDEX);
            mDictHeadKV = _generateKeyValueList(mHeadRow);
        }

        private Dictionary<int, KeyValuePair<string, string>> _generateKeyValueDic(IRow row)
        {
            var kvDic = new Dictionary<int, KeyValuePair<string, string>>();
            for (int i = 0; i < row.Cells.Count; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell == null || cell.ToString() == "")
                {
                    string errorMsg = string.Format("[GenerateKeyValueDic] : 表 {0}, 第 {1} 列, 数据为空, 行首部数据 {2}", XlsName, i, row.GetCell(0).ToString());
                    WarningWindow.PushWarning(errorMsg);
                    mErrorMsg += string.Format("第 {0} 列, 数据为空\n", i);
                    break;
                }

                string[] kvList = cell.ToString().Split(SPLITE_KEY_VALUE);
                string key = kvList[0];
                string value = kvList.Length > 1 ? kvList[1] : "";
                kvDic.Add(i, new KeyValuePair<string, string>(key.Trim(), value.Trim()));
            }

            return kvDic;
        }
        private Dictionary<int, KeyValuePair<string, List<ProtoXlsxEnumItem>>> _generateKeyValueList(IRow row)
        {
            var listDic = new Dictionary<int, KeyValuePair<string, List<ProtoXlsxEnumItem>>>();

            for (int i = 0; i < row.Cells.Count; i++)
            {
                ICell cell = row.GetCell(i);
                string[] listList = cell.ToString().Split(SPLITE_LINE);
                string key = listList[0].Trim();
                List<ProtoXlsxEnumItem> protoXlsxEnumList = new List<ProtoXlsxEnumItem>();

                for (int j = 1; j < listList.Length; j++)
                {
                    ProtoXlsxEnumItem item = new ProtoXlsxEnumItem(listList[j]);
                    if (item.valid)
                    {
                        protoXlsxEnumList.Add(item);
                    }
                }

                listDic.Add(i, new KeyValuePair<string, List<ProtoXlsxEnumItem>>(key, protoXlsxEnumList));
            }

            return listDic;
        }

        // 分析每行数据
        private void _generateDataRow()
        {
            IRow rowData = null;
            int i;

            // 从第6行开始是正式数据。
            for (i = XLS_DATA_INDEX; !_isEmptyRow(i); i++)
            {
                // 该行中每列数据的key和列的映射
                Dictionary<string, ICell> kvData = new Dictionary<string, ICell>();

                rowData = mCurSheet.GetRow(i);

                if (!_isValidRow(rowData, i))
                {
                    continue;
                }

                kvData.Clear();

                for (int j = 0; j < mCellCount; j++)
                {
                    string key = mDictHeadKV[j].Key;           // key的中文名
                    string enKey = mDictProtoKeyKV[j].Key;     // key的英文名

                    ICell cv = rowData.GetCell(j);
                    if (!kvData.ContainsKey(enKey))
                    {
                        kvData.Add(enKey, cv);
                    }

                    if (mDictProtoKeyKV.ContainsKey(j))
                    {
                        if (!kvData.ContainsKey(enKey))
                        {
                            kvData.Add(enKey, cv);
                        }
                    }
                }

                // 将该行的ID转成一个int，如果本身就是int，就用int本身，如果是string，就用strnig的hashcode
                // 该ikey用来判断ID是否重复。
                int ikey = kvData["ID"].CustomIDToInt(mCurIDType);

                if (!mDictData.ContainsKey(ikey))
                {
                    mDictData.Add(ikey, kvData);
                    mDictDataByLine.Add(i, kvData);
                    mDictID2Line.Add(ikey, i - XLS_DATA_INDEX);
                }
                else
                {
                    WarningWindow.PushError("存在重复ID {0},ID {1} 第 {2} 行与 {3} 重复", XlsName, ikey, i + 1, mDictID2Line[ikey] + XLS_DATA_INDEX + 1);
                }

                mRowCount = i + 1;
            }
        }

        // 判断每列数据是否是服务器需要的，第4行是serverFlag，如果标记为1，就是服务器需要的。
        private void _generateServerRow()
        {
            IRow rowData = mCurSheet.GetRow(PROTO_SERVER_FLAG);

            if (rowData == null)
            {
                return;
            }

            for (int i = 0; i < mCellCount; i++)
            {
                ICell cell = rowData.GetCell(i);

                if (cell != null)
                {
                    var array = cell.ToString().Trim().Split(SPLITE_KEY_VALUE);

                    if (array[0] == "1")
                    {
                        if (!mDicIsServer.ContainsKey(i))
                        {
                            mDicIsServer.Add(i, array.Length > 1 ? array[1] : "");
                        }
                        else
                        {
                            WarningWindow.PushError("already has the key {0}");
                        }
                    }
                }
            }
        }

        // 判断一行数据是否为空
        private bool _isEmptyRow(int index)
        {
            var cell = mCurSheet.GetRow(index);
            if (cell == null)
            {
                return true;
            }

            if (cell.GetCell(0) == null)
            {
                return true;
            }

            if (cell.GetCell(0).ToString().Equals(""))
            {
                return true;
            }

            return false;
        }

        // 判断一行数据的第1，2列数据是否有效
        private bool _isValidRow(IRow row, int idx)
        {
            ICell fstCell = row.GetCell(0);
            ICell sndCell = row.GetCell(1);

            if (fstCell == null || sndCell == null)
            {
                WarningWindow.PushError("[IsValidRow] : 表 {0}, 第 {1} 行， 第一第二列数据无效，请检查数据表", XlsName, idx);
                return false;
            }

            return true;
        }


        private static string[] VALID_MOD_STR = {
        "required",
        "optional",
        "repeated"
        };

        private static string[] VALID_TYPE_STR = {
        "sint32",
        "int32",
        "string",
        "bool",
        "enum",
        "union",
        "float",
        "union(float)",
        "realfloat"
        };

        private static string[] VALID_ID_TYPE_STR = {
        "sint32",
        "int32",
        "string",
        };

        // 检测每列的表头的类型是否都合法。
        private bool _isValidXls()
        {
            string curCellStr = "";

            Dictionary<string, int> typeNameDict = new Dictionary<string, int>();
            typeNameDict.Clear();

            for (int i = 0; i < mCellCount; i++)
            {
                curCellStr = mDictProtoModKV[i].Key;
                if (!_isValidProtoCell(curCellStr, VALID_MOD_STR))
                {
                    return false;
                }

                curCellStr = mDictProtoTypeKV[i].Key;
                if (!_isValidProtoCell(curCellStr, VALID_TYPE_STR))
                {
                    return false;
                }

                curCellStr = mDictProtoKeyKV[i].Key;
                if (!_isValidTypeName(curCellStr, ref typeNameDict, i))
                {
                    return false;
                }
            }


            for (int i = XLS_DATA_INDEX; i < mRowCount; i++)
            {
                IRow row = mCurSheet.GetRow(i);
                _isValidRow(row, i);
            }

            return true;
        }

        private bool _isValidProtoCell(string cellString, string[] array)
        {
            foreach (string item in array)
            {
                if (item == cellString)
                {
                    return true;
                }
            }

            WarningWindow.PushError("Table {0} : Type Must Be {1} ({2}) ", XlsName, string.Join(",", array), cellString);

            return false;
        }

        private bool _isValidTypeName(string typeName, ref Dictionary<string, int> typeNameDict, int idx = 0)
        {
            if (typeName.Length <= 0)
            {
                WarningWindow.PushError("[IsValidTypeName] : 表 {0}, 第 {1} 列 类型名为空", XlsName, idx);
                return false;
            }

            if (typeNameDict.ContainsKey(typeName))
            {
                WarningWindow.PushError("[IsValidTypeName] : 表 {0}, 第 {1} 列 类型名字与 第 {2} 列({3})相同", XlsName, idx, typeNameDict[typeName], typeName);
                return false;
            }
            else
            {
                typeNameDict.Add(typeName, idx);
                return true;
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

    }

    public class XlsxAnalyser
    {
    }
}
