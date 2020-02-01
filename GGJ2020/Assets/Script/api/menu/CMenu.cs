using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMenu
{
    protected List<CMenuOption> _options;

    protected int _index = -1;
    protected int _previousIndex;



    public CMenu()
    {
        _options = new List<CMenuOption>();

    }

    public virtual void Update()
    {
        /*
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool down = Input.GetKeyDown(KeyCode.DownArrow);

        if (up)
        {
            _previousIndex = _index;
            _index -= 1;
            if (_index < 0)
            {
                _index = _options.Count - 1;
            }

            if (_previousIndex != _index)
                UpdateSelected();

            //Debug.Log(_index);
        }
        else if (down)
        {
            _previousIndex = _index;
            _index += 1;
            if (_index > _options.Count - 1)
            {
                _index = 0;
            }

            if (_previousIndex != _index)
                UpdateSelected();

            //Debug.Log(_index);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            AcceptOption();
        }*/
    }
    public void AcceptOption(CMenuOption option = null)
    {
        if (option == null)
        {
            if (option.menuButtonEnabled)
            {
                option.OnAccept();
            }
            else
            {
                _options[_index].OnAccept();
            }
        }
        else
        {
            if (option.menuButtonEnabled)
            {
                option.OnAccept();
            }
            else
            {
                int index = +_options.IndexOf(option);
                SetSelectedIndex(index);
                _options[_index].OnAccept();
            }
        }

    }

    private void UpdateSelected()
    {
        if (_previousIndex != -1)
            _options[_previousIndex].SetDeselected();
        _options[_index].SetSelected();
    }

    public void AddOption(CMenuOption aOption)
    {
        aOption.SetMenuRef(this);

        _options.Add(aOption);
    }

    public void disableAllButtons()
    {
        for (int i = 0; i < _options.Count; i++)
        {
            _options[i].gameObject.SetActive(false);
        }
    }
    public void enableAllButtons()
    {
        for (int i = 0; i < _options.Count; i++)
        {
            _options[i].gameObject.SetActive(true);
        }
    }

    public virtual void RecalculatePosition(Vector3 pos, float sep)
    {
        for (int i = 0; i < _options.Count; i++)
        {
            (_options[i].transform as RectTransform).anchoredPosition3D =
                pos + Vector3.up * i * -sep;
        }
    }

    public void SetSelectedIndex(int _in, bool forceAll = false)
    {
        _previousIndex = _index;
        _index = _in;

        if (forceAll)
        {
            for (int i = 0; i < _options.Count; i++)
            {
                if (_in == i)
                {
                    _options[i].SetSelected();
                }
                else
                {
                    _options[i].SetDeselected();
                }
            }
        }
        else
        {
            UpdateSelected();
        }
    }

    public void OnOptionMouseEnter(CMenuOption option)
    {
        int nextIndex = _options.IndexOf(option);
        if (nextIndex == _index)
            return;
        SetSelectedIndex(nextIndex);
    }

    public int GetCurrentIndex()
    {
        return _index;
    }

    public CMenuOption GetCurrentOption()
    {
        return _options[_index];
    }

    public List<CMenuOption> getOptions()
    {
        return _options;
    }

    public void Clear()
    {
        for (int i = _options.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(_options[i].gameObject);
            _options.RemoveAt(i);
        }
    }

    public void AnimateIn(Vector3 offset, float time, float delay, bool backToFront = false)
    {
        if (backToFront)
        {
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                _options[i].AnimateIn(offset, time, (_options.Count - 1 - i) * delay);
            }
        }
        else
        {
            for (int i = 0; i < _options.Count; i++)
            {
                _options[i].AnimateIn(offset, time, i * delay);
            }
        }
    }


}
