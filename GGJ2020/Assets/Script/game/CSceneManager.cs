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
            return _inst;
        }
    }
    private static CSceneManager _inst;

    void Awake()
    {
        if(_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _inst = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void LoadSceneAdditive(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }
}
