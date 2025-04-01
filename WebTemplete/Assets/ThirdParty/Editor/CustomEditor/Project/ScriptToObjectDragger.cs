using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class ScriptToObjectDragger
{
    static ScriptToObjectDragger()
    {
        // Регистрируем обработчики для обоих окон
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        SceneView.duringSceneGui += HandleSceneViewDrag;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        ProcessDragEvent(selectionRect);
    }

    private static void HandleSceneViewDrag(SceneView sceneView)
    {
        // Обрабатываем перетаскивание в пустую область сцены
        if (Event.current.type == EventType.DragExited)
        {
            ProcessDragEvent(new Rect(0, 0, Screen.width, Screen.height), true);
        }
    }

    private static void ProcessDragEvent(Rect dropArea, bool isSceneView = false)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            // Проверяем, находится ли курсор в области сброса
            if (dropArea.Contains(Event.current.mousePosition))
            {
                // Проверяем, есть ли среди перетаскиваемых объектов скрипты
                bool hasScripts = false;
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj is MonoScript)
                    {
                        hasScripts = true;
                        break;
                    }
                }

                if (hasScripts)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        // Создаем объекты для каждого скрипта
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (obj is MonoScript script)
                            {
                                CreateObjectFromScript(script, isSceneView);
                            }
                        }

                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                }
            }
        }
    }

    private static void CreateObjectFromScript(MonoScript script, bool positionInSceneView = false)
    {
        string scriptName = Path.GetFileNameWithoutExtension(script.name);
        GameObject newObject = new GameObject(scriptName);

        // Позиционируем объект в сцене, если перетаскивали в SceneView
        if (positionInSceneView)
        {
            // Получаем позицию под курсором в мировых координатах
            Vector3 dropPosition = GetWorldDropPosition();
            newObject.transform.position = dropPosition;
        }

        System.Type scriptType = script.GetClass();
        if (scriptType != null && scriptType.IsSubclassOf(typeof(MonoBehaviour)))
        {
            newObject.AddComponent(scriptType);
        }

        Selection.activeGameObject = newObject;
        Undo.RegisterCreatedObjectUndo(newObject, "Create " + scriptName);
    }

    private static Vector3 GetWorldDropPosition()
    {
        // Получаем текущее представление сцены
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return Vector3.zero;

        // Преобразуем позицию курсора в мировые координаты
        Vector2 mousePosition = Event.current.mousePosition;
        Camera sceneCamera = sceneView.camera;

        // Создаем луч из камеры
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        
        // Если луч попадает на какой-то объект, используем точку пересечения
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        
        // Иначе размещаем объект на расстоянии 10 единиц от камеры
        return ray.GetPoint(10);
    }
}