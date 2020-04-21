using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UIAnimayionTypes
{
    Move,
    Scale,
    Rotate,
    Fade
}
public enum UIAnimayionMethods
{
    To,
    By
}

public class UITweener : MonoBehaviour
{
    [SerializeField] GameObject _target;

    [SerializeField] UIAnimayionTypes _animationType = UIAnimayionTypes.Move;
    [SerializeField] UIAnimayionMethods _animationMethod = UIAnimayionMethods.To;
    [SerializeField] LeanTweenType _easeType = LeanTweenType.linear;
    [SerializeField] float _duration = 0f;
    [SerializeField] float _delay = 0f;

    [SerializeField] bool _loop = false;
    [SerializeField] bool _pingPong = false;

    [SerializeField] bool _startOffset = true;
    [SerializeField] Vector3 _from = Vector3.zero;
    [SerializeField] Vector3 _to = Vector3.one;

    [SerializeField] bool _showOnEnable = true;

    [SerializeField] UnityEvent _onComplete = new UnityEvent();

    LTDescr _tweenObj;

    void OnEnable()
    {
        if(_showOnEnable)
        {
            Show();
        }
    }

    public void Show()
    {
        if(_target == null)
        {
            _target = gameObject;
        }

        switch(_animationType)
        {
            case UIAnimayionTypes.Move:
                Move();
                break;
            case UIAnimayionTypes.Scale:
                Scale();
                break;
            case UIAnimayionTypes.Rotate:
                Rotate();
                break;
            case UIAnimayionTypes.Fade:
                Fade();
                break;
        }

        _tweenObj.setDelay(_delay);
        _tweenObj.setEase(_easeType);

        if(_loop)
        {
            _tweenObj.setLoopCount(-1);
        }
        if(_pingPong)
        {
            _tweenObj.setLoopPingPong();
        }
        _tweenObj.setOnComplete(() => _onComplete?.Invoke());
    }

    void Fade()
    {
        var cg = _target.GetComponent<CanvasGroup>() ?? _target.AddComponent<CanvasGroup>();
        if (_startOffset)
        {
            cg.alpha = _from.x;
        }
        var to = _to;
        if(_animationMethod == UIAnimayionMethods.By)
        {
            to.x += cg.alpha;
        }
        _tweenObj = LeanTween.alphaCanvas(cg, to.x, _duration);
    }

    void Move()
    {
        var rt = _target.GetComponent<RectTransform>();
        if(rt != null)
        {
            MoveRt(rt);
            return;
        }
        if (_startOffset)
        {
            _target.transform.localPosition = _from;
        }
        var to = _to;
        if (_animationMethod == UIAnimayionMethods.By)
        {
            to += _target.transform.localPosition;
        }
        _tweenObj = LeanTween.move(_target, to, _duration);
    }

    void MoveRt(RectTransform rt)
    {
        if (_startOffset)
        {
            rt.anchoredPosition = _from;
        }
        var to = _to;
        if (_animationMethod == UIAnimayionMethods.By)
        {
            to += new Vector3(rt.anchoredPosition.x, rt.anchoredPosition.y, 0f);
        }
        _tweenObj = LeanTween.move(rt, to, _duration);
    }

    void Scale()
    {
        var rt = _target.GetComponent<RectTransform>();
        if (_startOffset)
        {
            rt.localScale = _from;
        }
        var to = _to;
        if (_animationMethod == UIAnimayionMethods.By)
        {
            to += rt.localScale;
        }
        _tweenObj = LeanTween.scale(rt, to, _duration);
    }

    void Rotate()
    {
        var rt = _target.GetComponent<RectTransform>();
        if (_startOffset)
        {
            rt.localRotation = Quaternion.Euler(_from);
        }
        var to = _to;
        if (_animationMethod == UIAnimayionMethods.By)
        {
            to.z += rt.localRotation.z;
        }
        _tweenObj = LeanTween.rotateAroundLocal(rt.gameObject, Vector3.forward, to.z, _duration);
    }
}
