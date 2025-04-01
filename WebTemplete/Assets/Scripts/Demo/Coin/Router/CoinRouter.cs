using UnityEngine;

public class CoinRouter : IRouter
{
    private CoinWindow Window => UiController.Instance.GetWindow<CoinWindow>();
    
    private int Coins => SDKMediator.Instance.GenerateSaveData().Coins;

    public void Init()
    {
        UpdateCoin();

        Window.TakenCoin += OnTakenCoin;
    }

    private void OnTakenCoin()
    {
        SDKMediator.Instance.SaveCoins(Coins + 1);
        UpdateCoin();
    }

    private void UpdateCoin()
    {
        Window.UpdateCoinCount(Coins);
    }

    public void Exit()
    {
        
    }
}