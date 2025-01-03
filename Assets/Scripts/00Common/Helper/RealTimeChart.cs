
using System;
using UnityEngine;

public class RealTimeChart : MonoSingleton<RealTimeChart>
{
    public bool bVisible;
    private System.Random RandomSupport = new System.Random();
    private ListView<ChartTrack> Tracks = new ListView<ChartTrack>();

    public void AddSample(string InTag, float InValue)
    {
        ChartTrack track = this.FindTrack(InTag);
        if (track != null)
        {
            track.AddSample(InValue);
        }
        else
        {
            object[] inParameters = new object[] { InTag };
            DebugHelper.Assert(false, "no valid track with tag {0}", inParameters);
        }
    }

    public ChartTrack AddTrack(string InTag, Color InColor)
    {
        ChartTrack item = this.FindTrack(InTag);
        if (item == null)
        {
            item = new ChartTrack(InTag, InColor);
            this.Tracks.Add(item);
            return item;
        }
        item.drawColor = InColor;
        return item;
    }

    public ChartTrack AddTrack(string InTag, Color InColor, bool bFixedRange, float InMin, float InMax)
    {
        ChartTrack item = this.FindTrack(InTag);
        if (item == null)
        {
            item = new ChartTrack(InTag, InColor, InMin, InMax);
            this.Tracks.Add(item);
        }
        else
        {
            item.drawColor = InColor;
        }
        item.SetFixedRange(bFixedRange, InMin, InMax);
        return item;
    }

    protected void DrawBase()
    {
        ChartTrack.DrawLine(new Vector2(0f, 0f), new Vector2(0f, Screen.height * (1f - (ChartTrack.Clip * 2f))), Color.white, 2f);
        ChartTrack.DrawLine(new Vector2(0f, 0f), new Vector2(Screen.width * (1f - (ChartTrack.Clip * 2f)), 0f), Color.white, 2f);
    }

    protected void DrawTracks()
    {
        float maxValue = float.MaxValue;
        float minValue = float.MinValue;
        bool flag = false;
        for (int i = 0; i < this.Tracks.Count; i++)
        {
            if (this.Tracks[i].hasSamples)
            {
                flag = true;
                float fixedMinSampleValue = 0f;
                float fixedMaxSampleValue = 100f;
                if (!this.Tracks[i].isFixedRange)
                {
                    fixedMinSampleValue = this.Tracks[i].minValue;
                    fixedMaxSampleValue = this.Tracks[i].maxValue;
                }
                else
                {
                    fixedMinSampleValue = this.Tracks[i].fixedMinSampleValue;
                    fixedMaxSampleValue = this.Tracks[i].fixedMaxSampleValue;
                }
                if (fixedMinSampleValue < maxValue)
                {
                    maxValue = fixedMinSampleValue;
                }
                if (fixedMaxSampleValue > minValue)
                {
                    minValue = fixedMaxSampleValue;
                }
            }
        }
        if (flag)
        {
            for (int j = 0; j < this.Tracks.Count; j++)
            {
                if (this.Tracks[j].hasSamples && this.Tracks[j].isVisiable)
                {
                    this.Tracks[j].OnRender(maxValue, minValue);
                }
            }
        }
    }

    public ChartTrack FindTrack(string InTag)
    {
        for (int i = 0; i < this.Tracks.Count; i++)
        {
            if (this.Tracks[i].tag == InTag)
            {
                return this.Tracks[i];
            }
        }
        return null;
    }

    public override void Init()
    {
    }

    private void OnGUI()
    {
        if (this.bVisible)
        {
            this.DrawBase();
            this.DrawTracks();
        }
    }

    public void RemoveTrack(string InTag)
    {
        for (int i = 0; i < this.Tracks.Count; i++)
        {
            if (this.Tracks[i].tag == InTag)
            {
                this.Tracks.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveTrack(ChartTrack InTrack)
    {
        if (InTrack != null)
        {
            this.RemoveTrack(InTrack.tag);
        }
    }

    public void SetSample(string InTag, float InValue, int reverseIndex)
    {
        ChartTrack track = this.FindTrack(InTag);
        if (track != null)
        {
            track.SetSample(InValue, reverseIndex);
        }
        else
        {
            object[] inParameters = new object[] { InTag };
            DebugHelper.Assert(false, "no valid track with tag {0}", inParameters);
        }
    }

    public bool isVisible
    {
        get
        {
            return this.bVisible;
        }
        set
        {
            this.bVisible = value;
        }
    }
}
