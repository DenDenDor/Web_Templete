using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinWindow : AbstractWindowUi
{
    [SerializeField] private TextMeshProUGUI _coinCount;
    
    private CoinPickable[] _coinPickables;

    public event Action TakenCoin;
    
    public override void Init()
    {
        _coinPickables = FindObjectsOfType<CoinPickable>();

        foreach (var coin in _coinPickables)
        {
            coin.TakenCoin += OnTakenCoin;
        }
    }

    private void OnTakenCoin()
    {
        TakenCoin?.Invoke();
    }

    public void UpdateCoinCount(int count)
    {
        _coinCount.text = $"Количество монет: {count}";
    }
}