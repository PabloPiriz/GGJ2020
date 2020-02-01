using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAudio
{
    public string mId;
    public AudioClip mClip;
    public string mText;
    public bool mNoise;
    public bool mMain;

    public CAudio(string aID, AudioClip aClip, string aText, bool isNoise, bool isMain)
    {
        mId = aID;
        mClip = aClip;
        mText = aText;
        mNoise = isNoise;
        mMain = isMain;
    }
}
