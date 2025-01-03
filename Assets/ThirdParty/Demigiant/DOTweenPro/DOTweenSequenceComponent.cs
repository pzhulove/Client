using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using System;

namespace DG.Tweening
{

    [Serializable]
    public class DOTweenSequenceComponentList
    {
        public List<DOTweenSequence> sequenceList = new List<DOTweenSequence>();
    }

    [Serializable]
    public class DOTweenSequenceComponentDic : SerializableDictionary<int, DOTweenSequenceComponentList>
    {

    }

    [AddComponentMenu("DOTween/DOTween SequenceComponent")]
    public class DOTweenSequenceComponent : MonoBehaviour
    {
        public DOTweenSequenceComponentDic sequenceDic = new DOTweenSequenceComponentDic();
        public List<bool> listFold = new List<bool>();
        public bool autoPlay = true;
        public bool autoKill = true;
        public string id = "";
        private Sequence mySequence;
        private void Awake()
        {
            CreateTween();
        }
        public void CreateTween()
        {
            mySequence = DOTween.Sequence();
            var its = sequenceDic.GetEnumerator();
            while (its.MoveNext())
            {
                var values = its.Current.Value;
                for (int i = 0; i < values.sequenceList.Count; i++)
                {
                    DOTweenSequence se = values.sequenceList[i];
                    Tween t = se.CreateSequenceTween();
                    if (i == 0)
                    {
                        mySequence.Append(t);
                    }
                    else
                    {
                        mySequence.Join(t);
                    }
                }
            }
            if (!string.IsNullOrEmpty(id)) mySequence.SetId(id);
            if (autoPlay)
            {
                mySequence.Play();
            }
            else
            {
                mySequence.Pause();
            }
            mySequence.SetAutoKill(autoKill);
        }
        public void DoPlayByID(string id = "")
        {
            if (mySequence != null && id == this.id)
            {
                mySequence.Play();
            }
        }
        public void DOPauseByID(string id = "")
        {
            if (mySequence != null && id == this.id)
            {
                mySequence.Pause();
            }
        }
        public void DORewindByID(string id = "")
        {
            if (mySequence != null && id == this.id)
            {
                mySequence.Rewind();
            }
        }
        void OnDestroy()
        {
            if (mySequence != null && mySequence.IsActive()) mySequence.Kill();
            mySequence = null;
        }
    }
}