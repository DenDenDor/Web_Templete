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
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded;
    }

    private static void OnAssemblyReloaded()
    {
        GenerateSaveMethods();
    }

    [MenuItem("Tools/Generate Save Methods")]
    public static void GenerateSaveMethods()
    {
        var saveDataType = typeof(SaveData);
        var fields = saveDataType.GetFields()
            .Where(f => f.GetCustomAttribute<AutoGenerateSaveMethodAttribute>() != null)
            .ToList();

        if (!fields.Any()) return;

        var mediatorScriptPath = FindScriptPath(typeof(SDKMediator));
        if (string.IsNullOrEmpty(mediatorScriptPath))
        {
            Debug.LogError("SDKMediator script not found!");
            return;
        }

        var scriptContent = File.ReadAllText(mediatorScriptPath);
        var builder = new StringBuilder();

        foreach (var field in fields)
        {
            var methodName = $"Save{field.Name}";
            var methodSignature = $"public void {methodName}({GetTypeName(field.FieldType)} value)";

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

        var insertIndex = scriptContent.LastIndexOf('}');
        if (insertIndex == -1) return;

        var newScriptContent = scriptContent.Insert(insertIndex - 1, builder.ToString());
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

    private static string GetTypeName(System.Type type)
    {
        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(string)) return "string";
        if (type == typeof(double)) return "double";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(char)) return "char";
        if (type == typeof(byte)) return "byte";
        
        // Для других типов возвращаем полное имя
        return type.Name;
    }
}
#endif