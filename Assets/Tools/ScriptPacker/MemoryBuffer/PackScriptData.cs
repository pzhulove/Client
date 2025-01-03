using MemoryWriteReaderAnimation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tenmove.Runtime;
using UnityEngine;

public class PackScriptData : SingletonData<PackScriptData>
{
    //private AssetLoadCallbacks m_Callback = new AssetLoadCallbacks(_OnLoadSuccess, _OnLoadFailure);

    private const int HEADER_VERSION = 1;
    
    public List<string> header_file_names = new List<string>();         // 二进制文件名称的集合                                                 
    public Dictionary<string, PackHeaderItemInfo> header_dic_string = 
        new Dictionary<string, PackHeaderItemInfo>();                   // 小文件与二进制文件的映射关系（可考虑按类型进行分组，提高查询效率）。

    private bool m_Inited = false;

    protected override void AwakeInstance()
    {
        InitInstance();
    }

    protected override void ReleaseInstance()
    {
        this.header_dic_string.Clear();
        this.header_file_names.Clear();
        this.m_Inited = false;

        ScriptHelper.Clear();
    }

    protected void InitInstance()
    {
        LoadHeaderInfo();
        //ScriptHelper.GetData("Data/ScriptData/DNLAavatarPart.bytes");
        //ScriptHelper.GetData("Data/ScriptData/DNLAnimation.bytes");
        //ScriptHelper.GetData("Data/ScriptData/DNLFashion.bytes");   
        //ScriptHelper.GetData("Data/ScriptData/DNLGraphicEffect.bytes");
        ScriptHelper.GetData("Data/ScriptData/SkillList.bytes");
    }

    public void Clear()
    {
        ReleaseInstance();
    }

    public void ReloadHeaderInfo()
    {
        //Clear();
        //LoadHeaderInfo();
    }

    private void OnHeaderLoaded(byte[] scriptData)
    {
        if (scriptData == null || scriptData.Length <= 4)
        {
            return;
        }

        int decompressedSize = BitConverter.ToInt32(scriptData, 0);
        byte[] header = ScriptHelper.DecodeLZ4(scriptData, 4, scriptData.Length - 4, decompressedSize);

        if (header == null)
        {
            return;
        }

        using (MemoryStream memoryStream = new MemoryStream(header))
        using (BinaryReader binaryReader = new BinaryReader(memoryStream))
        {
            int headerVersion = binaryReader.ReadInt32();
            if (headerVersion > HEADER_VERSION)
            {
                Debugger.LogError("PackScriptData HeaderVersion Error: {0}, should be <= {1}", headerVersion, HEADER_VERSION);
                return;
            }

            int headerFileNum = binaryReader.ReadInt32();
            if (this.header_file_names == null)
            {
                this.header_file_names = new List<string>(headerFileNum);
            }

            for (int i = 0; i < headerFileNum; i++)
            {
                string fileName = binaryReader.ReadString();
                this.header_file_names.Add(fileName);
            }

            int scriptNum = binaryReader.ReadInt32();
            if (this.header_dic_string == null)
            {
                this.header_dic_string = new Dictionary<string, PackHeaderItemInfo>(scriptNum);
            }

            for (int j = 0; j < scriptNum; j++)
            {
                string scriptName = binaryReader.ReadString();
                int nFileIndex = binaryReader.ReadInt32();
                int nPeekStart = binaryReader.ReadInt32();
                int nPeekLength = binaryReader.ReadInt32();

                if (nFileIndex >= 0 && nFileIndex < headerFileNum)
                {
                    PackHeaderItemInfo headerItem = new PackHeaderItemInfo(nFileIndex, nPeekStart, nPeekLength);
                    this.header_dic_string[scriptName] = headerItem;
                }
            }
        }
    }
    
    public void LoadHeaderInfo()
    {
        if (m_Inited)
            return;

        m_Inited = true;

        ScriptHelper.GetHeader("Data/ScriptData/ScriptHeader.bytes", OnHeaderLoaded);
    }

#if UNITY_EDITOR
    public bool SaveHeaderInfo()
    {
        string ScriptDataPath = "Assets/Resources/Data/ScriptData/ScriptHeader.bytes";
        string ScriptDataXMLPath = "ScriptHeaderDebug.xml";

        if (header_dic_string == null || header_dic_string.Count == 0)
            return true;

        System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
        System.Xml.XmlElement xmlRoot = xmlDoc.CreateElement("Root");

        MemoryBufferWriter memWriter = new MemoryBufferWriter();

        {
            memWriter.Write(HEADER_VERSION);
            int headerFileNum = this.header_file_names.Count;
            memWriter.Write(headerFileNum);

            System.Xml.XmlElement xmlHeaderFiles = xmlDoc.CreateElement("PackageFiles");

            for (int i = 0; i < headerFileNum; i++)
            {
                memWriter.Write(header_file_names[i]);

                System.Xml.XmlElement xmlHeaderFile = xmlDoc.CreateElement("PackageFile");
                xmlHeaderFile.SetAttribute("name", header_file_names[i]);
                xmlHeaderFiles.AppendChild(xmlHeaderFile);
            }
            xmlRoot.AppendChild(xmlHeaderFiles);
            System.Xml.XmlElement xmlFileInfos = xmlDoc.CreateElement("FileInfos");

            int scriptNum = header_dic_string.Count;
            memWriter.Write(scriptNum);

            foreach(var keyPair in header_dic_string)
            {
                memWriter.Write(keyPair.Key);

                memWriter.Write(keyPair.Value._FileIndex);
                memWriter.Write(keyPair.Value._nPeekStart);
                memWriter.Write(keyPair.Value._nPeekLength);

                System.Xml.XmlElement xmlFileInfo = xmlDoc.CreateElement("FileInfo");
                xmlFileInfo.SetAttribute("name", keyPair.Key);
                xmlFileInfo.SetAttribute("PackageIndex", keyPair.Value._FileIndex.ToString());
                xmlFileInfo.SetAttribute("Offset", keyPair.Value._nPeekStart.ToString());
                xmlFileInfo.SetAttribute("Length", keyPair.Value._nPeekLength.ToString());
                xmlFileInfos.AppendChild(xmlFileInfo);
            }

            xmlRoot.AppendChild(xmlFileInfos);
        }

        xmlDoc.AppendChild(xmlRoot);
        xmlDoc.Save(ScriptDataXMLPath);

        byte[] rawBuffer = memWriter.GetBufferData();
        byte[] packedBuffer = ScriptHelper.EncodeLZ4(rawBuffer, 0, rawBuffer.Length);
        using (FileStream fs = new FileStream(ScriptDataPath, FileMode.Create))
        {
            int rawSize = rawBuffer.Length;
            fs.Write(BitConverter.GetBytes(rawSize), 0, 4);
            fs.Write(packedBuffer, 0, packedBuffer.Length);
        }

        return true;
    }
#endif

    public bool IsExistScript(string strFileName)
    {
        if (!m_Inited)
        {
            LoadHeaderInfo();
        }

        if (this.header_dic_string.ContainsKey(strFileName))
        {
            return true;
        }

        return false;
    }

    public byte[] GetPackBinary(string file_name, ref int startIndex, ref int length)
    {
        if (string.IsNullOrEmpty(file_name))
        {
            return null;
        }

        if (!m_Inited)
        {
            LoadHeaderInfo();
        }

        //file_name = file_name.ToLower().Replace("\\", "/");
        PackHeaderItemInfo packHeaderItemInfo;

        if (this.header_dic_string.TryGetValue(file_name, out packHeaderItemInfo))
        {
            return ScriptHelper.GetScript(header_file_names[packHeaderItemInfo._FileIndex], 
                packHeaderItemInfo._nPeekStart, packHeaderItemInfo._nPeekLength, ref startIndex, ref length);
        }

        return null;
    }
}