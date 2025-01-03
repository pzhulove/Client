using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

public class VoiceManager : Singleton<VoiceManager>
{
    //private Dictionary<int, object> mVoiceTableCache = null;
    private List<List<VoiceTable>> mCacheVoices = new List<List<VoiceTable>>();

    class VoiceNode
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string path = "";

        /// <summary>
        /// 概率，单个
        /// </summary>
        public int    rate = 0;

        /// <summary>
        /// 权重
        /// </summary>
        public int    weight = 0;
    }

    public override void Init()
    {
        Dictionary<int, object> voicetable = TableManager.instance.GetTable<VoiceTable>();
        foreach (var item in voicetable)
        {
            VoiceTable tb = item.Value as VoiceTable;

            if (null != tb)
            {
                List<VoiceTable> find = _getVoiceListByType(tb.VoiceType);

                if (null == find)
                {
                    find = new List<VoiceTable>();
                    mCacheVoices.Add(find);
                }

                if (null != find)
                {
                    find.Add(tb);
                }
            }
        }
    }

    public override void UnInit()
    {
        mCacheVoices.Clear();
    }

    private List<VoiceTable> _getVoiceListByType(VoiceTable.eVoiceType type)
    {
        for (int i = 0; i < mCacheVoices.Count; ++i)
        {
            if (null != mCacheVoices[i] && mCacheVoices[i].Count > 0)
            {
                if (mCacheVoices[i][0].VoiceType == type)
                {
                    return mCacheVoices[i];
                }
            }
        }

        return null;
    }

    private List<VoiceNode> _getAllVoiceRes(VoiceTable.eVoiceType type, int id)
    {
        List<VoiceNode> ans = new List<VoiceNode>();

        List<VoiceTable> vt = _getVoiceListByType(type);
        if (null != vt)
        {
            for (int i = 0; i < vt.Count; ++i)
            {
                for (int j = 0; j < vt[i].VoiceUnitID.Count; j++)
                {
                    if (vt[i].VoiceUnitID[j] == id)
                    {
                        VoiceNode node = new VoiceNode();
                        node.path = vt[i].VoicePath;
                        node.rate = vt[i].VoiceRate;
                        node.weight = vt[i].VoiceWeight;

                        ans.Add(node);
                        break;
                    }
                }
            }
        }
        return ans;
    }

    private List<VoiceNode> _getAllVoiceRes(VoiceTable.eVoiceType type, string tag)
    {
        List<VoiceNode> ans = new List<VoiceNode>();
        List<VoiceTable> vt = _getVoiceListByType(type);

        if (null != vt)
        {
            for (int i = 0; i < vt.Count; ++i)
            {
                if (vt[i].VoiceTag == tag)
                {
                    VoiceNode node = new VoiceNode();
                    node.path = vt[i].VoicePath;
                    node.rate = vt[i].VoiceRate;
                    node.weight = vt[i].VoiceWeight;

                    ans.Add(node);
                }
            }
        }

        return ans;
    }

    private List<uint> mCachePlayMusic = new List<uint>();
    
    private bool _playSingleVoiceSound(VoiceNode res)
    {
        StopAllVoice();

        if (null == res)
        {
            return false;
        }
       
        if (!string.IsNullOrEmpty(res.path))
        {
            int rate = UnityEngine.Random.Range(1, 1000);
            if (rate <= res.rate)
            {
                uint handle = AudioManager.instance.PlaySound(res.path, AudioType.AudioVoice, 1.0f, false, null, false, false);
                mCachePlayMusic.Add(handle);
                return true;
            }
        }

        return false;
    }

    private bool _playVoiceSound(List<VoiceNode> list)
    {
#if !LOGIC_SERVER
        if (null != list)
        {
            List<int> ws = new List<int>();
            ws.Add(0);

            int sum = 0;

            for (int i = 0; i < list.Count; ++i)
            {
                ws.Add(ws[i] + list[i].weight);
                sum += list[i].weight;
            }

            int rt = UnityEngine.Random.Range(0, sum);

            Logger.LogProcessFormat("[播放语音] {0}[{1}]", rt, sum);

            for (int i = 0; i < list.Count; ++i)
            {
                if (rt < ws[i + 1] && rt >= ws[i])
                {
                    return _playSingleVoiceSound(list[i]);
                }
            }
        }
#endif
        return false;
    }

    /// <summary>
    /// 根据 资源ID 播放语音
    /// </summary>
    /// <param name="type"> 语音类型 </param>
    /// <param name="id"> 资源ID </param>
    public void PlayVoice(VoiceTable.eVoiceType type, int id)
    {
        Logger.LogProcessFormat("[VoiceTable] 播放 {0} ResID {1}", type, id);
        _playVoiceSound(_getAllVoiceRes(type, id));
    }

    private int _getResIDByMonsterID(int id)
    {
        UnitTable unit = TableManager.instance.GetTableItem<UnitTable>(id);

        if (null != unit)
        {
            return unit.Mode;
        }

        return -1;
    }

    /// <summary>
    /// 根据 怪物ID 播放语音
    /// </summary>
    /// <param name="type"> 语音类型 </param>
    /// <param name="id"> 怪物ID </param>
    public void PlayVoiceByMonsterID(VoiceTable.eVoiceType type, int id)
    {
        int resID = _getResIDByMonsterID(id);

        Logger.LogProcessFormat("[VoiceTable] 播放 {0} 怪物ID {1}, ResID {2}", type, id, resID);
        _playVoiceSound(_getAllVoiceRes(type, resID));
    }


    /// <summary>
    /// 根据 VoiceTag 播放语音
    /// </summary>
    /// <param name="type"> 语音类型 </param>
    /// <param name="tag"> VoiceTag </param>
    public void PlayVoice(VoiceTable.eVoiceType type, string tag)
    {
        Logger.LogProcessFormat("[VoiceTable] 播放 {0} Tag {1}", type, tag);
        _playVoiceSound(_getAllVoiceRes(type, tag));
    }

    /// <summary>
    /// 根据职业播放语音
    /// </summary>
    /// <param name="type"> 语音类型 </param>
    /// <param name="occ"> 职业ID </param>
    public void PlayVoiceByOccupation(VoiceTable.eVoiceType type, int occ)
    {
        PlayVoice(type, _getUnitResIDByOccupation(occ));
    }

    public void StopAllVoice()
    {
        for (int i = 0; i < mCachePlayMusic.Count; ++i)
        {
            AudioManager.instance.Stop(mCachePlayMusic[i]);
        }
        mCachePlayMusic.Clear();
    }

    private static int _getUnitResIDByOccupation(int occupation)
    {
        JobTable jobtable = TableManager.instance.GetTableItem<JobTable>(occupation);

        if (null != jobtable)
        {
            return jobtable.Mode;
        }

        return -1;
    }
}
