using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Dead
    }

    public bool IsAlive
    {
        get
        {
            return _health > 0;
        }
    }

    [SerializeField]
    GameObject _zombie = null;

    int _health = 2;
    State _state = State.Idle;
    NavMeshAgent _agent;
    Animator _ac;
    int _objectives = 3;
    Vector3 _target = Vector3.zero;

    float _nextAudio = 5f;

    List<AudioSource> _audios = new List<AudioSource>();

    public void Hit(int damage)
    {
        _audios[Random.Range(0, _audios.Count)].Play();
        if (--_health <= 0)
        {
            SetState(State.Dead);
        }
    }

    void GetNextObjective()
    {
        _target = Stage.Instance.Objectives[Random.Range(0, Stage.Instance.Objectives.Count)].position;
    }

    void Start()
    {
        Stage.Instance.AddCitizen(this);
        GetNextObjective();
        _ac = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(_target);
        SetState(State.Running);
        _audios.Insert(0, GetComponentInChildren<AudioSource>());
        _nextAudio = Random.Range(1f, 90f);
    }

    void Update()
    {
        if(_state == State.Running)
        {
            if((_target - transform.position).sqrMagnitude < 1f)
            {
                _objectives -= 1;
                if(_objectives > 0)
                {
                    GetNextObjective();
                    _agent.SetDestination(_target);
                }
                else if(_objectives == 0)
                {
                    _target = Stage.Instance.Cars[Random.Range(0, Stage.Instance.Cars.Count)].position;
                    _agent.SetDestination(_target);
                }
                else
                {
                    SetState(State.Idle);
                    _ac.CrossFade("Idle", 0.1f);
                    Destroy(gameObject, 0.5f);
                    Stage.Instance.RemoveCitizen(this);
                    Stage.Instance.RescueCitizen();
                }
            }
        }
        _nextAudio -= Time.deltaTime;
        if(_nextAudio < 0f)
        {
            _audios[Random.Range(0, _audios.Count)].Play();
            _nextAudio = Random.Range(1f, 90f);
        }
    }

    void SetState(State state)
    {
        if(_state != state)
        {
            _state = state;
            switch(_state)
            {
                case State.Running:
                    _ac.CrossFade("Run", 0.1f);
                break;

                case State.Dead:
                    Stage.Instance.RemoveCitizen(this);
                    _ac.CrossFade("Die", 0.1f);
                    Instantiate(_zombie, transform.position, transform.rotation);
                    Destroy(gameObject);
                break;
            }
        }
    }
}
