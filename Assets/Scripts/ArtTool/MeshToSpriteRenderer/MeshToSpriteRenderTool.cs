#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class MeshToSpriteRenderTool : MonoBehaviour
{
    [HideInInspector]
    public bool OddOffset = false;
    [HideInInspector]
    public bool SplitTexFlipX = false;
    [HideInInspector]
    public bool SplitTexFlipY = false;

    public void StartChange(bool ignoreOffset = false)
    {
        MeshToSpriteRender.InitLogAbout();
        MeshRenderer[] allRender = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in allRender)
        {
            MeshToSpriteRender splitCs = new MeshToSpriteRender(mr, SplitTexFlipX, SplitTexFlipY);
            splitCs.SplitAtlas();
            MeshToSprite(splitCs, ignoreOffset);
        }
        MeshToSpriteRender.OutputLogAbout();
        DestroyImmediate(this);
    }

    private void MeshToSprite(MeshToSpriteRender splitCs, bool ignoreOffset = false)
    {
        Sprite spr = splitCs.MainSprite;
        if (spr == null)
            return;
        GameObject objRender = splitCs.Mr.gameObject;
        Transform transRender = objRender.transform;
        MeshRenderer render = splitCs.Mr;
        MeshFilter mf = splitCs.Mf;
        DestroyImmediate(render);
        DestroyImmediate(mf);
        SpriteRenderer sprRender = objRender.AddComponent<SpriteRenderer>();
        if (!ignoreOffset)
            transRender.position += splitCs.PosOffset;
        sprRender.sharedMaterial = splitCs.NewMat;
        sprRender.sprite = spr;
        sprRender.flipX = splitCs.FlipX;
        sprRender.flipY = splitCs.FlipY;
        if (!splitCs.YEffective)
            transRender.localEulerAngles = new Vector3(transRender.localEulerAngles.x - 90, transRender.localEulerAngles.y,
                transRender.localEulerAngles.z);
        if (splitCs.UScale > 1 || splitCs.VScale > 1)
        {
            sprRender.drawMode = SpriteDrawMode.Tiled;
            sprRender.size = new Vector2(sprRender.size.x * splitCs.UScale, sprRender.size.y * splitCs.VScale);
        }
        transRender.localScale = new Vector3(transRender.localScale.x * splitCs.Scale.x, transRender.localScale.y * splitCs.Scale.y, transRender.localScale.z);
    }

    public void BackupCurPrefab(string assetPath = null)
    {
        if (assetPath == null)
            assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(assetPath))
            return;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        GameObject prefabBackup = Instantiate(prefab);
        string backupPath = assetPath.Substring(0, assetPath.LastIndexOf('.')) + "_Backup.prefab";
        PrefabUtility.SaveAsPrefabAsset(prefabBackup, backupPath);
        DestroyImmediate(prefabBackup);
    }
}
#endif