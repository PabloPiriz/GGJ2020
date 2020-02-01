using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainMenu : MonoBehaviour
{


    void Awake()
    {
        CAudioLoader.Inst.loadAllTexts();
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
