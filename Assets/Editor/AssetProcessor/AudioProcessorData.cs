using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

public class AudioProcessorData : BaseAssetProcessorData
{
    public bool forceToMono = false;
    public AudioClipLoadType loadType = AudioClipLoadType.DecompressOnLoad;
    public AudioSampleRateSetting sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
    public int sampleRateOverride = 44100;

    //是否后台线程加载，适合music
    public bool loadInBackground = false;
    public int quailty = 50;
    public AudioCompressionFormat compressFormat = AudioCompressionFormat.Vorbis;

    string[] sampleRateStrings = {
        "8,000 Hz",
        "11,025 Hz",
        "22,050 Hz",
        "44,100 Hz",
        "48,000 Hz",
        "96,000 Hz",
        "192,000 Hz",
    };

    int[] sampleRateValues = {
        8000,
        11025,
        22050,
        44100,
        48000,
        96000,
        192000
    };


    public AudioProcessorData()
    {
        assetType = typeof(AudioClip);
        forceToMono = false;
        loadType = AudioClipLoadType.DecompressOnLoad;
    }

    public override void DisplayAndChangeData()
    {
        //throw new System.NotImplementedException();
        EditorGUILayout.BeginVertical();
        {
            loadInBackground = EditorGUILayout.Toggle("Loading in background", loadInBackground);
            forceToMono = EditorGUILayout.Toggle("Force To Mono", forceToMono);
            loadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Load Type", loadType);
            sampleRateSetting = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Sample Rate Setting", sampleRateSetting);
            if (sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate)
            {
                sampleRateOverride = EditorGUILayout.IntPopup("Sample Rate", sampleRateOverride, sampleRateStrings, sampleRateValues);
            }
            quailty = (int)EditorGUILayout.Slider(quailty, 0, 100);
            compressFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Compression Format", compressFormat);
        }
        EditorGUILayout.EndVertical();
    }

    public override void OnImportAsset(AssetImporter assetImporter, string assetPath)
    {
        //throw new System.NotImplementedException();

        AudioImporter importer = assetImporter as AudioImporter;
        if (importer == null)
        {
            return;
        }

        if (importer.forceToMono != forceToMono)
        {
            importer.forceToMono = forceToMono;
        }

        if (importer.loadInBackground != loadInBackground)
            importer.loadInBackground = loadInBackground;
        
        

        AudioImporterSampleSettings sampleSettings = importer.defaultSampleSettings;

        sampleSettings.quality = quailty/100f;
        sampleSettings.compressionFormat = compressFormat;

        if (sampleSettings.loadType != loadType)
        {
            sampleSettings.loadType = loadType;   
        }
        if (sampleSettings.sampleRateSetting != sampleRateSetting)
        {
            sampleSettings.sampleRateSetting = sampleRateSetting;

            if (sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate && sampleRateOverride != sampleSettings.sampleRateOverride)
            {
                sampleSettings.sampleRateOverride = (uint)sampleRateOverride;
            }
        }
        importer.defaultSampleSettings = sampleSettings;
    }

    public override void SaveToXML(XmlDocument xml, XmlNode node)
    {
        base.SaveToXML(xml, node);

        System.Type thisType = this.GetType();
        FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (IsBaseField(field.Name))
                continue;

            XmlElement assetNode = xml.CreateElement("property");

            assetNode.SetAttribute("key", field.Name);
            assetNode.SetAttribute("value", field.GetValue(this).ToString());

            if (field.GetType().IsEnum)
            {
                // assetNode.SetAttribute("type", field.GetType().Name);
            }

            node.AppendChild(assetNode);
        }
    }

    public override bool LoadFromXML(XmlNode node)
    {
        System.Type thisType = this.GetType();
        FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        Dictionary<string, FieldInfo> fieldInfoDic = new Dictionary<string, FieldInfo>();
        foreach (var field in fields)
        {
            if (IsBaseField(field.Name))
                continue;

            fieldInfoDic.Add(field.Name, field);
        }


        foreach (XmlNode assetNode in node.ChildNodes)
        {
            string fieldName = (assetNode as XmlElement).GetAttribute("key");
            string value = (assetNode as XmlElement).GetAttribute("value");

            FieldInfo field;
            if (fieldInfoDic.TryGetValue(fieldName, out field))
            {
                //field.SetValue(this, );
                Type fieldType = field.FieldType;
                if (fieldType.IsEnum)
                {
                    // string typeName = (assetNode as XmlElement).GetAttribute("type");
                    field.SetValue(this, Enum.Parse(fieldType, value));
                }
                else if (fieldType == typeof(int) || fieldType == typeof(uint))
                {
                    field.SetValue(this, int.Parse(value));
                }
                else if (fieldType == typeof(string))
                {
                    field.SetValue(this, value);
                }
                else if (fieldType == typeof(bool))
                {
                    field.SetValue(this, bool.Parse(value));
                }
                else if (fieldType == typeof(float))
                {
                    field.SetValue(this, float.Parse(value));
                }
                else
                {
                    Debug.LogErrorFormat("Unknown Filed in TextureProcessorData, {0}, {1}", fieldType.Name, fieldName);
                }
            }
        }

        return base.LoadFromXML(node);
    }
}
