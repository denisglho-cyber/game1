using System;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public event Action OnSwordSwing;

    public void Attack()
    {
        OnSwordSwing?.Invoke();
    }
}