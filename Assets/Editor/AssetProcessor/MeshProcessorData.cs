using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshProcessorData : BaseAssetProcessorData
{
    public MeshProcessorData()
    {
        assetType = typeof(Mesh);
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
