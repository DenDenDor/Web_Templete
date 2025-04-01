using UnityEngine;

public class PlayerTriggerRouter : IRouter
{
    private PlayerTriggerWindow Window => UiController.Instance.GetWindow<PlayerTriggerWindow>();

    public void Init()
    {
        Window.Trigger.PickedUpCoin += OnPickedUpCoin;
    }

    private void OnPickedUpCoin(CoinPickable coin)
    {
        if (coin.IsPickedUp == false)
        {
            coin.TakeCoin();
        }
    }

    public void Exit()
    {
        Window.Trigger.PickedUpCoin -= OnPickedUpCoin;
    }
}