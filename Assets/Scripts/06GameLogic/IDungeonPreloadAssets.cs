using UnityEngine;
using System.Collections;

public interface IDungeonPreloadAssets
{
    string PreloadPath();

    void Preload(string path);
}
