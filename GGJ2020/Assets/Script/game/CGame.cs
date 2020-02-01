using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGame : MonoBehaviour
{
    private List<Vector3> mAudioSources;
    private Dictionary<Vector3, float> mAudioVolume;
    private List<Vector3> mNoiseSources;
    private Dictionary<Vector3, float> mNoiseVolume;
    private float mAudioRadius = 15;
    private float mNoiseRadius = 45;

    public CGyroscopeTesting _gyroscope;

    private int mAmountOfAudios = 5;
    private int mAmountOfNoises = 4;
    // Start is called before the first frame update
    void Start()
    {
        mAudioSources = new List<Vector3>();
        mAudioVolume = new Dictionary<Vector3, float>();
        mNoiseSources = new List<Vector3>();
        mNoiseVolume = new Dictionary<Vector3, float>();


        generateAudioSources();
        generateNoiseSources();

        CTransitionManager.Inst.SetFadeOutFlag();
    }

    // Update is called once per frame
    void Update()
    {
        updateNoiseFading();
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
                mAudioVolume.Add(aVector, 1 / mAmountOfAudios);
                attempts = 0;
            }

            if (attempts >= 5)
            {
                Debug.Log("too much!!");
                break;
            }
        }
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
    }

    private void updateNoiseFading()
    {
        if (_gyroscope != null)
        {
            Vector3 aCurrentDir = _gyroscope.getCurrentFacing();

            List<float> aDistances = new List<float>();

            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                float aDistance = Mathf.Abs(Vector3.Angle(mNoiseSources[i], aCurrentDir));

                aDistances.Add(aDistance);
            }

            float aTotal = 0;

            for (int i = 0; i < aDistances.Count; i++)
            {
                aTotal += aDistances[i];
            }

            for (int i = 0; i < mNoiseSources.Count; i++)
            {
                mNoiseVolume[mNoiseSources[i]] = aDistances[i] / aTotal;
                //Debug.Log("noise#" + (i + 1) + " has " + mNoiseVolume[mNoiseSources[i]] + " volume");
            }
        }
    }
}
