using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum State
    {
        Idle,
        Running,
        Aiming,
        Attacking,
        Dead
    }
    [SerializeField]
    GameObject _pistol = null;
    [SerializeField]
    GameObject _rifle = null;
    [SerializeField]
    Joystick _input = null;
    [SerializeField]
    Animator _ac = null;
    [SerializeField]
    ParticleSystem _particles = null;
    [SerializeField]
    LineRenderer _line = null;
    [SerializeField]
    Transform _arrow = null;
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    float _range = 15f;

    State _state;
    Enemy _target;
    AudioSource _audio;
    float _attackCooldown = 0f;

    private void Start()
    {
        _ac = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>(); 
        _attackCooldown = 0f;
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
        var target = FindClosestEnemy(false);
        _arrow.gameObject.SetActive(target != null);
        if(target != null)
        {
            _arrow.LookAt(target.transform.position + new Vector3(0f, 1.5f, 0f));
        }
        _attackCooldown -= Time.deltaTime;
        if(_pistol.activeSelf && Stage.Instance.Enemies.Count > 15)
        {
            _pistol.SetActive(false);
            _rifle.SetActive(true);
        }
    }

    public void Shoot()
    {
        _attackCooldown = _rifle.activeSelf ? 0.5f : 1f;
        _audio.Play();
        _particles.Play();
        if(_target != null)
        {
            _target.Hit(1); 
            _line.enabled = true;
            var target = _target.transform.position;
            target.y = 1f;
            _line.SetPosition(0, _particles.transform.position);
            _line.SetPosition(1, target);
            StopAllCoroutines();
            StartCoroutine(RecoilCoroutine());
        }
    }

    IEnumerator RecoilCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Recoil();
    }

    void Recoil()
    {
        _line.enabled = false;
    }

    bool IsInRange(Enemy enemy)
    {
        var dist = (enemy.transform.position - transform.position).sqrMagnitude;
        return dist < _range * _range;
    }

    Enemy FindClosestEnemy(bool InRange = true)
    {
        float closest = int.MaxValue;
        Enemy candidate = null;
        float sqRange = _range * _range;
        foreach (var enemy in Stage.Instance.Enemies)
        {
            var dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (enemy.IsAlive && dist < closest && (!InRange || dist < sqRange))
            {
                closest = dist;
                candidate = enemy;
            }
        }
        return candidate;
    }

    IEnumerator StartAttackCoroutine()
    {
        SetState(State.Aiming);
        if(_attackCooldown > 0f)
        {
            yield return new WaitForSeconds(_attackCooldown);
        }
        SetState(State.Attacking);
    }

    void UpdateState()
    {
        switch (_state)
        {
            case State.Running:
                SetState(State.Idle);
                break;
            case State.Idle:
                Enemy candidate = FindClosestEnemy();
                if(candidate != null)
                {
                    _target = candidate;
                    transform.LookAt(_target.transform);
                    StartCoroutine(StartAttackCoroutine());
                    Recoil();
                }
                break;
            case State.Attacking:
                if(_target == null || !_target.IsAlive || !IsInRange(_target))
                {
                    SetState(State.Idle);
                }
                else
                {
                    transform.LookAt(_target.transform);
                }
                break;
            case State.Dead:
                break;
            case State.Aiming:
                if(_target != null)
                {
                    transform.LookAt(_target.transform);
                }
                break;
        }
    }

    void SetState(State state)
    {
        if(state != _state)
        {
            _ac.speed = 1f;
            _state = state;
            switch(_state)
            {
                case State.Running:
                    StopAllCoroutines();
                    _ac.CrossFade("Run", 0.1f);
                    break;
                case State.Idle:
                    _ac.CrossFade("Idle", 0.1f);
                    break;
                case State.Attacking:
                    if(_rifle.activeSelf)
                    {
                        _ac.speed = 2f;
                    }
                    _ac.CrossFade("Attack", 0.1f);
                    break;
                case State.Dead:
                    _ac.CrossFade("Die", 0.1f);
                    break;
                case State.Aiming:
                    break;
            }
        }
    }
}
