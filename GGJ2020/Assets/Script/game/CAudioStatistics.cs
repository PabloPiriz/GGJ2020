using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CAudioStatistics
{
    public CAudio mAudio { get; set; }
    private List<CAudioRange> mPlayedSegments;

    public CAudioStatistics()
    {
        mPlayedSegments = new List<CAudioRange>();
    }
    
    public void AddSegment(float secondStart, float secondStop)
    {
        Debug.Assert(secondStart <= secondStop);
        if (secondStart == secondStop)
            return;
        mPlayedSegments.Add(new CAudioRange { min = secondStart, max = secondStop });
    }

    public float GetPercentage()
    {
        if (mPlayedSegments.Count == 0)
            return 0;

        mPlayedSegments.OrderByDescending(n => n.min);
        float aClipDuration = mAudio.mClip.length;
        float aCurrentMax = mPlayedSegments[0].min;
        float aPercentage = 0;
        foreach (var range in mPlayedSegments)
        {
            if (aCurrentMax < range.max)
            {
                aPercentage += (range.max - aCurrentMax) / aClipDuration;
                aCurrentMax = range.max;
            }
        }
        return aPercentage;
    }

    public class CAudioRange
    {
        public float min { get; set; }
        public float max { get; set; }
    }
}
