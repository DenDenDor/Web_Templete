using UnityEngine;
using UnityEditor;
using System.IO;

public class FolderStructureCreator : EditorWindow
{
    private string moduleName = "ModuleName";

    [MenuItem("Tools/Create Module Structure")]
    public static void ShowWindow()
    {
        GetWindow<FolderStructureCreator>("Create Module Structure");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Module Folder Structure", EditorStyles.boldLabel);
        moduleName = EditorGUILayout.TextField("Module Name", moduleName);

        if (GUILayout.Button("Create Structure"))
        {
            CreateStructure();
        }
    }

    private void CreateStructure()
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            Debug.LogError("Module name cannot be empty!");
            return;
        }

        string basePath = "Assets/" + moduleName;
        
        try
        {
            // Create root folder
            Directory.CreateDirectory(basePath);
            
            // Create subfolders
            Directory.CreateDirectory(Path.Combine(basePath, "Router"));
            Directory.CreateDirectory(Path.Combine(basePath, "Model"));
            Directory.CreateDirectory(Path.Combine(basePath, "View"));

            // Create router script
            CreateRouterScript(moduleName, Path.Combine(basePath, "Router", moduleName + "Router.cs"));
            
            // Create window script
            CreateWindowScript(moduleName, Path.Combine(basePath, "View", moduleName + "Window.cs"));

            AssetDatabase.Refresh();
            Debug.Log($"Module structure for {moduleName} created successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating module structure: {e.Message}");
        }
    }

    private void CreateRouterScript(string moduleName, string filePath)
    {
        string scriptContent = $@"using UnityEngine;

public class {moduleName}Router : IRouter
{{
    public void OnInit()
    {{
       
    }}

    public void OnExit()
    {{
       
    }}
}}";
        File.WriteAllText(filePath, scriptContent);
    }

    private void CreateWindowScript(string moduleName, string filePath)
    {
        string scriptContent = $@"using UnityEngine;

public class {moduleName}Window : AbstractWindowUi
{{
    public override void Init()
    {{
        
    }}
}}";
        File.WriteAllText(filePath, scriptContent);
    }
}