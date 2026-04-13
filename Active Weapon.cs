using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Sword currentSword; // Ссылка на ваш скрипт Sword

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        FollowMousePosition();
    }

    private void FollowMousePosition()
    {
        if (mainCamera == null) return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
        Vector3 direction = worldPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip чтобы меч не был вверх ногами
        transform.localScale = (angle > 90 || angle < -90) ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
    }

    public void Attack()
    {
        if (currentSword != null)
        {
            currentSword.Attack(); // Вызываем атаку у меча
        }
    }
}