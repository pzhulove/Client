using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using System.Collections;

class AudioFxManager : MonoSingleton<AudioFxManager>
{
    protected AudioSource soundManger = null;
    public float Volume
    {
        get { return (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume; }
        set
        {
            GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume = value;
            if (soundManger != null)
            {
                soundManger.volume = value;
            }
        }
    }
    public bool Mute
    {
        get { return GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute; }
        set
        {
            GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute = value;
            if(soundManger != null)
            {
                soundManger.mute = value;
            }
        }
    }

    public override void Init()
    {
        GameObject tmpGo = Utility.FindGameObject("Environment/AudioManager/Sfx");
        if (tmpGo)
            soundManger = tmpGo.GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volumeScale, bool shot, float pitch, bool loop = false)
    {
        if (soundManger != null)
        {
            soundManger.pitch = pitch; //音速 pitch
            soundManger.loop = loop;
            soundManger.PlayOneShot(clip, volumeScale);
        }
    }

    public void Stop()
    {
        if (soundManger != null)
        {
            soundManger.Stop();
        }
    }

    IEnumerator AysncPlaySound(UnityEngine.Object go, string path = null, float volumeScale = 1.0f, bool shot = true, float pitch = 1.0f, bool bloop = false)
    {
        var nepath = CFileManager.EraseExtension(path);
        //var rt = Resources.LoadAsync(nepath);
        var rt = AssetLoader.instance.LoadRes(nepath);
        yield return rt;
        //go = rt.asset;
        go = rt.obj;
        if (go!= null)
            PlaySound((AudioClip)go, volumeScale, shot, pitch, bloop);
    }

    public void PlaySFX(UnityEngine.Object go, string path = null, float volumeScale = 1.0f, bool shot = true, float pitch = 1.0f, bool bloop = false)
    {
        if (go == null && path == null)
            return;

        if (path != null)
        {
            //go = AssetLoader.instance.LoadRes(path).obj as GameObject;
            StartCoroutine(AysncPlaySound(null, path, volumeScale, shot, pitch, bloop));
        }

        if(go != null)
        PlaySound((AudioClip)go, volumeScale, shot, pitch, bloop);
    }
}

