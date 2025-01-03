using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ClipInfo
{
    public string ClipName;
    public Texture2D ShadowTexture;
    public int FrameCounts;

    public ClipInfo(string clipName, Texture2D texture, int frameCounts)
    {
        ClipName = clipName;
        ShadowTexture = texture;
        FrameCounts = frameCounts;
    }
}

public class Clips
{
    public string ActName;
    public List<ClipInfo> clips=new List<ClipInfo>();

    public Clips(string actName, List<ClipInfo> clipInfos)
    {
        ActName = actName;
        clips = clipInfos;
    }
}

[System.Serializable]
public class ActorInfo
{
    public string ActorName;
    public string ClipName;
    public Texture2D ShadowTexture;
    public int FrameCounts;

    public ActorInfo(string actorName, string clipName, Texture2D texture, int frameCounts)
    {
        ActorName = actorName;
        ClipName = clipName;
        ShadowTexture = texture;
        FrameCounts = frameCounts;
    }
}
public class FramesLookTable : ScriptableObject
{
    private static FramesLookTable _instance;
    public static FramesLookTable Instance {
        get
        {
            if (!_instance)
            { 
                _instance = Resources.Load<FramesLookTable>("Shadow/FramesLookTable");
            }
            return _instance;
        }
    }
    public ActorInfo[] _ActorInfos;
    
    private void _SetUpDicionary(Dictionary<string, Clips> m_QueryDictionary)
    {
        if (m_QueryDictionary.Count == 0 && _ActorInfos!=null)
        {
            for (int i = 0; i < _ActorInfos.Length; i++)
            {
                var temp = _ActorInfos[i];
                if (!m_QueryDictionary.ContainsKey(temp.ActorName))
                {
                    List<ClipInfo> tempclips = new List<ClipInfo>();
                    ClipInfo tempClip = new ClipInfo(temp.ClipName,temp.ShadowTexture,temp.FrameCounts);
                    tempclips.Add(tempClip);
                    m_QueryDictionary.Add(temp.ActorName, new Clips(temp.ActorName,tempclips));
                }
                else
                {
                    var tempClips = m_QueryDictionary[temp.ActorName];
                    ClipInfo tempClip = new ClipInfo(temp.ClipName, temp.ShadowTexture, temp.FrameCounts);
                    if (!tempClips.clips.Contains(tempClip))
                    {
                        tempClips.clips.Add(tempClip);
                    }
                }
            }
        }
    }
    Dictionary<string, Clips> m_QueryDictionary;

    public void _InitShadowDic()
    {
        m_QueryDictionary = new Dictionary<string, Clips>();
        _SetUpDicionary(m_QueryDictionary);
    }
    public void _Add(string actorName,string clipName,Texture2D texture,int frameCount)
    {
        Dictionary<string, Clips> m_QueryDictionary = new Dictionary<string, Clips>();
        _SetUpDicionary(m_QueryDictionary);
        if (m_QueryDictionary.ContainsKey(actorName))
        {
            var temp = m_QueryDictionary[actorName];
            List<ClipInfo> listClips = new List<ClipInfo>();
            foreach (var v in temp.clips)
            {
                if (v.ClipName == clipName)
                {
                    v.ShadowTexture = texture;
                    v.FrameCounts = frameCount;
                    return;
                }
            }
            listClips.AddRange(temp.clips);
            listClips.Add(new ClipInfo(clipName,texture,frameCount));
            m_QueryDictionary[actorName].clips = listClips;
        }
        else
        {
            ClipInfo clip =new ClipInfo(clipName,texture,frameCount);
            ClipInfo[] clips = new ClipInfo[1];
            clips[0] = clip;
            m_QueryDictionary.Add(actorName,new Clips(actorName,clips.ToList()));
        }
        _DeInitDictionary(m_QueryDictionary);
    }

    private void _DeInitDictionary(Dictionary<string, Clips> m_QueryDictionary)
    {
        var tempActorInfos = m_QueryDictionary.Values.ToList();
        List<ActorInfo> tempActor =new List<ActorInfo>();
        for (int i = 0; i < tempActorInfos.Count; i++)
        {
            var temp = tempActorInfos[i];
            for (int j = 0; j < temp.clips.Count; j++)
            {
                var tempClip = temp.clips[j];
                ActorInfo act = new ActorInfo(temp.ActName, tempClip.ClipName,tempClip.ShadowTexture,tempClip.FrameCounts);
                tempActor.Add(act);
            }
            
        }
        _ActorInfos = tempActor.ToArray();
    }

    public Clips _GetActorInfo(string actorName)
    {
        if(m_QueryDictionary==null)
            _InitShadowDic();
        if (m_QueryDictionary.ContainsKey(actorName))
        {
            return m_QueryDictionary[actorName];
        }
        return null;
    }
}
