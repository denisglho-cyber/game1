using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float _knockBackForce;
    [SerializeField] private float _knockBackMovingTimerMax;

    private float _knockBackMovingTimer;
    private Rigidbody2D rb;

    public bool IsGettingKnockedBack { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsGettingKnockedBack)
        {
            _knockBackMovingTimer -= Time.deltaTime;
            if (_knockBackMovingTimer <= 0)
            {
                StopKnockBackMovement();
            }
        }
    }

    public void GetKnockBack(Transform damageSours)
    {
        IsGettingKnockedBack = true;
        _knockBackMovingTimer = _knockBackMovingTimerMax;

        Vector2 difference = (transform.position - damageSours.position).normalized * _knockBackForce / rb.mass;

        Debug.Log("Отбрасываю с силой: " + difference + " Направление от: " + damageSours.name);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(difference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement()
    {
        if (!IsGettingKnockedBack) return;

        rb.linearVelocity = Vector2.zero;
        IsGettingKnockedBack = false;
    }
}