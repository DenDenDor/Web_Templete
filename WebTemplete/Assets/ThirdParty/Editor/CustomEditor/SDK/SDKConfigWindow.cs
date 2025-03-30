#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SDKConfigWindow : EditorWindow
{
    private const string ConfigFilePath = "Assets/Resources/SDKConfig.txt";
    private const string AdaptersFolder = "Assets/Resources/SDKAdapters";
    private TypeSDK selectedSDK;
    
    [MenuItem("Tools/SDK Configuration")]
    public static void ShowWindow()
    {
        GetWindow<SDKConfigWindow>("SDK Config").InitializeWindow();
    }
    
    private void InitializeWindow()
    {
        CreateMissingAdapters();
        LoadCurrentSelection();
    }
    
    private void CreateMissingAdapters()
    {
        // Получаем все типы адаптеров
        var adapterTypes = TypeCache.GetTypesDerivedFrom<AbstractSDKAdapter>()
            .Where(t => !t.IsAbstract && !t.IsGenericType)
            .ToList();
        
        // Создаем папку если не существует
        if (!Directory.Exists(AdaptersFolder))
        {
            Directory.CreateDirectory(AdaptersFolder);
        }
        
        bool createdAny = false;
        
        foreach (var type in adapterTypes)
        {
            string assetPath = $"{AdaptersFolder}/{type.Name}.asset";
            
            if (!File.Exists(assetPath))
            {
                var instance = CreateInstance(type);
                AssetDatabase.CreateAsset(instance, assetPath);
                createdAny = true;
                
                Debug.Log($"Created missing adapter: {type.Name} at {assetPath}");
            }
        }
        
        if (createdAny)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("SDK Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        selectedSDK = (TypeSDK)EditorGUILayout.EnumPopup("Active SDK Type:", selectedSDK);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Save Configuration", GUILayout.Height(30)))
        {
            SaveSelection();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("All missing adapter assets have been automatically created in Resources/SDKAdapters folder.", MessageType.Info);
    }
    
    private void LoadCurrentSelection()
    {
        if (File.Exists(ConfigFilePath))
        {
            string content = File.ReadAllText(ConfigFilePath);
            if (System.Enum.TryParse(content, out TypeSDK loadedSDK))
            {
                selectedSDK = loadedSDK;
            }
        }
        else
        {
            // Устанавливаем первое значение enum по умолчанию, если файла нет
            var values = System.Enum.GetValues(typeof(TypeSDK));
            if (values.Length > 0)
            {
                selectedSDK = (TypeSDK)values.GetValue(0);
            }
        }
    }
    
    private void SaveSelection()
    {
        // Проверяем существует ли asset для выбранного типа
        string adapterName = selectedSDK + "SDKAdapter";
        string assetPath = $"{AdaptersFolder}/{adapterName}.asset";
        
        if (!File.Exists(assetPath))
        {
            EditorUtility.DisplayDialog("Error", 
                $"Adapter asset for {selectedSDK} not found! Please check Resources/SDKAdapters folder.", 
                "OK");
            return;
        }
        
        // Сохраняем выбор
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }
        
        File.WriteAllText(ConfigFilePath, selectedSDK.ToString());
        AssetDatabase.Refresh();
        
        Debug.Log($"SDK selection saved: {selectedSDK}");
        EditorUtility.DisplayDialog("Success", 
            $"Active SDK set to {selectedSDK}", 
            "OK");
    }
}
#endif