using System;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public event Action<CoinPickable> PickedUpCoin;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out CoinPickable coin))
        {
            PickedUpCoin?.Invoke(coin);
        }
    }
}
