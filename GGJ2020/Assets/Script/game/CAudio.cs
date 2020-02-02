using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAudio
{
    public string mId;
    public AudioClip mClip;
    public string mText;
    public bool mNoise;
    public int mPuntaje;

    public CAudio(string aID, AudioClip aClip, string aText, bool isNoise, int aPuntaje)
    {
        mId = aID;
        mClip = aClip;
        mText = aText;
        mNoise = isNoise;
        mPuntaje = aPuntaje;
    }
}
