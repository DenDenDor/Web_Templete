using UnityEngine;
using UnityEditor;
using System.IO;

public static class SingletonControllerTemplate
{
    private const string Template = 
@"using UnityEngine;
using System;

public class {0} : MonoBehaviour
{{
    private static {0} _instance;

    public static {0} Instance
    {{
        get
        {{
            if (_instance == null)
            {{
                _instance = FindObjectOfType<{0}>();

                if (_instance == null)
                {{
                    throw new NotImplementedException(""{0} not found!"");
                }}
            }}

            return _instance;
        }}
    }}

    private void Awake()
    {{
        if (_instance != null && _instance != this)
        {{
            Destroy(gameObject);
            return;
        }}
        
        _instance = this;
    }}
}}";

    [MenuItem("Assets/Create/Singleton Controller", priority = 0)]
    public static void CreateSingletonController()
    {
        string className = "NewSingletonController"; // Дефолтное имя
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        
        if (string.IsNullOrEmpty(path))
            path = "Assets";
        else if (!Directory.Exists(path))
            path = Path.GetDirectoryName(path);

        // Запрашиваем у пользователя имя класса
        className = EditorInputDialog.Show("Create Singleton Controller", "Enter class name:", "XController");
        if (string.IsNullOrEmpty(className))
            return;

        // Формируем путь к файлу
        string filePath = Path.Combine(path, className + ".cs");
        
        // Заменяем {0} в шаблоне на имя класса
        string finalCode = string.Format(Template, className);
        
        // Создаем файл
        File.WriteAllText(filePath, finalCode);
        AssetDatabase.Refresh();
    }
}

// Вспомогательный класс для ввода имени
public class EditorInputDialog : EditorWindow
{
    private string inputText = "";
    private System.Action<string> onOk;
    
    public static string Show(string title, string label, string defaultText = "")
    {
        EditorInputDialog window = CreateInstance<EditorInputDialog>();
        window.titleContent = new GUIContent(title);
        window.inputText = defaultText;
        window.ShowModal();
        return window.inputText;
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter class name:");
        inputText = EditorGUILayout.TextField(inputText);
        
        if (GUILayout.Button("OK"))
        {
            Close();
        }
    }
}