using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameEntryPoint : MonoBehaviour
{
    protected abstract List<IRouter> Routers { get; }

    private void Start()
    {
        InitWindows();
        InitRouter();
    }

    private void OnDisable()
    {
        ExitRouter();
    }

    private void InitWindows()
    {
        IEnumerable<AbstractWindowUi> windows = FindObjectsOfType<AbstractWindowUi>(true);
        
        GameObject uiControllerObj = new GameObject("UiController");
        UiController controller = uiControllerObj.AddComponent<UiController>();
        controller.RegisterAllWindows(windows);

        foreach (var window in windows)
        {
            window.Init();
        }
    
        UiController.Instance.RegisterAllWindows(windows);
    }

    private void InitRouter()
    {
        foreach (var router in Routers)
        {
            router.OnInit();
        }
    }

    private void ExitRouter()
    {
        foreach (var router in Routers)
        {
            router.OnExit();
        }
    }
}
