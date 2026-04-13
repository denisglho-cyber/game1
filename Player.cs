using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _minMovingSpeed = 0.1f;
    [SerializeField] private bool _lockYPosition = false;

    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeedMultiplier = 3f;
    [SerializeField] private float _dashCoolDownTime = 0.5f;
    [SerializeField] private float _dashDuration = 0.2f;
    private bool _isDashing = false;
    private Vector2 _dashDirection; // Направление, зафиксированное в начале даша

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 20;
    [SerializeField] private float _damageImmunity = 0.5f;
    [SerializeField] private float _deathDelay = 1.5f;
    [SerializeField] private TrailRenderer _trailRenderer;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;
    private Animator _animator;
    private Camera _mainCamera;

    private Vector2 _currentInputVector;
    private bool _isRunning = false;
    private int _currentHealth;
    private float _damageImmunityTimer;
    private bool _isInvincible = false;

    // Свойства для проверки состояния
    public bool IsGettingKnockedBack => _knockBack != null && _knockBack.IsGettingKnockedBack;
    public bool IsDashing => _isDashing;
    public bool IsAlive => _currentHealth > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;

        if (_rb != null)
        {
            _rb.simulated = true;
            _rb.gravityScale = 0;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;

        if (GameInput.Instance != null)
        {
            // Отписываемся (на всякий случай) и подписываемся
            GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
            GameInput.Instance.OnPlayerDash -= GameInput_OnPlayerDash;

            GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
            GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
            GameInput.Instance.OnPlayerDash -= GameInput_OnPlayerDash;
        }
    }

    private void Update()
    {
        if (!IsAlive) return;

        HandleInvincibility();
        UpdateInput();
        UpdateAnimations();
    }

    private void UpdateInput()
    {
        // Не обновляем ввод движения, если игрока отбрасывает или он в даше (фиксируем направление)
        if (GameInput.Instance != null && !IsGettingKnockedBack && !_isDashing)
        {
            _currentInputVector = GameInput.Instance.GetMovementVector();
        }

        _isRunning = _currentInputVector.magnitude > _minMovingSpeed;
    }

    private void UpdateAnimations()
    {
        if (_animator != null)
        {
            _animator.SetBool("IsRunning", _isRunning);
            _animator.SetBool("IsDashing", _isDashing);
            _animator.SetFloat("Horizontal", _currentInputVector.x);
            _animator.SetFloat("Vertical", _currentInputVector.y);
            _animator.SetFloat("Speed", _isRunning ? 1f : 0f);
        }
    }

    private void FixedUpdate()
    {
        if (!IsAlive || IsGettingKnockedBack) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (_rb == null) return;

        // Если идет даш, используем DashSpeed и DashDirection
        float currentSpeed = _isDashing ? _moveSpeed * _dashSpeedMultiplier : _moveSpeed;
        Vector2 moveDir = _isDashing ? _dashDirection : _currentInputVector;

        if (moveDir.magnitude <= 0) return;

        Vector2 targetPosition = _rb.position + moveDir * (currentSpeed * Time.fixedDeltaTime);

        if (_lockYPosition) targetPosition.y = _rb.position.y;

        _rb.MovePosition(targetPosition);
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_isInvincible || !IsAlive) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        if (_knockBack != null)
            _knockBack.GetKnockBack(damageSource);

        _isInvincible = true;
        _damageImmunityTimer = _damageImmunity;

        OnFlashBlink?.Invoke(this, EventArgs.Empty);

        if (_currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    private void Die()
    {
        if (DeathUIHandler.Instance != null)
            DeathUIHandler.Instance.ShowDeathScreen();

        OnPlayerDeath?.Invoke(this, EventArgs.Empty);

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;
        }

        enabled = false;
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_deathDelay);
        Destroy(gameObject);
    }

    private void GameInput_OnPlayerDash(object sender, EventArgs e)
    {
        if (!_isDashing && IsAlive && !IsGettingKnockedBack)
            StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;

        // Фиксируем направление даша: либо куда жмем, либо куда смотрели (если стоим)
        _dashDirection = _currentInputVector.magnitude > 0 ? _currentInputVector.normalized : Vector2.up;

        if (_trailRenderer != null) _trailRenderer.emitting = true;

        yield return new WaitForSeconds(_dashDuration);

        if (_trailRenderer != null) _trailRenderer.emitting = false;

        yield return new WaitForSeconds(_dashCoolDownTime);
        _isDashing = false;
    }

    private void GameInput_OnPlayerAttack(object sender, EventArgs e)
    {
        if (!IsAlive || IsGettingKnockedBack || _isDashing) return;

        if (ActiveWeapon.Instance != null)
            ActiveWeapon.Instance.Attack();
    }

    private void HandleInvincibility()
    {
        if (_damageImmunityTimer > 0)
        {
            _damageImmunityTimer -= Time.deltaTime;
            if (_damageImmunityTimer <= 0)
                _isInvincible = false;
        }
    }

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
    public float GetHealthPercent() => (float)_currentHealth / _maxHealth;

    public Vector3 GetPlayerScreenPosition()
    {
        if (_mainCamera == null) return Vector3.zero;
        return _mainCamera.WorldToScreenPoint(transform.position);
    }
    public bool IsRunning()
    {
        return _currentInputVector.magnitude > _minMovingSpeed;
    }
}