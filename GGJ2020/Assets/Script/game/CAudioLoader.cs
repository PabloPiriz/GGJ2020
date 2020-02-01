using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;
using System.IO;

public class CAudioLoader : MonoBehaviour
{
    public static CAudioLoader Inst
    {
        get
        {
            if (_inst == null)
            {
                return Instantiate<GameObject>(
                        Resources.Load<GameObject>("AudioLoader"))
                        .GetComponent<CAudioLoader>();
            }
            else
                return _inst;
        }
    }
    private List<CAudio> mAudios;

    private static CAudioLoader _inst;

    void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _inst = this;
        DontDestroyOnLoad(this.gameObject);

        mAudios = new List<CAudio>();

        //loadAllTexts();
    }

    public void loadAllTexts()
    {
        Debug.Log(Directory.GetCurrentDirectory() + "/Resources/xml/audioText.xml");
        // try
        // {
        Debug.Log("dataPath: " + Application.dataPath + "/Resources/xml/audioText.xml");
        //XElement aDoc = XElement.Load(Application.dataPath + "/Resources/xml/audioText.xml");

        TextAsset textAsset = Resources.Load("xml/audioText") as TextAsset;
        Debug.Log(textAsset.text);
        XmlDocument aDoc = new XmlDocument();
        aDoc.LoadXml(textAsset.text);

        XmlNodeList aAudios = aDoc.SelectNodes("descendant::audio");

        Debug.Log("aAudiosCout: " + aAudios.Count);

        foreach (XmlNode item in aAudios)
        {
            string aID = item.Attributes["id"].Value;
            Debug.Log("id: " + aID);
            string aPath = item.Attributes["path"].Value;

            string aText = item.Attributes["text"].Value;

            AudioClip aClip = Resources.Load<AudioClip>(aPath);
            bool aNoise = bool.Parse(item.Attributes["isNoise"].Value);
            bool aMain = bool.Parse(item.Attributes["isMain"].Value);


            CAudio aAudio = new CAudio(aID, aClip, aText, aNoise, aMain);
            Debug.Log("adding audio with id: " + aID + " to audios");
            mAudios.Add(aAudio);
        }

        // for (int i = 0; i < aAudios.Count; i++)
        // {
        //     Debug.Log(aAudios[i].InnerText);

        //     string aID = aAudios.Attributes["id"].Value;
        //     Debug.Log("id: " + aID);
        // }
        // foreach (var audio in aAudios)
        // {
        //     try
        //     {
        //         string aID = audio.Attribute("id").Value;
        //         string aPath = audio.Attribute("path").Value;

        //         AudioClip aClip = Resources.Load<AudioClip>(aPath);
        //         bool aNoise = bool.Parse(audio.Attribute("isNoise").Value);
        //         bool aMain = bool.Parse(audio.Attribute("isMain").Value);

        //         CAudio aAudio = new CAudio(aID, aClip, aNoise, aMain);
        //         Debug.Log("adding audio with id: " + aID + " to audios");
        //         mAudios.Add(aAudio);

        //     }
        //     catch
        //     {
        //         Debug.Log("loading audio failed!");
        //     }
        // }
        // }
        // catch
        // {
        //     Debug.Log("i wasn't able to load xml");
        // }
    }

    public List<CAudio> getNoises()
    {
        List<CAudio> aAudios = new List<CAudio>();

        for (int i = 0; i < mAudios.Count; i++)
        {
            if (mAudios[i].mNoise)
            {
                aAudios.Add(mAudios[i]);
            }
        }

        return aAudios;
    }

    public List<CAudio> getBaseAudios()
    {
        List<CAudio> aAudios = new List<CAudio>();

        for (int i = 0; i < mAudios.Count; i++)
        {
            if (!mAudios[i].mNoise && !mAudios[i].mMain)
            {
                aAudios.Add(mAudios[i]);
            }
        }
        return aAudios;
    }

    public List<CAudio> getMainAudios()
    {
        List<CAudio> aAudios = new List<CAudio>();

        for (int i = 0; i < mAudios.Count; i++)
        {
            if (!mAudios[i].mNoise && mAudios[i].mMain)
            {
                aAudios.Add(mAudios[i]);
            }
        }
        return aAudios;
    }
}
