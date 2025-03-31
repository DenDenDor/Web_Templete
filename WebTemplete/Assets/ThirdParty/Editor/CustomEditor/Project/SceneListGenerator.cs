#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public static class SceneListGenerator
{
    private const string EnumName = "SceneList";
    private const string FilePath = "Assets/Scripts/Bootstrap/Model/SceneList.cs";
    private static string[] _lastScenes;

    static SceneListGenerator()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        string[] currentScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
            .ToArray();

        if (!ScenesChanged(currentScenes))
            return;

        GenerateEnum(currentScenes);
        _lastScenes = currentScenes;
    }

    private static bool ScenesChanged(string[] currentScenes)
    {
        if (_lastScenes == null)
            return true;

        if (currentScenes.Length != _lastScenes.Length)
            return true;

        for (int i = 0; i < currentScenes.Length; i++)
        {
            if (currentScenes[i] != _lastScenes[i])
                return true;
        }

        return false;
    }

    private static void GenerateEnum(string[] scenes)
    {
        string enumCode = $"public enum {EnumName}\n{{\n";

        for (int i = 0; i < scenes.Length; i++)
        {
            string sceneName = scenes[i];
            enumCode += $"    {sceneName} = {i},\n";
        }

        enumCode += "}";

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, enumCode);
        AssetDatabase.Refresh();

        Debug.Log($"SceneList enum updated at {FilePath}");
    }
}
#endif