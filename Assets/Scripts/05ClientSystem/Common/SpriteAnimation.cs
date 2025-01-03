using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.IO;
using System;

[ExecuteAlways][RequireComponent(typeof(Image))]
public class SpriteAnimation : MonoBehaviour {

    private Image ImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;
    private List<Sprite> SpriteFrames = new List<Sprite>();
    private bool bLoadSucceeded;
    private Vector3 nativeSize = Vector3.zero;

    public float FPS = 5;
    public string pathPre;
    public int iCount;
    public bool IsPlaying = false;
    public bool Foward = true;
    public bool AutoPlay = false;
    public bool Loop = false;
    public RectTransform target;
    public bool bSetNativeSize = true;


    public int FrameCount
    {
        get
        {
            return SpriteFrames.Count;
        }
    }

    void Awake()
    {
        ImageSource = GetComponent<Image>();
    }

    void Start()
    {
        SpriteFrames.Clear();

        bLoadSucceeded = LoadSpritesFromFile(pathPre);

        if (AutoPlay)
        {
            Play();
        }
        else
        {
            IsPlaying = false;
        }
    }

    private void SetSprite(int idx)
    {
        ImageSource.sprite = SpriteFrames[idx];
        if(bSetNativeSize)
        {
            ImageSource.SetNativeSize();
        }
    }

    bool LoadSpritesFromFile(string path)
    {
        SpriteFrames.Clear();

        if (path == null || iCount <= 0)
        {
            return false;
        }

        StringBuilder stringFormat = StringBuilderCache.Acquire();
        for (int i = 0; i < iCount; ++i)
        {
            stringFormat.Clear();
            stringFormat.AppendFormat("{0}{1:00000}", path,i);
            var sprite = AssetLoader.instance.LoadRes(stringFormat.ToString(),typeof(Sprite)).obj as Sprite;
            if (sprite != null)
            {
                SpriteFrames.Add(sprite);
            }
        }
        StringBuilderCache.Release(stringFormat);

        if (SpriteFrames.Count <= 0)
        {
            return false;
        }

        return true;
    }

    public void Play()
    {
        IsPlaying = true;
        Foward = true;
    }

    public void PlayReverse()
    {
        IsPlaying = true;
        Foward = false;
    }

    void Update()
    {
        if (!IsPlaying || 0 == FrameCount || !bLoadSucceeded)
        {
            return;
        }

        mDelta += Time.deltaTime;
        if (mDelta > 1 / FPS)
        {
            mDelta = 0;
            if (Foward)
            {
                mCurFrame++;
            }
            else
            {
                mCurFrame--;
            }

            if (mCurFrame >= FrameCount)
            {
                if (Loop)
                {
                    mCurFrame = 0;
                }
                else
                {
                    IsPlaying = false;
                    return;
                }
            }
            else if (mCurFrame < 0)
            {
                if (Loop)
                {
                    mCurFrame = FrameCount - 1;
                }
                else
                {
                    IsPlaying = false;
                    return;
                }
            }

            SetSprite(mCurFrame);

            if (bLoadSucceeded && target != null)
            {
                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(target.transform);
                Vector3 sourceSize = ImageSource.sprite.bounds.size;
                Vector3 scale = new Vector3(bounds.size.x / (sourceSize.x)* 0.010f, bounds.size.y / (sourceSize.y) * 0.010f, 1.0f);
                transform.localScale = scale;
            }
        }
    }

    public void Pause()
    {
        IsPlaying = false;
    }

    public void Resume()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
        }
    }

    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        IsPlaying = false;
    }

    public void Rewind()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        Play();
    }
}

