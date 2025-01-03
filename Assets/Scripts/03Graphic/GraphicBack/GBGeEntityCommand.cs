using UnityEngine;
using System.Collections.Generic;
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//实体相关命令集
public enum GeEntityBackCmdType
{
    Add_Shadow = 0,
    Create_Actor,
    Create_SnapShot,
    Actor_Shadow_Visible,
    Change_Model,
    Actor_Visible,
    Actor_LowLevel,
    Destroy,
    Change_Action,
    Stop_Action,
    Pause_Animation,
    Resume_Animation,
    Preload_Animation,
    Play_Attachment_Ani,
    Change_Avatar,
    Suit_Avatar,
    MAX_COUNT
};
public partial class GeEntity
{
#if !LOGIC_SERVER
    //增加人物，实体阴影命令
    public sealed class EntityAddShadowGBCommand : IGBCommand
    {
        public GeEntity _this;
        public Vector3 scale;
        public override bool Resume()
        {
            _this.AddSimpleShadow(scale);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            scale = Vector3.zero;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Add_Shadow; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityAddShadowGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityAddShadowGBCommand; }
    }
    //创建快照命令
    public sealed class EntityCreateSnapShotGBCommand : IGBCommand
    {
        public GeEntity _this;
        public Color32 color;
        public float TimeLen;
        public long timeStamp;
        public override bool Resume()
        {
            float leftTime = TimeLen - (FrameSync.GetTicksNow() - timeStamp) / 1000.0f;
            if (timeStamp > 0.0f && leftTime <= TimeLen && leftTime > 0.0f)
            {
                _this.CreateSnapshot(color, leftTime);
            }
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            color = Color.black;
            TimeLen = 0.0f;
            timeStamp = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Create_SnapShot; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityCreateSnapShotGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityCreateSnapShotGBCommand;
        }
    }
    //播放动画命令
    public sealed class EntityChangeActionGBCommand : IGBCommand
    {
        public string strAction = string.Empty;
        public float fSpeed = 0.0f;
        public bool bLoop = false;
        public bool bReplace = false;
        public bool bForce = false;
        public long timeStamp = 0;
        public float offset = 0.0f;
        public bool isPause = false;//内部状态维护
        public bool isStop = false;//内部状态维护
        public GeEntity _this;
        public override bool Resume()
        {
            if (!isPause && !isStop)
            {
                offset += (FrameSync.GetTicksNow() - timeStamp) / 1000.0f;
            }
            if (_this.m_Avatar != null)
                _this.m_Avatar.ChangeAction(strAction, fSpeed, bLoop, offset);// do nothing  cause the original function will save the record
            else
            {
                Logger.LogErrorFormat("EntityChangeActionGBCommand avatar is null {0}", _this != null ? _this.m_EntityName : "ERROR1:" + strAction);
            }
            return true;
        }
        public override void OnRecycle()
        {
                _this = null;
                strAction = string.Empty;
                fSpeed = 0.0f;
                bLoop = false;
                bReplace = false;
                bForce = false;
                timeStamp = 0;
                offset = 0.0f;
                isPause = false;//内部状态维护
                isStop = false;//内部状态维护
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Change_Action; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityChangeActionGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityChangeActionGBCommand;
        }
    }
    //暂停动画播放命令
    public sealed class EntityPauseAniGBCommand : IGBCommand
    {
        public GeEntity _this;
        public uint mask;
        public bool hitEffectPause;
        public long timeStamp;
        public override void OnRecycle()
        {
            _this = null;
            mask = 0;
            hitEffectPause = false;
            timeStamp = 0;
        }
        public override bool Resume()
        {
            _this.Pause((int)mask, hitEffectPause);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Pause_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityPauseAniGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityPauseAniGBCommand;
        }
    }
    //人物换装信息
    public class ChangeAvatarInfo
    {
        public GeAvatarChannel channel;
        public string modulePath;
        public bool asyncLoad;
        public bool highPriority;
        public int prodId;
    };
    //换装信息池
    public class ChangeAvatarInfoPool
    {
        private static readonly GamePool.ObjectPool<ChangeAvatarInfo> m_Pool = new GamePool.ObjectPool<ChangeAvatarInfo>(null, null);

        public static ChangeAvatarInfo Get()
        {
            return m_Pool.Get();
        }

        public static void Release(ChangeAvatarInfo inst)
        {
            m_Pool.Release(inst);
        }
        public static void Clear()
        {
            m_Pool.Clear();
        }
    }
    //人物换时装命令
    public class EntityChangeAvatarGBCommand : IGBCommand
    {
        public GeEntity _this;
        public Dictionary<int, ChangeAvatarInfo> changeAvatarGBInfo = new Dictionary<int, ChangeAvatarInfo>();
        public override bool Resume()
        {
            var iter = changeAvatarGBInfo.GetEnumerator();
            while (iter.MoveNext())
            {
                var changeInfo = iter.Current.Value;
                _this.ChangeAvatar(changeInfo.channel, changeInfo.modulePath, changeInfo.asyncLoad, changeInfo.highPriority, changeInfo.prodId);
            }
            return true;
        }
        public override void OnRecycle()
        {
           
            var iter = changeAvatarGBInfo.GetEnumerator();
            while (iter.MoveNext())
            {
                var curInfo = iter.Current.Value;
                ChangeAvatarInfoPool.Release(curInfo);
            }
            changeAvatarGBInfo.Clear();
            _this = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Change_Avatar; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static ChangeAvatarInfo AllocateChangeAvatarInfo()
        {
            return ChangeAvatarInfoPool.Get();
        }
        public static EntityChangeAvatarGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityChangeAvatarGBCommand;
        }

    }
    //换装命令
    public sealed class EntitySuitAvatarGBCommand : IGBCommand
    {
        public GeEntity _this;
        public bool isAsyncLoad;
        public bool highPriority;
        public override bool Resume()
        {
            _this.SuitAvatar();
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            isAsyncLoad = false;
            highPriority = false;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Suit_Avatar; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntitySuitAvatarGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntitySuitAvatarGBCommand;
        }
    }
    //恢复动画播放命令
    public sealed class EntityResumeAniGBCommand : IGBCommand
    {
        public GeEntity _this;
        public uint mask;
        public long timeStamp;
        public override bool Resume()
        {
            _this.Resume((int)mask);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            mask = 0;
            timeStamp = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Resume_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityResumeAniGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityResumeAniGBCommand;
        }
    }
    //停止动画播放命令
    public sealed class EntityStopAniGBCommand : IGBCommand
    {
        public GeEntity _this;
        public long timeStamp;
        public override bool Resume()
        {
            _this.StopAction();
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            _this = null;
            timeStamp = 0;
        }
        public static byte CommandType { get { return (byte)GeEntityBackCmdType.Stop_Action; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
        public static EntityStopAniGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityStopAniGBCommand; }
    }
    //动画预加载命令
    public sealed class EntityPreloadAniGBCommand : IGBCommand
    {
        public GeEntity _this;
        public string aniName;
        public List<string> aniNames = null;
        public override bool Resume()
        {
            if (aniNames != null)
            {
                for (int i = 0; i < aniNames.Count; i++)
                {
                    _this.ProloadAction(aniNames[i]);
                }
            }
            return true;
        }
            public override void OnRecycle()
            {
                _this = null;
                aniName = string.Empty;
                aniNames = null;
            }
            public override byte CmdType()
            {
                return CommandType;
            }
            public override byte Catalog()
            {
                return CatalogType;
            }
            public static byte CommandType { get { return (byte)GeEntityBackCmdType.Preload_Animation; } }
            public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
            public static EntityPreloadAniGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as EntityPreloadAniGBCommand; }
        }
        //挂件播放动画命令
        public class EntityAttachmentPlayAniGBCommond : IGBCommand
        {
            public GeEntity _this;
            public string attachName;
            public float fSpeed;
            public string aniName;
            public override bool Resume()
            {
                _this.PlayAttachmentAnimation(attachName,aniName, fSpeed);
                return true;
            }
            public override void OnRecycle()
            {
                _this = null;
                aniName = string.Empty;
                attachName = string.Empty;
                fSpeed = 0.0f;
            }
            public override byte Catalog()
            {
                return CatalogType;
            }
            public override byte CmdType()
            {
                return CommandType;
            }
            public static byte CommandType { get { return (byte)GeEntityBackCmdType.Play_Attachment_Ani; } }
            public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
            public static EntityAttachmentPlayAniGBCommond Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType,CommandType) as EntityAttachmentPlayAniGBCommond; }

        }
#endif
}

