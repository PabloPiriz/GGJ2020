using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTransition : MonoBehaviour
{
    public static int STATE_FADEIN = 0;
    public static int STATE_HOLD = 1;
    public static int STATE_FADEOUT = 2;
    public static int STATE_DONE = 3;

    public bool _isDefault = false;
    public float _minHoldTime = .5f;
    protected int _state;
    protected Coroutine _routine;
    protected bool _fadeOutFlag = false;

    public virtual void Awake()
    {
        SetState(STATE_FADEIN);
    }

    public bool GetFadeOutFlag()
    {
        return _fadeOutFlag;
    }

    public void SetFadeOutFlag(bool flag)
    {
        _fadeOutFlag = flag;
    }

    public virtual void SetState(int aState)
    {
        _state = aState;
        
        if (_state == STATE_DONE)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual int GetState()
    {
        return _state;
    }

    public virtual bool IsDone()
    {
        return _state == STATE_DONE;
    }
}
