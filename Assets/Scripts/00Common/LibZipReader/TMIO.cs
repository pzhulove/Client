namespace Tenmove
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using LibZip;

    public class TMZipFile
    {
        public const string FULL_STORAGE_TIPS = "手机空间不足，建议清理后重试";

        private static byte[] skEmptyBuffer = new byte[0];

        private static string TAG = "ZipFileReader";

        public static bool FileExist(string zipFilePath, string strZipFilePath)
        {
            if (!TMFile.FileExist(zipFilePath))
            {
                Logger.LogErrorFormat("zip package not found: {0}", zipFilePath);
                return false;
            }

            IntPtr zip = IntPtr.Zero;
            string filePath = strZipFilePath;
            IntPtr zipfile = LibZip.zip_fopen(zip, filePath, 0);
            if (zipfile == IntPtr.Zero)
            {
                Logger.LogError("zip file not found: " + Path.Combine(zipFilePath, filePath));
                return false;
            }

            LibZip.zip_fclose(zipfile);
            return true;
        }


        public static bool ReadAllText(string zipFilePath, string strZipFilePath, ref string ans)
        {
            byte[] textBuffer = null;
            Int64 textBufferSize = -1;

            if (Read(zipFilePath, strZipFilePath, ref textBuffer, ref textBufferSize))
            {
                return false;
            }

            ans = Encoding.UTF8.GetString(textBuffer);
            return true;
        }

        public static bool Read(string zipFilePath, string strZipFilePath, ref byte[] outBuffer, ref Int64 iBufferSize)
        {
            outBuffer = skEmptyBuffer;
            iBufferSize = 0;

            if (!TMFile.FileExist(zipFilePath))
            {
                Logger.LogError("zip package not found: " + zipFilePath);
                return false;
            }

            IntPtr zip = IntPtr.Zero;
            string filePath = strZipFilePath;
            IntPtr zipfile = LibZip.zip_fopen(zip, filePath, 0);
            if (zipfile == IntPtr.Zero)
            {
                Logger.LogError("zip file not found: " + Path.Combine(zipFilePath, filePath));
                return false;
            }

            Int64 relativeFileSize = _GetZipRelativeFileSize(zip, filePath);

            outBuffer = new byte[relativeFileSize + 1];
            outBuffer[relativeFileSize] = 0;
            iBufferSize = relativeFileSize + 1;

            Int64 numBytesRead = LibZip.zip_fread(zipfile, outBuffer, relativeFileSize);

            LibZip.zip_fclose(zipfile);
            return true;
        }


        private static Int64 _GetZipRelativeFileSize(IntPtr zipPtr, string relativePath)
        {
            zip_stat zipfileStat = new zip_stat();
            LibZip.zip_stat(zipPtr, relativePath, 0, ref zipfileStat);
            return zipfileStat.size;
        }

        public static readonly int skReadBuffSize = 1024 * 4;
        private static byte[] skReadBuff = new byte[skReadBuffSize];

        public static bool CompressFiles(string target, string root, string[] files)
        {
            if (null == files || 0 == files.Length)
            {
                Logger.LogErrorFormat("[Zip] CompressFiles file list is empty");
                return false;
            }

            IntPtr zipPtr = IntPtr.Zero;

            zipPtr = LibZip.zip_open(target, 1, IntPtr.Zero);
            if (IntPtr.Zero == zipPtr)
            {
                Logger.LogErrorFormat("[Zip] CompressFiles Open File fail {0}", target);
                return false;
            }

            for (int i = 0; i < files.Length; ++i)
            {
                WriteOneFile2ZipSource(zipPtr, root, files[i]);
            }

            LibZip.zip_close(zipPtr);
            return true;
        }

        public static bool WriteOneFile2ZipSource(IntPtr zipPtr, string root, string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            if (IntPtr.Zero == zipPtr)
            {
                return false;
            }

            IntPtr sourcePtr = LibZip.zip_source_file(zipPtr, filePath, 0, 0);
            if (IntPtr.Zero == sourcePtr)
            {
                Logger.LogErrorFormat("[Zip] zip_source_file error {0}", filePath);
                return false;
            }

            if (0 != LibZip.zip_source_begin_write(sourcePtr))
            {
                Logger.LogErrorFormat("[Zip] zip_source_begin_write error {0}", filePath);
                return false;
            }

            FileStream file2Read = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            long readSize = 0;
            do
            {
                int size = file2Read.Read(skReadBuff, 0, skReadBuffSize);
                if (size <= 0)
                {
                    break;
                }

                int sourceZipWriteSize = LibZip.zip_source_write(sourcePtr, skReadBuff, size);
                if (sourceZipWriteSize != size)
                {
                    Logger.LogErrorFormat("[Zip] zip_source_write write file size error {0}  {1} != {2}", filePath, size, sourceZipWriteSize);
                    file2Read.Close();
                    LibZip.zip_source_rollback_write(sourcePtr);
                    return false;
                }

                readSize += size;
            }
            while (true);

            string relativePath = (filePath).Substring(root.Length, filePath.Length - root.Length);
            long idx = LibZip.zip_file_add(zipPtr, relativePath, sourcePtr, 8192);
            if (idx < 0)
            {
                Logger.LogErrorFormat("[Zip] CompressFiles Add File fail {0}", filePath);
                file2Read.Close();
                LibZip.zip_source_rollback_write(sourcePtr);
                return false;
            }

            file2Read.Close();
            LibZip.zip_source_commit_write(sourcePtr);

            return true;
        }

    }

    public class TMFile
    {
        public static bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public static string[] GetFiles(string root, string filter = "*", bool isAllDirectories = true)
        {
            try
            {
                SearchOption option = SearchOption.TopDirectoryOnly;
                if (isAllDirectories)
                {
                    option = SearchOption.AllDirectories;
                }

                var files = Directory.GetFiles(root, filter, option);

                return files;
            }
            catch (System.Exception e)
            {
                // TODO log error

            }

            return new string[0];
        }

        /// <summary>
        /// 根据平台来加载DataPath下的数据
        ///
        /// Android是一个Zip包
        /// iOS和其他什么的都是路径
        /// </summary>
        public static bool ReadAllTextRelativeDataPath(string relativePath, out string content)
        {
            content = string.Empty;


            return false;
        }

        public static byte[] ReadAllBytes(string filePath)
        {
            if (File.Exists(filePath))
            {
                try 
                {
                    return File.ReadAllBytes(filePath);
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat(e.ToString());
                }
            }

            return new byte[0];
        }

        public static bool ReadAllText(string filePath, out string content)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    content = File.ReadAllText(filePath);
                }
                catch (Exception e)
                {
                    // TODO log error
                    Logger.LogErrorFormat(e.ToString());
                }
            }

            content = string.Empty;

            return false; ;
        }

        public static void WriteAllContents(string filePath, string content)
        {
            TMPathUtil.MakeParentRootExist(filePath);
            File.WriteAllText(filePath, content);
        }

        public static void AppendAllContents(string filePath, string content)
        {
            TMPathUtil.MakeParentRootExist(filePath);
            File.AppendAllText(filePath, content);
        }
    }

    public class TMPathUtil
    {
        private static string mRoot = "..";

        public static string Root
        {
            get
            {
                return mRoot;
            }

            set
            {
                mRoot = value;
            }
        }

        public enum Type
        {
            Logs,
            ScreenShots,
            Infos,
        }


        public static void MakeParentRootExist(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string rootPath = filePath.Remove(filePath.Length - fileName.Length, fileName.Length);

            CreateRootDir(rootPath);
        }

        public static void CreateRootDir(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public static string GetTypeRootPath(Type type)
        {
            var rootPath = Path.Combine(mRoot, type.ToString());

            CreateRootDir(rootPath);

            return rootPath;
        }

        public static string GetTypeRootPathWithFileName(Type type, string fileName)
        {
            var root = GetTypeRootPath(type);

            return Path.Combine(root, fileName);
        }

        private static string _SplitFileName(string filename)
        {
            string[] splits = filename.Split('_');

            if (null != splits && splits.Length > 0)
            {
                return splits[0];
            }

            return string.Empty;
        }

        public static string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        }

        public static string CreateTypeNumberDir(Type type, ref string dirName)
        {
            string rootPath = GetTypeRootPath(type);

            var allDirInRoot = Directory.GetDirectories(rootPath);

            if (string.IsNullOrEmpty(dirName))
            {
                int maxValue = 0;
                for (int i = 0; i < allDirInRoot.Length; ++i)
                {
                    int v = 0;

                    if (int.TryParse(_SplitFileName(Path.GetFileName(allDirInRoot[i])), out v))
                    {
                        if (maxValue < v)
                        {
                            maxValue = v;
                        }
                    }
                }

                maxValue++;

                dirName = string.Format("{0:D6}_{1}", maxValue, GetCurrentDateTime());
            }

            var dir = System.IO.Path.Combine(rootPath, dirName);

            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }
}
