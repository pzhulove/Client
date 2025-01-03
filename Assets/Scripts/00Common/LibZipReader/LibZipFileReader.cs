using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.IO;
using System.Security;
using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
#endif

namespace LibZip
{
    class LibZipFileReader
    {
        private static IntPtr zip = IntPtr.Zero;

        public const string FULL_STORAGE_TIPS = "手机空间不足，建议清理后重试";

        private static string TAG = "ZipFileReader";

        public static bool FileExist(string strZipFilePath)
        {
            if (zip == IntPtr.Zero)
            {
                //zip = LibZip.zip_open(Application.dataPath, 0, IntPtr.Zero);
                zip = LibZip.zip_open(Application.dataPath, 0, IntPtr.Zero);
            }

            if (zip == IntPtr.Zero)
            {
                //Logger.LogError( "zip package not found: " + Application.dataPath);
                Logger.LogError("zip package not found: " + Application.dataPath);
                return false;
            }

            string filePath = Path.Combine("assets", strZipFilePath);
            IntPtr zipfile = LibZip.zip_fopen(zip, filePath, 0);

            if (zipfile == IntPtr.Zero)
            {
                Logger.LogError("zip file not found: " + Path.Combine(Application.dataPath, filePath));
                //LibZip.zip_close( zip ) ;
                return false;
            }

            LibZip.zip_fclose(zipfile);
            //LibZip.zip_close( zip ) ;
            return true;
        }

        public static bool Read(string strZipFilePath, ref byte[] outBuffer, ref Int64 iBufferSize)
        {
            outBuffer = null;
            iBufferSize = 0;

            if (zip == IntPtr.Zero)
            {
                zip = LibZip.zip_open(Application.dataPath, 0, IntPtr.Zero);
            }

            if (zip == IntPtr.Zero)
            {
                Logger.LogError("zip package not found: " + Application.dataPath);
                return false;
            }

            string filePath = Path.Combine("assets", strZipFilePath);
            IntPtr zipfile = LibZip.zip_fopen(zip, filePath, 0);

            if (zipfile == IntPtr.Zero)
            {
                Logger.LogError("zip file not found: " + Path.Combine(Application.dataPath, filePath));
                //LibZip.zip_close(zip);
                return false;
            }

            zip_stat zipfile_stat = new zip_stat();
            LibZip.zip_stat(zip, filePath, 0, ref zipfile_stat);

            outBuffer = new byte[zipfile_stat.size + 1];
            outBuffer[zipfile_stat.size] = 0;
            iBufferSize = zipfile_stat.size + 1;
            Int64 numBytesRead = LibZip.zip_fread(zipfile, outBuffer, zipfile_stat.size);

            LibZip.zip_fclose(zipfile);
            //LibZip.zip_close(zip); 
            return true;
        }

		public static bool CompressFiles(string target, string[] files)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
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
                LibZip.zip_close(zipPtr);
				return false;
			}

			for (int i = 0; i < files.Length; ++i) 
            {
				//UnityEngine.Debug.Log ("File " + files [i]);

				if (File.Exists(files[i])) 
                {
					byte[] allbytes = File.ReadAllBytes(files[i]);

					IntPtr sourcePtr = LibZip.zip_source_buffer(zipPtr, allbytes, allbytes.Length, 0);
					if (IntPtr.Zero	== sourcePtr) 
                    {
						Logger.LogErrorFormat("[Zip] CompressFiles Read File fail {0}", files[i]);
						continue;
					}

					long idx = LibZip.zip_file_add(zipPtr, Path.GetFileName(files[i]), sourcePtr, 8192);
					if (idx < 0) 
                    {
						Logger.LogErrorFormat ("[Zip] CompressFiles Add File fail {0}", files[i]);
						continue;
					}
				}
			}

			LibZip.zip_close(zipPtr);
			return true;
# elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            try
            {
                using (ZipOutputStream s = new ZipOutputStream(File.Create(target)))
                {
                    s.SetLevel(9);//0-9 值越大压缩率越高
                    byte[] buffer = new byte[4096];
                    foreach (string file in files)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                            fs.Close();
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
#else
            return false;
#endif
        }

        public static bool UncompressAll(String zippath)
        {
            const Int64 BUFF_SIZE = 1024 * 256;
            // TODO check the sDataPath
            string sDataPath = Path.GetDirectoryName(zippath);

            byte[] bReadBuf = new byte[BUFF_SIZE];
            Int64 iReadLen = 0;

            IntPtr zipPtr = IntPtr.Zero;
            IntPtr filePtr = IntPtr.Zero;

            //zippath = Path.Combine(sDataPath, zippath);

            Logger.Log("zip path : " + zippath);

            // open the zip file
            zipPtr = LibZip.zip_open(zippath, 0, IntPtr.Zero);

            if (zipPtr == IntPtr.Zero)
            {
                Logger.Log( "zip not found: " + zippath) ;
                return false;
            }

            int iCount = LibZip.zip_get_num_files(zipPtr);

            Logger.Log("zip file count : " + iCount);

            zip_stat zipStat = new zip_stat();

            for (int i = 0; i < iCount; i++)
            {
                if (LibZip.zip_stat_index(zipPtr, i, 0, ref zipStat) == 0)
                {
                    string zipStatName = Marshal.PtrToStringAnsi(zipStat.name);

                    Logger.Log("size     : " + zipStat.size);
                    Logger.Log("filename : " + zipStatName);

                    if (zipStatName.EndsWith("/"))
                    {
                        // create the dir
                        string sPath = Path.Combine(sDataPath, zipStatName);
                        // TODO Create Directory
                        //M3MFileUtil.CreateDirectory(sPath);
                    }
                    else {
                        filePtr = LibZip.zip_fopen_index(zipPtr, i, 0);

                        if (filePtr == IntPtr.Zero)
                        {
                            Logger.LogError("open file fail " + zipStat.name);
                            return false;
                        }

                        string dstFileName = Path.Combine(sDataPath, zipStatName);
                        string dirName = Path.GetDirectoryName(dstFileName);

                        try
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        catch (System.Exception e)
                        {
                            Logger.LogError("create dir error " + e.ToString());
                        }

                        FileStream fs = new FileStream(dstFileName, FileMode.Create, FileAccess.Write);

                        Logger.Log("save file path : " + dstFileName);

                        while ((iReadLen = LibZip.zip_fread(filePtr, bReadBuf, BUFF_SIZE)) > 0)
                        {
                            try
                            {
                                fs.Write(bReadBuf, 0, (int)iReadLen);
                            }
                            catch (System.Exception e)
                            {
                                Logger.LogError(FULL_STORAGE_TIPS);
                                break;
                            }
                        }

                        Logger.Log("write finish!");

                        // write finish
                        fs.Close();

                        LibZip.zip_fclose(filePtr);
                        filePtr = IntPtr.Zero;
                    }
                }
            }

            LibZip.zip_close(zipPtr);
            return true;
        }

        public delegate void ProgressReport(float percentage);
        static WaitForEndOfFrame WAIT_FOR_EOF = new WaitForEndOfFrame();
        public static IEnumerator UncompressAllAsync(String zippath, int chunkSizeKB = 64 ,ProgressReport report = null)
        {
            Int64 BUFF_SIZE = 1024 * chunkSizeKB;
            // TODO check the sDataPath
            string sDataPath = Path.GetDirectoryName(zippath);

            byte[] bReadBuf = new byte[BUFF_SIZE];
            Int64 iReadLen = 0;

            IntPtr zipPtr = IntPtr.Zero;
            IntPtr filePtr = IntPtr.Zero;

            //zippath = Path.Combine(sDataPath, zippath);

            Logger.Log("zip path : " + zippath);

            // open the zip file
            zipPtr = LibZip.zip_open(zippath, 0, IntPtr.Zero);

            if (null != report)
                report(0.01f);

            if (zipPtr == IntPtr.Zero)
            {
                Logger.Log("zip not found: " + zippath);
                yield break;
            }

            int iCount = LibZip.zip_get_num_files(zipPtr);

            Logger.Log("zip file count : " + iCount);

            zip_stat zipStat = new zip_stat();

            for (int i = 0; i < iCount; i++)
            {
                report(0.01f + ((i * 0.98f)/iCount));

                if (LibZip.zip_stat_index(zipPtr, i, 0, ref zipStat) == 0)
                {
                    string zipStatName = Marshal.PtrToStringAnsi(zipStat.name);

                    Logger.Log("size     : " + zipStat.size);
                    Logger.Log("filename : " + zipStatName);

                    if (zipStatName.EndsWith("/"))
                    {
                        // create the dir
                        string sPath = Path.Combine(sDataPath, zipStatName);
                        // TODO Create Directory
                        //M3MFileUtil.CreateDirectory(sPath);
                    }
                    else
                    {
                        filePtr = LibZip.zip_fopen_index(zipPtr, i, 0);

                        if (filePtr == IntPtr.Zero)
                        {
                            Logger.LogError("open file fail " + zipStat.name);
                            continue;
                        }

                        string dstFileName = Path.Combine(sDataPath, zipStatName);
                        string dirName = Path.GetDirectoryName(dstFileName);

                        try
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        catch (System.Exception e)
                        {
                            Logger.LogError("create dir error " + e.ToString());
                        }

                        FileStream fs = new FileStream(dstFileName, FileMode.Create, FileAccess.Write);

                        Logger.Log("save file path : " + dstFileName);

                        while ((iReadLen = LibZip.zip_fread(filePtr, bReadBuf, BUFF_SIZE)) > 0)
                        {
                            try
                            {
                                fs.Write(bReadBuf, 0, (int)iReadLen);
                            }
                            catch (System.Exception e)
                            {
                                Logger.LogError(FULL_STORAGE_TIPS);
                                break;
                            }

                            yield return WAIT_FOR_EOF;
                        }

                        Logger.Log("write finish!");

                        // write finish
                        LibZip.zip_fclose(filePtr);
                        fs.Close();
                        filePtr = IntPtr.Zero;
                    }
                }
            }

            LibZip.zip_close(zipPtr);

            report(1.0f);
        }

    }
}
