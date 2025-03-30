using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSDKAdapter : ScriptableObject
{
  public virtual void Init()
  {
    Debug.Log("Init");
  }

  public abstract bool IsPaymentAvailable { get; }

  public abstract void Save<T>(T data);

  public abstract bool TryLoad<T>(out T data);
  public abstract bool IsMobile { get; }
  public abstract bool IsSpecialFlag { get; }
  public abstract string Language { get; }
  public abstract bool IsAuthorized { get; }

  public abstract void UpdateLeaderboard();

  public abstract void ShowFullscreenAd();

  public abstract bool GetName(out string nickname);
  public abstract bool GetLanguage(string language);
  public abstract void Watch(Action onWatch, RewardType rewardType);
  public abstract void OnStart();
  public abstract void OnEnd();
  public abstract void MakeReview();
  public abstract string GetPrice(string key);

  public abstract IEnumerator GetUserSprite(Action<Sprite> action);
  public abstract void BuyPayment(Action onBuy, string buyKey);
  public abstract void InvokeGRA();
  public abstract void SendMetrica(string metrica);
  public abstract void SendMetrica(string metrica, IDictionary<string, string> eventParams);
}

public enum RewardType
{
}
