using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CMenuOption : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{

    protected CMenu _menu;
    protected Coroutine _animateRoutine;

    public void SetMenuRef(CMenu menu)
    {
        _menu = menu;
    }

    public bool menuButtonEnabled;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            return;
        }
        Debug.Log("pointer down");
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            return;
        }
        Debug.Log("pointer up");
    }

    public virtual void SetSelected()
    {

    }

    public virtual void SetDeselected()
    {

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _menu.OnOptionMouseEnter(this);
    }

    public virtual void OnAccept()
    {
        
    }

    public virtual void AnimateIn(Vector3 offset, float time, float delay)
    {

    }

    public virtual bool IsAnimating()
    {
        return _animateRoutine != null;
    }

}
