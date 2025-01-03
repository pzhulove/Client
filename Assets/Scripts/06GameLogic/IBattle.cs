using UnityEngine;
using System.Collections;
using GameClient;

public enum eDungeonMode
{
    None,
    /// <summary>
    /// 单机测试本地数据
    /// </summary>
    Test,
    /// <summary>
    /// 本地执行帧数据
    /// </summary>
    LocalFrame,
    /// <summary>
    /// 服务端同步帧数据
    /// </summary>
    SyncFrame,

	/// <summary>
	/// 录像模式
	/// </summary>
	RecordFrame,
}

public interface IBattle
{
    eDungeonMode GetMode();

    BattleType GetBattleType();

    void End(bool needEndRecord = true);

    IEnumerator Start(IASyncOperation op);

    BeScene Restart();

    void Update(int delta);

    void FrameUpdate(int delta);
    bool CanReborn();
    bool IsRebornCountLimit();  //是否副本有总复活数量限制

    int GetLeftRebornCount();  //剩余总复活数量
    int GetMaxRebornCount();    //总复活数量
}


