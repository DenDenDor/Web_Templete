using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " PlayerPrefs", menuName = "SDKAdapter/PlayerPrefs")]
public class PlayerPrefsSDKAdapter : AbstractSDKAdapter
{
    private readonly string _saveKey = "SaveKey";

    public override bool IsPaymentAvailable => true;

    public override void Save<T>(T data)
    {
        if (string.IsNullOrEmpty(_saveKey))
        {
            Debug.LogError("Ключ не может быть пустым.");
            return;
        }

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(_saveKey, json);
        
        PlayerPrefs.Save();
    }

    public override bool TryLoad<T>(out T data)
    {
        
        if (string.IsNullOrEmpty(_saveKey))
        {
            Debug.LogError("Ключ не может быть пустым.");
            data = default;
            return false;
        }

        string json = PlayerPrefs.GetString(_saveKey, null);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"Данные с ключом '{_saveKey}' не найдены.");
            data = default;
            return false;
        }

        data = JsonUtility.FromJson<T>(json);
        
        return true;
    }

    public override bool IsMobile => false;
    public override bool IsSpecialFlag => true;
    public override string Language => Application.systemLanguage.ToString();
    public override bool IsAuthorized => false;

    public override void SendMetrica(string metrica)
    {
    }

    public override void SendMetrica(string metrica, IDictionary<string, string> eventParams)
    {
        
    }

    public override void UpdateLeaderboard()
    {
        
    }

    public override void ShowFullscreenAd()
    {
    }

    public override bool GetName(out string nickname)
    {
        nickname = "";

        return false;
    }

    public override bool GetLanguage(string language)
    {
        return language == Language;
    }

    public override void Watch(Action onWatch, RewardType rewardType)
    {
        onWatch?.Invoke();
    }

    public override void OnStart()
    {
        
    }

    public override void OnEnd()
    {
        
    }

    public override void MakeReview()
    {
        
    }

    public override string GetPrice(string key)
    {
        return "15 YaN";
    }

    public override IEnumerator GetUserSprite(Action<Sprite> action)
    {
        using (WWW www = new WWW(""))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = new Texture2D(2, 2);
                www.LoadImageIntoTexture(texture);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                action(sprite);
            }
            else
            {
                Debug.LogError("Ошибка загрузки изображения: " + www.error);
            }
        }
    }

    public override void BuyPayment(Action onBuy, string buyKey)
    {
        onBuy?.Invoke();
    }

    public override void InvokeGRA()
    {
        
    }
}
