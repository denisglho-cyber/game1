using System;
using UnityEngine;

public class deatructableObjects : MonoBehaviour
{
    public event EventHandler OnDestructableTakeDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<Sword>())
        {
   
            OnDestructableTakeDamage?.Invoke(this, EventArgs.Empty);

            if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

            if (TryGetComponent<SpriteRenderer>(out var rend)) rend.enabled = false;
            Destroy(gameObject, 0.1f);
        }
    }
}