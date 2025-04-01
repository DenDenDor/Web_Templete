using System;
using System.Linq;
using UnityEngine;

public class SDKMediator : MonoBehaviour
{
    private const string ConfigFilePath = "Resources/SDKConfig.txt";
    private static SDKMediator _instance;

    private TypeSDK _currentSDKType;
    private AbstractSDKAdapter _sdkAdapter;

    public static SDKMediator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SDKMediator>();

                if (_instance == null)
                {
                    throw new NotImplementedException("SDK Mediator not found!");
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        LoadSDKSelection();
        InitializeAdapter();

         _sdkAdapter.OnStart();

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _sdkAdapter.OnEnd();
    }

    private void LoadSDKSelection()
    {
        TextAsset configFile = Resources.Load<TextAsset>("SDKConfig");
        if (configFile != null)
        {
            string content = configFile.text;
            if (System.Enum.TryParse(content, out TypeSDK loadedSDK))
            {
                _currentSDKType = loadedSDK;
                Debug.Log($"Loaded SDK type from config: {_currentSDKType}");
            }
        }
        else
        {
            Debug.LogWarning("SDK config file not found. Using default SDK type.");
        }
    }

    private void InitializeAdapter()
    {
        var adapterName = _currentSDKType + "SDKAdapter";
        var adapters = Resources.LoadAll<AbstractSDKAdapter>("SDKAdapters");
        
        _sdkAdapter = adapters.FirstOrDefault(a => a.GetType().Name == adapterName);
        if (_sdkAdapter != null)
        {
            _sdkAdapter.Init();
            Debug.Log($"Initialized {_sdkAdapter.GetType().Name}");
        }
        else
        {
            Debug.LogError($"Adapter {adapterName} not found!");
        }
    }

    public SaveData GenerateSaveData()
    {
        SaveData defaultSaveData = new SaveData();

        if (_sdkAdapter.TryLoad(out SaveData saveData))
        {
            defaultSaveData = saveData;
        }

        return defaultSaveData;
    }

    public void SaveMusicValue(float value)
    {
        SaveData defaultSaveData = GenerateSaveData();
        defaultSaveData.MusicValue = value;
        _sdkAdapter.Save(defaultSaveData);
    }

    public void SaveSoundValue(float value)
    {
        SaveData defaultSaveData = GenerateSaveData();
        defaultSaveData.SoundValue = value;
        _sdkAdapter.Save(defaultSaveData);
    }
    public void SaveCoins(int value)
    {
        SaveData defaultSaveData = GenerateSaveData();
        defaultSaveData.Coins = value;
        _sdkAdapter.Save(defaultSaveData);
    }

}