using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[InitializeOnLoad]
public class SceneCreationProcessor : UnityEditor.AssetModificationProcessor
{
    private const string ProcessedScenesFile = "Assets/Resources/ProcessedScenes.txt";
    
    static SceneCreationProcessor()
    {
        EditorApplication.delayCall += CheckCurrentScene;
        EnsureProcessedScenesFileExists();
    }

    private static void EnsureProcessedScenesFileExists()
    {
        if (!File.Exists(ProcessedScenesFile))
        {
            string directory = Path.GetDirectoryName(ProcessedScenesFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(ProcessedScenesFile, "");
        }
    }

    private static void AddSceneToProcessedList(string sceneName)
    {
        HashSet<string> processedScenes = GetProcessedScenes();
        if (processedScenes.Contains(sceneName)) return;
        
        processedScenes.Add(sceneName);
        File.WriteAllText(ProcessedScenesFile, string.Join("\n", processedScenes));
    }

    private static HashSet<string> GetProcessedScenes()
    {
        EnsureProcessedScenesFileExists();
        string[] lines = File.ReadAllLines(ProcessedScenesFile);
        return new HashSet<string>(lines);
    }

    private static bool IsSceneProcessed(string sceneName)
    {
        return GetProcessedScenes().Contains(sceneName);
    }

    private static void OnWillCreateAsset(string assetPath)
    {
        if (!assetPath.EndsWith(".unity")) return;
        
        EditorApplication.delayCall += () => ProcessNewScene(assetPath);
    }

    private static void CheckCurrentScene()
    {
        Scene currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.path.StartsWith("Assets/Scenes/"))
        {
            string sceneName = Path.GetFileNameWithoutExtension(currentScene.path);
            
            // Skip if scene is already processed
            if (IsSceneProcessed(sceneName)) return;
            
            VerifyEntryPoint(sceneName, currentScene);
        }
    }

    private static void VerifyEntryPoint(string sceneName, Scene scene)
    {
        GameObject entryPoint = GameObject.Find($"{sceneName}GameEntryPoint");
        if (entryPoint == null) return;
        
        string scriptName = $"{sceneName}GameEntryPoint";
        System.Type type = System.Type.GetType(scriptName + ",Assembly-CSharp");
        
        if (type == null)
        {
            Debug.LogWarning($"Script {scriptName} not found");
            return;
        }

        if (entryPoint.GetComponent(type) == null)
        {
            entryPoint.AddComponent(type);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Added {scriptName} component to existing GameObject");
        }
        
        // Mark scene as processed
        AddSceneToProcessedList(sceneName);
    }

    private static void ProcessNewScene(string scenePath)
    {
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        
        // Skip if scene is already processed
        if (IsSceneProcessed(sceneName)) return;
        
        CreateGameEntryScript(sceneName);
        
        AssetDatabase.Refresh();
        EditorApplication.delayCall += () => AddEntryPointToScene(sceneName, scenePath);
    }

    private static void AddEntryPointToScene(string sceneName, string scenePath)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        GameObject entryPoint = GameObject.Find($"{sceneName}GameEntryPoint");
        if (entryPoint == null)
        {
            entryPoint = new GameObject($"{sceneName}GameEntryPoint");
        }
        
        string scriptName = $"{sceneName}GameEntryPoint";
        System.Type type = System.Type.GetType(scriptName + ",Assembly-CSharp");
        
        if (type == null)
        {
            Debug.LogError($"Failed to find script: {scriptName}");
            return;
        }

        if (entryPoint.GetComponent(type) == null)
        {
            entryPoint.AddComponent(type);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            // Mark scene as processed
            AddSceneToProcessedList(sceneName);
        }
    }

    private static void CreateGameEntryScript(string sceneName)
    {
        string scriptName = $"{sceneName}GameEntryPoint";
        string scriptPath = $"Assets/Scripts/Bootstrap/Router/{scriptName}.cs";
        
        if (File.Exists(scriptPath)) return;
        
        string directory = Path.GetDirectoryName(scriptPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        string scriptContent = $@"using System.Collections.Generic;
using UnityEngine;

public class {scriptName} : AbstractGameEntryPoint
{{
    protected override List<IRouter> Routers {{ get; }}
}}";
        
        File.WriteAllText(scriptPath, scriptContent);
    }
}