using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public System.Action OnEnemyAttack;

    [SerializeField] private Animator _animator;
    [SerializeField] private State _startingState;
    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimeMax = 2f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private int _damageAmount = 5;
    [SerializeField] private float _attackCooldownMax = 1f;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 1.5f;
    [SerializeField] private float _attackingDistance = 1.5f;
    [SerializeField] private float _attackSlowMultiplier = 0.5f;
    [SerializeField] private float _stopDistance = 0.8f;

    private State _state;
    private float _roamingTime;
    private Vector3 _roamPosition;
    private float _attackCooldown;
    [SerializeField] private Transform _Player;
    private Rigidbody2D _rb;
    private EnemyEntity _enemyEntity;
    private bool _isDead = false;

    private enum State
    {
        Death,
        Attacking,
        Chasing,
        Idle,
        Roaming
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyEntity = GetComponent<EnemyEntity>();

        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 0;
            _rb.freezeRotation = true;
        }
    }

    private void Start()
    {
        _state = _startingState;
        _roamingTime = _roamingTimeMax;
        _roamPosition = GetRoamPosition();
        _attackCooldown = 0f;
        FindPlayer();

        if (_enemyEntity != null)
        {
            _enemyEntity.OnDeath += EnemyEntity_OnDeath;
        }
    }

    public void SetDeathState()
    {
        EnemyEntity_OnDeath(null, System.EventArgs.Empty);
    }

    private void EnemyEntity_OnDeath(object sender, System.EventArgs e)
    {
        _isDead = true;
        _state = State.Death;
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;
            _animator.SetTrigger("IsDie");
        }
    }

    private void OnDestroy()
    {
        if (_enemyEntity != null)
        {
            _enemyEntity.OnDeath -= EnemyEntity_OnDeath;
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _Player = player.transform;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        transform.rotation = Quaternion.identity;

        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }

        if (_Player == null)
        {
            FindPlayer();
            return;
        }

        StateHandler();
    }

    private void StateHandler()
    {
        if (_isDead) return;

        switch (_state)
        {
            case State.Roaming:
                MoveToPosition(_roamPosition);
                _roamingTime -= Time.deltaTime;
                if (_roamingTime < 0)
                {
                    _roamPosition = GetRoamPosition();
                    _roamingTime = _roamingTimeMax;
                }
                break;

            case State.Chasing:
                MoveToPlayer();
                break;

            case State.Attacking:
                StopAndAttack();
                TryAttack();
                break;
        }

        _state = GetCurrentState();
    }

    private void StopAndAttack()
    {
        _rb.linearVelocity = Vector2.zero;

        if (_Player != null)
        {
            FlipUpdate(_Player.position);
        }
    }

    private void MoveToPlayer()
    {
        if (_Player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _Player.position);

        if (distanceToPlayer <= _stopDistance)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = _moveSpeed * _chasingSpeedMultiplier;
        Vector2 direction = (_Player.position - transform.position).normalized;
        _rb.linearVelocity = direction * currentSpeed;

        FlipUpdate(_Player.position);
    }

    private void TryAttack()
    {

        if (_Player == null || _attackCooldown > 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _Player.position);

        if (distanceToPlayer <= _attackingDistance)
        {
            _attackCooldown = _attackCooldownMax;
            OnEnemyAttack?.Invoke();

            var playerScript = _Player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(this.transform, _damageAmount);
            }
        }
    }

    private State GetCurrentState()
    {
        if (_isDead || _state == State.Death) return State.Death;

        if (_Player == null) return State.Roaming;

        float distanceToPlayer = Vector3.Distance(transform.position, _Player.position);

        if (distanceToPlayer <= _attackingDistance) return State.Attacking;
        if (distanceToPlayer <= _chasingDistance) return State.Chasing;

        return State.Roaming;
    }

    private void MoveToPosition(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);

        if (distance < 0.2f)
        {
            _roamPosition = GetRoamPosition();
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (target - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;

        FlipUpdate(target);
    }

    private void FlipUpdate(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(direction.x),
                Mathf.Abs(transform.localScale.y),
                Mathf.Abs(transform.localScale.z)
            );
        }
    }

    private Vector3 GetRoamPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere.normalized;
        randomDir.z = 0;
        return transform.position + randomDir * Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chasingDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackingDistance);
    }

    public bool IsRunning()
    {
        return !_isDead && _rb.linearVelocity.magnitude > 0.1f;
    }

    public float GetRoamingAnimationSpeed()
    {
        return (_state == State.Chasing) ? _chasingSpeedMultiplier : 1f;
    }
}