using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;
using XUPorterJSON;

namespace GameClient
{
    public class PlayerLocalSetting
    {
        protected static Hashtable Settings = new Hashtable();
        
        public static System.Object GetValue(string key)
        {
            if(Settings == null)
            {
                return null;
            }

            if(!Settings.ContainsKey(key))
            {
                return null;
            }

            return Settings[key];
        }

        public static void SetValue(string key,System.Object value)
        {
            if(Settings == null)
            {
                Settings = new Hashtable();
            }

            if (!Settings.ContainsKey(key))
            {
                Settings.Add(key, value);
            }
            else
            {
                Settings[key] = value;
            }
        }

        public static string getSettingPath()
        {
            return Application.persistentDataPath + "/player.config";
        }
        public static void LoadConfig()
        {
            //try
            //{
            //    if (CFileManager.IsFileExist(getSettingPath()))
            //    {
            //        byte[] data = CFileManager.ReadFile(getSettingPath());
            //        if (data != null)
            //        {
            //            Settings = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(data)) as Hashtable;
            //        }
            //    }
            //}
            //catch(Exception e)
            //{
            //}
#if !MG_TEST && !MG_TEST2
            byte[] data = null;
            try
            {
                FileArchiveAccessor.LoadFileInPersistentFileArchive("player.config", out data);
                if (data != null)
                {
                    Settings = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(data)) as Hashtable;
                }
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("Read player Config failed reason {0}", e.Message);
            }
#else
            string persistentPath = Application.persistentDataPath;
            string filePath = Path.Combine(persistentPath, "player.config");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("Delete Old Player Config failed reason {0}", e.Message);
            }
            try
            {
                byte[] data = null;

                FileArchiveAccessor.LoadFileInPersistentFileArchive("player_test.config", out data);
                if (data != null)
                {
                    Settings = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(data)) as Hashtable;
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("Read player_test Config failed reason {0}",e.Message);
            }
#endif
        }

        public static void SaveConfig()
        {
            //string str_data = MiniJSON.jsonEncode(Settings);
            //bool bRet = CFileManager.WriteFile(getSettingPath(), System.Text.UTF8Encoding.Default.GetBytes(str_data));
            // 
            //if( bRet == false )
            // {
            //     Logger.LogError("Config do not save correct!\n");
            // }
#if !MG_TEST  && !MG_TEST2
            try
            {
                string str_data = MiniJSON.jsonEncode(Settings);
                FileArchiveAccessor.SaveFileInPersistentFileArchive("player.config", str_data);
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("Save player Config failed reason {0}", e.Message);
            }
#else
            try
            {
                string str_data = MiniJSON.jsonEncode(Settings);
                FileArchiveAccessor.SaveFileInPersistentFileArchive("player_test.config", str_data);
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("Save player_test Config failed reason {0}",e.Message);
            }
#endif
        }
    }
}
