using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTransitionManager : MonoBehaviour
{
    public static CTransitionManager Inst
    {
        get
        {
            if (_inst == null)
            {
                return  Instantiate<GameObject>(
                        Resources.Load <GameObject>("Transitions/TransitionManager"))
                        .GetComponent<CTransitionManager>();
            }
            else
                return _inst;
        }
    }

    private static CTransitionManager _inst;
    private Dictionary<string, GameObject> _transitionAssets;

    private CTransition _activeTransition;
    private GameObject _defaultTransition;
    private Canvas _canvas;

    void Awake()
    {
        if(_inst != null && _inst != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _inst = this;
        DontDestroyOnLoad(this.gameObject);

        //Init
        _canvas = GetComponentInChildren<Canvas>();
        _transitionAssets = new Dictionary<string, GameObject>();
        CTransition[] transitionObjects = Resources.LoadAll<CTransition>("Transitions/");

        for(int i = 0; i < transitionObjects.Length;i++)
        {
            CTransition tr = transitionObjects[i];
            if (_transitionAssets.ContainsKey(tr.name))
                continue;

            _transitionAssets.Add(tr.name, tr.gameObject);
            if (tr._isDefault)
                _defaultTransition = tr.gameObject;
        }
    }

    //crear transicion
    public void CreateTransition(string name = "")
    {
        if (HasTransition())
            return;

        GameObject transitionObj = _defaultTransition;
        if(_transitionAssets.ContainsKey(name))
        {
            transitionObj = _transitionAssets[name];
        }

        GameObject newObj = Instantiate<GameObject>(transitionObj);
        newObj.transform.SetParent(_canvas.transform);
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localRotation = Quaternion.identity;
        newObj.transform.localScale = Vector3.one;

        _activeTransition = newObj.GetComponent<CTransition>();
    }

    //checkear si tengo transicion
    public bool HasTransition()
    {
        return _activeTransition != null;
    }

    //set fadeout
    public void SetFadeOutFlag()
    {
        if (_activeTransition != null)
            _activeTransition.SetFadeOutFlag(true);
    }

    //esta pantalla cubierta?
    public bool IsScreenCovered()
    {
        return HasTransition() && _activeTransition.GetState() == CTransition.STATE_HOLD;
    }
}
