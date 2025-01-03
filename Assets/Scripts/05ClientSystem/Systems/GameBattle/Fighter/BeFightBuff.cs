using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class BeFightBuff
    {
        int buffID;
        ulong guid;
        ulong playerId;
        ProtoTable.ChijiBuffTable table = null;
        GeEffectEx geEffect = null;
        float m_leftTime = 0.0f;
        int m_overLay = 0;
        public int BuffID{ get{return buffID;}}
        public ulong GUID { get { return guid; } }
        public ulong PlayerGUID { get { return playerId; } }
        public ProtoTable.ChijiBuffTable Table { get{ return table; } }
        public float LeftTime { get { return m_leftTime; } }
        public BeFightBuff(int id,ulong objId,ulong playerGuid,float leftTime,int overlay)
        {
            guid = objId;
            buffID = id;
            playerId = playerGuid;
            m_leftTime = leftTime;
            m_overLay = overlay;
            table = TableManager.GetInstance().GetTableItem<ProtoTable.ChijiBuffTable>(id);
        }
        public void Refresh(float leftTime,int overlay)
        {
            m_leftTime = leftTime;
            m_overLay = overlay;
            if(geEffect != null)
            {
                geEffect.SetTimeLen(99999999);

            }
        }
        public int OverLay { get { return m_overLay; } }
        public void Update(float elapseTime)
        {
            if (m_leftTime <= 0.0f) return;
            m_leftTime -= elapseTime;
        }
        public void DeInit()
        {
            geEffect = null;
        }
        public void Start(GeActorEx master)
        {
            if(master != null && table != null)
            {
                DAssetObject asset;

                asset.m_AssetObj = null;
                asset.m_AssetPath = table.EffectName;

                EffectsFrames effectInfo = new EffectsFrames();
                effectInfo.localPosition = new Vector3(0, 0, 0);
                effectInfo.timetype = EffectTimeType.BUFF;
                if (Utility.IsStringValid(table.EffectLocateName))
                {
                    effectInfo.attachname = table.EffectLocateName;
                }
                bool forceDisplay = table.IsLowLevelShow;
                geEffect = master.CreateEffect(asset, effectInfo, 99999u, new Vec3(0, 0, 0), 1f, 1f, table.EffectLoop, forceDisplay);
            }
        }
        public void Finish(GeActorEx master)
        {
      //      Logger.LogErrorFormat("Finish id {0} guid {1} playerid {2} succeed", buffID, guid, playerId);
            if (master != null && geEffect != null)
            {
                master.DestroyEffect(geEffect);
            }
        }
    }
    public class BeFightBuffManager
    {
        List<BeFightBuff> mBuffs = new List<BeFightBuff>();
        public BeFightBuffManager()
        {
        }

        public List<BeFightBuff> GetBuffList()
        {
            return mBuffs;
        }

        public void Update(float delta)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                if (mBuffs[i] != null)
                {
                    mBuffs[i].Update(delta);
                }
            }
        }
        public BeFightBuff AddBuff(int id,ulong guid,ulong playerId,float durTime,int overlay)
        {
         ///   Logger.LogErrorFormat("Add Buff id {0} guid {1} playerid {2}", id, guid, playerId);
            for (int i = 0; i < mBuffs.Count; i++)
            {
                if(mBuffs[i].GUID == guid)
                {
                    return null;
                }
            }
            var newBuff = new BeFightBuff(id,guid, playerId, durTime, overlay);
       //     Logger.LogErrorFormat("Add Buff id {0} guid {1} playerid {2} succeed", id, guid, playerId);
            mBuffs.Add(newBuff);
            return newBuff;
        }
        public void RemoveBuff(ulong guid)
        {
         //   Logger.LogErrorFormat("Remove Buff guid {0}",guid);
            for (int i = 0; i < mBuffs.Count;i++)
            {
                if(mBuffs[i].GUID == guid)
                {
                    mBuffs.RemoveAt(i);
                    break;
                }
            }
        }
        public bool HasBuff(ulong guid)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                if (mBuffs[i].GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }
        public BeFightBuff GetBuffByGUID(ulong guid)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                if (mBuffs[i].GUID == guid)
                {
                    return mBuffs[i];
                }
            }
            return null;
        }

        public bool HasBuffByID(int buffId)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                if (mBuffs[i].BuffID == buffId)
                {
                    return true;
                }
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= mBuffs.Count)
            {
                return;
            }
            mBuffs.RemoveAt(index);
        }
        public int Count() { return mBuffs.Count; }
        public BeFightBuff Get(int index)
        {
            if(index < 0 || index >= mBuffs.Count)
            {
                return null;
            }
            return mBuffs[index];
        }
        public void Clear()
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                mBuffs[i].DeInit();
            }
            mBuffs.Clear();
        }
        public void Start(GeActorEx player)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                 mBuffs[i].Start(player);
            }
        }
        public void Finish(GeActorEx player)
        {
            for (int i = 0; i < mBuffs.Count; i++)
            {
                mBuffs[i].Finish(player);
            }
        }
    }

}
