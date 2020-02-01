using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFadeTransition : CTransition
{
    public float _fadeInTime = .5f;
    public float _fadeOutTime = .3f;
    private CanvasGroup _group;
    public override void Awake()
    {
        //init
        //Se hace antes del base.Awake por orden
        //de ejecución del setstate
        _group = GetComponentInChildren<CanvasGroup>();
        _group.alpha = 0;

        base.Awake();
    }

    public override void SetState(int aState)
    {
        base.SetState(aState);
        if(_state == STATE_FADEIN)
        {
            _routine = StartCoroutine(FadeRoutine());
        }
    }

    protected IEnumerator FadeRoutine()
    {
        float elapsedTime = 0;

        while(elapsedTime < _fadeInTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / _fadeInTime;

            //interpolar-animar
            _group.alpha = t;

            yield return null;
        }
        SetState(STATE_HOLD);

        float holdTimeStamp = Time.time;
        while (!(_fadeOutFlag && (Time.time - holdTimeStamp) > _minHoldTime))
            yield return null;

        SetState(STATE_FADEOUT);
        elapsedTime = 0;
        while (elapsedTime < _fadeOutTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / _fadeOutTime;

            //interpolar-animar
            _group.alpha = 1 - t;

            yield return null;
        }
        _routine = null;
        SetState(STATE_DONE);
    }
}
