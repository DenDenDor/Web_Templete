#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class DemoRemover : EditorWindow
{
    private const string CONFIG_FILE = "Assets/Resources/DemoRemoverConfig.txt";
    private static bool _alreadyRemoved = false;

    [MenuItem("Tools/Remove Demo Content", false, 100000)] // 1000 - помещает в самый низ меню
    public static void ShowWindow()
    {
        CheckRemovalStatus();
        if (!_alreadyRemoved)
        {
            GetWindow<DemoRemover>("Demo Remover");
        }
    }

    [InitializeOnLoadMethod]
    private static void CheckRemovalStatus()
    {
        if (File.Exists(CONFIG_FILE))
        {
            string content = File.ReadAllText(CONFIG_FILE);
            _alreadyRemoved = content.Contains("removed=true");
        }
        else
        {
            _alreadyRemoved = false;
        }

        // Обновляем состояние меню
        Menu.SetChecked("Tools/Remove Demo Content", _alreadyRemoved);
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove Demo Content", EditorStyles.boldLabel);

        if (GUILayout.Button("Remove Demo"))
        {
            if (EditorUtility.DisplayDialog("Remove Demo Content",
                "This will permanently delete Demo content, Menu/Game scenes, and their references. Continue?",
                "Delete", "Cancel"))
            {
                RemoveDemoContent();
            }
        }
    }

    private void RemoveDemoContent()
    {
        try
        {
            // 1. Удаление папок Demo
            DeleteDirectory("Assets/Scenes/Demo");
            DeleteDirectory("Assets/Scripts/Demo");

            // 2. Удаление сцен Menu и Game
            DeleteScene("Assets/Scenes/Menu.unity");
            DeleteScene("Assets/Scenes/Game.unity");

            // 3. Удаление из Build Settings (SceneList)
            RemoveScenesFromBuildSettings("Menu");
            RemoveScenesFromBuildSettings("Game");

            // 4. Удаление Entry Points
            DeleteFile("Assets/Scripts/Bootstrap/Router/MenuGameEntryPoint.cs");
            DeleteFile("Assets/Scripts/Bootstrap/Router/GameGameEntryPoint.cs");

            // 5. Удаление из ProcessedScenes.txt
            CleanProcessedScenesFile();

            // 6. Сохраняем статус удаления
            SaveRemovalStatus();

            AssetDatabase.Refresh();
            Debug.Log("Demo content removed successfully!");
            _alreadyRemoved = true;
            Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error removing demo content: {e.Message}");
        }
    }

    private static void SaveRemovalStatus()
    {
        string directory = Path.GetDirectoryName(CONFIG_FILE);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(CONFIG_FILE, "removed=true");
        AssetDatabase.ImportAsset(CONFIG_FILE);
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            FileUtil.DeleteFileOrDirectory(path);
            FileUtil.DeleteFileOrDirectory(path + ".meta");
            Debug.Log($"Deleted directory: {path}");
        }
    }

    private static void DeleteScene(string scenePath)
    {
        if (File.Exists(scenePath))
        {
            FileUtil.DeleteFileOrDirectory(scenePath);
            FileUtil.DeleteFileOrDirectory(scenePath + ".meta");
            Debug.Log($"Deleted scene: {scenePath}");
        }
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            FileUtil.DeleteFileOrDirectory(filePath);
            FileUtil.DeleteFileOrDirectory(filePath + ".meta");
            Debug.Log($"Deleted file: {filePath}");
        }
    }

    private static void RemoveScenesFromBuildSettings(string sceneName)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        int removed = scenes.RemoveAll(scene => 
            Path.GetFileNameWithoutExtension(scene.path) == sceneName);
        
        if (removed > 0)
        {
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"Removed {sceneName} from Build Settings");
        }
    }

    private static void CleanProcessedScenesFile()
    {
        string path = "Assets/Resources/ProcessedScenes.txt";
        if (!File.Exists(path)) return;

        var lines = File.ReadAllLines(path)
            .Where(line => !line.Contains("Menu") && !line.Contains("Game"))
            .ToArray();

        File.WriteAllLines(path, lines);
        Debug.Log("Cleaned ProcessedScenes.txt");
    }
}
#endif