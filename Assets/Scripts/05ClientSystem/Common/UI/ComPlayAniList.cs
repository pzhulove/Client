using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComPlayAniList : MonoBehaviour
{
    [System.Serializable]
    private struct DOTweenAnimations
    {
        public List<DOTweenAnimation> AniList;
    }

    [SerializeField] private List<DOTweenAnimations> mAniListList;

    public void PlayAnis(int index, bool value = true)
    {
        if (mAniListList == null)
        {
            return;
        }

        if (mAniListList.Count <= index || index < 0)
        {
            return;
        }

        if (mAniListList[index].AniList == null)
        {
            return;
        }

        Utility.PlayAniList(mAniListList[index].AniList, value);
    }

    public void AddComplete(int index, UnityAction complete)
    {
        if (mAniListList == null)
        {
            return;
        }

        if (mAniListList.Count <= index || index < 0)
        {
            return;
        }

        if (mAniListList[index].AniList == null)
        {
            return;
        }

        Utility.AddComplete(mAniListList[index].AniList, complete);
    }

    public DOTweenAnimation GetMostDurableAni(int index)
    {
        if (mAniListList == null)
        {
            return null;
        }

        if (mAniListList.Count <= index || index < 0)
        {
            return null;
        }

        if (mAniListList[index].AniList == null)
        {
            return null;
        }

        return Utility.GetMostDurableAni(mAniListList[index].AniList);
    }

    public void FinishAnis(int index)
    {
        if (mAniListList == null)
        {
            return;
        }

        if (mAniListList.Count <= index || index < 0)
        {
            return;
        }

        if (mAniListList[index].AniList == null)
        {
            return;
        }

        Utility.FinishAnis(mAniListList[index].AniList);
    }

    public void SetAnisId(int index)
    {
        if (mAniListList == null)
        {
            return;
        }

        foreach (var aniListItem in mAniListList)
        {
            if (aniListItem.AniList == null)
            {
                continue;
            }

            foreach (var aniItem in aniListItem.AniList)
            {
                if (aniItem == null)
                {
                    continue;
                }

                aniItem.id = aniItem.id + index.ToString();
                aniItem.CreateTween();
            }
        }
    }
}
