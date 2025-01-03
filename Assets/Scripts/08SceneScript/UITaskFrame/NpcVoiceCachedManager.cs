using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameClient
{
    class NpcVoiceCachedManager : Singleton<NpcVoiceCachedManager>
    {
        protected AudioSource audioSource = null;
        protected GameObject audioManager = null;
        protected float fMaxSoundDistance = 7.0f;
        protected float fVolume = 1.0f;
        public float Volume
        {
            get { return fVolume; }
            set
            {
                fVolume = value;
            }
        }
        protected bool bInit = false;

        public void SetMute(bool bMute)
        {
            audioSource.mute = bMute;
        }

        public void SetVolume(float fVolume)
        {
            this.fVolume = fVolume;
        }

        public bool PlaySound(string path,float volume = 1.0f)
        {
            if (audioManager == null)
            {
                audioManager = GameObject.Find("Environment").transform.Find("AudioManager").Find("NpcSfx").gameObject;
                if (audioManager != null)
                {
                    audioSource = audioManager.GetComponent<AudioSource>();
                }
            }

            if(audioSource == null)
            {
                return false;
            }
            audioSource.volume = this.fVolume * volume;

            if (!(ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown))
            {
                return false;
            }

            ClientSystemTown current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(current == null || current.MainPlayer == null)
            {
                return false;
            }

            //Vector3 kDistance = current.MainPlayer.ActorData.MoveData.Position - position;
            //if(Mathf.Sqrt(kDistance.sqrMagnitude) >= fMaxSoundDistance)
            //{
            //    return false;
            //}

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            AudioClip audioClip = AssetLoader.instance.LoadRes(path,typeof(AudioClip)).obj as AudioClip;
            if(audioClip == null)
            {
                return false;
            }
            audioSource.PlayOneShot(audioClip, volume);

            return true;
        }

    }
}