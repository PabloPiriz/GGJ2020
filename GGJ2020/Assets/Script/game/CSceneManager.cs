using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSceneManager : MonoBehaviour
{
    public static CSceneManager Inst
    {
        get
        {
            if (_inst == null)
            {
                return Instantiate<GameObject>(
                        Resources.Load<GameObject>("SceneManager"))
                        .GetComponent<CSceneManager>();
            }
            else
                return _inst;
        }
    }
    private static CSceneManager _inst;

    void Awake()
    {
        if (_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _inst = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private bool haveLooped = false;

    public void LoadScene(string name)
    {
        if (name == "Main Menu")
        {
            haveLooped = true;
        }
        SceneManager.LoadSceneAsync(name);
    }

    public void LoadSceneAdditive(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }

    public bool haveILooped()
    {
        return haveLooped;
    }
}
