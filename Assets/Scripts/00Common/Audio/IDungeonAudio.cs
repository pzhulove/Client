using UnityEngine;
using System.Collections;

public interface IDungeonAudio 
{
    bool PushBgm(string path, string envPath = null);

    void PopBgm();

    void ClearBgm();
}
