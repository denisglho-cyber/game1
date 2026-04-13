using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class RedVisual : MonoBehaviour
{
    public event EventHandler OnTakeHit;

    [SerializeField] private EnemyAi _enemyAi;
    [SerializeField] private EnemyEntity _enemyEntity;
    [SerializeField] private GameObject _enemyShadow;

    private Animator _animator;
    private bool _isAttacking = false;
    private bool _hasSpeedMultiplier;
    private bool _hasAttackTrigger;

    private const string ISDIE = "IsDie";
    private const string IS_RUNNING = "IsRunning";
    private const string TAKEHIT = "TakeHit";
    private const string ATTACK_TRIGGER = "Attack";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _hasSpeedMultiplier = HasParameter(CHASING_SPEED_MULTIPLIER);
        _hasAttackTrigger = HasParameter(ATTACK_TRIGGER);
    }

    private void Start()
    {
        if (_enemyAi != null)
        {
            _enemyAi.OnEnemyAttack += EnemyAi_OnEnemyAttack;
        }

        if (_enemyEntity != null)
        {
            _enemyEntity.OntakeHit += _enemyEntity_OnTakeHit;
            _enemyEntity.OnDeath += _enemyEntity_OnDeath;
        }
    }

    private void _enemyEntity_OnDeath(object sender, EventArgs e)
    {
        _animator.SetBool(ISDIE, true);
        _spriteRenderer.sortingOrder = -1;
        _enemyShadow.SetActive(false);
    }

    private void _enemyEntity_OnTakeHit(object sender, EventArgs e)
    {
        _animator.SetTrigger(TAKEHIT);
        OnTakeHit?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        if (_enemyAi != null)
        {
            _enemyAi.OnEnemyAttack -= EnemyAi_OnEnemyAttack;
        }
        if (_enemyEntity != null)
        {
            _enemyEntity.OntakeHit -= _enemyEntity_OnTakeHit;
            _enemyEntity.OnDeath -= _enemyEntity_OnDeath;
        }
    }

    private void Update()
    {
        if (_isAttacking)
        {
            return;
        }

        if (_enemyAi != null)
        {
            bool isRunning = _enemyAi.IsRunning();
            _animator.SetBool(IS_RUNNING, isRunning);

            if (_hasSpeedMultiplier)
            {
                _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _enemyAi.GetRoamingAnimationSpeed());
            }
        }
    }

    private void EnemyAi_OnEnemyAttack()
    {
        if (_hasAttackTrigger)
        {
            _isAttacking = true;
            _animator.SetTrigger(ATTACK_TRIGGER);
        }
    }

    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
    }

    public void TriggerAttackAnimationTurnOff()
    {
        if (_enemyEntity != null)
        {
            _enemyEntity.PoligonColLiderTurnOff();
        }
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void TriggerAttackAnimationTurnOn()
    {
        if (_enemyEntity != null)
        {
            _enemyEntity.PoligonColiderTurnOn();
        }
    }

    private bool HasParameter(string paramName)
    {
        if (_animator == null) return false;

        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}