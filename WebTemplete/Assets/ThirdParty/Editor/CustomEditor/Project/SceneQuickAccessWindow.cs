using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneQuickAccessWindow : EditorWindow
{
    [MenuItem("Tools/Scene Quick Access", priority = -100)]
    public static void ShowWindow()
    {
        GetWindow<SceneQuickAccessWindow>("Scene Quick Access");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);

        // Получаем список сцен из Build Settings
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes in Build Settings!", MessageType.Warning);
            if (GUILayout.Button("Open Build Settings"))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
            return;
        }

        // Кнопки для каждой сцены
        foreach (var scene in scenes)
        {
            if (GUILayout.Button(System.IO.Path.GetFileNameWithoutExtension(scene.path)))
            {
                // Проверяем, нужно ли сохранять текущую сцену
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene.path);
                }
            }
        }
    }
}