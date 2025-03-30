#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

[InitializeOnLoad]
public static class SDKMediatorGenerator
{
    static SDKMediatorGenerator()
    {
        // Подписываемся на событие изменения скриптов
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded;
    }

    private static void OnAssemblyReloaded()
    {
        GenerateSaveMethods();
    }

    [MenuItem("Tools/Generate Save Methods")]
    public static void GenerateSaveMethods()
    {
        // Находим класс SaveData
        var saveDataType = typeof(SaveData);
        var fields = saveDataType.GetFields()
            .Where(f => f.GetCustomAttribute<AutoGenerateSaveMethodAttribute>() != null)
            .ToList();

        if (!fields.Any()) return;

        // Находим путь к файлу SDKMediator
        var mediatorScriptPath = FindScriptPath(typeof(SDKMediator));
        if (string.IsNullOrEmpty(mediatorScriptPath))
        {
            Debug.LogError("SDKMediator script not found!");
            return;
        }

        // Читаем содержимое файла
        var scriptContent = File.ReadAllText(mediatorScriptPath);
        var builder = new StringBuilder();

        // Генерируем методы для каждого поля
        foreach (var field in fields)
        {
            var methodName = $"Save{field.Name}";
            var methodSignature = $"public void {methodName}(float value)";

            // Проверяем, не существует ли уже такой метод
            if (scriptContent.Contains(methodSignature)) continue;

            builder.AppendLine();
            builder.AppendLine($"    {methodSignature}");
            builder.AppendLine("    {");
            builder.AppendLine("        SaveData defaultSaveData = GenerateSaveData();");
            builder.AppendLine($"        defaultSaveData.{field.Name} = value;");
            builder.AppendLine("        _sdkAdapter.Save(defaultSaveData);");
            builder.AppendLine("    }");
        }

        if (builder.Length == 0) return;

        // Вставляем сгенерированные методы перед последней закрывающей скобкой
        var insertIndex = scriptContent.LastIndexOf('}');
        if (insertIndex == -1) return;

        var newScriptContent = scriptContent.Insert(insertIndex - 1, builder.ToString());

        // Записываем изменения обратно в файл
        File.WriteAllText(mediatorScriptPath, newScriptContent);
        AssetDatabase.Refresh();
    }

    private static string FindScriptPath(System.Type type)
    {
        var guids = AssetDatabase.FindAssets($"t:Script {type.Name}");
        if (guids.Length == 0) return null;
        
        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return path;
    }
}
#endif