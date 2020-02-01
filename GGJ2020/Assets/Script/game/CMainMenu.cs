using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMainMenu : MonoBehaviour
{

    public Sprite _miniSprite;
    /*public Sprite _backSprite;
    public Sprite _avatarSprite;
    public Sprite _confirmSprite;*/

    public Vector3 _menuPosition;
    public float _menuOffset = 30;
    public float _buttonSize = 80;

    public Vector3 _animOffset = Vector3.up * -20;
    public float _animTime = .5f;
    public float _animDelay = .1f;

    private int _state;
    private CMenu _menu;

    public CMenuButton play;


    void Awake()
    {
        _menu = new CMenu();

        play.SetCallback(OnMiniPressed);
        play.SetColors(Color.white, Color.white);

        _menu.AddOption(play);


        //_menu.RecalculatePosition(transform.position, _menuOffset);
        _menu.AnimateIn(_animOffset, _animTime, _animDelay);
        //SetState(STATE_MINI);
    }
    /*
    public void SetState(int aState)
    {
        _state = aState;
        if(_state == STATE_MINI)
        {
            CreateMiniMenu();
        }
        else if(_state == STATE_OPTIONS)
        {
            CreateOptionsMenu();
        }
    }*/

    private void OnMiniPressed()
    {
        Debug.Log("Play!");

        StartCoroutine(transToPlay());





        //CSceneManager.Inst.LoadScene("Game");
        //SetState(STATE_OPTIONS);
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

    }
}
