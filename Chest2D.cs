using UnityEngine;

public class Chest2D : MonoBehaviour
{
    [Header("Настройки добычи")]
    public int minPoints = 0;
    public int maxPoints = 10;

    [Header("Ссылки")]
    public GameObject coinPrefab; 
    public Transform spawnPoint;  
    private Animator anim;
    private bool isOpened = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        Debug.Log("По сундуку попали!");
        if (isOpened) return;
        OpenChest();
    }

    private void OpenChest()
    {
        isOpened = true;
        int pointsAwarded = Random.Range(minPoints, maxPoints + 1);

        if (pointsAwarded > 0)
        {
            if (anim != null) anim.SetTrigger("OpenFull");


            for (int i = 0; i < pointsAwarded; i++)
            {
                SpawnCoin();
            }
        }
        else
        {
            if (anim != null) anim.SetTrigger("OpenEmpty");
        }
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null) return;


        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);


        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 pushDirection = new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 6f));
            rb.AddForce(pushDirection, ForceMode2D.Impulse);
        }
    }
    // Этот метод срабатывает, когда мышка наводится на объект
    private void OnMouseEnter()
    {
        Debug.Log("Мышка коснулась сундука!");
    }

    // Этот метод срабатывает при клике
    private void OnMouseDown()
    {
        Debug.Log("Клик по сундуку засчитан!");
        TakeDamage();
    }

}