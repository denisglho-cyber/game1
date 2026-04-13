using UnityEngine;

public class SwordVisual : MonoBehaviour
{
    private Animator animator;
    private PolygonCollider2D polygonCollider; // Ссылка на коллайдер

    [SerializeField] private Sword sword;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // Получаем ссылку на коллайдер на этом же объекте
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // --- ЭТИ ФУНКЦИИ ВЫЗОВЕТ АНИМАЦИЯ ---

    public void EnableAttackCollider()
    {
        polygonCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        polygonCollider.enabled = false;
    }

    // ------------------------------------

    private void Start()
    {
        if (sword != null) sword.OnSwordSwing += Sword_OnSwordSwing;
        // На всякий случай выключаем при старте
        DisableAttackCollider();
    }

    private void Sword_OnSwordSwing()
    {
        animator.SetTrigger("Attack");
    }
}