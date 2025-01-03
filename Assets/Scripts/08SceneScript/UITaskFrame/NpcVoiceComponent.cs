using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using GameClient;

public class NpcVoiceComponent : MonoBehaviour
{
    Int32 iNpcID;
    ProtoTable.NpcTable npcItem;

    public enum SoundEffectType
    {
        SET_INVALID = -1,
        SET_Random = 0,
        SET_Start,
        SET_End,
        SET_COUNT,
    }
    SoundEffectType eSoundEffectType;
    IList<string>[] akSoundEffectsArray = new IList<string>[(int)SoundEffectType.SET_COUNT] {null,null,null};
    static float fLastPlayTime;
    static float fMaxVoiceDistance = 6.5f;
    float fInterval;
    uint audioHandle = uint.MaxValue;

    public void Initialize(Int32 iNpcID)
    {
        this.iNpcID = iNpcID;
        fInterval = Mathf.Infinity;
        npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
        if(npcItem != null)
        {
            akSoundEffectsArray[(int)SoundEffectType.SET_Random] = npcItem.SEStand;
            akSoundEffectsArray[(int)SoundEffectType.SET_Start] = npcItem.SEStart;
            akSoundEffectsArray[(int)SoundEffectType.SET_End] = npcItem.SEEnd;
            fInterval = npcItem.SEPeriod;
        }
        this.eSoundEffectType = SoundEffectType.SET_INVALID;
    }

    public void PlaySound(SoundEffectType eSoundEffectType)
    {
        this.eSoundEffectType = eSoundEffectType;
        if (npcItem != null && eSoundEffectType != SoundEffectType.SET_INVALID)
        {
            if (eSoundEffectType >= SoundEffectType.SET_Random && eSoundEffectType <= SoundEffectType.SET_End)
            {
                var soundlist = akSoundEffectsArray[(int)eSoundEffectType];
                if (soundlist != null && soundlist.Count > 0)
                {
                    Int32 iRandomIdx = UnityEngine.Random.Range(0, soundlist.Count - 1);
                    //if(NpcVoiceCachedManager.GetInstance().PlaySound("Sound/NPCSound/" + soundlist[iRandomIdx]))
                    //{
                    //    fLastPlayTime = Time.time;
                    //}


                    if (uint.MaxValue != audioHandle)
                        AudioManager.instance.Stop(audioHandle);

                    audioHandle = uint.MaxValue;

                    if (soundlist.Count > 0 && !string.IsNullOrEmpty(soundlist[iRandomIdx]))
                    {
						audioHandle = AudioManager.instance.PlaySound("Sound/Voice/" + soundlist[iRandomIdx], AudioType.AudioEffect, 1);
                    }
                     
                    if (uint.MaxValue != audioHandle)
                    {
                        fLastPlayTime = Time.time;
                    }

                }
            }
        }
        if(eSoundEffectType == SoundEffectType.SET_End)
        {
            eSoundEffectType = SoundEffectType.SET_Random;
        }
    }

    public void Tick(float fdelta)
    {
        if(npcItem != null && eSoundEffectType == SoundEffectType.SET_Random)
        {
            ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(current != null)
            {
                var townNpc = current.GetTownNpcByNpcId(iNpcID);
                if(townNpc != null)
                {
                    BeTownNPCData npcData = townNpc.ActorData as BeTownNPCData;
                    if(npcData != null)
                    {
                        Vector3 kDistance = current.MainPlayer.ActorData.MoveData.Position - npcData.MoveData.Position;
                        kDistance.y = 0.0f;
                        float fDistance = Mathf.Sqrt(kDistance.sqrMagnitude);
                        if (fDistance > fMaxVoiceDistance)
                        {
                            return;
                        }
                    }
                }
            }

            if (fLastPlayTime + fInterval < Time.time)
            {
                float fRandom = UnityEngine.Random.Range(0.0f, 1.0f);
                if(fRandom <= npcItem.Probability)
                {
                    PlaySound(this.eSoundEffectType);
                }
            }
        }
    }
}