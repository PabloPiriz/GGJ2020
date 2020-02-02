using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGame : MonoBehaviour
{
    private List<Vector3> mAudioSources;
    private List<Vector3> mNoiseSources;
    private Dictionary<Vector3, float> mNoiseVolume;
    private float mAudioRadius = 15;
    private float mNoiseRadius = 45;
    private int mCurrentAudio;

    public CGyroscopeTesting _gyroscope;
    public Animator _background;

    private int mAmountOfAudios = 3;
    private int mAmountOfNoises = 2;

    private int mState;

    private const int STATE_PLAYING = 0;
    private const int STATE_SELECTING = 1;
    private const int STATE_ENDING = 2;
    // Start is called before the first frame update
    void Start()
    {
        mAudioSources = new List<Vector3>();
        mNoiseSources = new List<Vector3>();
        mNoiseVolume = new Dictionary<Vector3, float>();


        generateAudioSources();
        generateNoiseSources();

        CTransitionManager.Inst.SetFadeOutFlag();

        _background.SetBool("isActive", false);

        setState(STATE_PLAYING);
    }

    public void setState(int aState)
    {
        mState = aState;

        if (mState == STATE_PLAYING)
        {

        }
        else if (mState == STATE_SELECTING)
        {

        }
        else if (mState == STATE_ENDING)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mState == STATE_PLAYING)
        {
            updateNoiseFading();
            updateAudioFading();


        }
        else if (mState == STATE_SELECTING)
        {

        }
        else if (mState == STATE_ENDING)
        {

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (mAudioSources != null)
        {
            for (int i = 0; i < mAudioSources.Count; i++)
            {
                Gizmos.DrawRay(_gyroscope.transform.position, mAudioSources[i]);
            }
        }

        Gizmos.color = Color.yellow;

        if (mNoiseSources != null)
        {
            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                Gizmos.DrawRay(_gyroscope.transform.position, mNoiseSources[i]);
            }
        }
    }

    private void generateAudioSources()
    {
        int attempts = 0;
        while (mAudioSources.Count < mAmountOfAudios)
        {
            attempts += 1;
            Debug.Log("attempts made: " + attempts + " for audio#" + (mAudioSources.Count + 1));

            Vector3 pos = Random.insideUnitSphere;
            Vector3 aVector = (pos - _gyroscope.transform.position).normalized;

            bool tooClose = false;
            for (int i = 0; i < mAudioSources.Count; i++)
            {
                float aDist = Vector3.Angle(mAudioSources[i], aVector);

                Debug.Log("distance between audio#" + (i + 1) + ": " + aDist);
                if (aDist < mAudioRadius)
                {
                    Debug.Log("too close!");
                    tooClose = true;
                }
            }

            if (!tooClose)
            {
                mAudioSources.Add(aVector);
                attempts = 0;
            }

            if (attempts >= 5)
            {
                Debug.Log("too much!!");
                break;
            }
        }
        CAudioManager.Inst.LoadVoices(mAmountOfAudios);
    }

    private void generateNoiseSources()
    {
        int attempts = 0;
        while (mNoiseSources.Count < mAmountOfNoises)
        {
            attempts += 1;
            Debug.Log("attempts made: " + attempts + " for noise#" + (mNoiseSources.Count + 1));

            Vector3 pos = Random.insideUnitSphere;
            Vector3 aVector = (pos - _gyroscope.transform.position).normalized;

            bool tooClose = false;
            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                float aDist = Mathf.Abs(Vector3.Angle(mNoiseSources[i], aVector));

                Debug.Log("distance between noise#" + (i + 1) + ": " + aDist);
                if (aDist < mNoiseRadius)
                {
                    Debug.Log("too close!");
                    tooClose = true;
                }
            }

            if (!tooClose)
            {
                mNoiseSources.Add(aVector);
                mNoiseVolume.Add(aVector, 1 / mAmountOfNoises);
                attempts = 0;
            }

            if (attempts >= 5)
            {
                Debug.Log("too much!!");
                break;
            }
        }
        CAudioManager.Inst.LoadNoises(mAmountOfNoises);
    }

    private void updateNoiseFading()
    {
        if (_gyroscope != null)
        {
            Vector3 aCurrentDir = _gyroscope.getCurrentFacing();

            float aMinDistance = 4000;

            List<float> aDistances = new List<float>();

            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                float aDistance = Mathf.Abs(Vector3.Angle(mNoiseSources[i], aCurrentDir));

                aDistances.Add(aDistance);

                if (aMinDistance > aDistance)
                {
                    aMinDistance = aDistance;
                }
            }

            Debug.Log("aMinDistance for noise: " + aMinDistance);

            float aTotal = 0;

            for (int i = 0; i < aDistances.Count; i++)
            {
                aTotal += aDistances[i];
            }

            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                mNoiseVolume[mNoiseSources[i]] = 1 - (aDistances[i] / aTotal);
                CAudioManager.Inst.UpdateNoiseVolume(i, mNoiseVolume[mNoiseSources[i]]);
                //Debug.Log("noise#" + (i + 1) + " has " + mNoiseVolume[mNoiseSources[i]] + " volume");
            }
        }
    }

    public void updateAudioFading()
    {
        if (_gyroscope != null)
        {
            Vector3 aCurrentDir = _gyroscope.getCurrentFacing();

            //if we currently have an audio playing
            if (mCurrentAudio != -1)
            {
                if (mCurrentAudio >= 0 && mCurrentAudio < mAudioSources.Count)
                {
                    float aDistance = Mathf.Abs(Vector3.Angle(mAudioSources[mCurrentAudio], aCurrentDir));
                    //but we are no longer within its range
                    if (aDistance > mAudioRadius)
                    {
                        //set mCurrentAudio volume to 0 at AudioManager
                        CAudioManager.Inst.UpdateVoiceVolume(0);

                        //we reset mCurrentAudio to -1
                        setCurrentAudio(-1);

                        _background.SetBool("isActive", false);
                        Debug.Log("+++ NOT ACTIVE!!");
                    }
                    else
                    {
                        setAudioVolume(aDistance);

                    }
                }
            }

            if (mCurrentAudio == -1)
            {

                float aMinDistance = 40000;
                for (int i = 0; i < mAudioSources.Count; i++)
                {
                    float aDistance = Mathf.Abs(Vector3.Angle(mAudioSources[i], aCurrentDir));

                    if (aDistance < aMinDistance)
                    {
                        aMinDistance = aDistance;
                    }

                    if (aDistance < mAudioRadius)
                    {
                        setCurrentAudio(i);
                        setAudioVolume(aDistance);

                        Debug.Log("+++ ACTIVE!!");

                        _background.SetBool("isActive", true);
                    }
                }

                Debug.Log("aMinDistance for sound: " + aMinDistance);
            }


        }
    }

    public void setCurrentAudio(int aAudioIndex)
    {
        mCurrentAudio = aAudioIndex;

        // set 
        CAudioManager.Inst.SetVoice(mCurrentAudio);
    }

    public void setAudioVolume(float aDistance)
    {
        float aVolume = 1 - (aDistance / mAudioRadius);

        //set audio volume to aVolume at AudioManage
        CAudioManager.Inst.UpdateVoiceVolume(aVolume);
    }
}
