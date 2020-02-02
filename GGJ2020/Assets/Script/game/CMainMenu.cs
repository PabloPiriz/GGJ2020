using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainMenu : MonoBehaviour
{

    public Animator _animator;

    public int mState;

    public const int STATE_INTRO = 0;
    public const int STATE_MAIN_MENU = 1;
    void Awake()
    {
        CAudioLoader.Inst.hello();

        setState(mState);
    }

    public void setState(int aState)
    {
        mState = aState;

        Debug.Log("mState: " + mState);
        if (mState == STATE_INTRO)
        {

        }
        else if (mState == STATE_MAIN_MENU)
        {
            _animator.SetBool("introEnded", true);
        }
    }

    private void startMatch()
    {
        Debug.Log("Play!");

        StartCoroutine(transToPlay());
    }

    private IEnumerator transToPlay()
    {
        CTransitionManager.Inst.CreateTransition("LoadingFade");

        yield return null;

        while (!CTransitionManager.Inst.IsScreenCovered())
        {
            yield return null;
        }

        CSceneManager.Inst.LoadScene("Game");

        yield return null;
    }

    void Update()
    {
        if (mState == STATE_INTRO)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                _animator.SetBool("introEnded", true);
                setState(STATE_MAIN_MENU);
            }
        }
        else if (mState == STATE_MAIN_MENU)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startMatch();
            }
#elif UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startMatch();
            }
        }
#endif
        }
    }

}
