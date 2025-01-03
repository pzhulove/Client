using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 用于限制图片导入后的设置
/// </summary>
public class ImageImportSetting :  AssetPostprocessor
{
    static string[] uncompressImageNames = { "UI/Image/Packed/p_UI_Bounty.png",
                                            "UI/Image/Packed/p_UI_Common00.png",
                                            "UI/Image/Packed/p_UI_Common01.png",
                                            "UI/Image/Packed/p_UI_Creat.png",
                                            "UI/Image/Packed/p_UI_Duobao.png",
                                            "UI/Image/Packed/p_UI_Pet.png",
                                            "UI/Image/Packed/p_UI_Shop_01.png"};
    //纹理导入之前调用，针对入到的纹理进行设置  
    public void OnPreprocessTexture()
    {
        if(!this.assetPath.Contains("Assets/Resources"))
        {
            return;
        }
        foreach(var name in uncompressImageNames)
        {
            if(this.assetPath.Contains(name))
            {
                TextureImporter impor = this.assetImporter as TextureImporter;

                impor.textureCompression = TextureImporterCompression.Uncompressed;

                var androidTextureSetting = new TextureImporterPlatformSettings();
                androidTextureSetting.name = "Android";
                androidTextureSetting.overridden = false;
                impor.SetPlatformTextureSettings(androidTextureSetting);

                var iPhoneTextureSetting = new TextureImporterPlatformSettings();
                iPhoneTextureSetting.name = "iPhone";
                iPhoneTextureSetting.overridden = false;
                impor.SetPlatformTextureSettings(iPhoneTextureSetting);

                break;
            }
        }
    }
}
