using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Attacking,
        Dead
    }

    State _state = State.Idle;

    Animator _ac;
    NavMeshAgent _agent;
    Citizen _target;

    int lives = 2;
    public bool IsAlive
    {
        get { return lives > 0; }
    }

    void Start()
    {
        Stage.Instance.AddZombie(this);
        _ac = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        switch(_state)
        {
            case State.Idle:
                var citizen = FindClosestCitizenInRange();
                if(citizen != null)
                {
                    _target = citizen;
                    SetState(State.Running);
                }
            break;
            case State.Running:
                var c = FindClosestCitizenInRange();
                _target = c;
                if (_target != null && _target.IsAlive)
                {
                    if((transform.position - _target.transform.position).sqrMagnitude < 4f)
                    {
                        SetState(State.Attacking);
                    }
                    else
                    {
                        _agent.SetDestination(_target.transform.position);
                    }
                }
                else
                {
                    SetState(State.Idle);
                }
                break;
            case State.Attacking:
                if(_target != null && _target.IsAlive)
                {
                    transform.LookAt(_target.transform);
                }
                break;
        }
    }

    public void Attack()
    {
        if(_target != null && _target.IsAlive)
        {
            _target.Hit(1);
        }
    }

    public void AttackEnd()
    {
        SetState(State.Running);
    }

    public void Hit(int damage)
    {
        if(--lives <= 0)
        {
            SetState(State.Dead);
        }
    }

    Citizen FindClosestCitizenInRange()
    {
        float closest = int.MaxValue;
        Citizen candidate = null;
        foreach (var citizen in Stage.Instance.Citizens)
        {
            var dist = (citizen.transform.position - transform.position).sqrMagnitude;
            if (citizen.IsAlive && dist < closest)
            {
                closest = dist;
                candidate = citizen;
            }
        }
        return candidate;
    }

    void SetState(State state)
    {
        if(state != _state)
        {
            _state = state;
            switch(_state)
            {
                case State.Running:
                    _agent.isStopped = false;
                    _ac.CrossFade("Run", 0.1f);
                    break;
                case State.Idle:
                    _agent.isStopped = true;
                    _ac.CrossFade("Idle", 0.1f);
                    break;
                case State.Attacking:
                    _agent.isStopped = true;
                    _ac.CrossFade("Attack", 0.1f);
                    break;
                case State.Dead:
                    Stage.Instance.RemoveZombie(this);
                    _agent.isStopped = true;
                    _ac.CrossFade("Die", 0.1f);
                    Destroy(gameObject, 2f);
                    break;
            }
        }
    }
}
