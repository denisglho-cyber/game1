using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsDie = Animator.StringToHash("IsDie");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
        }
    }

    private void Update()
    {
        if (Player.Instance == null || !Player.Instance.IsAlive) return;

        _animator.SetBool(IsRunning, Player.Instance.IsRunning());
        _animator.SetBool(IsDashing, Player.Instance.IsDashing);

        AdjustFacingDirection();
    }

    private void AdjustFacingDirection()
    {
        if (GameInput.Instance == null) return;

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        bool shouldFlip = mousePos.x < playerPos.x;

        if (_spriteRenderer.flipX != shouldFlip)
        {
            _spriteRenderer.flipX = shouldFlip;
        }
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(IsDie, true);
        enabled = false;
    }
}