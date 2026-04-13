using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAi))]
public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO _enemySO;
    public event EventHandler OntakeHit;
    public event Action OnEnemyAttack;
    public event EventHandler OnDeath;

    private int _currentHealth;
    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2d;
    private EnemyAi _enemyAi;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2d = GetComponent<BoxCollider2D>();
        _enemyAi = GetComponent<EnemyAi>();
    }

    private void Start()
    {
        _currentHealth = _enemySO.enemyHealth;
        if (_polygonCollider2D != null) _polygonCollider2D.enabled = false;
    }

    private void OntriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemy))
        {
            enemy.TakeDamage(10);
            Destroy(gameObject);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player))
        {
            player.TakeDamage(transform, _enemySO.enemyDamageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        OntakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    public void Attack()
    {
        OnEnemyAttack?.Invoke();
    }

    public void PoligonColLiderTurnOff()
    {
        if (_polygonCollider2D != null) _polygonCollider2D.enabled = false;
    }

    public void PoligonColiderTurnOn()
    {
        if (_polygonCollider2D != null) _polygonCollider2D.enabled = true;
    }

    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            if (_boxCollider2d != null) _boxCollider2d.enabled = false;
            if (_polygonCollider2D != null) _polygonCollider2D.enabled = false;

            if (_enemyAi != null)
            {
                _enemyAi.SetDeathState();
            }

            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}