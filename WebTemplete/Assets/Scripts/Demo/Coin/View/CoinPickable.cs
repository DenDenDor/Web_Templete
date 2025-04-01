using System;
using UnityEngine;

public class CoinPickable : MonoBehaviour
{
    public event Action TakenCoin;
    public bool IsPickedUp { get; private set; }

    public void TakeCoin()
    {
        IsPickedUp = true;
        
        TakenCoin?.Invoke();
        
        Destroy(gameObject, 0.3f);
    }
}
