using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Shooting,
        Dead
    }

    [SerializeField]
    Joystick _input;
    [SerializeField]
    Animator _ac;
    [SerializeField]
    ParticleSystem _particles;
    [SerializeField]
    LineRenderer _line;
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    float _range = 15f;

    State _state;
    Enemy _target;
    AudioSource _audio;

    private void Start()
    {
        _ac = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(_input.Direction.sqrMagnitude > 0f)
        {
            var direction = transform.forward;
            transform.position += direction * Time.deltaTime * _speed;
            transform.LookAt(transform.position + (new Vector3(_input.Direction.x, 0f, _input.Direction.y)));
            SetState(State.Running);
        }
        else
        {
            UpdateState();
        }
    }

    public void Shoot()
    {
        _audio.Play();
        _particles.Play();
        if(_target != null)
        {
            _target.Hit(1); 
            _line.enabled = true;
            var target = _target.transform.position/* - _particles.transform.position*/;
            target.y = 1f;
            _line.SetPosition(0, _particles.transform.position);
            _line.SetPosition(1, target);
            StopAllCoroutines();
            StartCoroutine(Recoil());
        }
    }

    IEnumerator Recoil()
    {
        yield return new WaitForSeconds(0.1f);
        _line.enabled = false;
    }

    bool IsInRange(Enemy enemy)
    {
        var dist = (enemy.transform.position - transform.position).sqrMagnitude;
        return dist < _range * _range;
    }

    Enemy FindClosestEnemyInRange()
    {
        float closest = int.MaxValue;
        Enemy candidate = null;
        float sqRange = _range * _range;
        foreach (var enemy in Enemy.Enemies)
        {
            var dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (enemy.IsAlive && dist < closest && dist < sqRange)
            {
                closest = dist;
                candidate = enemy;
            }
        }
        return candidate;
    }

    void UpdateState()
    {
        switch (_state)
        {
            case State.Running:
                SetState(State.Idle);
                break;
            case State.Idle:
                Enemy candidate = FindClosestEnemyInRange();
                if(candidate != null)
                {
                    _target = candidate;
                    transform.LookAt(_target.transform);
                    SetState(State.Shooting);
                }
                break;
            case State.Shooting:
                if(_target == null || !_target.IsAlive || !IsInRange(_target))
                {
                    SetState(State.Idle);
                }
                break;
            case State.Dead:
                break;
        }
    }

    void SetState(State state)
    {
        if(state != _state)
        {
            _state = state;
            switch(_state)
            {
                case State.Running:
                    _ac.CrossFade("RunRanged", 0.1f);
                    break;
                case State.Idle:
                    _ac.CrossFade("IdleRanged", 0.1f);
                    break;
                case State.Shooting:
                    _ac.CrossFade("AttackRanged", 0.1f);
                    break;
                case State.Dead:
                    _ac.CrossFade("Die", 0.1f);
                    break;
            }
        }
    }
}
