using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationProcessorData : BaseAssetProcessorData
{
    public AnimationProcessorData()
    {
        assetType = typeof(Animation);
    }

    public override void DisplayAndChangeData()
    {
        throw new System.NotImplementedException();
    }

    public override void OnImportAsset(AssetImporter assetImporter, string assetPath)
    {
        throw new System.NotImplementedException();
    }
}
