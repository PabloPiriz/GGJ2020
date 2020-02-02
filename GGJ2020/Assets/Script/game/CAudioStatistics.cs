using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CAudioStatistics
{
    public CAudio mAudio { get; set; }
    public bool hasGrannyPlayed { get; set; }
    private List<CAudioRange> mPlayedSegments;
    

    public CAudioStatistics()
    {
        mPlayedSegments = new List<CAudioRange>();
        hasGrannyPlayed = false;
    }

    public void AddSegment(float secondStart, float secondStop)
    {
        Debug.Assert(secondStart <= secondStop);
        if (secondStart == secondStop)
            return;
        mPlayedSegments.Add(new CAudioRange { min = secondStart, max = secondStop });
    }

    public float GetPlayedTime()
    {
        if (mPlayedSegments.Count == 0)
            return 0;
        mPlayedSegments.OrderByDescending(n => n.min);
        float aCurrentMax = mPlayedSegments[0].min;
        float aTime = 0;
        foreach (var range in mPlayedSegments)
        {
            if (aCurrentMax < range.max)
            {
                aTime += (range.max - aCurrentMax);
                aCurrentMax = range.max;
            }
        }
        return aTime;
    }

    public float GetPercentage()
    {
        if (mPlayedSegments.Count == 0)
            return 0;

        return GetPlayedTime() / mAudio.mClip.length;
    }

    public class CAudioRange
    {
        public float min { get; set; }
        public float max { get; set; }
    }
}
