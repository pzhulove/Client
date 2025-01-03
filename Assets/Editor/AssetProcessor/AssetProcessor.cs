using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetProcessor : AssetPostprocessor
{
    public static bool Enable = false;
    public static bool textureEnable = false;

    #region Preprocess
    //Texture
    public void OnPreprocessTexture()
    {
        if (textureEnable) {
            return;
        }
        if(!AssetProcessProfile.IsLoaded())
        {
            AssetProcessProfile.LoadAssetProfile();
        }

        List<BaseAssetProcessorData> textureProcessor = AssetProcessProfile.GetAssetProfile(AssetProcessProfile.AssetType.Texture);
        if(textureProcessor.Count > 0)
        {
            foreach(var processor in textureProcessor)
            {
                if(processor.ImportAsset(assetPath, assetImporter))
                {
                    break;
                }
            }
        }
    }

    //Mesh
    public void OnPreprocessModel()
    {
    }

    //Audio
    public void OnPreprocessAudio()
    {
        if (!AssetProcessProfile.IsLoaded())
        {
            AssetProcessProfile.LoadAssetProfile();
        }

        List<BaseAssetProcessorData> processors = AssetProcessProfile.GetAssetProfile(AssetProcessProfile.AssetType.Audio);
        if (processors.Count > 0)
        {
            foreach (var processor in processors)
            {
                if (processor.ImportAsset(assetPath, assetImporter))
                {
                    break;
                }
            }
        }
    }

    //Animation
    public void OnPreprocessAnimation()
    {
    }

    //All Asset
    public void OnPreprocessAsset()
    {
    }
#endregion


#region Postprocess
    //Texture
    //Texture 导入Texture都会调用
    //public void OnPostprocessTexture(Texture2D texture)
    //{
    //    Debug.Log("Post Process Texture");
    //}
    ////Cubemap 设置为Cubemap会调用
    //public void OnPostprocessCubemap(Cubemap texture)
    //{
    //    Debug.Log("Post Process Cubemap");
    //}
    ////Sprite 设置Texture为Sprite会调用
    //public void OnPostprocessSprites(Texture2D texture,Sprite[] sprites)
    //{
    //    Debug.Log("Post Process Sprites");
    //}

    ////Mesh
    //public void OnPostprocessModel(GameObject go)
    //{
    //    Debug.Log("Post Process Mesh");
    //}

    ////Audio
    //public void OnPostprocessAudio(AudioClip audioClip)
    //{
    //    Debug.Log("Post Process Audio");
    //}

    ////All Asset
    //public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //{
    //    Debug.Log("Post Process All");
    //}

    ////Other
    //public void OnAssignMaterialModel(Material material,Renderer renderer)
    //{
    //    Debug.Log("On Assign Material");
    //}
    //public void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName)
    //{
    //    Debug.Log("Assetbundle Name Changed");
    //}
    ////可以根据用户定义的字符串进行不同的处理
    //public void OnPostprocessGameObjectWithUserProperties(GameObject go,string[] propNames,System.Object[] values)
    //{
    //    Debug.Log("Post Process GameObject With UserProperties");
    //}
    //public void OnPostprocessMaterial(Material material)
    //{
    //    Debug.Log("Post Process Material");
    //}


#endregion


}
