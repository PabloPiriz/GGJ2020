using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CMenuButton : CMenuOption, IPointerUpHandler, IPointerDownHandler
{

    Image _img;

    [SerializeField]
    private System.Action _callback;

    private Color _selectedColor;
    private Color _deselectedColor;

    private Text _label;

    private bool innactive = false;



    public static CMenuButton Create(string name, System.Action callback, bool menuButton = false)
    {
        GameObject obj = new GameObject(name);
        CMenuButton button = obj.AddComponent<CMenuButton>();
        button.SetCallback(callback);
        button.menuButtonEnabled = menuButton;
        return button;
    }

    public void setInnactive(bool active)
    {
        innactive = active;
    }

    public void SetCallback(System.Action action)
    {
        _callback = action;
    }

    public override void OnAccept()
    {
       if (!innactive)
        {
            _callback();
        }
    }

    private void Awake()
    {
        _img = GetComponent<Image>();
        if (_img == null)
            _img = gameObject.AddComponent<Image>();
    }

    public CMenuButton SetParent(Transform tf)
    {
        transform.SetParent(tf);
        transform.localScale = Vector3.one;

        return this;
    }

    public CMenuButton SetSize(Vector2 size)
    {
        (transform as RectTransform).sizeDelta = size;
        return this;
    }

    public CMenuButton SetPosition(Vector3 pos)
    {
        (transform as RectTransform).anchoredPosition3D = pos;
        return this;
    }

    public CMenuButton SetSprite(Sprite sp)
    {
        _img.sprite = sp;
        return this;
    }

    public CMenuButton SetColors(Color selected, Color deselected)
    {
        _selectedColor = selected;
        _deselectedColor = deselected;
        return this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (innactive)
        {
            return;
        }
        if (eventData.dragging)
        {
            return;
        }

        _menu.AcceptOption(this);
        //Debug.Log("pointer down");

        if (!menuButtonEnabled)
        {
            _img.color = _selectedColor;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (innactive)
        {
            return;
        }

        if (eventData.dragging)
        {
            return;
        }

        //_img.color = _deselectedColor;
        //Debug.Log("pointer up");

        
    }

    public override void SetSelected()
    {
        if (innactive)
            return;
        _img.color = _selectedColor;
    }

    public override void SetDeselected()
    {
        if (innactive)
            return;
        _img.color = _deselectedColor;
    }
    /*
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _menu.OnOptionMouseEnter(this);
    }*/

    public CMenuButton SetText(string text)
    {
        CheckText();
        _label.text = text;


        return this;
    }
    private void CheckText()
    {
        if (_label != null)
            return;
        GameObject obj = new GameObject("text");
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;


        _label = obj.AddComponent<Text>();
        _label.horizontalOverflow = HorizontalWrapMode.Overflow;
        _label.verticalOverflow = VerticalWrapMode.Overflow;
        _label.color = Color.white;
        _label.alignment = TextAnchor.MiddleCenter;
        _label.rectTransform.sizeDelta = _img.rectTransform.sizeDelta;
    }

    public CMenuButton SetFont(Font font)
    {
        CheckText();
        _label.font = font;
        return this;
    }

    public CMenuButton SetFontSize(int size)
    {
        CheckText();
        _label.fontSize = size;
        return this;
    }

    public CMenuButton SetTextAlignment(TextAnchor anchor)
    {
        CheckText();
        _label.alignment = anchor;
        return this;
    }

    public CMenuButton SetTextColor(Color color)
    {
        CheckText();
        _label.color = color;
        return this;
    }

    public CMenuButton SetTextOverflow(HorizontalWrapMode hMode, VerticalWrapMode vMode)
    {
        CheckText();
        _label.horizontalOverflow = hMode;
        _label.verticalOverflow = vMode;
        return this;
    }

    public Image getImage()
    {
        return _img;
    }

    public CMenuButton SetAnchor(Vector2 anchor)
    {
        (transform as RectTransform).anchorMin = anchor;
        (transform as RectTransform).anchorMax = anchor;
        return this;
    }

    public override void AnimateIn(Vector3 offset, float time, float delay)
    {
        if (IsAnimating())
        {
            StopCoroutine(_animateRoutine);
        }

        //setup inicial
        if (_img != null)
            _img.color = new Color(_img.color.r, _img.color.b, _img.color.g, 0);
        if (_label != null)
            _label.color = new Color(_label.color.r, _label.color.b, _label.color.g, 0);

        //empezamos a animar
        _animateRoutine = StartCoroutine(FadeAnimateIn(offset, time, delay));
    }

    private IEnumerator FadeAnimateIn(Vector3 offset, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0;
        Vector3 endPos = (transform as RectTransform).anchoredPosition3D;
        Vector3 startPos = endPos + offset;
        while (elapsed < time)
        {
            //calcular tiempo
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            t = Mathfx.Hermite(0, 1, t); //cambio la curva de animacion de lineal a ...

            //actualizar elementos
            if (_img != null)
                _img.color = new Color(_img.color.r, _img.color.b, _img.color.g, t);
            if (_label != null)
                _label.color = new Color(_label.color.r, _label.color.b, _label.color.g, t);

            (transform as RectTransform).anchoredPosition3D = Vector3.Lerp(
                startPos, endPos, t);

            //esperar 1 frame
            yield return null;
        }
        _animateRoutine = null;
    }

    //fontsize
    //alignment
    //color
    //overflow

}
