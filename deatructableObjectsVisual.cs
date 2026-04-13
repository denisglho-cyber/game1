using UnityEngine;

public class deatructableObjectsVisual : MonoBehaviour
{
    [SerializeField] private deatructableObjects deatructableObjects;
    [SerializeField] private GameObject skullVFXPrefab;
    [SerializeField] private float vfxLifetime = 2f; 

    private void Start()
    {
        if (deatructableObjects != null)
        {
            deatructableObjects.OnDestructableTakeDamage += DestructibleObjects_OnDestructableTakeDamage;
        }
    }

    private void DestructibleObjects_OnDestructableTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        if (skullVFXPrefab != null)
        {

            GameObject vfx = Instantiate(skullVFXPrefab, transform.position, Quaternion.identity);


            Destroy(vfx, vfxLifetime);
        }
    }

    private void OnDestroy()
    {
        if (deatructableObjects != null)
        {
            deatructableObjects.OnDestructableTakeDamage -= DestructibleObjects_OnDestructableTakeDamage;
        }
    }
}